using Common.Packet;
using Common.Util;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIChestInfo : UIObject
{
    public Image m_ChestImage;
    public Text m_AreaText;
    public Text m_NameText;
    public UISlider m_StarSlider;
    public Text m_GoldText;
    public Text m_CardCountText;
    public List<Image> m_GradeImages;
    public Text m_WarningText;
    public Button m_OpenButton;
    public Button m_ImmediateOpenButton;
    public Text m_RubyText;
    public Text m_EmptyText;

    long m_Sequence;
    bool m_Openable;

    protected override void Awake()
    {
        base.Awake();

        m_OpenButton.onClick.AddListener(OnOpenButtonClick);
        m_ImmediateOpenButton.onClick.AddListener(OnImmediateOpenButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestOpen += OnChestOpen;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestOpen -= OnChestOpen;
        }
    }

    void OnChestOpen(int boxIndex, int gold, List<CBoxResult> boxResultList)
    {
        Kernel.uiManager.Close(UI.ChestInfo);

        UIChestDirector chestDirector = Kernel.uiManager.Get<UIChestDirector>(UI.ChestDirector, true, false);
        if (chestDirector != null)
        {
            chestDirector.SetReward(boxIndex, gold, boxResultList);
            Kernel.uiManager.Open(UI.ChestDirector);
            chestDirector.DirectionByCoroutine();
        }
    }

    public long sequnece
    {
        get
        {
            return m_Sequence;
        }

        set
        {
            m_Sequence = value;

            CRewardBox rewardBox = Kernel.entry.chest.FindRewardBox(m_Sequence);
            if (rewardBox != null)
                SetUI(rewardBox.m_iBoxIndex, rewardBox.m_byObtainArea);
        }
    }

    public void SetUI(int boxIndex, byte obtainArea = 0, bool isBoxGet = true, bool isPackage = false)
    {
        string boxIconName = "";
        TEXT_UI boxName = TEXT_UI.NONE;
        int boxStarPoint = 0;
        int boxGoldMin = 0;
        int boxGoldMax = 0;
        int boxCardTotalCount   = 0;
        int boxGradeAMinCount   = 0;
        int boxGradeBMinCount   = 0;
        int boxGradeSMinCount   = 0;
        int boxGradeSSMinCount  = 0;

        if (!isPackage)
        {
            DB_BoxGet.Schema boxGet = DB_BoxGet.Query(DB_BoxGet.Field.Index, boxIndex);

            if (boxGet == null)
                return;

            boxIconName         = boxGet.Box_IdentificationName;
            boxName             = boxGet.TEXT_UI;
            boxStarPoint        = boxGet.Star_Point;
            boxGoldMin          = boxGet.Gold_Min;
            boxGoldMax          = boxGet.Gold_Max;
            boxCardTotalCount   = boxGet.Total_Count;
            boxGradeAMinCount   = boxGet.A_Type_Min;
            boxGradeBMinCount   = boxGet.B_Type_Min;
            boxGradeSMinCount   = boxGet.S_Type_Min;
        }
        else
        {
            DB_Package_BoxGet.Schema packageBoxGet = DB_Package_BoxGet.Query(DB_Package_BoxGet.Field.Index, boxIndex);

            if (packageBoxGet == null)
                return;

            boxIconName         = packageBoxGet.Box_IdentificationName;
            boxName             = packageBoxGet.TEXT_UI;
            boxStarPoint        = packageBoxGet.Star_Point;
            boxGoldMin          = packageBoxGet.Gold_Min;
            boxGoldMax          = packageBoxGet.Gold_Max;
            boxCardTotalCount   = packageBoxGet.Total_Count;
            boxGradeAMinCount   = packageBoxGet.A_Type_Min;
            boxGradeBMinCount   = packageBoxGet.B_Type_Min;
            boxGradeSMinCount   = packageBoxGet.S_Type_Min;
            boxGradeSSMinCount  = packageBoxGet.SS_Type_Min;
        }

            m_ChestImage.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, boxIconName);
            m_ChestImage.SetNativeSize();
            m_AreaText.text = obtainArea > 0 ? string.Format("{0} {1}", obtainArea, Languages.ToString(TEXT_UI.CARD_DECK_AREA)) : "";
            m_NameText.text = Languages.ToString(boxName);
            m_StarSlider.maxValue = boxStarPoint;
            m_StarSlider.value = Kernel.entry.account.starPoint;
            m_GoldText.text = string.Format("{0} ~ {1}", boxGoldMin, boxGoldMax);
            m_CardCountText.text = string.Format("x{0}", boxCardTotalCount);
            m_RubyText.text = Languages.ToString<int>(boxStarPoint);

            int minimum = 0;
            bool activeSelf = false;
            for (int i = 0; i < m_GradeImages.Count; i++)
            {
                switch ((Grade_Type)(i + 3))
                {
                    case Grade_Type.Grade_B:
                        minimum = boxGradeBMinCount;
                        break;
                    case Grade_Type.Grade_A:
                        minimum = boxGradeAMinCount;
                        break;
                    case Grade_Type.Grade_S:
                        minimum = boxGradeSMinCount;
                        break;
                    case Grade_Type.Grade_SS:
                        minimum = boxGradeSSMinCount;
                        break;
                }
                m_GradeImages[i].gameObject.SetActive(minimum > 0);
                activeSelf = activeSelf || m_GradeImages[i].gameObject.activeSelf;
            }

            m_EmptyText.gameObject.SetActive(!activeSelf);
            m_Openable = (Kernel.entry.account.starPoint >= boxStarPoint);
            m_OpenButton.gameObject.SetActive(isBoxGet && m_Openable);
            m_ImmediateOpenButton.gameObject.SetActive(isBoxGet && !m_Openable);
            m_WarningText.gameObject.SetActive(isBoxGet && !m_Openable);
            m_StarSlider.gameObject.SetActive(isBoxGet);
    }

    void OnOpenButtonClick()
    {
        if (Kernel.entry != null && m_Openable)
        {
            Kernel.entry.chest.REQ_PACKET_CG_GAME_OPEN_REWARD_BOX_SYN(m_Sequence, eGoodsType.StarPoint);
        }
    }

    void OnImmediateOpenButtonClick()
    {
        if (Kernel.entry != null && !m_Openable)
        {
            Kernel.entry.chest.REQ_PACKET_CG_GAME_OPEN_REWARD_BOX_SYN(m_Sequence, eGoodsType.Ruby);
        }
    }
}
