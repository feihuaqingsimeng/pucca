using UnityEngine;

public class Kernel : MonoBehaviour, ILogger
{
    //버전.
    public static int VerNumber_A = 1;    //릴리즈, 시즌업데이트.
    public static int VerNumber_B = 0;    //대형 업데이트.
    public static int VerNumber_C = 0;    //버그수정 및 신규빌드.
    public static int VerNumber_D = 0;    //데이터패치.




    #region Singleton
    static Entry m_Entry;

    public static Entry entry
    {
        get
        {
            return m_Entry;
        }
    }

    public static SceneManager sceneManager
    {
        get;
        private set;
    }

    public static CanvasManager canvasManager
    {
        get;
        private set;
    }
    /*
    public static DataManager dataManager
    {
        get;
        private set;
    }

    public static GameManager gameManager
    {
        get;
        private set;
    }
    */
    public static NetworkManager networkManager
    {
        get;
        private set;
    }
    /*
    public static SoundManager soundManager
    {
        get;
        private set;
    }
    */
    public static TextureManager textureManager
    {
        get;
        private set;
    }

    public static UIManager uiManager
    {
        get;
        private set;
    }

    public static DataLoader dataLoader
    {
        get;
        private set;
    }

    public static ColorManager colorManager
    {
        get;
        private set;
    }

    public static SoundManager soundManager
    {
        get;
        private set;
    }

    public static PacketRequestIterator packetRequestIterator
    {
        get;
        private set;
    }

    public static AchieveManager achieveManager
    {
        get;
        private set;
    }

    public static NetworkEventHandler networkEventHandler
    {
        get;
        private set;
    }

    public static FacebookManager facebookManager
    {
        get;
        private set;
    }

    public static AppleGameCenterManager gamecenterManager
    {
        get;
        private set;
    }

#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
    public static GPGSManager gpgsManager
    {
        get;
        private set;
    }
#endif

    public static IAPManager iapManager
    {
        get;
        private set;
    }

    public static NotificationManager notificationManager
    {
        get;
        private set;
    } public static AdministratorWindowMobile administratorWindowMobile
    {
        get;
        private set;
    }
    #endregion

    private const string STR_LANGUAGE_KEY = "LANGUAGET_KEY";

    // 임시
    public GAME_SERVER_TYPE m_GameServerType;
    public static GAME_SERVER_TYPE gameServerType;

    public string m_URL_COMMON;
    public static string URL_COMMON;

    public int m_PORT_COMMON;
    public static int PORT_COMMON;

    public bool m_UseClientTable;
    public static bool UseClientTable;

    public bool m_PacketLogEnable;
    public static bool packetLogEnable;

    private static LanguageCode m_eLanguageCode;
    public static LanguageCode languageCode
    {
        get { return m_eLanguageCode; }
        set
        {
            m_eLanguageCode = value;
            PlayerPrefs.SetInt(STR_LANGUAGE_KEY, (int)value);
            PlayerPrefs.Save();
        }
    }

    public string m_UID;
    public static string uid;

    void Awake()
    {
        /*
#if !UNITY_EDITOR
        Debug.logger.logEnabled = false;
#endif
        */
        Debug.Log(Application.temporaryCachePath);
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        Screen.SetResolution(1280, 720, Screen.fullScreen);
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        m_Entry = new Entry(this);

        gameServerType = m_GameServerType;
        URL_COMMON = m_URL_COMMON;
        PORT_COMMON = m_PORT_COMMON;
        UseClientTable = false;
#if UNITY_EDITOR
        UseClientTable = m_UseClientTable;
#endif
        packetLogEnable = m_PacketLogEnable;

        if (PlayerPrefs.HasKey(STR_LANGUAGE_KEY))
        {
            int languageKey = PlayerPrefs.GetInt(STR_LANGUAGE_KEY);
            languageCode = (LanguageCode)languageKey;
        }
        else
            languageCode = GetSystemLanguageCode();

        //Debug.Log("System Language : " + Application.systemLanguage + ", Language Code : " + languageCode + " (Use System Language : " + m_UseSystemLanguage + ")");
        Debug.Log("System Language : " + Application.systemLanguage + ", Language Code : " + languageCode);

        if (string.IsNullOrEmpty(m_UID))
        {
            m_UID = SystemInfo.deviceUniqueIdentifier;
        }

        uid = m_UID;
    }

