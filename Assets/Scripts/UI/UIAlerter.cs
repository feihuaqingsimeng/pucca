using UnityEngine;
using UnityEngine.UI;

public class UIAlerter : UIObject
{
    public enum Composition
    {
        Confirm, // 확인
        Confirm_Cancel, // 확인, 취소
    }

    public enum Response
    {
        Confirm,
        Cancel,
    }

    static UIAlerter m_Instance;

    static UIAlerter instance
    {
        get
        {
            if (m_Instance == null)
            {
                if (Kernel.uiManager != null)
                {
                    m_Instance = Kernel.uiManager.Get<UIAlerter>(UI.Alerter, true, false);
                }
            }

            return m_Instance;
        }
    }

    public Text m_TitleText;
    public Text m_DescriptionText;
    public Button m_ConfirmButton;
    public Text m_ConfirmButtonText;
    public Button m_CancelButton;
    public Text m_CancelButtonText;

    object[] m_Args;
    Vector2 m_CachedConfirmButtonAnchoredPosition;
    Vector2 m_CachedCancelButtonAnchoredPosition;

    public delegate void OnResponse(Response response, params object[] args);
    public OnResponse onResponse;

    protected override void Awake()
    {
        m_ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
        m_CancelButton.onClick.AddListener(OnCancelButtonClick);

        m_CachedConfirmButtonAnchoredPosition = m_ConfirmButton.image.rectTransform.anchoredPosition;
        m_CachedCancelButtonAnchoredPosition = m_CancelButton.image.rectTransform.anchoredPosition;
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        m_ConfirmButtonText.text = Languages.ToStringBuiltIn(TEXT_UI.OK);
        m_CancelButtonText.text = Languages.ToStringBuiltIn(TEXT_UI.CANCEL);
    }

    public static void Alert(string description,
                             Composition composition,
                             OnResponse onResponse = null,
                             string title = null,
                             params object[] args)
    {
        if (instance == null)
        {
            return;
        }

        m_Instance.SetComposition(composition);
        m_Instance.m_TitleText.text = title;
        m_Instance.m_DescriptionText.text = description;

        if (onResponse != null)
        {
            m_Instance.onResponse = onResponse;
        }

        if (args != null && args.Length > 0)
        {
            m_Instance.m_Args = args;
        }

        Kernel.uiManager.Open(m_Instance.ui);
    }

    void SetComposition(Composition composition)
    {
        switch (composition)
        {
            case Composition.Confirm:
                m_ConfirmButton.image.rectTransform.anchoredPosition = new Vector2(m_ConfirmButton.image.rectTransform.sizeDelta.x * .5f,
                                                                                   m_ConfirmButton.image.rectTransform.anchoredPosition.y);
                break;
            case Composition.Confirm_Cancel:
                m_ConfirmButton.image.rectTransform.anchoredPosition = m_CachedConfirmButtonAnchoredPosition;
                m_CancelButton.image.rectTransform.anchoredPosition = m_CachedCancelButtonAnchoredPosition;
                break;
        }

        m_ConfirmButton.gameObject.SetActive(Equals(Composition.Confirm, composition)
                                          || Equals(Composition.Confirm_Cancel, composition));
        m_CancelButton.gameObject.SetActive(Equals(Composition.Confirm_Cancel, composition));
    }

    void Close(Response response)
    {
        if (onResponse != null)
        {
            onResponse(response, m_Args);
        }

        onResponse = null;
        m_Args = null;
        Kernel.uiManager.Close(ui);
    }

    void OnConfirmButtonClick()
    {
        Close(Response.Confirm);
    }

    void OnCancelButtonClick()
    {
        Close(Response.Cancel);
    }
}
