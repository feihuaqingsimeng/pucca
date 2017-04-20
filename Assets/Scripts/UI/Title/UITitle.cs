using Common.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LanguageTitleLogo
{
    public LanguageCode m_eLanguageCode;
    public Sprite m_LogoSprite;
}

public class UITitle : UIObject
{
    public Slider m_DownloadProgressSlider;
    public Text m_DownloadProgressText;
    public Text m_DownloadDescriptionText;
    public Text m_VersionText;
    public GameObject m_TouchScreenGameObject;

    public List<LanguageTitleLogo> m_arrlanguageTitleLogo;
    public Sprite m_BasicTitleLogo;
    public Image m_TitleLogo;

    protected override void Awake()
    {
        m_DownloadProgressSlider.gameObject.SetActive(false);
        m_DownloadDescriptionText.gameObject.SetActive(false);
        m_DownloadProgressSlider.value = 0;
        m_DownloadProgressText.text = string.Empty;
        m_DownloadDescriptionText.text = string.Empty;
        m_TouchScreenGameObject.SetActive(false);

        m_VersionText.text = string.Empty;
    }

    // Use this for initialization
    protected override void Start()
    {
        Kernel.dataLoader.pEndDelegate = new DataLoader.LoadEndDelegate(OnDownloadFinishCallback);

        CheckGameVersion();

        LanguageTitleLogo logo = m_arrlanguageTitleLogo.Find(item => item.m_eLanguageCode == Kernel.languageCode);
        if (logo != null)
            m_TitleLogo.sprite = logo.m_LogoSprite;
        else
            m_TitleLogo.sprite = m_BasicTitleLogo;

        m_TitleLogo.SetNativeSize();

    }

    float totalProgress, progress;

