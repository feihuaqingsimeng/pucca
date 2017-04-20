using Common.Util;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AdministratorWindow : EditorWindow
{
    eGoodsType m_GoodsType;
    int m_Amount;
    int m_BoxIndex;
    int m_BoxArea;
    int m_TimeScale;
    Dictionary<string, int> m_CardIndexDictionary = new Dictionary<string, int>();
    List<string> m_CardNameList = new List<string>();
    int m_CardIndex;
    Dictionary<string, int> m_BoxIndexDictionary = new Dictionary<string, int>();
    List<string> m_BoxNameList = new List<string>();
    int m_Level;
    long m_AID;
    bool m_IsDaily;
    int m_AchieveIndex;
    int m_AchieveAccumulate;
    Dictionary<string, int> m_SoulIndexDictionary = new Dictionary<string, int>();
    List<string> m_SoulNameList = new List<string>();
    int m_SoulIndex;
    int m_SoulCount;

    [MenuItem("UI Utility/Administrator Window")]
    static void Init()
    {
        EditorWindow.GetWindow<AdministratorWindow>();
    }

    void OnEnable()
    {
        minSize = new Vector2(256f, 256f);
        m_TimeScale = 1;
    }

    void OnGUI()
    {
        #region
        if (Kernel.entry != null &&
            Kernel.dataLoader != null &&
            Kernel.dataLoader.isLoadComplete &&
            int.Equals(m_CardIndexDictionary.Count, 0))
        {
            m_CardIndexDictionary.Clear();
            m_CardNameList.Clear();
            m_BoxIndexDictionary.Clear();
            m_BoxNameList.Clear();
            m_SoulIndexDictionary.Clear();
            m_SoulNameList.Clear();

            m_CardIndexDictionary.Add("All", 0);
            m_CardNameList.Add("All");
            foreach (var item in DB_Card.instance.schemaList)
            {
                DBStr_Character.Schema strCharacter = DBStr_Character.Query(DBStr_Character.Field.Char_Index, item.Index);
                string charName = (strCharacter != null) ? strCharacter.StringData : item.IdentificationName;

                if (!m_CardIndexDictionary.ContainsKey(charName))
                {
                    m_CardIndexDictionary.Add(charName, item.Index);
                    m_CardNameList.Add(charName);
                }
                else Debug.LogErrorFormat("{0} ({1})", charName, item.Index);

                if (!m_SoulIndexDictionary.ContainsKey(charName))
                {
                    m_SoulIndexDictionary.Add(charName, item.Index);
                    m_SoulNameList.Add(charName);
                }
                else Debug.LogErrorFormat("{0} ({1})", charName, item.Index);
            }

            foreach (var item in DB_BoxGet.instance.schemaList)
            {
                string boxName = Languages.ToString(item.TEXT_UI);

                if (!m_BoxIndexDictionary.ContainsKey(boxName))
                {
                    m_BoxIndexDictionary.Add(boxName, item.Index);
                    m_BoxNameList.Add(boxName);
                }
                else Debug.LogErrorFormat("{0} ({1})", boxName, item.Index);
            }
        }
        else
        {
            if (!Application.isPlaying)
            {
                m_CardIndexDictionary.Clear();
                m_CardNameList.Clear();
                m_BoxIndexDictionary.Clear();
                m_BoxNameList.Clear();
            }
        }
        #endregion

        GUILayout.Space(10f);

        #region 레벨
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("level");//level
        GUILayout.Space(10f);
        m_Level = EditorGUILayout.IntField(m_Level);
        GUILayout.Space(10f);
        if (GUILayout.Button("request"))//request
        {
            if (Kernel.entry != null)
            {
                Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_ACCOUNT_LEVEL_SYN((byte)m_Level);
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region 재화
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Goods");
        GUILayout.Space(10f);
        m_GoodsType = (eGoodsType)EditorGUILayout.EnumPopup(m_GoodsType);
        GUILayout.Space(10f);
        m_Amount = EditorGUILayout.IntField(m_Amount);
        GUILayout.Space(10f);
        if (GUILayout.Button("Request"))
        {
            if (Kernel.entry != null && Kernel.dataLoader != null && Kernel.dataLoader.isLoadComplete)
            {
                Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_GOODS_SYN(m_GoodsType, m_Amount);
            }
        }
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region 상자
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Box");
        GUILayout.Space(10f);
        m_BoxIndex = EditorGUILayout.Popup(m_BoxIndex, m_BoxNameList.ToArray());
        GUILayout.Space(10f);
        GUILayout.Label("Area");
        GUILayout.Space(10f);
        m_BoxArea = EditorGUILayout.IntField(m_BoxArea);
        GUILayout.Space(10f);
        if (GUILayout.Button("Request"))
        {
            if (m_BoxIndexDictionary.ContainsKey(m_BoxNameList[m_BoxIndex]))
            {
                if (Kernel.entry != null && Kernel.dataLoader != null && Kernel.dataLoader.isLoadComplete)
                {
                    Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_REWARD_BOX_SYN(m_BoxIndexDictionary[m_BoxNameList[m_BoxIndex]], (byte)m_BoxArea);
                }
            }
        }
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region 카드
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Card");
        m_CardIndex = EditorGUILayout.Popup(m_CardIndex, m_CardNameList.ToArray());
        GUILayout.Space(10f);
        if (GUILayout.Button("Request"))
        {
            if (m_CardIndexDictionary.ContainsKey(m_CardNameList[m_CardIndex]))
            {
                if (Kernel.entry != null && Kernel.dataLoader != null && Kernel.dataLoader.isLoadComplete)
                {
                    Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_CARD_SYN(m_CardIndexDictionary[m_CardNameList[m_CardIndex]]);
                }
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region 소울
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Soul");
        m_SoulIndex = EditorGUILayout.Popup(m_SoulIndex, m_SoulNameList.ToArray());
        GUILayout.Space(10f);
        m_SoulCount = EditorGUILayout.IntField(m_SoulCount);
        GUILayout.Space(10f);
        if (GUILayout.Button("Request"))
        {
            if (m_SoulIndexDictionary.ContainsKey(m_SoulNameList[m_SoulIndex]))
            {
                if (Kernel.entry != null && Kernel.dataLoader != null && Kernel.dataLoader.isLoadComplete)
                {
                    Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_SOUL_SYN(m_SoulIndexDictionary[m_SoulNameList[ m_SoulIndex]], m_SoulCount);
                }
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region Time Scale
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Time Scale");
        GUILayout.Space(10f);
        m_TimeScale = EditorGUILayout.IntField(m_TimeScale);
        if (m_TimeScale != Time.timeScale)
        {
            Time.timeScale = m_TimeScale;
        }
        GUILayout.Space(10f);
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region 업적
        GUILayout.BeginHorizontal();
        GUILayout.Label("Daily achievements");
        GUILayout.Space(10f);
        m_IsDaily = EditorGUILayout.Toggle(m_IsDaily);
        GUILayout.Space(10f);
        GUILayout.Label("Number");
        GUILayout.Space(10f);
        m_AchieveIndex = EditorGUILayout.IntField(m_AchieveIndex);
        GUILayout.Space(10f);
        GUILayout.Label("Quantity");
        GUILayout.Space(10f);
        m_AchieveAccumulate = EditorGUILayout.IntField(m_AchieveAccumulate);
        GUILayout.Space(10f);
        if (GUILayout.Button("Request"))
        {
            if (Kernel.entry != null)
            {
                Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_ACHIEVE_SYN(m_IsDaily, m_AchieveIndex, m_AchieveAccumulate);
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region Delete All PlayerPrefs
        if (GUILayout.Button("Delete All PlayerPrefs", GUILayout.Width(128f)))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        #endregion

        GUILayout.Space(10f);

        #region Delete Account
        if (Kernel.entry != null &&
            Kernel.entry.account.initialized &&
            m_AID == 0)
        {
            m_AID = (int)Kernel.entry.account.userNo;
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("AID");
        GUILayout.Space(10f);
        m_AID = EditorGUILayout.LongField(m_AID);
        GUILayout.Space(10f);
        if (GUILayout.Button("Delete Account", GUILayout.Width(128f)))
        {
            Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_SYN(m_AID);
        }
        GUILayout.EndHorizontal();
        #endregion
    }
}
