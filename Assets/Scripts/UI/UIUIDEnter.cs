using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Util;
using Common.Packet;

public class UIUIDEnter : UIObject
{
    public InputField m_InputField;
    public Button m_Button;

    protected override void Awake()
    {
        base.Awake();

        if (!string.IsNullOrEmpty(Kernel.uid))
        {
            m_InputField.text = Kernel.uid;
        }

        m_Button.onClick.AddListener(delegate()
        {
            string uid = m_InputField.text;
            if (!string.IsNullOrEmpty(uid))
            {
                Kernel.uid = uid;
                Kernel.entry.account.REQ_PACKET_CG_AUTH_LOGIN_SYN(eLoginType.Guest, Kernel.uid, string.Empty, string.Empty);
            }
        });
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        Kernel.entry.account.onLogInResult += Listener;
    }

    protected override void OnDisable()
    {
        Kernel.entry.account.onLogInResult -= Listener;
    }

    void Listener(bool isNewUser, bool isFirstLogIn)
    {
        OnCloseButtonClick();
    }
}
