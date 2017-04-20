using Common.Packet;
using System.Collections;
using System.Collections.Generic;

public class GuildScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    public override IEnumerator Preprocess()
    {
        completed = false;

        Kernel.entry.guild.onGuildJoinStateUpdate += OnGuildJoinStateUpdate;

        Kernel.entry.guild.REQ_PACKET_CG_GUILD_REFRESH_GUILD_JOINED_SYN();

        return base.Preprocess();
    }

    void OnGuildJoinStateUpdate(long gid, string guildName)
    {
        Kernel.entry.guild.onGuildJoinStateUpdate -= OnGuildJoinStateUpdate;

        if (gid > 0)
        {
            Kernel.entry.guild.onGuildReceiveCardResult += OnGuildReceiveCardResult;
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_ENTER_GUILD_MAIN_SYN();
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_MEMBER_LIST_SYN();
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_GET_CARD_REQUEST_LIST_SYN();
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_REFRESH_CHATTING_LIST_SYN();
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_GET_SHOP_BUY_COUNT_LIST_SYN();
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_RECEIVED_CARD_SYN();
        }
        else
        {
            Kernel.entry.guild.onGuildListUpdate += OnGuildListUpdate;
            Kernel.entry.guild.REQ_PACKET_CG_GUILD_RECOMMEND_LIST_SYN();
        }
    }

    void OnGuildReceiveCardResult(int cardIndex, List<CReceicedCard> receivedCardList)
    {
        Kernel.entry.guild.onGuildReceiveCardResult -= OnGuildReceiveCardResult;

        Kernel.uiManager.Open(UI.Guild);

        if (cardIndex != 0 && receivedCardList != null && receivedCardList.Count > 0)
        {
            UIGuildReceiveCardInfo receiveCardInfo = Kernel.uiManager.Open<UIGuildReceiveCardInfo>(UI.GuildReceiveCardInfo);
            if (receiveCardInfo)
            {
                receiveCardInfo.SetGuildReceiveCardResult(cardIndex, receivedCardList);
            }
        }

        completed = true;
    }

    void OnGuildListUpdate(List<CGuildBase> recommendGuildList, List<CGuildBase> waitingApprovalList)
    {
        Kernel.entry.guild.onGuildListUpdate -= OnGuildListUpdate;
        Kernel.uiManager.Open(UI.GuildEnter);
        completed = true;
    }
}
