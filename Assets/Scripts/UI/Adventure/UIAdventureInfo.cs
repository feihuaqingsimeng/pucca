using Common.Packet;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class PVE_MobData
{
    public int Card_Index;
    public int BattlePower;
}


public class UIAdventureInfo : UIObject
{
    private UIAdventure AdventureManager;

    private int         SelectStageIndex;


    //적 정보 UI.
    //public  GameObject  StageFlag_Elite;
    //public  GameObject  StageFlag_Boss;

    public Sprite       StageNormalSprite;
    public Sprite       StageEliteSprite;
    public Sprite       StageBossSprite;

    public  Image       StageHardFlag;

    public  Text        StageHardText;
    public  Text        StageName;
    public  Text        GetExpCount;

    public  Text        LeftSweepCount;
    public  Text        UseHeartCount;

    //카드정보.
    public  GameObject  Prefab_MobSpineInfo;
    public  Transform   MobSpineInfoParent;
    [HideInInspector]
    public  List<UIMobSpineInfo>    MobSpineInfoList = new List<UIMobSpineInfo>();

    //보상정보.
    public  Transform           RewardObjectParents;
    public  GameObject          PrefabGoodsCard;
    private List<GameObject>    RewardCardList = new List<GameObject>();


    //버튼.
    public Button   Button_Sweep;
    public Button   Button_Card;
    public Button   Button_Battle;


    private Sprite ButtonSpr_Enable;
    private Sprite ButtonSpr_Disable;



    //계산용.
    List<PVE_MobData> SortMobList = new List<PVE_MobData>();



    private int CurSweepTicketCount;





    protected override void Awake()
    {
        base.Awake();


        Button_Sweep.onClick.AddListener(PressButton_Sweep);
        Button_Card.onClick.AddListener(PressButton_Card);
        Button_Battle.onClick.AddListener(PressButton_Battle);

        ButtonSpr_Enable = Button_Sweep.GetComponent<Image>().sprite;
        ButtonSpr_Disable = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");


        for(int idx = 0; idx < 6; idx++)
        {
            GameObject pSpineObj = Instantiate(Prefab_MobSpineInfo) as GameObject;
            pSpineObj.transform.parent = MobSpineInfoParent;
            pSpineObj.transform.localPosition = Vector3.zero;
            pSpineObj.transform.localScale = Vector3.one;
            pSpineObj.SetActive(false);

            MobSpineInfoList.Add(pSpineObj.GetComponent<UIMobSpineInfo>());
        }

        CurSweepTicketCount = Kernel.entry.account.clearTicket;

    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (UIHUD.instance)
        {
            UIHUD.instance.onBackButtonClicked = OnBackButtonClicked;
        }
    }


    protected override void OnDisable()
    {
        base.OnDisable();

        for(int idx = 0; idx < RewardCardList.Count; idx++)
        {
            Destroy(RewardCardList[idx].gameObject);
        }
        RewardCardList.Clear();

        UIHUD.instance.onBackButtonClicked = null;
    }



    protected override void Start()
    {
        base.Start();
    }


    bool OnBackButtonClicked()
    {
        OnCloseButtonClick();

        return true;
    }




    protected override void Update()
    {
        base.Update();

        //남은 소탕권 수.
        LeftSweepCount.text = Languages.GetNumberComma(Kernel.entry.account.GetValue(Goods_Type.SweepTicket));
        if (CurSweepTicketCount != Kernel.entry.account.GetValue(Goods_Type.SweepTicket))
            UpdateSweepButton();
    }



    //정보세팅.
    public void InitAdventureInfo(UIAdventure UIAdventureMng, int stageIndex)
    {
        AdventureManager = UIAdventureMng;
        SelectStageIndex = stageIndex;

        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, SelectStageIndex);

        //스테이지 정보.
        string difficultyString = Languages.ToString(TEXT_UI.DIFFICULTY);
        string colorString = "FFFFFFFF";
        switch (StageData.STAGE_TYPE)
        {
            case STAGE_TYPE.NORMAL  :
                difficultyString = difficultyString + Languages.ToString(TEXT_UI.EASY);
                colorString = "FFFFFFFF";
                StageHardFlag.sprite = StageNormalSprite; break;
            case STAGE_TYPE.SPECIAL :
                difficultyString = difficultyString + Languages.ToString(TEXT_UI.NORMAL);
                colorString = "FCC415FF";
                StageHardFlag.sprite = StageEliteSprite;  break;
            case STAGE_TYPE.BOSS    :
                difficultyString = difficultyString + Languages.ToString(TEXT_UI.HARD);
                colorString = "FF1E05FF";
                StageHardFlag.sprite = StageBossSprite;   break;
        }
        difficultyString = string.Format("<color=#{0}>{1}</color>", colorString, difficultyString);
        StageHardText.text = difficultyString;