    // Use this for initialization
    void Start()
    {
        CreateSingletonInstance();

        if (sceneManager != null)
        {
            sceneManager.LoadScene(Scene.TitleScene);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.Update();
            Kernel.entry.post.Update();
            Kernel.entry.treasure.Update();
            Kernel.entry.franchise.Update();
        }
    }

    public static void Reload()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < gameObjects.Length; i++)
        {
            GameObject gameObject = gameObjects[i];
            if (gameObject != null)
            {
                if (IAPManager.Instance.gameObject == gameObject)
                {
                    continue;
                }

                DestroyImmediate(gameObject);
            }
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Kernel");
    }

    void OnApplicationQuit()
    {
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    void CreateSingletonInstance()
    {
        sceneManager = SceneManager.CreateInstance();
        canvasManager = CanvasManager.CreateInstance();
        dataLoader = DataLoader.CreateInstance();
        soundManager = SoundManager.CreateInstance();
        networkManager = NetworkManager.CreateInstance();
        textureManager = TextureManager.CreateInstance();
        uiManager = UIManager.CreateInstance();
        colorManager = ColorManager.CreateInstance();
        packetRequestIterator = PacketRequestIterator.CreateInstance();
        achieveManager = AchieveManager.CreateInstance();
        networkEventHandler = NetworkEventHandler.CreateInstance();
        facebookManager = FacebookManager.CreateInstance();
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        gpgsManager = GPGSManager.CreateInstance();
#endif
        gamecenterManager = AppleGameCenterManager.CreateInstance();
        iapManager = IAPManager.CreateInstance();
        notificationManager = NotificationManager.CreateInstance();
        //administratorWindowMobile = AdministratorWindowMobile.CreateInstance();
    }

    LanguageCode GetSystemLanguageCode()
    {
        LanguageCode languageCode = LanguageCode.English;
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseSimplified:
            case SystemLanguage.ChineseTraditional:
                languageCode = LanguageCode.Chinese;
                break;
            case SystemLanguage.English:
                languageCode = LanguageCode.English;
                break;
            case SystemLanguage.French:
                languageCode = LanguageCode.French;
                break;
            case SystemLanguage.German:
                languageCode = LanguageCode.German;
                break;
            case SystemLanguage.Indonesian:
                languageCode = LanguageCode.Indonesian;
                break;
            case SystemLanguage.Japanese:
                languageCode = LanguageCode.Japanese;
                break;
            case SystemLanguage.Korean:
                languageCode = LanguageCode.Korean;
                break;
            case SystemLanguage.Portuguese:
                languageCode = LanguageCode.Portuguese;
                break;
            case SystemLanguage.Russian:
                languageCode = LanguageCode.Russian;
                break;
            case SystemLanguage.Spanish:
                languageCode = LanguageCode.Spanish;
                break;
            case SystemLanguage.Thai:
                languageCode = LanguageCode.Thai;
                break;
            case SystemLanguage.Turkish:
                languageCode = LanguageCode.Turkish;
                break;
            case SystemLanguage.Vietnamese:
                languageCode = LanguageCode.Vietnamese;
                break;
        }

        return languageCode;
    }



    public static string GetVersionText()
    {
        string VerType;
        switch(Kernel.gameServerType)
        {
            case GAME_SERVER_TYPE.TYPE_DEV:
                VerType = "D";
                break;
            case GAME_SERVER_TYPE.TYPE_QA:
                VerType = "Q";
                break;
            case GAME_SERVER_TYPE.TYPE_ADHOC:
                VerType = "A";
                break;
            case GAME_SERVER_TYPE.TYPE_RELEASE:
                VerType = "R";
                break;
            default:
                VerType = "Manual";
                break;
        }

        return string.Format("{0}.{1}.{2}.{3}_{4}", VerNumber_A, VerNumber_B, VerNumber_C, VerNumber_D, VerType);
    }


    #region ILogger
    public void Log(string format, params object[] args)
    {
        Debug.Log(string.Format(format, args));
    }

    public void LogWarning(string format, params object[] args)
    {
        Debug.LogWarning(string.Format(format, args));
    }

    public void LogError(string format, params object[] args)
    {
        Debug.LogError(string.Format(format, args));
    }
    #endregion
}
