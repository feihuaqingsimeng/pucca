using Common.Packet;
using Common.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum GAME_SERVER_TYPE
{
    TYPE_BUILD_MODE,    // 자동
    TYPE_DEV,           // 개발 서버
    TYPE_QA,            // QA용.
    TYPE_ADHOC,         // 외부배포용.
    TYPE_RELEASE,       // 상용.
    TYPE_MANUAL // 수동
}


public class NetworkManager : Singleton<NetworkManager>
{
    //네트워크 - 메인 클래스.

    //개발서버 IP, Port
    string DEV_IP = "106.15.60.190";
    int DEV_PORT = 21000;

    //QA(QA용)서버 IP, Port
    string QA_IP = "1.255.56.172";
    int QA_PORT = 21000;

    //ADHOC(외부배포용)서버 IP, Port
    string ADHOC_IP = "14.63.198.210";
    int ADHOC_PORT = 21000;

    //RELEASE(상용)서버 IP, Port
//    string RELEASE_IP = "14.49.41.204";
//    int RELEASE_PORT = 21000;
	string RELEASE_IP = "106.15.60.190";
    int RELEASE_PORT = 21000;


    //통신용 데이터.
    string CURRUNT_IP;
    int CURRUNT_PORT;

    string ConnectURL;

    Queue<PacketInfo> m_PacketInfoQueue = new Queue<PacketInfo>();
    int m_PacketSequence;
    float m_Timeout = 30f;
    bool m_Busy;
    int m_RetryCount;
    bool m_RequestEnable = true;

    public delegate void OnPacketSend(ePACKET_CATEGORY packetCategory, byte packetIndex);
    public OnPacketSend onPacketSend;

    public delegate void OnPacketReceive(PACKET_BASE packetBase);
    public OnPacketReceive onPacketReceive;

    public delegate void OnException(Result_Define.eResult result, string error, ePACKET_CATEGORY packetCategory, byte packetIndex);
    public OnException onException;

    protected override void Awake()
    {
        base.Awake();

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ko-KR");

            // iOS 버전에서 .Net Reflector 의 Serialization Exception 처리. (추가 2017.01.23 dotsoft)
            // Forces a different code path in the BinaryFormatter that doesn't rely on run-time code generation (which would break on iOS).
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
        }

