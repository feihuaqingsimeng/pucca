using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIStarSlot : MonoBehaviour
{
    public GameObject   EffectPrefab_GetStar;
    private GameObject  LoadEffectObject;
    public GameObject   StarAnimationObj;

    public void InitStarSlot(float ShowDelay)
    {
        LoadEffectObject = Instantiate(EffectPrefab_GetStar) as GameObject;
        LoadEffectObject.transform.SetParent(transform);
        LoadEffectObject.transform.localPosition = Vector3.zero;
        LoadEffectObject.transform.localScale = Vector3.one;
        LoadEffectObject.SetActive(false);

        StarAnimationObj.SetActive(false);

        Invoke("ShowStarSlot", ShowDelay);
    }

    public void HideStartAnimation()
    {
        StarAnimationObj.SetActive(false);
    }


    public void ShowStarSlot()
    {
        if (LoadEffectObject == null)
            return;

        StarAnimationObj.SetActive(true);
        LoadEffectObject.SetActive(true);
        Destroy(LoadEffectObject, 1.5f);
        Kernel.soundManager.PlayUISound(SOUND.SND_UI_PVP_RESULT_STARPOINT);

    }

}