    // Update is called once per frame
    protected override void Update()
    {
        if (Kernel.dataLoader != null)
        {
            switch (Kernel.dataLoader.downloadState)
            {
                case DataLoader.DownloadState.DownloadAssetBundle:
                    int totalDownloadFileCount = Kernel.dataLoader.totalDownloadFileCount;
                    if (totalDownloadFileCount > 0)
                    {
                        int downloadFileCount = Kernel.dataLoader.downloadFileCount;

                        progress = downloadFileCount + AssetBundleManager.progress;

                        m_DownloadProgressSlider.gameObject.SetActive(true);
                        m_DownloadProgressSlider.normalizedValue = (progress / (float)totalDownloadFileCount);
                        m_DownloadProgressText.gameObject.SetActive(true);
                        m_DownloadProgressText.text = string.Format("{0:F0}%", m_DownloadProgressSlider.normalizedValue * 100f);
                        m_DownloadDescriptionText.gameObject.SetActive(true);
                        m_DownloadDescriptionText.text = string.Format("{0} ({1}/{2})", Languages.ToStringBuiltIn(TEXT_UI.UPDATA_INFO), downloadFileCount, totalDownloadFileCount);
                    }
                    else
                    {
                        m_DownloadProgressSlider.gameObject.SetActive(false);
                        m_DownloadDescriptionText.gameObject.SetActive(true);
                        m_DownloadDescriptionText.text = Languages.ToStringBuiltIn(TEXT_UI.UPDATA_INFO);
                    }
                    break;
                case DataLoader.DownloadState.Idle:
                    m_DownloadProgressSlider.gameObject.SetActive(false);
                    m_DownloadProgressText.gameObject.SetActive(false);
                    m_DownloadDescriptionText.gameObject.SetActive(false);

                    if (Kernel.entry.account.initialized)
                    {
                        m_TouchScreenGameObject.gameObject.SetActive(true);

                        if (Application.isMobilePlatform)
                        {
                            if (Input.touchCount > 0)
                            {
                                Touch t = Input.GetTouch(0);
                                if (t.phase == TouchPhase.Ended)
                                {
                                    //전투 테스트모드 추가.
                                    GameObject TestModuleObj = GameObject.Find("BattleTestModule");
                                    if (TestModuleObj != null)
                                    {
                                        Kernel.entry.battle.CurBattleKind = BATTLE_KIND.TEST_BATTLE;
                                        Kernel.sceneManager.LoadScene(Scene.Battle);
                                    }
                                    else
                                    {
                                        //임시추가. 튜토리얼.
                                        Kernel.entry.tutorial.GroupNumber = Kernel.entry.account.TutorialGroup;

                                        //                                        Kernel.entry.tutorial.GroupNumber = 0;
                                        Kernel.sceneManager.LoadScene(Scene.Lobby);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Input.GetMouseButtonUp(0))
                            {
                                //전투 테스트모드 추가.
                                GameObject TestModuleObj = GameObject.Find("BattleTestModule");
                                if (TestModuleObj != null)
                                {
                                    Kernel.entry.battle.CurBattleKind = BATTLE_KIND.TEST_BATTLE;
                                    Kernel.sceneManager.LoadScene(Scene.Battle);
                                }
                                else
                                {
                                    //임시추가. 튜토리얼.
                                    Kernel.entry.tutorial.GroupNumber = Kernel.entry.account.TutorialGroup;
                                    //                                    Kernel.entry.tutorial.GroupNumber = 10;

                                    Kernel.sceneManager.LoadScene(Scene.Lobby);
                                }
                            }
                        }
                    }
                    break;
            }
        }
    }

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onLogInResult += OnLogInResult;
            Kernel.entry.data.onTerms += OnTerms;
        }

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        if (Kernel.gpgsManager != null)
        {
            Kernel.gpgsManager.onSignIn += OnGPGSSignIn;
        }
#endif

        if (Kernel.gamecenterManager != null)
        {
            Kernel.gamecenterManager.onSignIn += OnGameCenterLogin;
        }

        if (Kernel.facebookManager != null)
        {
            Kernel.facebookManager.onLogIn += OnFacebookLogIn;
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onLogInResult -= OnLogInResult;
            Kernel.entry.data.onTerms -= OnTerms;
        }

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        if (Kernel.gpgsManager != null)
        {
            Kernel.gpgsManager.onSignIn -= OnGPGSSignIn;
        }
#endif

        if (Kernel.gamecenterManager != null)
        {
            Kernel.gamecenterManager.onSignIn -= OnGameCenterLogin;
        }

        if (Kernel.facebookManager != null)
        {
            Kernel.facebookManager.onLogIn -= OnFacebookLogIn;
        }
    }

    void OnTerms(string personalInformationUsageAgreement, string TermsOfService)
    {
        Kernel.uiManager.Open(UI.AccessTerms);
    }

    void OnLogInResult(bool isNewUser, bool isFirstLogIn)
    {
        Kernel.uiManager.Close(UI.NicknameEditor);
        Kernel.uiManager.Close(UI.AccountInterconnector);

        if (isNewUser)
        {
            Kernel.uiManager.Open(UI.NicknameEditor);
        }
    }

