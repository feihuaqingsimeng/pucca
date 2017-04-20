using UnityEngine;
using UnityEngine.UI;

public enum UI
{
    None,
    Alerter,
    HUD,
    Lobby,
    Loading,
    NotificationCenter,
    Shortcut,
    Title,
    CharCardOption,
    Tooltip,
    Deck,
    Battle,
    CardInfo,
    BattleStart,
    BattleResult,
    Indicator,
    ChestInfo,
    ChestDirector,
    GuildEnter,
    GuildEditor,
    Guild,
    GuildMemberList,
    GuildShop,
    GuildCardRequest,
    GuildShopHelp,
    GuildDonation,
    GuildReceiveCardInfo,
    GuildIntroduceEdit,
    GuildInfo,
    Adventure,
    AdventureInfo,
    AdventureSweep,
    AdventureResult,
    Treasure,
    LevelUp,
    Ranking,
    RankingSeasonRewardInfo,
    RankingDailyRewardInfo,
    UserInfo,
    Promotion,
    Post,
    Achieve,
    AchieveNotification,
    PopupReceive,
    NormalShop,
    PopupBuy,
    StrangeShop,
    StrangeShopDirector,
    SecretBoxSelect,
    SecretBusiness,
    SecretCardHelp,
    SecretCardInfo,
    RevengeBattle,
    DeckEditPopup,
    RevengeResult,
    MileageDirector,
    AccessTerms,
    AccountInterconnector,
    NicknameEditor,
    Franchise,
    FranchiseInfo,
    StrangeShopOption,
    Option,
    LanguageSeletion,
    CouponCheck,
    DropOutAccount,
    Detect,
    DetectAR,
    DetectChestDirection,
    DetectManual,
    DetectPopup,
    Tutorial,
    PackageInfo,
    SupportCheck,
    UIDEnter,
    Notice,
}

public class UIObject : MonoBehaviour
{
    public UI ui
    {
        get;
        set;
    }

    [SerializeField]
    int m_SortingOrder;

    public int sortingOrder
    {
        get
        {
            return m_SortingOrder;
        }
    }

    [SerializeField]
    bool m_BlurOut;

    public bool blurOut
    {
        get
        {
            return m_BlurOut;
        }
    }

    [SerializeField]
    bool m_Destroyable;

    public bool destroyable
    {
        get
        {
            return m_Destroyable;
        }
    }

    RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get
        {
            if (!m_RectTransform)
            {
                m_RectTransform = transform as RectTransform;
            }

            return m_RectTransform;
        }
    }

    public virtual bool active
    {
        get
        {
            return gameObject.activeSelf;
        }

        set
        {
            if (gameObject.activeSelf != value)
            {
                gameObject.SetActive(value);
            }
        }
    }

    [SerializeField]
    Animator m_Animator;

    protected Animator animator
    {
        get
        {
            return m_Animator;
        }
    }

    [SerializeField]
    protected Button m_CloseButton;

    public delegate void OnAnimationEvent(UIObject obj, string triggerName);
    public OnAnimationEvent onAnimationEvent;

    protected virtual void Reset()
    {
        if (m_Animator == null)
        {
            m_Animator = GetComponent<Animator>();
        }
    }

    protected virtual void Awake()
    {
        if (m_CloseButton)
        {
            m_CloseButton.onClick.AddListener(OnCloseButtonClick);
        }
    }

    // Use this for initialization
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void OnDestroy()
    {

    }

    public void SetTrigger(string triggerName)
    {
        if (m_Animator != null)
        {
            m_Animator.ResetTrigger("Normal");
            m_Animator.ResetTrigger("Popup_open_ani");
            m_Animator.ResetTrigger("Popup_Close_ani");
            m_Animator.SetTrigger(triggerName);
        }
        else
        {
            AnimationEvent(triggerName);
        }
    }

    public virtual void AnimationEvent(string triggerName)
    {
        if (onAnimationEvent != null)
        {
            onAnimationEvent(this, triggerName);
        }
    }

    protected virtual void OnCloseButtonClick()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Close(ui);
        }
    }

    public bool IsAnimation(string triggerName)
    {
        if (m_Animator != null)
            return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(triggerName);

        return false;
    }


    public bool CheckAnimator()
    {
        if (m_Animator == null)
            return false;

        return true;
    }

}
