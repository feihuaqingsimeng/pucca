using Common.Packet;
using UnityEngine;
using UnityEngine.UI;

public class UILobbyCharacterSummary : MonoBehaviour
{
    private TextLevelMaxEffect  m_LevelMaxEffect;

    public SkeletonAnimation    m_SkeletonAnimation;
    public RectTransform        m_rtrsParent;
    
    
    public Text     m_Name_Text;
    public Text     m_Level_Text;

    public Image    m_ClassIconImage;

    public Button   m_Button;

    long m_CID;

    void Awake()
    {
        m_Button.onClick.AddListener(OnClick);

      
    }

    // Use this for initialization

    // Update is called once per frame

    public Vector2 headWorldPosition
    {
        get
        {
            if (m_SkeletonAnimation != null &&
            m_SkeletonAnimation.skeleton != null &&
            m_SkeletonAnimation.skeleton.bones != null)
            {
                foreach (var bone in m_SkeletonAnimation.skeleton.bones)
                {
                    if (string.Equals(bone.data.name, "head"))
                    {
                        float worldX, worldY;
                        bone.localToWorld(bone.x, bone.y, out worldX, out worldY);

                        return m_SkeletonAnimation.transform.TransformPoint(worldX, worldY + (bone.scaleY * 0.5f), 0f);
                    }
                }
            }

            return Vector2.zero;
        }
    }

    public long cid
    {
        get
        {
            return m_CID;
        }

        set
        {
            if (m_CID != value)
            {
                m_CID = value;

                CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(m_CID);
                if (cardInfo != null)
                {
                    DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, cardInfo.m_iCardIndex);
                    if (card != null)
                    {
                        SetSkeletonAnimation(card.IdentificationName, new Vector3(card.SizeRate * 120f, card.SizeRate * 120f, 0f)); // 120f : 1.2배
                        m_ClassIconImage.sprite = TextureManager.GetClassTypeIconSprite(card.ClassType);
                    }

                    string htmlStringRGBA;
                    Kernel.colorManager.TryGetHtmlStringRGBA(card.Grade_Type.ToString(), out htmlStringRGBA);

                    m_Name_Text.text = string.Format("<color={0}>{1}</color>", htmlStringRGBA, Languages.FindCharName(cardInfo.m_iCardIndex));
                    m_Level_Text.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), cardInfo.m_byLevel);

                    m_Name_Text.rectTransform.sizeDelta = new Vector2(m_Name_Text.preferredWidth, m_Name_Text.preferredHeight);
                    m_Level_Text.rectTransform.sizeDelta = new Vector2(m_Level_Text.preferredWidth, m_Level_Text.preferredHeight);

                    float parentSizeX = m_ClassIconImage.rectTransform.sizeDelta.x 
                        + m_Level_Text.rectTransform.anchoredPosition.x
                        + m_Level_Text.rectTransform.sizeDelta.x
                        + m_Name_Text.rectTransform.anchoredPosition.x
                        + m_Name_Text.rectTransform.sizeDelta.x;

                    m_rtrsParent.sizeDelta = new Vector2(parentSizeX, m_rtrsParent.sizeDelta.y);

                    RectTransform rtrsTrans = GetComponent<RectTransform>();

                    if (rtrsTrans != null)
                        m_rtrsParent.anchoredPosition = new Vector2((float)(rtrsTrans.anchoredPosition.x - (parentSizeX * 0.5)), m_rtrsParent.anchoredPosition.y);

                    if (m_Level_Text != null && m_LevelMaxEffect == null)
                        m_LevelMaxEffect = m_Level_Text.gameObject.GetComponent<TextLevelMaxEffect>();

                    if (m_LevelMaxEffect != null)
                        m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Card_Level_Limit);

                    if (m_LevelMaxEffect != null)
                        m_LevelMaxEffect.Value = cardInfo.m_byLevel;
                }
            }
        }
    }

    bool SetSkeletonAnimation(string identificationName, Vector3 localScale)
    {
        if (!string.IsNullOrEmpty(identificationName))
        {
            string assetPath = string.Format("Spines/Character/{0}/{0}_SkeletonData", identificationName);
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                m_SkeletonAnimation.skeletonDataAsset = skeletonDataAsset;
                m_SkeletonAnimation.initialSkinName = identificationName;

                //모션 이름 임시.
                Spine.AnimationStateData MotionData = m_SkeletonAnimation.skeletonDataAsset.GetAnimationStateData();
                if (MotionData.skeletonData.FindAnimation("pose") != null)
                    m_SkeletonAnimation.AnimationName = "pose";
                else
                    m_SkeletonAnimation.AnimationName = "wait";


                //                m_SkeletonAnimation.AnimationName = "wait";
                m_SkeletonAnimation.loop = true;
                m_SkeletonAnimation.Reset();
                m_SkeletonAnimation.transform.localScale = localScale;

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
        if (Kernel.uiManager != null)
        {
            //튜토리얼.
            if (Kernel.entry.tutorial.TutorialActive)
                return;

            // ref. PUC-866
            UICardInfo cardInfo = Kernel.uiManager.Open<UICardInfo>(UI.CardInfo);
            if (cardInfo != null)
            {
                cardInfo.cid = m_CID;
            }
        }
    }
}
