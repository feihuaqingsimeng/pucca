using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UICharCard : MonoBehaviour
{
    #region Variables
    private TextLevelMaxEffect m_LevelMaxEffect;

    public Button m_Button;
    public Image m_EmptyImage;
    public Image m_BackgroundImage;
    public Image m_PortraitImage;
    public Image m_FrameImage;
    public Image m_ClassImage;
    public Text m_SkillCostText;
    public Text m_LevelText;
    public Image m_NameBackgroundImage;
    public Text m_NameText;
    public List<Image> m_StarImageList;
    public UISlider m_Slider;
    public Text m_AreaText;
    public Text m_CountText;
    public Image m_UpgradableImage;
    public Image m_UpgradableMaskImage;
    public Image m_UpgradableFrameImage;
    public Image m_UpgradableScrollImage;
    public UITooltipObject m_TooltipObject;
    public LayoutElement m_LayoutElement;
    public Image m_BadgeImage;

    int m_CardIndex;
    long m_CID;
    bool m_Empty = true;
    RectTransform m_RectTransform;
    Graphic[] m_Graphics;
    #endregion

    #region Delegates
    public delegate void OnClicked(UICharCard charCard);
    public OnClicked onClicked;
    #endregion

    #region MonoBehaviour Life Cycle
    void Awake()
    {
        if (m_Button)
        {
            m_Button.onClick.AddListener(OnClick);
        }
    }

    // Use this for initialization
    void Start()
    {
        if (m_NameBackgroundImage && m_NameBackgroundImage.gameObject.activeSelf)
        {
            m_NameBackgroundImage.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_UpgradableImage != null && m_UpgradableImage.enabled)
        {
            float value = m_UpgradableScrollImage.rectTransform.anchoredPosition.y + 1f;
            if (value > 45f) value = 0f;

            m_UpgradableScrollImage.rectTransform.anchoredPosition = new Vector2(0f, value);
        }
    }

    void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardInfoUpdate += OnCardInfoUpdate;
            Kernel.entry.account.onPromoteTicketUpdate += OnPromoteTicketUpdate;
        }
    }

    void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardInfoUpdate -= OnCardInfoUpdate;
            Kernel.entry.account.onPromoteTicketUpdate -= OnPromoteTicketUpdate;
        }
    }
    #endregion

    public int cardIndex
    {
        get
        {
            return m_CardIndex;
        }

        set
        {
            if (m_CardIndex != value)
            {
                m_CardIndex = value;

                DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, m_CardIndex);
                if (card != null)
                {
                    area = card.Drop_Area;
                    gradeType = card.Grade_Type;

                    m_ClassImage.sprite = TextureManager.GetClassTypeIconSprite(card.ClassType);
                    m_BackgroundImage.sprite = TextureManager.GetGradeTypeBackgroundSprite(card.Grade_Type);
                    m_FrameImage.sprite = TextureManager.GetGradeTypeFrameSprite(card.Grade_Type);
                    m_PortraitImage.sprite = TextureManager.GetPortraitSprite(m_CardIndex);

                    DB_Skill.Schema skill = DB_Skill.Query(DB_Skill.Field.Index, m_CardIndex, DB_Skill.Field.SkillType, SkillType.Active);
                    m_SkillCostText.text = ((skill != null) ? skill.Cost : 0).ToString();
                }

                m_NameText.text = Languages.FindCharName(m_CardIndex);
                gameObject.name = string.Format("{0} ({1}, {2})", m_NameText.text, m_CardIndex, m_CID);
                // Warning, cid에서 호출되는 경우 아래 함수들은 중복 호출되고 있습니다.
                level = 0;
                this.skill = 0;
                grayscale = true;
                soulCount = -1;
                empty = false;
                upgradable = false;
                UpdateBattlePower(0);
                isNew = false;
            }
        }
    }

    public void SetCardInfo(CCardInfo cardInfo, bool editable) // editable..?
    {
        if (cardInfo != null)
        {
            cardIndex = cardInfo.m_iCardIndex;
            level = cardInfo.m_byLevel;
            skill = cardInfo.m_bySkill;
            grayscale = false;

            if (editable)
            {
                UpdateSoulCount(m_CardIndex, cardInfo.m_byLevel);
                UpdateBattlePower(m_CID);
                UpdateLevelUpAvailable(cardInfo.m_iCardIndex, cardInfo.m_byLevel, cardInfo.m_bySkill);
            }
            else
            {
                // 임시, 컴포넌트 비활성화 처리 정리 필요!
                m_Slider.gameObject.SetActive(false);
                UpdateBattlePower(0);
                upgradable = false;
            }

            isNew = cardInfo.m_bIsNew;
        }
    }

    // 다른 유저의 CCardInfo를 받을 때 soulCount, battlePower, levelUpAvailable 처리.
    public CCardInfo cardInfo
    {
        set
        {
            if (value != null)
            {
                cardIndex = value.m_iCardIndex;
                level = value.m_byLevel;
                this.skill = value.m_bySkill;
                grayscale = false;
                UpdateSoulCount(m_CardIndex, value.m_byLevel);
                UpdateBattlePower(m_CID);
                UpdateLevelUpAvailable(value.m_iCardIndex, value.m_byLevel, value.m_bySkill);

                isNew = value.m_bIsNew;
            }
        }
    }

    public long cid
    {
        get
        {
            return m_CID;
        }

        set
        {
            //if (m_CID != value)
            {
                m_CID = value;

                if (m_CID > 0)
                {
                    CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
                    if (cardInfo != null)
                    {
                        SetCardInfo(cardInfo, true);
                        //this.cardInfo = cardInfo;
                    }
                }
                else
                {
                    this.soulCount = -1;
                }

                empty = !(m_CID > 0);
                // UIScrollRectContentActivator와 호출 순서의 문제가 있어, 임시로 여기서 비활성화합니다.
                m_NameBackgroundImage.gameObject.SetActive(false);
            }
        }
    }

    byte level
    {
        set
        {
            if (m_LevelText != null)
                //m_Level.text = (level > 0) ? string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level) : string.Empty;
                m_LevelText.text = (value <= 0) ? string.Empty : string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), value);

            if (m_LevelText != null && m_LevelMaxEffect == null)
                m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Card_Level_Limit);
           
            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.Value = value;
        }
    }

    byte skill
    {
        set
        {
            if (m_StarImageList != null && m_StarImageList.Count > 0)
            {
                // 11.5f : (23f - 3f) * .5f
                // 23f : Image.rectTransform.rect.width
                // 3f : GridLayoutGroup.spacing.y
                float anchoredPositionX = (value != 0) ? -((value - 1) * 11.5f) : 0f;
                for (int i = 0; i < m_StarImageList.Count; i++)
                {
                    Image image = m_StarImageList[i];
                    if (image != null)
                    {
                        image.enabled = (i < value);
                        image.rectTransform.anchoredPosition = new Vector2(anchoredPositionX, image.rectTransform.anchoredPosition.y);
                        anchoredPositionX = anchoredPositionX + 23f;
                    }
                }
            }
        }
    }

    // Slider.maxValue 값이 변경되었는지 알 수 없기 때문에, Slider.value 보다 Slider.maxValue 값을 먼저 변경해야 합니다.
    // (UISlider 컴포넌트는 Slider.value 프로퍼티를 오버라이드하여 사용)
    int soulCount
    {
        set
        {
            if (m_Slider != null)
            {
                m_Slider.value = value;
                m_Slider.gameObject.SetActive(value > -1);
            }
        }
    }

    // Slider.maxValue 값이 변경되었는지 알 수 없기 때문에, Slider.value 보다 Slider.maxValue 값을 먼저 변경해야 합니다.
    // (UISlider 컴포넌트는 Slider.value 프로퍼티를 오버라이드하여 사용)
    int maxSoulCount
    {
        set
        {
            if (m_Slider != null)
            {
                m_Slider.maxValue = value;
            }
        }
    }

    bool grayscale
    {
        set
        {
            foreach (Graphic graphic in graphics)
            {
                graphic.material = value ? UIUtility.transparentMaterial : null;
            }

            if (m_AreaText != null)
            {
                DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, m_CardIndex);
                if (card != null)
                {
                    if (card.Drop_Area > 0)
                    {
                        m_AreaText.text = value ? string.Format("{0} {1}", Languages.ToString(TEXT_UI.CARD_DECK_AREA), card.Drop_Area) : string.Empty;
                    }
                    else
                    {
                        m_AreaText.text = value ? Languages.ToString(TEXT_UI.SECRET_EXCHANGE) : string.Empty;
                    }
                }
            }
        }
    }

    bool empty
    {
        get
        {
            return m_Empty;
        }

        set
        {
            if (m_Empty != value)
            {
                m_Empty = value;
            }

            foreach (Graphic graphic in graphics)
            {
                if (graphic != null)
                {
                    if (graphic.transform.IsChildOf(m_Slider.transform))
                        continue;

                    graphic.gameObject.SetActive(Equals(graphic, m_EmptyImage) ? value : !value);
                }
            }
        }
    }

    bool upgradable
    {
        set
        {
            if (m_UpgradableImage != null)
            //if (m_UpgradableImage != null && m_UpgradableImage.enabled != value)
            {
                m_UpgradableImage.enabled = value;
                m_UpgradableMaskImage.enabled = value;
                m_UpgradableFrameImage.enabled = value;
                m_UpgradableScrollImage.enabled = value;
            }
        }
    }

    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = transform as RectTransform;
            }

            return m_RectTransform;
        }
    }

    Graphic[] graphics
    {
        get
        {
            if (m_Graphics == null || m_Graphics.Length == 0)
            {
                m_Graphics = GetComponentsInChildren<Graphic>(true);
            }

            return m_Graphics;
        }
    }

    public CBoxResult boxResult
    {
        set
        {
            if (value != null)
            {
                cardIndex = value.m_iCardIndex;
                grayscale = false;
                m_CountText.text = string.Format("x{0}", Languages.ToString<int>(value.m_iCardCount));
                UpdateSoulCount(value.m_iCardIndex);

                // 임시 처리
                isNew = !Kernel.entry.character.IsDirected(value.m_iCardIndex);
                Kernel.entry.character.Directed(value.m_iCardIndex);
                //Debug.Log(gameObject.name + ", isNew :" + isNew);
                /*
                CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(value.m_iCardIndex);
                if (cardInfo != null)
                {
                    isNew = cardInfo.m_bIsNew;
                }
                else Debug.LogError(value.m_iCardIndex);
                */
            }
        }
    }

    public List<RectTransform> children
    {
        get
        {
            List<RectTransform> rectTransforms = new List<RectTransform>();
            for (int i = 0; i < transform.childCount; i++)
            {
                rectTransforms.Add((RectTransform)transform.GetChild(i));
            }

            return rectTransforms;
        }
    }

    public bool ignoreLayout
    {
        get
        {
            return m_LayoutElement ? m_LayoutElement.ignoreLayout : false;
        }

        set
        {
            if (m_LayoutElement)
            {
                m_LayoutElement.ignoreLayout = value;
            }
        }
    }

    public int area
    {
        get;
        private set;
    }

    public Grade_Type gradeType
    {
        get;
        private set;
    }

    public bool isNew
    {
        get
        {
            return (m_BadgeImage != null) ? m_BadgeImage.enabled : false;
        }

        set
        {
            if (m_BadgeImage != null
                && m_BadgeImage.enabled != value)
            {
                m_BadgeImage.enabled = value;
            }
        }
    }

    void UpdateLevelUpAvailable(int cardIndex, byte level, byte skillLevel)
    {
        bool cardLevelUpAvailable = Kernel.entry.character.CardLevelUpAvailable(cardIndex, level);
        bool skillLevelUpAvailable = Kernel.entry.character.SkillLevelUpAvailable(cardIndex, skillLevel);
        bool equipmentLevelUpAvailable = Kernel.entry.character.EquipmentLevelUpAvailable(cardIndex, Goods_Type.EquipUpAccessory);
        equipmentLevelUpAvailable = equipmentLevelUpAvailable ||
                                    Kernel.entry.character.EquipmentLevelUpAvailable(cardIndex, Goods_Type.EquipUpArmor);
        equipmentLevelUpAvailable = equipmentLevelUpAvailable ||
                                    Kernel.entry.character.EquipmentLevelUpAvailable(cardIndex, Goods_Type.EquipUpWeapon);

        upgradable = cardLevelUpAvailable || skillLevelUpAvailable || equipmentLevelUpAvailable;
    }

    void UpdateBattlePower(long cid)
    {
        if (cid > 0 && m_TooltipObject != null)
        {
            CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(cid);
            if (cardInfo != null)
            {
                int battlePower;
                if (Settings.Card.TryGetBattlePower(cardInfo.m_iCardIndex,
                                                    cardInfo.m_byLevel,
                                                    cardInfo.m_byAccessoryLV,
                                                    cardInfo.m_byWeaponLV,
                                                    cardInfo.m_byArmorLV,
                                                    cardInfo.m_bySkill,
                                                    out battlePower))
                {
                    m_TooltipObject.content = string.Format("{0} : {1}", Languages.ToString(TEXT_UI.BATTLE_POWER), Languages.ToString<int>(battlePower));
                }
            }
        }

        if (m_TooltipObject != null)
        {
            m_TooltipObject.enabled = (cid > 0);
        }
    }

    void UpdateSoulCount(int cardIndex, byte cardLevel = 0, int soulCount = -1)
    {
        if (cardLevel.Equals(0))
        {
            CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(cardIndex);
            if (cardInfo != null)
            {
                cardLevel = cardInfo.m_byLevel;
            }
        }

        if (soulCount.Equals(-1))
        {
            CSoulInfo soulInfo = Kernel.entry.character.FindSoulInfo(cardIndex);
            if (soulInfo != null)
            {
                soulCount = soulInfo.m_iSoulCount;
            }
        }

        int maxSoulCount = 0;
        DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardIndex);
        if (card != null)
        {
            DB_CardLevelUp.Schema cardLevelUp = DB_CardLevelUp.Query(DB_CardLevelUp.Field.Grade_Type, card.Grade_Type, DB_CardLevelUp.Field.CardLevel, cardLevel);
            if (cardLevelUp != null)
            {
                maxSoulCount = cardLevelUp.Count;
            }
        }

        this.maxSoulCount = maxSoulCount;
        this.soulCount = soulCount;
    }

    void OnPromoteTicketUpdate(int attacker, int buffer, int debuffer, int defender, int ranger, int armor, int accessory, int weapon)
    {
        if (m_CID != 0)
        {
            CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
            if (cardInfo != null)
            {
                UpdateLevelUpAvailable(cardInfo.m_iCardIndex, cardInfo.m_byLevel, cardInfo.m_bySkill);
            }
        }
    }

    void OnCardInfoUpdate(long cid, int cardIndex, bool isNew)
    {
        if (m_CID != cid)
        {
            return;
        }

        this.cid = cid;
    }

    void OnClick()
    {
        if (onClicked != null)
        {
            onClicked(this);
        }
    }
}
