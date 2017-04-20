using UnityEngine;
using UnityEngine.UI;

public class UIRankingRewardInfoObject : MonoBehaviour
{
    public Image m_ConditionImage;
    public Text m_ConditionText;
    public Text m_RewardText;
    public bool m_IsDaily;
    public int m_Index;

    // Use this for initialization
    void Start()
    {
        if (m_IsDaily)
        {
            DB_DailyReward.Schema dailyReward = DB_DailyReward.Query(DB_DailyReward.Field.Index, m_Index);
            if (dailyReward != null)
            {
                if (m_ConditionImage != null)
                {
                    m_ConditionImage.gameObject.SetActive(false);
                }

                m_ConditionText.text = Languages.ToString(TEXT_UI.MORE, dailyReward.Need_RankingPoint);
                m_RewardText.text = Languages.ToString(TEXT_UI.RUBY_GIVE, dailyReward.Ruby_Obtain);
            }
        }
        else
        {
            DB_SeasonReward.Schema seasonReward = DB_SeasonReward.Query(DB_SeasonReward.Field.Index, m_Index);
            if (seasonReward != null)
            {
                if (m_ConditionImage == null)
                {
                    string measure = string.Empty;
                    switch (seasonReward.Rank_Type)
                    {
                        case Rank_Type.Rank:
                            measure = string.Format("{0}\n{1}~{2}", Languages.ToString(TEXT_UI.RANK), seasonReward.Rank_Min, seasonReward.Rank_Max);
                            break;
                        case Rank_Type.Rate:
                            measure = string.Format("{0}~{1}%", seasonReward.Rank_Min, seasonReward.Rank_Max);
                            break;
                    }

                    m_ConditionText.text = measure;
                }

                m_RewardText.text = Languages.ToString(TEXT_UI.RUBY_GIVE, seasonReward.Ruby_Obtain);
            }
        }
    }

    // Update is called once per frame

}
