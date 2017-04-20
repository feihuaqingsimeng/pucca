using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Packet;

public class UIBattleResult : UIObject
{
    //획득한 랭킹포인트.
    public RectTransform    RankPointInfo;
    public Text             GetRankingPoint;

    //별보상.
    public GameObject       UIStarSlotPrefab;
    public GameObject       StarSlotGrid;

    public GameObject       EffectPrefab_PopBox;
    private int             GetStarCount;


    //승리 텍스트 이미지.
    public GameObject       ResultText_Win;
    public GameObject       ResultText_Lose;
    public GameObject       ResultText_Draw;


    //상자 스파인.
    private bool                GetChest = false;
    public Transform            BoxParentTransform;
    public Animator             BoxPopEventAnimation;
    public AnimationEventHandler BoxPopEventHandler;

    public SkeletonAnimation    BoxSpineAnimaiton;


    //연승보너스 리스트.
    public Text             StreakTitleText;
    public GameObject       StreakInfoMsg;
    public GameObject[]     StreakStarList;
    private int             GetStreakStarCount;
    private int             NowStreakStarOpenCount;

    //마일리지.
    public Text             txtMileageCount;
    public RectTransform    GaugeMileage;
    public float            GaugeMileageWidth;
    public UITooltipObject  MileageTooltipObject;

    private bool            MileageGauge_FillMode;
    private float           MileageGauge_TotalAddValue;
    private float           MileageGauge_AddTime;
    private float           MileageGauge_DelayTime = 0.01f;

    private int             GetMileageCount_Base;
    private int             GetMileageCount_Next;
    private CReceivedGoods  GetMileageResult;

    private BATTLE_RESULT_STATE CurResultType;


    //나가기 버튼.
    public GameObject       ChestButtonObject;
    public GameObject       ExitButtonObject;


    private bool             SkipMode;


    protected override void Awake()
    {
        base.Awake();

        BoxPopEventHandler.onAnimationEventCallback += OnAnimationEvent;
        ExitButtonObject.GetComponent<Button>().onClick.AddListener(OnPressExitBattle);
        ChestButtonObject.GetComponent<Button>().onClick.AddListener(OnPressExitBattle);
        BoxPopEventAnimation = BoxParentTransform.GetComponent<Animator>();

        if (MileageTooltipObject != null)
        {
            MileageTooltipObject.content = Languages.ToString(TEXT_UI.WIN_BONUS_INFO, 1, 10);
        }
    }




    public void InitBattleResult(BattleManager pBattleMng, BATTLE_RESULT_STATE eResultType, 
        int GetRankPoint, int GetStarPoint, int Boxindex, 
        int StreakCount, int StreakReward, 
        int MileageCount, CReceivedGoods MileageReward)
    {
        BoxParentTransform.gameObject.SetActive(false);

        //획득 랭킹점수.
        if(GetRankPoint >= 0)
            GetRankingPoint.text = "+ " + Languages.GetNumberComma(GetRankPoint);
        else
            GetRankingPoint.text = Languages.GetNumberComma(GetRankPoint);


        //승리 텍스트와 별 오브젝트.
        ResultText_Win.SetActive(false);
        ResultText_Lose.SetActive(false);
        ResultText_Draw.SetActive(false);
        GetStarCount = GetStarPoint;


        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.GroupNumber == 10)
        {
            GetStarCount = 5;
            StreakCount = 1;
        }

        CurResultType = eResultType;
        switch (CurResultType)
        {
            case BATTLE_RESULT_STATE.WIN:
                ResultText_Win.SetActive(true);
                break;

            case BATTLE_RESULT_STATE.LOSE:
                ResultText_Lose.SetActive(true);
                break;
            
            case BATTLE_RESULT_STATE.DRAW:
                ResultText_Draw.SetActive(true);
                break;
        }

        ExitButtonObject.SetActive(false);
        ChestButtonObject.SetActive(false);


        for (int idx = 0; idx < 5; idx++)
        {
            GameObject TempStarSlot = Instantiate(UIStarSlotPrefab) as GameObject;
            TempStarSlot.transform.parent = StarSlotGrid.transform;
            TempStarSlot.transform.localPosition = Vector3.zero;
            TempStarSlot.transform.localScale = Vector3.one;

            UIStarSlot pStarSlot = TempStarSlot.GetComponent<UIStarSlot>();
            if (idx < GetStarCount)
                pStarSlot.InitStarSlot(0.7f + idx * 0.13f);
            else
                pStarSlot.HideStartAnimation();
        }

        //데이터 갱신.
        Kernel.entry.account.winningStreak = StreakCount;
        Kernel.entry.account.winPoint = MileageCount;


