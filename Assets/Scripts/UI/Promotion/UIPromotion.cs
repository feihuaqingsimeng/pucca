using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

//** 애니메이션을 위한 필요정보
public class CharInfo
{
    public Vector2              m_OriginPosition;
    public RectTransform        m_position;
    public SkeletonAnimation    m_anim;
}

public class UIPromotion : UIObject 
{
    private const string    STR_BASE_BG_NAME        = "img_Lobby_Background_{0}";
    private const string    STR_BASE_ICON_BG_NAME   = "ui_raise_bg_0{0}";

    //private const float     F_CHAR_DISTANCE         = 800.0f;   //캐릭터가 도착해야하는 위치
    private const float     F_UI_DISTANCE           = 300.0f;   //처음에 UI를 세팅할 위치    //기존 200. 8/17수정.
    
    //private const float     F_CHAR_MOVE_SPEED       = 5.0f;
    private const float     F_INFO_ALPHA_IN_SPEED   = 0.03f;   // 0 ~ 1
    private const float     F_INFO_ALPHA_STOP_TIME  = 1.5f;     
    private const float     F_WHILE_ALPHA_IN_SPEED  = 0.05f;   // 0 ~ 1
    private const float     F_WHILE_ALPHA_STOP_TIME = 0.2f;
    private const float     F_PANEL_ACTIVE_SPEED    = 10.0f;    //기존 7.0f. 8/17수정.

    private bool            m_bIsPromotion;
    private int             m_nPVPArea;
    private float           m_fNextPosition;

    //** UILobby Data
    private RectTransform   m_TopPanel;
    private RectTransform   m_LeftPanel;
    private RectTransform   m_BottomPanel;
    private RectTransform   m_BottomPanel_02;
    private RectTransform   m_RightPanel;

    private List<CharInfo>  m_listCharInfo = new List<CharInfo>();

    public  CanvasGroup     m_GroupCanvas;

    public  Text            m_AreaText;

    private Image           m_AreaImg;
    private Transform       m_BackgroundAddOn_Parent;
    private GameObject      m_BackgroundAddOn_Object;
    private GameObject      m_BackgroundAddOn_Coppy_Object;

    public  Image           m_AreaIconImg;
    public  Image           m_WhiteImg;

    public  Image           m_MainTleImg;

    //** Promotion MainTle
    public  Sprite          m_PromotionTle;
    public  Sprite          m_DemotionTle;
    
    //** Promotion Effect
    public GameObject       m_PromotionEffect;
    public GameObject       m_PromotionParticle;

    public GameObject       m_DemotionEffect;
    public GameObject       m_DemotionParticle;


    //** 기본 정보들 세팅
    public void SetInit(RectTransform trLeft, RectTransform trBottom, RectTransform trBottom_02, RectTransform trRight, Image areaImage, Transform backgroundParent, GameObject backgroundObj)
    {
        SetUIInit(trLeft, trBottom, trBottom_02, trRight, areaImage, backgroundParent, backgroundObj);
        //SetCharacterInit(characterSummary);
    }

    //** UI 세팅
    private void SetUIInit(RectTransform trLeft, RectTransform trBottom, RectTransform trBottom_02, RectTransform trRight, Image areaImage, Transform backgroundParent, GameObject backgroundObj)
    {
        GameObject m_TopPanelObj = Kernel.uiManager.Get(UI.HUD).gameObject;
        m_TopPanel = m_TopPanelObj.GetComponent<RectTransform>();

        if (trLeft == null || trBottom == null || trBottom_02 == null || m_TopPanel == null || areaImage == null)
            return;

        m_LeftPanel = trLeft;
        m_BottomPanel = trBottom;
        m_BottomPanel_02 = trBottom_02;
        m_RightPanel = trRight;

        m_AreaImg = areaImage;
        m_BackgroundAddOn_Parent = backgroundParent;
        m_BackgroundAddOn_Object = backgroundObj;

        SetUI();
    }

