using Common.Packet;
using UnityEngine.UI;

public class UINicknameEditor : UIObject
{
    public InputField m_NicknameInputField;
    public Text m_WarningText;
    public Button m_ConfirmButton;

    protected override void Awake()
    {
        base.Awake();
        m_ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onCreateNicknameResult += OnCreateNicknameResult;
        }

        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException += OnCreateNicknameException;
        }

        m_NicknameInputField.characterLimit = 8;
        m_WarningText.text = string.Empty;
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onCreateNicknameResult -= OnCreateNicknameResult;
        }

        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException -= OnCreateNicknameException;
        }
    }

    protected override void OnCloseButtonClick()
    {
        base.OnCloseButtonClick();
        Kernel.uiManager.Open(UI.AccountInterconnector);
    }

    void OnCreateNicknameException(Result_Define.eResult result, string error = null, ePACKET_CATEGORY category = 0, byte index = 0)
    {
        if (category == ePACKET_CATEGORY.CG_AUTH &&
                index == (byte)eCG_AUTH.CREATE_NICKNAME_ACK)
        {
            m_WarningText.text = Languages.ToString(result);
        }
    }

    void OnCreateNicknameResult(string nickname)
    {
        Kernel.uiManager.Close(ui);
    }

    void OnConfirmButtonClick()
    {
        if (Languages.IsAvailableName(m_NicknameInputField.text))
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.MAKE_NAME_IDENFIFY, m_NicknameInputField.text),
                                           UIAlerter.Composition.Confirm_Cancel,
                                           delegate(UIAlerter.Response response, object[] args)
                                           {
                                               if (response == UIAlerter.Response.Confirm)
                                               {
                                                   Kernel.entry.account.REQ_PACKET_CG_AUTH_CREATE_NICKNAME_SYN(m_NicknameInputField.text);
                                               }
                                           },
                                           Languages.ToString(TEXT_UI.OK));
        }
        else
        {
            //NetworkEventHandler.OnNetworkException(Result_Define.eResult.ID_CREATE_RULE_ERROR);
            OnCreateNicknameException(Common.Packet.Result_Define.eResult.ID_CREATE_RULE_ERROR,
                                      string.Empty,
                                      ePACKET_CATEGORY.CG_AUTH,
                                      (byte)eCG_AUTH.CREATE_NICKNAME_ACK);
        }
    }
}
