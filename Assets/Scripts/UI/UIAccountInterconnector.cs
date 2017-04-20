using Common.Util;
using UnityEngine;
using UnityEngine.UI;

public class UIAccountInterconnector : UIObject
{
    public Button m_GameCenterButton;
    public Button m_GPGSButton;
    public Button m_FacebookButton;
    public Button m_GuestButton;

    protected override void Awake()
    {
        base.Awake();

        m_GameCenterButton.onClick.AddListener(OnGameCenterButtonClick);
        m_GPGSButton.onClick.AddListener(OnGPGSButtonClick);
        m_FacebookButton.onClick.AddListener(OnFacebookButtonClick);
        m_GuestButton.onClick.AddListener(OnGuestButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        m_CloseButton.interactable = (Kernel.languageCode == LanguageCode.Korean) ||
                                     (Kernel.sceneManager.activeSceneObject.scene != Scene.TitleScene);


#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        m_GPGSButton.gameObject.SetActive(true);
#else
        m_GPGSButton.gameObject.SetActive(false);
#endif

        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_GameCenterButton.gameObject.SetActive(false);
        }
        else
        {
            m_GameCenterButton.gameObject.SetActive(false);
        }

        m_FacebookButton.gameObject.SetActive(true);

        bool needGuestButton = Equals(Kernel.sceneManager.activeSceneObject.scene.ToString(), Scene.TitleScene.ToString());
        m_GuestButton.gameObject.SetActive(needGuestButton);
    }

    protected override void OnCloseButtonClick()
    {
        base.OnCloseButtonClick();

        // Title 씬에서만 이용약관이 뜨도록 함. (계정 연동에서 쓸때에는 안뜨도록..)
        if (Equals(Kernel.sceneManager.activeSceneObject.scene.ToString(), Scene.TitleScene.ToString()) &&
            Kernel.languageCode == LanguageCode.Korean)
        {
            Kernel.uiManager.Open(UI.AccessTerms);
        }
    }

    void OnGameCenterButtonClick()
    {
        if (Kernel.gamecenterManager != null)
        {
            Kernel.gamecenterManager.SignIn();
        }
    }

    void OnGPGSButtonClick()
    {
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        if (Kernel.gpgsManager != null)
        {
            Kernel.gpgsManager.SignIn();
        }
#endif
    }

    void OnFacebookButtonClick()
    {
        if (Kernel.facebookManager != null)
        {
            Kernel.facebookManager.LogIn();
        }
    }

    void OnGuestButtonClick()
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.CHANGE_PHONE_LOSS),
                        UIAlerter.Composition.Confirm_Cancel,
                        delegate(UIAlerter.Response response, object[] args)
                        {
                            if (response == UIAlerter.Response.Confirm)
                            {
                                Kernel.entry.account.REQ_PACKET_CG_AUTH_LOGIN_SYN(eLoginType.Guest,
                                                                                  Kernel.uid,
                                                                                  string.Empty,
                                                                                  string.Empty);
                            }
                        },
                        Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }
}
