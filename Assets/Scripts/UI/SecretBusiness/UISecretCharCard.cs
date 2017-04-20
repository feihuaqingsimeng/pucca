using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class SecretEmptyCard
{
    public GameObject   m_Empty;
    public Image        m_EmptyClassTypeIcon;

}

[System.Serializable]
public class SecretCharCard
{
    public TextLevelMaxEffect m_LevelMaxEffect;

    public GameObject   m_CharCard;
    public GameObject   m_Select;
    public GameObject   m_StarObjs;
    public GameObject   m_SlotInputEffect;

    public Image        m_CharImage;
    public Image        m_GradeTypeFrame;
    public Image        m_GradeTypeBackGround;
    public Image        m_GradeTypeIcon;
    public Image[]      m_GradeStarIcon;
    public Image        m_ClassTypeIcon;

    public Text         m_ManaCount;
    public Text         m_CharLevel;

    public UISlider m_Slider;
    
}

public enum eCardType
{
    CT_NONE,
    CT_NEED_SELECT_CARD,
    CT_HAVE_CARD,
    CT_CARD_INFO,
}

public class UISecretCharCard : MonoBehaviour 
{
    [HideInInspector]
    public  CharCardData    m_CharCardData;
    [HideInInspector]
    public  SecretSlotData  m_SlotData;

    private eCardType m_eCardType = eCardType.CT_NONE;

    [Space(10)]
    public  SecretEmptyCard m_EmptyCard;
    public  SecretCharCard  m_CharCard;

    public  Button          m_CardButton;

    private int m_nHaveCount;
    private int haveCount
    {
        get { return m_nHaveCount; }
        set
        {
            //m_NeedCount.text = string.Format("{0} / {1}", value, m_nNeedCount);
            m_nHaveCount = value;
            m_CharCard.m_Slider.value = value;
            m_EmptyCard.m_Empty.SetActive(value == 0);
            m_CharCard.m_CharCard.SetActive(value > 0);
        }
    }

    private void Awake()
    {
        if(m_CardButton != null)
            m_CardButton.onClick.AddListener(OnClickCard);
    }

    private void OnDestroy()
    {
        if (m_CardButton != null)
            m_CardButton.onClick.RemoveAllListeners();
    }

    //** 처음으로 카드 생성시 기본 세팅.(선택 슬롯, 보유 카드)
    public void SetCard(SecretSlotData slotData, CharCardData charCardData, eCardType cardType)
    {
        if (slotData != null)
        {
            m_SlotData = slotData;
            m_CharCard.m_Slider.maxValue = m_SlotData.m_nNeedCount;
        }

        m_CharCard.m_Select.SetActive(false);

        if(m_CharCard.m_SlotInputEffect != null)
            m_CharCard.m_SlotInputEffect.SetActive(false);

        m_CharCardData  = charCardData;
        m_eCardType     = cardType;
        haveCount = 0;

        if(charCardData == null)
            SetEmptyCard();
        else
            SetChoiceCard(charCardData);

        // ref. PUC-883
        bool interactable = true;
        if (cardType == eCardType.CT_HAVE_CARD &&
            slotData != null &&
            charCardData != null)
        {
            interactable = (slotData.m_nNeedCount <= charCardData.m_nHaveCount);
            m_CharCard.m_CharCard.SetActive(true);
        }
        m_CardButton.interactable = interactable;
    }

  
    //** 선택 전 빈 카드의 UI 및 Data 세팅
    public void SetEmptyCard()
    {
        SetGradeAndClassUI(m_SlotData.m_eGradeType, m_SlotData.m_eClassType, true);
        haveCount = 0;
    }

    public void SetSlotInputCard(CharCardData cardData)
    {
        if (m_CharCard.m_SlotInputEffect != null)
        {
            if (m_CharCard.m_SlotInputEffect.activeSelf)
                m_CharCard.m_SlotInputEffect.SetActive(false);

            m_CharCard.m_SlotInputEffect.SetActive(true);
        }
        
        SetChoiceCard(cardData);
    }

    //** 선택된 카드와 보유 카드 UI 및 Data 세팅
    public void SetChoiceCard(CharCardData cardData)
    {
        m_CharCardData  = cardData;
        haveCount       = cardData.m_nHaveCount;
        // Grade and Class
        SetGradeAndClassUI(cardData.m_CardData.Grade_Type, cardData.m_CardData.ClassType, false);

        // skill Mana Cost
        DB_Skill.Schema skill = DB_Skill.Query(DB_Skill.Field.Index, cardData.m_CardData.Index, DB_Skill.Field.SkillType, SkillType.Active);
        m_CharCard.m_ManaCount.text = ((skill != null) ? skill.Cost : 0).ToString();

        // char Img
        m_CharCard.m_CharImage.sprite = TextureManager.GetPortraitSprite(cardData.m_CardData.Index);

        // level
        byte level = cardData.m_CardInfo == null ? (byte)1 : cardData.m_CardInfo.m_byLevel;
        m_CharCard.m_CharLevel.text = Languages.LevelString(level);

        if (m_CharCard.m_CharLevel != null)
            m_CharCard.m_LevelMaxEffect = m_CharCard.m_CharLevel.GetComponent<TextLevelMaxEffect>();

        if (m_CharCard.m_LevelMaxEffect != null)
            m_CharCard.m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Card_Level_Limit);

