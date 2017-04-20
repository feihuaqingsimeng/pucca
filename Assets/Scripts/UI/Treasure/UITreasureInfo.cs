using UnityEngine;
using Common.Packet;
using System.Collections;
using UnityEngine.UI;
using System;

public class UITreasureInfo : MonoBehaviour {

    private const float     F_MUTI_MINSLIDERVALUE       = 0.1f;
    private const float     F_GAGEBAR_CHAGE_COLOR_SPEED = 0.5f;

    private const string    STR_MAIN_IMAGE_BASE_NAME    = "Farm_Dock_0";
    private const string    STR_BOX_IMAGE_BASE_NAME     = "Reward_Box_";
    private const string    STR_BOAT_BASE_NAME          = "TREASURE_BOAT_";
    private const string    STR_BOAT_DEC_BASE_NAME      = "TREASURE_BOAT_TEXT_";

    //private const string    STR_TIME_COLOR              = "<color=#FF4949FF>{0:00}</color>";

    //** TreasureBox Data
    private UITreasure      m_Owner;
    private TreasureInfo    m_TreasureInfo;
    private long            m_lSequence;
    public  int             m_iBoxNum;
    public  int             m_iBoxIndex;

    private bool            m_bSettingComplet;
    private bool            m_bOpenedBox;    // 계정 레벨 조건이 되서 열려있는지...
    private bool            m_bComplet;     // 시간이 다되서 받을 수 있는 상태가 됬는지...

    public Button           m_OpenButton;
    public Slider           m_TimeSlider;

    public SkeletonAnimation m_SkeletonAnim;
    //public Animation        m_ActiveAnimation;
    public Animator         m_ActiveAnimation;

    public GameObject       m_Lock;
    public GameObject       m_SliderMaxImage;

    public Image            m_MainImg;
    public Image            m_TreasureBoxBackGround;
    public Image            m_TreasureBoxImg;
    //public Image            m_BadgeImage;
    public Text             m_BoatName;
    public Text             m_BoatDec;
    public Text             m_RemainTime;
    public Text             m_NeedOpenLevel;

    public Sprite           m_WaitButtonSprite;
    public Sprite           m_CompletButtonSprite;


    private void Awake()
    {
        // OpenButton
        if (m_OpenButton != null)
            m_OpenButton.onClick.AddListener(OnClickOpenBox);

        m_ActiveAnimation = GetComponent<Animator>();
    }

    private void SetInit()
    {
        m_bSettingComplet = false;
        m_bComplet = false;
        m_Lock.SetActive(false);
        //m_BadgeImage.enabled = false;

        m_TreasureBoxBackGround.sprite = m_WaitButtonSprite;
    }

    //** Setting Data Items
    public void SettingItem(UITreasure owner, CTreasureBox treasureBox, int boxNum)
    {
        if (treasureBox == null || owner == null)
            return;

        SetInit();

        m_Owner = owner;
        m_lSequence = treasureBox.m_Sequence;
        m_iBoxNum = boxNum;
        m_iBoxIndex = treasureBox.m_iBoxIndex;
        m_TreasureInfo = Kernel.entry.treasure.FindTreasureBox(m_lSequence);

        // OpenLevel
        SetLockBox(boxNum);

        // MainImg
        string mainImgName = STR_MAIN_IMAGE_BASE_NAME + (boxNum - 1).ToString() + (m_bOpenedBox ? "" : "_Locked");
        m_MainImg.sprite = TextureManager.GetSprite(SpritePackingTag.Treasure, mainImgName);

        // TreasureBoxImg
        //박스 이름이 6부터 시작하기 때문. 만약 추가된 박스가 숫자가 증가가 아닌 랜덤이면 수정해야함.
        m_TreasureBoxImg.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, STR_BOX_IMAGE_BASE_NAME + (boxNum + 5).ToString());    //고정이미지
        DB_BoxGet.Schema boxGet = DB_BoxGet.Query(DB_BoxGet.Field.Index, treasureBox.m_iBoxIndex);
        SetSkeletonAnimation(boxGet.Box_IdentificationName);

