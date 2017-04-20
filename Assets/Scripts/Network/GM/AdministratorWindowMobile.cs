using System;
using System.Runtime.InteropServices;
using Common.Util;

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AdministratorWindowMobile  : Singleton<AdministratorWindowMobile>
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

    public Vector2 v1=new Vector2();
    public Vector2 v2 = new Vector2();
    public Vector2 v3 = new Vector2();
    private Vector2 v4=new Vector2();
    public  Rect rect=new Rect();
    public  Rect rectF=new Rect(200,200,100,100);
    public bool isOpen;
    private UnityEngine.UI.Image bg;
    private Sprite sprite;
    private Texture2D tex ;
    private GUIStyle btnStyle;
    static int guisize = 0;
    static int guisizeRect = 0;
    public int guisize2 = 0;

   
    protected override void Awake()
    {
        base.Awake();
        tex = new Texture2D(1, 1);
        btnStyle = new GUIStyle() { normal = new GUIStyleState() { textColor = Color.red, background = tex } };
    }
    void OnEnable()
    {
        if(!Application.isPlaying)return;
        //minSize = new Vector2(256f, 256f);
        m_TimeScale = 1;
        rect.x = 0;
        rect.y = 0;
        rect.width = Screen.width ;
        rect.height = Screen.height;
        
    }

    
    void OnGUI()
    {
        guisizeRect = guisize2;
        if (!Application.isPlaying) return;
        if (GUI.Button(rectF, "GM",btnStyle))
        {
            if (
                Event.current.type != EventType.mouseDrag&&
                Event.current.type != EventType.mouseMove&&
                Event.current.type != EventType.MouseDrag&&
                Event.current.type != EventType.mouseMove
                )
            {
                isOpen = !isOpen;
                if (isOpen)
                {
                    rect.width = Screen.width;
                    rect.height = Screen.height;
                    if (bg == null)
                    {
                        bg = new GameObject("gm_bg").AddComponent<UnityEngine.UI.Image>();
                        bg.color = Color.black;
                        //bg.sortingOrder = "UI";
                        //bg.sortingOrder = 1000;
                        if (CanvasManager.Instance)
                            bg.transform.parent = CanvasManager.Instance.transform.FindChild("1000");
                        if(sprite==null)
                            sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
                        bg.sprite = sprite;
                        bg.gameObject.layer = LayerMask.NameToLayer("UI");
                        //bg.gameObject.AddComponent<BoxCollider>().size=new Vector2(10000,10000);
                        bg.transform.localScale=new Vector3(10000,10000,0);
                    }

                }
                else
                {
                    if (bg != null)
                    {
                       DestroyImmediate(bg.gameObject); 
                    }
                }
            }
           
            
        }
        Debug.Log(Event.current.type);
        if (Event.current.type == EventType.MouseDrag)
        {
            rectF.x = Input.mousePosition.x;
            rectF.y = Screen.height-Input.mousePosition.y;
        }
        if (!isOpen)return;
        GUILayout.BeginArea(rect);

        
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
        GUILayout.Label("level", GUILayout.Width(guisize*5));//level
        GUILayout.Space(10f);
        m_Level = int.Parse(GUILayout.TextField(m_Level.ToString()));
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
        GUILayout.Label("Goods", GUILayout.Width(guisize * 5));
        GUILayout.Space(10f);
        m_GoodsType = EnumPopup(m_GoodsType,2,ref v1,new Vector2(300,100));//;.EnumPopup(m_GoodsType);
        GUILayout.Space(10f);
        m_Amount = IntField(m_Amount);
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
        GUILayout.Label("Box", GUILayout.Width(guisize*5));
        GUILayout.Space(10f);
        m_BoxIndex = Popup(m_BoxIndex, m_BoxNameList.ToArray(), 8, ref v2, new Vector2(600, 50));
        GUILayout.Space(10f);
        GUILayout.Label("Area", GUILayout.Width(guisize * 5));
        GUILayout.Space(10f);
        m_BoxArea = IntField(m_BoxArea);
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
        GUILayout.Label("Card", GUILayout.Width(guisize * 5));
        m_CardIndex = Popup(m_CardIndex, m_CardNameList.ToArray(), 2, ref v3, new Vector2(200, 100));
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
        GUILayout.Label("Soul", GUILayout.Width(guisize * 5));
        m_SoulIndex = Popup(m_SoulIndex, m_SoulNameList.ToArray(), 2, ref v4, new Vector2(200, 100));
        GUILayout.Space(10f);
        m_SoulCount = IntField(m_SoulCount);
        GUILayout.Space(10f);
        if (GUILayout.Button("Request"))
        {
            if (m_CardIndexDictionary.ContainsKey(m_SoulNameList[m_SoulIndex]))
            {
                if (Kernel.entry != null && Kernel.dataLoader != null && Kernel.dataLoader.isLoadComplete)
                {
                    Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_SOUL_SYN(m_SoulIndex, m_SoulCount);
                }
            }
        }
        GUILayout.EndHorizontal();
        #endregion

        GUILayout.Space(10f);

        #region Time Scale
        GUILayout.BeginHorizontal();
        GUILayout.Space(10f);
        GUILayout.Label("Time Scale", GUILayout.Width(guisize * 8));
        GUILayout.Space(10f);
        m_TimeScale = IntField(m_TimeScale);
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
        GUILayout.Label("Daily achievements", GUILayout.Width(guisize * 15));
        GUILayout.Space(10f);
        m_IsDaily = GUILayout.Toggle(m_IsDaily,"");
        GUILayout.Space(10f);
        GUILayout.Label("Number", GUILayout.Width(guisize * 5));
        GUILayout.Space(10f);
        m_AchieveIndex = IntField(m_AchieveIndex);
        GUILayout.Space(10f);
        GUILayout.Label("Quantity", GUILayout.Width(guisize * 8));
        GUILayout.Space(10f);
        m_AchieveAccumulate = IntField(m_AchieveAccumulate);
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
        GUILayout.Label("AID", GUILayout.Width(guisize * 3));
        GUILayout.Space(10f);
        m_AID = LongField(m_AID);
        GUILayout.Space(10f);
        if (GUILayout.Button("Delete Account", GUILayout.Width(128f)))
        {
            Kernel.entry.administrator.REQ_PACKET_CG_GAME_CHEAT_DELETE_ACCOUNT_SYN(m_AID);
        }
        GUILayout.EndHorizontal();
        #endregion
        GUILayout.EndArea();
    }

    static public T EnumPopup<T>(T _enum, int xCount, ref Vector2 v,Vector2 viewSize)
    {
        v = GUILayout.BeginScrollView(v, GUILayout.Width(viewSize.x), GUILayout.Height(viewSize.y));
        string[] list = Enum.GetNames(_enum.GetType());
        int index=0;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == _enum.ToString())
            {
                index = i;
            }
        }

        index = GUILayout.SelectionGrid(index, list, xCount, GUILayout.Width(guisize * 5));
       for (int i = 0; i < list.Length; i++)
       {
           if (i == index)
           {
               _enum=(T)Enum.Parse(typeof(T),list[i]);
           }
       }
       GUILayout.EndScrollView();
       return _enum;
    }


    static public int Popup(int m_BoxIndex, string[] m_BoxList, int xCount, ref Vector2 v, Vector2 viewSize)
    {

        v = GUILayout.BeginScrollView(v, GUILayout.Width(viewSize.x), GUILayout.Height(viewSize.y));
       int i = GUILayout.SelectionGrid(m_BoxIndex, m_BoxList, xCount, GUILayout.Width(guisize * 5));
        GUILayout.EndScrollView();

        return i;
    }
    

    static public int IntField(int num)
    {
        var s = GUILayout.TextField(num.ToString());
        int n;
        if (int.TryParse(s, out n))
            return n;
        return 0;
    }
    static public long LongField(long num)
    {
        var s = GUILayout.TextField(num.ToString());
        long n;
        if (long.TryParse(s, out n))
            return n;
        return 0;
    }
}
