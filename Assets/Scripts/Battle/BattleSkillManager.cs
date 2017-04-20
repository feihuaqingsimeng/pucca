using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleSkillManager
{
    private BattleManager       BattleMng;
    private BattlePawn          PawnData;

    [HideInInspector]
    public  bool                AI_Mode;

    //리더스킬.
    public  DB_Skill.Schema     DB_LeaderSkill;
    public  SkillEffectData     SkillEffData_Leader;

    //액티브스킬.
    public  DB_Skill.Schema     DB_ActiveSkill;
    public  SkillEffectData     SkillEffData_Active;

    //맥스스킬.
    public  DB_Skill.Schema     DB_MaxSkill;
    public  SkillEffectData     SkillEffData_Max;



    //투사체가 있으면 생성.
    public  GameObject          ThrowSkillObject;
    public  GameObject          ThrowSkillObject_Max;

    public  GameObject[]        SummonSkillObject;
    public  GameObject[]        SummonSkillObject_Max;
    public  GameObject          SummonStuffObject;
    public  GameObject          SummonStuffObject_Max;


    private ArrayList           arrList_Target = new ArrayList();
    private List<BattlePawn>    LinqList = new List<BattlePawn>();



    //직업군 찾기 우선순위.
    private ClassType[] eClassArray_Keeper = new ClassType[5] { ClassType.ClassType_Keeper, ClassType.ClassType_Hitter, ClassType.ClassType_Healer, ClassType.ClassType_Ranger, ClassType.ClassType_Wizard};
    private ClassType[] eClassArray_Hitter = new ClassType[5] { ClassType.ClassType_Hitter, ClassType.ClassType_Keeper, ClassType.ClassType_Healer, ClassType.ClassType_Ranger, ClassType.ClassType_Wizard };
    private ClassType[] eClassArray_Ranger = new ClassType[5] { ClassType.ClassType_Ranger, ClassType.ClassType_Healer, ClassType.ClassType_Wizard, ClassType.ClassType_Keeper, ClassType.ClassType_Hitter };
    private ClassType[] eClassArray_Healer = new ClassType[5] { ClassType.ClassType_Healer, ClassType.ClassType_Wizard, ClassType.ClassType_Ranger, ClassType.ClassType_Keeper, ClassType.ClassType_Hitter };
    private ClassType[] eClassArray_Wizard = new ClassType[5] { ClassType.ClassType_Wizard, ClassType.ClassType_Ranger, ClassType.ClassType_Healer, ClassType.ClassType_Keeper, ClassType.ClassType_Hitter };


    [HideInInspector]
    public  bool    RandomActive_Success = false;       //확률로 성공 실패 여부.



    public void InitBattleSkillManager(BattleManager pBattleMng, BattlePawn pPawnData, bool LeaderPawn)
    {
        BattleMng = pBattleMng;
        PawnData = pPawnData;

        //스킬 AI 사용여부.
        /*
         * 
        if (PawnData.HeroTeam)
            AI_Mode = false;
        else
            AI_Mode = true;
        */
        AI_Mode = true;



        //리더면 리더스킬 추가.
        if (LeaderPawn)
        {
            DB_LeaderSkill = DB_Skill.Query(DB_Skill.Field.Index, PawnData.CardIndex, DB_Skill.Field.SkillType, SkillType.Leader);
            SkillEffData_Leader = new SkillEffectData();
            SkillEffData_Leader.SetSkillEffectData(pBattleMng.pEffectPoolMng, DB_LeaderSkill);
        }
        else
        {
            DB_LeaderSkill = null;
            SkillEffData_Leader = null;
        }


        //액티브 스킬.
        DB_ActiveSkill = DB_Skill.Query(DB_Skill.Field.Index, PawnData.CardIndex, DB_Skill.Field.SkillType, SkillType.Active);
        SkillEffData_Active = new SkillEffectData();
        SkillEffData_Active.SetSkillEffectData(pBattleMng.pEffectPoolMng, DB_ActiveSkill);

        //맥스 스킬.
        DB_MaxSkill = DB_Skill.Query(DB_Skill.Field.Index, PawnData.CardIndex, DB_Skill.Field.SkillType, SkillType.Max);
        SkillEffData_Max = new SkillEffectData();
        SkillEffData_Max.SetSkillEffectData(pBattleMng.pEffectPoolMng, DB_MaxSkill);



        //액티브 스킬의 요소중 이펙트타입에 따라서...
        switch (DB_ActiveSkill.SKILL_EFFECT_TYPE)
        {
            case SKILL_EFFECT_TYPE.BULLET_NONETARGET:
                ThrowSkillObject = GameObject.Instantiate(Resources.Load("Prefabs/Battle/ThrowObject/BattleBullet_Throw")) as GameObject;
                ThrowSkillObject.GetComponent<BattleBullet>().InitSkillBullet_ThrowHorizon(BattleMng, PawnData, this, SkillType.Active);

                ThrowSkillObject_Max = GameObject.Instantiate(Resources.Load("Prefabs/Battle/ThrowObject/BattleBullet_Throw")) as GameObject;
                ThrowSkillObject_Max.GetComponent<BattleBullet>().InitSkillBullet_ThrowHorizon(BattleMng, PawnData, this, SkillType.Max);
                break;

            case SKILL_EFFECT_TYPE.BULLET_HOMING_NORMAL:
                switch(DB_ActiveSkill.Index)
                {
                    case 19:
                    case 1003:
                    case 25:
                        SummonSkillObject = new GameObject[6]; //6개정도 강제로 만들어두자.
                        SummonSkillObject_Max = new GameObject[6]; //6개정도 강제로 만들어두자.
                        for (int idx = 0; idx < SummonSkillObject.Length; idx++)
                        {
                            SummonSkillObject[idx] = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillSummonSky")) as GameObject;
                            SummonSkillObject[idx].GetComponent<TargetAreaDrop>().InitAreaDrop(BattleMng, PawnData, this, SkillType.Active, true);

                            SummonSkillObject_Max[idx] = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillSummonSky")) as GameObject;
                            SummonSkillObject_Max[idx].GetComponent<TargetAreaDrop>().InitAreaDrop(BattleMng, PawnData, this, SkillType.Max, true);
                        }
                        break;
                }
                break;
            
            case SKILL_EFFECT_TYPE.BULLET_HOMING_MAKE:
                ThrowSkillObject = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillBulletThrowMake")) as GameObject;
                ThrowSkillObject.GetComponent<BattleBullet>().InitSkillBullet_ThrowHorizon(BattleMng, PawnData, this, SkillType.Active);

                ThrowSkillObject_Max = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillBulletThrowMake")) as GameObject;
                ThrowSkillObject_Max.GetComponent<BattleBullet>().InitSkillBullet_ThrowHorizon(BattleMng, PawnData, this, SkillType.Max);
                break;
            
            case SKILL_EFFECT_TYPE.MAGIC:
                ThrowSkillObject = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillAreaMagic")) as GameObject;
                ThrowSkillObject.GetComponent<TargetAreaMagic>().InitMagicObject_Skill(BattleMng, PawnData, this, SkillType.Active);

                ThrowSkillObject_Max = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillAreaMagic")) as GameObject;
                ThrowSkillObject_Max.GetComponent<TargetAreaMagic>().InitMagicObject_Skill(BattleMng, PawnData, this, SkillType.Max);
                break;
            
            case SKILL_EFFECT_TYPE.TARGET:
                ThrowSkillObject = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillTargetMagic")) as GameObject;
                ThrowSkillObject.GetComponent<TargetMagic>().InitTargetMagic_Skill(BattleMng, PawnData, this, SkillType.Active);

                ThrowSkillObject_Max = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillTargetMagic")) as GameObject;
                ThrowSkillObject_Max.GetComponent<TargetMagic>().InitTargetMagic_Skill(BattleMng, PawnData, this, SkillType.Max);
                break;
            
            case SKILL_EFFECT_TYPE.SUMMON_SKY:
                SummonSkillObject = new GameObject[6];  //6개정도 강제로 만들어두자.
                SummonSkillObject_Max = new GameObject[6];  //6개정도 강제로 만들어두자.
                for (int idx = 0; idx < SummonSkillObject.Length; idx++)
                {
                    SummonSkillObject[idx] = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillSummonSky")) as GameObject;
                    SummonSkillObject[idx].GetComponent<TargetAreaDrop>().InitAreaDrop(BattleMng, PawnData, this, SkillType.Active);

                    SummonSkillObject_Max[idx] = GameObject.Instantiate(Resources.Load("Prefabs/Battle/SkillObject/SkillSummonSky")) as GameObject;
                    SummonSkillObject_Max[idx].GetComponent<TargetAreaDrop>().InitAreaDrop(BattleMng, PawnData, this, SkillType.Max);
                }
                break;
            
            case SKILL_EFFECT_TYPE.SUMMON_HOMING:
                SummonStuffObject = GameObject.Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
                SummonStuffObject.GetComponent<BattlePawn>().InitPawnStuff(PawnData.HeroTeam, PawnData.CardIndex, 10, 10.0f);
                SummonStuffObject.SetActive(false);

                SummonStuffObject_Max = GameObject.Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
                SummonStuffObject_Max.GetComponent<BattlePawn>().InitPawnStuff(PawnData.HeroTeam, PawnData.CardIndex, 10, 10.0f);
                SummonStuffObject_Max.SetActive(false);
                break;
            
            case SKILL_EFFECT_TYPE.SUMMON_GROUND:
                switch (DB_ActiveSkill.Index)
                {
                    case 24:
                        DB_Buff.Schema BuffBase_Active = DB_Buff.Query(DB_Buff.Field.Index, DB_ActiveSkill.BuffNumber_1);
                        SummonStuffObject = GameObject.Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
                        SummonStuffObject.GetComponent<BattlePawn>().InitPawnStuff(PawnData.HeroTeam, PawnData.CardIndex, 10, BuffBase_Active.MaxTime);
                        SummonStuffObject.SetActive(false);

                        DB_Buff.Schema BuffBase_Max = DB_Buff.Query(DB_Buff.Field.Index, DB_MaxSkill.BuffNumber_1);
                        SummonStuffObject_Max = GameObject.Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
                        SummonStuffObject_Max.GetComponent<BattlePawn>().InitPawnStuff(PawnData.HeroTeam, PawnData.CardIndex, 10, BuffBase_Max.MaxTime);
                        SummonStuffObject_Max.SetActive(false);
                        break;
                }
                break;
        }
    }




    [HideInInspector]
    public bool         ActiveCheckOK = false;
    private SkillType   ActiveSkillType;

    //스킬 AI 체크부분.
    public void UpdateSkill_AI()
    {
        DB_Skill.Schema DB_SkillTemp;

        if (PawnData.HeroTeam)
        {
            if (BattleMng.UseExSkill_H)
            {
                DB_SkillTemp = DB_MaxSkill;
                ActiveSkillType = SkillType.Max;
            }
            else
            {
                DB_SkillTemp = DB_ActiveSkill;
                ActiveSkillType = SkillType.Active;
            }

            if (PawnData.SkillCoolTime_Cur < PawnData.SkillCoolTime_Max)
                return;
        }
        else
        {
            if (BattleMng.UseExSkill_E)
            {
                DB_SkillTemp = DB_MaxSkill;
                ActiveSkillType = SkillType.Max;
            }
            else
            {
                DB_SkillTemp = DB_ActiveSkill;
                ActiveSkillType = SkillType.Active;
            }

            if (PawnData.SkillCoolTime_Cur < PawnData.SkillCoolTime_Max)
                return;
        }

        ActiveCheckOK = false;
        //적을 찾는 조건 체크.
        switch (DB_SkillTemp.SKILLACTIVE_ACTION)
        {
            case SKILLACTIVE_ACTION.MY_HIT_COUNT:   //
                if (PawnData.Check_HitCount >= DB_SkillTemp.ActiveCheckValue)
                    ActiveCheckOK = true;
                break;
            
            case SKILLACTIVE_ACTION.MYTEAM_HIT_COUNT:   //
                if (PawnData.HeroTeam)
                {
                    if (BattleMng.Check_TeamHitCount >= DB_SkillTemp.ActiveCheckValue)
                        ActiveCheckOK = true;
                }
                else
                {
                    if (BattleMng.Check_EnemyTeamHitCount >= DB_SkillTemp.ActiveCheckValue)
                        ActiveCheckOK = true;
                }
                break;
            
            case SKILLACTIVE_ACTION.TARGET_HIT_COUNT:
                break;
            
            case SKILLACTIVE_ACTION.ENEMYTEAM_HIT_COUNT:    //
                if (PawnData.HeroTeam)
                {
                    if (BattleMng.Check_EnemyTeamHitCount >= DB_SkillTemp.ActiveCheckValue)
                        ActiveCheckOK = true;
                }
                else
                {
                    if (BattleMng.Check_TeamHitCount >= DB_SkillTemp.ActiveCheckValue)
                        ActiveCheckOK = true;
                }
                break;
            
            case SKILLACTIVE_ACTION.MY_MISS_COUNT:
                break;
            
            case SKILLACTIVE_ACTION.MYTEAM_MISS_COUNT:
                break;
            
            case SKILLACTIVE_ACTION.TARGET_MISS_COUNT:
                break;
            
            case SKILLACTIVE_ACTION.ENEMYTEAM_MISS_COUNT:
                break;
            
            case SKILLACTIVE_ACTION.MYTEAM_TOTAL_HP:    //
                if (PawnData.HeroTeam)
                {
                    if (BattleMng.Check_TeamHP <= DB_SkillTemp.ActiveCheckValue)
                        ActiveCheckOK = true;
                }
                else
                {
                    if (BattleMng.Check_EnemyTeamHP <= DB_SkillTemp.ActiveCheckValue)
                        ActiveCheckOK = true;
                }
                break;
            
            case SKILLACTIVE_ACTION.ENEMYTEAM_TOTAL_HP:
                break;
            
            case SKILLACTIVE_ACTION.TIME:   //
                if (PawnData.Check_TimeCount >= DB_SkillTemp.ActiveCheckValue)
                    ActiveCheckOK = true;
                break;
        }



        if (ActiveCheckOK == false)
        {
            if (DB_SkillTemp.RangeCheck)
            {
                List<BattlePawn> TempList;
                if (PawnData.HeroTeam)
                    TempList = BattleMng.EnemyPawnList;
                else
                    TempList = BattleMng.HeroPawnList;

                int AreaInCount = 0;
                for (int idx = 0; idx < TempList.Count; idx++)
                {
                    if (TempList[idx].IsDeath())
                        continue;

                    float Distance = BattleUtil.GetDistance_X(PawnData.PawnTransform.position.x, TempList[idx].PawnTransform.position.x);
                    if (Distance >= DB_SkillTemp.AreaRange_Min && Distance <= DB_SkillTemp.AreaRange_Max)
                        AreaInCount++;
                }

                if (AreaInCount >= DB_SkillTemp.AreaPawnCount)
                    ActiveCheckOK = true;
            }
        }
    }





    public void CheckUseSkill()
    {
        if (PawnData.HeroTeam)
        {
            if (BattleMng.UseExSkill_H)
                ActiveSkillType = SkillType.Max;
            else
                ActiveSkillType = SkillType.Active;

            if (PawnData.SkillCoolTime_Cur < PawnData.SkillCoolTime_Max)
            {
                if (PawnData.SkillUseTurn)
                    PawnData.SkillUseTurn = false;
                return;
            }
        }
        else
        {
            if (BattleMng.UseExSkill_E)
                ActiveSkillType = SkillType.Max;
            else
                ActiveSkillType = SkillType.Active;

            if (PawnData.SkillCoolTime_Cur < PawnData.SkillCoolTime_Max)
            {
                if (PawnData.SkillUseTurn)
                    PawnData.SkillUseTurn = false;
                return;
            }
        }

        if (PawnData.SkillUseTurn)
        {
            PawnData.SkillUseTurn = false;
            BattleMng.SetWaitUseSkillCheck(PawnData.HeroTeam,  false);
            bool CheckUse = false;
            PawnData.SetUseSkill(ActiveSkillType, ref CheckUse);
        }

    }









    public void UseSkill(SkillType eUseSkillType)
    {
        DB_Skill.Schema     tempDB_Skill = null;
        SkillEffectData     tempEffectData = null;
        List<BattlePawn>    tempPawnList = null;

        //대상리스트 초기화.
        arrList_Target.Clear();

        tempDB_Skill = GetDB_Skill(eUseSkillType);
        tempEffectData = GetSkillEffectData(eUseSkillType);

        //대상에 따라 리스트 정의.
        switch (tempDB_Skill.SKILL_ALLY_TYPE)
        {
            case SKILL_ALLY_TYPE.ALLY:
                if (PawnData.HeroTeam)
                    tempPawnList = BattleMng.HeroPawnList;
                else
                    tempPawnList = BattleMng.EnemyPawnList;
                break;

            case SKILL_ALLY_TYPE.ENEMY:
                if (PawnData.HeroTeam)
                    tempPawnList = BattleMng.EnemyPawnList;
                else
                    tempPawnList = BattleMng.HeroPawnList;
                break;
        }


        //사운드.
        if (tempDB_Skill.SKILL_KIND == SKILL_KIND.BUFF)
        {
            if(tempDB_Skill.BuffNumber_1 != 0)
            {
                DB_Buff.Schema tempBuffData_1 = DB_Buff.Query(DB_Buff.Field.Index, tempDB_Skill.BuffNumber_1);
                if(tempBuffData_1.BUFF_GOOD)
                    Kernel.soundManager.PlaySound(SOUND.SFX_BUFF_0);
                else
                    Kernel.soundManager.PlaySound(SOUND.SFX_DEBUFF_0);
            }

            if (tempDB_Skill.BuffNumber_2 != 0)
            {
                DB_Buff.Schema tempBuffData_2 = DB_Buff.Query(DB_Buff.Field.Index, tempDB_Skill.BuffNumber_2);
                if (tempBuffData_2.BUFF_GOOD)
                    Kernel.soundManager.PlaySound(SOUND.SFX_BUFF_0);
                else
                    Kernel.soundManager.PlaySound(SOUND.SFX_DEBUFF_0);
            }
        }


        //타겟타입에 따라서 적용대상 리스트 생성.
        int nAddCount = 0;
        switch (tempDB_Skill.SKILL_TARGET_TYPE)
        {
            case SKILL_TARGET_TYPE.NONE:
                break;

            case SKILL_TARGET_TYPE.AREA:
                if (tempDB_Skill.SKILL_EFFECT_TYPE != SKILL_EFFECT_TYPE.SUMMON_SKY)
                {
                    if ((eUseSkillType == SkillType.Max && ThrowSkillObject_Max == null) || (eUseSkillType == SkillType.Active && ThrowSkillObject == null))
                        return;

                    switch (PawnData.CardIndex)
                    {
//                        case 5: //충격파.
                        case 37:
                            ThrowSkillObject_Max.GetComponent<TargetAreaMagic>().SetCasting(PawnData.GetPawnPosition_Ground(), eUseSkillType);
                            break;

                        case 22:
                        case 53:
                            ThrowSkillObject_Max.GetComponent<TargetAreaMagic>().SetCasting_Scale(PawnData.GetPawnPosition_Center(), eUseSkillType, true, false);
                            break;

                        default:
                            ThrowSkillObject_Max.GetComponent<TargetAreaMagic>().SetCasting(PawnData.GetPawnPosition_Center(), eUseSkillType);
                            break;
                    }
                    return;
                }
                break;

            case SKILL_TARGET_TYPE.SELF:
                arrList_Target.Add(PawnData.GetBattlePawnKey());
                nAddCount = 1;
                break;

            case SKILL_TARGET_TYPE.TEAM_ALL:
                if (eUseSkillType != SkillType.Leader)
                {
                    switch (PawnData.CardIndex)
                    {
                        case 36:
                        case 52:
                            ThrowSkillObject.GetComponent<TargetAreaMagic>().SetCasting(PawnData.GetPawnPosition_Center(), eUseSkillType, true, true);
                            break;

                        default:
                            for (int idx = 0; idx < tempPawnList.Count; idx++)
                            {
                                if (tempPawnList[idx].IsDeath())
                                    continue;

                                arrList_Target.Add(tempPawnList[idx].GetBattlePawnKey());
                                nAddCount++;
                            }
                            break;
                    }
                }
                else
                {
                    for (int idx = 0; idx < tempPawnList.Count; idx++)
                    {
                        if (tempPawnList[idx].IsDeath())
                            continue;

                        arrList_Target.Add(tempPawnList[idx].GetBattlePawnKey());
                        nAddCount++;
                    }
                }
                break;

            case SKILL_TARGET_TYPE.RANDOM:
                int RandomPawnKey = 0;
                while (true)
                {
                    if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                        break;

                    int randomSlot = Random.Range(0, tempPawnList.Count);
                    if(tempPawnList[randomSlot].IsDeath() == false)     //살아있으면.
                    {
                        RandomPawnKey = tempPawnList[randomSlot].GetBattlePawnKey();

                        if (tempPawnList[randomSlot].Pawn_Type == PAWN_TYPE.STUFF)
                            continue;

                        if (IsAlreadyAddList(RandomPawnKey) == false)   //이미 추가한 폰인지 체크하고...
                        {
                            arrList_Target.Add(RandomPawnKey);      //추가하고...
                            nAddCount++;
                            if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                break;

                            if (nAddCount >= GetLivePawnCount(tempPawnList))    //추가한 숫자가 현재 살아있는 총 수와 같거나 크면 끝.
                                break;
                        }
                    }
                }
                break;
            
            case SKILL_TARGET_TYPE.FIRST_KEEPER:
                for (int idx = 0; idx < 5; idx++)
                {
                    if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                        break;

                    for (int sub = 0; sub < tempPawnList.Count; sub++)
                    {
                        if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                            continue;

                        if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                            continue;

                        if(eClassArray_Keeper[idx] == tempPawnList[sub].GetClassType())
                        {
                            arrList_Target.Add(tempPawnList[sub].GetBattlePawnKey());      //추가하고...
                            nAddCount++;
                            if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                break;
                        }
                    }
                }
                break;
            
            case SKILL_TARGET_TYPE.FIRST_HITTER:
                for (int idx = 0; idx < 5; idx++)
                {
                    if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                        break;

                    for (int sub = 0; sub < tempPawnList.Count; sub++)
                    {
                        if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                            continue;

                        if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                            continue;

                        if (tempDB_Skill.Index == 31)
                        {
                            if (tempPawnList[sub].FreezeMode)
                                continue;
                        }

                        if(eClassArray_Hitter[idx] == tempPawnList[sub].GetClassType())
                        {
                            arrList_Target.Add(tempPawnList[sub].GetBattlePawnKey());      //추가하고...
                            nAddCount++;
                            if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                break;
                        }
                    }
                }

                if (nAddCount == 0)     //얼어있지 않은 적이 없으면 다시 돈다.
                {
                    for (int idx = 0; idx < 5; idx++)
                    {
                        if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                            break;

                        for (int sub = 0; sub < tempPawnList.Count; sub++)
                        {
                            if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                                continue;

                            if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                                continue;

                            if (eClassArray_Hitter[idx] == tempPawnList[sub].GetClassType())
                            {
                                arrList_Target.Add(tempPawnList[sub].GetBattlePawnKey());      //추가하고...
                                nAddCount++;
                                if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                    break;
                            }
                        }
                    }
                }

                break;
            
            case SKILL_TARGET_TYPE.FIRST_RANGER:
                for (int idx = 0; idx < 5; idx++)
                {
                    if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                        break;

                    for (int sub = 0; sub < tempPawnList.Count; sub++)
                    {
                        if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                            continue;

                        if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                            continue;

                        if(eClassArray_Ranger[idx] == tempPawnList[sub].GetClassType())
                        {
                            arrList_Target.Add(tempPawnList[sub].GetBattlePawnKey());      //추가하고...
                            nAddCount++;
                            if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                break;
                        }
                    }
                }
                break;
            
            case SKILL_TARGET_TYPE.FIRST_HEALER:
                for (int idx = 0; idx < 5; idx++)
                {
                    if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                        break;

                    for (int sub = 0; sub < tempPawnList.Count; sub++)
                    {
                        if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                            continue;

                        if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                            continue;

                        if(eClassArray_Healer[idx] == tempPawnList[sub].GetClassType())
                        {
                            arrList_Target.Add(tempPawnList[sub].GetBattlePawnKey());      //추가하고...
                            nAddCount++;
                            if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                break;
                        }
                    }
                }
                break;
            
            case SKILL_TARGET_TYPE.FIRST_WIZARD:
                for (int idx = 0; idx < 5; idx++)
                {
                    if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                        break;

                    for (int sub = 0; sub < tempPawnList.Count; sub++)
                    {
                        if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                            continue;

                        if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                            continue;

                        if(eClassArray_Wizard[idx] == tempPawnList[sub].GetClassType())
                        {
                            arrList_Target.Add(tempPawnList[sub].GetBattlePawnKey());      //추가하고...
                            nAddCount++;
                            if (nAddCount >= tempDB_Skill.HitCount) //추가한 숫자가 히트카운트보다 크면 끝.
                                break;
                        }
                    }
                }
                break;
            
            case SKILL_TARGET_TYPE.HIGH_HP:
                //체력순으로 정렬.
                LinqList.Clear();
                LinqList = (from pawn in tempPawnList
                                  where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                                  orderby ((float)pawn.CurHP * 1.0f / (float)pawn.MaxHP) descending
                                  select pawn).ToList();

                for (int idx = 0; idx < LinqList.Count; idx++)
                {
                    arrList_Target.Add(LinqList[idx].GetBattlePawnKey());
                    nAddCount++;
                    if (nAddCount >= tempDB_Skill.HitCount)
                        break;
                }
                break;
            
            case SKILL_TARGET_TYPE.LOW_HP:
                if (tempDB_Skill.SKILL_EFFECT_TYPE == SKILL_EFFECT_TYPE.SUMMON_GROUND)
                    break;

                //체력순으로 정렬.
                LinqList.Clear();

                if (tempDB_Skill.SKILL_KIND == SKILL_KIND.HEAL && PawnData.AI_Module.IsLonelyPawn_Healer() == false)
                {
                    LinqList = (from pawn in tempPawnList
                                where pawn.IsDeath() == false && pawn != PawnData && pawn.Pawn_Type != PAWN_TYPE.STUFF
                                orderby ((float)pawn.CurHP * 1.0f / (float)pawn.MaxHP) ascending
                                select pawn).ToList();
                }
                else
                {
                    LinqList = (from pawn in tempPawnList
                                where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                                orderby ((float)pawn.CurHP * 1.0f / (float)pawn.MaxHP) ascending
                                select pawn).ToList();
                }

                for (int idx = 0; idx < LinqList.Count; idx++)
                {
//                    if (PawnData == LinqList[idx])
 //                       continue;

                    arrList_Target.Add(LinqList[idx].GetBattlePawnKey());
                    nAddCount++;
                    if (nAddCount >= tempDB_Skill.HitCount)
                        break;
                }
                break;
            
            case SKILL_TARGET_TYPE.HIGH_AP:
                LinqList.Clear();
                LinqList = (from pawn in tempPawnList
                            where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                            orderby pawn.CurAP descending
                            select pawn).ToList();

                for (int idx = 0; idx < LinqList.Count; idx++)
                {
                    arrList_Target.Add(LinqList[idx].GetBattlePawnKey());
                    nAddCount++;
                    if (nAddCount >= tempDB_Skill.HitCount)
                        break;
                }

                break;
            
            case SKILL_TARGET_TYPE.LOW_AP:
                LinqList.Clear();
                LinqList = (from pawn in tempPawnList
                            where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                            orderby pawn.CurAP ascending
                            select pawn).ToList();

                for (int idx = 0; idx < LinqList.Count; idx++)
                {
                    arrList_Target.Add(LinqList[idx].GetBattlePawnKey());
                    nAddCount++;
                    if (nAddCount >= tempDB_Skill.HitCount)
                        break;
                }
                break;

            case SKILL_TARGET_TYPE.HIGH_DP:     //높은 방어순.
                LinqList.Clear();
                LinqList = (from pawn in tempPawnList
                            where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                            orderby pawn.CurDP descending
                            select pawn).ToList();

                for (int idx = 0; idx < LinqList.Count; idx++)
                {
                    arrList_Target.Add(LinqList[idx].GetBattlePawnKey());
                    nAddCount++;
                    if (nAddCount >= tempDB_Skill.HitCount)
                        break;
                }
                break;

            case SKILL_TARGET_TYPE.LOW_DP:      //낮은 방어순.
                LinqList.Clear();
                LinqList = (from pawn in tempPawnList
                            where pawn.IsDeath() == false && pawn.Pawn_Type != PAWN_TYPE.STUFF
                            orderby pawn.CurDP ascending
                            select pawn).ToList();

                for (int idx = 0; idx < LinqList.Count; idx++)
                {
                    arrList_Target.Add(LinqList[idx].GetBattlePawnKey());
                    nAddCount++;
                    if (nAddCount >= tempDB_Skill.HitCount)
                        break;
                }
                break;
        }



        int MakeCount = 0;
        GameObject[] TempSkillObjList = null;

        //적용대상 리스트 생성되었으면 적용.
        if (eUseSkillType == SkillType.Leader)
        {
            for (int idx = 0; idx < arrList_Target.Count; idx++)
            {
                GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]).AddBuff(PawnData, SkillType.Leader);
            }
        }
        else
        {
            //이펙트 타입에 따라서.
            switch (tempDB_Skill.SKILL_EFFECT_TYPE)
            {
                case SKILL_EFFECT_TYPE.NONE:
                    switch (tempDB_Skill.Index)
                    {
                        case 18:    //에드워드.
                            DB_Skill.Schema TempSkillData = PawnData.SkillManager.GetDB_Skill(eUseSkillType);
                            int IgnoreBuffIndex = GetBuffIndex(TempSkillData, BUFF_KIND.LOST_HP);
                            int LeftBuffIndex = GetBuffIndex(TempSkillData, BUFF_KIND.COOLTIME_DOWN);

                            for (int idx = 0; idx < arrList_Target.Count; idx++)
                            {
                                BattlePawn pPawn = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);
                                BattleMng.pEffectPoolMng.SetBattleEffect(pPawn.GetPawnHitPosition(), tempEffectData.EffID_Hit);

                                pPawn.AddBuff_Ignore(PawnData, eUseSkillType, IgnoreBuffIndex);
                            }

                            //자신에게 남은버프 부여.
                            PawnData.AddBuff_Ignore(PawnData, eUseSkillType, LeftBuffIndex);

                            break;

                        case 47:                    //정화스킬.
                            BattleMng.ShowTeamSkillEffect(PawnData.HeroTeam, true, tempEffectData.EffID_Active);

                            for (int idx = 0; idx < arrList_Target.Count; idx++)
                            {
                                BattlePawn pPawn = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);
                                pPawn.RemoveGoodBuff();
                                BattleMng.pEffectPoolMng.SetBattleEffect(pPawn.GetPawnHitPosition(), tempEffectData.EffID_Hit);

                                pPawn.AddBuff(PawnData, eUseSkillType);
                            }
                            break;

                        case 23:                //정화의 불꽃.
                            BattleMng.ShowTeamSkillEffect(PawnData.HeroTeam, true, tempEffectData.EffID_Active);

                            for (int idx = 0; idx < arrList_Target.Count; idx++)
                            {
                                BattlePawn pPawn = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);
                                pPawn.RemoveBadBuff();
                                BattleMng.pEffectPoolMng.SetBattleEffect(pPawn.GetPawnHitPosition(), tempEffectData.EffID_Hit);

                                if (PawnData.GetSkillValue(eUseSkillType) != 0.0f)      //힐.
                                    pPawn.GetHeal_Skill(PawnData, pPawn, eUseSkillType, tempEffectData.EffID_Hit);

                                pPawn.AddBuff(PawnData, eUseSkillType);
                            }
                            break;

                        case 39:                //시끄러움. //모든 소환물 제거.
                            BattleMng.ShowTeamSkillEffect(PawnData.HeroTeam, true, tempEffectData.EffID_Active);

                            for (int idx = 0; idx < arrList_Target.Count; idx++)
                            {
                                BattlePawn pPawn = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);

                                if (pPawn.Pawn_Type == PAWN_TYPE.STUFF)
                                    pPawn.DestroyStuff();
                                else if (pPawn.Pawn_Type == PAWN_TYPE.SUMMONER)
                                    pPawn.DestroyDummyPawn();
                                else
                                {
                                    if (!PawnData.IsDeath() && !PawnData.SpecialActionMode)
                                    {
                                        if (PawnData.GetSkillValue(eUseSkillType) != 0.0f)      //데미지.
                                            pPawn.GetDamage_Skill(PawnData, eUseSkillType, tempEffectData.EffID_Hit);

                                        pPawn.AddBuff(PawnData, eUseSkillType);
                                    }
                                    continue;
                                }

                                BattleMng.pEffectPoolMng.SetBattleEffect(pPawn.GetPawnHitPosition(), tempEffectData.EffID_Hit);
                            }

                            break;

                        default:
                            for (int idx = 0; idx < arrList_Target.Count; idx++)
                            {
                                BattlePawn pPawn = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);

                                if (tempDB_Skill.SKILL_KIND == SKILL_KIND.DAMAGE)
                                {
                                    if (PawnData.GetSkillValue(eUseSkillType) != 0.0f)      //데미지.
                                        pPawn.GetDamage_Skill(PawnData, eUseSkillType, tempEffectData.EffID_Hit);

                                    pPawn.AddBuff(PawnData, eUseSkillType);

                                }
                                else
                                {
                                    if (tempDB_Skill.SKILL_KIND == SKILL_KIND.HEAL)
                                    {
                                        if (PawnData.GetSkillValue(eUseSkillType) != 0.0f)      //힐.
                                            pPawn.GetHeal_Skill(PawnData, pPawn, eUseSkillType, tempEffectData.EffID_Hit);
                                    }

                                    pPawn.AddBuff(PawnData, eUseSkillType);
                                }
                            }
                            break;
                    }

                    break;

                case SKILL_EFFECT_TYPE.TARGET:
                    ThrowSkillObject.GetComponent<TargetMagic>().SetCastingMagicToList(arrList_Target, eUseSkillType);
                    break;

                case SKILL_EFFECT_TYPE.BULLET_NONETARGET:
                    if(eUseSkillType == SkillType.Active)
                        ThrowSkillObject.GetComponent<ThrowObject>().SetThrow(false, PawnData.GetPawnPosition_Center(), null, eUseSkillType);
                    else
                        ThrowSkillObject_Max.GetComponent<ThrowObject>().SetThrow(false, PawnData.GetPawnPosition_Center(), null, eUseSkillType);

                    PawnData.ShowGunFireEffect();

                    break;

                case SKILL_EFFECT_TYPE.BULLET_HOMING_MAKE:
                    BattlePawn pHomingMakeTargetPawn = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[0]);
                    if (pHomingMakeTargetPawn != null)
                    {
                        if (eUseSkillType == SkillType.Active)
                            ThrowSkillObject.GetComponent<ThrowObject>().SetThrow(true, PawnData.GetPawnThrowPosition(), pHomingMakeTargetPawn, eUseSkillType);
                        else
                            ThrowSkillObject_Max.GetComponent<ThrowObject>().SetThrow(true, PawnData.GetPawnThrowPosition(), pHomingMakeTargetPawn, eUseSkillType);
                    }
                    break;

                case SKILL_EFFECT_TYPE.BULLET_HOMING_NORMAL:
                    MakeCount = 0;
                    TempSkillObjList = null;
                    if(eUseSkillType == SkillType.Active)
                        TempSkillObjList = SummonSkillObject;
                    else
                        TempSkillObjList = SummonSkillObject_Max;

                    switch (tempDB_Skill.Index)
                    {
                        case 19:
                        case 1003:
                            for (int idx = 0; idx < arrList_Target.Count; idx++)
                            {
                                if (idx >= TempSkillObjList.Length)
                                    break;

                                for (int sub = 0; sub < TempSkillObjList.Length; sub++)
                                {
                                    if (TempSkillObjList[sub].GetComponent<TargetAreaDrop>().ActiveMode == false)
                                    {
                                        BattlePawn pHomingTarget = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);
                                        TempSkillObjList[sub].GetComponent<TargetAreaDrop>().SetMakeHoming(pHomingTarget, eUseSkillType);
                                        MakeCount++;
                                        break;
                                    }
                                }

                                if (MakeCount >= tempDB_Skill.MakeCount)
                                    break;
                            }
                            break;

                        case 25:
                            if (RandomActive_Success)
                            {
                                for (int idx = 0; idx < arrList_Target.Count; idx++)
                                {
                                    if (idx >= TempSkillObjList.Length)
                                        break;

                                    for (int sub = 0; sub < TempSkillObjList.Length; sub++)
                                    {
                                        if (TempSkillObjList[sub].GetComponent<TargetAreaDrop>().ActiveMode == false)
                                        {
                                            BattlePawn pHomingTarget = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[idx]);
                                            TempSkillObjList[sub].GetComponent<TargetAreaDrop>().SetMakeHoming(pHomingTarget, eUseSkillType);
                                            MakeCount++;
                                            break;
                                        }
                                    }

                                    if (MakeCount >= tempDB_Skill.MakeCount)
                                        break;
                                }
                            }
                            break;
                    }
                    break;


                case SKILL_EFFECT_TYPE.SUMMON_HOMING:
                    BattlePawn pStuffHomingTarget = GetBattlePawnOnKey(tempPawnList, (int)arrList_Target[0]);
                    if (pStuffHomingTarget)
                    {
                        BattlePawn pStuffObj = null;
                        if(eUseSkillType == SkillType.Active)
                        {
                            pStuffObj = SummonStuffObject.GetComponent<BattlePawn>();
                            SummonStuffObject.SetActive(true);

                            if(SummonStuffObject_Max != null && SummonStuffObject_Max.activeInHierarchy)
                                SummonStuffObject_Max.GetComponent<BattlePawn>().DestroyStuff();
                        }
                        else
                        {
                            pStuffObj = SummonStuffObject_Max.GetComponent<BattlePawn>();
                            SummonStuffObject_Max.SetActive(true);

                            if(SummonStuffObject != null && SummonStuffObject.activeInHierarchy)
                                SummonStuffObject.GetComponent<BattlePawn>().DestroyStuff();
                        }

                        Vector3 SummonBasePos = new Vector3(PawnData.transform.position.x, PawnData.FirstPos_Y, PawnData.FirstPos_Z);
                        pStuffObj.ResetStuff(PawnData, SummonBasePos, tempDB_Skill, eUseSkillType, pStuffHomingTarget);
                    }
                    break;

                case SKILL_EFFECT_TYPE.SUMMON_GROUND:
                    switch (tempDB_Skill.Index)
                    {
                        case 4: //상어소환-골드맨.
                        case 10: //분신소환-돌격닌자.
                        case 38:
                            AddSummonDummyPawn(eUseSkillType);
                            break;

                        case 24:
                            BattlePawn pStuffObj = null;
                            if (eUseSkillType == SkillType.Active)
                            {
                                pStuffObj = SummonStuffObject.GetComponent<BattlePawn>();
                                SummonStuffObject.SetActive(true);

                                if (SummonStuffObject_Max != null && SummonStuffObject_Max.activeInHierarchy)
                                    SummonStuffObject_Max.GetComponent<BattlePawn>().DestroyStuff();
                            }
                            else
                            {
                                pStuffObj = SummonStuffObject_Max.GetComponent<BattlePawn>();
                                SummonStuffObject_Max.SetActive(true);

                                if (SummonStuffObject != null && SummonStuffObject.activeInHierarchy)
                                    SummonStuffObject.GetComponent<BattlePawn>().DestroyStuff();
                            }
                            Vector3 SummonBasePos = new Vector3(PawnData.transform.position.x, PawnData.FirstPos_Y, PawnData.FirstPos_Z);
                            pStuffObj.ResetStuff(PawnData, SummonBasePos + ((Vector3.right * tempDB_Skill.MakeLength) * PawnData.MoveDirection), tempDB_Skill, eUseSkillType);
                            break;
                    }
                    break;

                case SKILL_EFFECT_TYPE.SUMMON_SKY:
                    MakeCount = 0;
                    if(eUseSkillType == SkillType.Active)
                        TempSkillObjList = SummonSkillObject;
                    else
                        TempSkillObjList = SummonSkillObject_Max;

                    switch (tempDB_Skill.Index)
                    {
                        case 14:
                        case 22:
                            for (int idx = 0; idx < TempSkillObjList.Length; idx++)
                            {
                                if (TempSkillObjList[idx].GetComponent<TargetAreaDrop>().ActiveMode)
                                    continue;
                                else
                                {
                                    BattlePawn pTarget = PawnData.SkillManager.FindClassTarget_Healer(false);
                                    if (pTarget != null)
                                    {
                                        Vector3 MakePos = new Vector3(pTarget.transform.position.x, 0.0f, 0.0f);
                                        TempSkillObjList[idx].GetComponent<TargetAreaDrop>().SetMake(MakePos, eUseSkillType);
                                    }
                                    break;
                                }
                            }
                            break;

                        case 30:
                            for (int idx = 0; idx < TempSkillObjList.Length; idx++)
                            {
                                if (TempSkillObjList[idx].GetComponent<TargetAreaDrop>().ActiveMode == false)
                                {
                                    Vector3 SummonTimeBombPos = new Vector3(PawnData.transform.position.x + (tempDB_Skill.MakeLength * PawnData.MoveDirection), 0.0f, 0.0f);
                                    TempSkillObjList[idx].GetComponent<TargetAreaDrop>().SetMakeTimeBomb(SummonTimeBombPos, eUseSkillType, 3.0f);
                                    MakeCount++;
                                }

                                if (MakeCount >= tempDB_Skill.MakeCount)
                                    break;
                            }
                            break;

                        default:
                            for (int idx = 0; idx < TempSkillObjList.Length; idx++)
                            {
                                if (TempSkillObjList[idx].GetComponent<TargetAreaDrop>().ActiveMode == false)
                                {
                                    Vector3 MakePos = PawnData.PawnTransform.position + new Vector3(PawnData.MoveDirection * ((idx + 1) * tempDB_Skill.MakeLength), 0.0f, 0.0f);
                                    TempSkillObjList[idx].GetComponent<TargetAreaDrop>().SetMake(MakePos, eUseSkillType);
                                    MakeCount++;
                                }

                                if (MakeCount >= tempDB_Skill.MakeCount)
                                    break;
                            }
                            break;
                    }
                    break;

            }


        }

    }




    //리스트에 살아있는 폰이 몇마리인가?
    public int GetLivePawnCount(List<BattlePawn> tempList)
    {
        int nLiveCount = 0;
        for (int idx = 0; idx < tempList.Count; idx++)
        {
            if(tempList[idx].IsDeath())
                continue;

            nLiveCount++;
        }

        return nLiveCount;
    }


    //이미 리스트에 추가되었는가?
    public bool IsAlreadyAddList(int Key)
    {
        for (int idx = 0; idx < arrList_Target.Count; idx++)
        {
            if ((int)arrList_Target[idx] == Key)
                return true;
        }

        return false;
    }

    public BattlePawn GetBattlePawnOnKey(List<BattlePawn> pTargetList, int Key)
    {
        for (int idx = 0; idx < pTargetList.Count; idx++)
        {
            if (pTargetList[idx].GetBattlePawnKey() == Key)
                return pTargetList[idx];
        }

        return null;
    }







    //외부 체크용.
    //직업군 별 타겟 받아오기.
    public BattlePawn FindClassTarget_Healer(bool FindMyTeam)
    {
        List<BattlePawn> tempPawnList = null;
        BattlePawn  returnPawnData = null;

        if(FindMyTeam)
        {
            if (PawnData.HeroTeam)
                tempPawnList = BattleMng.HeroPawnList;
            else
                tempPawnList = BattleMng.EnemyPawnList;
        }
        else
        {
            if (PawnData.HeroTeam)
                tempPawnList = BattleMng.EnemyPawnList;
            else
                tempPawnList = BattleMng.HeroPawnList;
        }

        for (int idx = 0; idx < 5; idx++)
        {
            for (int sub = 0; sub < tempPawnList.Count; sub++)
            {
                if (tempPawnList[sub].IsDeath() || IsAlreadyAddList(tempPawnList[sub].GetBattlePawnKey()))
                    continue;

                if (tempPawnList[sub].Pawn_Type == PAWN_TYPE.STUFF)
                    continue;

                if (eClassArray_Healer[idx] == tempPawnList[sub].GetClassType())
                {
                    returnPawnData = tempPawnList[sub];
                    break;
                }
            }
        }

        return returnPawnData;
    }






    public DB_Skill.Schema GetDB_Skill(SkillType eType)
    {
        switch (eType)
        {
            case SkillType.Leader:
                return DB_LeaderSkill;

            case SkillType.Active:
                return DB_ActiveSkill;

            case SkillType.Max:
                return DB_MaxSkill;

            default:
                return null;
        }
    }

    public SkillEffectData GetSkillEffectData(SkillType eType)
    {
        switch (eType)
        {
            case SkillType.Leader:
                return SkillEffData_Leader;

            case SkillType.Active:
                return SkillEffData_Active;

            case SkillType.Max:
                return SkillEffData_Max;

            default:
                return null;
        }
    }






    //소환.
    public void AddSummonDummyPawn(SkillType eSkillType)
    {
        for (int idx = 0; idx < GetDB_Skill(eSkillType).MakeCount; idx++)
        {
            GameObject SummonDummyPawnObj = GameObject.Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
            BattlePawn SummonDummyPawn = SummonDummyPawnObj.GetComponent<BattlePawn>();

            DB_Skill.Schema TempDBSkill = GetDB_Skill(eSkillType);

            SummonDummyPawn.InitDummyPawn(PawnData, eSkillType, TempDBSkill.ActiveCheckValue);

            Vector3 DummySummonPos = new Vector3(PawnData.transform.position.x, PawnData.FirstPos_Y, PawnData.FirstPos_Z);
            SummonDummyPawn.ResetDummyPawn(PawnData, DummySummonPos + ((Vector3.right * (TempDBSkill.MakeLength + (0.3f * idx))) * PawnData.MoveDirection), eSkillType);
        }

        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SUMMON);
    }





    public int GetBuffIndex(DB_Skill.Schema SkillData, BUFF_KIND eCheckKind)
    {
        DB_Buff.Schema TempData;
        TempData = DB_Buff.Query(DB_Buff.Field.Index, SkillData.BuffNumber_1);
        if (TempData.BUFF_KIND == eCheckKind)
            return SkillData.BuffNumber_1;

        TempData = DB_Buff.Query(DB_Buff.Field.Index, SkillData.BuffNumber_2);
        if (TempData.BUFF_KIND == eCheckKind)
            return SkillData.BuffNumber_2;

        return -1;
    }



}
