        // BoatName
        string boatName = STR_BOAT_BASE_NAME + (boxNum);
        TEXT_UI enumBoatName = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), boatName);
        m_BoatName.text = Languages.ToString(enumBoatName);

        // BoatDec
        string boatDecName = STR_BOAT_DEC_BASE_NAME + (boxNum);
        TEXT_UI enumBoatDecName = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), boatDecName);
        m_BoatDec.text = Languages.ToString(enumBoatDecName);

        // Slider
        TreasureInfo boxData = Kernel.entry.treasure.FindTreasureBox(m_lSequence);
        m_TimeSlider.maxValue = boxData.m_treasureDBData.Open_Time;

        m_SkeletonAnim.gameObject.SetActive(m_bComplet);
        m_TreasureBoxImg.gameObject.SetActive(!m_bComplet);

        m_bSettingComplet = true;

        // 모든 
        m_Owner.SetAbleButtons(true);
    }

    //** 보물 상자 SkeletonAnimation
    private void SetSkeletonAnimation(string identificationName)
    {
        if (!string.IsNullOrEmpty(identificationName))
        {
            string assetPath = string.Format("Spines/RewardBox/{0}/{0}_SkeletonData", identificationName);
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                m_SkeletonAnim.skeletonDataAsset = skeletonDataAsset;
                m_SkeletonAnim.initialSkinName = identificationName;
                m_SkeletonAnim.AnimationName = "lock";
                m_SkeletonAnim.loop = true;
                m_SkeletonAnim.Reset();
            }
            else
            {
                Debug.LogError(assetPath);
            }
        }
    }

    //** BoxLock(계정 레벨에 따른)
    private void SetLockBox(int boxNum)
    {
        int openLevel = Kernel.entry.treasure.FindTreasureBoxOpenLevel(m_lSequence);
        m_bOpenedBox = Kernel.entry.account.level < openLevel ? false : true;

        m_NeedOpenLevel.text = !m_bOpenedBox
            ? Languages.ToString(TEXT_UI.ACCOUNT_LEVEL_OPEN, openLevel)
            : "";

        m_Lock.SetActive(!m_bOpenedBox);
        m_TimeSlider.gameObject.SetActive(m_bOpenedBox);
    }

    private void Update()
    {
        if (!m_bSettingComplet || !m_bOpenedBox)
            return;
        // 완료된 상태에서 value값을 주지 않으면, 게임상에서 슬라이드를 조절할 수 있는 문제점이 있어서.. 
        // 업데이트에서 value값을 계속 변경.
        if (m_bComplet)
        {
            m_TimeSlider.value = m_TimeSlider.maxValue;
            return;
        }

        if (m_TreasureInfo == null)
            return;

        bool complet = m_TreasureInfo.m_RemainTime <= TimeSpan.Zero;

        m_SliderMaxImage.SetActive(complet);
        m_RemainTime.gameObject.SetActive(!complet);

        if (complet)
            CompletBox();
        else
            WaitBox();
    }

    //** 보상 완료 되기 전
    private void WaitBox()
    {
        // 01 : Time에 색상 표기 방식
        //string hourTime = string.Format(STR_TIME_COLOR, m_TreasureInfo.m_RemainTime.Hours);
        //string minuteTime = string.Format(STR_TIME_COLOR, m_TreasureInfo.m_RemainTime.Minutes);
        //string seconTime = string.Format(STR_TIME_COLOR, m_TreasureInfo.m_RemainTime.Seconds);

        //m_RemainTime.text = string.Format(hourTime + " : " + minuteTime + " : " + seconTime);

        // 02 : Time에 흰색으로 그냥 표기 방식
        m_RemainTime.text = string.Format("{0:00} : {1:00} : {2:00}", m_TreasureInfo.m_RemainTime.Hours, m_TreasureInfo.m_RemainTime.Minutes, m_TreasureInfo.m_RemainTime.Seconds);

        float totalSecons = m_TimeSlider.maxValue - (float)m_TreasureInfo.m_RemainTime.TotalSeconds;
        float minSecons = m_TimeSlider.maxValue * F_MUTI_MINSLIDERVALUE;
        m_TimeSlider.value = (totalSecons <= minSecons && m_TimeSlider.value != 0.0f) ? minSecons : totalSecons;

        //m_BadgeImage.enabled = false;
    }

    //** 보상받을 수 있는 상태가 될때
    private void CompletBox()
    {
        m_bComplet = true;

        //// 시간 Text를 완료로 변경.
        //m_RemainTime.text = Languages.ToString(TEXT_UI.FINISH);

        m_TreasureBoxBackGround.sprite = m_CompletButtonSprite;
        m_SkeletonAnim.gameObject.SetActive(m_bComplet);
        m_TreasureBoxImg.gameObject.SetActive(!m_bComplet);

        //m_BadgeImage.enabled = true;
    }

    //** Box 오픈!!!
    private void OnClickOpenBox()
    {
        SoundDataInfo.CancelSound(m_OpenButton.gameObject);

        // 계정 레벨 조건이 되는지?
        if (!m_bOpenedBox)
        {
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.NOT_ENOUGH_ACCOUNT_LEVEL));
            return;
        }

        // Open된 것 중 시간 완료된것 체크
        if (!m_bComplet)
        {
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.CANT_OPEN_YET));
            return;
        }

        SoundDataInfo.RevertSound(m_OpenButton.gameObject);

        // 모든 조건 만족
        m_Owner.SetAbleButtons(false);
        Kernel.entry.treasure.REQ_PACKET_CG_GAME_OPEN_TREASURE_BOX_SYN(m_lSequence);
    }
}
