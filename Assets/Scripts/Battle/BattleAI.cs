using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum     BATTLE_AI_TYPE
{
    AI_KEEPER = 0,
    AI_HITTER,
    AI_RANGER,
    AI_WIZARD,
    AI_HEALER
}



public class BattleAI
{
    private     BattleManager   BattleMng;
    private     BattlePawn      BasePawnData;

    private     List<BattlePawn>    MyTeamList = null;
    private     List<BattlePawn>    TargetTeamList = null;

    private     BATTLE_AI_TYPE      eAI_Type;
    private     bool                Healer_AttackMode;
    
    //사용데이터.
    [HideInInspector]
    public      BattlePawn      TargetPawnData;


    //공격대기.
    private     bool            WaitAttackMode;
    private     float           CurWaitAttackTime;
    private     float           MaxWaitAttackTime = 1.0f;
            

    private     bool            MoveFrontMode;
    private     bool            MoveBackMode;


    //탱커푸시.
    public      bool            TankerPushMode;     //미는중.
    public      bool            TankerPushEqual;    //동등하게 미는중.
    public      BattlePawn      PushTankPawn;

    private     float           TankerPushRange;
    public      bool            TankerPush_WallBlock;   //벽 막혀서 못밀어냄.
    public      bool            TankerPushed_WallBlock; //벽까지 밀려있음.

    public void InitBattleAI(BattleManager pBattleMng, BattlePawn pBasePawn)
    {
        BattleMng = pBattleMng;
        BasePawnData = pBasePawn;

        if (pBasePawn.HeroTeam)
        {
            MyTeamList = BattleMng.HeroPawnList;
            TargetTeamList = BattleMng.EnemyPawnList;
        }
        else
        {
            MyTeamList = BattleMng.EnemyPawnList;
            TargetTeamList = BattleMng.HeroPawnList;
        }

        switch (pBasePawn.GetClassType())
        {
            case ClassType.ClassType_Keeper:
                eAI_Type = BATTLE_AI_TYPE.AI_KEEPER;
                break;
            case ClassType.ClassType_Hitter:
                eAI_Type = BATTLE_AI_TYPE.AI_HITTER;
                break;
            case ClassType.ClassType_Ranger:
                eAI_Type = BATTLE_AI_TYPE.AI_RANGER;
                break;
            case ClassType.ClassType_Wizard:
                eAI_Type = BATTLE_AI_TYPE.AI_WIZARD;
                break;
            case ClassType.ClassType_Healer:
                eAI_Type = BATTLE_AI_TYPE.AI_HEALER;
                break;
        }

        TargetPawnData = null;
        Healer_AttackMode = false;
        MoveFrontMode = false;
        MoveBackMode = false;

        TankerPushMode = false;
        PushTankPawn = null;
        TankerPushRange = 1.5f;
    }


