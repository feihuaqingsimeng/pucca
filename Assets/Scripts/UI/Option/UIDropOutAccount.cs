using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIDropOutAccount : UIObject 
{
    private const string STR_CHECK_STRING = "puccawarsbye";


    //** Text
    public Text m_Title_Text;
    public Text m_Dec;
    public Text m_Ok_Text;
    public Text m_Cancle_Text;

    //** Text Field
    public InputField m_DroupOutField;

    //** Button
    public Button m_Ok_Button;

    protected override void Awake()
    {
        base.Awake();

        SetUIInit();
    }

    protected override void OnEnable()
    {
        if(Kernel.entry.account.onDropOutAccountResult == null)
            Kernel.entry.account.onDropOutAccountResult += DropOutAccountResult;

        base.OnEnable();
        
    }

    protected override void OnDisable()
    {
        if (Kernel.entry.account.onDropOutAccountResult != null)
            Kernel.entry.account.onDropOutAccountResult -= DropOutAccountResult;

        base.OnDisable();
    }

    protected override void OnCloseButtonClick()
    {
        // 초기화
        m_DroupOutField.text = "";

        base.OnCloseButtonClick();
    }

    //** UI Init
    public void SetUIInit()
    {
        // 텍스트 입력
        m_Title_Text.text   = Languages.ToString(TEXT_UI.ACCOUNT_ESCAPE);
        m_Dec.text          = Languages.ToString(TEXT_UI.ACCOUNT_SECESSION_ERROR_TYPING);
        m_Ok_Text.text      = Languages.ToString(TEXT_UI.OK);
        m_Cancle_Text.text  = Languages.ToString(TEXT_UI.CANCEL);

        m_Ok_Button.onClick.AddListener(OnClickOKButton);
        m_DroupOutField.text = "";
    }

    //** 확인 버튼 클릭시
    public void OnClickOKButton()
    {
        bool isCollect = string.Equals(m_DroupOutField.text, STR_CHECK_STRING);

        if (!isCollect)
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.ACCOUNT_SECESSION_WARNING), UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
            return;
        }

        Kernel.entry.account.REQ_PACKET_CG_AUTH_WITHDRAW_ACCOUNT_SYN();
    }

    //** 계정 탈퇴 결과
    private void DropOutAccountResult()
    {
        PlayerPrefs.SetInt(Kernel.entry.account.mainLoginKey, (int)Common.Util.eLoginType.None);

        if (Kernel.entry.account.subLoginType != Common.Util.eLoginType.None)
            PlayerPrefs.SetInt(Kernel.entry.account.subLoginKey, (int)Common.Util.eLoginType.None);

        PlayerPrefs.Save();

        Kernel.Reload();
    }
}
