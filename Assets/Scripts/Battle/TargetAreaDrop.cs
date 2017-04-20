using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetAreaDrop : MonoBehaviour
{
    private BattleManager BattleMng;
    private BattlePawn AttackerPawn;
    private BattleSkillManager SkillMng;

    private List<BattlePawn> EnemyPawnList;

    private int SkillIndex;

    public bool HomingDrop;
    private BattlePawn HomingTargetPawn;

    public float ThrowDelayTime; //활성화 후 발사 대기시간.
    private float curDelayTime;

    public float GroundPos_Y;
    public float DropSpeed = 9.8f;
    public float EffectOffset_Y = 0.0f;

    public float SelfHideTime;   //혼자 비활성화 되는 시간.
    private float curSelfHideTime;

    public Vector2 MagicOffset;    //투사체 생성 위치 오프셋.

    public int HitCount;       //총 타격 수.
    public bool HitDestroy;     //타격시 소멸여부.


    private GameObject  DropEffect;             //탄환 이펙트 메인.
    private GameObject  DropEndEffect;          //
    private int TimeBombEffect_ID;
    private int EffID_Hit;              //이펙트 풀에 넣어둔 피격 이펙트의 ID.

    [HideInInspector]
    public bool ActiveMode;         //활성화여부.
    private bool WaitDelayEnd;      //활성화여부.

    private float AreaRange_Length;


    //애니메이터.
    private Animator EffectAnimator;


    private bool    TimeBombMode;
    private float   CurTimeBombWaitTime;
    private float   MaxTimeBombWaitTime;
    private int     PreTimeEffectTime;


    private SkillType   DropSkillType;

    //투사체 오브젝트 초기화.
    public void InitAreaDrop(BattleManager pBattleMng, BattlePawn pBasePawn, BattleSkillManager pSkillManager, SkillType eSkillType, bool bHomingDrop = false)
    {
        BattleMng = pBattleMng;
        AttackerPawn = pBasePawn;

        HomingDrop = bHomingDrop;
        GroundPos_Y = BattleMng.pBattleFieldMng.GroundPos_Center;

        SkillMng = pSkillManager;

        DB_Skill.Schema tempDBSkill = SkillMng.GetDB_Skill(eSkillType);
        SkillEffectData tempSkillEffData = SkillMng.GetSkillEffectData(eSkillType);

        //적 리스트.
        if (tempDBSkill.SKILL_ALLY_TYPE == SKILL_ALLY_TYPE.ALLY)
        {
            if (AttackerPawn.HeroTeam)
                EnemyPawnList = BattleMng.HeroPawnList;
            else
                EnemyPawnList = BattleMng.EnemyPawnList;
        }
        else
        {
            if (AttackerPawn.HeroTeam)
                EnemyPawnList = BattleMng.EnemyPawnList;
            else
                EnemyPawnList = BattleMng.HeroPawnList;
        }



        //탄환 오브젝트.
        DropEffect = Instantiate(tempSkillEffData.Effect_ActivePrefab) as GameObject;
        DropEffect.SetActive(false);
        EffID_Hit = tempSkillEffData.EffID_Hit;
        TimeBombEffect_ID = -1;

        //데이터 세팅.
        HitCount = tempDBSkill.HitCount;

        AreaRange_Length = tempDBSkill.AreaRange_Max;

        ActiveMode = false;
        SelfHideTime = 2.0f;


        TimeBombMode = false;
        CurTimeBombWaitTime = 0.0f;
        MaxTimeBombWaitTime = 3.0f;

        EffectAnimator = null;

        SkillIndex = tempDBSkill.Index;
        switch (SkillIndex)
        {
            case 1:
                SelfHideTime = 1.0f;
                break;

            case 14:
            case 22:
                DropEndEffect = Instantiate(Resources.Load("Effects/EF_Hit_Weight")) as GameObject;
                DropEndEffect.transform.position = Vector3.zero;
                DropEndEffect.SetActive(false);
                break;

            case 19:
            case 1003:
                AreaRange_Length = 1.0f;
                break;

            case 25:
                ThrowDelayTime = 1.0f;

                DropEndEffect = Instantiate(Resources.Load("Effects/EF_Hit_Weight")) as GameObject;
                DropEndEffect.transform.position = Vector3.zero;
                DropEndEffect.SetActive(false);

                EffectAnimator = DropEffect.transform.FindChild("mesh_animator").GetComponent<Animator>();
                break;

            case 30:
//                TimeBombEffect_ID = BattleMng.pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Hit_Weight") as GameObject , false, 6);
                break;
        }

    }


    //발사!
    public void SetMake(Vector3 BasePos, SkillType eSkillType)
    {
        DropSkillType = eSkillType;
        ActiveMode = true;
        WaitDelayEnd = false;
        curDelayTime = 0.0f;
        curSelfHideTime = 0.0f;
        DropEffect.transform.position = new Vector3(BasePos.x, 5.0f + EffectOffset_Y, 0.0f);


        switch (SkillIndex)
        {
            case 14:
            case 22:
                Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_HEAVY_DROP);
                break;
        }
    }

    public void SetMakeHoming(BattlePawn Target, SkillType eSkillType)
    {
        DropSkillType = eSkillType;
        ActiveMode = true;
        WaitDelayEnd = false;
        curDelayTime = 0.0f;
        curSelfHideTime = 0.0f;
        HomingTargetPawn = Target;
        DropEffect.transform.position = new Vector3(Target.transform.position.x, 5.0f + EffectOffset_Y, 0.0f);

        if (EffectAnimator)
        {
            EffectAnimator.ResetTrigger("HIT");
        }

        switch (SkillIndex)
        {
            case 14:
            case 22:
            case 19:
            case 1003:
            case 25:
                Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_MISSILE);
                break;
        }


    }


    public void SetMakeTimeBomb(Vector3 BasePos, SkillType eSkillType, float MaxDelayTime)
    {
        DropSkillType = eSkillType;

        ActiveMode = true;
        WaitDelayEnd = false;
        curDelayTime = 0.0f;
        curSelfHideTime = 0.0f;

        TimeBombMode = true;
        CurTimeBombWaitTime = 0.0f;
        MaxTimeBombWaitTime = MaxDelayTime;
        PreTimeEffectTime = 5;

        DropEffect.transform.position = new Vector3(BasePos.x, 5.0f + EffectOffset_Y, 0.0f);
    }



    void Update()
    {
        if (!ActiveMode)
            return;

        if (BattleMng.GamePauseMode)
            return;


        if (!TimeBombMode)
        {
            //일정 시간뒤에 자동 삭제.
            curSelfHideTime += Time.deltaTime;
            if (curSelfHideTime >= SelfHideTime)
            {
                HideDropEffect();
                HideMagicObject();
            }
        }


        //활성화 후 발사 딜레이.
        if (WaitDelayEnd == false)
        {
            curDelayTime += Time.deltaTime;
            if (curDelayTime >= ThrowDelayTime)
            {
                curDelayTime = 0.0f;

                DropEffect.SetActive(true);
                WaitDelayEnd = true;
            }
            return;
        }


        DropEffect.transform.position += Vector3.down * (Time.deltaTime * DropSpeed);

        if (HomingDrop && HomingTargetPawn != null)
        {
            DropEffect.transform.position = new Vector3(HomingTargetPawn.transform.position.x, DropEffect.transform.position.y, DropEffect.transform.position.z);
        }

        if (DropEffect.transform.position.y <= GroundPos_Y + EffectOffset_Y)
        {
            DropEffect.transform.position = new Vector3(DropEffect.transform.position.x, GroundPos_Y + EffectOffset_Y, DropEffect.transform.position.z);

            //사운드.
            switch (SkillIndex)
            {
                case 14:
                case 22:
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HEAVY_DROP_LANDING);
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HEAVY_DROP_EXPLOSION);
                    break;
                case 19:
                case 1003:
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_EXPLOSION_2);
                    break;
                case 25:
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_WEIGHT);
                    break;
            }



            if (TimeBombMode)
            {
                CurTimeBombWaitTime += Time.deltaTime;

                if (CurTimeBombWaitTime >= MaxTimeBombWaitTime)
                {
                    CurTimeBombWaitTime = 0.0f;
                    TimeBombMode = false;

                    if (TimeBombEffect_ID != -1)
                    {
                        Vector3 TimeBombEffectPos = new Vector3(DropEffect.transform.position.x, GroundPos_Y + EffectOffset_Y, DropEffect.transform.position.z);
                        BattleMng.pEffectPoolMng.SetBattleEffect(TimeBombEffectPos, TimeBombEffect_ID);
                    }
                }
                else
                    return;
            }


            BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_HARD);

            if (DropEndEffect != null)
            {
                DropEndEffect.SetActive(true);
                DropEndEffect.transform.position = new Vector3(DropEffect.transform.position.x, GroundPos_Y + EffectOffset_Y, DropEffect.transform.position.z);
            }

            if (EffectAnimator != null)
            {
                EffectAnimator.SetTrigger("HIT");
                Invoke("HideDropEffect", 1.0f);
            }


            //히트체크.
            for (int idx = 0; idx < EnemyPawnList.Count; idx++)
            {
                if (EnemyPawnList[idx].IsDeath())
                    continue;

                float tempLength = BattleUtil.GetDistance_X(DropEffect.transform.position.x, EnemyPawnList[idx].PawnTransform.position.x);

                if (tempLength <= AreaRange_Length / 2)
                {
                    if (SkillMng.DB_ActiveSkill.SKILL_KIND == SKILL_KIND.DAMAGE)
                    {
                        if (AttackerPawn.GetSkillValue(DropSkillType) != 0.0f)      //데미지.
                        {
                            EnemyPawnList[idx].GetDamage_Skill(AttackerPawn, DropSkillType, EffID_Hit);
                        }

                        EnemyPawnList[idx].AddBuff(AttackerPawn, DropSkillType);

                    }
                    else
                    {
                        if (AttackerPawn.CurSkillValue != 0.0f)      //힐.
                        {
                            EnemyPawnList[idx].GetHeal_Skill(AttackerPawn, EnemyPawnList[idx], DropSkillType, EffID_Hit);
                        }

                        EnemyPawnList[idx].AddBuff(AttackerPawn, DropSkillType);
                    }
                }

                if (EffectAnimator == null)
                    HideDropEffect();
                HideMagicObject();
            }
        }
    }


    void HideDropEffect()
    {
        DropEffect.SetActive(false);
    }

    void HideMagicObject()
    {
        curSelfHideTime = 0.0f;
        ActiveMode = false;
        if (DropEndEffect != null)
            Invoke("HideDropEndEffect", 1.0f);
    }


    void HideDropEndEffect()
    {
        DropEndEffect.SetActive(false);
    }



}
