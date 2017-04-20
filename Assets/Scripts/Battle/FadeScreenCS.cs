using UnityEngine;
using System.Collections;

public class FadeScreenCS : MonoBehaviour
{
    private SpriteRenderer  pRenderer;

    private bool            bFadeScreenMode;
    public  float           fFadeDir = -1.0f;
    public  float           fFadeSpeed = 1.0f;
    private float           fCurAlphaValue = 0.0f;
    public  float           fMaxAlphaValue = 0.83f;


    private float           Color_R, Color_G, Color_B;

    void Awake()
    {
        pRenderer = gameObject.GetComponent<SpriteRenderer>();
        Color_R = pRenderer.color.r;
        Color_G = pRenderer.color.g;
        Color_B = pRenderer.color.b;
        pRenderer.color = new Color(Color_R, Color_G, Color_B, 0.0f);
    }


    public void StartFadeScreen()
    {
        bFadeScreenMode = true;
        fCurAlphaValue = 0.0f;
        fFadeDir = 1.0f;
    }

    public void ReleaseFadeScreen()
    {
        bFadeScreenMode = true;
        fCurAlphaValue = fMaxAlphaValue;
        fFadeDir = -1.0f;
    }


    public void UpdateFadeScreen()
    {
        if (!bFadeScreenMode)
            return;

        fCurAlphaValue += fFadeDir * fFadeSpeed *Time.deltaTime;
        if(fFadeDir > 0)
        {
            if (fCurAlphaValue >= fMaxAlphaValue)
            {
                fCurAlphaValue = fMaxAlphaValue;
                bFadeScreenMode = false;
            }
        }
        
        pRenderer.color = new Color(Color_R, Color_G, Color_B, fCurAlphaValue);
    }


    public void ResetFadeScreen()
    {
        bFadeScreenMode = false;
        fCurAlphaValue = 0.0f;
        pRenderer.color = new Color(Color_R, Color_G, Color_B, fCurAlphaValue);
    }



    // Update is called once per frame
    void Update ()
    {
        UpdateFadeScreen();
    }
}


