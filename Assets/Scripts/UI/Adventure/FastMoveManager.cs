using UnityEngine;
using System.Collections;

public class FastMoveManager : MonoBehaviour
{
    public  UIAdventure             UIAdventureMng;
    public  FastMoveLinkButton[]    FastMoveLinkList;


    public void InitFastMoveManager()
    {
        for(int idx = 0; idx < FastMoveLinkList.Length; idx++)
        {
            FastMoveLinkList[idx].InitLinkButton(this, idx);
        }

        UpdateFastMoveLink();
    }


    public void UpdateFastMoveLink()
    {
        bool bSelected = false;
        bool bActive = false;

        int LastStageIndex = Kernel.entry.account.lastStageIndex;
        if (LastStageIndex == 0)
            LastStageIndex = 101;

        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, LastStageIndex);
        int NextStageIndex = StageData.NextStageID;

        for(int idx = 0; idx < FastMoveLinkList.Length; idx++)
        {
            bSelected = false;
            if (Kernel.entry.adventure.SelectAreaIndex == idx)
                bSelected = true;

            bActive = false;
            if (idx < NextStageIndex / 100 || NextStageIndex == 0)
                bActive = true;

            FastMoveLinkList[idx].UpdateLinkButton(bSelected, bActive);
        }

        UIAdventureMng.ChangeAdventureMap_Manual();

    }


}
