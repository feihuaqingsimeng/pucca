using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UINotice : UIObject 
{
    public  Text            m_Title_Text;
    public  Text            m_CurrentServerTime_Text;
    public  Text            m_OkButton_Text;

    public  Button          m_OK_Button;

    public  GridLayoutGroup m_trsParentGrid;

    public UIIndicator      m_indicator;

    public  UINoticeObject          m_CopyNoticeObject;
    private List<UINoticeObject>    m_listCopyNoticeObject = new List<UINoticeObject>();

    [HideInInspector]
    public  WebViewObject   m_WebViewObject;
    [HideInInspector]
    public  string          m_CurrentURL;


    protected override void Awake()
    {
        base.Awake();

        SetInit();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        m_OK_Button.onClick.AddListener(OnClickOKButton);

        UIHUD hud = Kernel.uiManager.Get<UIHUD>(UI.HUD, true, false);

        if (hud != null && hud.isActiveAndEnabled)
            Kernel.uiManager.Close(UI.HUD);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_OK_Button.onClick.RemoveAllListeners();
    }

    protected override void Update()
    {
        DateTime serverTime = TimeUtility.currentServerTime;
        m_CurrentServerTime_Text.text = string.Format("{0}-{1}-{2}T{3:00}:{4:00}:{5:00}", serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, serverTime.Minute, serverTime.Second);
    }

    //** Init
    public void SetInit()
    {
        m_Title_Text.text       = Languages.ToString(TEXT_UI.NOTICE_MAIN_TITLE);
        m_OkButton_Text.text    = Languages.ToString(TEXT_UI.OK);
    }

    //** 아이템 생성
    public void CreateItems()
    {
        DestroyAllItems();

        m_CopyNoticeObject.gameObject.SetActive(true);

        List<NoticeData> noticeDatas = Kernel.entry.notice.GetNoticeDatas();

        if (noticeDatas == null)
            return;

        for (int i = 0; i < noticeDatas.Count; i++)
        {
            NoticeData data = noticeDatas[i];

            UINoticeObject copyObject = Instantiate<UINoticeObject>(m_CopyNoticeObject);
            UIUtility.SetParent(copyObject.transform, m_trsParentGrid.transform);
            copyObject.SetInit(data);

            m_listCopyNoticeObject.Add(copyObject);
        }

        m_CopyNoticeObject.gameObject.SetActive(false);

        SetParentSize();
        SetFirstItemView();
    }

    //** 첫번째 아이템으로 세팅
    private void SetFirstItemView()
    {
        if (m_listCopyNoticeObject == null || m_listCopyNoticeObject.Count <= 0)
        {
            m_indicator.gameObject.SetActive(false);
        }
        else
        {
            m_indicator.gameObject.SetActive(true);
            UINoticeObject firstObject = m_listCopyNoticeObject[0];
            firstObject.OnClickItem();
        }
    }

    //** Parent 사이즈 조절
    private void SetParentSize()
    {
        RectTransform recttrans = m_trsParentGrid.GetComponent<RectTransform>();

        if (recttrans == null || m_listCopyNoticeObject == null)
            return;

        float sizeY = (m_trsParentGrid.cellSize.y * m_listCopyNoticeObject.Count) + (m_trsParentGrid.spacing.y * m_listCopyNoticeObject.Count) + m_trsParentGrid.padding.top;
        float sizeX = recttrans.sizeDelta.x;

        recttrans.sizeDelta = new Vector2(sizeX, sizeY);
    }

    //** 모든 아이템 제거
    private void DestroyAllItems()
    {
        if (m_listCopyNoticeObject == null)
            return;

        for (int i = 0; i < m_listCopyNoticeObject.Count; i++)
        {
            UINoticeObject noticeObejct = m_listCopyNoticeObject[i];

            Destroy(noticeObejct.gameObject);
        }

        m_listCopyNoticeObject.Clear();
    }

    //** 확인 버튼 클릭시
    public void OnClickOKButton()
    {
        if (m_WebViewObject != null)
            Destroy(m_WebViewObject);

        OnCloseButtonClick();
        Kernel.sceneManager.LoadScene(Scene.Lobby);
    }
}
