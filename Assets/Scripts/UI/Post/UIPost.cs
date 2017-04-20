using UnityEngine;
using System.Collections;
using Common.Packet;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIPost : UIObject 
{
    //** Scroll Load를 위한.
    private int                 m_nLastItemCount;
    private float               m_fLastScrollPostion;
    private float               m_fLastItemParentPosition;

    private bool                m_bItemRevWait;
    private bool                m_bItemRemoveWait;

    private List<RectTransform> m_listItemRect              = new List<RectTransform>();
    private List<UIPostInfo>    m_listItem                  = new List<UIPostInfo>();

    public Text                 m_PostMainTitle;
    public Text                 m_PostCount;
    public Text                 m_PostDec;
    public Text                 m_NothingDec;
    public Text                 m_AllRev;

    public Button               m_AllRevButton;

    public UIPostInfo           m_CopyPostInfo;
    public GameObject           m_ItemParent;
    public GameObject           m_Nothing;
    public UIScrollRect         m_ScrollRect;

    private int m_nPostCount;
    private int postCount
    {
        get
        {
            return m_nPostCount;
        }
        set
        {
            m_nPostCount = value;
            m_PostCount.text = value.ToString();
            m_Nothing.SetActive(value <= 0);
        }
    }

#region 50 Create ScrollItem

    private int m_nCurrentGroup;
    private int m_nSendGroup;

    private bool existNextGroup
    {
        get
        {
            Post post = Kernel.entry.post;

            int groupSetCount = post.N_CREATE_ITEM_GROUP_COUNT;
            int m_nTotalGroup = (int)(post.m_nTotalCount / groupSetCount);

            if (post.m_nTotalCount % groupSetCount != 0)
                m_nTotalGroup++;

            if (m_nTotalGroup > m_nCurrentGroup)
                return true;

            return false;
        }
    }

    private bool endScroll
    {
        get
        {
            if (existNextGroup)
                return false;

            if (postCount <= m_listItem.Count)
                return false;

            return true;
        }
    }

#endregion



    protected override void Awake()
    {
        Kernel.entry.post.onOpenPoupCallback            += OpenResultPopup;
        Kernel.entry.post.onOpenPostCallback            += ReciveOneItem;
        Kernel.entry.post.onOpenAllPostCallback         += ReciveAllItem;
        m_ScrollRect.onInitializePotentialDragged       += OnInitializePotentialDragged;

         m_AllRevButton.onClick.AddListener(AllItemRev);

        //스크롤 밑단의 Postion 구하기.
         m_fLastScrollPostion = m_ScrollRect.viewport.anchoredPosition.y - (m_ScrollRect.viewport.rect.height * 0.5f);

         SetUI();

         base.Awake();
    }

    protected override void OnEnable()
    {
        Kernel.entry.post.LinkUpdate(false);
        Kernel.entry.post.onCreateUIItemPostCallback    += SetItemData;
        Kernel.entry.post.newPostCheckExist = false;
        Kernel.entry.post.onActivePostCallback(Kernel.entry.post.newPostCheckExist);

        SetItemData();

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        Kernel.entry.post.LinkUpdate(true);
        Kernel.entry.post.onCreateUIItemPostCallback    -= SetItemData;
        Kernel.entry.post.onCreateUIItemPostCallback    = null;

        SetInit();

        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        Kernel.entry.post.onOpenPoupCallback            -= OpenResultPopup;
        Kernel.entry.post.onOpenPostCallback            -= ReciveOneItem;
        Kernel.entry.post.onOpenAllPostCallback         -= ReciveAllItem;
        m_ScrollRect.onInitializePotentialDragged       -= OnInitializePotentialDragged;

        m_AllRevButton.onClick.RemoveAllListeners();
        
    }

    //** 초기화
    private void SetInit()
    {
        ReciveAllItem();

        if (m_listItemRect != null)
            m_listItemRect.Clear();

        if (m_listItem != null)
            m_listItem.Clear();

        m_ScrollRect.content.anchoredPosition = Vector2.zero;

        m_nLastItemCount = 0;
        m_nCurrentGroup = 0;

    }

    //** UI 세팅
    private void SetUI()
    {
        m_PostMainTitle.text    = Languages.ToString(TEXT_UI.MAIL);
        m_PostDec.text          = Languages.ToString(TEXT_UI.MAIL_DATE_KEEP);
        m_NothingDec.text       = Languages.ToString(TEXT_UI.MAIL_EMPTY);
        m_AllRev.text           = Languages.ToString(TEXT_UI.ALL_GET);
    }

    ////** 중요 순서대로 정렬하기
    ////** 기획상 재화만이 아닌 다른 종류의 우편이 생긴다면 정렬이 필요.
    //private void SetPositionData(List<CPostBox> postList)
    //{
    //    if (postList.Count <= 0 || postList == null)
    //        return;
            
    //    List<CPostBox> listAllPost      = new List<CPostBox>();
    //    List<CPostBox> listNoticePost   = new List<CPostBox>();
    //    List<CPostBox> listNormalPost   = new List<CPostBox>();

    //    for (int i = 0; i < postList.Count; i++)
    //    {
    //        if (postList[i].m_sMailType == "Notice")
    //            listNoticePost.Add(postList[i]);
    //        else if (postList[i].m_sMailType == "Normal")
    //            listNormalPost.Add(postList[i]);
    //    }

    //    if (listNoticePost != null)
    //        listAllPost = listNoticePost;

    //    for (int i = 0; i < listNormalPost.Count; i++)
    //        listAllPost.Add(listNormalPost[i]);

    //    SetItemData();
    //}

    //** 스크롤 아이템에 데이터 세팅
    private void SetItemData()
    {
        Post post = Kernel.entry.post;

        if (post == null)
            return;

        postCount = post.m_nTotalCount;

        int count = 0;
        int movePositionCount = 0;
        foreach (CPostBox postData in post.m_dicPost.Values)
        {
            if (postData == null)
                continue;

            // 다음 만들어야 하는 포지션까지 이동하기.
            if (m_nLastItemCount > 0)
            {
                if (movePositionCount < m_nLastItemCount)
                {
                    movePositionCount++;
                    continue;
                }
            }

            UIPostInfo postItem = Instantiate<UIPostInfo>(m_CopyPostInfo);
            UIUtility.SetParent(postItem.transform, m_ItemParent.transform);
            postItem.name = string.Format("UIPostInfo Item_{0}", postData.m_Sequence);

            m_listItemRect.Add(postItem.GetComponent<RectTransform>());
            m_listItem.Add(postItem);

            postItem.SetData(postData, this);

            count++;

            // 50개만 만듬.
            if (count >= post.N_CREATE_ITEM_GROUP_COUNT || m_listItem.Count == Kernel.entry.post.m_nTotalCount)
            {
                m_nCurrentGroup++;
                m_nLastItemCount = m_listItem.Count;
                break;
            }
        }
        
        ScrollRectReSize();
    }

    public void ScrollRectReSize()
    {
        if (m_listItemRect.Count <= 0 || m_listItemRect == null)
            return;

        int itemCount = m_listItemRect.Count;
        float spacing = m_ScrollRect.content.gameObject.GetComponent<GridLayoutGroup>().spacing.y;

        float y = (itemCount * m_listItemRect[0].rect.height) + (itemCount * spacing);

        m_ScrollRect.content.sizeDelta = new Vector2(m_ScrollRect.content.sizeDelta.x, y);
    }

    private void OnInitializePotentialDragged(PointerEventData eventData)
    {
        //지우는 중이면
        if (m_bItemRemoveWait)
            return;

        //더 이상 만들 것이 없다면 그만 체크.
        if (endScroll)
            return;

        m_fLastItemParentPosition = m_ScrollRect.content.anchoredPosition.y - (m_ScrollRect.content.rect.height * 0.95f);

        if (m_fLastItemParentPosition < m_fLastScrollPostion)
            return;

        if (m_nCurrentGroup > m_nSendGroup)
        {
            m_nSendGroup++;
            Kernel.entry.post.REQ_PACKET_CG_READ_POST_BOX_LIST_SYN();
        }
        else if (existNextGroup)
            SetItemData();
    }

    //** 아이템 삭제
    public void RemoveItem(long sequence, bool deleteBaseData = true)
    {
        if (m_listItem == null || m_listItem.Count <= 0)
            return;

        Post post = Kernel.entry.post;

        if (post == null || post.m_dicPost == null)
            return;

        DeletItem();

        // dicData 삭제
        if (deleteBaseData)
        {
            post.RemovePost(sequence);
            postCount = post.m_dicPost.Count;
            post.m_nTotalCount = post.m_dicPost.Count;

            // 새로 위에서 부터 세팅
            int count = 0;
            foreach (CPostBox postData in post.m_dicPost.Values)
            {
                if (m_listItem.Count <= count)
                    break;

                m_listItem[count].SetData(postData, this);
                count++;
            }
        }
    }

    //** ScrollItem의 Item을 뒤에서 부터 삭제
    private void DeletItem()
    {
        // 있는지 부터 체크
        if (m_listItemRect == null || m_listItemRect.Count <= 0)
            return;

        // Object 삭제
        RectTransform rectLast = m_listItemRect[m_listItemRect.Count - 1];
        UIPostInfo ItemLast = m_listItem[m_listItem.Count - 1];

        GameObject.Destroy(rectLast.gameObject);
        m_listItemRect.Remove(rectLast);
        m_listItem.Remove(ItemLast);

        m_nLastItemCount = m_listItem.Count;

        ScrollRectReSize();
    }

    //** 우편 한개 열기
    public void OneItemRev(long sequence, Common.Util.ePostType postType)
    {
        if (m_bItemRevWait)
            return;

        m_bItemRevWait = true;
        Kernel.entry.post.REQ_PACKET_CG_GAME_RECEIVE_POST_BOX_SYN(sequence, postType);
    }

    //** 모든 우편 열기
    public void AllItemRev()
    {
        Post post = Kernel.entry.post;
        if (post != null)
        {
            if (post.m_dicPost != null && post.m_dicPost.Count > 0)
            {
                int goodsTypeCount = 0;

                foreach (CPostBox postBox in post.m_dicPost.Values)
                {
                    if (postBox.m_ePostType == Common.Util.ePostType.Goods)
                    {
                        goodsTypeCount++;
                        break;
                    }
                }

                if(goodsTypeCount > 0)
                    Kernel.entry.post.REQ_PACKET_CG_GAME_RECEIVE_POST_ALL_SYN();
            }
                
        }
    }

    //** 한개 우편 받기
    public void ReciveOneItem(long sequence)
    {
        m_bItemRevWait = false;
        RemoveItem(sequence);
    }

    //** 모든 우편 받기 (상자, 카드 제외)
    public void ReciveAllItem(List<CPostBox> remainPostBox = null)
    {
        m_bItemRemoveWait = true;

        if (m_listItem != null)
        {
            // 모두 받기 결과
            if (remainPostBox != null)
            {
                for (int i = 0; i < remainPostBox.Count; i++)
                {
                    CPostBox postBox = remainPostBox[i];

                    if (i > m_listItem.Count)
                        continue;

                    m_listItem[i].SetData(postBox, this);
                }

                int remainCount = m_listItem.Count - remainPostBox.Count;

                if (remainCount <= 0)
                    return;

                for (int i = 0; i < remainCount; i++)
                    DeletItem();

                postCount = Kernel.entry.post.m_nTotalCount;
            }
            else
            {
                //모두 삭제
                List<long> removeSequence = new List<long>();

                for (int i = 0; i < m_listItem.Count; i++)
                {
                    UIPostInfo postinfo = m_listItem[i];

                    removeSequence.Add(postinfo.m_lsequence);
                }

                for (int i = 0; i < removeSequence.Count; i++)
                {
                    long sequence = removeSequence[i];
                    RemoveItem(sequence, false);
                }
            }
        }

        m_bItemRemoveWait = false;
    }

    //** 우편 받기 결과 팝업 띄우기
    public void OpenResultPopup(List<CReceivedGoods> postBox, eReceivePopupType rewardType)
    {
        List<Goods_Type> goodsType = new List<Goods_Type>();
        List<int> count = new List<int>();

        for (int i = 0; i < postBox.Count; i++)
        {
            DB_Goods.Schema goodsData = DB_Goods.Query(DB_Goods.Field.Index, (int)postBox[i].m_eGoodsType);
            goodsType.Add(goodsData.Goods_Type);
            count.Add(postBox[i].m_iReceivedAmount);
        }

        string title = rewardType == eReceivePopupType.RT_ONE ? Languages.ToString(TEXT_UI.MAIL_GET) : Languages.ToString(TEXT_UI.ALL_GET);
        string subTitle = rewardType == eReceivePopupType.RT_ONE ? Languages.ToString(TEXT_UI.MAIL_GET_INFO) : "";

        UIPopupReceive popupReceive = Kernel.uiManager.Get<UIPopupReceive>(UI.PopupReceive, true, false);

        if (popupReceive != null)
        {
            popupReceive.SetData(rewardType, goodsType, count, title, subTitle);
            UIManager.Instance.Open(UI.PopupReceive);
        }
            
    }
}