        //획득가능 경험치.
        GetExpCount.text = "+" + Languages.GetNumberComma(StageData.AccountEXP);
        
        //사용 하트 수.
        UseHeartCount.text = "- " + Languages.GetNumberComma(StageData.Need_Heart);
        
        //남은 소탕권 수.
        LeftSweepCount.text = Languages.GetNumberComma(Kernel.entry.account.clearTicket);


        //스테이지 이름.
        DBStr_Stage.Schema StageString = DBStr_Stage.Query(DBStr_Stage.Field.Stage_Id, SelectStageIndex);
        StageName.text = StageString.StageName;


        //몬스터 리스트.
        CheckMonsterList(StageData.MobGroup_Id);

        //화면 표시.
        ShowCheckResultMobInfo();


        //보상.
        RewardCardList.Clear();
        for (int idx = 0; idx < DB_StageReward.instance.schemaList.Count; idx++)
        {
            if (DB_StageReward.instance.schemaList[idx].Reward_Group == StageData.Reward_Group_Link)
            {
                GameObject RewardCard = Instantiate(PrefabGoodsCard) as GameObject;
                RewardCard.GetComponent<UIAdventureRewardCard>().InitRewardCard(DB_StageReward.instance.schemaList[idx].Goods_Type);
                RewardCard.transform.parent = RewardObjectParents;
                RewardCard.transform.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1.0f);

                RewardCardList.Add(RewardCard);
            }
        }

        RectTransform pParentsTransform = RewardObjectParents.GetComponent<RectTransform>();
        pParentsTransform.localPosition = new Vector3(pParentsTransform.sizeDelta.x / 2, pParentsTransform.localPosition.y, pParentsTransform.localPosition.z);


        UpdateSweepButton();

        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 301)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }
    }



    public void UpdateSweepButton()
    {
        Text SweepBtnText = Button_Sweep.transform.FindChild("txtButtonName").GetComponent<Text>();
        Color Color_Outline, Color_Shadow;

        if (SelectStageIndex <= Kernel.entry.account.lastStageIndex && Kernel.entry.account.clearTicket > 0)
        {
            Button_Sweep.GetComponent<Image>().sprite = ButtonSpr_Enable;
            Kernel.colorManager.TryGetColor("ui_button_02_outline", out Color_Outline);
            Kernel.colorManager.TryGetColor("ui_button_02_shadow", out Color_Shadow);

            SetColor(SweepBtnText, Color_Outline, Color_Shadow);
            Button_Sweep.transform.FindChild("txtSweepCount").gameObject.SetActive(true);
        }
        else
        {
            Button_Sweep.GetComponent<Image>().sprite = ButtonSpr_Disable;
            Kernel.colorManager.TryGetColor("ui_button_04_outline", out Color_Outline);
            Kernel.colorManager.TryGetColor("ui_button_04_shadow", out Color_Shadow);
            SetColor(SweepBtnText, Color_Outline, Color_Shadow);

            if (SelectStageIndex <= Kernel.entry.account.lastStageIndex) //가능한데 부족한거면...
                Button_Sweep.transform.FindChild("txtSweepCount").gameObject.SetActive(true);
            else
                Button_Sweep.transform.FindChild("txtSweepCount").gameObject.SetActive(false);
        }
    }




    public void PressButton_Sweep()
    {
        SoundDataInfo.CancelSound(Button_Sweep.gameObject);

        //소탕버튼은 깼을때만 활성화.
        if (SelectStageIndex <= Kernel.entry.account.lastStageIndex)
        {
            if (Kernel.entry.account.clearTicket <= 0)
                UINotificationCenter.Enqueue(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_CLEAR_TICKET));
            else
            {
                SoundDataInfo.RevertSound(Button_Sweep.gameObject);
                Kernel.entry.adventure.SelectStageIndex = SelectStageIndex;

                UIAdventureSweep uiSweep = Kernel.uiManager.Get<UIAdventureSweep>(UI.AdventureSweep, true, false);

                if(uiSweep != null)
                    Kernel.uiManager.Open(UI.AdventureSweep);
            }
        }
        else
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.SWEEP_CANT_USE));
    }



    public void PressButton_Card()
    {
        UIHUD.instance.onBackButtonClicked = null;
        Kernel.entry.adventure.PreSelectStageIndex = AdventureManager.SelectStageIdx;
        Kernel.sceneManager.LoadScene(Scene.Deck);
    }



    public void PressButton_Battle()
    {
        Kernel.entry.adventure.SelectStageIndex = SelectStageIndex;

        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, SelectStageIndex);

        SoundDataInfo.CancelSound(Button_Battle.gameObject);

        if (Kernel.entry.account.heart < StageData.Need_Heart)
        {
            UINotificationCenter.Enqueue(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_TOKEN));
        }
        else
        {
            SoundDataInfo.RevertSound(Button_Battle.gameObject);
            Kernel.entry.battle.CurBattleKind = BATTLE_KIND.PVE_BATTLE;
            Kernel.sceneManager.LoadScene(Scene.Battle, true);
        }
    }





    void SetColor(Text text, Color outlineColor, Color shadowColor)
    {
        if (text != null)
        {
            foreach (var item in text.GetComponentsInChildren<Shadow>(true))
            {
                if (item is Outline)
                {
                    item.effectColor = outlineColor;
                }
                else if (item is Shadow)
                {
                    item.effectColor = shadowColor;
                }
            }
        }
    }














    //몬스터 리스트 체크.
    public void CheckMonsterList(int MobGroupIndex)
    {
        if (SortMobList.Count > 0)
        {
            while (true)
            {
                SortMobList.RemoveAt(0);

                if (SortMobList.Count <= 0)
                {
                    SortMobList.Clear();
                    break;
                }
            }
        }


        int nCurGroupID = MobGroupIndex;

        while (true)
        {
            DB_StageMob.Schema GroupData = DB_StageMob.Query(DB_StageMob.Field.GroupIndex, nCurGroupID);

            //리스트 체크.
            for (int idx = 0; idx < 5; idx++)
            {
                int nMobID = 0;
                switch (idx)
                {
                    case 0:     nMobID = GroupData.MobIndex_1;      break;
                    case 1:     nMobID = GroupData.MobIndex_2;      break;
                    case 2:     nMobID = GroupData.MobIndex_3;      break;
                    case 3:     nMobID = GroupData.MobIndex_4;      break;
                    case 4:     nMobID = GroupData.MobIndex_5;      break;
                }

                if(nMobID == 0)
                    continue;

                DB_PVEMobData.Schema TempMobData = DB_PVEMobData.Query(DB_PVEMobData.Field.MobIndex, nMobID);
                if (TempMobData == null)
                    continue;

                AddSortMobList(TempMobData.Card_Index, (byte)TempMobData.Level_Base, (byte)TempMobData.Level_Skill, (byte)TempMobData.Level_Weapon, (byte)TempMobData.Level_Armor, (byte)TempMobData.Level_Acc);
            }

            nCurGroupID = GroupData.NextGroup;
            if (nCurGroupID == 0)
                break;
        }
    }




    public void AddSortMobList(int CardIndex, byte Lv_Base, byte Lv_Skill, byte Lv_Weapon, byte Lv_Armor, byte Lv_Acc)
    {
        //전투력 계산.
        int TempBattlePower = 0;
        Settings.Card.TryGetBattlePower(CardIndex, Lv_Base, Lv_Acc, Lv_Weapon, Lv_Armor, Lv_Skill, out TempBattlePower);


        bool IsEnable = false;
        for (int idx = 0; idx < SortMobList.Count; idx++)
        {
            if (SortMobList[idx].Card_Index == CardIndex)
            {
                IsEnable = true;
                break;
            }
        }

        if (IsEnable)
        {
            for (int idx = 0; idx < SortMobList.Count; idx++)
            {
                if (SortMobList[idx].Card_Index == CardIndex)
                {
                    if (SortMobList[idx].BattlePower < TempBattlePower)
                        SortMobList[idx].BattlePower = TempBattlePower;

                    break;
                }
            }
        }
        else
        {
            PVE_MobData pTempMobData = new PVE_MobData();
            pTempMobData.Card_Index = CardIndex;
            pTempMobData.BattlePower = TempBattlePower;
            SortMobList.Add(pTempMobData);
        }

    }




    //추가.
    public void ShowCheckResultMobInfo()
    {
        int nAddCount = 0;
        for (int idx = 0; idx < MobSpineInfoList.Count; idx++)
        {
            if (idx >= SortMobList.Count)
            {
                MobSpineInfoList[idx].gameObject.SetActive(false);
            }
            else
            {
                MobSpineInfoList[idx].gameObject.SetActive(true);
                MobSpineInfoList[idx].InitUIMobSpineInfo(SortMobList[nAddCount].Card_Index, SortMobList[nAddCount].BattlePower);
                nAddCount++;
            }
        }
    }


}






