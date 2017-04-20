using UnityEngine;
using System.Collections;
using System;


public class AdventureMapManager : MonoBehaviour
{
    public StageButtonInfo[]    StageButtonArray;




    public void InitAdventureMapManager(UIAdventure AdventureMng, int MapNumber, int MaxStage)
    {
        int nLastMapNumber = (AdventureMng.LastStageIdx / 100) - 1;

        //다음 지역의 데이터 받아오기.
        DB_StagePVE.Schema pStageData = null;
        if(AdventureMng.LastStageIdx != 0)
            pStageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, AdventureMng.LastStageIdx);


        StageButtonArray = new StageButtonInfo[MaxStage];
        for (int idx = 0; idx < MaxStage; idx++)
        {
            string ChildName = string.Format("{0}{1:00}", MapNumber + 1, idx + 1);
            int MapIdx = Convert.ToInt32(ChildName);

            StageButtonArray[idx] = transform.FindChild(ChildName).GetComponent<StageButtonInfo>();


            STAGEBUTTON_OPEN_STATE eState = STAGEBUTTON_OPEN_STATE.OPEN;


            if (AdventureMng.LastStageIdx == 0 && MapIdx == 101)    //깬게 없고 첫 스테이지 버튼이면...
                eState = STAGEBUTTON_OPEN_STATE.SELECT;
            else
            {
                if (AdventureMng.LastStageIdx == 615)    //마지막 스테이지까지 깼으면.
                    eState = STAGEBUTTON_OPEN_STATE.OPEN;
                else
                {
                    if (pStageData != null && MapIdx == pStageData.NextStageID)
                        eState = STAGEBUTTON_OPEN_STATE.SELECT;
                    else
                    {
                        if (MapIdx <= AdventureMng.LastStageIdx) //마지막으로 깬 스테이지보다 작거나 같으면 오픈상태로.
                            eState = STAGEBUTTON_OPEN_STATE.OPEN;
                        else
                            eState = STAGEBUTTON_OPEN_STATE.CLOSE;
                    }
                }
            }


            StageButtonArray[idx].InitStageButton(AdventureMng, MapIdx, eState);
        }
    }

}
