using UnityEngine;
using System.Collections;
using TMPro;


public class FadeFontProCS : MonoBehaviour
{
    public TextMeshPro pTextPro;

    private bool bActive;

    private float fCurAlpha;
    private float fDir;
    public float fAlphaSpeed = 1.0f;

    private bool bWaitDelay;
    private float fCurDelay = 0.0f;
    public float fAlphaDelay = 0.1f;


    void Start()
    {
        HideFont();
    }

    public void ShowFont()
    {
        StartFontAlpha(1.0f);
        fCurDelay = 0.0f;
        bActive = true;
        bWaitDelay = true;
        pTextPro.color = new Color(pTextPro.color.r, pTextPro.color.g, pTextPro.color.b, 0.0f);
    }


    public void HideFont()
    {
        StartFontAlpha(-1.0f);
        fCurDelay = 0.0f;
        bActive = true;
        bWaitDelay = true;
        pTextPro.color = new Color(pTextPro.color.r, pTextPro.color.g, pTextPro.color.b, 1.0f);
    }


    public void StartFontAlpha(float fModeDir)
    {
        fDir = fModeDir;
        if (fDir < 0)
            fCurAlpha = 1.0f;
        else
            fCurAlpha = 0.0f;
    }



    // Update is called once per frame
    void Update()
    {
        if (!bActive)
            return;

        if (bWaitDelay)
        {
            fCurDelay += Time.deltaTime;
            if (fCurDelay >= fAlphaDelay)
            {
                fCurDelay = 0.0f;
                bWaitDelay = false;
            }
            pTextPro.color = new Color(pTextPro.color.r, pTextPro.color.g, pTextPro.color.b, fCurAlpha);
            return;
        }


        fCurAlpha += (fAlphaSpeed * fDir) * Time.deltaTime;
        if (fDir < 0)
        {
            if (fCurAlpha <= 0.0f)
            {
                fCurAlpha = 0.0f;
                bActive = false;
            }
        }
        else if (fCurAlpha >= 1.0f)
        {
            fCurAlpha = 1.0f;
            bActive = false;
        }

        pTextPro.color = new Color(pTextPro.color.r, pTextPro.color.g, pTextPro.color.b, fCurAlpha);
    }
}