    public void SetUI()
    {
        // alpha = 0, position = 0
        m_GroupCanvas.alpha = 0.0f;
        m_fNextPosition = 0.0f;

        // Area Text
        m_nPVPArea = Kernel.entry.account.currentPvPArea - 1;
        string areaEnumName = "AREA_" + Kernel.entry.account.currentPvPArea.ToString();
        TEXT_UI areaName = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), areaEnumName);
        m_AreaText.text = Languages.ToString(areaName);

        // Effect Image
        m_bIsPromotion = Kernel.entry.battle.fluctuationPvpArea == 1 ? true : false;

        //** Test
        //m_AreaImg.sprite = TextureManager.GetSprite(SpritePackingTag.Lobby, string.Format(STR_BASE_BG_NAME, m_nPVPArea));
        //m_bIsPromotion = false;

        // Area Image
        SetBackground(true);

        m_MainTleImg.sprite = m_bIsPromotion ? m_PromotionTle : m_DemotionTle;
        m_PromotionEffect.SetActive(m_bIsPromotion);
        m_PromotionParticle.SetActive(m_bIsPromotion);
        m_DemotionEffect.SetActive(!m_bIsPromotion);
        m_DemotionParticle.SetActive(!m_bIsPromotion);
    }

    private void SetBackground(bool isPre)
    {
        if (m_BackgroundAddOn_Coppy_Object != null)
            Destroy(m_BackgroundAddOn_Coppy_Object);

        int area = 0;

        if (isPre)
            area =  Kernel.entry.account.prePVPArea -1;
        else
            area = Kernel.entry.account.currentPvPArea - 1;

        m_AreaImg.sprite = TextureManager.GetSprite(SpritePackingTag.Lobby, string.Format(STR_BASE_BG_NAME, area));
        m_AreaIconImg.sprite = TextureManager.GetSprite(SpritePackingTag.Promotion, string.Format(STR_BASE_ICON_BG_NAME, Kernel.entry.account.currentPvPArea));
        m_BackgroundAddOn_Object = Instantiate(Resources.Load("Prefabs/UI/Lobby/LobbyAddOn_" + area.ToString())) as GameObject;
        m_BackgroundAddOn_Object.transform.SetParent(m_BackgroundAddOn_Parent);

        RectTransform objectRect = m_BackgroundAddOn_Object.GetComponent<RectTransform>();
        objectRect.anchoredPosition = Vector2.zero;
        objectRect.sizeDelta = Vector2.zero;
        m_BackgroundAddOn_Object.transform.localScale = Vector3.one;
        m_BackgroundAddOn_Coppy_Object = m_BackgroundAddOn_Object;

        RectTransform backgroundRect = m_AreaImg.GetComponent<RectTransform>();

        if (area == 3)    //해적선 위일때 배경 바다를 움직이기위해 애니메이션 재생.
            m_AreaImg.GetComponent<Animation>().Play();
        else
            m_AreaImg.GetComponent<Animation>().Stop();

        backgroundRect.anchoredPosition = Vector2.zero;
        backgroundRect.sizeDelta = Vector2.zero;
    }

    //** 연출 시작
    public void StartAct()
    {
        //** Animation Init
        m_TopPanel.anchoredPosition         = new Vector2(0.0f, F_UI_DISTANCE);
        m_LeftPanel.anchoredPosition        = new Vector2(-F_UI_DISTANCE, 0.0f);
        m_BottomPanel.anchoredPosition      = new Vector2(0.0f, -F_UI_DISTANCE);
        m_BottomPanel_02.anchoredPosition   = new Vector2(0.0f, -F_UI_DISTANCE);
        m_RightPanel.anchoredPosition       = new Vector2(F_UI_DISTANCE, 0.0f);

        StartCoroutine(BaseAnimation());
    }

    //** 연출의 모든 흐름
    private IEnumerator BaseAnimation()
    {
        // Fade Out
        while (m_GroupCanvas.alpha < 1.0f)
        {
            m_GroupCanvas.alpha += F_INFO_ALPHA_IN_SPEED /** Time.deltaTime*/;
            yield return null;
        }

        yield return new WaitForSeconds(F_INFO_ALPHA_STOP_TIME);

        while (m_WhiteImg.color.a < 1.0f)
        {
            SetWhiteUIAlpha(F_WHILE_ALPHA_IN_SPEED);
            yield return null;
        }

        // Wait and Change Image
        yield return new WaitForSeconds(F_WHILE_ALPHA_STOP_TIME);

        SetBackground(false);

        // Fade In
        while (m_WhiteImg.color.a > 0.0f)
        {
            SetWhiteUIAlpha(-F_WHILE_ALPHA_IN_SPEED);
            yield return null;
        }

        yield return new WaitForSeconds(F_INFO_ALPHA_STOP_TIME);

        // Particle을 먼저 꺼줌.
        m_PromotionParticle.SetActive(false);
        m_DemotionParticle.SetActive(false);

        while (m_GroupCanvas.alpha > 0.0f)
        {
            m_GroupCanvas.alpha -= F_INFO_ALPHA_IN_SPEED /** Time.deltaTime*/;
            yield return null;
        }

        // IconMove
        StartCoroutine(UIMoveAnim());
    }

    private void SetWhiteUIAlpha(float speed)
    {
        Color a = m_WhiteImg.color;
        a.a += speed /** Time.deltaTime*/;
        m_WhiteImg.color = a;
    }

    //** UI 나타나기 Animation
    private IEnumerator UIMoveAnim()
    {
        m_fNextPosition = F_PANEL_ACTIVE_SPEED /** Time.deltaTime*/;

        while (m_TopPanel.anchoredPosition.y >= 0.0f)
        {
            m_TopPanel.anchoredPosition         += new Vector2(0.0f, - m_fNextPosition);
            m_LeftPanel.anchoredPosition        += new Vector2(m_fNextPosition, 0.0f);
            m_BottomPanel.anchoredPosition      += new Vector2(0.0f, m_fNextPosition);
            m_BottomPanel_02.anchoredPosition   += new Vector2(0.0f, m_fNextPosition);
            m_RightPanel.anchoredPosition       += new Vector2(-m_fNextPosition, 0.0f);

            yield return null;
        }

        // 원래 자리로 돌려놓기
        m_TopPanel.anchoredPosition         = Vector2.zero;
        m_LeftPanel.anchoredPosition        = Vector2.zero;
        m_BottomPanel.anchoredPosition      = Vector2.zero;
        m_BottomPanel_02.anchoredPosition   = Vector2.zero;
        m_RightPanel.anchoredPosition       = Vector2.zero;

        CompletAct();
    }

    //** 완료
    private void CompletAct()
    {
        // 한번 연출이 끝나고 나중에 연출이 또 되지 않도록 하기 위함.
        Kernel.entry.battle.fluctuationPvpArea = 0;
        Kernel.uiManager.Close(UI.Promotion);
    }
}
