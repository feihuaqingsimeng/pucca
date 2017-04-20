using UnityEngine;
using UnityEngine.UI;

public class UIRankingOwnGuildInfo : MonoBehaviour
{
    public GameObject m_Container;
    public UIGuildFlag m_GuildFlag;
    public Text m_GuildNameText;
    public Text m_GuildLeaderNameText;
    public Text m_GuildRankingText;
    public Text m_GuildRankingRateText;
    public Text m_GuildMemberCountText;
    public Text m_GuildTotalSupportedCardCountText;
    public Text m_GuildRankingPointText;
    public Text m_SupportCardCountText;
    public Text m_RankingPointText;
    public Text m_EmptyText;

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Renewal();
        }
    }

    void Renewal()
    {
        if (Kernel.entry != null)
        {
            if (Kernel.entry.guild.gid != 0)
            {
                m_GuildFlag.SetGuildEmblem(Kernel.entry.guild.guildEmblem);
                m_GuildNameText.text = Kernel.entry.guild.guildName;
                m_GuildLeaderNameText.text = Kernel.entry.guild.guildHeadName;
                m_GuildRankingText.text = Kernel.entry.guild.guildRanking.ToString();
                UIUtility.FitSizeToContent(m_GuildRankingText);
                float rate = Mathf.Min(((float)Kernel.entry.guild.guildRanking / (float)Kernel.entry.guild.totalRankedGuildCount) * 100f, 100f);
                m_GuildRankingRateText.text = string.Format("{0} ({1:F0}%)", Languages.Ordinal(Kernel.entry.guild.guildRanking), rate);
                UIUtility.FitSizeToContent(m_GuildRankingRateText);
                float x = -(m_GuildRankingRateText.rectTransform.anchoredPosition.x + (m_GuildRankingRateText.rectTransform.sizeDelta.x * .5f));
                m_GuildRankingText.rectTransform.anchoredPosition = new Vector2(x, m_GuildRankingText.rectTransform.anchoredPosition.y);

                m_GuildMemberCountText.text = string.Format("{0}/{1}", Kernel.entry.guild.memberCount, Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Guild_Member_Limit));
                m_GuildTotalSupportedCardCountText.text = Languages.ToString(Kernel.entry.guild.totalSupportedCardCount);
                m_GuildRankingPointText.text = Languages.ToString(Kernel.entry.guild.guildRankingPoint);
                m_SupportCardCountText.text = Languages.ToString(Kernel.entry.account.supportCardCount);
                m_RankingPointText.text = Languages.ToString(Kernel.entry.account.rankingPoint);
            }

            m_Container.SetActive(Kernel.entry.guild.gid != 0);
            m_EmptyText.gameObject.SetActive(Kernel.entry.guild.gid == 0);
        }
    }
}
