using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Common.Packet;


public enum ADVENTURE_RESULT_TYPE
{
    BATTLE_WIN = 0,
    BATTLE_LOSE,
    SWEEP_END
}

public class UIAdventureResult : UIObject
{
    [HideInInspector]
    public  ADVENTURE_RESULT_TYPE       AdventureResultType;

    public  GameObject  Background;

    public  GameObject  ResultObject_Win;
    public  GameObject  ResultObject_Lose;


    //경험치.
    private TextLevelMaxEffect m_LevelMaxEffect;
    public  RectTransform   NameLevelParent;
    public  Text            txtUserName;
    public  Text            txtUserLevel;
    public  Text            txtGetExpCount;
    public  Image           imgExpGauge;
    public  float           imgExpGaugeWidth = 500.0f;
    public  Text            txtExpGaugeCount;


    //승리or소탕에 사용되는 UI.
    public  GameObject      EXP_Obj;

    public  Animation       RewardPanelAnimation;
    public  Transform       RewardGridParent;
    public  GameObject      PrefabRewardCard;
    private List<GameObject>    RewardCardList = new List<GameObject>();


    //버튼.
    public  float       ResultButtonGap;
    public  Button      ResultButton_OK;
    public  Button      ResultButton_Retry;
    public  Button      ResultButton_Next;


    public  Button      ResultSweepButton_OK;
    public  Button      ResultSweepButton_Retry;

    private Sprite      ButtonSpr_Enable;
    private Sprite      ButtonSpr_Disable;


    public  Text        txtLeftSweepTicket;


    private float       CurExpCount;
    private int         MaxExpCount;

    //연출용.
    private float       OneAddExp;
    private float       LeftAddExp;
    private float       CurAddExpTime;
    private float       TimeDelayAddExp = 0.01f;
    private bool        UpdateAddExpMode;

    

    //레벨업 데이터.
    private bool        LevelUpActive;
    private int         LvUp_Heart;
    private int         LvUp_ruby;
    private int         LvUp_Gold;
    private int         CurAccountExp;
    private int         CurAccountGold;


    //스킵.
    private bool        SkipMode;


    //스파인.
    public  SkeletonAnimation   PuccaSpineAnimation;
    private bool                CheckEndMotion;







    protected override void Awake()
    {
        base.Awake();


        ButtonSpr_Enable = ResultSweepButton_Retry.GetComponent<Image>().sprite;
        ButtonSpr_Disable = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_button_disable");

    }


    protected override void OnEnable()
    {
        base.OnEnable();

        Background.SetActive(false);
        ResultObject_Win.SetActive(false);
        ResultObject_Lose.SetActive(false);

        ResultButton_OK.onClick.AddListener(PressOKButton);
        ResultButton_OK.gameObject.SetActive(false);
        ResultButton_Retry.onClick.AddListener(PressRetryButton);
        ResultButton_Retry.gameObject.SetActive(false);
        ResultButton_Next.onClick.AddListener(PressNextButton);
        ResultButton_Next.gameObject.SetActive(false);


        ResultSweepButton_OK.onClick.AddListener(PressOKButton);
        ResultSweepButton_OK.gameObject.SetActive(false);
        ResultSweepButton_Retry.onClick.AddListener(PressRetrySweepButton);
        ResultSweepButton_Retry.gameObject.SetActive(false);

        PuccaSpineAnimation.gameObject.SetActive(false);
    }


    protected override void OnDisable()
    {
 	    base.OnDisable();

        for (int idx = 0; idx < RewardCardList.Count; idx++)
        {
            Destroy(RewardCardList[idx].gameObject);
        }
        RewardCardList.Clear();



        ResultButton_OK.onClick.RemoveAllListeners();
        ResultButton_Retry.onClick.RemoveAllListeners();
        ResultButton_Next.onClick.RemoveAllListeners();


        ResultSweepButton_OK.onClick.RemoveAllListeners();
        ResultSweepButton_Retry.onClick.RemoveAllListeners();

    }

    // Max Level 표기
    private void SetLevelMax(int maxLevel, int level)
    {
        if (txtUserLevel != null)
            m_LevelMaxEffect = txtUserLevel.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = maxLevel;

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.Value = level;
    }