        if (m_CharCard.m_LevelMaxEffect != null)
            m_CharCard.m_LevelMaxEffect.Value = level;

        // star
        if (cardData.m_CardInfo == null)
            return;

        if (m_CharCard.m_GradeStarIcon != null && m_CharCard.m_GradeStarIcon.Length > 0)
        {
            // 11.5f : (23f - 3f) * .5f
            // 23f : Image.rectTransform.rect.width
            // 3f : GridLayoutGroup.spacing.y

            byte byteSkill = cardData.m_CardInfo.m_bySkill;
            float anchoredPositionX = (byteSkill != 0) ? -((byteSkill - 1) * 11.5f) : 0f;
            for (int i = 0; i < m_CharCard.m_GradeStarIcon.Length; i++)
            {
                Image image = m_CharCard.m_GradeStarIcon[i];
                if (image == null)
                    continue;

                image.enabled = (i < byteSkill);
                image.rectTransform.anchoredPosition = new Vector2(anchoredPositionX, image.rectTransform.anchoredPosition.y);
                anchoredPositionX = anchoredPositionX + 23f;
            }
        }
    }

    public void SetLegendCard(CharCardData charCardData)
    {
        // 쓰이지 않는 것들은 꺼준다.
        m_CharCard.m_Slider.gameObject.SetActive(false);
        m_CharCard.m_Select.gameObject.SetActive(false);
        m_CharCard.m_CharLevel.gameObject.SetActive(false);
        m_CharCard.m_GradeTypeIcon.gameObject.SetActive(false);
        m_CharCard.m_StarObjs.SetActive(false);

        if (m_CharCard.m_SlotInputEffect != null)
            m_CharCard.m_SlotInputEffect.SetActive(false);
        

        m_eCardType = eCardType.CT_CARD_INFO;

        SetChoiceCard(charCardData);
    }


    //** 등급 및 직업 UI 세팅
    private void SetGradeAndClassUI(Grade_Type gradeType, ClassType classType, bool empty)
    {
        m_EmptyCard.m_EmptyClassTypeIcon.sprite = TextureManager.GetClassTypeIconSprite(classType != ClassType.None ? classType : ClassType.ClassType_Healer);
        m_EmptyCard.m_EmptyClassTypeIcon.gameObject.SetActive(classType != ClassType.None);

        m_CharCard.m_GradeTypeIcon.sprite = TextureManager.GetGradeTypeSprite(gradeType);

        if (empty)
            return;

        m_CharCard.m_ClassTypeIcon.sprite = TextureManager.GetClassTypeIconSprite(classType != ClassType.None ? classType : ClassType.ClassType_Healer);
        m_CharCard.m_GradeTypeFrame.sprite = TextureManager.GetGradeTypeFrameSprite(gradeType);
        m_CharCard.m_GradeTypeBackGround.sprite = TextureManager.GetGradeTypeBackgroundSprite(gradeType);
    }

    //** 선택 마크 표기
    public void SetSelectCardMark(CharCardData charCardData)
    {
        if (m_eCardType != eCardType.CT_HAVE_CARD)
            return;

        m_CharCard.m_Select.SetActive(m_CharCardData == charCardData && !m_CharCard.m_Select.activeSelf);
    }

    //** 카드를 선택 버튼
    private void OnClickCard()
    {
        SecretBusiness secret = Kernel.entry.secretBusiness;

        if (secret == null)
            return;

        switch (m_eCardType)
        {
            case eCardType.CT_NONE: break;
            case eCardType.CT_NEED_SELECT_CARD:
                if (secret.onClickSlotCardCallBack != null)
                    secret.onClickSlotCardCallBack(m_SlotData.m_nSlotIndex);
                break;
            case eCardType.CT_HAVE_CARD:
                if (secret.onClickHaveCardCallBack != null)
                    secret.onClickHaveCardCallBack(m_CharCardData);
                break;
            case eCardType.CT_CARD_INFO:
                if (secret.onClickCardInfoCallBack != null)
                    secret.onClickCardInfoCallBack(m_CharCardData);
                break;
        }
    }
}
