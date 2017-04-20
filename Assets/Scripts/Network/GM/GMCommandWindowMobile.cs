
using System;
using Common.Util;

using UnityEngine;
using System.Collections.Generic;
[ExecuteInEditMode]
public class GMCommandWindowMobile:MonoBehaviour

{
    ePostType m_PostType;
    eGoodsType m_GoodsType;
    int m_GoodsAmount;
    int m_CardIndex;
    int m_BoxIndex;
    int m_BoxAmount;
    string m_Title;
    string m_Message;

    string[] m_CardList;
    string[] m_BoxList;
    private bool isShowm_PostType=false;
    private bool isShowm_GoodsType = false;
    private bool isShowm_CardList = false;
    private bool isShowm_BoxList = false;

    public GUIStyle stye;
    void OnEnable()
    {
        m_Title = PlayerPrefs.GetString("gm.command.title", string.Empty);
        m_Message = PlayerPrefs.GetString("gm_command.message", string.Empty);

        if (Kernel.entry != null && Kernel.dataLoader != null && Kernel.dataLoader.isLoadComplete)
        {
            List<string> cardList = new List<string>();
            for (int i = 0; i < DB_Card.instance.schemaList.Count; i++)
            {
                cardList.Add(DB_Card.instance.schemaList[i].IdentificationName);
            }
            m_CardList = cardList.ToArray();

            List<string> boxList = new List<string>();
            for (int i = 0; i < DB_BoxGet.instance.schemaList.Count; i++)
            {
                boxList.Add(DB_BoxGet.instance.schemaList[i].Box_IdentificationName);
            }
            m_BoxList = boxList.ToArray();
        }
    }

    void OnDisable()
    {
        PlayerPrefs.SetString("gm.command.title", m_Title);
        PlayerPrefs.SetString("gm_command.message", m_Message);
    }

    void OnGUI()
    {
        if (Kernel.entry == null || Kernel.dataLoader == null || !Kernel.dataLoader.isLoadComplete)
        {
            GUILayout.Label("GM Command 를 사용하시려면 게임에 로그인 되어 있어야 합니다.(若想使用,被登录游戏,我才能。)");
            return;
        }

        GUILayout.Label("[유저 일괄 보상 시스템]批量补偿系统");
        GUILayout.Space(10f);

        GUILayout.BeginVertical();
        GUILayout.Label("우편함 제목（信箱题目）");
        m_Title =GUILayout.TextField(m_Title);
        GUILayout.Label("우편함 메시지（信箱信息）");
        m_Message = GUILayout.TextField(m_Message);
        GUILayout.Label("우편함 상품타입（信箱商品类型）");
        m_PostType = (ePostType)EnumPopup<ePostType>(m_PostType,ref isShowm_PostType);

        if (m_PostType == ePostType.Goods)
        {
            GUILayout.Label("보상 종류（补偿种类）");
            m_GoodsType = (eGoodsType)EnumPopup(m_GoodsType,ref isShowm_GoodsType);
            GUILayout.Label("보상 개수（补偿计数）");
            m_GoodsAmount = IntField(m_GoodsAmount);
        }
        else if (m_PostType == ePostType.Card)
        {
            GUILayout.Label("카드 종류（卡种类）");
            m_CardIndex = Popup(m_CardIndex, m_CardList,ref isShowm_CardList);
        }
        else if (m_PostType == ePostType.RandomBox)
        {
            GUILayout.Label("박스 종류（box种类）");
            m_BoxIndex = Popup(m_BoxIndex, m_BoxList,ref  isShowm_BoxList);
        }

        GUILayout.Space(10f);
        if (GUILayout.Button("지급 요청（支付请求）"))
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
                AchieveIndex = m_CardIndex;
                AchieveAmount = 1;
            }
            else if (m_PostType == ePostType.RandomBox)
            {
                AchieveIndex = m_BoxIndex;
                AchieveAmount = 1;
            }
            
            Kernel.entry.administrator.REQ_PACKET_CG_GAME_GM_ADD_GOODS_SYN(m_Title, m_Message, m_PostType, AchieveIndex, AchieveAmount);
        }

        GUILayout.EndVertical();

    }
 static public T EnumPopup<T>(T _enum, ref bool isShow)
 {

     if(GUILayout.Button("↕  " + _enum.ToString()))
     {
         isShow = !isShow; 
     }
     
     if (isShow)
     {
         foreach (var v in Enum.GetNames(_enum.GetType()))
         {
             if (GUILayout.Button("   "+v))
             {
                 Debug.Log("select: " + v);
                 var oj = Enum.Parse(_enum.GetType(), v);
                 isShow = false;
                 return (T) oj;

             }
         }

     }
     return _enum;
     // GUILayout.VerticalScrollbar();
 }

    public static float value=0;
    public static Rect rect=new Rect(0,0,200,500);
   static public int Popup(int m_BoxIndex, string[] m_BoxList,ref bool isShow,GUIStyle stl=null)
   {
     // return GUILayout.SelectionGrid(m_BoxIndex, m_BoxList, 1);
       if (m_BoxList == null) return 0;

       if (GUILayout.Button("↕  " + m_BoxList[m_BoxIndex].ToString()))
       {
           isShow = !isShow;
       }

       if (isShow)
       {
           if (stl!=null)
           GUILayout.BeginArea(rect, stl);
           else
           {
               GUILayout.BeginArea(rect);
           }
           for (int i = 0; i < m_BoxList.Length; i++)
           {
               GUILayout.BeginHorizontal();
               GUILayout.Space(5);
               if (GUILayout.Button("   " + m_BoxList[i]))
               {
                   Debug.Log("select: " + m_BoxList[i]);

                   isShow = false;
                   return i;
               }
               GUILayout.EndHorizontal();
           }

           value = GUILayout.VerticalScrollbar(value, 500, 0, 500);
           GUILayout.EndArea();
       }
    
        return m_BoxIndex; 
    }

    static public int IntField(int num )
    {
        var s = GUILayout.TextField(num.ToString());
        int n;
        if (int.TryParse(s, out n))
            return n;
        return 0;
    }
}
