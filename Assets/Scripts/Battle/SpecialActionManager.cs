using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialActionManager
{
    public  BattlePawn      BasePawnData;
    private BattleManager   BattleMng;

    [HideInInspector]
    public  bool            SpecialActionActive;





    //사용데이터.
    private bool            HeroTeam;
    private float           MoveSpeed;

    private Vector3         BasePos;
    private Vector3         NextPos;

    private float           CurPos_X;
    private float           CurPos_Y;
    private float           CurPos_Z;
    private Vector3         CurPos;

    private float           FieldSize = 15.0f;   //화면크기...



    //대상정보.
    [HideInInspector]
    public  BattlePawn      TargetPawn;




    //이펙트 트레일.
    private GameObject      DashEffect;
    private GameObject      SmokeEffect;

    //쌍절곤 이펙트.
    private GameObject      NunchakuEffect;
    //이펙트 폭발.
    private GameObject      ExplosionEffect;
    //참격 이펙트.
    private GameObject      SlashEffect;

    //히트 리스트.
    private List<BattlePawn>     HitList = new List<BattlePawn>();




    //매니저 데이터.
    private SkillType       SpecialSkillType;
    private bool            ReturnMoveMode;         //복귀모드.
    private bool            EndSpecialActionMode;   //액션 종료.



    //공격 딜레이 데이터.
    private bool            AttackDelayMode = false;
    private float           AttackDelayTime_Cur;
    private float           AttackDelayTime_Max;


    //모션 대기.
    private bool            WaitMotionEndMode = false;
    private float           WaitMotionTime_Cur;
    private float           WaitMotionTime_Max;
            
    //다중히트.
    private bool            MultiAttackMode;
    private int             MultiAttack_CurCount;
    private int             MultiAttack_MaxCount;
    private float           MultiAttack_CurDelay;
    private float           MultiAttack_Delay;


    public void InitSpecialAction(BattlePawn BasePawn)
    {
        BasePawnData = BasePawn;
        BattleMng = BasePawn.BattleMng;
        SpecialActionActive = true;
        HeroTeam = BasePawnData.HeroTeam;
        DashEffect = null;
        SmokeEffect = null;

        bool LoadEffect_Dash = false;
        bool LoadEffect_Smoke = false;


        switch (BasePawnData.CardIndex)
        {
            case 1:     //뿌까.
            case 56:    //뿌까전설.
                LoadEffect_Dash = true;
                LoadEffect_Smoke = true;
                break;

            case 3:     //가루.
            case 42:    //파이야.
            case 45:    //돌격스미스.
            case 54:    //해머루스.
                LoadEffect_Dash = true;
                break;

            case 27:    //아뵤.
                LoadEffect_Dash = true;

                NunchakuEffect = MonoBehaviour.Instantiate(Resources.Load("Effects/EF_Abyo_Skill_01")) as GameObject;
                NunchakuEffect.transform.position = Vector3.zero;
                NunchakuEffect.transform.localScale = Vector3.one;
                NunchakuEffect.SetActive(false);
                break;

            case 41:    //작열이.
                LoadEffect_Dash = true;

                ExplosionEffect = MonoBehaviour.Instantiate(Resources.Load("Effects/EF_Hit_Flowerbomb")) as GameObject;
                ExplosionEffect.transform.position = Vector3.zero;
                ExplosionEffect.transform.localScale = Vector3.one;
                ExplosionEffect.SetActive(false);
                break;

            case 53:    //또베.
                LoadEffect_Dash = true;

                SlashEffect = MonoBehaviour.Instantiate(Resources.Load("Effects/EF_Proj_SwordSlash_01")) as GameObject;
                SlashEffect.transform.position = Vector3.zero;
                SlashEffect.transform.localScale = Vector3.one;
                SlashEffect.SetActive(false);
                break;

            default:        //기능 없으면 리턴.
                SpecialActionActive = false;
                break;
        }

        if (LoadEffect_Dash)
        {
            DashEffect = MonoBehaviour.Instantiate(Resources.Load("Effects/EF_Proj_SpecialDash")) as GameObject;
            DashEffect.transform.position = Vector3.zero;
            DashEffect.transform.localScale = Vector3.one;
            DashEffect.SetActive(false);
        }

        if (LoadEffect_Smoke)
        {
            SmokeEffect = MonoBehaviour.Instantiate(Resources.Load("Effects/EF_Smoke_seq")) as GameObject;
            SmokeEffect.transform.position = Vector3.zero;
            SmokeEffect.transform.localScale = Vector3.one;
            SmokeEffect.SetActive(false);
        }

    }


    public void StartSpecialAction(SkillType eSkillType)
    {
        if (BasePawnData.SpecialActionMode)
            return;

        BasePawnData.SpecialActionMode = true;
        EndSpecialActionMode = false;
        SpecialSkillType = eSkillType;

        //히트리스트.
        HitList.Clear();

        //각종 초기화.
        AttackDelayMode = false;
        AttackDelayTime_Cur = 0.0f;
        AttackDelayTime_Max = 0.0f;

        WaitMotionEndMode = false;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = 0.0f;

        MultiAttackMode = false;
        MultiAttack_CurCount = 0;
        MultiAttack_MaxCount = 0;
        MultiAttack_CurDelay = 0.0f;
        MultiAttack_Delay = 0.0f;

        BasePawnData.transform.position = new Vector3(BasePawnData.transform.position.x, BasePawnData.FirstPos_Y, BasePawnData.transform.position.z);


        float TempPosX = 0.0f;

        switch (BasePawnData.CardIndex)
        {
            case 1:     //뿌까.
            case 56:    //뿌까 전설.
                BasePos = BasePawnData.transform.position;
                TempPosX = BasePos.x;
                if (HeroTeam)
                    TempPosX = BattleMng.MainCamera.transform.position.x + FieldSize / 2;
                else
                    TempPosX = BattleMng.MainCamera.transform.position.x - FieldSize / 2;

                NextPos = new Vector3(TempPosX, BasePos.y, BasePos.z);
                MoveSpeed = 10.0f;
                ReturnMoveMode = false;
                BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.MOVE);
                ShowDashEffect();
                ShowSmokeEffect();
                break;

            case 3:     //가루.
            case 27:    //아뵤.
            case 41:    //작열이.
            case 42:    //파이야.
            case 45:    //돌격스미스.
            case 53:    //또베.
            case 54:    //해머루스.
                TargetPawn = BasePawnData.AI_Module.GetNearPawnData(false);

                BasePos = BasePawnData.transform.position;
                NextPos = TargetPawn.transform.position;
                MoveSpeed = 10.0f;
                ReturnMoveMode = false;
                BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.MOVE);
                ShowDashEffect();

                if(ExplosionEffect != null)
                    ExplosionEffect.SetActive(false);

                if (NunchakuEffect != null)
                    NunchakuEffect.SetActive(false);

                if (SlashEffect != null)
                    SlashEffect.SetActive(false);

                break;

            default:        //기능 없으면 리턴.
                break;
        }
    }



	
	public void UpdateSpecialAction()
    {
        if (!BasePawnData.SpecialActionMode)
            return;

        CurPos = BasePawnData.transform.position;
        CurPos_X = BasePawnData.transform.position.x;
        CurPos_Y = BasePawnData.transform.position.y;
        CurPos_Z = BasePawnData.transform.position.z;

        switch (BasePawnData.CardIndex)
        {
            case 1:     //뿌까.
            case 56:    //뿌까 전설.
                if (HeroTeam)
                {
                    CurPos_X += MoveSpeed * Time.deltaTime;
                    if (CurPos_X >= NextPos.x)
                    {
                        if (!ReturnMoveMode)
                        {
                            CurPos_X = NextPos.x - FieldSize;
                            if (CurPos_X >= BasePos.x)
                                CurPos_X = BasePos.x;
                            NextPos = BasePos;
                            ReturnMoveMode = true;
                        }
                        else
                            EndSpecialActionMode = true;
                    }
                }
                else
                {
                    CurPos_X -= MoveSpeed * Time.deltaTime;
                    if (CurPos_X <= NextPos.x)
                    {
                        if (!ReturnMoveMode)
                        {
                            CurPos_X = NextPos.x + FieldSize;
                            if (CurPos_X <= BasePos.x)
                                CurPos_X = BasePos.x;

                            NextPos = BasePos;
                            ReturnMoveMode = true;
                        }
                        else
                            EndSpecialActionMode = true;
                    }
                }
                if (ReturnMoveMode)
                {
                    HideDashEffect();
                    HideSmokeEffect();
                    BasePawnData.StopLoopSound();
                }
                else
                {
                    UpdateDashEffect();
                    UpdateSmokeEffect();
                }

                CheckCrashPawn(CurPos_X, 1.0f, false);
                break;      //1. 뿌까 종료.


            case 3:     //가루.
            case 27:    //아뵤.
            case 41:    //작열이.
            case 42:    //파이야.
            case 45:    //돌격스미스.
            case 53:    //또베.
            case 54:    //해머루스.
                if (WaitMotionEndMode)
                {
                    WaitMotionTime_Cur += Time.deltaTime;
                    if (WaitMotionTime_Cur >= WaitMotionTime_Max)
                    {
                        ReturnMoveMode = true;
                        BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.MOVE_BACK);
                        NextPos = BasePos;
                        WaitMotionEndMode = false;
                        WaitMotionTime_Cur = 0.0f;
                    }
                }

                if (AttackDelayMode)    //공격대기일때.
                {
                    AttackDelayTime_Cur += Time.deltaTime;
                    if (AttackDelayTime_Cur >= AttackDelayTime_Max)
                    {
                        AttackDelayTime_Cur = 0.0f;
                        AttackDelayMode = false;
                        SetDamageTarget();
                    }
                }


                if(MultiAttackMode)
                {
                    MultiAttack_CurDelay += Time.deltaTime;
                    if (MultiAttack_CurDelay >= MultiAttack_Delay)
                    {
                        MultiAttack_CurDelay = 0.0f;

                        //때린다.
                        SetDamageTarget();

                        MultiAttack_CurCount++;
                        if (MultiAttack_CurCount >= MultiAttack_MaxCount)
                            MultiAttackMode = false;
                    }
                }



                //모션 대기나 공격 대기가 아니면 이동.
                if(!WaitMotionEndMode && !AttackDelayMode)
                {
                    if (HeroTeam)
                    {
                        if(ReturnMoveMode)
                        {
                            CurPos_X -= MoveSpeed * Time.deltaTime;
                            if (CurPos_X <= NextPos.x)
                            {
                                CurPos_X = NextPos.x;
                                EndSpecialActionMode = true;
                            }
                        }
                        else
                        {
                            CurPos_X += MoveSpeed * Time.deltaTime;
                            if (CurPos_X >= NextPos.x)
                                CurPos_X = NextPos.x;
                        }
                    }
                    else
                    {
                        if(ReturnMoveMode)
                        {
                            CurPos_X += MoveSpeed * Time.deltaTime;
                            if (CurPos_X >= NextPos.x)
                            {
                                CurPos_X = NextPos.x;
                                EndSpecialActionMode = true;
                            }
                        }
                        else
                        {
                            CurPos_X -= MoveSpeed * Time.deltaTime;
                            if (CurPos_X <= NextPos.x)
                                CurPos_X = NextPos.x;
                        }
                    }

                    if (!ReturnMoveMode)
                    {
                        if (TargetPawn == null || TargetPawn.IsDeath())  //대상이 없거나 죽으면.
                        {
                            TargetPawn = BasePawnData.AI_Module.GetNearPawnData(false);     //다른 대상을 찾아보고.
                            if (TargetPawn == null)  //그래도 없으면.
                                EndSpecialAction(); //종료.
                            else
                                NextPos = TargetPawn.transform.position;
                        }
                        else
                            NextPos = TargetPawn.transform.position;

                        CheckMoveToTarget(CurPos_X, 1.0f);
                    }
                }

                if (ReturnMoveMode)
                    HideDashEffect();
                else
                    UpdateDashEffect();
                break;      //3. 가루 종료.

        }


        BasePawnData.transform.position = new Vector3(CurPos_X, CurPos_Y, CurPos_Z);

        if (EndSpecialActionMode)
            EndSpecialAction();
	}





    //특수액션 종료.
    public void EndSpecialAction()
    {
        BasePawnData.SpecialActionMode = false;
        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE, true);
    }






    //대쉬 이펙트.
    public void ShowDashEffect()
    {
        if (DashEffect == null)
            return;

        DashEffect.transform.position = BasePawnData.transform.position + Vector3.up * 0.5f;
        DashEffect.SetActive(true);

        BasePawnData.ActiveLoopSound();
    }

    public void UpdateDashEffect()
    {
        if (DashEffect == null)
            return;

        if (DashEffect.activeInHierarchy)
            DashEffect.transform.position = BasePawnData.transform.position + Vector3.up * 0.5f;
    }

    public void HideDashEffect()
    {
        if (DashEffect == null)
            return;

        DashEffect.SetActive(false);
    }



    //연기 이펙트.
    public void ShowSmokeEffect()
    {
        if (SmokeEffect == null)
            return;

        SmokeEffect.transform.position = BasePawnData.transform.position;
        SmokeEffect.SetActive(true);
    }


    public void UpdateSmokeEffect()
    {
        if (SmokeEffect == null)
            return;
        
        if (SmokeEffect.activeInHierarchy)
            SmokeEffect.transform.position = BasePawnData.transform.position;
    }


    public void HideSmokeEffect()
    {
        if (SmokeEffect == null)
            return;

        SmokeEffect.SetActive(false);
    }










    //특수스킬인지 체크. 이건 나중에 테이블 Skill_Kind 에서 받아오자.
    public bool IsSpecialActionSkill()
    {
        return SpecialActionActive;
    }



    //공격체크.
    private void CheckMoveToTarget(float Pos_X, float AreaLength)                       //대상 앞에 위치했는가?
    {
        if (AttackDelayMode || ReturnMoveMode)
            return;

        if (BattleUtil.GetDistance_X(Pos_X, TargetPawn.transform.position.x) <= AreaLength)   //거리안에 있으면 히트.
        {
            NextPos = BasePawnData.transform.position;  //거리안에 있으니까 다음 포지션을 강제로 현위치로 변경.

            BasePawnData.StopLoopSound();

            switch (BasePawnData.CardIndex)
            {
                case 3:     //가루.
                    SetAttackModule_Garu();
                    break;

                case 27:    //아뵤.
                    SetAttackModule_Abyo();
                    break;

                case 41:    //작열이.
                    SetAttackModule_Fire01();
                    break;

                case 42:    //파이야.
                    SetAttackModule_Fiya();
                    break;

                case 45:    //돌격스미스.
                    SetAttackModule_Smith01();
                    break;

                case 53:    //또베.
                    SetAttackModule_Tobe();
                    break;

                case 54:    //해머루스.
                    SetAttackModule_Hammer();
                    break;
            }
        }
    }


    //가루의 공격모듈.
    private void SetAttackModule_Garu()
    {
        if (IsAddedHitList(TargetPawn))
            return;

        AddHitList(TargetPawn);

        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.ATTACK_KEEPER);

        AttackDelayMode = true;
        AttackDelayTime_Cur = 0.0f;
        AttackDelayTime_Max = 0.3f;
    }


    //아뵤의 공격모듈.
    private void SetAttackModule_Abyo()
    {
        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.SKILL_0);

        BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.SKILL_2);

        if(BasePawnData.HeroTeam)
        {
            NunchakuEffect.transform.position = BasePawnData.transform.position + new Vector3(0.5f, 0.0f);
            NunchakuEffect.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        else
        {
            NunchakuEffect.transform.position = BasePawnData.transform.position - new Vector3(0.5f, 0.0f);
            NunchakuEffect.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        NunchakuEffect.SetActive(true);
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_NUNCHAKU);

        AttackDelayMode = false;

        MultiAttackMode = true;
        MultiAttack_CurCount = 0;
        MultiAttack_MaxCount = 4;
        MultiAttack_CurDelay = 0.0f;
        MultiAttack_Delay = 0.1f;
    }



    private void SetAttackModule_Fire01()
    {
        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.SKILL_0);
        
        AttackDelayMode = false;
        MultiAttackMode = false;

        ExplosionEffect.transform.position = BasePawnData.transform.position + new Vector3(0.0f, 0.5f);
        ExplosionEffect.SetActive(true);

        SetDamageArea();

        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_EXPLOSION_2);

    }


    private void SetAttackModule_Fiya()
    {
        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.KNOCKBACK);
      
        AttackDelayMode = false;
        MultiAttackMode = false;

        SetBuffTarget();

        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_PUNCH);
    }



    private void SetAttackModule_Smith01()
    {
        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.HIT);

        AttackDelayMode = false;
        MultiAttackMode = false;

        SetDamageTarget();

        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_PUNCH);
    }



    private void SetAttackModule_Tobe()
    {
        AddHitList(TargetPawn);

        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.ATTACK_KEEPER);

        AttackDelayMode = true;
        AttackDelayTime_Cur = 0.0f;
        AttackDelayTime_Max = 0.1f;

        if(BasePawnData.HeroTeam)
        {
            SlashEffect.transform.position = BasePawnData.transform.position + new Vector3(0.5f, 0.2f);
            SlashEffect.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
        else
        {
            SlashEffect.transform.position = BasePawnData.transform.position - new Vector3(0.5f, 0.2f);
            SlashEffect.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }
        SlashEffect.SetActive(true);

        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_SWORD_SLASH);
    }


    private void SetAttackModule_Hammer()
    {
        if (IsAddedHitList(TargetPawn))
            return;

        AddHitList(TargetPawn);

        WaitMotionEndMode = true;
        WaitMotionTime_Cur = 0.0f;
        WaitMotionTime_Max = BasePawnData.SetMotion_Manual(PAWN_ANIMATION_KIND.ATTACK_KEEPER);

        AttackDelayMode = true;
        AttackDelayTime_Cur = 0.0f;
        AttackDelayTime_Max = 0.3f;

        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_WIELD_HAMMER);
    }

    //충돌체크.
    private void CheckCrashPawn(float Pos_X, float AreaLength, bool CheckMyTeam)        //적 몬스터와 충돌했는가?
    {
        // Edit - dotsoft
        List<BattlePawn> TempList = GetEnemyTeam(CheckMyTeam);

        for (int idx = 0; idx < TempList.Count; idx++)
        {
            if (TempList[idx].IsDeath())
                continue;

            if (TempList[idx].SpecialActionMode)
                continue;

            if (IsAddedHitList(TempList[idx]) == true)
                continue;

            if (BattleUtil.GetDistance_X(Pos_X, TempList[idx].transform.position.x) <= AreaLength)   //거리안에 있으면 히트.
            {
                // GetDamage_Skill 함수에서 경우에 따라 리스트에서 삭제 된다. 따라서 객체를 얻어 두고 사용하자.
                BattlePawn battlePawn = TempList[idx];

                //스킬어택.
                battlePawn.GetDamage_Skill(BasePawnData, SpecialSkillType);               //특수액션 스킬은 히트 없는게 없으니 처리.
                battlePawn.AddBuff(BasePawnData, SpecialSkillType);
                AddHitList(battlePawn);

                //화면효과.
                BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_HARD);
            }
        }

        /*
        List<BattlePawn> TempList = GetEnemyTeam(CheckMyTeam);

        for (int idx = 0; idx < TempList.Count; idx++)
        {
            if (TempList[idx].IsDeath())
                continue;

            if (TempList[idx].SpecialActionMode)
                continue;

            if (IsAddedHitList(TempList[idx]) == true)
                continue;

            if (BattleUtil.GetDistance_X(Pos_X, TempList[idx].transform.position.x) <= AreaLength)   //거리안에 있으면 히트.
            {
                //스킬어택.
                TempList[idx].GetDamage_Skill(BasePawnData, SpecialSkillType);               //특수액션 스킬은 히트 없는게 없으니 처리.
                TempList[idx].AddBuff(BasePawnData, SpecialSkillType);
                AddHitList(TempList[idx]);

                //화면효과.
                BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_HARD);
            }
        }
        */
    }

    private void SetDamageTarget()
    {
        if (TargetPawn == null || TargetPawn.IsDeath())
            return;

        //스킬어택.
        TargetPawn.GetDamage_Skill(BasePawnData, SpecialSkillType, BasePawnData.SkillManager.SkillEffData_Active.EffID_Hit);               //특수액션 스킬은 히트 없는게 없으니 처리.
        TargetPawn.AddBuff(BasePawnData, SpecialSkillType);
        AddHitList(TargetPawn);

        //화면효과.
        BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_HARD);
    }


    private void SetBuffTarget()
    {
        if (TargetPawn == null || TargetPawn.IsDeath())
            return;

        //스킬어택.
        TargetPawn.AddBuff(BasePawnData, SpecialSkillType);
        AddHitList(TargetPawn);

        //화면효과.
        BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_HARD);
    }



    private void SetDamageArea()
    {
        List<BattlePawn> TempList = GetEnemyTeam(false);

        for (int idx = 0; idx < TempList.Count; idx++)
        {
            if (TempList[idx].IsDeath())
                continue;

            if (TempList[idx].SpecialActionMode)
                continue;

            if (IsAddedHitList(TempList[idx]) == true)
                continue;

            if (BattleUtil.GetDistance_X(BasePawnData.transform.position.x, TempList[idx].transform.position.x) <= 3.0f)   //거리안에 있으면 히트.
            {
                //스킬어택.
                TempList[idx].GetDamage_Skill(BasePawnData, SpecialSkillType);               //특수액션 스킬은 히트 없는게 없으니 처리.
                TempList[idx].AddBuff(BasePawnData, SpecialSkillType);
                AddHitList(TempList[idx]);
            }
        }
        //화면효과.
        BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_HARD);
    }












    //히트리스트 검색.
    private bool IsAddedHitList(BattlePawn pPawn)
    {
        for (int idx = 0; idx < HitList.Count; idx++)
        {
            if (HitList[idx] == pPawn)
                return true;
        }

        return false;
    }

    //히트리스트 추가.
    private void AddHitList(BattlePawn pPawn)
    {
        HitList.Add(pPawn);
    }


    
    //모션 체크.
    private bool IsAnimation(string MotionName)
    {
        return BasePawnData.MotionAnimation.IsPlaying(MotionName);
    }




    private List<BattlePawn> GetEnemyTeam(bool CheckMyTeam)
    {
        List<BattlePawn> TempList = null;

        if (CheckMyTeam)
        {
            if (HeroTeam)
                TempList = BattleMng.HeroPawnList;
            else
                TempList = BattleMng.EnemyPawnList;
        }
        else
        {
            if (HeroTeam)
                TempList = BattleMng.EnemyPawnList;
            else
                TempList = BattleMng.HeroPawnList;
        }

        return TempList;
    }












}