    // Name And Level Text Center
    private void SetTextCenter()
    {
        txtUserLevel.rectTransform.sizeDelta = new Vector2(txtUserLevel.preferredWidth, txtUserLevel.preferredHeight);
        txtUserName.rectTransform.sizeDelta = new Vector2(txtUserName.preferredWidth, txtUserName.preferredHeight);

        float parentSizeX = txtUserLevel.rectTransform.sizeDelta.x + txtUserName.rectTransform.anchoredPosition.x + txtUserName.rectTransform.sizeDelta.x;
        NameLevelParent.sizeDelta = new Vector2(parentSizeX, NameLevelParent.sizeDelta.y);
        NameLevelParent.anchoredPosition = new Vector2(-(float)(parentSizeX * 0.5f), NameLevelParent.anchoredPosition.y);
    }

    //승리 결과창(전투).
    public void ShowResult_Win(PACKET_CG_GAME_PVE_RESULT_ACK packet)
    {
        var totalExp = packet.m_TotalExp;

        SkipMode = false;

        ResultObject_Win.SetActive(true);
        ResultObject_Lose.SetActive(false);
        ResultButton_OK.gameObject.SetActive(false);


        AdventureResultType = ADVENTURE_RESULT_TYPE.BATTLE_WIN;
        Background.SetActive(false);

        //경험치 증가 연출.
        DB_AccountLevel.Schema accountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, Kernel.entry.battle.BattleStartLevel);
        CurExpCount = Kernel.entry.account.exp;
        MaxExpCount = accountLevel.Need_AccountExp;

        LeftAddExp = packet.m_iEarnExp;
        OneAddExp = LeftAddExp / 20;
        if(OneAddExp <= 0.0f)
            OneAddExp = 1;
        CurAddExpTime = 0.0f;

        UpdateAddExpMode = false;


        //레벨업 했으면 레벨업처리.
        LevelUpActive = false;
        if(packet.m_bIsLevelUp)
        {
            LevelUpActive = true;
            LvUp_Heart = packet.m_LevelReward.m_iTotalHeart;
            LvUp_ruby = packet.m_LevelReward.m_iTotalRuby;
            LvUp_Gold = packet.m_LevelReward.m_iTotalTrainingPoint;
        }
        CurAccountExp = packet.m_iExp;
        CurAccountGold = packet.m_iTotalGold;

        //클리어한 스테이지 갱신.
        Kernel.entry.account.lastStageIndex = packet.m_iLastStageIndex;

        //화폐 갱신.
        Kernel.entry.account.promoteTicket = packet.m_PromoteTicket;

        //소탕권 갱신.
        Kernel.entry.account.clearTicket = packet.m_iClearTicketCount;

        //지도 갱신.
        Kernel.entry.account.treasureMap = packet.m_TreasureMap;


        //리스트 추가.
        EXP_Obj.SetActive(false);
        txtGetExpCount.text = Languages.GetNumberComma(packet.m_iEarnExp);

