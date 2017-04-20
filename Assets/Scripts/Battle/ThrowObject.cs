using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ThrowObject : MonoBehaviour
{
    private     BattleManager       BattleMng;
    private     BattleBullet        BulletManager;
    private     BattlePawn          AttackerPawn;
    private     BattlePawn          TargetPawn;
    private     Vector3             BaseThrowPos;
    private     Vector3             TargetThrowPos;

    private     BattleSkillManager  SkillManager;

    private     AddBuffEffect       AddBuffEffectMng;

    private     ArrayList           HitList = new ArrayList();
    private     List<BattlePawn>    EnemyPawnList;

    public      float               ThrowDelayTime; //활성화 후 발사 대기시간.
    private     float               curDelayTime;

    private     float               SelfHideTime;   //혼자 비활성화 되는 시간.
    private     float               curSelfHideTime;

    public      float               MoveSpeed;      //투사체 이동 속도.
    public      Vector2             ThrowOffset;    //투사체 생성 위치 오프셋.

    public      int                 HitCount;       //총 타격 수.
    public      bool                HitDestroy;     //타격시 소멸여부.

    public      GameObject          Prefab_Eff_Bullet;      //탄환 이펙트 프리팹.
    private     GameObject          Eff_Bullet;             //탄환 이펙트 메인.

    public      GameObject          Prefab_Eff_Hit;         //탄환 피격 이펙트 프리팹.
    private     int                 EffID_Hit;              //이펙트 풀에 넣어둔 피격 이펙트의 ID.

    private     bool                SkillBullet;
    private     bool                ActiveMode;     //활성화여부.
    private     bool                MoveMode;       //이동중.

    private     bool                ParabolaShot;       //곡사.
    private     bool                HomingTargetMode;


    public      SkillType           ThrowSkillType;


    //전체화면 이동모드.
    public      bool                AllScreenMoveMode;
    public      Vector3             AllScreenMove_CurPos;
    public      Vector3             AllScreenMove_NextPos;
    public      float               AllScreenMove_Dir;


    //영역 타격모드.
    public      bool                AreaDamageMode;


    //포물선용.
    private     float vx;
    private     float vy;
    private     float g;

    private     float dat;
    private     float mh;
    private     float dh;
    private     float Parabola_time = 0.0f;
    private     float Max_Y;
    private     float mht;


    //투사체 오브젝트 초기화.
    public void InitThrowObject(BattleManager pBattleMng, BattleBullet BulletMng, BattlePawn pBasePawn)
    {
        BattleMng = pBattleMng;
        BulletManager = BulletMng;
        AttackerPawn = pBasePawn;
        AddBuffEffectMng = null;

        //적 리스트.
        if (AttackerPawn.HeroTeam)
            EnemyPawnList = BattleMng.EnemyPawnList;
        else
            EnemyPawnList = BattleMng.HeroPawnList;

        //탄환 데이터 받아온다.
        DB_BattleBullet.Schema NormalBulletData = DB_BattleBullet.Query(DB_BattleBullet.Field.Bullet_ID, AttackerPawn.DBData_Base.Index, 
            DB_BattleBullet.Field.BATTLE_BULLET_TYPE, BATTLE_BULLET_TYPE.HORIZON);

        //탄환 오브젝트.
        Prefab_Eff_Bullet = Resources.Load("Effects/" + NormalBulletData.BulletEffectName) as GameObject;
        if (Prefab_Eff_Bullet != null)
        {
            Eff_Bullet = Instantiate(Prefab_Eff_Bullet) as GameObject;
            Eff_Bullet.transform.SetParent(transform);
            Eff_Bullet.transform.localPosition = Vector3.zero;
            Eff_Bullet.transform.localScale = Vector3.one;
            Eff_Bullet.SetActive(false);

            if (pBasePawn.MoveDirection == 1.0f)
                Eff_Bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            else
                Eff_Bullet.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }


        //히트 이펙트는 최대 타격 수 만큼 만들어두자.
        Prefab_Eff_Hit = Resources.Load("Effects/" + NormalBulletData.HitEffectName) as GameObject;
        if(Prefab_Eff_Hit != null)
            EffID_Hit = BattleMng.pEffectPoolMng.AddEffectPool(Prefab_Eff_Hit, false, HitCount);

        //데이터 세팅.
        MoveSpeed = NormalBulletData.MoveSpeed;
        HitCount = NormalBulletData.MaxHitCount;

        SkillBullet = false;
        MoveMode = false;
        ActiveMode = false;
        SelfHideTime = 1.0f;
        HomingTargetMode = false;
    }








    public void InitThrowObject_Skill(BattleManager pBattleMng, BattleBullet BulletMng, BattlePawn pBasePawn, BattleSkillManager pBattleSkillMng, SkillType eSkillType)
    {
        DB_Skill.Schema TempSkill;

        BattleMng = pBattleMng;
        BulletManager = BulletMng;
        AttackerPawn = pBasePawn;
        SkillManager = pBattleSkillMng;
        AddBuffEffectMng = null;

        //적 리스트.
        if (AttackerPawn.HeroTeam)
            EnemyPawnList = BattleMng.EnemyPawnList;
        else
            EnemyPawnList = BattleMng.HeroPawnList;

        //탄환 오브젝트.
        if (eSkillType == SkillType.Max)
            Prefab_Eff_Bullet = SkillManager.SkillEffData_Max.Effect_ActivePrefab;
        else
            Prefab_Eff_Bullet = SkillManager.SkillEffData_Active.Effect_ActivePrefab;

        TempSkill = SkillManager.GetDB_Skill(eSkillType);

        
        if (Prefab_Eff_Bullet != null)
        {
            Eff_Bullet = Instantiate(Prefab_Eff_Bullet) as GameObject;
            Eff_Bullet.transform.SetParent(transform);
            Eff_Bullet.transform.localPosition = Vector3.zero;
            Eff_Bullet.transform.localScale = Vector3.one;
            Eff_Bullet.SetActive(false);

            if (pBasePawn.MoveDirection == 1.0f)
                Eff_Bullet.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            else
                Eff_Bullet.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
        }


        //히트 이펙트는 최대 타격 수 만큼 만들어두자.
        if (eSkillType == SkillType.Max)
            EffID_Hit = SkillManager.SkillEffData_Max.EffID_Hit;
        else
            EffID_Hit = SkillManager.SkillEffData_Active.EffID_Hit;

        //데이터 세팅.
        HitCount = TempSkill.HitCount;

        SkillBullet = true;
        MoveMode = false;
        ActiveMode = false;

        //소멸시간 제어.
        switch(TempSkill.Index)
        {
            case 16:
                SelfHideTime = 1.8f;
                break;

            default:
                SelfHideTime = 1.0f;
                break;
        }

        HomingTargetMode = false;
        if (TempSkill.SKILL_EFFECT_TYPE == SKILL_EFFECT_TYPE.BULLET_HOMING_MAKE
            || TempSkill.SKILL_EFFECT_TYPE == SKILL_EFFECT_TYPE.BULLET_HOMING_NORMAL)
        {
            HomingTargetMode = true;
            AddBuffEffectMng = Eff_Bullet.AddComponent<AddBuffEffect>();
            if (AddBuffEffectMng != null)
            {
                switch (TempSkill.Index)
                {
                    case 12:
                    case 20:
                    case 28:
                    case 1001:
                        AddBuffEffectMng.InitAddBuffEffect(2.0f, AttackerPawn, AttackerPawn.HeroTeam);
                        break;

                    default:
                        AddBuffEffectMng.InitAddBuffEffect(TempSkill.AreaRange_Max, AttackerPawn, AttackerPawn.HeroTeam);
                        break;
                }
            }
        }
    }













    //발사!
    public void SetThrow(bool bParabolaShot, Vector3 ThrowPos, BattlePawn pTargetPawn, SkillType eSkillType)
    {
        ThrowSkillType = eSkillType;

        HitList.Clear();
        Eff_Bullet.SetActive(false);

        ParabolaShot = bParabolaShot;
        TargetPawn = pTargetPawn;

        if (ParabolaShot)
        {
            BaseThrowPos = ThrowPos;
            MoveSpeed = 20.0f;
        }
        else
        {
            BaseThrowPos = AttackerPawn.transform.position + ThrowPos;
        }

        ActiveMode = true;
        MoveMode = false;
        curDelayTime = 0.0f;
        curSelfHideTime = 0.0f;
        transform.position = ThrowPos;
        Parabola_time = 0.0f;

        if (SkillBullet)
        {
            switch (AttackerPawn.CardIndex)
            {
                case 11:
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_BULLET_SHOT);
                    break;

                case 16:
                    SetAllScreenMoveBullet(AttackerPawn.MoveDirection);
                    BaseThrowPos = new Vector3(BaseThrowPos.x, AttackerPawn.FirstPos_Y, BaseThrowPos.z);
                    break;

                case 29:
                    AreaDamageMode = true;
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_WIND_B);
                    break;

            }
        }
        else
        {
            switch (AttackerPawn.CardIndex)
            {
                case 11:
                case 34:
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_BULLET_SHOT);
                    break;

                default:
                    Kernel.soundManager.PlaySound(SOUND.SFX_THROW_0);
                    break;
            }
        }

    }




    void Update()
    {
        if (!ActiveMode)
            return;

        if (BattleMng.GamePauseMode)
            return;


        //활성화 후 발사 딜레이.
        curDelayTime += Time.deltaTime;
        if (curDelayTime >= ThrowDelayTime)
        {
            curDelayTime = 0.0f;
            Eff_Bullet.SetActive(true);

            if (SkillBullet)
            {
                switch (AttackerPawn.CardIndex)
                {
                    case 6:
                    case 44:
                    case 58:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_FLYINGAXE);
                        break;

                    case 16:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_CAST_RUNS);
                        break;

                    case 7:
                    case 52:
                    case 1006:
                        Kernel.soundManager.PlaySound(SOUND.SFX_FLAME);
                        break;

                    case 35:
                    case 60:
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_WIND_B);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                switch (AttackerPawn.CardIndex)
                {
                    case 12:
                        break;

                    default:
                        break;
                }
            }
            MoveMode = true;
        }

        if (!MoveMode)
            return;

        if (AreaDamageMode)
        {
            UpdateBullet_AreaDamage();
        }
        else
        {
            if (AllScreenMoveMode)
                UpdateBullet_ScreenAllMove();
            else
                UpdateBullet_Normal();
        }


        //일정 시간뒤에 자동 삭제.
        curSelfHideTime += Time.deltaTime;
        if (curSelfHideTime >= SelfHideTime)
        {
            HideThrowObject();
        }
    }









    public void UpdateBullet_ScreenAllMove()
    {
        //화면 전체 이동모드.
        float CurPos_X = transform.position.x;

        if (AllScreenMove_Dir > 0)   //
        {
            CurPos_X += MoveSpeed * Time.deltaTime;
            if (CurPos_X >= AllScreenMove_NextPos.x)
            {
                AllScreenMoveMode = false;
                HideThrowObject();
            }
        }
        else
        {
            CurPos_X -= MoveSpeed * Time.deltaTime;
            if (CurPos_X <= AllScreenMove_NextPos.x)
            {
                AllScreenMoveMode = false;
                HideThrowObject();
            }
        }


        transform.position = new Vector3(CurPos_X, transform.position.y, transform.position.z);

        float HitDistance = 0.0f;
        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].IsDeath())
                continue;

            if (EnemyPawnList[idx].SpecialActionMode)
                continue;

            if (CheckAlreadyHitPawn(EnemyPawnList[idx].GetBattlePawnKey()) == true)
                continue;

            if (HitList.Count >= HitCount)
                continue;   


            HitDistance = BattleUtil.GetDistance_X(transform.position.x, EnemyPawnList[idx].PawnTransform.position.x);

            if (HitDistance <= 1.0f)
            {
                EnemyPawnList[idx].GetDamage_Skill(AttackerPawn, ThrowSkillType, EffID_Hit);
                EnemyPawnList[idx].AddBuff(AttackerPawn, ThrowSkillType);
                BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);

                //히트리스트에 추가하고...
                HitList.Add(EnemyPawnList[idx].GetBattlePawnKey());
            }
        }
    }






    public void UpdateBullet_Normal()
    {
        if (BattleMng.ApplicationPauseMode)
            return;

        //화면 전체 이동모드.
        bool bHit = false;
        BattlePawn HitPawnData = null;
        int HitDamage = 0;
        bool HitCritical = false;

        //이동한다.
        if (ParabolaShot)
        {
            PreCalculateParabola();

            Parabola_time += Time.deltaTime;
            if (Parabola_time > dat)
            {
                HideThrowObject();
                return;
            }

            Vector3 pTempVector = GetParabolaMove();
            if (pTempVector == Vector3.zero) //Nan일경우.
                transform.position = TargetThrowPos;
            else
                transform.position = GetParabolaMove();
        }
        else
        {
            float CurPos_X = transform.position.x;
            CurPos_X += AttackerPawn.MoveDirection * (Time.deltaTime * MoveSpeed);
            transform.position = new Vector3(CurPos_X, transform.position.y, transform.position.z);
        }




        //적과 부딧혔는지 체크하고 데미지를 준다.
        if (ParabolaShot)
        {
            if (TargetPawn != null)
                TargetThrowPos = TargetPawn.transform.position;
            float tempLength = BattleUtil.GetDistance_X(transform.position.x, TargetThrowPos.x);
            if (tempLength <= 1.0f && TargetPawn != null)
            {
                if (bHit == false)
                {
                    bHit = true;
                    HitPawnData = TargetPawn;
                }

                if (SkillBullet)
                {
                    TargetPawn.GetDamage_Skill(AttackerPawn, ThrowSkillType, EffID_Hit);
                    TargetPawn.AddBuff(AttackerPawn, ThrowSkillType);
                    BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);

                    switch (AttackerPawn.DBData_Base.Index)
                    {
                        case 12:
                        case 20:
                        case 28:
                        case 1001:
                            Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HEAVY_GLASS_BREAK);
                            break;
                    }
                }
                else
                {
                    AttackerPawn.MakeBattleDamage(AttackerPawn, TargetPawn, ref HitDamage, ref HitCritical);

                    //                    ClassType eTargetClass = TargetPawn.GetClassType();
                    //                    if (eTargetClass == ClassType.ClassType_Ranger || eTargetClass == ClassType.ClassType_Wizard || eTargetClass == ClassType.ClassType_Healer)
                    //                        HitDamage = (int)((float)HitDamage * 1.5f);

                    TargetPawn.SetDelayHit();
                    TargetPawn.ChangeTextureColor();
                    TargetPawn.GetDamage_Normal(HitDamage, HitCritical, EffID_Hit);

                    //데미지반사.
                    if (TargetPawn.ReflectDmgMode)
                        AttackerPawn.GetDamage_Reflect(HitDamage, TargetPawn.ReflectAddValue);

                    if (TargetPawn.GetClassType() == ClassType.ClassType_Hitter)
                        TargetPawn.SetKnockBack(AttackerPawn, KNOCKBACK_TYPE.KNOCKBACK_THROW);
                }
                HideThrowObject();
            }
        }
        else
        {
            float tempLength = 0.0f;

            for (int idx = 0; idx < EnemyPawnList.Count; idx++)
            {
                if (EnemyPawnList[idx].IsDeath())
                    continue;

                if (CheckAlreadyHitPawn(EnemyPawnList[idx].GetBattlePawnKey()) == true)
                    continue;

                tempLength = BattleUtil.GetDistance_X(transform.position.x, EnemyPawnList[idx].PawnTransform.position.x);

                if (tempLength <= 1.0f)
                {
                    if (bHit == false)
                    {
                        bHit = true;
                        HitPawnData = EnemyPawnList[idx];
                    }

                    if (SkillBullet)
                    {
                        EnemyPawnList[idx].GetDamage_Skill(AttackerPawn, ThrowSkillType, EffID_Hit);
                        EnemyPawnList[idx].AddBuff(AttackerPawn, ThrowSkillType);
                        BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);
                    }
                    else
                    {
                        AttackerPawn.MakeBattleDamage(AttackerPawn, EnemyPawnList[idx], ref HitDamage, ref HitCritical);
                        EnemyPawnList[idx].SetDelayHit();
                        EnemyPawnList[idx].GetDamage_Normal(HitDamage, HitCritical, EffID_Hit);

                        //데미지반사.
                        if (EnemyPawnList[idx].ReflectDmgMode)
                            AttackerPawn.GetDamage_Reflect(HitDamage, EnemyPawnList[idx].ReflectAddValue);

                        if (EnemyPawnList[idx].GetClassType() == ClassType.ClassType_Hitter)
                            EnemyPawnList[idx].SetKnockBack(AttackerPawn, KNOCKBACK_TYPE.KNOCKBACK_THROW);


                    }

                    //히트리스트에 추가하고...
                    HitList.Add(EnemyPawnList[idx].GetBattlePawnKey());

                    if (HitList.Count >= HitCount)
                    {
                        HideThrowObject();
                        break;
                    }
                }
            }
        }


        //독처럼 해당 영역에 이펙트 버프 추가.
        if (bHit && AddBuffEffectMng != null)
        {
            AddBuffEffectMng.SetAddBuff(HitPawnData, ThrowSkillType);
        }
    }




    private List<BattlePawn>    LinqList = new List<BattlePawn>();
    public void UpdateBullet_AreaDamage()
    {
        if (BattleMng.ApplicationPauseMode)
            return;

        //이동한다.
        PreCalculateParabola();

        Parabola_time += Time.deltaTime;
        if (Parabola_time > dat)
        {
            HideThrowObject();
            return;
        }

        Vector3 pTempVector = GetParabolaMove();
        if (pTempVector == Vector3.zero) //Nan일경우.
            transform.position = TargetThrowPos;
        else
            transform.position = GetParabolaMove();



        //적과 부딧혔는지 체크하고 데미지를 준다.
        if (TargetPawn != null)
            TargetThrowPos = TargetPawn.transform.position;
        float tempLength = BattleUtil.GetDistance_X(transform.position.x, TargetThrowPos.x);
        if (tempLength <= 1.0f && TargetPawn != null)
        {
            for (int idx = 0; idx < EnemyPawnList.Count; idx++)
            {
                if (EnemyPawnList[idx].IsDeath())
                    continue;

                EnemyPawnList[idx].SetTargetLengthValue(TargetThrowPos.x);
            }


            //대상과 가까운 적 확인.
            LinqList.Clear();
            LinqList = (from pawn in EnemyPawnList
                        where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                        orderby pawn.TargetLengthValue ascending
                        select pawn).ToList();

            int AreaHitCount = 0;
            for (int idx = 0; idx < LinqList.Count; idx++)
            {
                if (AreaHitCount >= HitCount)
                    break;

                LinqList[idx].GetDamage_Skill(AttackerPawn, ThrowSkillType, EffID_Hit);
                LinqList[idx].AddBuff(AttackerPawn, ThrowSkillType);

                AreaHitCount++;
            }

            BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);

            HideThrowObject();
            Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_EXPLOSION_2);
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



    void HideThrowObject()
    {
        curSelfHideTime = 0.0f;
        Eff_Bullet.SetActive(false);
        ActiveMode = false;
        MoveMode = false;

        BulletManager.ActiveBullet = false;
    }














    private void PreCalculateParabola()
    {
        Vector3 Pos;
        if (TargetPawn != null)
            Pos = TargetPawn.GetPawnHitPosition();
        else
            Pos = TargetThrowPos;

        float Length = Mathf.Abs(Pos.x - BaseThrowPos.x);
        Max_Y = (Length * 0.05f) + Mathf.Abs(Pos.y - BaseThrowPos.y);


        float TickMoveTime = MoveSpeed * Time.deltaTime;

        mht = (Length / TickMoveTime) / (1.0f / Time.deltaTime);

        dh = Pos.y;
        mh = Max_Y - Pos.y;

        g = 2 * mh / (mht * mht);
        vy = Mathf.Sqrt(2 * g * mh);

        float a = g;
        float b = -2 * vy;
        float c = 2 * dh;

        dat = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);
        vx = -(BaseThrowPos.x - Pos.x) / dat;


        if (float.IsNaN(vy) || float.IsNaN(dat))
            TargetPawn = null;
        else
            TargetThrowPos = Pos;



    }


    private float PreX;
    private float PreY;

    private Vector3 GetParabolaMove()
    {
        float x = BaseThrowPos.x + vx * Parabola_time;
        float y = BaseThrowPos.y + vy * Parabola_time - 0.5f * g * Parabola_time * Parabola_time;

        if (float.IsNaN(x) || float.IsNaN(y))
        {
            x = PreX;
            y = PreY;
        }
        else
        {
            PreX = x;
            PreY = y;
        }
        return new Vector3(x, y, 0.0f);
    }




    public void SetAllScreenMoveBullet(float Dir)
    {
        AllScreenMoveMode = true;
        AllScreenMove_Dir = Dir;

        Eff_Bullet.transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        if (AllScreenMove_Dir > 0)   //
        {
            AllScreenMove_CurPos = new Vector3(BattleMng.MainCamera.transform.position.x - BattleMng.pBattleFieldMng.StartPosLength, BaseThrowPos.y, BaseThrowPos.z);
            AllScreenMove_NextPos = new Vector3(BattleMng.MainCamera.transform.position.x + BattleMng.pBattleFieldMng.StartPosLength, BaseThrowPos.y, BaseThrowPos.z);

            transform.position = AllScreenMove_CurPos;
        }
        else
        {
            AllScreenMove_CurPos = new Vector3(BattleMng.MainCamera.transform.position.x + BattleMng.pBattleFieldMng.StartPosLength, BaseThrowPos.y, BaseThrowPos.z);
            AllScreenMove_NextPos = new Vector3(BattleMng.MainCamera.transform.position.x - BattleMng.pBattleFieldMng.StartPosLength, BaseThrowPos.y, BaseThrowPos.z);

            transform.position = AllScreenMove_CurPos;
        }
    }

















    public void PauseThrowTrailEffect()
    {
        if (Eff_Bullet == null)
            return;

        ParticleSystem pParticle = Eff_Bullet.GetComponent<ParticleSystem>();
        if (pParticle == null)
        {
            pParticle = Eff_Bullet.GetComponentInChildren<ParticleSystem>();
            if (pParticle == null)
                return;
        }

        pParticle.Pause(true);
    }


    public void ResumeThrowTrailEffect()
    {
        if(Eff_Bullet == null)
            return;

        ParticleSystem pParticle = Eff_Bullet.GetComponent<ParticleSystem>();
        if (pParticle == null)
        {
            pParticle = Eff_Bullet.GetComponentInChildren<ParticleSystem>();
            if (pParticle == null)
                return;
        }

        pParticle.Play(true);
    }




}
