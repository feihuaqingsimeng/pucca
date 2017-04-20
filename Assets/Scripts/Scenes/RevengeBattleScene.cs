using Common.Packet;
using System.Collections;
using System.Collections.Generic;

public class RevengeBattleScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    public override IEnumerator Preprocess()
    {
        completed = false;

        Kernel.entry.revengeBattle.onUpdatedRevengeMatchInfoList += OnUpdatedRevengeMatchInfoList;

        Kernel.entry.revengeBattle.REQ_PACKET_CG_READ_REVENGE_MATCH_LIST_SYN();

        return base.Preprocess();
    }

    void OnUpdatedRevengeMatchInfoList(List<CRevengeMatchInfo> revengeMatchInfoList)
    {
        Kernel.entry.revengeBattle.onUpdatedRevengeMatchInfoList -= OnUpdatedRevengeMatchInfoList;

        Kernel.uiManager.Open(UI.RevengeBattle);

        completed = true;
    }
}