    public void UpdatePawnAI()
    {
        //죽었거나, 밀리고있거나, 점프중이거나 맞고있으면 AI계산하지 않음.
        if (BasePawnData.IsDeath() || BasePawnData.bModule_Push || BasePawnData.bModule_Jump || BasePawnData.StunMode || BasePawnData.FreezeMode)
            return;

        if (BasePawnData.AirborneMode)
            return;

        if (BasePawnData.SpecialActionMode) //특수스킬중일때 AI취소.
            return;

        //허수아비모드일때.
        if (BasePawnData.DummyMode)
            return;


        bool EnemyAlive = false;

        if(BattleMng.CurBattleKind == BATTLE_KIND.PVE_BATTLE && BasePawnData.HeroTeam)
        {
            if(BattleMng.CheckAlivePawn(false) == true)
                EnemyAlive = true;
        }


        TankerPush_WallBlock = false;
        TankerPushed_WallBlock = false;

        //타겟설정 - 현재는 가까운적.
        switch (eAI_Type)
        {
            case BATTLE_AI_TYPE.AI_KEEPER:
                MaxWaitAttackTime = BasePawnData.AttackSpeed;
                
                TargetPawnData = GetNearPawnData(false);    //가까운 적이 있는지 확인.
                if (TargetPawnData == null)    //타겟이 없으면 그냥 중지.
                {
                    if(BasePawnData.TankerPushEffect != null)
                        BasePawnData.TankerPushEffect.SetActive(false);

                    if(EnemyAlive)
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                    else
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                }
                else
                {
                    if (WaitAttackMode) //공격 대기중일때.
                    {
                        CurWaitAttackTime += Time.deltaTime;
                        if (CurWaitAttackTime >= MaxWaitAttackTime)
                        {
                            WaitAttackMode = false;
                            CurWaitAttackTime = 0.0f;
                        }
                    }

                    //수호자 공격대기 코드 수정중.

                    //밀어내기.
                    bool PushMode = false;
                    if (GetWallLength(true) < TankerPushRange)
                    {
                        PushMode = false;
                        TankerPush_WallBlock = true;
                    }
                    else
                    {
                        PushMode = true;
                        CheckTankerPush();
                        TankerPush_WallBlock = false;
                    }

                    if (BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.IDLE 
                        || BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.MOVE
                        || BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.MOVE_CHARGING
                        || BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.ATTACK_WAIT)
                    {
                        //타겟과 거리 받아옴.
                        float fTargetLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, TargetPawnData.PawnTransform.position.x);
                        bool TeamPushMode = false;
                        if(!TankerPushMode)
                        {
                            if (BasePawnData.HeroTeam && BattleMng.ChargingKeeper_HeroTeam)
                            {
                                fTargetLength -= 0.5f;
                                TeamPushMode = true;
                            }

                            if (!BasePawnData.HeroTeam && BattleMng.ChargingKeeper_EnemyTeam)
                            {
                                fTargetLength -= 0.5f;
                                TeamPushMode = true;
                            }
                        }

                        if (fTargetLength <= TankerPushRange)
                        {
                            if (!WaitAttackMode)
                            {
                                BasePawnData.SetMotion(PAWN_ANIMATION_KIND.ATTACK_KEEPER);
                                BasePawnData.Invoke("SetAreaDamage", 0.25f);
                                WaitAttackMode = true;
                                CurWaitAttackTime = 0.0f;
                            }
                            else
                            {
                                if (PushMode)
                                {
                                    if (TankerPushMode || (PushTankPawn != null && PushTankPawn.IsDeath() == false) || TankerPushEqual || TeamPushMode)
                                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE_CHARGING);
                                    else
                                    {
                                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                                        if (BasePawnData.TankerPushEffect != null)
                                            BasePawnData.TankerPushEffect.SetActive(false);
                                    }
                                }
                                else
                                {
                                    BasePawnData.SetMotion(PAWN_ANIMATION_KIND.ATTACK_WAIT);
                                    if (BasePawnData.TankerPushEffect != null)
                                        BasePawnData.TankerPushEffect.SetActive(false);
                                }
                            }
                        }
                        else
                        {
                            //거리안에 없고, 대기상태면 이동.
                            if (BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.IDLE 
                                || BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.ATTACK_WAIT 
                                || BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.MOVE_CHARGING
                                || BasePawnData.ePawnAniState == PAWN_ANIMATION_KIND.MOVE)
                                BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                        }
                    }
                }
                break;



            case BATTLE_AI_TYPE.AI_HITTER:
                TargetPawnData = GetNearPawnData(false);
                if (TargetPawnData == null)    //타겟이 없으면 그냥 중지.
                {
                    if (EnemyAlive)
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                    else
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                }
                else
                {
                    if (BasePawnData.bHeadingDelay) //박치기 딜레이.
                    {
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                        BasePawnData.fCurTime_HeadingDelay += Time.deltaTime;
                        if (BasePawnData.fCurTime_HeadingDelay >= BasePawnData.fMaxTime_HeadingDelay)
                        {
                            BasePawnData.bHeadingDelay = false;
                            BasePawnData.fCurTime_HeadingDelay = 0.0f;
                        }
                        return;
                    }

                    //타겟과 거리 받아옴.
                    float fTargetLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, TargetPawnData.PawnTransform.position.x);

