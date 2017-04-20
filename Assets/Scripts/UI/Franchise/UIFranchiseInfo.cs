using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

[System.Serializable]
public class NeedGoodsObject
{
    public GameObject   m_GoodsObject;
    public Image        m_GoodsIcon;
    public Text         m_GoodsValueText;
}

public class UIFranchiseInfo : UIObject 
{
    private int      m_nBuildingNum;
    private int      m_nFloor;

    //** Main
    public  Text     m_TitleText;
    public  Button   m_OpenButton;
    public  Text     m_OpenButtonText;
    private Image    m_OpenButtonImage;

    public  Outline  m_OpenButtonOutLine;
    public  Shadow   m_OpenButtonShadow;
    public  Color    m_OpenAbleOutLineColor;
    public  Color    m_OpenEnAbleOutLineColor;
    public  Color    m_OpenAbleShadowColor;
    public  Color    m_OpenEnAbleShadowColor;
    
    //** Left
    public  Text     m_DecText;
    public  Text     m_CoolTimeText;
    public  Text     m_CoolTimeValueText;
    public  Image    m_RoomImage;
    public  Image    m_RoomTableImg;
    public  SkeletonAnimation m_SkeletonAnim;

    //** Right
    public  Text     m_RightTitleText;
    public  Text     m_LevelText;
    public  Text     m_LevelValueText;
    public  Text     m_PreTermsText;
    public  Text     m_PreTermsValueText;
    public  Text     m_NeedGoodsText;
    public  NeedGoodsObject[] m_NeedGoodsObjects;

    protected override void Awake()
    {
        base.Awake();

        if (m_OpenButton != null)
            m_OpenButton.onClick.AddListener(OnClickOpen);

        if (m_OpenButtonImage == null)
            m_OpenButtonImage = m_OpenButton.GetComponent<Image>();

    }

    //** 데이터 및 UI 세팅
    public void SetData(UIFranchiseRoom roomData)
    {
        m_nFloor = roomData.m_nFloor;
        m_nBuildingNum = roomData.m_owner.m_nBuildingNumber;

        // Main
        m_TitleText.text = Languages.ToString(TEXT_UI.FLOOR, FindBuildingName(roomData.m_owner.m_nBuildingNumber), roomData.m_nFloor);
        bool openAble = roomData.IsOpenAble();
        m_OpenButton.interactable = openAble;
        m_OpenButtonImage.sprite = openAble ? TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_02") : TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");
        m_OpenButtonOutLine.effectColor = openAble ? m_OpenAbleOutLineColor : m_OpenEnAbleOutLineColor;
        m_OpenButtonShadow.effectColor = openAble ? m_OpenAbleShadowColor : m_OpenEnAbleShadowColor;
        m_OpenButtonText.text = Languages.ToString(TEXT_UI.EXPANSION);

        // Left
        TEXT_UI decString       = GetSpeak(m_nBuildingNum);
        m_DecText.text          = Languages.ToString(decString, 0, Languages.ToString(roomData.m_eRevGoodsType), roomData.m_nRevGoodsCount);
        //m_DecText.text          = Languages.ToString(TEXT_UI.FRANCHISE_REWARD_INFO, Languages.ToString(roomData.m_eRevGoodsType), roomData.m_nRevGoodsCount);
        m_CoolTimeText.text     = Languages.ToString(TEXT_UI.TIME_RWQUIRED);

        float hour = roomData.m_fCoolTime / 3600.0f;
        float minute = (roomData.m_fCoolTime % 3600.0f) / 60.0f;
        float sec = (roomData.m_fCoolTime % 3600.0f) % 60.0f;
        m_CoolTimeValueText.text = string.Format("{0:00} : {1:00} : {2:00}", hour, minute, sec);

        m_RoomImage.sprite = roomData.m_RoomImg.sprite;
        SetSkeletonCharater(roomData.m_CharIndex);

        string tableImageName = string.Format("Building0{0}_Table", roomData.m_owner.m_nBuildingNumber);
        Sprite tableSprite = TextureManager.GetSprite(SpritePackingTag.Franchise, tableImageName);

        m_RoomTableImg.gameObject.SetActive(tableSprite);

        if (tableSprite != null)
        {
            m_RoomTableImg.sprite = tableSprite;
            m_RoomTableImg.SetNativeSize();
        }

        // Right
        m_RightTitleText.text       = Languages.ToString(TEXT_UI.FLOOR_OPEN_TERMS);
        m_LevelText.text            = Languages.ToString(TEXT_UI.ACCOUNT_LEVEL);
        m_LevelValueText.text       = Languages.ToString(TEXT_UI.LV) + roomData.m_OpenTrems.m_nNeedLevel.ToString();
        m_PreTermsText.text         = Languages.ToString(TEXT_UI.PRECEDE_TERMS);
        m_NeedGoodsText.text        = Languages.ToString(TEXT_UI.NEED_TERMS);

        if (roomData.m_OpenTrems.m_nNeedOpenedFloor != 0)
            m_PreTermsValueText.text = Languages.ToString(TEXT_UI.FLOOR, FindBuildingName(roomData.m_owner.m_nBuildingNumber), roomData.m_OpenTrems.m_nNeedOpenedFloor);
        else
            m_PreTermsValueText.text = Languages.ToString(TEXT_UI.NOTHING);

