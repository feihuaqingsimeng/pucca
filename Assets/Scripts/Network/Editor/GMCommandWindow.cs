using Common.Util;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class GMCommandWindow : EditorWindow
{
    struct BoxItem
    {
        public int id;
        public string name;
    }

    ePostType m_PostType;
    eGoodsType m_GoodsType;
    int m_GoodsAmount;
    int m_CardIndex;
    int m_BoxIndex;
    int m_BoxAmount;
    string m_Title;
    string m_Message;

    int[] m_CardIndexList;
    string[] m_CardNameList;

    int[] m_BoxIndexList;
    string[] m_BoxNameList;

    [MenuItem("UI Utility/GM Command")]
    static void Init()
    {
        EditorWindow.GetWindow<GMCommandWindow>();
    }

    void OnEnable()
    {
        // m_Title = PlayerPrefs.GetString("gm.command.title", string.Empty);
        // m_Message = PlayerPrefs.GetString("gm_command.message", string.Empty);
    }

    void OnDisable()
    {
        // PlayerPrefs.SetString("gm.command.title", m_Title);
        // PlayerPrefs.SetString("gm_command.message", m_Message);
    }

    void InitCard()
    {
        if (m_CardIndexList != null && m_CardNameList != null)
            return;

        List<int> indexList = new List<int>();
        List<string> nameList = new List<string>();

        foreach (var item in DB_Card.instance.schemaList)
        {
            DBStr_Character.Schema strCharacter = DBStr_Character.Query(DBStr_Character.Field.Char_Index, item.Index);
            if (strCharacter != null)
            {
                string charName = (strCharacter != null) ? strCharacter.StringData : item.IdentificationName;

                indexList.Add(item.Index);
                nameList.Add(charName);
            }
        }

        m_CardIndexList = indexList.ToArray();
        m_CardNameList = nameList.ToArray();
    }

    void InitBox()
    {
        if (m_BoxNameList != null && m_BoxIndexList != null)
            return;

        List<int> indexList = new List<int>();
        List<string> nameList = new List<string>();

        foreach (var item in DB_Package_BoxGet.instance.schemaList)
        {
            string boxName = Languages.ToString(item.TEXT_UI);
            indexList.Add(item.Index);
            nameList.Add(boxName);
        }

        m_BoxIndexList = indexList.ToArray();
        m_BoxNameList = nameList.ToArray();
    }

    void OnGUI()
    {
        if (Kernel.entry == null || Kernel.dataLoader == null || !Kernel.dataLoader.isLoadComplete)
        {
            GUILayout.Label("GM Command 를 사용하시려면 게임에 로그인 되어 있어야 합니다.");
            m_BoxIndexList = null;
            m_BoxNameList = null;
            m_CardIndexList = null;
            m_CardNameList = null;
            m_Title = string.Empty;
            m_Message = string.Empty;
            return;
        }
        else
        {
            if (m_CardIndexList == null || m_CardNameList == null)
                InitCard();

            if (m_BoxIndexList == null || m_BoxNameList == null)
                InitBox();
        }

        GUILayout.Label("[유저 일괄 보상 시스템]");
        GUILayout.Space(10f);

        GUILayout.BeginVertical();
        GUILayout.Label("우편함 제목");
        m_Title = EditorGUILayout.TextField(m_Title);
        GUILayout.Label("우편함 메시지");
        m_Message = EditorGUILayout.TextField(m_Message);
        GUILayout.Label("우편함 상품타입");
        m_PostType = (ePostType)EditorGUILayout.EnumPopup(m_PostType);

        if (m_PostType == ePostType.Goods)
        {
            GUILayout.Label("보상 종류");
            m_GoodsType = (eGoodsType)EditorGUILayout.EnumPopup(m_GoodsType);
            GUILayout.Label("보상 개수");
            m_GoodsAmount = EditorGUILayout.IntField(m_GoodsAmount);
        }
        else if (m_PostType == ePostType.Card)
        {
            GUILayout.Space(10f);
            GUILayout.Label("카드는 지급할 수 없습니다.");
            /*
            GUILayout.Label("카드 종류");
            m_CardIndex = EditorGUILayout.Popup(m_CardIndex, m_CardNameList);
            GUILayout.Label("카드 개수");
            m_GoodsAmount = EditorGUILayout.IntField(m_GoodsAmount);
            */
        }
        else if (m_PostType == ePostType.RandomBox)
        {
            GUILayout.Label("박스 종류");
            m_BoxIndex = EditorGUILayout.Popup(m_BoxIndex, m_BoxNameList);
        }

        if (m_PostType != ePostType.Card)
        {
            GUILayout.Space(10f);

            if (!string.IsNullOrEmpty(m_Title) && !string.IsNullOrEmpty(m_Message))
            {
                if (GUILayout.Button("지급 요청"))
                {
                    int AchieveIndex = 0;
                    int AchieveAmount = 0;
                    if (m_PostType == ePostType.Goods)
                    {
                        AchieveIndex = (int)m_GoodsType;
                        AchieveAmount = m_GoodsAmount;
                    }
                    else if (m_PostType == ePostType.Card)
                    {
                        AchieveIndex = m_CardIndexList[m_CardIndex];
                        AchieveAmount = m_GoodsAmount;
                    }
                    else if (m_PostType == ePostType.RandomBox)
                    {
                        AchieveIndex = m_BoxIndexList[m_BoxIndex];
                        AchieveAmount = 1;
                    }

                    Kernel.entry.administrator.REQ_PACKET_CG_GAME_GM_ADD_GOODS_SYN(m_Title, m_Message, m_PostType, AchieveIndex, AchieveAmount);
                }
            }
        }

        GUILayout.EndVertical();
    }
}
