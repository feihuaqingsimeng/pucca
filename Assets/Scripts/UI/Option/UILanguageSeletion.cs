using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UILanguageSeletion : UIObject
{
    public List<LanguageSeletionObject> m_listLanguageButton;

    [HideInInspector]
    public LanguageSeletionObject m_PreLanguage;

    [HideInInspector]
    public LanguageSeletionObject m_SelectLanguage;


    //** Text
    public Text m_Title_Text;
    public Text m_Ok_Text;

    //** Image
    public Sprite m_SelectSprite;
    public Sprite m_NormalSprite;

    //** Color
    public Color m_SelectShadow_Color;
    public Color m_NormalShadow_Color;
    public Color m_SelectOutline_Color;
    public Color m_NormalOutline_Color;

    protected override void Awake()
    {
        base.Awake();

        SetUIInit();
    }

    private void SetUIInit()
    {
        m_Title_Text.text   = Languages.ToString(TEXT_UI.LANGUAGE);
        m_Ok_Text.text      = Languages.ToString(TEXT_UI.CANCEL);

        if (m_listLanguageButton == null)
            return;

        for (int i = 0; i < m_listLanguageButton.Count; i++)
        {
            LanguageSeletionObject button = m_listLanguageButton[i];

            bool currentLanguage = Kernel.languageCode == button.m_eLanguageCode;

            Sprite sprite       = currentLanguage ? m_SelectSprite          : m_NormalSprite;
            Color outlineColor  = currentLanguage ? m_SelectOutline_Color   : m_NormalOutline_Color;
            Color shadowColor   = currentLanguage ? m_SelectShadow_Color    : m_NormalShadow_Color;

            button.ButtonSetting(this, sprite, outlineColor, shadowColor);

            if (currentLanguage)
                m_PreLanguage = button;
        }
    }

    //** 언어 변경
    public void ChangeLanguageButton(LanguageSeletionObject select)
    {
        //if (select.m_eLanguageCode != LanguageCode.Korean && select.m_eLanguageCode != LanguageCode.English)
        //{
        //    UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.NOTICE_PREPARE));
        //    return;
        //}

        m_PreLanguage.ButtonSetting(this, m_NormalSprite, m_NormalOutline_Color, m_NormalShadow_Color);
        select.ButtonSetting(this, m_SelectSprite, m_SelectOutline_Color, m_SelectShadow_Color);

        m_SelectLanguage = select;

        UIAlerter.Alert(Languages.ToString(TEXT_UI.LANGUAGE_RESTART), UIAlerter.Composition.Confirm_Cancel, OnApplicationQuitForChangeLanguage, Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }

    //** 재시작
    private void OnApplicationQuitForChangeLanguage(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
        {
            m_SelectLanguage.ButtonSetting(this, m_NormalSprite, m_NormalOutline_Color, m_NormalShadow_Color);
            m_PreLanguage.ButtonSetting(this, m_SelectSprite, m_SelectOutline_Color, m_SelectShadow_Color);

            m_SelectLanguage = m_PreLanguage;
            return;
        }

        Kernel.languageCode = m_SelectLanguage.m_eLanguageCode;

        Kernel.Reload();

//        if (UnityEngine.Application.isEditor && UnityEngine.Application.isPlaying)
//        {
//#if UNITY_EDITOR
//            UnityEditor.EditorApplication.isPlaying = false;
//#else
//                    UnityEngine.Application.Quit();
//#endif
//        }
    }
}