        // 필요 재화 세팅
        int needGoodsDataCount = roomData.m_OpenTrems.m_listNeedGoods.Count;
        for (int i = 0; i < needGoodsDataCount; i++)
        {
            if (i > m_NeedGoodsObjects.Length - 1)
            {
                Debug.LogError("[UIFranchiseInfo] SetData : m_NeedGoodsObjects need More Length" + i);
                continue;
            }

            FranchiseOpenGoods goodsTerms = roomData.m_OpenTrems.m_listNeedGoods[i];

            NeedGoodsObject needGoodsObject = m_NeedGoodsObjects[i];
            needGoodsObject.m_GoodsIcon.sprite = TextureManager.GetGoodsTypeSprite(goodsTerms.m_eNeedGoodsType);
            needGoodsObject.m_GoodsIcon.SetNativeSize();
            needGoodsObject.m_GoodsValueText.text = goodsTerms.m_nNeedGoodsCount.ToString();
            needGoodsObject.m_GoodsObject.SetActive(true);
        }

        // 불필요 오브젝트 엑티브 끄기
        int enActiveObjectCount = m_NeedGoodsObjects.Length - needGoodsDataCount;
        for (int i = 0; i < enActiveObjectCount; i++)
        {
            NeedGoodsObject needGoodsObject = m_NeedGoodsObjects[(m_NeedGoodsObjects.Length - i) -1];
            needGoodsObject.m_GoodsObject.SetActive(false);
        }

        SetNotEnoughTerms(roomData.m_OpenTrems);
    }

    private string FindBuildingName(int buildingNum)
    {
        switch (buildingNum)
        {
            case 1: return Languages.ToString(TEXT_UI.FRANCHISE_CHINA);
            case 2: return Languages.ToString(TEXT_UI.FRANCHISE_JAPEN);
            case 3: return Languages.ToString(TEXT_UI.FRANCHISE_KOREA);
            case 4: return Languages.ToString(TEXT_UI.FRANCHISE_ITALY);
            case 5: return Languages.ToString(TEXT_UI.FRANCHISE_FASTFOOD);
            default: return Languages.ToString(TEXT_UI.FRANCHISE_CHINA);
        }
    }

    //** 조건 충족에 따른 Text 컬러변경
    private void SetNotEnoughTerms(FranchiseOpenTerms termsData)
    {
        string strRedColor = "<color=#FF4949FF>{0}</color>";
        string strNormalColor = "<color=#665E57FF>{0}</color>";
        string strCountNormalColor = "<color=#FFFFFFFF>{0}</color>";
        string strColor = "";

        // 레벨 충족
        strColor = termsData.m_nNeedLevel > Kernel.entry.account.level ? strRedColor : strNormalColor;
        m_LevelValueText.text = string.Format(strColor, m_LevelValueText.text);

        // 이전 층 오픈 충족
        strColor = !termsData.m_bNeedFloorOpen ? strRedColor : strNormalColor;
        m_PreTermsValueText.text = string.Format(strColor, m_PreTermsValueText.text);

        // 재화 충족
        for(int i = 0; i < termsData.m_listNeedGoods.Count; i++)
        {
            FranchiseOpenGoods terms = termsData.m_listNeedGoods[i];
            NeedGoodsObject goodsObject = m_NeedGoodsObjects[i];

            strColor = terms.m_nNeedGoodsCount > Kernel.entry.account.GetValue(terms.m_eNeedGoodsType) ? strRedColor : strCountNormalColor;
            goodsObject.m_GoodsValueText.text = string.Format(strColor, goodsObject.m_GoodsValueText.text);
        }
    }

    //** 캐릭터 세팅
    private bool SetSkeletonCharater(int charIndex)
    {
        if (charIndex > 0)
        {
            DB_Card.Schema charData = DB_Card.Query(DB_Card.Field.Index, charIndex);

            if (charData == null)
            {
                Debug.LogError(string.Format("[UIFranchiseRoom] SetSkeletonCharater : charData(DB_Card index = {0}) is Null or Zero", charIndex));
                return false;
            }

            string identificationName = charData.IdentificationName;
            string assetPath = "Spines/Character/" + identificationName + "/" + identificationName + "_SkeletonData";
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                m_SkeletonAnim.skeletonDataAsset = skeletonDataAsset;
                m_SkeletonAnim.initialSkinName = identificationName;
                m_SkeletonAnim.AnimationName = "wait";
                m_SkeletonAnim.loop = true;
                m_SkeletonAnim.Reset();

                return true;
            }
            else
            {
                Debug.LogError(assetPath);
            }
        }

        return false;
    }

    //** 랜덤 Speak string 반환.
    private TEXT_UI GetSpeak(int buildingNum)
    {
        if (Kernel.entry == null)
            return TEXT_UI.FRANCHISE;

        List<TEXT_UI> speakList =  Kernel.entry.franchise.FindBuildingSpeak(buildingNum);

        if (speakList == null)
            return TEXT_UI.FRANCHISE;

        int randomNum = UnityEngine.Random.Range(0, speakList.Count);

        return speakList[randomNum];
    }

    //** 확장 버튼
    private void OnClickOpen()
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.FLOOR_OPEN_TERMS_INFO, m_nFloor), UIAlerter.Composition.Confirm_Cancel,OnResponseCallback, Languages.ToString(TEXT_UI.FLOOR_OPEN));
    }

    //** 확장 버튼 눌렀을때 콜백
    private void OnResponseCallback(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
            return;

        UIManager.Instance.Close(this.ui);
        Kernel.entry.franchise.REQ_PACKET_CG_GAME_OPEN_FRANCHISE_BUILDING_FLOOR_SYN((byte)m_nFloor, m_nBuildingNum);
    }
}
