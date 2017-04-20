using UnityEngine;
using System.Collections;
using TMPro;

public enum DAMAGE_COUNT_KIND
{
    DAMAGE_NORMAL = 0,
    DAMAGE_CRITICAL,
    HEAL_NORMAL,
    HEAL_CRITICAL,
    MISS,
    DOT_DAMAGE,
    DOT_HEAL,
    DAMAGE_REFLECT
}


[System.Serializable]
public class DamageColorData
{
    public Color    pUpColor;
    public Color    pDownColor;
}


public class BattleDamageCount : MonoBehaviour
{
    public  TextMeshPro pTextMeshPro;
    public  GameObject  pCriticalTextObj;

    [HideInInspector]
    public  bool        bActive;
    private bool        bPauseMode;
    private float       fCurAlphaTime;
    private float       fDelayAlphaTime = 0.5f;
    private float       fHideSpeed = 5.0f;
    private bool        bFadeStart;

    public DamageColorData      pColor_NormalDmg_H;
    public DamageColorData      pColor_NormalDmg_E;
    public DamageColorData      pColor_CriticalDmg;
    public DamageColorData      pColor_Heal;
    public DamageColorData      pColor_Miss;
    public DamageColorData      pColor_Dot_Dmg;
    public DamageColorData      pColor_Dot_Heal;

    private Color       pBaseColor;
    private Color       pBaseColor_Sub;
    private float       fCurAlpha;

    private Animation   pAnimation;

    private AnimationClip   pAniClip_Normal;
    private AnimationClip   pAniClip_Critical;
         

    public void Awake()
    {
        pAnimation = transform.GetComponent<Animation>();

        if(pCriticalTextObj != null)
            pCriticalTextObj.SetActive(false);

        HideDamageCount();

        pAniClip_Normal = pAnimation.GetClip("AniBattleCount_N");
        pAniClip_Critical = pAnimation.GetClip("AniBattleCount_C");
    }



    //데미지 카운트 표기.
    public void ShowDamageCount(Vector3 pPos, int nDmgCount, DAMAGE_COUNT_KIND eDamageType, float fAngleDir)
    {
        bActive = true;
        bPauseMode = false;

        bool bCritical = false;

        DamageColorData pTempColorData = null;
        switch (eDamageType)
        {
            case DAMAGE_COUNT_KIND.DAMAGE_NORMAL:
                if (fAngleDir < 0)
                    pTempColorData = pColor_NormalDmg_H;
                else
                    pTempColorData = pColor_NormalDmg_E;
                break;

            case DAMAGE_COUNT_KIND.DAMAGE_CRITICAL:
                pTempColorData = pColor_CriticalDmg;
                bCritical = true;
                break;

            case DAMAGE_COUNT_KIND.HEAL_NORMAL:
                pTempColorData = pColor_Heal;
                break;

            case DAMAGE_COUNT_KIND.HEAL_CRITICAL:
                pTempColorData = pColor_Heal;
                bCritical = true;
                break;

            case DAMAGE_COUNT_KIND.MISS:
                pTempColorData = pColor_Miss;
                break;

            case DAMAGE_COUNT_KIND.DOT_DAMAGE:
                pTempColorData = pColor_Dot_Dmg;
                break;

            case DAMAGE_COUNT_KIND.DOT_HEAL:
                pTempColorData = pColor_Dot_Heal;
                break;

            case DAMAGE_COUNT_KIND.DAMAGE_REFLECT:
                pTempColorData = pColor_Dot_Dmg;
                break;

            default:
                pTempColorData = pColor_Miss;
                break;
        }

        fCurAlpha = 1.0f;
        pTextMeshPro.text = Languages.GetNumberComma(nDmgCount);
        pTextMeshPro.color = new Color(1.0f, 1.0f, 1.0f, fCurAlpha);
        pTextMeshPro.colorGradient = new VertexGradient(pTempColorData.pUpColor, pTempColorData.pUpColor, pTempColorData.pDownColor, pTempColorData.pDownColor);

        bFadeStart = false;
        fCurAlphaTime = 0.0f;

        if (bCritical)
        {
            transform.localScale = Vector3.one * 1.5f;
            transform.position = pPos + (Vector3.up * 1.2f);
        }
        else
        {
            transform.localScale = Vector3.one * 1.0f;
            transform.position = pPos + (Vector3.up * 0.5f);
        }

        if (pCriticalTextObj != null)
        {
            if (bCritical)
                pCriticalTextObj.SetActive(true);
            else
                pCriticalTextObj.SetActive(false);
        }

        pAnimation.Stop();
        if (bCritical)
            pAnimation.clip = pAniClip_Critical;
        else
            pAnimation.clip = pAniClip_Normal;
        pAnimation.Play();


        pTextMeshPro.GetComponent<FadeFontProCS>().HideFont();
        pCriticalTextObj.GetComponent<FadeFontProCS>().HideFont();
    }



