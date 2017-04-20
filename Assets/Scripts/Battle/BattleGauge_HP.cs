using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleGauge_HP : MonoBehaviour
{
    private RectTransform   pTransform;
    private CanvasGroup     pCanvasGroup;
    
    public  bool            NotDestroyGauge;
    public  bool            WidthMode;
    public  float           BaseWidth;

    public  Image           pGauge_R;
    private RectTransform   pTransform_Gauge_R;
    public  Image           pGauge_G;
    private RectTransform   pTransform_Gauge_G;

    private float           fPreValue;
    private float           fHideTimer;

    private bool            ReturnColorMode;
    private float           fReturnColorTime;



    void Awake()
    {
        pCanvasGroup = gameObject.GetComponent<CanvasGroup>();
        pTransform = gameObject.GetComponent<RectTransform>();
        HideBattleGauge_HP();

        if (WidthMode)
        {
            pTransform_Gauge_R = pGauge_R.GetComponent<RectTransform>();
            pTransform_Gauge_R.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BaseWidth);
            pTransform_Gauge_G = pGauge_G.GetComponent<RectTransform>();
            pTransform_Gauge_G.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BaseWidth);
        }
        else
        {
            pGauge_R.fillAmount = 1.0f;
            pGauge_G.fillAmount = 1.0f;
        }
        fPreValue = 1.0f;
        fHideTimer = 0.0f;
        ReturnColorMode = true;
        fReturnColorTime = 1.0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (ReturnColorMode)
        {
            fReturnColorTime += Time.deltaTime;
            if (fReturnColorTime >= 1.0f)
            {
                fReturnColorTime = 0.0f;
                ReturnColorMode = false;
            }
        }
	}



    public void UpdateBattleGauge_HP(Vector3 pPos, float fValue)
    {
        if (pTransform == null)
        {
            pTransform = gameObject.GetComponent<RectTransform>();
        }

        if (fPreValue != fValue)
        {
            if (!NotDestroyGauge)
            {
                ShowBattleGauge_HP();
                fHideTimer = 0.0f;
            }

            if(fPreValue < fValue)
            {
                pGauge_R.gameObject.SetActive(false);
                pGauge_G.gameObject.SetActive(true);

                ReturnColorMode = true;
                fReturnColorTime = 0.0f;
            }
            else
            {
                pGauge_R.gameObject.SetActive(true);
                pGauge_G.gameObject.SetActive(false);
            }
        }


        fPreValue = fValue;

        if(!NotDestroyGauge)
            pTransform.position = pPos;


        if (WidthMode)
        {
            float CalWidth = BaseWidth * fPreValue;
            if (CalWidth <= 0.0f)
                CalWidth = 0.0f;
            if (CalWidth >= BaseWidth)
                CalWidth = BaseWidth;
            pTransform_Gauge_R.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalWidth);
            pTransform_Gauge_G.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalWidth);
        }
        else
        {
            pGauge_R.fillAmount = fPreValue;
            pGauge_G.fillAmount = fPreValue;
        }


        //타이머.
        if (!NotDestroyGauge)
        {
            fHideTimer += Time.deltaTime;
            if (fHideTimer >= 2.0f)
            {
                HideBattleGauge_HP();
            }
        }

        if (ReturnColorMode)
        {
            fReturnColorTime += Time.deltaTime;
            if (fReturnColorTime >= 1.0f)
            {
                fReturnColorTime = 0.0f;
                ReturnColorMode = false;
                pGauge_R.gameObject.SetActive(true);
                pGauge_G.gameObject.SetActive(false);
            }
        }

    }

    public void ShowBattleGauge_HP()
    {
        if (NotDestroyGauge)
            return;

        if (pCanvasGroup == null)
            return;

        pCanvasGroup.alpha = 1.0f;
    }

    public void HideBattleGauge_HP()
    {
        if (NotDestroyGauge)
            return;

        if (pCanvasGroup == null)
            return;

        pCanvasGroup.alpha = 0.0f;
    }

}
