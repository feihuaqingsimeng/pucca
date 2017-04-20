using Common.Packet;
using System.Collections;
using System.Collections.Generic;

public class RankingScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    public override IEnumerator Preprocess()
    {
        completed = false;
        Kernel.entry.ranking.onRankerInfoListUpdate += OnRankerInfoListUpdate;
        Kernel.entry.ranking.REQ_PACKET_CG_READ_RANK_REWARD_INFO_SYN();
        Kernel.entry.ranking.REQ_PACKET_CG_RANK_REQUEST_GUILD_RANKING_SYN(1, true);
        Kernel.entry.ranking.REQ_PACKET_CG_RANK_REQUEST_PVP_RANKING_SYN(1, true);

        return base.Preprocess();
    }

    void OnRankerInfoListUpdate(byte page, List<CRankerInfo> rankerInfoList, int startIndex, int length)
    {
        Kernel.entry.ranking.onRankerInfoListUpdate -= OnRankerInfoListUpdate;
        Kernel.uiManager.Open(UI.Ranking);
        completed = true;
    }
}
