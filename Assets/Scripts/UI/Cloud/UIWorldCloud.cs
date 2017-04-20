using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIWorldCloud : MonoBehaviour
{
    public  Animation   CloudAnimation;
    public  Text        CloudLoadingText;

    //구름 이펙트 애니메이션 종료 뒤 콜백.
    public delegate void    LoadingPreEventCallback_Cloud();
    public LoadingPreEventCallback_Cloud LoadingPreEventEnd_Cloud;

    private bool CheckAnimationEnd = false;


    void Awake()
    {
        CloudAnimation = gameObject.GetComponent<Animation>();
        CloudLoadingText.gameObject.SetActive(false);
    }


    void Update()
    {
        if (CheckAnimationEnd)
        {
            if (CloudAnimation.isPlaying == false)
            {
                CheckAnimationEnd = false;
                LoadingPreEventEnd_Cloud();
            }
        }
    }


    public void CloseCloudEffect()
    {
        CloudAnimation.Stop();
        CloudAnimation.Play("AffiliaterCloud_Close");
        CheckAnimationEnd = true;
    }

    public void OpenCloudEffect()
    {
        HideCloudLoadingText();
        CloudAnimation.Stop();
        CloudAnimation.Play("AffiliaterCloud_Open");
        Invoke("HideCloudObject", 2.0f);
    }


    public void ShowCloudLoadingText()
    {
        if (Kernel.entry.battle.CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            CloudLoadingText.text = string.Empty;
            CloudLoadingText.gameObject.SetActive(false);
        }
        else
        {
            CloudLoadingText.text = Languages.ToString(TEXT_UI.MATCH_TRY);
            CloudLoadingText.gameObject.SetActive(true);
        }
    }


    public void HideCloudLoadingText()
    {
        CloudLoadingText.gameObject.SetActive(false);
    }


    private void HideCloudObject()
    {
        gameObject.SetActive(false);
    }
    



}