        //연승횟수.
        if (StreakCount == 0)
        {
            StreakTitleText.gameObject.SetActive(false);

            RankPointInfo.anchoredPosition = new Vector2(60.0f, -50.0f);
        }
        else
        {
            RankPointInfo.anchoredPosition = new Vector2(60.0f, -6.6f);

            StreakTitleText.gameObject.SetActive(true);
            string WinCountStr = "<color=#ffc600ff>" + StreakCount.ToString() + "</color>";
            StreakTitleText.text = Languages.ToString(TEXT_UI.WIN_CONTINUE_BONUS, WinCountStr);
            GetStreakStarCount = StreakReward;
            NowStreakStarOpenCount = 0;
        }

        if (StreakCount < 2 && eResultType == BATTLE_RESULT_STATE.WIN)
            StreakInfoMsg.gameObject.SetActive(true);
        else
            StreakInfoMsg.gameObject.SetActive(false);


        //연승 별.
        for (int idx = 0; idx < StreakStarList.Length; idx++)
        {
            StreakStarList[idx].SetActive(false);
        }


        //마일리지.
        GetMileageResult = MileageReward;


        //0일때는 보상이 없으면 정말 0, 있으면 10.
        if (MileageCount == 0)
        {
            if (GetMileageResult.m_eGoodsType == Common.Util.eGoodsType.Free)
            {
                GetMileageCount_Base = 0;
                GetMileageCount_Next = 0;
            }
            else
            {
                GetMileageCount_Base = 9;
                GetMileageCount_Next = 10;
                Kernel.entry.account.SetValue(MileageReward.m_eGoodsType, MileageReward.m_iTotalAmount);    //보상적용.
            }
        }
        else
        {
            if(eResultType == BATTLE_RESULT_STATE.WIN)
                GetMileageCount_Base = MileageCount - 1;
            else
                GetMileageCount_Base = MileageCount;
            GetMileageCount_Next = MileageCount;
        }

        txtMileageCount.text = GetMileageCount_Base.ToString();

        //상자.
        if (Boxindex > 0)
        {
            GetChest = true;
            DB_BoxGet.Schema BoxData = DB_BoxGet.Query(DB_BoxGet.Field.Index, Boxindex);
            if (BoxData != null)
            {
                string assetPath = string.Format("Spines/RewardBox/{0}/{0}_SkeletonData", BoxData.Box_IdentificationName);
                SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
                if (skeletonDataAsset != null)
                {
                    BoxSpineAnimaiton.skeletonDataAsset = skeletonDataAsset;
                    BoxSpineAnimaiton.initialSkinName = BoxData.Box_IdentificationName;
                    BoxSpineAnimaiton.Reset();
                }
                else
                {
                    Debug.LogError(assetPath);
                }
            }
            BoxParentTransform.gameObject.SetActive(false);
            Invoke("ShowChestEvent", 1.5f);
            Invoke("MakeChestPopEffect", 1.5f);
        }
        else
        {
            GetChest = false;
            if (eResultType == BATTLE_RESULT_STATE.WIN)
            {
                if (GetStreakStarCount > 0)    //연승별보상 작업중....
                    Invoke("ShowStreakStarEvent", 0.5f);
                else
                    Invoke("ShowMileageEvent", 0.5f);
            }
            else
            {
                BoxParentTransform.gameObject.SetActive(false);
                Invoke("ShowExitButton", 0.5f);
            }
        }