        ChangeGameServer_IP();
    }

    void ChangeGameServer_IP()
    {
        switch (Kernel.gameServerType)
        {
            case GAME_SERVER_TYPE.TYPE_BUILD_MODE:
#if UNITY_EDITOR
                CURRUNT_IP = DEV_IP;
                CURRUNT_PORT = DEV_PORT;
#else
                CURRUNT_IP = RELEASE_IP;
                CURRUNT_PORT = RELEASE_PORT;
#endif
                break;

            case GAME_SERVER_TYPE.TYPE_DEV:
                CURRUNT_IP = DEV_IP;
                CURRUNT_PORT = DEV_PORT;
                break;

            case GAME_SERVER_TYPE.TYPE_QA:
                CURRUNT_IP = QA_IP;
                CURRUNT_PORT = QA_PORT;
                break;

            case GAME_SERVER_TYPE.TYPE_ADHOC:
                CURRUNT_IP = ADHOC_IP;
                CURRUNT_PORT = ADHOC_PORT;
                break;

            case GAME_SERVER_TYPE.TYPE_RELEASE:
                CURRUNT_IP = RELEASE_IP;
                CURRUNT_PORT = RELEASE_PORT;
                break;

            case GAME_SERVER_TYPE.TYPE_MANUAL:
                CURRUNT_IP = Kernel.URL_COMMON;
                CURRUNT_PORT = Kernel.PORT_COMMON;
                break;

        }

        ConnectURL = string.Format("http://{0}:{1}/Main.aspx", CURRUNT_IP, CURRUNT_PORT);
        Debug.Log("Connect Server : " + Kernel.gameServerType.ToString() + "-" + ConnectURL);

    }

    public void WebRequest(PACKET_BASE packetBase, bool indication = true)
    {
        if (!m_RequestEnable)
        {
            return;
        }

        if (packetBase != null)
        {
            packetBase.m_AID = Kernel.entry.account.userNo;
            packetBase.m_iDBIndex = Kernel.entry.account.dbNo;
            packetBase.m_iAuthKey = Kernel.entry.account.AuthKey;
            packetBase.m_iPacketSeq = m_PacketSequence++;

            m_PacketInfoQueue.Enqueue(new PacketInfo(packetBase, indication));

            if (!m_Busy && gameObject != null)
            {
                StartCoroutine(WebRequestByCoroutine());
            }
        }
    }

    IEnumerator WebRequestByCoroutine()
    {
        if (Equals(m_PacketInfoQueue, null) || int.Equals(m_PacketInfoQueue.Count, 0))
        {
            yield break;
        }
        else if (m_Busy)
        {
            yield break;
        }

        m_Busy = true;

        bool deserializeException = false;
        string error = string.Empty;
        PacketInfo requestPacketInfo = m_PacketInfoQueue.Peek();
        byte[] bytes = CEncrypter.Serialize(requestPacketInfo.packetBase);
        using (WWW www = new WWW(ConnectURL, bytes))
        {
            if (Kernel.packetLogEnable)
            {
                Debug.Log(string.Format("[REQ] {0} (Category : {1}, Index :{2}, Size : {3})", requestPacketInfo.packetBase.GetType(), requestPacketInfo.packetBase.Category, requestPacketInfo.packetBase.m_PacketIndex, bytes.Length));
            }

            if (onPacketSend != null)
            {
                onPacketSend(requestPacketInfo.packetBase.m_Category, requestPacketInfo.packetBase.m_PacketIndex);
            }

            if (requestPacketInfo.indication)
            {
                Kernel.uiManager.Open(UI.Indicator);
            }

            float deltaTime = 0f;
            while (!www.isDone)
            {
                if (deltaTime > m_Timeout)
                {
                    break;
                }

                deltaTime = deltaTime + Time.deltaTime;

                yield return null;
            }

            if (requestPacketInfo.indication)
            {
                Kernel.uiManager.Close(UI.Indicator);
            }

            error = www.isDone ? www.error : "TIMEOUT";
            Result_Define.eResult result = Result_Define.eResult.SUCCESS;
            PACKET_BASE ackPacketBase = null;
            if (string.IsNullOrEmpty(error) && www.isDone)
            {
                if (www.bytes != null && www.bytes.Length > 0)
                {
                    try
                    {
                        string base64 = Encoding.Default.GetString(www.bytes);
                        bytes = Convert.FromBase64String(base64);
                        bytes = CEncrypter.Decrypt(bytes);
                        ackPacketBase = CEncrypter.Deserialize<PACKET_BASE>(bytes, false, true);
                    }
                    catch (Exception e)
                    {
                        error = e.ToString();
                        deserializeException = true;

                        Debug.LogFormat("[LOG] www.bytes : {0}", e.ToString());
                    }

                    if (!deserializeException && ackPacketBase != null)
                    {
                        if (Kernel.packetLogEnable)
                        {
                            Debug.Log(string.Format("[ACK] {0} (Category : {1}, Index : {2}, Size : {3})", ackPacketBase.GetType(), ackPacketBase.m_Category, ackPacketBase.m_PacketIndex, bytes.Length));
                        }

                        if (onPacketReceive != null)
                        {
                            onPacketReceive(ackPacketBase);
                        }

                        Kernel.entry.packetBroadcaster.Broadcast(ackPacketBase, ref result);
                    }
                }
            }

            if (!deserializeException)
            {
                if (!string.IsNullOrEmpty(error) || result != Result_Define.eResult.SUCCESS)
                {
                    Debug.Log("[LOG] result != Result_Define.eResult.SUCCESS");

                    Debug.LogError(string.Format("[LOG] [{0}] error : {1}, result : {2}",
                                             (ackPacketBase != null) ? ackPacketBase.GetType() : requestPacketInfo.packetBase.GetType(),
                                             error,
                                             result));

                    if (onException != null)
                    {
                        ePACKET_CATEGORY category;
                        byte index;
                        if (string.IsNullOrEmpty(error))
                        {
                            category = ackPacketBase.m_Category;
                            index = ackPacketBase.m_PacketIndex;
                        }
                        else
                        {
                            category = requestPacketInfo.packetBase.m_Category;
                            index = requestPacketInfo.packetBase.m_PacketIndex;
                        }

                        onException(result, error, category, index);
                    }
                }
            }
        }

        if (deserializeException)
        {
            Debug.LogFormat("[LOG] deserializeException : ", deserializeException);

            m_RetryCount++;

            if (m_RetryCount > 2)
            {
                m_RequestEnable = false;

                if (onException != null)
                {
                    onException(0, error, 0, 0);
                }
            }
        }
        else
        {
            m_RetryCount = 0;
            m_PacketInfoQueue.Dequeue();
        }

        m_Busy = false;

        if (m_RequestEnable && m_RetryCount < 3 && m_PacketInfoQueue != null && m_PacketInfoQueue.Count > 0)
        {
            StartCoroutine(WebRequestByCoroutine());
        }
    }
}
