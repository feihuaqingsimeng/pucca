using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniCharCard : MonoBehaviour
{
    private TextLevelMaxEffect m_LevelMaxEffect;

    public Button m_Button;
    public Image m_Background;
    public Image m_Frame;
    public Image m_Portrait;
    public Image m_Class;
    public Text m_Level;
    public List<Image> m_Images;
    public UITooltipObject m_TooltipObject;

    int m_CardIndex;

    public delegate void OnClicked();
    public OnClicked onClicked;

    void Awake()
    {
        if (m_Button != null)
        {
            m_Button.onClick.AddListener(OnClick);
        }

        if (m_Level != null)
            m_LevelMaxEffect = m_Level.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Card_Level_Limit);
    }

    // Use this for initialization

    // Update is called once per frame

    public void SetCardInfo(CCardInfo cardInfo)
    {
        if (cardInfo != null)
        {
            SetCardInfo(cardInfo.m_iCardIndex, cardInfo.m_byLevel, cardInfo.m_bySkill);

            if (m_TooltipObject != null)
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
    }

    public void SetCardInfo(int cardIndex, byte level = 0, byte skill = 0)
    {
        m_CardIndex = cardIndex;

        DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, m_CardIndex);
        if (card != null)
        {
            m_Background.sprite = TextureManager.GetGradeTypeBackgroundSprite(card.Grade_Type);
            m_Frame.sprite = TextureManager.GetGradeTypeFrameSprite(card.Grade_Type);
            m_Portrait.sprite = TextureManager.GetPortraitSprite(m_CardIndex);

            if (m_Class != null)
            {
                m_Class.sprite = TextureManager.GetClassTypeIconSprite(card.ClassType);
            }
        }

        if (m_Level != null)
            //m_Level.text = (level > 0) ? string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level) : string.Empty;
            m_Level.text = (level <= 0)? string.Empty : string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level);

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.Value = level;

        if (m_Images != null && m_Images.Count > 0)
        {
            for (int i = 0; i < m_Images.Count; i++)
            {
                Image image = m_Images[i];
                if (image != null)
                {
                    m_Images[i].gameObject.SetActive(i < skill);
                }
            }
        }
    }

    void OnClick()
    {
        if (onClicked != null)
        {
            onClicked();
        }
    }
}