        SkipMode = false;
    }




    
    public void ShowChestEvent()
    {
        if(!SkipMode)
            Invoke("MakeResultBox", 0.2f);
    }

    public void MakeResultBox()
    {
        BoxParentTransform.gameObject.SetActive(true);
        BoxPopEventAnimation.ResetTrigger("Normal");
        BoxPopEventAnimation.ResetTrigger("Chest_Open_ani");
        BoxPopEventAnimation.SetTrigger("Chest_Open_ani");
    }



    void MakeChestPopEffect()
    {
        GameObject pEff = Instantiate(EffectPrefab_PopBox) as GameObject;
        pEff.transform.SetParent(BoxParentTransform);
        pEff.transform.localPosition = new Vector3(0.0f, 65.0f, 0.0f);
        pEff.transform.localScale = Vector3.one * 2.0f;
        Destroy(pEff, 2.0f);
        Kernel.soundManager.PlayUISound(SOUND.SND_UI_PVP_RESULT_BOX);
    }


    new void OnAnimationEvent(string value)
    {
        if (string.Equals("Dropped", value))
        {
            BoxSpineAnimaiton.AnimationName = "lock";
            BoxSpineAnimaiton.loop = true;
            BoxSpineAnimaiton.Reset();

            if (!SkipMode)
            {
                if (GetStreakStarCount > 0)    //연승별보상 작업중....
                    Invoke("ShowStreakStarEvent", 0.5f);
                else
                    Invoke("ShowMileageEvent", 0.5f);
            }
        }
    }

    void SkipChestAnimation()
    {
        if (!GetChest)
            return;

        BoxParentTransform.gameObject.SetActive(true);

        BoxPopEventAnimation.Stop();
        BoxPopEventAnimation.ResetTrigger("Normal");
        BoxPopEventAnimation.ResetTrigger("Chest_Open_ani");

        BoxSpineAnimaiton.GetComponent<RectTransform>().localPosition = new Vector3(0.0f, -440.8f, 0.0f);
        BoxSpineAnimaiton.AnimationName = "lock";
        BoxSpineAnimaiton.loop = true;
        BoxSpineAnimaiton.Reset();

    }






    //연승보너스.
    public void ShowStreakStarEvent()
    {
        //스스로 루프.
        if (NowStreakStarOpenCount < GetStreakStarCount)
        {
            StreakStarList[NowStreakStarOpenCount].SetActive(true);
            NowStreakStarOpenCount++;

            if (SkipMode)
                ShowStreakStarEvent();
            else
                Invoke("ShowStreakStarEvent", 0.15f);
        }
        else
        {
            if (SkipMode)
                ShowMileageEvent();
            else
                Invoke("ShowMileageEvent", 0.5f);
        }
    }

    public void ShowMileageEvent()
    {
        if (GetMileageCount_Next == 0)
        {
            if (SkipMode)
                ShowExitButton();
            else
                Invoke("ShowExitButton", 0.5f);
        }
        else
        {
            //마일리지.
            MileageGauge_FillMode = true;
            MileageGauge_AddTime = 0.0f;
            MileageGauge_TotalAddValue = 0.0f;
        }
    }








    protected override void Update()
    {
        base.Update();

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


        if (MileageGauge_FillMode)
        {
            if (SkipMode)
            {
                MileageGauge_AddTime = MileageGauge_DelayTime;
                MileageGauge_TotalAddValue = 1.0f;
            }

            MileageGauge_AddTime += Time.deltaTime;
            if (MileageGauge_AddTime >= MileageGauge_DelayTime)
            {
                MileageGauge_AddTime = 0.0f;
                MileageGauge_TotalAddValue += 0.02f;
            }

            if (MileageGauge_TotalAddValue >= 1.0f)
            {
                MileageGauge_TotalAddValue = 1.0f;
                MileageGauge_FillMode = false;

                if (GetMileageCount_Next == 10)  //보상이면.
                {
                    UIMileageDirector mileageDirector = Kernel.uiManager.Get<UIMileageDirector>(UI.MileageDirector, true, false);
                    if (mileageDirector != null)
                    {
                        mileageDirector.RecvGoods = GetMileageResult;
                        Kernel.uiManager.Open(UI.MileageDirector);
                    }

                    GetMileageCount_Next = 0;
                    GetMileageCount_Base = 0;
                    MileageGauge_TotalAddValue = 0.0f;
                }
                txtMileageCount.text = GetMileageCount_Next.ToString();

                if (SkipMode)
                    ShowExitButton();
                else
                    Invoke("ShowExitButton", 0.5f);
            }
        }

        float GaugeValue = ((float)GetMileageCount_Base + MileageGauge_TotalAddValue) * GaugeMileageWidth / 10.0f;
        GaugeMileage.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GaugeValue);
    }



















    public void ShowExitButton()
    {
        ExitButtonObject.SetActive(true);
        ChestButtonObject.SetActive(true);


        if (Kernel.entry.tutorial.TutorialActive)
        {
            if (Kernel.entry.tutorial.GroupNumber == 10)
            {
                if (Kernel.entry.tutorial.WaitSeq == 105)
                {
                    Kernel.entry.tutorial.onSetNextTutorial();
                }
            }
        }

    }




    void OnPressExitBattle()
    {
        Kernel.uiManager.Close(UI.BattleResult);
        Kernel.sceneManager.LoadScene(Scene.Lobby);

    }










    private void SkipResultEvent()
    {
        if (SkipMode)
            return;

        SkipMode = true;

        CancelInvoke("MakeChestPopEffect");

        BoxPopEventHandler.onAnimationEventCallback -= OnAnimationEvent;
        SkipChestAnimation();

        if (CurResultType == BATTLE_RESULT_STATE.WIN)
        {
            CancelInvoke("ShowStreakStarEvent");
            ShowStreakStarEvent();

            CancelInvoke("ShowMileageEvent");
            ShowMileageEvent();
        }
        else
        {
            CancelInvoke("ShowStreakStarEvent");
            CancelInvoke("ShowMileageEvent");
        }

        ShowExitButton();
    }



}
