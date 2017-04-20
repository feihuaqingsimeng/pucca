using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIOption : UIObject 
{
    public ToggleSiblingGroup            m_toggleGroup;

    public OptionEnvironmentSetting m_EnvironmentSettingTab;
    public OptionGameInfo           m_GameInfoTab;

    //** Text
    public Text m_MainTitle_Text;
    public Text m_EnvironmetSetting_Text;
    public Text m_GameInfo_Text;
    public Text m_Ok_Text;

    public Button   m_TempDeleteAccount;
    public Text     m_TempDeleteAccountText;

    protected override void Awake()
    {
        base.Awake();

        m_MainTitle_Text.text           = Languages.ToString(TEXT_UI.OPTION_GAME_SETTING);
        m_EnvironmetSetting_Text.text   = Languages.ToString(TEXT_UI.OPTION_GAME_SETTING);
        m_GameInfo_Text.text            = Languages.ToString(TEXT_UI.OPTION_GAME_INFO);
        m_Ok_Text.text                  = Languages.ToString(TEXT_UI.OK);

        m_TempDeleteAccountText.text = Languages.ToString(TEXT_UI.ACCOUNT_ESCAPE);

        m_EnvironmentSettingTab.SetUIInit();
        m_GameInfoTab.SetUIInit();

        for (int i = 0; i < m_toggleGroup.m_Toggles.Count; i++)
        {
            Toggle toggle = m_toggleGroup.m_Toggles[i];

            if (i == 0)
            {
                toggle.onValueChanged.AddListener(OnActiveEnvironmentTab);
            }
            else
            {
                toggle.onValueChanged.AddListener(OnActiveGameInfoTab);
            }
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        // 무조건 환경설정 먼저 켜짐.
        for (int i = 0; i < m_toggleGroup.m_Toggles.Count; i++)
        {
            Toggle toggle = m_toggleGroup.m_Toggles[i];

            if (i == 0)
            {
                toggle.isOn = true;
            }
            else
            {
                toggle.isOn = false;
            }
        }

        OnActiveEnvironmentTab(true);

        m_GameInfoTab.LinkAccountLinkFuc();

        //** 임시
        if (Kernel.entry.account.onDropOutAccountResult == null)
            Kernel.entry.account.onDropOutAccountResult += TempDropOutAccountResult;
        //** 임시
        m_TempDeleteAccount.onClick.AddListener(REQDropOutAccount);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        m_GameInfoTab.ClearAccountLinkFuc();

        //** 임시
        if (Kernel.entry.account.onDropOutAccountResult != null)
            Kernel.entry.account.onDropOutAccountResult -= TempDropOutAccountResult;
        //** 임시
        m_TempDeleteAccount.onClick.RemoveListener(REQDropOutAccount);
    }

    private void OnActiveEnvironmentTab(bool value)
    {
        m_EnvironmentSettingTab.gameObject.SetActive(true);
        m_GameInfoTab.gameObject.SetActive(false);
    }

    private void OnActiveGameInfoTab(bool value)
    {
        m_EnvironmentSettingTab.gameObject.SetActive(false);
        m_GameInfoTab.gameObject.SetActive(true);
    }


    //** 계정 탈퇴(임시)
    private void REQDropOutAccount()
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_WITHDRAW_ACCOUNT_SYN();
    }

    //** 계정 탈퇴 결과(임시)
    private void TempDropOutAccountResult()
    {
        PlayerPrefs.SetInt(Kernel.entry.account.mainLoginKey, (int)Common.Util.eLoginType.None);

        if (Kernel.entry.account.subLoginType != Common.Util.eLoginType.None)
            PlayerPrefs.SetInt(Kernel.entry.account.subLoginKey, (int)Common.Util.eLoginType.None);

        PlayerPrefs.Save();

        Kernel.Reload();
    }
}