        //유저네임 및 레벨
        int level = Kernel.entry.battle.BattleStartLevel;
        txtUserLevel.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level);
        txtUserName.text = string.Format("{0}", Kernel.entry.account.name);
        SetLevelMax(Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), level);
        SetTextCenter();

        //보상추가.
        RewardCardList.Clear();
        for (int idx = 0; idx < packet.m_PvEResultList.Count; idx++)
        {
            CPvEResult pResultData = packet.m_PvEResultList[idx];
            if (pResultData == null)
                break;

            if (pResultData.m_eGoodsType == Common.Util.eGoodsType.SweepTicket)
            {
                Debug.Log("sweep! : " + pResultData.m_iRewardCount.ToString());
            }

            GameObject RewardCard = Instantiate(PrefabRewardCard) as GameObject;
            RewardCard.GetComponent<UIAdventureRewardCard>().InitRewardCard(pResultData.m_eGoodsType, pResultData.m_iRewardCount);
            RewardCard.transform.parent = RewardGridParent;
            RewardCard.GetComponent<RectTransform>().localScale = Vector3.one;
            RewardCardList.Add(RewardCard);
        }

        //애니메이션 포지션.
        RewardPanelAnimation.gameObject.SetActive(false);
 

        Invoke("ShowResultObject_Exp", 0.5f);
    }


    //패배 결과창(전투).
    public void ShowResult_Lose()
    {
        SkipMode = false;

        ResultObject_Lose.SetActive(true);
        ResultObject_Win.SetActive(false);
        ResultButton_OK.gameObject.SetActive(false);

        AdventureResultType = ADVENTURE_RESULT_TYPE.BATTLE_LOSE;
        Background.SetActive(false);

        //유저네임.
        int level = Kernel.entry.battle.BattleStartLevel;
        txtUserLevel.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level);
        txtUserName.text = string.Format("{0}", Kernel.entry.account.name);
        SetLevelMax(Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), level);
        SetTextCenter();

        //경험치.
        txtGetExpCount.text = "0";

        DB_AccountLevel.Schema accountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, Kernel.entry.battle.BattleStartLevel);
        CurExpCount = Kernel.entry.account.exp;
        MaxExpCount = accountLevel.Need_AccountExp;


        //스파인.
        PuccaSpineAnimation.gameObject.SetActive(true);
        ResetDefeatMotion();

        Invoke("ShowResultObject_ButtonOK", 0.5f);
    }


    //소탕 결과창.
    public void ShowResult_Sweep(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet)
    {
        var totalExp = packet.m_TotalExp;

        SkipMode = false;
        ResultObject_Win.SetActive(true);
        ResultObject_Lose.SetActive(false);
        ResultButton_OK.gameObject.SetActive(false);

        AdventureResultType = ADVENTURE_RESULT_TYPE.SWEEP_END;
        Background.SetActive(true);

        //경험치 증가 연출.
        DB_AccountLevel.Schema accountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, Kernel.entry.battle.BattleStartLevel);
        CurExpCount = Kernel.entry.account.exp;
        MaxExpCount = accountLevel.Need_AccountExp;

        LeftAddExp = packet.m_iEarnExp;
        OneAddExp = LeftAddExp / 20;
        if (OneAddExp <= 0.0f)
            OneAddExp = 0.1f;
        CurAddExpTime = 0.0f;

        UpdateAddExpMode = false;


        //레벨업 했으면 레벨업처리.
        LevelUpActive = false;
        if (packet.m_bIsLevelUp)
        {
            LevelUpActive = true;
            LvUp_Heart = packet.m_LevelUpReward.m_iTotalHeart;
            LvUp_ruby = packet.m_LevelUpReward.m_iTotalRuby;
            LvUp_Gold = packet.m_LevelUpReward.m_iTotalTrainingPoint;
        }
        CurAccountExp = packet.m_iExp;
        CurAccountGold = packet.m_iTotalGold;


        //화폐 갱신.
        Kernel.entry.account.promoteTicket = packet.m_PromoteTicket;
        //남은 소탕권 갱신.
        Kernel.entry.account.clearTicket = packet.m_iRemainClearTicket;
        //지도 갱신.
        Kernel.entry.account.treasureMap = packet.m_TreasureMap;


        txtLeftSweepTicket.text = Kernel.entry.account.clearTicket.ToString();

        Text SweepBtnText = ResultSweepButton_Retry.transform.FindChild("Text").GetComponent<Text>();
        Color Color_Outline, Color_Shadow;

        if (Kernel.entry.account.clearTicket > 0)
        {
            ResultSweepButton_Retry.GetComponent<Image>().sprite = ButtonSpr_Enable;
            Kernel.colorManager.TryGetColor("ui_button_02_outline", out Color_Outline);
            Kernel.colorManager.TryGetColor("ui_button_02_shadow", out Color_Shadow);

            SetColor(SweepBtnText, Color_Outline, Color_Shadow);
        }
        else
        {
            ResultSweepButton_Retry.GetComponent<Image>().sprite = ButtonSpr_Disable;
            Kernel.colorManager.TryGetColor("ui_button_04_outline", out Color_Outline);
            Kernel.colorManager.TryGetColor("ui_button_04_shadow", out Color_Shadow);
            SetColor(SweepBtnText, Color_Outline, Color_Shadow);
        }


        //리스트 추가.
        EXP_Obj.SetActive(false);
        txtGetExpCount.text = Languages.GetNumberComma(packet.m_ClearTicketResult.m_iEarnExp);

        //유저네임 및 레벨
        int level = Kernel.entry.account.level;
        txtUserLevel.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level);
        txtUserName.text = string.Format("{0}", Kernel.entry.account.name);
        SetLevelMax(Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), level);
        SetTextCenter();

        //보상추가.
        RewardCardList.Clear();
        for (int idx = 0; idx < packet.m_ClearTicketResult.m_PvEResultList.Count; idx++)
        {
            CPvEResult pResultData = packet.m_ClearTicketResult.m_PvEResultList[idx];
            if (pResultData == null)
                break;

            GameObject RewardCard = Instantiate(PrefabRewardCard) as GameObject;
            RewardCard.GetComponent<UIAdventureRewardCard>().InitRewardCard(pResultData.m_eGoodsType, pResultData.m_iRewardCount);
            RewardCard.transform.parent = RewardGridParent;
            RewardCard.GetComponent<RectTransform>().localScale = Vector3.one;
            RewardCardList.Add(RewardCard);
        }

        //애니메이션 포지션.
        RewardPanelAnimation.gameObject.SetActive(false);

        Invoke("ShowResultObject_Exp", 0.5f);
    }




    protected override void Update()
    {
        base.Update();

        if (CheckEndMotion)
        {
            if (IsPlaying(PuccaSpineAnimation) == false)
            {
                ResetDefeatLoopMotion();
                CheckEndMotion = false;
            }
        }

        if (!SkipMode)
        {
            bool m_Clicked = false;
            if (Application.isMobilePlatform && Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                m_Clicked = (t.phase == TouchPhase.Began);
            }
            else
            {
                m_Clicked = Input.GetMouseButtonDown(0);
            }
            
            if (m_Clicked)
                SkipResultEvent();
        }


        //레벨업처리.
        if (UpdateAddExpMode)
        {
            if (SkipMode)
            {
                while (true)
                {
                    if (LeftAddExp <= OneAddExp)
                    {
                        CurExpCount += LeftAddExp;
                        LeftAddExp = 0;
                    }
                    else
                    {
                        CurExpCount += OneAddExp;
                        LeftAddExp -= OneAddExp;
                        if (LeftAddExp <= 0)
                            LeftAddExp = 0;
                    }

                    if (CurExpCount >= MaxExpCount)
                    {
                        CurExpCount -= MaxExpCount;
                        int NextLevel = Kernel.entry.battle.BattleStartLevel + 1;
                        if (NextLevel >= Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Account_Level_Limit))
                        {
                            CurExpCount = 0;
                            NextLevel = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Account_Level_Limit);
                            LeftAddExp = 0;
                        }
                        DB_AccountLevel.Schema TempAccountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, NextLevel);
                        MaxExpCount = TempAccountLevel.Need_AccountExp;
                        txtUserLevel.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), NextLevel);
                        txtUserName.text = string.Format("{0}", Kernel.entry.account.name);
                        SetLevelMax(Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Account_Level_Limit), NextLevel);
                        SetTextCenter();

                        if (LevelUpActive)
                        {
                            Kernel.entry.account.heart = LvUp_Heart;
                            Kernel.entry.account.ruby = LvUp_ruby;
                            CurAccountGold = LvUp_Gold;
                            Kernel.entry.account.gold = CurAccountGold;
                            Kernel.entry.account.level++;
                        }
                    }

                    if (LeftAddExp <= 0)
                        break;
                }
            }
            else
            {
                CurAddExpTime += Time.deltaTime;
                if (CurAddExpTime >= TimeDelayAddExp)
                {
                    CurAddExpTime = 0.0f;

                    if (LeftAddExp <= OneAddExp)
                    {
                        CurExpCount += LeftAddExp;
                        LeftAddExp = 0;
                    }
                    else
                    {
                        CurExpCount += OneAddExp;
                        LeftAddExp -= OneAddExp;
                        if (LeftAddExp <= 0)
                            LeftAddExp = 0;
                    }
                }

                if (CurExpCount >= MaxExpCount)
                {
                    CurExpCount -= MaxExpCount;
                    int NextLevel = Kernel.entry.battle.BattleStartLevel + 1;
                    if (NextLevel >= Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Account_Level_Limit))
                    {
                        CurExpCount = 0;
                        NextLevel = Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Account_Level_Limit);
                        LeftAddExp = 0;
                    }
                    DB_AccountLevel.Schema TempAccountLevel = DB_AccountLevel.Query(DB_AccountLevel.Field.AccountLevel, NextLevel);
                    MaxExpCount = TempAccountLevel.Need_AccountExp;

                    txtUserLevel.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), NextLevel);
                    txtUserName.text = string.Format("{0}", Kernel.entry.account.name);
                    SetLevelMax(Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Account_Level_Limit), NextLevel);
                    SetTextCenter();

                    if (LevelUpActive)
                    {
                        Kernel.entry.account.heart = LvUp_Heart;
                        Kernel.entry.account.ruby = LvUp_ruby;
                        CurAccountGold = LvUp_Gold;
                        Kernel.entry.account.gold = CurAccountGold;
                        Kernel.entry.account.level++;
                    }
                }
            }

            if (LeftAddExp <= 0)
            {
                UpdateAddExpMode = false;
                if (AdventureResultType != ADVENTURE_RESULT_TYPE.BATTLE_LOSE)
                {
                    Kernel.entry.account.exp = CurAccountExp;
                    Kernel.entry.account.gold = CurAccountGold;
                }

                if (SkipMode)
                {
                    ShowResultObject_Reward();
                    ShowResultObject_ButtonOK();
                }
                else
                {
                    Invoke("ShowResultObject_Reward", 0.3f);
                    Invoke("ShowResultObject_ButtonOK", 1.0f);
                }
            }
        }

        float CurWidth = CurExpCount * imgExpGaugeWidth / MaxExpCount;
        if (CurWidth <= 0.0f)
            CurWidth = 0.0f;
        if (CurWidth >= imgExpGaugeWidth)
            CurWidth = imgExpGaugeWidth;
        imgExpGauge.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurWidth);

        txtExpGaugeCount.text = string.Format("({0}/{1})", (int)CurExpCount, MaxExpCount);
    }



    public void PressOKButton()
    {
        switch (AdventureResultType)
        {
            case ADVENTURE_RESULT_TYPE.BATTLE_WIN:
            case ADVENTURE_RESULT_TYPE.BATTLE_LOSE:
                Kernel.sceneManager.LoadScene(Scene.Adventure);
                break;

            case ADVENTURE_RESULT_TYPE.SWEEP_END:
                Kernel.uiManager.Close(UI.AdventureResult);
                break;
        }
    }



    public void PressRetryButton()
    {
        Kernel.entry.adventure.PreSelectStageIndex = Kernel.entry.adventure.SelectStageIndex;
        Kernel.sceneManager.LoadScene(Scene.Adventure);
    }

    public void PressNextButton()
    {
        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, Kernel.entry.adventure.SelectStageIndex);
        int NextStageIdx = StageData.NextStageID;

        Kernel.entry.adventure.PreSelectStageIndex = NextStageIdx;
        Kernel.sceneManager.LoadScene(Scene.Adventure);
    }


    public void PressRetrySweepButton()
    {
        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, Kernel.entry.adventure.SelectStageIndex);

        if (Kernel.entry.account.clearTicket <= 0)
        {
            UINotificationCenter.Enqueue(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_CLEAR_TICKET));
        }
        else if (Kernel.entry.account.heart < StageData.Need_Heart)
        {
            UINotificationCenter.Enqueue(Languages.ToString(Result_Define.eResult.NOT_ENOUGH_TOKEN));
        }

        else
        {
            for (int idx = 0; idx < RewardCardList.Count; idx++)
            {
                Destroy(RewardCardList[idx].gameObject);
            }
            RewardCardList.Clear();

            Kernel.uiManager.Close(UI.AdventureResult);
            Kernel.entry.adventure.onUseClearTicket += RecvSweep;
            Kernel.entry.adventure.REQ_PACKET_CG_GAME_USE_CLEAR_TICKET_SYN();
        }
    }






    //결과창 애니 처리용.
    public void ShowResultObject_Exp()
    {
        EXP_Obj.SetActive(true);

        if (SkipMode)
            StartExpAnimation();
        else
            Invoke("StartExpAnimation", 0.5f);
    }

    public void StartExpAnimation()
    {
        UpdateAddExpMode = true;

    }

    public void ShowResultObject_Reward()
    {
        RewardPanelAnimation.gameObject.SetActive(true);
 
        RewardPanelAnimation.Play("AniResultPanelMove"); 
    }


    public void ShowResultObject_ButtonOK()
    {
        if (AdventureResultType == ADVENTURE_RESULT_TYPE.SWEEP_END)
        {
            ResultSweepButton_OK.gameObject.SetActive(true);
            ResultSweepButton_Retry.gameObject.SetActive(true);
        }
        else
        {
            ResultButton_Retry.gameObject.SetActive(true);
            ResultButton_OK.gameObject.SetActive(true);

            if (AdventureResultType == ADVENTURE_RESULT_TYPE.BATTLE_WIN)
            {
                ResultButton_Next.gameObject.SetActive(true);

                ResultButton_Retry.transform.localPosition = new Vector3(-ResultButtonGap, ResultButton_Retry.transform.localPosition.y, ResultButton_Retry.transform.localPosition.z);
                ResultButton_OK.transform.localPosition = new Vector3(0.0f, ResultButton_OK.transform.localPosition.y, ResultButton_OK.transform.localPosition.z);
                ResultButton_Next.transform.localPosition = new Vector3(ResultButtonGap, ResultButton_Next.transform.localPosition.y, ResultButton_Next.transform.localPosition.z);
            }
            else
            {
                ResultButton_Next.gameObject.SetActive(false);

                ResultButton_Retry.transform.localPosition = new Vector3(-ResultButtonGap / 2, ResultButton_Retry.transform.localPosition.y, ResultButton_Retry.transform.localPosition.z);
                ResultButton_OK.transform.localPosition = new Vector3(ResultButtonGap / 2, ResultButton_OK.transform.localPosition.y, ResultButton_OK.transform.localPosition.z);
            }

            //튜토리얼.
            if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 303)
                Kernel.entry.tutorial.onSetNextTutorial();
        }
    }












    public void RecvSweep(PACKET_CG_GAME_USE_CLEAR_TICKET_ACK packet)
    {
        Kernel.entry.adventure.onUseClearTicket -= RecvSweep;
        Kernel.entry.account.heart = packet.m_iRemainHeart;
        Kernel.entry.battle.BattleStartLevel = Kernel.entry.account.level;
        UIAdventureResult pResultMng = (UIAdventureResult)Kernel.uiManager.Open(UI.AdventureResult);
        pResultMng.ShowResult_Sweep(packet);

        Kernel.soundManager.PlayUISound(SOUND.SND_UI_PVP_PVE_RESULT_CLEAR);
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



    private void SkipResultEvent()
    {
        if (SkipMode)
            return;

        SkipMode = true;

        CancelInvoke("ShowResultObject_Exp");
        ShowResultObject_Exp();

        CancelInvoke("ShowResultObject_Reward");
        ShowResultObject_Reward();


        CancelInvoke("ShowResultObject_ButtonOK");
        ShowResultObject_ButtonOK();


    }






    //스파인.
    private void SetSpineMotion(string MotionName, bool loop)
    {
        PuccaSpineAnimation.state.SetAnimation(0, MotionName, loop);
    }

    private bool IsPlaying(SkeletonAnimation pAnimation)
    {
        if (pAnimation.state.GetCurrent(0) == null)
            return false;

        return true;
    }


    private void ResetDefeatMotion()
    {
        PuccaSpineAnimation.Reset();
        SetSpineMotion("defeat", false);
        CheckEndMotion = true;
    }


    private void ResetDefeatLoopMotion()
    {
        SetSpineMotion("defeat_loop", true);
    }
}
