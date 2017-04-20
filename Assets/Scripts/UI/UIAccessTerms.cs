using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAccessTerms : UIObject
{
    // EULA : End-User License Agreement
    // PIUA : Personal Information Usage Agreement
    // TOS : Terms of Service

    public Text m_PIUAContentText;
    public Text m_TOSContentText;
    public Toggle m_PIUAAgreeToggle;
    public Toggle m_TOSAgreeToggle;
    public Button m_ConfirmButton;

    protected override void Awake()
    {
        m_PIUAAgreeToggle.onValueChanged.AddListener(OnToggleValueChanged);
        m_TOSAgreeToggle.onValueChanged.AddListener(OnToggleValueChanged);
        m_ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        m_PIUAAgreeToggle.isOn = false;
        m_TOSAgreeToggle.isOn = false;
        m_ConfirmButton.interactable = false;
        m_ConfirmButton.image.sprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");
        UIUtility.SetBaseMeshEffectColor(m_ConfirmButton.gameObject,
                                         true,
                                         Kernel.colorManager.GetColor("ui_button_disable_shadow"),
                                         Kernel.colorManager.GetColor("ui_button_disable_outline"));

        if (Kernel.entry != null)
        {
            m_PIUAContentText.text = Kernel.entry.data.PersonalInformationUsageAgreement;
            UIUtility.FitSizeToContent(m_PIUAContentText);
            m_TOSContentText.text = Kernel.entry.data.TermsOfService;
            UIUtility.FitSizeToContent(m_TOSContentText);
        }
    }

    void OnConfirmButtonClick()
    {
        if (Kernel.entry != null)
        {
            OnCloseButtonClick();
            Kernel.uiManager.Open(UI.AccountInterconnector);
        }
    }

    void OnToggleValueChanged(bool value)
    {
        if (m_PIUAAgreeToggle.isOn && m_TOSAgreeToggle.isOn)
        {
            m_ConfirmButton.interactable = true;
            m_ConfirmButton.image.sprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_02");
            UIUtility.SetBaseMeshEffectColor(m_ConfirmButton.gameObject,
                                             true,
                                             Kernel.colorManager.GetColor("ui_button_02_shadow"),
                                             Kernel.colorManager.GetColor("ui_button_02_outline"));
        }
        else
        {
            m_ConfirmButton.interactable = false;
            m_ConfirmButton.image.sprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");
            UIUtility.SetBaseMeshEffectColor(m_ConfirmButton.gameObject,
                                             true,
                                             Kernel.colorManager.GetColor("ui_button_disable_shadow"),
                                             Kernel.colorManager.GetColor("ui_button_disable_outline"));
        }
    }
}