    void OnDownloadFinishCallback()
    {
        Kernel.uiManager.Open(UI.Loading); // Preload.

        if (!Application.isEditor && !Application.isMobilePlatform)
        //if (true)
        {
            Kernel.uiManager.Open(UI.UIDEnter);
        }
        else
        {
            Debug.Log(string.Format("PlayerPrefs.HasKey(Kernel.entry.account.mainLoginKey) : {0} / PlayerPrefs.HasKey(uid) : {1} / (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.mainLoginKey) Type : {2}",
            PlayerPrefs.HasKey(Kernel.entry.account.mainLoginKey), PlayerPrefs.HasKey("uid"), (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.mainLoginKey)));

            if (!PlayerPrefs.HasKey(Kernel.entry.account.mainLoginKey) || !PlayerPrefs.HasKey("uid") || (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.mainLoginKey) == eLoginType.None)
            {
                if (Kernel.languageCode == LanguageCode.Korean)
                {
                    Kernel.entry.data.REQ_PACKET_CG_AUTH_GET_TERMS_SYN();
                }
                else
                {
                    Kernel.uiManager.Open(UI.AccountInterconnector);
                }
            }
            //else if ((eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.mainLoginKey) == eLoginType.None)
            //{
            //    if (Kernel.entry != null)
            //    {
            //    OnCloseButtonClick();
            //    Kernel.uiManager.Open(UI.AccountInterconnector);
            //    }
            //}
            else
            {
                eLoginType loginType = eLoginType.None;

                if (PlayerPrefs.HasKey(Kernel.entry.account.subLoginKey) && (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.subLoginKey) != eLoginType.None)
                {
                    // Debug.LogError("if");
                    loginType = (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.subLoginKey);
                }
                else if (PlayerPrefs.HasKey(Kernel.entry.account.mainLoginKey) && (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.mainLoginKey) != eLoginType.None)
                {
                    // Debug.LogError("else");
                    loginType = (eLoginType)PlayerPrefs.GetInt(Kernel.entry.account.mainLoginKey);
                }

                Debug.Log("LoginType" + loginType);
                switch (loginType)
                {
                    /*
                    case eLoginType.Apple:
                        Kernel.gamecenterManager.SignIn();
                        break;
                    */
                    case eLoginType.Facebook:
                        Kernel.facebookManager.LogIn();
                        break;
                    case eLoginType.Google:
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
                        Kernel.gpgsManager.SignIn();
#endif
                        break;
                    case eLoginType.Guest:
                    case eLoginType.GuestEditer:
                    default : 
                        Kernel.entry.account.REQ_PACKET_CG_AUTH_LOGIN_SYN(loginType, Kernel.uid, string.Empty, string.Empty);
                        break;
                }
            }
        }
    }

    void OnGameCenterLogin(string authToken)
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_LOGIN_SYN(eLoginType.Apple, Kernel.uid, authToken, string.Empty);
    }

    void OnGPGSSignIn(string id, string serverAuthCode)
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_LOGIN_SYN(eLoginType.Google, Kernel.uid, string.Empty, serverAuthCode);
    }

    void OnFacebookLogIn(string userId, string accessToken)
    {
        Debug.Log("userId : " + userId + ", accessToken : " + accessToken);
        Kernel.entry.account.REQ_PACKET_CG_AUTH_LOGIN_SYN(eLoginType.Facebook, Kernel.uid, accessToken, string.Empty);
    }

    void OnRetrieveVerificationSignature(string publicKeyUrl)
    {
        UIAlerter.Alert(publicKeyUrl, UIAlerter.Composition.Confirm);
    }


    void CheckGameVersion()
    {
        Kernel.entry.account.onGetGameVersion += RecvCheckVersion;
        Kernel.entry.account.REQ_PACKET_CG_AUTH_GET_CLIENT_VERSION_SYN();
    }


    void RecvCheckVersion(int verA, int verB, int verC, int verD)
    {
        Kernel.entry.account.onGetGameVersion -= RecvCheckVersion;

        bool CheckValue = false;
        if (Kernel.VerNumber_A == verA && Kernel.VerNumber_B == verB && Kernel.VerNumber_C == verC)
        {
            CheckValue = true;
            Kernel.VerNumber_D = verD;
        }

        if (CheckValue)
        {
            StartCoroutine(Kernel.dataLoader.LoadData());
        }
        else //버전 다를때.
        {
            UIAlerter.Alert(Languages.ToStringBuiltIn(TEXT_UI.NEW_VERSION), UIAlerter.Composition.Confirm_Cancel, OnApplicationUpdateResponded);
        }

        m_VersionText.text = Kernel.GetVersionText();
    }


    void OnApplicationUpdateResponded(UIAlerter.Response response, params object[] args)
    {
        if (response == UIAlerter.Response.Confirm)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.mseedgames.withpuccawars");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }



}
