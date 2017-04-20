using Common.Packet;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UICardCharInfo : UICardInfoComponent
{
    private TextLevelMaxEffect m_LevelMaxEffect;
    public Image m_ClassImage;
    public Image m_BackgroundImage;
    public Image m_FrameImage;
    public Image m_PortraitImage;
    public Image m_GradeImage;
    public Text m_LevelText;
    public Text m_NameText;
    public Text m_HPText;
    public Text m_HPAddText;
    public Text m_APText;
    public Text m_APAddText;
    public Text m_DPText;
    public Text m_DPAddText;
    public AnimationEventHandler m_AnimationEventHandler;
    public Animator m_Level_txt_Animation;
    public GameObject m_CardLevelUpFX;
    public AnimationEventHandler m_LevelUpTextAnimationEventHandler;

    bool m_Group0;
    bool m_Group1;
    bool m_Group2;

    protected override void Awake()
    {
        base.Awake();

        m_AnimationEventHandler.onAnimationEventCallback += OnAnimationEvent;
        m_LevelUpTextAnimationEventHandler.onAnimationEventCallback += OnAnimationEvent;
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardLevelUp += OnCardLevelUp;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.entry != null)
        {
            Kernel.entry.character.onCardLevelUp -= OnCardLevelUp;
        }
    }

    #region Properties
    public override long cid
    {
        get
        {
            return base.cid;
        }

        set
        {
            //if (m_CID != value)
            {
                base.cid = value;

                CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
                if (cardInfo != null)
                {
                    level = cardInfo.m_byLevel;

                    CSoulInfo soulInfo = Kernel.entry.character.FindSoulInfo(cardInfo.m_iCardIndex);
                    if (soulInfo != null)
                    {
                        ticket = soulInfo.m_iSoulCount;
                    }

                    DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardInfo.m_iCardIndex);
                    if (card != null)
                    {
                        DB_CardLevelUp.Schema cardLevelUp = DB_CardLevelUp.Query(DB_CardLevelUp.Field.Grade_Type,
                                                                                 card.Grade_Type,
                                                                                 DB_CardLevelUp.Field.CardLevel,
                                                                                 cardInfo.m_byLevel);
                        if (cardLevelUp != null)
                        {
                            requiredTicket = cardLevelUp.Count;
                            requiredGold = cardLevelUp.Need_Gold;
                        }
                    }

                    int hp, ap, dp;
                    Settings.Card.TryGetStat(cardInfo.m_iCardIndex, cardInfo.m_byLevel, out hp, out ap, out dp);

                    m_PortraitImage.sprite = TextureManager.GetPortraitSprite(cardInfo.m_iCardIndex);
                    m_ClassImage.sprite = TextureManager.GetClassTypeIconSprite(card.ClassType);
                    m_BackgroundImage.sprite = TextureManager.GetGradeTypeBackgroundSprite(card.Grade_Type);
                    m_FrameImage.sprite = TextureManager.GetGradeTypeFrameSprite(card.Grade_Type);
                    m_GradeImage.sprite = TextureManager.GetGradeTypeSprite(card.Grade_Type);
                    m_LevelText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level);
                    m_NameText.text = Languages.FindCharName(cardInfo.m_iCardIndex);
                    m_HPText.text = Languages.ToString<int>(hp);
                    m_APText.text = Languages.ToString<int>(ap);
                    m_DPText.text = Languages.ToString<int>(dp);
                    if (levelUpAvailable)
                    {
                        int addHP, addAP, addDP;
                        Settings.Card.TryGetStat(cardInfo.m_iCardIndex, (byte)(cardInfo.m_byLevel + 1), out addHP, out addAP, out addDP);

                        m_HPAddText.text = Languages.ToString<int>(addHP - hp);
                        m_APAddText.text = Languages.ToString<int>(addAP - ap);
                        m_DPAddText.text = Languages.ToString<int>(addDP - dp);
                    }

                    if (m_LevelText != null && m_LevelMaxEffect == null)
                        m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

                    if (m_LevelMaxEffect != null)
                        m_LevelMaxEffect.MaxValue = maxLevel;

                    if (m_LevelMaxEffect != null)
                        m_LevelMaxEffect.Value = level;

                    m_HPAddText.gameObject.SetActive(levelUpAvailable);
                    m_APAddText.gameObject.SetActive(levelUpAvailable);
                    m_DPAddText.gameObject.SetActive(levelUpAvailable);
                    m_Slider.maxValue = requiredTicket;
                    m_Slider.value = ticket;
                    UpdateLevelUpButton();
                }
            }
        }
    }

    protected override byte maxLevel
    {
        get
        {
            return Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Card_Level_Limit);
        }
    }
    #endregion

    protected override void LevelUp()
    {
        Kernel.entry.character.REQ_PACKET_CG_CARD_LEVEL_UP_SYN(m_CID);
    }

    protected override void Initialize()
    {
        base.Initialize();

        #region
        m_Level_txt_Animation.transform.localScale = Vector3.one;
        m_AnimationEventHandler.transform.localScale = Vector3.one;
        ((RectTransform)m_AnimationEventHandler.transform).anchoredPosition = new Vector2(0f, -234f);
        #endregion
        m_CardLevelUpFX.gameObject.SetActive(false);
        m_HPText.transform.localScale = Vector3.one;
        m_HPAddText.color = new Color(m_HPAddText.color.r, m_HPAddText.color.g, m_HPAddText.color.b, 1f);
        m_APText.transform.localScale = Vector3.one;
        m_APAddText.color = new Color(m_APAddText.color.r, m_APAddText.color.g, m_APAddText.color.b, 1f);
        m_DPText.transform.localScale = Vector3.one;
        m_DPAddText.color = new Color(m_DPAddText.color.r, m_DPAddText.color.g, m_DPAddText.color.b, 1f);
        m_Group0 = m_Group1 = m_Group2 = false;
    }

    void OnCardLevelUp(long cid)
    {
        if (m_CID != cid)
        {
            return;
        }

        StartCoroutine(Animation());
    }

    void OnAnimationEvent(string value)
    {
        if (Equals("Level_txt_Animation", value))
        {
            m_Level_txt_Animation.ResetTrigger("Normal");
            m_Level_txt_Animation.SetTrigger("Level_txt_Animation");
            m_LevelText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level + 1);
        }
        else if (Equals("Card_levelup_ani", value))
        {
            cid = m_CID;

            m_HPAddText.color = new Color(m_HPAddText.color.r, m_HPAddText.color.g, m_HPAddText.color.b, 1f);
            m_APAddText.color = new Color(m_APAddText.color.r, m_APAddText.color.g, m_APAddText.color.b, 1f);
            m_DPAddText.color = new Color(m_DPAddText.color.r, m_DPAddText.color.g, m_DPAddText.color.b, 1f);
        }
        else if (Equals("finish", value))
        {
            interactable = true;
        }
    }

    IEnumerator Animation()
    {
        interactable = false;

        m_AnimationEventHandler.animator.ResetTrigger("Normal");
        m_AnimationEventHandler.animator.SetTrigger("Card_levelup_ani");
        m_CardLevelUpFX.SetActive(true);

        float deltaTime = 0f, normalizedValue = 0f;
        while (deltaTime <= m_SliderAnimationDuration)
        {
            normalizedValue = Mathf.Clamp01(1f - (float)PennerDoubleAnimation.QuintEaseOut(deltaTime, 0f, 1f, m_SliderAnimationDuration));
            if (float.IsNaN(normalizedValue))
            {
                normalizedValue = 0f;
            }
            m_Slider.normalizedValue = normalizedValue;

            if (deltaTime > m_SliderAnimationDuration * 0.25f && !m_Group0)
            {
                m_Group0 = true;
                StartCoroutine(Group(m_HPText, m_HPAddText, m_SliderAnimationDuration * 0.25f));
            }

            if (deltaTime > m_SliderAnimationDuration * 0.325f && !m_Group1)
            {
                m_Group1 = true;
                StartCoroutine(Group(m_APText, m_APAddText, m_SliderAnimationDuration * 0.25f));
            }

            if (deltaTime > m_SliderAnimationDuration * 0.5f && !m_Group2)
            {
                m_Group2 = true;
                StartCoroutine(Group(m_DPText, m_DPAddText, m_SliderAnimationDuration * 0.25f));
            }

            deltaTime = deltaTime + Time.deltaTime;

            yield return 0;
        }

        m_CardLevelUpFX.SetActive(false);
        m_Group0 = m_Group1 = m_Group2 = false;
        //interactable = true;

        yield break;
    }

    IEnumerator Group(Text a, Text b, float duration)
    {
        float deltaTime = 0f, localScale = 0f, alpha = 0f;
        while (deltaTime <= duration)
        {
            localScale = Mathf.Max((float)PennerDoubleAnimation.ElasticEaseOut(deltaTime, 0f, 1f, duration), 1f);
            a.rectTransform.localScale = new Vector3(localScale, localScale, 1f);

            alpha = Mathf.Clamp01(1f - (float)PennerDoubleAnimation.ExpoEaseOut(deltaTime, 0f, 1f, duration * 0.25f));
            b.color = new Color(b.color.r, b.color.g, b.color.b, alpha);

            deltaTime = deltaTime + Time.deltaTime;

            yield return 0;
        }

        yield break;
    }
}
