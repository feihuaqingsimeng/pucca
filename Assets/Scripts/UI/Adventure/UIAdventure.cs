using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIAdventure : UIObject
{
    public  GameObject[]    AdventureMapObjectList;

    public  Button          AdventureMapArrow_L;
    public  Button          AdventureMapArrow_R;

    public  int             LastStageIdx;   
    public  int             SelectStageIdx;   
    public  int             CurAdventureMapNumber;
    public  int             CurAdventureStageNumber;

    public  FastMoveManager FastMoveMng;


    protected override void Awake()
    {
        base.Awake();

        LastStageIdx = Kernel.entry.account.lastStageIndex;

        if (LastStageIdx == 0)
        {
            CurAdventureMapNumber = 0;
            CurAdventureStageNumber = 0;
        }
        else
        {
            DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, LastStageIdx);
            int NextStageIdx = StageData.NextStageID;
            if (NextStageIdx == 0)
                NextStageIdx = LastStageIdx;

            CurAdventureMapNumber = (NextStageIdx / 100) - 1;
            CurAdventureStageNumber = (NextStageIdx % 100) - 1;
        }


        AdventureMapArrow_L.onClick.AddListener(ChangeAdventureMap_Left);
        AdventureMapArrow_R.onClick.AddListener(ChangeAdventureMap_Right);


        AdventureMapArrow_L.gameObject.SetActive(true);
        AdventureMapArrow_R.gameObject.SetActive(true);
        if (CurAdventureMapNumber == 0)
            AdventureMapArrow_L.gameObject.SetActive(false);
        if (CurAdventureMapNumber == AdventureMapObjectList.Length-1)
            AdventureMapArrow_R.gameObject.SetActive(false);

        InitUIAdventure();
    }



    public void InitUIAdventure()
    {
        if (Kernel.entry.adventure.PreSelectStageIndex != 0)
            SelectStageIdx = Kernel.entry.adventure.PreSelectStageIndex;

        if (SelectStageIdx != 0)
        {
            CurAdventureMapNumber = (SelectStageIdx / 100) - 1;
            CurAdventureStageNumber = (SelectStageIdx % 100) - 1;
            SetAdventureMap();
            ShowStageInfo(SelectStageIdx);

            Kernel.entry.adventure.PreSelectStageIndex = 0;

            AdventureMapArrow_L.gameObject.SetActive(true);
            AdventureMapArrow_R.gameObject.SetActive(true);
            if (CurAdventureMapNumber == 0)
                AdventureMapArrow_L.gameObject.SetActive(false);
            if (CurAdventureMapNumber == AdventureMapObjectList.Length - 1)
                AdventureMapArrow_R.gameObject.SetActive(false);
        }


        //정보대로 아이콘들 초기화.
        for (int idx = 0; idx < AdventureMapObjectList.Length; idx++)
        {
            if (CurAdventureMapNumber == idx)
                AdventureMapObjectList[idx].GetComponent<RectTransform>().localPosition = Vector3.zero;
            else
                AdventureMapObjectList[idx].GetComponent<RectTransform>().localPosition = new Vector3(2000.0f, 0.0f, 0.0f);

            int MaxStage = 0;
            switch (idx)
            {
                case 0:
                    MaxStage = 5;
                    break;

                default:
                    MaxStage = 15;
                    break;
            }
            AdventureMapObjectList[idx].GetComponent<AdventureMapManager>().InitAdventureMapManager(this, idx, MaxStage);
        }


        //빠른이동 초기화.
        Kernel.entry.adventure.SelectAreaIndex = CurAdventureMapNumber;
        FastMoveMng.InitFastMoveManager();

        SetAdventureMap();


        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive)
        {
            switch (Kernel.entry.tutorial.WaitSeq)
            {
                case 300:
                    Kernel.entry.tutorial.onSetNextTutorial();
                    break;
            }
        }
   }




    public void SetAdventureMap()
    {
        for (int idx = 0; idx < AdventureMapObjectList.Length; idx++)
        {
            if (CurAdventureMapNumber == idx)
                AdventureMapObjectList[idx].GetComponent<RectTransform>().localPosition = Vector3.zero;
            else
                AdventureMapObjectList[idx].GetComponent<RectTransform>().localPosition = new Vector3(2000.0f, 0.0f, 0.0f);
        }
    }



    public void ChangeAdventureMap_Left()
    {
        if (CurAdventureMapNumber <= 0)
            return;

        CurAdventureMapNumber--;

        //빠른이동 갱신.
        Kernel.entry.adventure.SelectAreaIndex = CurAdventureMapNumber;
        FastMoveMng.UpdateFastMoveLink();

        AdventureMapArrow_R.gameObject.SetActive(true);
        if (CurAdventureMapNumber == 0)
            AdventureMapArrow_L.gameObject.SetActive(false);
        else
            AdventureMapArrow_L.gameObject.SetActive(true);

        SetAdventureMap();
    }


    public void ChangeAdventureMap_Right()
    {
        if (CurAdventureMapNumber >= AdventureMapObjectList.Length-1)
            return;

        CurAdventureMapNumber++;

        //빠른이동 갱신.
        Kernel.entry.adventure.SelectAreaIndex = CurAdventureMapNumber;
        FastMoveMng.UpdateFastMoveLink();

        AdventureMapArrow_L.gameObject.SetActive(true);
        if (CurAdventureMapNumber >= AdventureMapObjectList.Length - 1)
            AdventureMapArrow_R.gameObject.SetActive(false);
        else
            AdventureMapArrow_R.gameObject.SetActive(true);

        SetAdventureMap();
    }



    public void ChangeAdventureMap_Manual()
    {
        CurAdventureMapNumber = Kernel.entry.adventure.SelectAreaIndex;

        if (CurAdventureMapNumber == 0)
            AdventureMapArrow_L.gameObject.SetActive(false);
        else
            AdventureMapArrow_L.gameObject.SetActive(true);

        if (CurAdventureMapNumber >= AdventureMapObjectList.Length - 1)
            AdventureMapArrow_R.gameObject.SetActive(false);
        else
            AdventureMapArrow_R.gameObject.SetActive(true);

        SetAdventureMap();
    }





    public void ShowStageInfo(int StageID)
    {
        SelectStageIdx = StageID;
        UIObject AdventureInfo = Kernel.uiManager.Open(UI.AdventureInfo);
        AdventureInfo.GetComponent<UIAdventureInfo>().InitAdventureInfo(this, SelectStageIdx);


    }











}
