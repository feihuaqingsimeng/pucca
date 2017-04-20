using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UISecretBoxSelectItem : MonoBehaviour 
{
    [HideInInspector]
    public int          m_nSlotType;

    public Button       m_SelectButton;

    public Image        m_BoxFieldBack;
    public Image        m_BoxIcon;

    public Text         m_BoxName;

    private void Awake()
    {
        m_SelectButton.onClick.AddListener(OnClickSelect);
    }

    private void OnDestroy()
    {
        m_SelectButton.onClick.RemoveAllListeners();
    }

    //** 박스 선택 아이템 UI 및 데이터 세팅
    public void SetItem(SecretBoxData boxData)
    {
        m_nSlotType = boxData.m_nSlotType;

        m_BoxIcon.sprite = TextureManager.GetSprite(SpritePackingTag.Chest, boxData.m_strBoxIconName);
        m_BoxName.text = boxData.m_strBoxIconName;
    }

    //**  상자 선택시
    public void OnClickSelect()
    {
        UISecretBusiness secret = UIManager.Instance.Get<UISecretBusiness>(UI.SecretBusiness, true, false);

        if (secret != null)
            secret.SetBaseData(m_nSlotType);

        UIManager.Instance.Open<UISecretBusiness>(UI.SecretBusiness);
    }
}
