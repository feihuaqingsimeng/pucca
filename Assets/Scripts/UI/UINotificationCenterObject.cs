using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UINotificationCenterObject : MonoBehaviour
{
    public Image m_BackgroundImage;
    public Text m_Text;

    public int index
    {
        get;
        set;
    }

    public string text
    {
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            m_Text.text = value;
            UIUtility.FitSizeToContent(m_Text);
            m_BackgroundImage.rectTransform.sizeDelta = m_Text.rectTransform.sizeDelta + margin;
            active = true;
            StartCoroutine(LifeCycle());
        }
    }

    public float lifeTime
    {
        private get;
        set;
    }

    public float crossFadeDuration
    {
        private get;
        set;
    }

    public Vector2 margin
    {
        private get;
        set;
    }

    public bool active
    {
        get
        {
            return gameObject.activeSelf;
        }

        /*private*/
        set
        {
            if (gameObject.activeSelf != value)
            {
                gameObject.SetActive(value);

                if (!gameObject.activeSelf)
                {
                    m_DeltaTime = 0f;
                    index = -1;
                }

                if (onActiveChangeCallback != null)
                {
                    onActiveChangeCallback(this, gameObject.activeSelf);
                }
            }
        }
    }

    RectTransform m_RectTransform;

    public RectTransform rectTransform
    {
        get
        {
            return m_RectTransform ?? (m_RectTransform = transform as RectTransform);
        }
    }

    bool m_IgnoreTimeScale = true;
    float m_DeltaTime;

    public delegate void OnActiveChangeCallback(UINotificationCenterObject item, bool value);
    public OnActiveChangeCallback onActiveChangeCallback;

    // Use this for initialization

    // Update is called once per frame

    IEnumerator LifeCycle()
    {
        m_BackgroundImage.canvasRenderer.SetAlpha(1f);
        m_Text.canvasRenderer.SetAlpha(1f);

        while (lifeTime > m_DeltaTime)
        {
            m_DeltaTime = m_DeltaTime + Time.deltaTime;

            yield return 0;
        }

        m_BackgroundImage.CrossFadeAlpha(0f, crossFadeDuration, m_IgnoreTimeScale);
        m_Text.CrossFadeAlpha(0f, crossFadeDuration, m_IgnoreTimeScale);

        yield return new WaitForSeconds(m_IgnoreTimeScale ? crossFadeDuration * Time.timeScale : crossFadeDuration);

        active = false;

        yield break;
    }
}
