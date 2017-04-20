using UnityEngine;
using UnityEngine.UI;

public class UICardEquipObject : MonoBehaviour
{
    private TextLevelMaxEffect m_LevelMaxEffect;
    public Animator m_Animator;
    public Toggle m_Toggle;
    public Image m_IconImage;
    public Text m_NameText;
    public Text m_LevelText;
    public Text m_StatText;
    public Text m_AddText;
    public Image m_UpgradableImage;
    public Image m_UpgradableScrollImage;
    public Goods_Type m_Equipment;
    public RuntimeAnimatorController m_ToggleRuntimeAnimatorController;
    public RuntimeAnimatorController m_LevelupRuntimeAnimatorController;

    [HideInInspector]
    public byte m_Level;

    public delegate void OnToggleValueChangeCallback(Goods_Type goodsType, byte level);
    public OnToggleValueChangeCallback onToggleValueChangeCallback;

    public delegate void OnAnimationEventCallback(string value);
    public OnAnimationEventCallback onAnimationEventCallback;

    void Awake()
    {
        m_Toggle.onValueChanged.AddListener(OnValueChange);
    }

    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        if (m_UpgradableImage != null && m_UpgradableScrollImage != null && m_UpgradableImage.gameObject.activeSelf)
        {
            float value = m_UpgradableScrollImage.rectTransform.anchoredPosition.y + 1f;
            if (value > 45f) value = 0f;

            m_UpgradableScrollImage.rectTransform.anchoredPosition = new Vector2(0f, value);
        }
    }

    void OnEnable()
    {
        if (m_Animator.runtimeAnimatorController != m_ToggleRuntimeAnimatorController)
        {
            m_Animator.runtimeAnimatorController = m_ToggleRuntimeAnimatorController;
        }
    }

    // AnimationEventHandler 컴포넌트를 사용하지 않고, 애니메이션으로부터 직접 호출됩니다.
    public void OnAnimationEvent(string value)
    {
        if (!string.Equals("FX", value))
        {
            m_Animator.runtimeAnimatorController = m_ToggleRuntimeAnimatorController;
        }

        if (onAnimationEventCallback != null)
        {
            onAnimationEventCallback(value);
        }
    }

    public void Direction()
    {
        if (m_Animator.runtimeAnimatorController != m_LevelupRuntimeAnimatorController)
        {
            m_Animator.runtimeAnimatorController = m_LevelupRuntimeAnimatorController;
        }

        m_Animator.SetTrigger("Item_Frame_Levelup_Ani");
    }

    public void SetValue(int cardIndex, byte level, int stat, int add)
    {
        m_Level = level;
        m_IconImage.sprite = TextureManager.GetEquipSprite(m_Equipment, level);
        m_NameText.text = GetEquipName(m_Equipment, level);
        m_LevelText.text = string.Format("{0}{1}", Languages.ToString(TEXT_UI.LV), level);
        m_StatText.text = Languages.ToString<int>(stat);

        bool levelUpAvailable = Kernel.entry.character.EquipmentLevelUpAvailable(cardIndex, m_Equipment);
        m_AddText.text = levelUpAvailable ? Languages.ToString<int>(add) : string.Empty;
        m_UpgradableImage.gameObject.SetActive(levelUpAvailable);

        if (m_LevelText != null && m_LevelMaxEffect == null)
            m_LevelMaxEffect = m_LevelText.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Equipment_Level_Limit);

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.Value = level;
    }

    void OnValueChange(bool value)
    {
        if (value && onToggleValueChangeCallback != null)
        {
            onToggleValueChangeCallback(m_Equipment, m_Level);
        }
    }

    string GetEquipName(Goods_Type goodsType, byte level)
    {
        string equipName = string.Empty;
        switch (goodsType)
        {
            case Goods_Type.EquipUpAccessory:
                if (level <= 10)
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_ACC_0);
                }
                else if (level > 10 && level <= 20)
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_ACC_1);
                }
                else
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_ACC_2);
                }
                break;
            case Goods_Type.EquipUpArmor:
                if (level <= 10)
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_ARMOR_0);
                }
                else if (level > 10 && level <= 20)
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_ARMOR_1);
                }
                else
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_ARMOR_2);
                }
                break;
            case Goods_Type.EquipUpWeapon:
                if (level <= 10)
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_WEAPON_0);
                }
                else if (level > 10 && level <= 20)
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_WEAPON_1);
                }
                else
                {
                    equipName = Languages.ToString(TEXT_UI.EQUIPITEMNAME_WEAPON_2);
                }
                break;
        }

        return equipName;
    }
}
