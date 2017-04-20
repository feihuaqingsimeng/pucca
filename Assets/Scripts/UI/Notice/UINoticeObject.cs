using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class NoticeTypeUI
{
    public eNoticeType  m_eNoticeType;
    public Sprite       m_BackImage;

    public Color        m_NormalColor;
    public Color        m_OutLineColor;
    public Color        m_ShadowColor;
}

[System.Serializable]
public class NoticeIssueTypeUI
{
    public eNoticeIssueType m_eNoticeIssueType;
    public Sprite           m_IconImage;
}

public class UINoticeObject : MonoBehaviour 
{
    public UINotice             m_owenr;

    private eNoticeType         m_eNoticeType       = eNoticeType.NT_NONE;
    private eNoticeIssueType    m_eNoticeIssueType  = eNoticeIssueType.NIT_NONE;

    public Text                 m_NoticeType_Text;
    public Text                 m_Dec_Text;

    public Outline              m_NoticeType_Outline;
    public Shadow               m_NoticeType_Shadow;

    public Image                m_TypeBack_Image;
    public Image                m_IssueType_Image;

    public Button               m_NoticeButton;

    public GameObject           m_WebViewParent;

    private string              m_WebViewURL;

    public List<NoticeTypeUI>       m_arrNoticeTypeSprite;
    public List<NoticeIssueTypeUI>  m_arrNoticeIssueTypeSprite;

    private void OnEnable()
    {
        if (m_NoticeButton != null)
            m_NoticeButton.onClick.AddListener(OnClickItem);


    }

    private void OnDisable()
    {
        if (m_NoticeButton != null)
            m_NoticeButton.onClick.RemoveAllListeners();
    }

    //** Init
    public void SetInit(NoticeData data)
    {
        m_eNoticeType       = data.m_eNoticeType;
        m_eNoticeIssueType  = data.m_eNoticeIssueType;
        m_WebViewURL        = data.m_strURL;
        m_Dec_Text.text     = data.m_strDec;

        switch(data.m_eNoticeType)
        {
            case eNoticeType.NT_EVENT : m_NoticeType_Text.text = Languages.ToString(TEXT_UI.NOTICE_TITLE_TAG_EVENT); break;
            case eNoticeType.NT_PROMOTION: m_NoticeType_Text.text = Languages.ToString(TEXT_UI.NOTICE_TITLE_TAG_PROMOTION); break;
            case eNoticeType.NT_SYSTEM: m_NoticeType_Text.text = Languages.ToString(TEXT_UI.NOTICE_TITLE_TAG_SYSTEM); break;
            default: m_NoticeType_Text.text = ""; break;
        }

        NoticeTypeUI noticeTypeSprite = m_arrNoticeTypeSprite.Find(item => item.m_eNoticeType == data.m_eNoticeType);

        if (noticeTypeSprite != null)
        {
            m_TypeBack_Image.sprite             = noticeTypeSprite.m_BackImage;
            m_NoticeType_Text.color             = noticeTypeSprite.m_NormalColor;
            m_NoticeType_Outline.effectColor    = noticeTypeSprite.m_OutLineColor;
            m_NoticeType_Shadow.effectColor     = noticeTypeSprite.m_ShadowColor;
        }
            
        NoticeIssueTypeUI noticeIssueTypeSprite = m_arrNoticeIssueTypeSprite.Find(item => item.m_eNoticeIssueType == data.m_eNoticeIssueType);

        if (noticeIssueTypeSprite != null)
            m_IssueType_Image.sprite = noticeIssueTypeSprite.m_IconImage;
    }

    //** 아이템 클릭 시 
    public void OnClickItem()
    {
        if (m_owenr.m_CurrentURL == m_WebViewURL)
            return;

        m_owenr.m_CurrentURL = m_WebViewURL;

        Debug.Log("StartWebView : " + m_WebViewURL);
        string strUrl = m_WebViewURL;

        m_owenr.m_WebViewObject =
            m_WebViewParent.AddComponent<WebViewObject>();
        m_owenr.m_WebViewObject.Init((msg) =>
        {
            Debug.Log(string.Format("CallFromJS[{0}]", msg));
        });

        m_owenr.m_WebViewObject.LoadURL(strUrl);
        m_owenr.m_WebViewObject.SetVisibility(true);
        m_owenr.m_WebViewObject.SetMargins(760, 120, 38, 206);
    }
}