public class SkillEffectData
{
    public int EffID_Active;
    public int EffID_Hit;

    public int EffID_Buff1_Active;
    public int EffID_Buff1_Hit;

    public int EffID_Buff2_Active;
    public int EffID_Buff2_Hit;

    public int[] SubUseEffect;      //추가 사용시 쓰는 이펙트 번호.(강제 적용).

    public GameObject   Effect_ActivePrefab;


    public void ResetSkillEffectData()
    {
        EffID_Active = -1;
        EffID_Hit = -1;

        EffID_Buff1_Active = -1;
        EffID_Buff1_Hit = -1;

        EffID_Buff2_Active = -1;
        EffID_Buff2_Hit = -1;


        if (SubUseEffect != null)
        {
            for (int idx = 0; idx < SubUseEffect.Length; idx++)
            {
                SubUseEffect[idx] = -1;
            }
        }

    }

    public void SetSkillEffectData(EffectPoolManager pEffectPoolMng, DB_Skill.Schema pSkillData)
    {
        ResetSkillEffectData();

        if (pSkillData == null)
            return;

        if (!IsEffectStringNone(pSkillData.Effect_Active))
        {
            switch (pSkillData.SKILL_EFFECT_TYPE)
            {
                case SKILL_EFFECT_TYPE.BULLET_NONETARGET:
                case SKILL_EFFECT_TYPE.BULLET_HOMING_NORMAL:
                case SKILL_EFFECT_TYPE.BULLET_HOMING_MAKE:
                case SKILL_EFFECT_TYPE.SUMMON_SKY:
                    Effect_ActivePrefab = AddSkillEffToPrefab(pSkillData.Effect_Active);
                    EffID_Active = -1;
                    break;

                default:
                    EffID_Active = AddSkillEffToEffectPool(pEffectPoolMng, pSkillData.Effect_Active, 1);
                    Effect_ActivePrefab = null;
                    break;

            }
        }

        if (!IsEffectStringNone(pSkillData.Effect_Hit))
            EffID_Hit = AddSkillEffToEffectPool(pEffectPoolMng, pSkillData.Effect_Hit, 6);

        //버프1.
        if(pSkillData.BuffNumber_1 > 0)
        {
            DB_Buff.Schema BuffData_1 = DB_Buff.Query(DB_Buff.Field.Index, pSkillData.BuffNumber_1);
            if (BuffData_1 != null)
            {
                if (!IsEffectStringNone(BuffData_1.ACTIVE_EFFECT))
                    EffID_Buff1_Active = AddSkillEffToEffectPool(pEffectPoolMng, BuffData_1.ACTIVE_EFFECT, 6);

                if (!IsEffectStringNone(BuffData_1.HIT_EFFECT))
                    EffID_Buff1_Hit = AddSkillEffToEffectPool(pEffectPoolMng, BuffData_1.HIT_EFFECT, 6);
            }
        }

        //버프2.
        if(pSkillData.BuffNumber_2 > 0)
        {
            DB_Buff.Schema BuffData_2 = DB_Buff.Query(DB_Buff.Field.Index, pSkillData.BuffNumber_2);
            if (BuffData_2 != null)
            {
                if (!IsEffectStringNone(BuffData_2.ACTIVE_EFFECT))
                    EffID_Buff2_Active = AddSkillEffToEffectPool(pEffectPoolMng, BuffData_2.ACTIVE_EFFECT, 6);

                if (!IsEffectStringNone(BuffData_2.HIT_EFFECT))
                    EffID_Buff2_Hit = AddSkillEffToEffectPool(pEffectPoolMng, BuffData_2.HIT_EFFECT, 6);
            }
        }


        //추가 사용 이펙트.
        switch (pSkillData.Index)
        {
            case 25:
                if (pSkillData.SkillType == SkillType.Active)
                {
                    SubUseEffect = new int[2];
                    SubUseEffect[0] = AddSkillEffToEffectPool(pEffectPoolMng, "EF_LuckySkill_Sign_Success", 1);
                    SubUseEffect[1] = AddSkillEffToEffectPool(pEffectPoolMng, "EF_LuckySkill_Sign_Failed", 1);
                }
                break;
        }

    }




    public bool IsEffectStringNone(string szEffName)
    {
        if (szEffName.Equals("empty") || szEffName.Length <= 0 || szEffName.Equals("NONE"))
            return true;

        return false;
    }

    public int AddSkillEffToEffectPool(EffectPoolManager pEffectPoolMng, string szEffName, int nAddCount)
    {
        GameObject pEffActive = Resources.Load("Effects/" + szEffName) as GameObject;
        if (pEffActive == null)
        {
            Debug.Log("None Effect!! : " + szEffName);
            return -1;
        }

        return pEffectPoolMng.AddEffectPool(pEffActive, false, nAddCount+1);
    }


    public GameObject AddSkillEffToPrefab(string szEffName)
    {
        GameObject pEffActive = Resources.Load("Effects/" + szEffName) as GameObject;
        pEffActive.transform.localPosition = Vector3.zero;
        if (pEffActive == null)
        {
            Debug.Log("None Effect!! : " + szEffName);
            return null;
        }
        return pEffActive;
    }

}