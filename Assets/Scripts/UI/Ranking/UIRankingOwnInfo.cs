using Common.Packet;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIRankingOwnInfo : MonoBehaviour
{
    public UIMiniCharCard m_charCard;
    public Text m_NameText;
    public Text m_RankingText;
    public Text m_RankingRateText;
    public Text m_RankingPointText;
    public Text m_TotalPlayCountText;
    public Text m_WinCountText;
    public Button m_SeasonRewardButton;
    public Button m_SeasonReawrdHelpButton;
    public UISlider m_SeasonTimeSlider;
    public Text m_SeasonTimeText;
    public Button m_DailyRewardButton;
    public UISlider m_DailyTimeSlider;
    public Text m_DailyTimeText;
    public Button m_DailyRewardHelpButton;
    DateTime m_SeasonStartTime;
    DateTime m_SeasonEndTime;
    DateTime m_DayEndTime;

    void Awake()
    {
        m_SeasonRewardButton.onClick.AddListener(OnSeasonRewardButtonClick);
        m_SeasonReawrdHelpButton.onClick.AddListener(OnSeasonRewardHelpButtonClick);
        m_DailyRewardButton.onClick.AddListener(OnDailyRewardButtonClick);
        m_DailyRewardHelpButton.onClick.AddListener(OnDailyRewardHelpButtonClick);
    }

    // Use this for initialization
    void Start()
    {
        m_DayEndTime = DateTime.Today.AddDays(1);
        m_SeasonTimeSlider.maxValue = (float)TimeSpan.FromDays(Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Ranking_Reset_Cycle_Day)).TotalSeconds;
        m_DailyTimeSlider.maxValue = (float)TimeSpan.FromDays(1).TotalSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (Kernel.entry == null)
        {
            return;
        }

        // *일 *시간 전, 24시간 이하인 경우 *:*:*
        TimeSpan ts = m_SeasonEndTime - TimeUtility.currentServerTime;
        m_SeasonTimeSlider.value = m_SeasonTimeSlider.maxValue - (float)ts.TotalSeconds;
        m_SeasonTimeText.text = Languages.TimerString(ts);

        ts = m_DayEndTime - TimeUtility.currentServerTime;
        m_DailyTimeSlider.value = m_DailyTimeSlider.maxValue - (float)ts.TotalSeconds;
        m_DailyTimeText.text = Languages.TimerString(ts);
    }

    void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onRankingInfoUpdate += OnRankingInfoUpdate;
            Kernel.entry.ranking.onDailyRewardResult += OnDailyRewardResult;
            Kernel.entry.ranking.onSeasonRewardResult += OnSeasonRewardResult;

            Renewal();
        }
    }

    void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onRankingInfoUpdate -= OnRankingInfoUpdate;
            Kernel.entry.ranking.onDailyRewardResult -= OnDailyRewardResult;
            Kernel.entry.ranking.onSeasonRewardResult -= OnSeasonRewardResult;
        }
    }

    void OnSeasonRewardResult(int ruby)
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.RUBY_OBTAIN, ruby), UIAlerter.Composition.Confirm);
    }

    void OnDailyRewardResult(int ruby)
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.RUBY_OBTAIN, ruby), UIAlerter.Composition.Confirm);
    }

    void OnRankingInfoUpdate(CFranchiseRankingInfo rankingInfo)
    {
        if (rankingInfo != null)
        {
            m_RankingText.text = rankingInfo.m_Rank.ToString();
            UIUtility.FitSizeToContent(m_RankingText);
            float rate = Mathf.Min(((float)rankingInfo.m_Rank / (float)rankingInfo.m_TotalRanker) * 100f, 100f);
            m_RankingRateText.text = string.Format("{0} ({1:F0}%)", Languages.Ordinal(rankingInfo.m_Rank), rate);
            UIUtility.FitSizeToContent(m_RankingRateText);
            float x = -(m_RankingRateText.rectTransform.anchoredPosition.x + (m_RankingRateText.rectTransform.sizeDelta.x * .5f));
            m_RankingText.rectTransform.anchoredPosition = new Vector2(x, m_RankingText.rectTransform.anchoredPosition.y);

            m_TotalPlayCountText.text = Languages.ToString(rankingInfo.m_iTotalPlayCount);
            m_WinCountText.text = Languages.ToString(rankingInfo.m_iWinCount);
        }
    }

    void Renewal()
    {
        if (Kernel.entry != null)
        {
            m_charCard.SetCardInfo(Kernel.entry.account.leaderCardIndex, 0, 0);
            m_NameText.text = Kernel.entry.account.name;
            m_RankingPointText.text = Languages.ToString(Kernel.entry.account.rankingPoint);
            OnRankingInfoUpdate(Kernel.entry.ranking.rankingInfo);

            m_SeasonStartTime = TimeUtility.ToDateTime(Kernel.entry.ranking.seasonRewardTime);
            m_SeasonEndTime = m_SeasonStartTime.AddDays(Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Ranking_Reset_Cycle_Day));
        }
    }

    void OnSeasonRewardButtonClick()
    {
        if (Kernel.entry != null)
        {
            if (Kernel.entry.ranking.seasonRankReward.m_bIsObtain)
            {
                SoundDataInfo.CancelSound(m_SeasonRewardButton.gameObject);
                NetworkEventHandler.OnNetworkException(Result_Define.eResult.ALREADY_OBTAIN_REWARD);
            }
            else if (Kernel.entry.ranking.seasonRankReward.m_iObtainRuby <= 0)
            {
                SoundDataInfo.CancelSound(m_SeasonRewardButton.gameObject);
                NetworkEventHandler.OnNetworkException(Result_Define.eResult.NO_REWARD_TO_OBTAIN);
            }
            else
            {
                SoundDataInfo.RevertSound(m_SeasonRewardButton.gameObject);
                Kernel.entry.ranking.REQ_PACKET_CG_RANK_RECEIVE_SEASON_REWARD_SYN();
            }
        }
    }

    void OnSeasonRewardHelpButtonClick()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.RankingSeasonRewardInfo);
        }
    }

    void OnDailyRewardButtonClick()
    {
        if (Kernel.entry != null)
        {
            if (Kernel.entry.ranking.dailyRankReward.m_bIsObtain)
            {
                SoundDataInfo.CancelSound(m_DailyRewardButton.gameObject);
                NetworkEventHandler.OnNetworkException(Result_Define.eResult.ALREADY_OBTAIN_REWARD);
            }
            else if (Kernel.entry.ranking.dailyRankReward.m_iObtainRuby <= 0)
            {
                SoundDataInfo.CancelSound(m_DailyRewardButton.gameObject);
                NetworkEventHandler.OnNetworkException(Result_Define.eResult.NO_REWARD_TO_OBTAIN);
            }
            else
            {
                SoundDataInfo.RevertSound(m_DailyRewardButton.gameObject);
                Kernel.entry.ranking.REQ_PACKET_CG_RANK_RECEIVE_DAILY_REWARD_SYN();
            }
        }
    }

    void OnDailyRewardHelpButtonClick()
    {
        if (Kernel.uiManager)
        {
            Kernel.uiManager.Open(UI.RankingDailyRewardInfo);
        }
    }
}
