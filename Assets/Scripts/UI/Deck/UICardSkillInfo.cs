using Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardSkillInfo : UICardInfoComponent
{
    public Text m_LeaderSkillNameText;
    public UITooltipObject m_LeaderSkillTooltipObject;
    public Text m_FeatureText;
    public UITooltipObject m_FeatureTooltipObject;
    public List<Animator> m_StarAnimators;
    public List<AnimationEventHandler> m_AnimationEventHandlers;
    public Text m_MainSkillNameText;
    public Text m_MainSkillCostText;
    public Text m_MainSkillDescriptionText;
    public GameObject m_SkillLevelUpFX;

    bool m_Initialized;
    int m_Count;
    int m_Index;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_AnimationEventHandlers.Count; i++)
        {
            m_AnimationEventHandlers[i].onAnimationEventCallback += OnAnimationEvent;
        }
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry != null)
        {
            Kernel.entry.character.onSkillLevelUpCallback += OnSkillLevelUp;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.entry != null)
        {
            Kernel.entry.character.onSkillLevelUpCallback -= OnSkillLevelUp;
        }

        m_Initialized = false;
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
            base.cid = value;

            CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
            if (cardInfo != null)
            {
                level = cardInfo.m_bySkill;

                DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardInfo.m_iCardIndex);
                if (card != null)
                {
                    ticket = Kernel.entry.account.GetValue(card.ClassType);
                }

                DB_SkillLevelUp.Schema skillLevelUp = DB_SkillLevelUp.Query(DB_SkillLevelUp.Field.Grade_Type, card.Grade_Type,
                                                                            DB_SkillLevelUp.Field.Skill_Level, cardInfo.m_bySkill);
                if (skillLevelUp != null)
                {
                    requiredTicket = skillLevelUp.Count;
                    requiredGold = skillLevelUp.Need_Gold;
                }

                m_LeaderSkillNameText.text = Languages.FindSkillName(cardInfo.m_iCardIndex, SkillType.Leader);
                m_LeaderSkillTooltipObject.content = Languages.GetSkillToolTip(cardInfo.m_iCardIndex, cardInfo.m_byLevel, SkillType.Leader);
                //m_FeatureTooltipObject.content = string.Empty;
                m_MainSkillNameText.text = Languages.FindSkillName(cardInfo.m_iCardIndex, SkillType.Active);
                DB_Skill.Schema skill = DB_Skill.Query(DB_Skill.Field.Index, cardInfo.m_iCardIndex, DB_Skill.Field.SkillType, SkillType.Active);
                if (skill != null)
                {
                    m_MainSkillCostText.text = skill.Cost.ToString();
                }
                m_Count = m_Index = 0;
                for (int i = 0; i < m_StarAnimators.Count; i++)
                {
                    Animator animator = m_StarAnimators[i];
                    bool activeSelf = animator.gameObject.activeSelf;

                    animator.gameObject.SetActive(i < cardInfo.m_bySkill);

                    if (activeSelf != animator.gameObject.activeSelf && m_Initialized)
                    {
                        m_Count++;
                        animator.SetTrigger("Star_Animation");
                    }
                }
                m_MainSkillDescriptionText.text = Languages.GetSkillExplain(cardInfo.m_iCardIndex, cardInfo.m_bySkill, SkillType.Active);
                m_SliderIconImage.sprite = TextureManager.GetSprite(SpritePackingTag.Characteristic, string.Format("{0}_Double", card.ClassType));
                m_Slider.maxValue = skillLevelUp.Count;
                m_Slider.value = Kernel.entry.account.GetValue(card.ClassType);
                UpdateLevelUpButton();
                // 레벨 업을 통해 cid 프로퍼티를 실행한 경우, m_Initialized는 true.
                // OnAnimationEvent() 함수에서 m_LevelUpAvailable 값을 변경합니다.
                // 연출을 위해 임시 처리한 것으로, 개선이 필요합니다.
                if (m_Initialized && m_Count > 0)
                {
                    interactable = false;
                }
            }

            if (!m_Initialized)
            {
                m_Initialized = true;
            }
        }
    }

    protected override byte maxLevel
    {
        get
        {
            return Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Skill_Level_Limit);
        }
    }
    #endregion

    protected override void LevelUp()
    {
        Kernel.entry.character.REQ_PACKET_CG_CARD_SKILL_LEVEL_UP_SYN(m_CID);
    }

    protected override void Initialize()
    {
        base.Initialize();

        m_SkillLevelUpFX.SetActive(false);

        for (int i = 0; i < m_StarAnimators.Count; i++)
        {
            Animator animator = m_StarAnimators[i];
            if (animator != null && animator.isInitialized)
            {
                m_StarAnimators[i].SetTrigger("Normal");
            }
        }
    }

    void OnSkillLevelUp(long cid)
    {
        if (m_CID != cid)
        {
            return;
        }

        StartCoroutine(Animation());
    }

    void OnAnimationEvent(string value)
    {
        if (string.Equals("FX", value))
        {
            for (int i = m_StarAnimators.Count - 1; i >= 0; i--)
            {
                if (m_StarAnimators[i].gameObject.activeSelf)
                {
                    UIUtility.SetParent(m_SkillLevelUpFX.transform, m_StarAnimators[i].transform);
                    m_SkillLevelUpFX.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            m_Index++;

            if (m_Count != m_Index)
            {
                return;
            }

            m_SkillLevelUpFX.SetActive(false);
            interactable = true;
        }
    }

    IEnumerator Animation()
    {
        interactable = false;

        float deltaTime = 0f, normalizedValue = 0f;
        while (deltaTime <= m_SliderAnimationDuration)
        {
            normalizedValue = Mathf.Clamp01(1f - (float)PennerDoubleAnimation.QuintEaseOut(deltaTime, 0f, 1f, m_SliderAnimationDuration));
            if (float.IsNaN(normalizedValue))
            {
                normalizedValue = 0f;
            }
            m_Slider.normalizedValue = normalizedValue;

            deltaTime = deltaTime + Time.deltaTime;

            yield return 0;
        }

        this.cid = m_CID;

        yield break;
    }
}
