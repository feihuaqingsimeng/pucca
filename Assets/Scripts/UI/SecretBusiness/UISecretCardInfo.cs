using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Common.Packet;

public class UISecretCardInfo : UIObject 
{
    [Space(10)]
    public UISecretCharCard m_CharCardPrefab;
    public UITooltipObject  m_LeaderSkillTooltipObject;
    //public UITooltipObject  m_FeatureTooltipObject;
    
    public Image m_GradeIcon;

    public Text m_LevelAndName;
    public Text m_CharInfoDec;
    

    //** State Info
    [Header("State")]
    public Text m_HealthTitle;
    public Text m_AttackTitle;
    public Text m_ShieldTitle;
    public Text m_HealthCount;
    public Text m_AttackCount;
    public Text m_ShieldCount;

    //** Skill Info
    [Header("Skill")]
    public Text m_LeaderSkillTitle;
    public Text m_FeatureSkillTitle;
    public Text m_MainSkillTitle;
    public Text m_LeaderSkillName;
    public Text m_FeatureSkillName;
    public Text m_MainSkillName;
    public Text m_MainSkillDec;
    public Text m_MainSkillManaCount;
    public Text m_SkillCost;


    protected override void Awake()
    {
        SetUI();
        base.Awake();
    }

    //** 팝업창의 기본 UI 세팅
    public void SetUI()
    {
        m_CharInfoDec.text          = Languages.ToString(TEXT_UI.CARD_DECK_GET, Languages.ToString(TEXT_UI.SECRET_EXCHANGE));

        m_HealthTitle.text          = Languages.ToString(TEXT_UI.HP);
        m_AttackTitle.text          = Languages.ToString(TEXT_UI.AP);
        m_ShieldTitle.text          = Languages.ToString(TEXT_UI.DP);

        m_LeaderSkillTitle.text     = Languages.ToString(TEXT_UI.LEADER);
        m_FeatureSkillTitle.text    = Languages.ToString(TEXT_UI.FEATURE);
        m_MainSkillTitle.text       = Languages.ToString(TEXT_UI.MAIN);

    }
    
    //** 기본 정보 세팅 (캐릭터 아이콘, 캐릭터 레벨 및 이름, 캐릭터 등급 아이콘, 캐릭터 등급 Frame)
    public void SetCharBase(CharCardData charCardData)
    {
        if (charCardData == null)
            return;

        m_CharCardPrefab.SetLegendCard(charCardData);

        m_GradeIcon.sprite = TextureManager.GetGradeTypeSprite(charCardData.m_CardData.Grade_Type);
        m_LevelAndName.text = string.Format("{0} {1}", Languages.LevelString(1), Languages.FindCharName(charCardData.m_CardData.Index));
        m_LevelAndName.GetComponent<RectTransform>().sizeDelta = new Vector2(m_LevelAndName.preferredWidth, 40.0f);

        SetState(charCardData.m_CardData);
        SetSkill(charCardData.m_CardData);
    }

    //** 캐릭터 State 세팅
    public void SetState(DB_Card.Schema baseCardData)
    {
        m_HealthCount.text = baseCardData.LvBase_Hp.ToString();
        m_AttackCount.text = baseCardData.LvBase_Ap.ToString();
        m_ShieldCount.text = baseCardData.LvBase_Dp.ToString();
    }

    //** 캐릭터 스킬 (리더, 특징, 메인) 세팅
    public void SetSkill(DB_Card.Schema baseCardData)
    {
        m_LeaderSkillTooltipObject.content = Languages.GetSkillToolTip(baseCardData.Index, 1, SkillType.Leader);
        m_LeaderSkillName.text = Languages.FindSkillName(baseCardData.Index, SkillType.Leader);

        //m_FeatureTooltipObject.content = string.Empty;

        m_MainSkillName.text = Languages.FindSkillName(baseCardData.Index, SkillType.Active);
        m_MainSkillDec.text = Languages.GetSkillExplain(baseCardData.Index, 1, SkillType.Active);

        DB_Skill.Schema skill = DB_Skill.Query(DB_Skill.Field.Index, baseCardData.Index, DB_Skill.Field.SkillType, SkillType.Active);

        if (skill != null)
            m_SkillCost.text = skill.Cost.ToString();
    }

    // ref. PUC-870
    // Invoked by UIDeck.OnCharCardClick()
    public void SetCharBaseData(DB_Card.Schema card)
    {
        if (card != null)
        {
            SetCharBase(new CharCardData()
            {
                m_CardData = card,
            });
            m_CharCardPrefab.m_CharCard.m_CharCard.gameObject.SetActive(true);
            m_CharInfoDec.text = Languages.ToString(TEXT_UI.CARD_DECK_AREA_GET, card.Drop_Area > 0 ? card.Drop_Area.ToString() : Languages.ToString(TEXT_UI.SECRET_EXCHANGE));
        }
    }
}
