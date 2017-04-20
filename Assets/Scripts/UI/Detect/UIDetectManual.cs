using Common.Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIDetectManual : UIObject
{
    public  Text    IslandName;
    public  Text    LeftTime;

    //밑판 섬 배경.
    public  GameObject[]        IslandObjectList;
    public  SkeletonAnimation[] PuccaSpineList;

    //뿌까.
    private SkeletonAnimation    PuccaSpine;

    //게이지.
    public  float           GaugeBaseWidth;
    public  Image           FillGaugeImage;
    public  RectTransform   FillGaugeTransform;


    private bool            TimeActiveMode;
    private float           CurDetectTime;
    public  float           MaxDetectTime;

    //상자오브젝트.
    public  GameObject      QuestionMarkObject;
    public  Animator        QuestionMarkAnimator;

    public  GameObject      BoxBaseObject;
    public  Image           BoxImage;

    private int             BoxIndex;


    //가속 버튼.
    public  Button          AddTimeButton;

    //취소 버튼.
    public  Button          DetectCancelButton;


    private bool            PauseMode;


    protected override void Awake()
    {
        base.Awake();
    }



    protected override void OnEnable()
    {
        base.OnEnable();

        AddTimeButton.onClick.AddListener(OnPressAddTime);
        DetectCancelButton.onClick.AddListener(OnPressDetectPause);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        AddTimeButton.onClick.RemoveAllListeners();
        DetectCancelButton.onClick.RemoveAllListeners();
    }




    public void InitDetectManual(int nBoxIndex, string szIslandName, int IslandNum)
    {
        TimeActiveMode = true;
        CurDetectTime = 0.0f;
        BoxIndex = nBoxIndex;

        IslandName.text = szIslandName;

        for (int idx = 0; idx < IslandObjectList.Length; idx++)
        {
            if (IslandNum == idx)
            {
                IslandObjectList[idx].SetActive(true);
                PuccaSpine = PuccaSpineList[idx];
            }
            else
                IslandObjectList[idx].SetActive(false);
        }


        QuestionMarkObject.SetActive(true);
        QuestionMarkAnimator.SetTrigger("Bubble");




        BoxBaseObject.SetActive(false);

        DB_TreasureDetectBoxGet.Schema BoxGetData = DB_TreasureDetectBoxGet.Query(DB_TreasureDetectBoxGet.Field.Index, BoxIndex);
        BoxImage.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, BoxGetData.Box_IdentificationName);    //고정이미지

        SetSpineAnimation("run");


        DetectCancelButton.gameObject.SetActive(true);
        PauseMode = false;
    }










    protected override void Update()
    {
        base.Update();

        if (TimeActiveMode && !PauseMode)
        {
            CurDetectTime += Time.deltaTime;
            if (CurDetectTime >= MaxDetectTime)
            {
                CurDetectTime = 0.0f;
                TimeActiveMode = false;
                ShowDetectBox();
            }
        }

        if (TimeActiveMode)
        {
            float CurGaugeWidth = CurDetectTime * GaugeBaseWidth / MaxDetectTime;
            FillGaugeTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CurGaugeWidth);

            LeftTime.text = Languages.ToString(TEXT_UI.DETECT_AUTO_FINISH_TIME, (int)(MaxDetectTime - CurDetectTime) + 1);
        }
        else
        {
            FillGaugeTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GaugeBaseWidth);
            LeftTime.text = string.Empty;
        }
    }














    private void ShowDetectBox()
    {
        SetSpineAnimation("win");
        QuestionMarkObject.SetActive(false);
        BoxBaseObject.SetActive(true);

        DetectCancelButton.gameObject.SetActive(false);

        Invoke("SetResultDetectBox", 1.0f);
    }

    private void SetResultDetectBox()
    {
        Kernel.entry.detect.onDetectOpenBox += ResultDetectBox;
        Kernel.entry.detect.REQ_PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_SYN();
    }


    private void ResultDetectBox(PACKET_CG_GAME_TREASURE_DETECT_OPEN_BOX_ACK packet)
    {
        Kernel.entry.detect.onDetectOpenBox -= ResultDetectBox;

        Kernel.entry.account.gold = packet.m_iTotalGold;

        for (int i = 0; i < packet.m_CardList.Count; i++)
        {
            Kernel.entry.character.UpdateCardInfo(packet.m_CardList[i]);
        }

        for (int i = 0; i < packet.m_SoulList.Count; i++)
        {
            Kernel.entry.character.UpdateSoulInfo(packet.m_SoulList[i]);
        }

        UIChestDirector chestDirector = Kernel.uiManager.Get<UIChestDirector>(UI.ChestDirector, true, false);
        if (chestDirector != null)
        {
            chestDirector.SetReward(BoxIndex, packet.m_iEarnGold, packet.m_BoxResultList);
            Kernel.uiManager.Close(UI.DetectManual);
            Kernel.uiManager.Open(UI.ChestDirector);
            chestDirector.DirectionByCoroutine();
        }

        if (Kernel.entry.detect.onUpdateIslandInfo != null)
            Kernel.entry.detect.onUpdateIslandInfo();
    }




    private void SetSpineAnimation(string AniName)
    {
        if(PuccaSpine == null)
            return;

        PuccaSpine.AnimationName = AniName;
        PuccaSpine.loop = true;
        PuccaSpine.Reset();
    }







    private void OnPressAddTime()
    {
        if (TimeActiveMode && !PauseMode)
        {
            QuestionMarkAnimator.SetTrigger("Bubble");
            Kernel.soundManager.PlaySound(SOUND.SND_BUTTON_GOOD_0);

            CurDetectTime += 0.25f;
            if (CurDetectTime >= MaxDetectTime)
            {
                CurDetectTime = 0.0f;
                TimeActiveMode = false;
                ShowDetectBox();
            }
        }
    }



    private void OnPressDetectPause()
    {
        PauseMode = true;
        UIAlerter.Alert(Languages.ToString(TEXT_UI.TREASURE_DETECT_EXIT), UIAlerter.Composition.Confirm_Cancel, OnResponseCallback);
    }


    private void OnResponseCallback(UIAlerter.Response response, params object[] args)
    {
        if (response == UIAlerter.Response.Confirm)
        {
            Kernel.uiManager.Close(UI.DetectManual);

            if (Kernel.entry.detect.onUpdateIslandInfo != null)
                Kernel.entry.detect.onUpdateIslandInfo();
        }
        else
        {
            PauseMode = false;
        }
    }













}
