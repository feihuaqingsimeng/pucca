using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeUguiText : MonoBehaviour
{
    private Text    BaseText;

    private bool    ActiveFadeText;
    private bool    WaitFadeStart;
    private float   CurWaitTime;
    private float   MaxWaitTime;
    private float   CurAlpha;
    private float   HideSpeed = 2.0f;


    void Awake()
    {
        BaseText = gameObject.GetComponent<Text>();
        ActiveFadeText = false;
        BaseText.color = new Color(BaseText.color.r, BaseText.color.g, BaseText.color.b, 0.0f);
    }



    public void SetFadeText(string szText, float HideTime)
    {
        BaseText.text = szText;

        ActiveFadeText = true;
        WaitFadeStart = true;
        CurAlpha = 1.0f;
        CurWaitTime = 0.0f;
        MaxWaitTime = HideTime;
        BaseText.color = new Color(BaseText.color.r, BaseText.color.g, BaseText.color.b, CurAlpha);

    }

	// Update is called once per frame
	void Update ()
    {
        if (!ActiveFadeText)
            return;

        if (WaitFadeStart)
        {
            CurWaitTime += Time.deltaTime;
            if (CurWaitTime >= MaxWaitTime)
            {
                WaitFadeStart = false;
                CurWaitTime = 0.0f;
            }
            return;
        }

        CurAlpha -= Time.deltaTime * HideSpeed;
        if (CurAlpha <= 0.0f)
        {
            CurAlpha = 0.0f;
            ActiveFadeText = false;
        }

        BaseText.color = new Color(BaseText.color.r, BaseText.color.g, BaseText.color.b, CurAlpha);
	}
}
