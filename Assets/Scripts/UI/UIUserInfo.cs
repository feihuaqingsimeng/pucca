using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUserInfo : UIObject
{
    private TextLevelMaxEffect m_LevelMaxEffect;
    public RectTransform m_rtrsTitleParent;

    public UIGuildFlag m_GuildFlag;
    public Text m_LevelText;
    public Text m_NameText;
    public Text m_GuildNameText;
    public Text m_RankingPointText;
    public Text m_SupportCardCountText;
    public Text m_TotalPlayCountText;
    public Text m_WinCountText;
    public Text m_CurrentPvPAreaText;
    public List<UIMiniCharCard> m_MiniCharCardList;
    public Image m_LeaderImage;

    // Use this for initialization

    // Update is called once per frame

    protected override void Awake()
    {
        base.Awake();

      
    }

    public void SetUserInfo(UserInfo userInfo)
    {
        if (userInfo != null)
        {
            // gid를 알 수 없어 guildName으로 isOwnGuildEmblem 여부를 판단합니다.
            m_GuildFlag.SetGuildEmblem(userInfo.guildEmblem);
            m_LevelText.text = Languages.LevelString(userInfo.accountLevel);
            m_NameText.text = userInfo.userName;
            m_GuildNameText.text = userInfo.guildName;
            m_RankingPointText.text = Languages.ToString(userInfo.rankingPoint);
            m_SupportCardCountText.text = Languages.ToString(userInfo.guildSupportCardCount);
            m_TotalPlayCountText.text = Languages.ToString(userInfo.rankingInfo.m_iTotalPlayCount);
            m_WinCountText.text = Languages.ToString(userInfo.rankingInfo.m_iWinCount);
            m_CurrentPvPAreaText.text = Languages.AreaString(userInfo.currentPvPArea);

            m_NameText.GetComponent<RectTransform>().sizeDelta = new Vector2(m_LevelText.preferredWidth, m_LevelText.preferredHeight);

            if (m_LevelText != null && m_LevelMaxEffect == null)
                m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit);

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.Value = userInfo.accountLevel;

            float parentSizeX = m_LevelText.preferredWidth + m_NameText.preferredWidth + m_NameText.rectTransform.anchoredPosition.x;
            m_rtrsTitleParent.sizeDelta = new Vector2(parentSizeX, m_rtrsTitleParent.sizeDelta.y);
            m_rtrsTitleParent.anchoredPosition = new Vector2(-(float)(parentSizeX * 0.5), m_rtrsTitleParent.anchoredPosition.y);

            m_LeaderImage.gameObject.SetActive(false);
            if (userInfo.cardInfoList != null && userInfo.cardInfoList.Count > 0)
            {
                for (int i = 0; i < m_MiniCharCardList.Count; i++)
                {
                    UIMiniCharCard miniCharCard = m_MiniCharCardList[i];
                    if (miniCharCard != null)
                    {
                        CCardInfo cardInfo = (i < userInfo.cardInfoList.Count) ? userInfo.cardInfoList[i] : null;
                        if (cardInfo != null)
                        {
                            miniCharCard.SetCardInfo(cardInfo);
                            miniCharCard.gameObject.SetActive(true);

                            // 리더 표시
                            if (userInfo.leaderCID.Equals(cardInfo.m_Cid))
                            {
                                UIUtility.SetParent(m_LeaderImage.transform, miniCharCard.transform);
                                m_LeaderImage.gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            miniCharCard.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    public void SetGuildMember(CGuildBase guildBase,
                       CGuildMember guildMember,
                       CFranchiseRankingInfo rankingInfo,
                       List<CCardInfo> cardInfoList)
    {
        if (guildBase != null)
        {
            m_GuildFlag.SetGuildEmblem(guildBase.m_sGuildEmblem);
            m_GuildNameText.text = guildBase.m_sGuildName;
        }

        if (guildMember != null)
        {
            m_LevelText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), 0);
            m_NameText.text = guildMember.m_sUserName;
            m_RankingPointText.text = Languages.ToString<int>(guildMember.m_iRankingPoint);
            m_SupportCardCountText.text = Languages.ToString<int>(guildMember.m_iSupportCardCount);

            List<DB_AreaPvP.Schema> table = DB_AreaPvP.instance.schemaList;
            int lastIndex = 0;
            if (table != null && table.Count > 0)
            {
                for (int i = 0; i < table.Count; i++)
                {
                    DB_AreaPvP.Schema areaPvP = table[i];
                    if (areaPvP.Need_Rank_Point > guildMember.m_iRankingPoint)
                    {
                        break;
                    }

                    lastIndex = areaPvP.Index;
                }
            }

            m_CurrentPvPAreaText.text = string.Format("{0} {1}", Languages.ToString(TEXT_UI.CARD_DECK_AREA, lastIndex));
        }

        if (rankingInfo != null)
        {
            m_TotalPlayCountText.text = Languages.ToString<int>(rankingInfo.m_iTotalPlayCount);
            m_WinCountText.text = Languages.ToString<int>(rankingInfo.m_iWinCount);
        }

        m_LeaderImage.gameObject.SetActive(false);
        if (cardInfoList != null && cardInfoList.Count > 0)
        {
            for (int i = 0; i < m_MiniCharCardList.Count; i++)
            {
                UIMiniCharCard miniCharCard = m_MiniCharCardList[i];
                if (miniCharCard != null)
                {
                    CCardInfo cardInfo = (i < cardInfoList.Count) ? cardInfoList[i] : null;
                    if (cardInfo != null)
                    {
                        miniCharCard.SetCardInfo(cardInfo);
                        miniCharCard.gameObject.SetActive(true);

                        // 리더 표시
                        if (guildMember != null && guildMember.m_iLeaderCardIndex.Equals(cardInfo.m_iCardIndex))
                        {
                            UIUtility.SetParent(m_LeaderImage.transform, miniCharCard.transform);
                            m_LeaderImage.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        miniCharCard.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