    //데미지 카운트 표기.
    public void ShowDamageCount_Text(Vector3 pPos, string szDmgText, DAMAGE_COUNT_KIND eDamageType, float fAngleDir)
    {
        bActive = true;
        bPauseMode = false;

        bool bCritical = false;

        DamageColorData pTempColorData = null;
        switch (eDamageType)
        {
            case DAMAGE_COUNT_KIND.DAMAGE_NORMAL:
                if (fAngleDir < 0)
                    pTempColorData = pColor_NormalDmg_H;
                else
                    pTempColorData = pColor_NormalDmg_E;
                break;

            case DAMAGE_COUNT_KIND.DAMAGE_CRITICAL:
                pTempColorData = pColor_CriticalDmg;
                bCritical = true;
                break;

            case DAMAGE_COUNT_KIND.HEAL_NORMAL:
                pTempColorData = pColor_Heal;
                break;

            case DAMAGE_COUNT_KIND.HEAL_CRITICAL:
                pTempColorData = pColor_Heal;
                bCritical = true;
                break;

            case DAMAGE_COUNT_KIND.MISS:
                pTempColorData = pColor_Miss;
                break;

            case DAMAGE_COUNT_KIND.DOT_DAMAGE:
                pTempColorData = pColor_Dot_Dmg;
                break;

            case DAMAGE_COUNT_KIND.DOT_HEAL:
                pTempColorData = pColor_Dot_Heal;
                break;

            case DAMAGE_COUNT_KIND.DAMAGE_REFLECT:
                pTempColorData = pColor_Dot_Dmg;
                break;

            default:
                pTempColorData = pColor_Miss;
                break;
        }

        fCurAlpha = 1.0f;
        pTextMeshPro.text = szDmgText;
        pTextMeshPro.color = new Color(1.0f, 1.0f, 1.0f, fCurAlpha);
        pTextMeshPro.colorGradient = new VertexGradient(pTempColorData.pUpColor, pTempColorData.pUpColor, pTempColorData.pDownColor, pTempColorData.pDownColor);

        bFadeStart = false;
        fCurAlphaTime = 0.0f;

        if (bCritical)
        {
            transform.localScale = Vector3.one * 1.5f;
            transform.position = pPos + (Vector3.up * 1.2f);
        }
        else
        {
            transform.localScale = Vector3.one * 1.0f;
            transform.position = pPos + (Vector3.up * 0.5f);
        }

        if (pCriticalTextObj != null)
        {
            if (bCritical)
                pCriticalTextObj.SetActive(true);
            else
                pCriticalTextObj.SetActive(false);
        }

        pAnimation.Stop();
        if (bCritical)
            pAnimation.clip = pAniClip_Critical;
        else
            pAnimation.clip = pAniClip_Normal;
        pAnimation.Play();


        pTextMeshPro.GetComponent<FadeFontProCS>().HideFont();
        pCriticalTextObj.GetComponent<FadeFontProCS>().HideFont();
    }


    public void HideDamageCount()
    {
        bActive = false;
        bPauseMode = false;
        transform.position = new Vector3(-1500.0f, 0.0f, 0.0f);
    }



    void Update()
    {
        if (bPauseMode)
            return;

        if (!bActive)
            return;

        if(bFadeStart)
        {
            fCurAlpha -= fHideSpeed * Time.deltaTime;
            if (fCurAlpha <= 0.0f)
            {
                fCurAlpha = 0.0f;
                HideDamageCount();
            }

            pTextMeshPro.color = new Color(pBaseColor.r, pBaseColor.g, pBaseColor.b, fCurAlpha);
        }
        else
        {
            fCurAlphaTime += Time.deltaTime;
            if (fCurAlphaTime >= fDelayAlphaTime)
            {
                fCurAlphaTime = 0.0f;
                bFadeStart = true;
            }
        }
    }



    public void SetPause()
    {
        pAnimation["AniBattleCount_N"].speed = 0.0f;
        pAnimation["AniBattleCount_C"].speed = 0.0f;
        bPauseMode = true;
    }

    public void SetResume()
    {
        pAnimation["AniBattleCount_N"].speed = 1.0f;
        pAnimation["AniBattleCount_C"].speed = 1.0f;
        bPauseMode = false;
    }




}
