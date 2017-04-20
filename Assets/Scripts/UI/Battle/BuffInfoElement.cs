using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BuffInfoElement : MonoBehaviour
{
    [HideInInspector]
    public bool         ActiveBuffInfo;
    [HideInInspector]
    public BUFF_KIND    BuffInfoKind;
    private bool        WaitCloseAnimationMode;

    public Animation    BuffAnimation;

    public Image        BuffFrame;
    public Image        BuffIcon;
    public Text         BuffTimeCount;


    private bool        ShineMode;
    private float       CurAlpha;
    public  float       ShineSpeed = 1.0f;
    private float       ShineDir;


    private int         BuffEffectType = 0; //0 : 좋음, 1 : 나쁨, 2 : 힐.


    public  Color       BuffColor_Good;
    public  Color       BuffColor_Bad;
    public  Color       BuffColor_Support;


    //버프정보 실행.
    public void InitBuffInfoElement(BUFF_KIND BuffKind)
    {
        //테이블화 시켜줄것.
        int BuffIconNumber = 0;
        string BuffFrameName = "";

        BuffInfoKind = BuffKind;

        switch (BuffInfoKind)
        {
            case BUFF_KIND.SLOW:
                BuffIconNumber = 0;
                break;

            case BUFF_KIND.CRITICALRATE_UP:
            case BUFF_KIND.CRITICALRATE_DOWN:
                BuffIconNumber = 1;
                break;

            case BUFF_KIND.ATT_UP:
            case BUFF_KIND.ATT_DOWN:
                BuffIconNumber = 2;
                break;

            case BUFF_KIND.DEF_UP:
            case BUFF_KIND.DEF_DOWN:
                BuffIconNumber = 3;
                break;

            case BUFF_KIND.EVADERATE_UP:
            case BUFF_KIND.EVADERATE_DOWN:
                BuffIconNumber = 4;
                break;

            case BUFF_KIND.ACCURATE_UP:
            case BUFF_KIND.ACCURATE_DOWN:
                BuffIconNumber = 5;
                break;

            case BUFF_KIND.CRITICALDMG_UP:
            case BUFF_KIND.CRITICALDMG_DOWN:
                BuffIconNumber = 6;
                break;

            case BUFF_KIND.FAST:
                BuffIconNumber = 7;
                break;

            case BUFF_KIND.DOT_DAMAGE:
            case BUFF_KIND.SACRIFICE:
                BuffIconNumber = 8;
                break;

            case BUFF_KIND.POISON:
                BuffIconNumber = 9;
                break;

            case BUFF_KIND.FREEZE:
                BuffIconNumber = 10;
                break;

            case BUFF_KIND.SILENCE:
                BuffIconNumber = 11;
                break;

            case BUFF_KIND.HEALING_CONSIST:
            case BUFF_KIND.TOTEM_HEALING_CONSIST:
                BuffIconNumber = 12;
                break;

            case BUFF_KIND.HARD_TANKING:
                BuffIconNumber = 13;
                break;

            case BUFF_KIND.SKILL_SHIELD:
                BuffIconNumber = 14;
                break;
        }

        switch (BuffInfoKind)
        {
            case BUFF_KIND.FREEZE:
            case BUFF_KIND.DOT_DAMAGE:
            case BUFF_KIND.SACRIFICE:
            case BUFF_KIND.POISON:
            case BUFF_KIND.ATT_DOWN:
            case BUFF_KIND.DEF_DOWN:
            case BUFF_KIND.SLOW:
            case BUFF_KIND.ACCURATE_DOWN:
            case BUFF_KIND.EVADERATE_DOWN:
            case BUFF_KIND.CRITICALRATE_DOWN:
            case BUFF_KIND.CRITICALDMG_DOWN:
            case BUFF_KIND.SILENCE:
                BuffEffectType = 1;
                BuffFrameName = "ui_frame_debuff";
                break;

            case BUFF_KIND.HEALING_CONSIST:
            case BUFF_KIND.TOTEM_HEALING_CONSIST:
                BuffEffectType = 2;
                BuffFrameName = "ui_frame_heal";
                break;

            default:
                BuffEffectType = 0;
                BuffFrameName = "ui_frame_buff";
                break;
        }



        BuffFrame.sprite = TextureManager.GetSprite(SpritePackingTag.BuffIcon, BuffFrameName);
        BuffIcon.sprite = TextureManager.GetSprite(SpritePackingTag.BuffIcon, "ui_BuffIcon_" + BuffIconNumber.ToString());

        gameObject.SetActive(false);

    }



    public void ActiveBuffInfoElement()
    {
        ActiveBuffInfo = true;
        WaitCloseAnimationMode = false;

        switch (BuffEffectType)
        {
            case 0:     BuffIcon.color = BuffColor_Good;        break;
            case 1:     BuffIcon.color = BuffColor_Bad;         break;
            case 2:     BuffIcon.color = BuffColor_Support;     break;
        }
        CurAlpha = 1.0f;

        BuffAnimation.Play("AniBuffInfo_Open");
    }


    //버프정보 해제.
    public void ReleaseBuffInfoElement()
    {
        ActiveBuffInfo = false;
        BuffAnimation.Play("AniBuffInfo_Close");
        WaitCloseAnimationMode = true;
    }

	
	// Update is called once per frame
	void Update () 
    {
        if (WaitCloseAnimationMode)
        {
            if (BuffAnimation.IsPlaying("AniBuffInfo_Close") == false)
            {
                WaitCloseAnimationMode = false;
                gameObject.SetActive(false);
            }
        }
    }


    public void UpdateBuffInfo(float TimeSec)
    {
        if (!ActiveBuffInfo)
            return;

        if (TimeSec <= 0.0f)
        {
            ReleaseBuffInfoElement();
            return;
        }


        if (ShineMode)
        {
            if (ShineDir < 0)   //흐려지게.
            {
                CurAlpha -= ShineSpeed * Time.deltaTime;
                if (CurAlpha <= 0.4f)
                {
                    CurAlpha = 0.4f;
                    ShineDir = 1.0f;
                }
            }
            else
            {
                CurAlpha += ShineSpeed * Time.deltaTime;
                if (CurAlpha >= 1.0f)
                {
                    CurAlpha = 1.0f;
                    ShineDir = -1.0f;
                }
            }


            Color TempColor;
            switch (BuffEffectType)
            {
                case 1:
                    TempColor = BuffColor_Bad;
                    break;
                
                case 2:
                    TempColor = BuffColor_Support;
                    break;

                default:
                    TempColor = BuffColor_Good;
                    break;
            }
            TempColor.a = CurAlpha;
            BuffIcon.color = TempColor;

            if (TimeSec >= 3.0f)
                EndShineMode();
        }
        else
        {
            if (TimeSec < 3.0f)
                SetShineMode();
        }


        //시간.
        int leftTime_S = (int)(TimeSec / 1.0f);
        int leftTime_MS = (int)((TimeSec % 1.0f) * 10.0f);

        BuffTimeCount.text = string.Format("{0}.{1}s", leftTime_S, leftTime_MS);
	}


    //반짝거리기 시작.
    public void SetShineMode()
    {
        ShineMode = true;
        CurAlpha = 1.0f;
        ShineDir = -1.0f;
    }


    //반짝거리기 종료.
    public void EndShineMode()
    {
        ShineMode = false;
        CurAlpha = 1.0f;
        ShineDir = -1.0f;
    }

}
