using UnityEngine;
using UnityEngine.UI;

public class UIShortcutObject : MonoBehaviour
{
    public Button   m_Button;
    public Text     m_NameText;
    public Image    m_IconImage;
    public GameObject   m_NewIcon;
    public GameObject   m_LockIcon;

    private int     m_Level;
    private int     m_TutorialGroup;

    [SerializeField]
    ShortCutType m_ShortcutType;

    void Awake()
    {
        m_Button.onClick.AddListener(OnClicked);
    }

    void OnEnable()
    {
        ResetButton();
        InitShortcutButton();
    }

    // Use this for initialization
    void InitShortcutButton()
    {
        //레벨제한용.
        m_Level = 1;
        m_TutorialGroup = 10;

        switch (m_ShortcutType)
        {
            case ShortCutType.ShortCut_Lobby:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_LOBBY);
                break;
            case ShortCutType.ShortCut_Card:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_CARD);
                break;
            case ShortCutType.ShortCut_Achieve:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_ACHIEVE);
                break;
            case ShortCutType.ShortCut_Ranking:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_RANKING);
                break;
            case ShortCutType.ShortCut_GuildInfo:
                m_Level = 4;
                m_TutorialGroup = 100;
                m_NameText.text = Languages.ToString(TEXT_UI.SC_GUILDINFO);
                break;
            case ShortCutType.ShortCut_Adventure:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_ADVENTURE);
                break;
            case ShortCutType.ShortCut_RevengeBattle:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_REVENGBATTLE);
                break;
            case ShortCutType.ShortCut_Treasure:
                m_Level = 3;
                m_TutorialGroup = 80;
                m_NameText.text = Languages.ToString(TEXT_UI.SC_TREASURE);
                break;
            case ShortCutType.ShortCut_Franchise:
                m_Level = 3;
                m_TutorialGroup = 90;
                m_NameText.text = Languages.ToString(TEXT_UI.SC_FRANCHISE);
                break;
            case ShortCutType.ShortCut_Treasure_Detect:
                m_TutorialGroup = 70;
                m_NameText.text = Languages.ToString(TEXT_UI.SC_TREASURE_DETECT);
                break;
            case ShortCutType.ShortCut_SecretExchange:
                m_Level = 5;
                m_TutorialGroup = 110;
                m_NameText.text = Languages.ToString(TEXT_UI.SC_SECRET_EXCHANGE);
                break;
            case ShortCutType.ShortCut_StrangeShop:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_STRANGESHOP);
                break;
            case ShortCutType.ShortCut_ShopP:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_SHOP);
                break;
            case ShortCutType.ShortCut_Option:
                m_NameText.text = Languages.ToString(TEXT_UI.SC_OPTION);
                break;

        }

        //버튼숨기기.
        HideShortcutButton();
    }

    // Update is called once per frame

    void OnClicked()
    {
        if (Kernel.uiManager)
        {
            if (Kernel.entry.account.TutorialGroup <= m_TutorialGroup)  //그룹으로 체크.
            {
                UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.SC_DISABLED_ICON, m_Level));
                return;
            }


            Kernel.uiManager.Close(UI.Shortcut);

            switch (m_ShortcutType)
            {
                case ShortCutType.ShortCut_Lobby:
                    Kernel.sceneManager.LoadScene(Scene.Lobby);
                    break;
                case ShortCutType.ShortCut_Card:
                    Kernel.sceneManager.LoadScene(Scene.Deck);
                    break;
                case ShortCutType.ShortCut_Achieve:
                    Kernel.sceneManager.LoadScene(Scene.Achieve);
                    break;
                case ShortCutType.ShortCut_Ranking:
                    Kernel.sceneManager.LoadScene(Scene.Ranking);
                    break;
                case ShortCutType.ShortCut_GuildInfo:
                    Kernel.sceneManager.LoadScene(Scene.Guild);
                    break;
                case ShortCutType.ShortCut_Adventure:
                    Kernel.sceneManager.LoadScene(Scene.Adventure);
                    break;
                case ShortCutType.ShortCut_RevengeBattle:
                    Kernel.sceneManager.LoadScene(Scene.RevengeBattle);
                    break;
                case ShortCutType.ShortCut_Treasure:
                    Kernel.sceneManager.LoadScene(Scene.Treasure);
                    break;
                case ShortCutType.ShortCut_Franchise:
                    Kernel.sceneManager.LoadScene(Scene.Franchise);
                    break;
                case ShortCutType.ShortCut_Treasure_Detect:
                    Kernel.sceneManager.LoadScene(Scene.Detect);
                    break;
                case ShortCutType.ShortCut_SecretExchange:
                    Kernel.sceneManager.LoadScene(Scene.SecretBusiness);
                    break;
                case ShortCutType.ShortCut_StrangeShop:
                    Kernel.sceneManager.LoadScene(Scene.StrangeShop);
                    break;
                case ShortCutType.ShortCut_ShopP:
                    Kernel.sceneManager.LoadScene(Scene.NormalShop);
                    Kernel.entry.normalShop.m_eCurrentTabType = eNormalShopItemType.NSI_PACKAGE;
                    break;
                case ShortCutType.ShortCut_Option:
                    UIOption option = Kernel.uiManager.Get<UIOption>(UI.Option, true, false);
                    if(option != null)
                        Kernel.uiManager.Open(UI.Option);
                    break;
            }
        }
    }




    void HideShortcutButton()
    {
        bool HideMode = false;

        switch (m_ShortcutType)
        {
            case ShortCutType.ShortCut_Card:
                if (Kernel.entry.account.TutorialGroup <= 40)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_Achieve:
            case ShortCutType.ShortCut_Ranking:
            case ShortCutType.ShortCut_RevengeBattle:
            case ShortCutType.ShortCut_StrangeShop:
            case ShortCutType.ShortCut_ShopP:
                if (Kernel.entry.account.TutorialGroup <= 60)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_GuildInfo:
                if (Kernel.entry.account.TutorialGroup <= 100)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_Adventure:
                if (Kernel.entry.account.TutorialGroup <= 30)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_Franchise:
                if (Kernel.entry.account.TutorialGroup <= 90)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_Treasure_Detect:
                if (Kernel.entry.account.TutorialGroup <= 70)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_Treasure:
                if (Kernel.entry.account.TutorialGroup <= 80)
                    HideMode = true;
                break;

            case ShortCutType.ShortCut_SecretExchange:
                if (Kernel.entry.account.TutorialGroup <= 110)
                    HideMode = true;
                break;
        }

        if (HideMode)
        {
            m_Button.GetComponent<Image>().color = Color.gray;
            m_IconImage.color = Color.gray;
            m_NameText.color = new Color(0.15f, 0.15f, 0.15f, 1.0f);
            /*
            if (m_NewIcon != null)
                m_NewIcon.SetActive(false);
            */
            if (m_LockIcon != null)
                m_LockIcon.SetActive(true);

        }
    }


    void ResetButton()
    {
        m_Button.GetComponent<Image>().color = Color.white;
        m_IconImage.color = Color.white;
        m_NameText.color = new Color(0.4f, 0.38f, 0.37f, 1.0f);
        /*
        if (m_NewIcon != null)
            m_NewIcon.SetActive(true);
        */
        if (m_LockIcon != null)
            m_LockIcon.SetActive(false);
    }

}
