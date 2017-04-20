using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LanguageSeletionObject : MonoBehaviour 
{
    private UILanguageSeletion m_owner;

    public LanguageCode m_eLanguageCode;

    public Image    m_Language_Image;
    public Text     m_Language_Text;
    public Shadow   m_Language_Text_Shdow;
    public Outline  m_Language_Text_Outline;
    public Button   m_Language_Button;

    public void ButtonSetting(UILanguageSeletion owner, Sprite sprite, Color textOutlineColor, Color textShadowColor)
    {
        m_owner = owner;

        m_Language_Image.sprite = sprite;
        m_Language_Text_Outline.effectColor = textOutlineColor;
        m_Language_Text_Shdow.effectColor = textShadowColor;

        m_Language_Text.text = Languages.ToString(m_eLanguageCode);
        m_Language_Button.onClick.AddListener(OnClickLanguageButton);
    }

    private void OnClickLanguageButton()
    {
        m_owner.ChangeLanguageButton(this);
    }
}
