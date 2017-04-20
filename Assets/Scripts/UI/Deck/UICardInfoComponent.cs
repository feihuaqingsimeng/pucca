using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICardInfoComponent : MonoBehaviour
{
    public UISlider m_Slider;
    public Image m_SliderIconImage;
    public Button m_LevelUpButton;
    public Text m_LevelUpButtonText;
    public Text m_GoldText;
    public List<Graphic> m_GrayscaleGraphics;
    public float m_SliderAnimationDuration;
    public Result_Define.eResult m_NotEnoughTicketResult;

    protected long m_CID;
    protected bool m_Interactable;

    protected virtual void Awake()
    {
        m_LevelUpButton.onClick.AddListener(OnLevelUpButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    protected virtual void OnEnable()
    {
        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException += OnFail;
        }
    }

    protected virtual void OnDisable()
    {
        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException -= OnFail;
        }

        /*
NullReferenceException
  at (wrapper managed-to-native) UnityEngine.GameObject:SetActive (bool)
  at UICardSkillInfo.Initialize () [0x00000] in <filename unknown>:0 
  at UICardInfoComponent.OnDisable () [0x00000] in <filename unknown>:0 
  at UICardSkillInfo.OnDisable () [0x00000] in <filename unknown>:0 
 
(Filename:  Line: -1)

NullReferenceException
  at (wrapper managed-to-native) UnityEngine.Component:get_transform ()
  at UICardCharInfo.Initialize () [0x00000] in <filename unknown>:0 
  at UICardInfoComponent.OnDisable () [0x00000] in <filename unknown>:0 
  at UICardCharInfo.OnDisable () [0x00000] in <filename unknown>:0 
 
(Filename:  Line: -1)
        */
        if (gameObject == null)
        {
            return;
        }

        Initialize();
    }

    #region Properties
    public virtual long cid
    {
        get
        {
            return m_CID;
        }

        set
        {
            if (m_CID != value)
            {
                m_CID = value;
            }
        }
    }

    protected bool goldAvailable
    {
        get
        {
            return (Kernel.entry.account.gold >= requiredGold);
        }
    }

    public bool isMaxLevel
    {
        get
        {
            return (level >= maxLevel);
        }
    }

    protected virtual byte maxLevel
    {
        get
        {
            return 0;
        }
    }

    private byte m_level;
    protected byte level
    {
        get { return m_level; }
        set
        {
            m_level = value;
            Kernel.entry.character.onLevelUpCallback();
        }
    }

    protected bool levelUpAvailable
    {
        get
        {
            return goldAvailable && !isMaxLevel && ticketAvailable;
        }
    }

    protected int requiredGold
    {
        get;
        set;
    }

    protected int requiredTicket
    {
        get;
        set;
    }

    protected int ticket
    {
        get;
        set;
    }

    protected bool ticketAvailable
    {
        get
        {
            return (ticket >= requiredTicket);
        }
    }

    protected virtual bool interactable
    {
        get
        {
            return m_Interactable;
        }

        set
        {
            if (m_Interactable != value)
            {
                m_Interactable = value;
            }
        }
    }
    #endregion

    void OnFail(Result_Define.eResult result, string error, ePACKET_CATEGORY packetCategory, byte packetIndex)
    {
        if (packetCategory == ePACKET_CATEGORY.CG_CARD
            && (packetIndex == (byte)eCG_CARD.LEVEL_UP_ACK
                || packetIndex == (byte)eCG_CARD.ITEM_LEVEL_UP_ACK
                || packetIndex == (byte)eCG_CARD.SKILL_LEVEL_UP_ACK))
        {
            interactable = true;
        }
    }

    protected virtual void LevelUp()
    {

    }

    protected virtual void Initialize()
    {
        StopAllCoroutines();
        m_CID = 0;
        level = 0;
        requiredGold = 0;
        requiredTicket = 0;
        ticket = 0;
        interactable = true;
    }

    protected void UpdateLevelUpButton()
    {
        m_GoldText.text = isMaxLevel ? Languages.ToString(TEXT_UI.LEVEL_MAX) : Languages.ToString<int>(requiredGold);

        Color ui_button_04_outline, ui_button_04_shadow;
        if (isMaxLevel)
        {
            Color button04_max_text;
            Kernel.colorManager.TryGetColor("ui_button_04_max", out button04_max_text);
            Kernel.colorManager.TryGetColor("ui_button_04_outline", out ui_button_04_outline);
            Kernel.colorManager.TryGetColor("ui_button_04_shadow", out ui_button_04_shadow);

            SetColor(m_GoldText, button04_max_text, Color.black, Color.black);
            SetColor(m_LevelUpButtonText, Color.white, ui_button_04_outline, ui_button_04_shadow);
        }
        else
        {
            if (levelUpAvailable)
            {
                Kernel.colorManager.TryGetColor("ui_button_02_outline", out ui_button_04_outline);
                Kernel.colorManager.TryGetColor("ui_button_02_shadow", out ui_button_04_shadow);

                SetColor(m_GoldText, Color.white, ui_button_04_outline, ui_button_04_shadow);
                SetColor(m_LevelUpButtonText, Color.white, ui_button_04_outline, ui_button_04_shadow);
            }
            else
            {
                Kernel.colorManager.TryGetColor("ui_button_04_outline", out ui_button_04_outline);
                Kernel.colorManager.TryGetColor("ui_button_04_shadow", out ui_button_04_shadow);

                if (goldAvailable)
                {
                    SetColor(m_GoldText, Color.white, ui_button_04_outline, ui_button_04_shadow);
                }
                else
                {
                    SetColor(m_GoldText, Color.red, Color.black, Color.black);
                }

                SetColor(m_LevelUpButtonText, Color.white, ui_button_04_outline, ui_button_04_shadow);
            }
        }

        grayscale = levelUpAvailable ? false : true;
    }

    protected bool grayscale
    {
        set
        {
            for (int i = 0; i < m_GrayscaleGraphics.Count; i++)
            {
                m_GrayscaleGraphics[i].material = value ? UIUtility.grayscaleMaterial : null;
            }
        }
    }

    void SetColor(Text text, Color textColor, Color outlineColor, Color shadowColor)
    {
        if (text != null)
        {
            text.color = textColor;

            foreach (var item in text.GetComponentsInChildren<Shadow>(true))
            {
                if (item is Outline)
                {
                    item.effectColor = outlineColor;
                }
                else if (item is Shadow)
                {
                    item.effectColor = shadowColor;
                }
            }
        }
    }

    protected void OnLevelUpButtonClick()
    {
        if (Kernel.entry == null)
        {
            return;
        }

        SoundDataInfo.CancelSound(m_LevelUpButton.gameObject);

        if (isMaxLevel)
        {
            NetworkEventHandler.OnNetworkException(Result_Define.eResult.ALREADY_MAX_LEVEL);
        }
        else if (!ticketAvailable)
        {
            NetworkEventHandler.OnNetworkException(m_NotEnoughTicketResult);
        }
        else if (!goldAvailable)
        {
            NetworkEventHandler.OnNetworkException(Result_Define.eResult.NOT_ENOUGH_GOLD);
        }

        if (levelUpAvailable && interactable)
        {
            interactable = false;
            SoundDataInfo.ChangeUISound(SOUND.SND_UI_UPGRADE, m_LevelUpButton.gameObject);
            LevelUp();
        }
    }
}