                    if (fTargetLength <= BasePawnData.AttackRange)
                    {
                        int DmgValue = 0;
                        bool isCritical = false;

                        bool TargetAirborn = false;
                        if (TargetPawnData.ePawnAniState == PAWN_ANIMATION_KIND.AIRBORNE)
                            TargetAirborn = true;

                        if (TargetPawnData.SpecialActionMode && TargetPawnData.SpecialActionMng.TargetPawn == BasePawnData)
                        {
                            BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                        }
                        else
                        {
                            //내가 밀고.
                            if (BasePawnData.Pawn_Type != PAWN_TYPE.STUFF)
                            {
                                BasePawnData.MakeBattleDamage(BasePawnData, TargetPawnData, ref DmgValue, ref isCritical);
                                TargetPawnData.GetDamage_Normal(DmgValue, isCritical, BattleMng.EffectID_DamageNormal);

                                //데미지반사.
                                if (TargetPawnData.ReflectDmgMode)
                                    BasePawnData.GetDamage_Reflect(DmgValue, TargetPawnData.ReflectAddValue);
                            }

                            if (TargetPawnData.GetClassType() != ClassType.ClassType_Keeper)
                                TargetPawnData.SetKnockBack(BasePawnData, KNOCKBACK_TYPE.KNOCKBACK_JUMP);


                            //적이 나를 밀어낸다.
                            if (TargetAirborn == false && TargetPawnData.Pawn_Type != PAWN_TYPE.STUFF)
                            {
                                TargetPawnData.MakeBattleDamage(TargetPawnData, BasePawnData, ref DmgValue, ref isCritical);
                                BasePawnData.GetDamage_Normal(DmgValue, isCritical, BattleMng.EffectID_DamageNormal);

                                //데미지반사.
                                if (BasePawnData.ReflectDmgMode)
                                    TargetPawnData.GetDamage_Reflect(DmgValue, BasePawnData.ReflectAddValue);
                            }

                            if (TargetPawnData.GetClassType() != ClassType.ClassType_Keeper)
                                BasePawnData.SetKnockBack(TargetPawnData, KNOCKBACK_TYPE.KNOCKBACK_JUMP);
                            else
                                BasePawnData.SetKnockBack(TargetPawnData, KNOCKBACK_TYPE.KNOCKBACK_SLIDE);

                            BasePawnData.bHeadingDelay = true;
                            BasePawnData.fCurTime_HeadingDelay = 0.0f;
                        }
                    }
                    else
                    {
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                    }
                }
                break;

            case BATTLE_AI_TYPE.AI_WIZARD:
            case BATTLE_AI_TYPE.AI_RANGER:
            case BATTLE_AI_TYPE.AI_HEALER:      //힐러는 AI가 바뀜.
                if (eAI_Type == BATTLE_AI_TYPE.AI_HEALER)
                {
                    if (Healer_AttackMode == false)
                        Healer_AttackMode = IsLonelyPawn_Healer();

                    if (Healer_AttackMode)
                        TargetPawnData = GetNearPawnData(false);
                    else
                        TargetPawnData = GetLowHP_PawnData(true);
                }
                else
                    TargetPawnData = GetNearPawnData(false);
    
                if(TargetPawnData != null)
                {
                    //타겟과 거리 받아옴.
                    float fTargetLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, TargetPawnData.PawnTransform.position.x);
                    float fWallLength = GetWallLength();

                    if (!MoveFrontMode && !MoveBackMode)    //전방이동이나 후방이동 상태가 아닐때.
                    {
                        if (fTargetLength <= BasePawnData.AttackRange)  //사정거리 안에 대상이 있으면
                        {
                            if (BasePawnData.bAttackReady)  //공격준비되어있으면 공격.
                            {
                                BasePawnData.SetMotion(PAWN_ANIMATION_KIND.ATTACK_1);

                                if (eAI_Type == BATTLE_AI_TYPE.AI_HEALER)
                                {
                                    if (Healer_AttackMode)
                                        BasePawnData.SetThrowAttack_Normal();
                                    else
                                        BasePawnData.SetCastingMagic(TargetPawnData, true);
                                }
                                else
                                    BasePawnData.SetThrowAttack_Normal();
                                MoveBackMode = false;
                            }
                            else
                            {
                                if (fWallLength > 1.5f) //벽에 붙어있지 않을때...
                                {
                                    if (fTargetLength < (BasePawnData.AttackRange * 0.6f))  //대기중에 적이 내 앞으로 다가오면.
                                    {
                                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE_BACK);  //백스텝모드.
                                        MoveBackMode = true;
                                        MoveFrontMode = false;
                                    }
                                    else
                                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                                }
                                else
                                    BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                            }
                        }
                        else        //사정거리 밖에 대상이 있으면.
                        {
                            BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                            MoveFrontMode = true;
                            MoveBackMode = false;
                        }

                    }
                    else
                    {
                        if (MoveBackMode)    //백스텝모드일떄.
                        {
                            if (fWallLength <= 1.5f) //벽이랑 붙었으면.
                                MoveBackMode = false;

                            if (fTargetLength > BasePawnData.AttackRange * 0.8f)    //거리 벌리고 대기.
                                MoveBackMode = false;
                        }

                        if (MoveFrontMode)
                        {
                            if (fTargetLength <= BasePawnData.AttackRange * 0.8f)   //전방 이동모드중에는 사정거리보다 좀더 다가간다.
                                MoveFrontMode = false;                              //전방 이동모드 종료되면 대기.
                        }

                        if(MoveBackMode && BasePawnData.ePawnAniState != PAWN_ANIMATION_KIND.KNOCKBACK)
                            BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE_BACK);

                        if(MoveFrontMode && BasePawnData.ePawnAniState != PAWN_ANIMATION_KIND.MOVE)
                            BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);

                    }
                }
                else
                {
                    if (EnemyAlive)
                    {
                        BasePawnData.bRangeMoveMode = false;
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.MOVE);
                    }
                    else
                        BasePawnData.SetMotion(PAWN_ANIMATION_KIND.IDLE);
                    MoveBackMode = false;
                    MoveFrontMode = false;
                }
                break;
        }

    }








    //벽과의 거리 받아온다.
    public float GetWallLength(bool bForwardWall = false)
    {
        //벽과의 거리.
        float fLeftWallPos = BattleMng.pBattleFieldMng.WallPos_Left;
        float fRightWallPos = BattleMng.pBattleFieldMng.WallPos_Right;
        if (BattleMng.CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            fRightWallPos = 5000.0f;
        }

        float fWallLength = 0.0f;

        if (bForwardWall)
        {
            if (BasePawnData.HeroTeam)
                fWallLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, fRightWallPos);
            else
                fWallLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, fLeftWallPos);
        }
        else
        {
            if (BasePawnData.HeroTeam)
                fWallLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, fLeftWallPos);
            else
                fWallLength = BattleUtil.GetDistance_X(BasePawnData.PawnTransform.position.x, fRightWallPos);
        }


        return fWallLength;
    }






    //특정 리스트에서 해당 좌표랑 가장 가까운 폰을 뽑아준다.
    public  BattlePawn GetNearPawnData(bool bMyTeam, bool CheckFollowStuff = false)
    {
        List<BattlePawn>    tempList = null;
        BattlePawn          resultPawn = null;
        float               fLength = 0.0f;

        //팀세팅.
        if(bMyTeam)
            tempList = MyTeamList;
        else
            tempList = TargetTeamList;


        //확인.
        float fBasePos_X = BasePawnData.transform.position.x;
        for (int idx = 0; idx < tempList.Count; idx++)
        {
            if (tempList[idx] == BasePawnData)
                continue;

            if (tempList[idx].IsDeath())
                continue;

            if (tempList[idx].Pawn_Type == PAWN_TYPE.STUFF)
            {
                if (!CheckFollowStuff)
                    continue;
            }

            float tempLength = BattleUtil.GetDistance_X(fBasePos_X, tempList[idx].transform.position.x);

            bool bUpdateData = false; 
            if (fLength == 0.0f || resultPawn == null)
                bUpdateData = true;
            else if (tempLength < fLength)
                bUpdateData = true;

            if(bUpdateData)
            {
                fLength = tempLength;
                resultPawn = tempList[idx];
            }
        }

        return resultPawn;
    }


    //특정 리스트에서 해당 좌표랑 가장 가까운 폰을 뽑아준다.
    public BattlePawn GetNearPawnData_Far(bool bMyTeam)
    {
        List<BattlePawn> tempList = null;
        BattlePawn resultPawn = null;
        float fLength = 0.0f;

        //팀세팅.
        if (bMyTeam)
            tempList = MyTeamList;
        else
            tempList = TargetTeamList;


        //확인.
        float fBasePos_X = BasePawnData.transform.position.x;
        for (int idx = 0; idx < tempList.Count; idx++)
        {
            if (tempList[idx] == BasePawnData)
                continue;

            if (tempList[idx].IsDeath())
                continue;

            switch(tempList[idx].GetClassType())
            {
                case ClassType.ClassType_Keeper:
                case ClassType.ClassType_Hitter:
                    continue;
            }

            float tempLength = BattleUtil.GetDistance_X(fBasePos_X, tempList[idx].transform.position.x);

            bool bUpdateData = false;
            if (fLength == 0.0f || resultPawn == null)
                bUpdateData = true;
            else if (tempLength < fLength)
                bUpdateData = true;

            if (bUpdateData)
            {
                fLength = tempLength;
                resultPawn = tempList[idx];
            }
        }

        return resultPawn;
    }



















    //특정 리스트에서 체력이 가장 낮은 아군을 찾아온다.
    private BattlePawn GetLowHP_PawnData(bool bMyTeam)
    {
        List<BattlePawn> tempList = null;
        BattlePawn resultPawn = null;
        float LowHPValue = 0.0f;

        //팀세팅.
        if (bMyTeam)
            tempList = MyTeamList;
        else
            tempList = TargetTeamList;


        //확인.
        for (int idx = 0; idx < tempList.Count; idx++)
        {
            if (tempList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            if (tempList[idx] == BasePawnData)
                continue;

            if (tempList[idx].IsDeath())
                continue;

            bool bUpdateData = false;
            if (LowHPValue == 0.0f || resultPawn == null)
                bUpdateData = true;
            else if ((tempList[idx].CurHP * 1.0f / tempList[idx].MaxHP) < LowHPValue)
                bUpdateData = true;

            if (bUpdateData)
            {
                LowHPValue = tempList[idx].CurHP * 1.0f / tempList[idx].MaxHP;
                resultPawn = tempList[idx];
            }
        }

        return resultPawn;
    }



    //아군중에 홀로 생존중인가?
    private bool IsLonelyPawn()
    {
        int LivePawnCount =0 ;
        for(int idx = 0; idx < MyTeamList.Count; idx++)
        {
            if (BasePawnData == MyTeamList[idx])
                continue;
            if (MyTeamList[idx].IsDeath())
                continue;
            LivePawnCount++;
        }

        if (LivePawnCount == 0)
            return true;

        return false;
    }


    //아군중에 힐러만 생존중인가?
    public bool IsLonelyPawn_Healer()
    {
        int LivePawnCount = 0;
        for (int idx = 0; idx < MyTeamList.Count; idx++)
        {
            if (BasePawnData == MyTeamList[idx])
                continue;
            if (MyTeamList[idx].IsDeath())
                continue;
            if (MyTeamList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            if (MyTeamList[idx].GetClassType() == ClassType.ClassType_Healer)
                continue;

            LivePawnCount++;
        }

        if (LivePawnCount == 0)
            return true;

        return false;
    }







    //밀어내기.
    //수호자 뒤에 적이 있으면 수호자 앞으로 강제 좌표이동.
    public void CheckTankerPush()
    {
        List<BattlePawn> pEnemyList;
        if (BasePawnData.HeroTeam)
            pEnemyList = BattleMng.EnemyPawnList;
        else
            pEnemyList = BattleMng.HeroPawnList;

        bool HeroPushMode = BasePawnData.HeroTeam;


        //밀어붙이는 속도 = 이동속도 * 50% + ( (밀기거리 * 0.1) + (공격력 * 0.05) ) / 100
        float MyPushPower = (BasePawnData.PushRange * 0.1f) + (BasePawnData.CurAP * 0.5f) * 0.01f;
        float TargetPushPower = 0.0f;
        float PowerGap = 0.0f;

        if (BasePawnData.ChargingUpMode)
            MyPushPower *= BasePawnData.ChargingAddValue;


        BattlePawn PushPawn = null;
        BattlePawn PushedPawn = null;


        bool bPush = false;
        bool bInArea = false;
        bool bSamePower = false;
        for (int idx = 0; idx < pEnemyList.Count; idx++)
        {
            if (pEnemyList[idx].IsDeath())
                continue;

            if (pEnemyList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            if (pEnemyList[idx].SpecialActionMode)
                continue;

            bInArea = false;
            if (HeroPushMode)
            {
                if (pEnemyList[idx].transform.position.x <= BasePawnData.transform.position.x + TankerPushRange)
                {
                    bPush = true;
                    bInArea = true;
                }
            }
            else
            {
                if (pEnemyList[idx].transform.position.x >= BasePawnData.transform.position.x - TankerPushRange)
                {
                    bPush = true;
                    bInArea = true;
                }
            }

            if (!bInArea)
            {
                pEnemyList[idx].AI_Module.PushTankPawn = null;
                continue;
            }

            if (pEnemyList[idx].GetClassType() == ClassType.ClassType_Keeper)
            {
                if (pEnemyList[idx].AI_Module.TankerPush_WallBlock)
                    BasePawnData.AI_Module.TankerPushed_WallBlock = true;

                TargetPushPower = (pEnemyList[idx].PushRange * 0.1f) + (pEnemyList[idx].CurAP * 0.5f) * 0.01f;
                if (pEnemyList[idx].ChargingUpMode)
                    TargetPushPower *= pEnemyList[idx].ChargingAddValue;


                if (pEnemyList[idx].AirborneMode || pEnemyList[idx].FreezeMode || pEnemyList[idx].StunMode)
                {
                    BasePawnData.TankerPushSpeed = BasePawnData.MoveSpeed * 0.5f;
                    PushPawn = BasePawnData;
                    PushedPawn = pEnemyList[idx];
                }
                else
                {
                    if (PowerGap == 0.0f || PowerGap <= Mathf.Abs(TargetPushPower - MyPushPower))
                    {
                        if (MyPushPower > TargetPushPower)
                        {
                            BasePawnData.TankerPushSpeed = BasePawnData.MoveSpeed * 0.3f;
                            pEnemyList[idx].TankerPushSpeed = -(BasePawnData.MoveSpeed * 0.3f);

                            PushPawn = BasePawnData;
                            PushedPawn = pEnemyList[idx];
                        }
                        else if (MyPushPower < TargetPushPower)
                        {
                            BasePawnData.TankerPushSpeed = -(BasePawnData.MoveSpeed * 0.3f);
                            pEnemyList[idx].TankerPushSpeed = BasePawnData.MoveSpeed * 0.3f;

                            PushPawn = pEnemyList[idx];
                            PushedPawn = BasePawnData;
                        }
                        else
                        {
                            BasePawnData.TankerPushSpeed = 0.0f;
                            pEnemyList[idx].TankerPushSpeed = 0.0f;

                            PushPawn = null;
                            PushedPawn = null;

                            bSamePower = true;
                        }

                        PowerGap = Mathf.Abs(TargetPushPower - MyPushPower);
                    }
                }

                pEnemyList[idx].AI_Module.PushTankPawn = BasePawnData;
            }
            else
            {
                TargetPushPower = pEnemyList[idx].MoveSpeed * 0.3f;
                BasePawnData.TankerPushSpeed = TargetPushPower;

                PushPawn = BasePawnData;
                PushedPawn = pEnemyList[idx];
            }
        }

        if (PushPawn == null && PushedPawn == null)
        {
            if(bPush)
            {
                BasePawnData.AI_Module.TankerPushEqual = true;
                BasePawnData.AI_Module.TankerPushMode = false;
                BasePawnData.AI_Module.PushTankPawn = null;
            }
            else
            {
                BasePawnData.AI_Module.TankerPushEqual = false;
                BasePawnData.AI_Module.TankerPushMode = false;
                BasePawnData.AI_Module.PushTankPawn = null;
            }
        }
        else
        {
            PushPawn.AI_Module.TankerPushEqual = false; 
            PushPawn.AI_Module.TankerPushMode = true;
            PushPawn.AI_Module.PushTankPawn = null;
            PushedPawn.AI_Module.TankerPushEqual = false;
            PushedPawn.AI_Module.TankerPushMode = false;
            PushedPawn.AI_Module.PushTankPawn = PushPawn;
        }

        if ((bPush && BasePawnData.AI_Module.TankerPushMode) || bSamePower)
        {
            Vector3 EffectPos = BasePawnData.GetPawnPosition_Center();
            if (BasePawnData.TankerPushEffect != null)
            {
                if (HeroPushMode)
                    EffectPos = new Vector3(BasePawnData.transform.position.x + TankerPushRange / 2, EffectPos.y + 0.3f, EffectPos.z);
                else
                    EffectPos = new Vector3(BasePawnData.transform.position.x - TankerPushRange / 2, EffectPos.y + 0.3f, EffectPos.z);
                BasePawnData.TankerPushEffect.transform.position = EffectPos;
                BasePawnData.TankerPushEffect.SetActive(true);
            }
        }
        else
        {
            if (BasePawnData.TankerPushEffect != null)
                BasePawnData.TankerPushEffect.SetActive(false);

        }

    }


    public void UpdatePushPosition()
    {
        if (BasePawnData.IsDeath())
            return;

        List<BattlePawn> pEnemyList;
        if (BasePawnData.HeroTeam)
            pEnemyList = BattleMng.EnemyPawnList;
        else
            pEnemyList = BattleMng.HeroPawnList;

        bool HeroPushMode = BasePawnData.HeroTeam;

        for (int idx = 0; idx < pEnemyList.Count; idx++)
        {
            if (pEnemyList[idx].IsDeath() || pEnemyList[idx].KnockbackMode || pEnemyList[idx].bModule_Jump || pEnemyList[idx].bModule_Push)
                continue;

            if (pEnemyList[idx].SpecialActionMode)
                continue;

            if (pEnemyList[idx].ePawnAniState == PAWN_ANIMATION_KIND.MOVE_BACK)
                continue;

            if (pEnemyList[idx].GetClassType() == ClassType.ClassType_Keeper)
            {
                if (!BasePawnData.AI_Module.TankerPushEqual && BasePawnData.AI_Module.TankerPushMode)
                {
                    if (HeroPushMode)
                    {
                        if (pEnemyList[idx].transform.position.x <= BasePawnData.transform.position.x + TankerPushRange)
                            pEnemyList[idx].transform.position = new Vector3(BasePawnData.transform.position.x + TankerPushRange - 0.1f, pEnemyList[idx].transform.position.y, pEnemyList[idx].transform.position.z);
                    }
                    else
                    {
                        if (pEnemyList[idx].transform.position.x >= BasePawnData.transform.position.x - TankerPushRange)
                            pEnemyList[idx].transform.position = new Vector3(BasePawnData.transform.position.x - TankerPushRange + 0.1f, pEnemyList[idx].transform.position.y, pEnemyList[idx].transform.position.z);
                    }
                }
            }
            else
            {
                if (HeroPushMode)
                {
                    if (pEnemyList[idx].transform.position.x <= BasePawnData.transform.position.x + TankerPushRange)
                        pEnemyList[idx].transform.position = new Vector3(BasePawnData.transform.position.x + TankerPushRange - 0.1f, pEnemyList[idx].transform.position.y, pEnemyList[idx].transform.position.z);
                }
                else
                {
                    if (pEnemyList[idx].transform.position.x >= BasePawnData.transform.position.x - TankerPushRange)
                        pEnemyList[idx].transform.position = new Vector3(BasePawnData.transform.position.x - TankerPushRange + 0.1f, pEnemyList[idx].transform.position.y, pEnemyList[idx].transform.position.z);
                }
            }
        }
    }






}
