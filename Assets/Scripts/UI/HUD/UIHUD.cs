using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : UIObject
{
    #region Singleton
    static UIHUD m_Instance;

    public static UIHUD instance
    {
        get
        {
            return m_Instance;
        }
    }
    #endregion

    private TextLevelMaxEffect m_LevelMaxEffect;

    public GameObject m_Acount;
    public Text m_CurrentPvPAreaText;
    public Sprite[] m_RankingAreaSprite;
    public Text m_RankingPointText;
    public UIGuildFlag m_GuildFlag;
    public Text m_GuildNameText;
    public Button m_RankingButton;
    public Image m_RankingBackGround;
    public UIMiniCharCard m_MiniCharCard;
    public Button m_UserInfoButton;
    public Text m_NameText;
    public Text m_LevelText;
    public GameObject m_LevelMaxImg;
    public UISlider m_ExpSlider;
    public Button m_BackButton;
    public GameObject m_Money;
    public Text m_GoldText;
    public Button m_GoldButton;
    public Text m_RubyText;
    public Button m_RubyButton;
    public Text m_HeartText;
    public Button m_HeartButton;
    public Image m_HeartImage;
    public GameObject m_HeartEffect;
    public Text m_HeartUpdateTimeText;
    public GameObject m_HeartMaxImage;
    public GameObject m_StarContainer;
    public Text m_StarText;
    public GameObject m_GuildPointContainer;
    public Text m_GuildPointText;
    public GameObject m_RevengePointContainer;
    public Text m_RevengePointText;
    public Toggle m_ShortcutToggle;
    public UIShortcut m_Shortcut;
    public UIGoodsView m_GoodsView;

    public UIGoodsRewardAnimation m_goodsRewardAnim;

    public UIEventTime m_EventTime;

    Stack<Scene> m_History = new Stack<Scene>();
    int m_HeartRecoveryCycleSec;

    public delegate bool OnBackButtonClicked();
    public OnBackButtonClicked onBackButtonClicked;

    protected override void Awake()
    {
        m_Instance = this;

        base.Awake();

        //m_MiniCharCard.onClicked += OnMiniCharCardClicked;
        m_UserInfoButton.onClick.AddListener(OnMiniCharCardClicked);
        m_BackButton.onClick.AddListener(OnBackButtonClick);
        m_RankingButton.onClick.AddListener(OnRankingButtonClick);
        m_GoldButton.onClick.AddListener(OnGoldButtonClick);
        m_RubyButton.onClick.AddListener(OnRubyButtonClick);
        m_HeartButton.onClick.AddListener(OnHeartButtonClick);
        m_ShortcutToggle.onValueChanged.AddListener(OnShortcutToggleValueChange);

        m_HeartRecoveryCycleSec = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Heart_Recovery_Cycle_Sec);

        if (m_LevelText != null && m_LevelMaxEffect == null)
            m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit);
    }

    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        //base.Update();

        if (Kernel.entry == null)
        {
            return;
        }

        TimeSpan ts = TimeUtility.currentServerTime - TimeUtility.ToDateTime(Kernel.entry.account.heartLastUpdateTime);
        float value = Mathf.Clamp((float)ts.TotalSeconds, 0, m_HeartRecoveryCycleSec);
        value = Mathf.FloorToInt(value);
        ts = TimeSpan.FromSeconds(m_HeartRecoveryCycleSec - value);

        bool isMaxHeart = Kernel.entry.account.maxHeart <= Kernel.entry.account.heart;

        if (m_HeartMaxImage.activeInHierarchy != isMaxHeart)
        {
            m_HeartMaxImage.SetActive(isMaxHeart);
            m_HeartUpdateTimeText.gameObject.SetActive(!isMaxHeart);
        }

        if (value > 0)
        {
            m_HeartImage.fillAmount = (value / m_HeartRecoveryCycleSec);
            m_HeartUpdateTimeText.text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
            m_HeartEffect.SetActive(false);
        }
        else
        {
            // ref. PUC-868
            m_HeartImage.fillAmount = 1;
            m_HeartUpdateTimeText.text = Languages.ToString(TEXT_UI.LEVEL_MAX);
            m_HeartEffect.SetActive(true);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.onCompleteLoadScene += OnLoadSceneComplete;
        }

        if (Kernel.entry != null)
        {
            Kernel.entry.account.onUserBaseUpdate += OnUserBaseUpdate;
            Kernel.entry.account.onGoodsUpdate += OnGoodsUpdate;

            Renewal();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.onCompleteLoadScene -= OnLoadSceneComplete;
        }

        if (Kernel.entry != null)
        {
            Kernel.entry.account.onUserBaseUpdate -= OnUserBaseUpdate;
            Kernel.entry.account.onGoodsUpdate -= OnGoodsUpdate;
        }
    }

    byte level
    {
        set
        {
            m_LevelText.text = value.ToString();

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.Value = value;
        }
    }

    long exp
    {
        set
        {
            DB_AccountLevel.Schema accountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, Kernel.entry.account.level);
            if (accountLevel != null)
            {
                m_ExpSlider.maxValue = accountLevel.Need_AccountExp;
            }

            m_ExpSlider.value = value;
        }
    }

    new string name
    {
        set
        {
            m_NameText.text = value;
        }
    }

    byte currentPvPArea
    {
        set
        {
            m_CurrentPvPAreaText.text = string.Format("{0} {1}", Languages.ToString(TEXT_UI.CARD_DECK_AREA), value);

            if (m_RankingAreaSprite.Length < value)
                return;

            m_RankingBackGround.sprite = m_RankingAreaSprite[value - 1];
        }
    }

    string guildName
    {
        set
        {
            m_GuildNameText.text = string.IsNullOrEmpty(value) ? Languages.ToString(TEXT_UI.GUILD_NONE) : value;
        }
    }

    int gold
    {
        set
        {
            m_GoldText.text = Languages.MillionFormatter(value);
        }
    }

    int heart
    {
        set
        {
            m_HeartText.text = string.Format("{0}/{1}", (value > 999) ? "999+" : value.ToString(), Kernel.entry.account.maxHeart);
        }
    }

    int ruby
    {
        set
        {
            m_RubyText.text = Languages.MillionFormatter(value);
        }
    }

    int star
    {
        set
        {
            m_StarText.text = Languages.MillionFormatter(value);
        }
    }

    int guildPoint
    {
        set
        {
            m_GuildPointText.text = Languages.MillionFormatter(value);
        }
    }

    int rankingPoint
    {
        set
        {
            m_RankingPointText.text = Languages.ToString<int>(value);
        }
    }

    int leaderCardIndex
    {
        set
        {
            m_MiniCharCard.SetCardInfo(value, 0, 0);
        }
    }

    int revengePoint
    {
        set
        {
            m_RevengePointText.text = Languages.MillionFormatter(value);
        }
    }

    void OnLoadSceneComplete(Scene scene)
    {
        // 같은 씬에서 이동한 경우, 처리하지 않습니다.
        bool duplicated = false;
        if (m_History.Count > 0)
        {
            if (m_History.Peek() == scene)
            {
                duplicated = true;
            }
        }

        if (!duplicated)
        {
            m_History.Push(scene);

            switch (scene)
            {
                case Scene.Lobby:
                    m_Acount.gameObject.SetActive(true);
                    m_BackButton.gameObject.SetActive(false);
                    break;
                default:
                    m_Acount.gameObject.SetActive(false);
                    m_BackButton.gameObject.SetActive(true);
                    break;
            }

            m_StarContainer.SetActive(!Equals(Scene.Guild, scene) && !Equals(Scene.RevengeBattle, scene) && !Equals(Scene.StrangeShop, scene) && !Equals(Scene.Franchise, scene));
            m_GuildPointContainer.SetActive(Equals(Scene.Guild, scene));
            m_RevengePointContainer.SetActive(Equals(Scene.RevengeBattle, scene) || Equals(Scene.StrangeShop, scene));
            m_GoodsView.gameObject.SetActive(Equals(Scene.Franchise, scene));
        }

        // 임시
        Renewal();

        //튜토리얼.
        if (scene == Scene.Lobby)
        {
            if (Kernel.entry.tutorial.GroupNumber < 10000)  //종료가 아닐떄.
                Kernel.uiManager.Open(UI.Tutorial);
        }

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive)
        {
            if (scene == Scene.Lobby)
            {
                Kernel.entry.tutorial.onUpdateTutorialUI();
            }

            switch (Kernel.entry.tutorial.WaitSeq)
            {
                case 304:
                    Kernel.entry.tutorial.onSetNextTutorial();
                    break;
            }
        }

        m_EventTime.SetInit();
    }

    void OnUserBaseUpdate(byte level, long exp, string name, byte currentPvPArea, int leaderCardIndex)
    {
        this.level = level;
        this.exp = exp;
        this.name = name;
        this.currentPvPArea = currentPvPArea;
        this.guildName = Kernel.entry.account.guildName;
        this.leaderCardIndex = leaderCardIndex;

        string flagSpriteName = string.IsNullOrEmpty(Kernel.entry.account.guildEmblem) ? "ui_user_guild_02" : "ui_user_guild_01";
        m_GuildFlag.SetGuildEmblem(flagSpriteName, Kernel.entry.account.guildEmblem);

        // 레벨 업했을 때, 최대 하트 값 갱신 임시 처리
        heart = Kernel.entry.account.heart;
    }

    void OnGoodsUpdate(int friendship, int gold, int heart, int ranking, int ruby, int star, int guildPoint, int revengePoint, int smilePoint)
    {
        this.gold = gold;
        this.heart = heart;
        this.rankingPoint = ranking;
        this.ruby = ruby;
        this.star = star;
        this.guildPoint = guildPoint;
        this.revengePoint = revengePoint;

        m_GoodsView.OnGoodsUpdate();
    }

    void Renewal()
    {
        if (Kernel.entry != null)
        {
            OnUserBaseUpdate(Kernel.entry.account.level,
                             Kernel.entry.account.exp,
                             Kernel.entry.account.name,
                             Kernel.entry.account.currentPvPArea,
                             Kernel.entry.account.leaderCardIndex);
            OnGoodsUpdate(Kernel.entry.account.friendshipPoint,
                          Kernel.entry.account.gold,
                          Kernel.entry.account.heart,
                          Kernel.entry.account.rankingPoint,
                          Kernel.entry.account.ruby,
                          Kernel.entry.account.starPoint,
                          Kernel.entry.account.guildPoint,
                          Kernel.entry.account.revengePoint,
                          Kernel.entry.account.smilePoint);
            guildName = Kernel.entry.account.guildName;

            m_GoodsView.OnGoodsUpdate();

        }
    }

    void OnMiniCharCardClicked()
    {
        if (Kernel.entry != null)
        {
            //튜토리얼 방어.
            if (Kernel.entry.account.TutorialGroup <= 60)
                return;

            Kernel.entry.ranking.REQ_PACKET_CG_READ_DETAIL_USER_INFO_SYN(Kernel.entry.account.userNo);
        }
    }

    void OnBackButtonClick()
    {
        if (onBackButtonClicked != null)
        {
            if (onBackButtonClicked())
            {
                onBackButtonClicked = null;
            }
        }
        else
        {
            if (m_History.Count > 0)
            {
                m_History.Pop(); // 임시
                Scene scene = m_History.Pop();
                SceneObject activeSceneObject = Kernel.sceneManager.activeSceneObject;
                if (activeSceneObject != null && activeSceneObject.scene != scene)
                {
                    Kernel.sceneManager.LoadScene(scene);
                }
            }
        }
    }

    void OnRankingButtonClick()
    {
        if (Kernel.sceneManager)
        {
            //튜토리얼 방어.
            if (Kernel.entry.account.TutorialGroup <= 60)
                return;

            Kernel.sceneManager.LoadScene(Scene.Ranking);
        }
    }

    void OnGoldButtonClick()
    {
        if (Kernel.entry == null || Kernel.sceneManager == null)
            return;

        //튜토리얼 방어.
        if (Kernel.entry.account.TutorialGroup <= 60)
            return;

        Kernel.entry.normalShop.m_eCurrentTabType = eNormalShopItemType.NSI_GOLD;
        OnOpenShop();
    }

    void OnRubyButtonClick()
    {
        if (Kernel.entry == null || Kernel.sceneManager == null)
            return;

        //튜토리얼 방어.
        if (Kernel.entry.account.TutorialGroup <= 60)
            return;

        Kernel.entry.normalShop.m_eCurrentTabType = eNormalShopItemType.NSI_RUBBY;
        OnOpenShop();

    }

    void OnHeartButtonClick()
    {
        if (Kernel.entry == null || Kernel.sceneManager == null)
            return;

        //튜토리얼 방어.
        if (Kernel.entry.account.TutorialGroup <= 60)
            return;

        Kernel.entry.normalShop.m_eCurrentTabType = eNormalShopItemType.NSI_HEART;
        OnOpenShop();
    }

    void OnOpenShop()
    {
        if (Kernel.entry == null || Kernel.sceneManager == null)
            return;

        //튜토리얼 방어.
        if (Kernel.entry.account.TutorialGroup <= 60)
            return;

        if (Equals(Kernel.sceneManager.activeSceneObject.scene.ToString(), Scene.NormalShop.ToString()))
            Kernel.entry.normalShop.onCreatNormalShopItem();
        else
            Kernel.sceneManager.LoadScene(Scene.NormalShop);
    }

    void OnShortcutToggleValueChange(bool value)
    {
        //튜토리얼 방어.
        if (Kernel.entry.account.TutorialGroup <= 60)
            return;

        Kernel.uiManager.Toggle(UI.Shortcut, true);
    }

    //** 재화 아이템 보상 받기 연출
    public void UseGoodsRewardAnimation(Vector3 startPos, List<GoodsRewardAnimationData> listGoodsAnimData)
    {
        if (m_goodsRewardAnim != null)
            m_goodsRewardAnim.Setting(startPos, listGoodsAnimData);
    }
}
