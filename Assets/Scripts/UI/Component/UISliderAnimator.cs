using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class SliderColor
{
    private static float m_fgagebar_chage_color_speed = 0.5f;
    private static Color m_CurColor = Color.white;

    public static Color ChangeColor()
    {
        m_CurColor = Color.Lerp(Color.white, Color.gray, Mathf.PingPong(Time.time, m_fgagebar_chage_color_speed));

        return m_CurColor;
    }
}

[RequireComponent(typeof(Slider))]
public class UISliderAnimator : MonoBehaviour
{
    [SerializeField]
    private Slider m_Slider;
    public Slider slider
    {
        get
        {
            return m_Slider;
        }
    }
    [SerializeField]
    private GameObject m_MaximumFX;
    [SerializeField]
    private Sprite m_NormalSprite;
    [SerializeField]
    private Sprite m_MaximumSprite;
    [SerializeField]
    private Sprite m_MaximumFXSprite;
    [SerializeField]
    private Image m_FillImage;
    private Image fillRectImage
    {
        get
        {
            if (m_FillImage == null && m_Slider.fillRect != null)
            {
                m_FillImage = m_Slider.fillRect.GetComponent<Image>();
            }

            return m_FillImage;
        }
    }

    [SerializeField]
    private bool m_bIsUse = true;
    public bool isUse
    {
        get
        {
            return m_bIsUse;
        }

        set
        {
            if (m_bIsUse != value)
            {
                m_bIsUse = value;
                OnValueChanged(m_Slider.value);
            }
        }
    }

#if UNITY_EDITOR
    void Reset()
    {
        m_Slider = GetComponent<Slider>();

        if (m_Slider == null)
            m_Slider = GetComponent<UISlider>();

        string strTextureBasePath = "Assets/Textures/UI/Common/";

        if (fillRectImage.mainTexture.name == "ui_gauge_big_bar_01")
        {
            m_NormalSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_gauge_big_bar_01.png");
            m_MaximumSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_gauge_big_bar_02_light.png");
            m_MaximumFXSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_gauge_big_effect.png");
        }
        else
        {
            m_NormalSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_gauge_bar_01.png");
            m_MaximumSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_gauge_bar_02_light.png");
            m_MaximumFXSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(strTextureBasePath + "ui_gauge_effect.png");
        }

        Image[] children = m_Slider.gameObject.GetComponentsInChildren<Image>();

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].mainTexture.name == m_MaximumFXSprite.texture.name)
            {
                m_MaximumFX = children[i].gameObject;
                break;
            }
        }
    }
#endif

    void Awake()
    {
        if (m_Slider == null)
            m_Slider = GetComponent<Slider>();

        if (m_Slider != null)
            m_Slider.onValueChanged.AddListener(OnValueChanged);
    }

    void Start()
    {
        // Deck 화면에서 화면이 뜨기도 전에 Active를 끌려고 하다보니 꺼지지가 않는 문제 때문에..
        SetInit();
    }

    void OnEnable()
    {
        // Deck Card Info에서 같은 캐릭터 화면을 끄고 켰을때 코루틴이 이상해지는 문제 때문에..
        SetInit();
    }

    private void SetInit()
    {
        fillRectImage.canvasRenderer.SetColor(Color.white);
        fillRectImage.sprite = m_NormalSprite;

        m_MaximumFX.SetActive(false);

        OnValueChanged(m_Slider.value);
    }

    private void OnValueChanged(float value)
    {
        if (!m_Slider.gameObject.activeInHierarchy)
            return;

        SetMaximumFXActive(m_bIsUse && m_Slider.normalizedValue >= 1f);

        if (m_bIsUse && m_Slider.normalizedValue >= 1f)
            Complet();
    }

    private void SetMaximumFXActive(bool active)
    {
        if (m_MaximumFX != null && m_MaximumFX.activeSelf != active)
            m_MaximumFX.SetActive(active);
    }

    private void Complet()
    {
        if (m_NormalSprite == null || m_MaximumSprite == null)
            return;

        fillRectImage.sprite = m_MaximumSprite;

        StartCoroutine(CompletAnimation());
    }

    private IEnumerator CompletAnimation()
    {
        while (m_Slider.normalizedValue >= 1f)
        {
            fillRectImage.canvasRenderer.SetColor(SliderColor.ChangeColor());
            yield return null;
        }

        SetInit();
    }
}
