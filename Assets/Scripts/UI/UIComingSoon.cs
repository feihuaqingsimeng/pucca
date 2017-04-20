using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIComingSoon : MonoBehaviour, IPointerClickHandler
{
    Button m_Button;
    Toggle m_Toggle;

    void Awake()
    {
        StartCoroutine(Coroutine());
    }

    // Use this for initialization

    // Update is called once per frame

    IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(1f);

        m_Button = GetComponent<Button>();
        if (m_Button != null)
        {
            m_Button.onClick.RemoveAllListeners();
        }
        m_Toggle = GetComponent<Toggle>();
        if (m_Toggle != null)
        {
            m_Toggle.onValueChanged.RemoveAllListeners();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        UIAlerter.Alert(Languages.ToString(TEXT_UI.NOTICE_PREPARE), UIAlerter.Composition.Confirm);
    }
}
