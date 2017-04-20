using Common.Packet;
using UnityEngine;
using UnityEngine.UI;

public class UIChest : MonoBehaviour
{
    public Button m_Button;
    public SkeletonAnimation m_SkeletonAnimation;
    public Image m_EmptyImage;
    public Image m_BackgroundImage;
    public Text m_NameText;
    public Text m_AreaText;
    public UISlider m_StarSlider;

    int m_Index;
    long m_Sequnece;
    bool m_Available;

    void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onGoodsUpdate += OnGoodsUpdate;
        }
    }

    void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.account.onGoodsUpdate -= OnGoodsUpdate;
        }
    }

    void OnGoodsUpdate(int friendship, int gold, int heart, int ranking, int ruby, int star, int guildPoint, int revengePoint, int smilePoint)
    {
        m_StarSlider.value = star;
    }

    public CRewardBox rewardBox
    {
        set
        {
            if (value != null)
            {
                m_Available = (value.m_iBoxIndex > 0);
                if (m_Available)
                {
                    m_Index = value.m_iBoxIndex;
                    m_Sequnece = value.m_Sequence;

                    DB_BoxGet.Schema boxGet = DB_BoxGet.Query(DB_BoxGet.Field.Index, m_Index);
                    if (boxGet != null)
                    {
                        SetSkeletonAnimation(boxGet.Box_IdentificationName);
                        m_NameText.text = Languages.ToString(boxGet.TEXT_UI);
                        m_AreaText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.CARD_DECK_AREA), value.m_byObtainArea);
                        m_StarSlider.maxValue = boxGet.Star_Point;
                        m_StarSlider.value = Kernel.entry.account.starPoint;
                    }
                }
                else
                {
                    m_Index = 0;
                    m_Sequnece = 0;
                }
            }

            m_EmptyImage.gameObject.SetActive(!m_Available);
            m_BackgroundImage.gameObject.SetActive(m_Available);
        }
    }

    bool SetSkeletonAnimation(string identificationName)
    {
        if (!string.IsNullOrEmpty(identificationName))
        {
            string assetPath = string.Format("Spines/RewardBox/{0}/{0}_SkeletonData", identificationName);
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                m_SkeletonAnimation.skeletonDataAsset = skeletonDataAsset;
                m_SkeletonAnimation.initialSkinName = identificationName;
                m_SkeletonAnimation.AnimationName = "lock";
                m_SkeletonAnimation.loop = true;
                m_SkeletonAnimation.Reset();

                return true;
            }
            else
            {
                Debug.LogError(assetPath);
            }
        }

        return false;
    }

    void OnClick()
    {
        if (m_Available && Kernel.uiManager != null)
        {
            UIChestInfo chestInfo = Kernel.uiManager.Get<UIChestInfo>(UI.ChestInfo, true, false);
            if (chestInfo != null)
            {
                chestInfo.sequnece = m_Sequnece;
                Kernel.uiManager.Open(UI.ChestInfo);
            }
        }
    }
}
