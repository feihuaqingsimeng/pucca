using UnityEngine;
using UnityEngine.UI;

public class UIRankingObject : MonoBehaviour
{
    private TextLevelMaxEffect m_LevelMaxEffect;

    public Button m_Button;
    public Image m_RankingImage;
    public Text m_RankingText;
    public Text m_LevelText;
    public Text m_NameText;
    public Text m_RankingPointText;

    #region RectTransform
    RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = transform as RectTransform;
            }

            return m_RectTransform;
        }
    }
    #endregion

    protected virtual void Awake()
    {
        m_Button.onClick.AddListener(OnClick);

        if (m_LevelText != null && m_LevelMaxEffect == null)
            m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();
    }

    // Use this for initialization

    // Update is called once per frame

    protected int rank
    {
        set
        {
            bool isHighRank = (value < 4);
            if (isHighRank)
            {
                m_RankingImage.sprite = TextureManager.GetSprite(SpritePackingTag.Guild, string.Format("{0}st_Class", value));
            }
            else
            {
                m_RankingText.text = value.ToString();
            }

            m_RankingImage.gameObject.SetActive(isHighRank);
            m_RankingText.gameObject.SetActive(!isHighRank);
        }
    }

    protected new string name
    {
        set
        {
            m_NameText.text = gameObject.name = value;
        }
    }

    protected int rankingPoint
    {
        set
        {
            m_RankingPointText.text = Languages.ToString(value);
        }
    }

    protected byte level
    {
        set
        {
            m_LevelText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), value);

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.Value = value;
        }
    }

    //** 길드와 유저 랭킹 오브젝트의 max 레벨이 다르므로 세팅해줘야함.
    private byte m_bytMaxLevel;
    protected byte maxLevel
    {
        get { return m_bytMaxLevel; }
        set 
        { 
            m_bytMaxLevel = value;

            if (m_LevelMaxEffect != null)
                m_LevelMaxEffect.MaxValue = value;
        }
    }

    protected virtual void OnClick()
    {

    }
}
