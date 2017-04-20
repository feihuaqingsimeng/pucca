using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TargetAreaMagic : MonoBehaviour
{
    private     BattleManager       BattleMng;
    private     BattlePawn          AttackerPawn;
    private     BattleSkillManager  SkillMng;

    private     ArrayList           HitList = new ArrayList();
    private     List<BattlePawn>    EnemyPawnList;

    public      float               ThrowDelayTime; //활성화 후 발사 대기시간.
    private     float               curDelayTime;

    public      float               HitDelayTime;
    private     float               CurHitDelayTime;

    public      float               SelfHideTime;   //혼자 비활성화 되는 시간.
    private     float               curSelfHideTime;

    public      Vector3             MagicPosition;
    public      Vector2             MagicOffset;    //투사체 생성 위치 오프셋.

    public      int                 HitCount;       //총 타격 수.
    public      bool                HitDestroy;     //타격시 소멸여부.


    private     int                 EffID_Magic;             //탄환 이펙트 메인.
    private     int                 EffID_Hit;              //이펙트 풀에 넣어둔 피격 이펙트의 ID.


    private     bool                ActiveMode;         //활성화여부.
    private     bool                WaitDelayEnd;      //활성화여부.

    private     float               AreaRange_Start;
    private     float               AreaRange_Length;
    private     float               EffDir;

    private     SkillType           AreaMagicSkillType;
    private     bool                AllHitMode;

    private     bool                RotationToScaleMode;

    //투사체 오브젝트 초기화.
    public void InitMagicObject_Skill(BattleManager pBattleMng, BattlePawn pBasePawn, BattleSkillManager pSkillManager, SkillType eSkillType)
    {
        BattleMng = pBattleMng;
        AttackerPawn = pBasePawn;

        SkillMng = pSkillManager;

        DB_Skill.Schema tempDBSkill;
        SkillEffectData tempSkillEffData;

        if (eSkillType == SkillType.Max)
        {
            tempDBSkill = SkillMng.DB_MaxSkill;
            tempSkillEffData = SkillMng.SkillEffData_Max;
        }
        else
        {
            tempDBSkill = SkillMng.DB_ActiveSkill;
            tempSkillEffData = SkillMng.SkillEffData_Active;
        }


        //적 리스트.
        EffDir = 1.0f;
        if(tempDBSkill.SKILL_ALLY_TYPE == SKILL_ALLY_TYPE.ALLY)
        {
            if (AttackerPawn.HeroTeam)
            {
                EnemyPawnList = BattleMng.HeroPawnList;
                EffDir = -1;
            }
            else
                EnemyPawnList = BattleMng.EnemyPawnList;
        }
        else
        {
            if (AttackerPawn.HeroTeam)
                EnemyPawnList = BattleMng.EnemyPawnList;
            else
            {
                EnemyPawnList = BattleMng.HeroPawnList;
                EffDir = -1;
            }
        }

        

        //탄환 오브젝트.
        EffID_Magic = tempSkillEffData.EffID_Active;
        EffID_Hit = tempSkillEffData.EffID_Hit;

        //데이터 세팅.
        HitCount = tempDBSkill.HitCount;

        AreaRange_Start = tempDBSkill.AreaRange_Min * EffDir;
        AreaRange_Length = tempDBSkill.AreaRange_Max;

        ActiveMode = false;
        SelfHideTime = 2.0f;

        switch (tempDBSkill.Index)
        {
            case 1:
                SelfHideTime = 1.0f;
                break;
        }

    }


    //발사!
    public void SetCasting(Vector3 BasePos, SkillType eSkillType, bool FrontMake = false, bool bAllHit = false, bool RotationMode = true)
    {
        AreaMagicSkillType = eSkillType;

        HitList.Clear();
        
        ActiveMode = true;
        WaitDelayEnd = false;
        curDelayTime = 0.0f;
        CurHitDelayTime = 0.0f;
        curSelfHideTime = 0.0f;

        if (bAllHit || FrontMake)
            MagicPosition = BasePos + new Vector3(0.5f * EffDir, 0.0f, 0.0f);
        else
            MagicPosition = BasePos + new Vector3(AreaRange_Start + ((AreaRange_Length / 2) * EffDir), 0.0f, 0.0f);

        AllHitMode = bAllHit;
        RotationToScaleMode = false;
    }


    public void SetCasting_Scale(Vector3 BasePos, SkillType eSkillType, bool FrontMake = false, bool bAllHit = false, bool RotationMode = true)
    {
        AreaMagicSkillType = eSkillType;

        HitList.Clear();

        ActiveMode = true;
        WaitDelayEnd = false;
        curDelayTime = 0.0f;
        CurHitDelayTime = 0.0f;
        curSelfHideTime = 0.0f;

        if (bAllHit || FrontMake)
            MagicPosition = BasePos + new Vector3(0.5f * EffDir, 0.0f, 0.0f);
        else
            MagicPosition = BasePos + new Vector3(AreaRange_Start + ((AreaRange_Length / 2) * EffDir), 0.0f, 0.0f);

        AllHitMode = bAllHit;
        RotationToScaleMode = true;
    }





    void Update()
    {
        if (!ActiveMode)
            return;

        if (BattleMng.GamePauseMode)
            return;

        //일정 시간뒤에 자동 삭제.
        curSelfHideTime += Time.deltaTime;
        if (curSelfHideTime >= SelfHideTime)
        {
            HideMagicObject();
        }


        //활성화 후 발사 딜레이.
        if (WaitDelayEnd == false)
        {
            curDelayTime += Time.deltaTime;
            if (curDelayTime >= ThrowDelayTime)
            {
                curDelayTime = 0.0f;
                CurHitDelayTime = 0.0f;
                
                if(RotationToScaleMode)
                    BattleMng.pEffectPoolMng.SetBattleEffect_Scale(MagicPosition, EffID_Magic, EffDir);
                else
                    BattleMng.pEffectPoolMng.SetBattleEffect(MagicPosition, EffID_Magic, EffDir);


                switch (SkillMng.GetDB_Skill(AreaMagicSkillType).Index)
                {
                    case 30:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_WIELD_PUNCH);
                        break;

                    case 51:
                    case 57:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_FLYINGAXE);
                        break;

//                    case 5:     //충격파.
                    case 37:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SHORKWAVE);
                        break;

                    case 10:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_WIND_S);
                        break;

                    case 22:
                    case 53:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SWORD);
                        break;

                    case 36:
                    case 52:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_LASER);
                        break;
                }

                WaitDelayEnd = true;
            }
            return;
        }

        CurHitDelayTime += Time.deltaTime;
        if (CurHitDelayTime >= HitDelayTime)
        {
            CurHitDelayTime = 0.0f;

            if (AllHitMode)
            {
                //히트체크.
                for (int idx = 0; idx < EnemyPawnList.Count; idx++)
                {
                    if (EnemyPawnList[idx].IsDeath())
                        continue;

                    if (SkillMng.GetDB_Skill(AreaMagicSkillType).SKILL_KIND == SKILL_KIND.DAMAGE)
                    {
                        if (AttackerPawn.CurSkillValue != 0.0f)      //데미지.
                            EnemyPawnList[idx].GetDamage_Skill(AttackerPawn, AreaMagicSkillType, EffID_Hit);

                        EnemyPawnList[idx].AddBuff(AttackerPawn, AreaMagicSkillType);
                    }
                    else
                    {
                        if (AttackerPawn.CurSkillValue != 0.0f)      //힐.
                            EnemyPawnList[idx].GetHeal_Skill(AttackerPawn, EnemyPawnList[idx], AreaMagicSkillType, EffID_Hit);

                        EnemyPawnList[idx].AddBuff(AttackerPawn, AreaMagicSkillType);
                    }
                }

                BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);
            }
            else
            {
                //히트체크.
                for (int idx = 0; idx < EnemyPawnList.Count; idx++)
                {
                    if (EnemyPawnList[idx].IsDeath())
                        continue;

                    if (CheckAlreadyHitPawn(EnemyPawnList[idx].GetBattlePawnKey()) == true)
                        continue;

                    float tempLength = BattleUtil.GetDistance_X(MagicPosition.x, EnemyPawnList[idx].PawnTransform.position.x);

                    if (tempLength <= AreaRange_Length / 2)
                    {
                        if (SkillMng.GetDB_Skill(AreaMagicSkillType).SKILL_KIND == SKILL_KIND.DAMAGE)
                        {
                            if (AttackerPawn.CurSkillValue != 0.0f)      //데미지.
                            {
                                EnemyPawnList[idx].GetDamage_Skill(AttackerPawn, AreaMagicSkillType, EffID_Hit);
                            }

                            EnemyPawnList[idx].AddBuff(AttackerPawn, AreaMagicSkillType);

                        }
                        else
                        {
                            if (AttackerPawn.CurSkillValue != 0.0f)      //힐.
                            {
                                EnemyPawnList[idx].GetHeal_Skill(AttackerPawn, EnemyPawnList[idx], AreaMagicSkillType, EffID_Hit);
                            }

                            EnemyPawnList[idx].AddBuff(AttackerPawn, AreaMagicSkillType);
                        }

                        //히트리스트에 추가하고...
                        HitList.Add(EnemyPawnList[idx].GetBattlePawnKey());

                        if (HitList.Count >= HitCount)
                            break;
                    }
                }

                if (HitList.Count >= 0)
                    BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);
            }


            HideMagicObject();
        }
    }


    //이미 맞은 폰인지 체크.
    private bool CheckAlreadyHitPawn(int PawnKey)
    {
        //작업할것.
        for (int idx = 0; idx < HitList.Count; idx++)
        {
            if ((int)HitList[idx] == PawnKey)
                return true;
        }

        return false;
    }



    void HideMagicObject()
    {
        curSelfHideTime = 0.0f;
        ActiveMode = false;
    }





}
