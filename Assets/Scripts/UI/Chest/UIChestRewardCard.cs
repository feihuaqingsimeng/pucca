using Common.Packet;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIChestRewardCard : MonoBehaviour
{
    public Animator m_Animator;
    public AnimationEventHandler m_AnimationEventHandler;
    public Button m_Button;
    public UICharCard m_CharCard;
    public UIGoodsCard m_GoodsCard;
    public Text m_CountText;
    public UISlider m_CardCountSlider;
    public GameObject m_GoldCountGameObject;
    public float m_CardRotateSpeed;
    public GameObject m_FX;
    public GameObject m_LegendCardFX;

    bool m_IsCharCard;

    #region RectTransform Property
    RectTransform m_RectTransform;

    RectTransform rectTransform
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

    public RuntimeAnimatorController runtimeAnimatorController
    {
        set
        {
            if (m_Animator != null)
            {
                m_Animator.runtimeAnimatorController = value;
            }
        }
    }

    public bool rotated
    {
        get;
        private set;
    }

    public delegate void OnRotateCallback();
    public OnRotateCallback onRotateCallback;

    void Awake()
    {
        m_AnimationEventHandler.onAnimationEventCallback += OnAnimationEvent;
        m_Button.onClick.AddListener(OnClick);
    }

    // Use this for initialization

    // Update is called once per frame

    void OnDisable()
    {
        rotated = false;
    }

    void OnAnimationEvent(string value)
    {
        if (string.Equals("Card_Get_ani", value, System.StringComparison.OrdinalIgnoreCase))
        {
            if (m_CharCard != null
                && m_CharCard.gradeType == Grade_Type.Grade_S || m_CharCard.gradeType == Grade_Type.Grade_SS)
            {
                m_LegendCardFX.gameObject.SetActive(true);
            }
        }
    }

    public int gold
    {
        set
        {
            m_GoodsCard.SetGoods(Goods_Type.Gold, value);
            m_IsCharCard = false;
            //m_CountSlider.m_bIsUse = false;
        }
    }

    public CBoxResult boxResult
    {
        set
        {
            m_CharCard.boxResult = value;
            m_IsCharCard = true;
            m_CardCountSlider.gameObject.SetActive(false); // 임시
            m_LegendCardFX.gameObject.SetActive(false); // 임시
        }
    }

    IEnumerator Rotate()
    {
        rotated = true;

        Kernel.soundManager.PlayUISound(SOUND.SND_UI_CARD_ACT_01);

        Vector3 localEulerAngles = Vector3.zero;
        float deltaTime = 0f;
        while (localEulerAngles.y < 90f)
        {
            localEulerAngles.y = Mathf.Clamp(localEulerAngles.y + deltaTime, 0f, 90f);
            m_Button.transform.localEulerAngles = localEulerAngles;
            deltaTime = deltaTime + (Time.deltaTime * m_CardRotateSpeed);

            yield return 0;
        }

        Transform transform = m_IsCharCard ? m_CharCard.transform : m_GoodsCard.transform;
        localEulerAngles = new Vector3(0f, 90f, 0f);
        while (localEulerAngles.y > 0f)
        {
            if (localEulerAngles.y > 45f && !m_FX.gameObject.activeSelf)
            {
                m_FX.gameObject.SetActive(true);
            }

            localEulerAngles.y = Mathf.Clamp(localEulerAngles.y - deltaTime, 0f, 90f);
            transform.localEulerAngles = localEulerAngles;
            deltaTime = deltaTime + (Time.deltaTime * m_CardRotateSpeed);

            yield return 0;
        }

        m_CountText.gameObject.SetActive(true);
        m_CardCountSlider.gameObject.SetActive(m_IsCharCard);
        m_GoldCountGameObject.SetActive(!m_IsCharCard);

        if (onRotateCallback != null)
        {
            onRotateCallback();
        }

        yield break;
    }

    public void OnClick()
    {
        if (!gameObject.activeSelf)
        {
            return;
        }

        m_Button.interactable = false;
        StartCoroutine(Rotate());
    }

    public void SetTrigger(float x)
    {
        m_Animator.ResetTrigger("Normal");
        m_Animator.ResetTrigger("Card_Get_ani");
        m_Animator.SetTrigger("Card_Get_ani");
    }
}
