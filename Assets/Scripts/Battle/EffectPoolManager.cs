using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectPoolManager : MonoBehaviour
{
    //이펙트 리스트.
    [HideInInspector]
    public  List<EffectPoolElement> pEffectList = new List<EffectPoolElement>();
    private int AddEffectCount;

    //데미지카운트 리스트.
    [HideInInspector]
    public  List<BattleDamageCount> pDamageCountList = new List<BattleDamageCount>();

    //데미지카운트 프리팹.
    private GameObject pPrefab_DamageCount;



    ////////////////////////////////////////////
    //  사용 데이터.
    ////////////////////////////////////////////
    //이펙트 풀링 카운트.
    private int     nEffectAddCount;

    //일시정지상태.
    private bool    bPauseMode;




    
    ////////////////////////////////////////////
    // 
    ////////////////////////////////////////////
    public void InitEffectPoolManager()
    {
        nEffectAddCount = 0;
        bPauseMode = false;

        //이펙트풀 초기화.
        ClearEffectPool();

        //데미지카운트 리스트 초기화.
        ClearBattleDamageCount();
        pPrefab_DamageCount = Resources.Load("Prefabs/Battle/DamageCount") as GameObject;
        for (int idx = 0; idx < 20; idx++)
        {
            GameObject pTempDmgCountObj = Instantiate(pPrefab_DamageCount) as GameObject;
            pTempDmgCountObj.name = "DamageCount";
            BattleDamageCount pDmgCountInfo = pTempDmgCountObj.GetComponent<BattleDamageCount>();
            pTempDmgCountObj.transform.SetParent(transform);
            pDmgCountInfo.HideDamageCount();
            pDamageCountList.Add(pDmgCountInfo);
        }

    }



    
    
    
    
    
    
    
	void Update () 
    {
        for (int idx = 0; idx < pEffectList.Count; idx++)
        {
            if (pEffectList[idx].bActive == false)
                continue;

            if (pEffectList[idx].pEffectObj == null)
                continue;

            ParticleSystem pParticle = pEffectList[idx].pEffectObj.GetComponent<ParticleSystem>();
            if (pParticle == null)
            {
                pParticle = pEffectList[idx].pEffectObj.GetComponentInChildren<ParticleSystem>();
            }

            if (pEffectList[idx].bForceEnd)  //강제종료면.
            {
                SetDisenableEffect(pEffectList[idx]);
                continue;
            }

            if (pEffectList[idx].bForceLoopEffect)
            {
                if(pParticle != null)
                    pParticle.loop = true;
                continue;
            }


            if (pEffectList[idx].fMaxLifeTime > 0.0f)
            {
                if (!bPauseMode)
                    pEffectList[idx].fCurLifeTime += Time.deltaTime;

                if (pEffectList[idx].fCurLifeTime >= pEffectList[idx].fMaxLifeTime)
                {
                    SetDisenableEffect(pEffectList[idx]);
                }
            }
        }
	}





    //일시정지.
    public void SetPause()
    {
        bPauseMode = true;

        //파티클 이펙트.
        for (int idx = 0; idx < pEffectList.Count; idx++)
        {
            if (pEffectList[idx].bNotPauseEffect || !pEffectList[idx].bActive)
                continue;

            ParticleSystem pParticle = pEffectList[idx].pEffectObj.GetComponent<ParticleSystem>();
            if (pParticle == null)
            {
                pParticle = pEffectList[idx].pEffectObj.GetComponentInChildren<ParticleSystem>();
                if (pParticle == null)
                    continue;
            }

            pParticle.Pause(true);
        }


        //데미지카운트.
        for (int idx = 0; idx < pDamageCountList.Count; idx++)
        {
            pDamageCountList[idx].SetPause();
        }
    }


    //재생.
    public void SetResume()
    {
        bPauseMode = false;

        //파티클 이펙트.
        for (int idx = 0; idx < pEffectList.Count; idx++)
        {
            if (pEffectList[idx].bNotPauseEffect || !pEffectList[idx].bActive)
                continue;

            ParticleSystem pParticle = pEffectList[idx].pEffectObj.GetComponent<ParticleSystem>();
            if (pParticle == null)
            {
                pParticle = pEffectList[idx].pEffectObj.GetComponentInChildren<ParticleSystem>();
                if (pParticle == null)
                    continue;
            }

            pParticle.Play(true);
        }

        //데미지카운트.
        for (int idx = 0; idx < pDamageCountList.Count; idx++)
        {
            pDamageCountList[idx].SetResume();
        }
    }

























    //이펙트 활성화.
    public void SetBattleEffect(Vector3 pTargetPos, int nEffectID, float EffectDir = 1.0f, float fHideTime = 1.5f)
    {
        EffectPoolElement pTempEffectElement = GetFreeEffectPool(nEffectID);

        if (pTempEffectElement == null)
            return;

        pTempEffectElement.bActive = true;
        pTempEffectElement.bForceEnd = false;

        pTempEffectElement.pEffectObj.transform.position = pTargetPos;
        if (EffectDir == -1.0f)
            pTempEffectElement.pEffectObj.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        else
            pTempEffectElement.pEffectObj.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);

        pTempEffectElement.pEffectObj.SetActive(true);

        pTempEffectElement.fCurLifeTime = 0.0f;
        pTempEffectElement.fMaxLifeTime = fHideTime;
        pTempEffectElement.bForceLoopEffect = false;
    }


    //이펙트 활성화.
    public void SetBattleEffect_Scale(Vector3 pTargetPos, int nEffectID, float EffectDir = 1.0f, float fHideTime = 1.5f)
    {
        EffectPoolElement pTempEffectElement = GetFreeEffectPool(nEffectID);

        if (pTempEffectElement == null)
            return;

        pTempEffectElement.bActive = true;
        pTempEffectElement.bForceEnd = false;
        pTempEffectElement.pEffectObj.SetActive(true);
        pTempEffectElement.pEffectObj.transform.position = pTargetPos;

        pTempEffectElement.pEffectObj.transform.localScale = new Vector3(EffectDir, 1.0f, 1.0f);

        pTempEffectElement.fCurLifeTime = 0.0f;
        pTempEffectElement.fMaxLifeTime = fHideTime;
        pTempEffectElement.bForceLoopEffect = false;
    }

    


    //따라다니는 이펙트.
    public EffectPoolElement SetBattleEffect_Follow(Transform pTargetObj, bool bFollow, int nEffectID, float fDir = 1.0f, float fHideTime = 1.5f, float fZPos = -0.5f)
    {
        EffectPoolElement pTempEffectElement = GetFreeEffectPool(nEffectID);
        Vector3 pEffectAddPos_Z = new Vector3(0.0f, 0.0f, -fZPos);

        if (pTempEffectElement == null)
            return null;

        pTempEffectElement.bActive = true;
        pTempEffectElement.bForceEnd = false;
        pTempEffectElement.pEffectObj.SetActive(true);

        if (bFollow)
        {
            pTempEffectElement.pEffectObj.transform.SetParent(pTargetObj);
            pTempEffectElement.pEffectObj.transform.localScale = new Vector3(fDir, 1.0f, 1.0f);
            pTempEffectElement.pEffectObj.transform.localPosition = pEffectAddPos_Z;
        }
        else
            pTempEffectElement.pEffectObj.transform.position = pTargetObj.position + pEffectAddPos_Z;

        pTempEffectElement.fCurLifeTime = 0.0f;
        pTempEffectElement.fMaxLifeTime = fHideTime;
        pTempEffectElement.bForceLoopEffect = false;

        return pTempEffectElement;
    }

    //이펙트 활성화 - 강제무한루프이펙트.
    public EffectPoolElement SetBattleEffect_ForceLoop(Transform pTargetObj, bool bFollow, int nEffectID, float fZPos = -0.5f)
    {
        EffectPoolElement pTempEffectElement = GetFreeEffectPool(nEffectID);
        Vector3 pEffectAddPos_Z = new Vector3(0.0f, 0.0f, -fZPos);

        if (pTempEffectElement == null)
            return null;

        pTempEffectElement.bActive = true;
        pTempEffectElement.bForceEnd = false;
        pTempEffectElement.pEffectObj.SetActive(true);

        if (bFollow)
        {
            pTempEffectElement.pEffectObj.transform.SetParent(pTargetObj);
            pTempEffectElement.pEffectObj.transform.localScale = Vector3.one;
            pTempEffectElement.pEffectObj.transform.localPosition = pEffectAddPos_Z;
        }
        else
            pTempEffectElement.pEffectObj.transform.position = pTargetObj.position + pEffectAddPos_Z;

        pTempEffectElement.fCurLifeTime = 0.0f;
        pTempEffectElement.fMaxLifeTime = 0.0f;
        pTempEffectElement.bForceLoopEffect = true;

        pTempEffectElement.pEffectObj.name = "ForceLoopEff_" + nEffectID.ToString() + "_Key-" + pTempEffectElement.EffectKey.ToString();
        return pTempEffectElement;
    }






    //이펙트 비활성화.
    public void SetDisenableEffect(EffectPoolElement pElement)
    {
        if (pElement == null)
            return;

        pElement.fCurLifeTime = 0.0f;
        pElement.bActive = false;
        pElement.bForceLoopEffect = false;
        pElement.pEffectObj.transform.SetParent(transform);
        pElement.pEffectObj.transform.localScale = Vector3.one;
        pElement.pEffectObj.SetActive(false);
    }


    //이펙트풀 리스트 초기화.
    public void ClearEffectPool()
    {
        AddEffectCount = 0;

        if (pEffectList.Count == 0)
        {
            pEffectList.Clear();
            nEffectAddCount = 0;    //추가한 수 초기화.
            return;
        }


        while (true)
        {
            if (pEffectList[0].pEffectObj)
                Destroy(pEffectList[0].pEffectObj);

            pEffectList.RemoveAt(0);

            if (pEffectList.Count <= 0)
                break;
        }
        pEffectList.Clear();
        nEffectAddCount = 0;    //추가한 수 초기화.
    }



    //놀고있는 이펙트 객체 가져오기.
    public EffectPoolElement GetFreeEffectPool(int nEffectID)
    {
        for (int idx = 0; idx < pEffectList.Count; idx++)
        {
            if (pEffectList[idx].nEffectID != nEffectID)
                continue;

            if (pEffectList[idx].bActive == false)
                return pEffectList[idx];
        }

        return null;
    }

    //이펙트풀 등록.
    public int AddEffectPool(GameObject pPrefab, bool NotPauseEffect, int nMakeCount)
    {
        nEffectAddCount++;      //이펙트 추가할때 마다 카운트 증가시키자. 한번에 여러개 하도록하자. (보강해야되려나...)

        for (int idx = 0; idx < nMakeCount+1; idx++)
        {
            EffectPoolElement pTempElement = new EffectPoolElement();
            pTempElement.bActive = false;
            pTempElement.bForceEnd = false;
            pTempElement.bNotPauseEffect = NotPauseEffect;
            pTempElement.nEffectID = nEffectAddCount;
            pTempElement.pEffectObj = Instantiate(pPrefab) as GameObject;
            pTempElement.pEffectObj.name = pPrefab.name;    //Clone 떼어내기.
            pTempElement.pEffectObj.transform.SetParent(transform);
            pTempElement.pEffectObj.SetActive(false);
            pTempElement.fCurLifeTime = 0.0f;
            pTempElement.fMaxLifeTime = 0.0f;
            pTempElement.bForceLoopEffect = false;

            pTempElement.EffectKey = AddEffectCount;
            pTempElement.pEffectObj.name = "EffectPoolElement_" + pTempElement.EffectKey.ToString();
            AddEffectCount++;

            pEffectList.Add(pTempElement);
        }

        return nEffectAddCount;
    }




















    ////////////////////////////////////////////////////////////////////
    // 데미지카운트 부분.


    //배틀카운트 리스트 초기화.
    public void ClearBattleDamageCount()
    {
        if (pDamageCountList.Count <= 0)
        {
            pDamageCountList.Clear();
            return;
        }

        while (true)
        {
            Destroy(pDamageCountList[0]);
            pDamageCountList.RemoveAt(0);

            if (pDamageCountList.Count <= 0)
                break;
        }
        pDamageCountList.Clear();
    }




    //놀고있는 데미지카운트 객체 가져오기.
    public BattleDamageCount GetFreeDamageCount()
    {
        for (int idx = 0; idx < pDamageCountList.Count; idx++)
        {
            if (pDamageCountList[idx].bActive == false)
                return pDamageCountList[idx];
        }

        //없으면 더 생성.
        GameObject pTempDmgCountObj = Instantiate(pPrefab_DamageCount) as GameObject;
        pTempDmgCountObj.name = "DamageCount";
        BattleDamageCount pDmgCountInfo = pTempDmgCountObj.GetComponent<BattleDamageCount>();
        pTempDmgCountObj.transform.SetParent(transform);
        pDmgCountInfo.HideDamageCount();
        pDamageCountList.Add(pDmgCountInfo);

        return pDmgCountInfo;
    }






    //데미지카운트 - 데미지.
    public void SetDamageCount(Vector3 pTargetPos, int nDmgCount, bool bHero, bool bCritical)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();

        float fDir = 1.0f;
        if (bHero)
            fDir = -1.0f;

        DAMAGE_COUNT_KIND eDmgKind = DAMAGE_COUNT_KIND.DAMAGE_NORMAL;
        if (bCritical)
            eDmgKind = DAMAGE_COUNT_KIND.DAMAGE_CRITICAL;

        pTempDamageCount.ShowDamageCount(pTargetPos + (Vector3.up * 1.0f), nDmgCount, eDmgKind, fDir);
    }



    //데미지카운트 - 힐.
    public void SetHealCount(Vector3 pTargetPos, int nHealCount, bool bHero, bool bCritical, float fBasePosY = 1.0f)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();

        DAMAGE_COUNT_KIND eHealKind = DAMAGE_COUNT_KIND.HEAL_NORMAL;
        if (bCritical)
            eHealKind = DAMAGE_COUNT_KIND.HEAL_CRITICAL;

        pTempDamageCount.ShowDamageCount(pTargetPos + (Vector3.up * fBasePosY), nHealCount, eHealKind, 0.0f);
    }



    //데미지카운트 - Miss
    public void SetMissCount(Vector3 pTargetPos, float fBasePosY = 1.0f)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();
        pTempDamageCount.ShowDamageCount_Text(pTargetPos + (Vector3.up * fBasePosY), "MISS", DAMAGE_COUNT_KIND.MISS, 0.0f);
    }


    //데미지카운트 - 도트데미지.
    public void SetDamageCount_Dot(Vector3 pTargetPos, int nDmgCount)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();
        pTempDamageCount.ShowDamageCount(pTargetPos + (Vector3.up * 1.0f), nDmgCount, DAMAGE_COUNT_KIND.DOT_DAMAGE, 0.0f);
    }

    //데미지카운트 - 도트힐.
    public void SetHealCount_Dot(Vector3 pTargetPos, int nDmgCount)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();
        pTempDamageCount.ShowDamageCount(pTargetPos + (Vector3.up * 1.0f), nDmgCount, DAMAGE_COUNT_KIND.DOT_HEAL, 0.0f);
    }


    //데미지카운트 - 반사데미지.
    public void SetDamageCount_Reflect(Vector3 pTargetPos, int nDmgCount)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();
        pTempDamageCount.ShowDamageCount(pTargetPos + (Vector3.up * 1.0f), nDmgCount, DAMAGE_COUNT_KIND.DAMAGE_REFLECT, 0.0f);
    }


    public void SetDamageText(Vector3 pTargetPos, string szText, DAMAGE_COUNT_KIND Kind)
    {
        BattleDamageCount pTempDamageCount = GetFreeDamageCount();
        pTempDamageCount.ShowDamageCount_Text(pTargetPos + (Vector3.up * 1.0f), szText, Kind, 0.0f);
    }










}




[System.Serializable]
public class EffectPoolElement
{
    public  int         EffectKey;

    public  bool        bActive;

    public  bool        bNotPauseEffect;
    public  bool        bForceEnd;
    public  int         nEffectID;
    public  GameObject  pEffectObj;

    public  float       fCurLifeTime;
    public  float       fMaxLifeTime;

    public  bool        bForceLoopEffect;
}
