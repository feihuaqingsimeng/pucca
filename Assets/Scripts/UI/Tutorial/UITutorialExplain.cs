using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UITutorialExplain : MonoBehaviour
{
    public Text     PopupTitle;
    public Text     PopupExplain;

    public GameObject       IconImage;
    private Animator        WindowAnimator;

    void Awake()
    {
        if(IconImage != null)
            IconImage.SetActive(false);

        WindowAnimator = gameObject.GetComponent<Animator>();
    }

    public void SetPopupText(string szTitle, string szExplain)
    {
        PopupTitle.text = szTitle;
        PopupExplain.text = szExplain;

        if (IconImage != null)
            Invoke("ShowIconImage", 0.1f);

        if(WindowAnimator != null)
            WindowAnimator.SetTrigger("Popup_open_ani");
    }

    public void ShowIconImage()
    {
        if(IconImage != null)
            IconImage.SetActive(true);
    }


}
