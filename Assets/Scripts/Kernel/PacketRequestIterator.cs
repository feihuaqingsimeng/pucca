using Common.Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketRequestIterator : Singleton<PacketRequestIterator>
{
    // Improvement
    // 1. REQ 송신 후, SYN 수신 여부를 판단할 수 있도록 개선해야 합니다.
    // 2. RepetitiveRequest()
    // 3. PacketRequestRepeator 외부에서 송신한 경우, PacketRequestInfo.deltaTime을 갱신해야 합니다.

    class PacketRequestInfo
    {
        public Type packetType
        {
            get;
            set;
        }

        public float repeatRate
        {
            get;
            set;
        }

        public float deltaTime
        {
            get;
            set;
        }

        public Generator generator;

        public PACKET_BASE packetBase
        {
            get
            {
                if (generator != null)
                {
                    return generator();
                }
                else
                {
                    return Activator.CreateInstance(packetType, false) as PACKET_BASE;
                }
            }
        }
    }

    List<PacketRequestInfo> m_PacketRequestInfos = new List<PacketRequestInfo>();

    public delegate PACKET_BASE Generator();

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        StartCoroutine(RepetitiveRequest());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator RepetitiveRequest()
    {
        while (true)
        {
            for (int i = 0; i < m_PacketRequestInfos.Count; i++)
            {
                PacketRequestInfo packetRequestInfo = m_PacketRequestInfos[i];
                if (packetRequestInfo != null)
                {
                    packetRequestInfo.deltaTime = packetRequestInfo.deltaTime + Time.fixedDeltaTime;
                    if (packetRequestInfo.deltaTime > packetRequestInfo.repeatRate)
                    {
                        PACKET_BASE packetBase = packetRequestInfo.packetBase;
                        if (packetBase != null)
                        {
                            Kernel.networkManager.WebRequest(packetBase, false);
                        }

                        packetRequestInfo.deltaTime = 0f;
                    }
                }
            }

            yield return 0;
        }
    }

    public bool AddPacketRequestInfo<T>(float repeatRate, Generator generator = null) where T : PACKET_BASE, new()
    {
        for (int i = 0; i < m_PacketRequestInfos.Count; i++)
        {
            PacketRequestInfo packetRequestInfo = m_PacketRequestInfos[i];
            if (packetRequestInfo != null)
            {
                if (Type.Equals(packetRequestInfo.packetType, typeof(T)))
                {
                    return false;
                }
            }
        }

        m_PacketRequestInfos.Add(new PacketRequestInfo()
            {
                packetType = typeof(T),
                repeatRate = repeatRate,
                generator = generator,
            });

        return true;
    }

    public bool RemovePacketRequestInfo<T>() where T : PACKET_BASE, new()
    {
        for (int i = 0; i < m_PacketRequestInfos.Count; i++)
        {
            PacketRequestInfo packetRequestInfo = m_PacketRequestInfos[i];
            if (packetRequestInfo != null)
            {
                if (Type.Equals(packetRequestInfo.packetType, typeof(T)))
                {
                    m_PacketRequestInfos.RemoveAt(i);

                    return true;
                }
            }
        }

        return false;
    }
}
