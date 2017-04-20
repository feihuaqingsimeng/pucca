using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OptionEnvironmentSetting : MonoBehaviour 
{
    //** Toggle
    public ToggleSiblingGroup m_BGM_Toggle;
    public ToggleSiblingGroup m_EFS_Toggle;
    public ToggleSiblingGroup m_Push_Toggle;

    //** Text
    public Text     m_BGM_Text;
    public Text     m_EFS_Text;
    public Text     m_Push_Text;
    public Text     m_Languages_Text;
    public Text     m_Select_Language_Text;
    public Text[]   m_On_Text;
    public Text[]   m_Off_Text;

    //** Button
    public Button   m_Select_Language_Button;

    //** UI Init
    public void SetUIInit()
    {
        m_BGM_Text.text             = Languages.ToString(TEXT_UI.BGM);
        m_EFS_Text.text             = Languages.ToString(TEXT_UI.SFX);
        m_Push_Text.text            = Languages.ToString(TEXT_UI.PUSH_NOTICE);
        m_Languages_Text.text       = Languages.ToString(TEXT_UI.LANGUAGE);
        m_Select_Language_Text.text = Languages.ToString(Kernel.languageCode);
        //m_Select_Language_Text.text = Languages.ToString(LanguageCode.English); //비활성. 기본 영어로 표기

        for (int i = 0; i < m_On_Text.Length; i++)
        {
            Text onText = m_On_Text[i];
            onText.text = "on";
        }

        for (int i = 0; i < m_Off_Text.Length; i++)
        {
            Text offText = m_Off_Text[i];
            offText.text = "off";
        }

        m_Select_Language_Button.onClick.RemoveAllListeners();
        m_Select_Language_Button.onClick.AddListener(OnClickLanguageButton);

        SetToggleInit();
    }
    
    //** Toggle
    private void SetToggleInit()
    {
        // BGM
        int bgmToggleValue = SoundManager.Instance.BGM_On ? 0 : 1;
        m_BGM_Toggle.m_Toggles[bgmToggleValue].isOn = true;

        // EFS
        int efsToggleValue = SoundManager.Instance.SFX_On ? 0 : 1;
        m_EFS_Toggle.m_Toggles[efsToggleValue].isOn = true;

        for (int i = 0; i < m_BGM_Toggle.m_Toggles.Count; i++)
            m_BGM_Toggle.m_Toggles[i].onValueChanged.AddListener(SetBGMToggleValue);

        for (int i = 0; i < m_EFS_Toggle.m_Toggles.Count; i++)
            m_EFS_Toggle.m_Toggles[i].onValueChanged.AddListener(SetEFSToggleValue);

        // Push
        bool pushOn = Kernel.notificationManager.Push_On;

        int pushToggleValue = pushOn ? 0 : 1;
        m_Push_Toggle.m_Toggles[pushToggleValue].isOn = true;

        for (int i = 0; i < m_Push_Toggle.m_Toggles.Count; i++)
            m_Push_Toggle.m_Toggles[i].onValueChanged.AddListener(SetPushToggleValue);
    }

    //** BGM Value Change
    private void SetBGMToggleValue(bool on)
    {
        if (!on)
            return;

        bool bgmOn = m_BGM_Toggle.m_Toggles[0].isOn;
        SoundManager.Instance.BGM_On = bgmOn;
    }

    //** EFS Value Change
    private void SetEFSToggleValue(bool on)
    {
        if (!on)
            return;

        bool efsOn = m_EFS_Toggle.m_Toggles[0].isOn;
        SoundManager.Instance.SFX_On = efsOn;
    }

    //** Push Value Change
    private void SetPushToggleValue(bool on)
    {
        if (!on)
            return;

        bool pushOn = m_Push_Toggle.m_Toggles[0].isOn;
        Kernel.notificationManager.Push_On = pushOn;
    }

    //** 언어선택 버튼 클릭.
    private void OnClickLanguageButton()
    {
        //비활성화
        //UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.NOTICE_PREPARE));
        //return;

        if (Kernel.uiManager)
        {
            UILanguageSeletion languageSeletion = Kernel.uiManager.Get<UILanguageSeletion>(UI.LanguageSeletion, true, false);

            if (languageSeletion)
                Kernel.uiManager.Open(UI.LanguageSeletion);
        }
    }
}
