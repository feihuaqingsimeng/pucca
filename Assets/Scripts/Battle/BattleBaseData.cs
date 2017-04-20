using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleBaseData : MonoBehaviour
{
    [HideInInspector]
    public  DB_Card.Schema      DBData_Base;

    [HideInInspector]
    public  List<BuffElement>   BuffList = new List<BuffElement>();


    public  int     CardIndex;          //카드번호.

    public  PAWN_TYPE   Pawn_Type;


    public  int     Level;              //레벨.
    public  int     SkillLevel;         //스킬레벨.

    public  int     EquipLevel_Acc;
    public  int     EquipLevel_Weapon;
    public  int     EquipLevel_Armor;

    public  int     CurHP;
    public  int     BaseHP;
    public  int     EquipHP;
    public  int     MaxHP;

    public  float   CurAP;
    public  int     BaseAP;
    public  int     EquipAP;

    public  float   CurDP;
    public  int     BaseDP;
    public  int     EquipDP;
    
    public  float   CurSkillValue;
    public  float   CurSkillValue_Max;

    public  float   Accurate;
    public  float   BaseAccurate;
    public  float   Evade;
    public  float   BaseEvade;
    public  float   CriticalRate;
    public  float   BaseCriticalRate;
    public  float   CriticalDamage;
    public  float   BaseCriticalDamage;

    public  float   MoveSpeed;
    public  float   BaseMoveSpeed;
    public  float   TankerPushSpeed;
    public  float   AttackSpeed;
    public  float   BaseAttackSpeed;
    public  float   AttackRange;
    public  float   BaseAttackRange;
    public  float   PushRange;
    public  float   BasePushRange;
    public  float   PushedRange;
    public  float   BasePushedRange;

    public  float   AnimationSpeed;

    public  float   ScaleSize;

    public  int     SkillCost;
    public  int     SkillCost_Max;


    //개별쿨타임.
    public  float   SkillCoolTime_Cur;
    public  float   SkillCoolTime_Max;



    public void InitBattleBaseData(int nCardIndex, int nLevel, int nEquipLevel_Acc, int nEquipLevel_Weapon, int nEquipLevel_Armor, int nSkillLevel, float fStat_Compensate, bool bHero)
    {
        CardIndex = nCardIndex;
        if (CardIndex == -1)
            return;

        DBData_Base = DB_Card.Query(DB_Card.Field.Index, CardIndex);

        Level = nLevel;
        EquipLevel_Acc = nEquipLevel_Acc;
        EquipLevel_Weapon = nEquipLevel_Weapon;
        EquipLevel_Armor = nEquipLevel_Armor;
        SkillLevel = nSkillLevel;

        ScaleSize = DBData_Base.SizeRate;


        //체력.
        Settings.Card.TryGetStat(nCardIndex, (byte)nLevel, out BaseHP, out BaseAP, out BaseDP); // [WARNING] Excpt!
        Settings.Equipment.TryGetStat(nCardIndex, (byte)EquipLevel_Acc, (byte)EquipLevel_Weapon, (byte)EquipLevel_Armor, out EquipHP, out EquipAP, out EquipDP);

        CurHP = MaxHP = BaseHP + EquipHP;

        //공격력.
        CurAP = BaseAP + EquipAP;

        //방어력.
        CurDP = BaseDP + EquipDP;


        if (!bHero && Kernel.entry.battle.CurBattleKind == BATTLE_KIND.PVP_BATTLE && Kernel.entry.account.currentPvPArea <= 2)
        {
            switch(Kernel.entry.account.currentPvPArea)
            {
                case 1:
                    CurHP = MaxHP = (int)((float)MaxHP * 0.6f);
                    CurAP *= 0.8f;
                    CurDP *= 0.8f;
                    break;

                case 2:
                    CurHP = MaxHP = (int)((float)MaxHP * 0.8f);
                    CurAP *= 0.9f;
                    CurDP *= 0.9f;
                    break;

                default:
                    CurHP = MaxHP = (int)((float)MaxHP * fStat_Compensate);
                    CurAP *= fStat_Compensate;
                    CurDP *= fStat_Compensate;
                    break;
            }
        }
        else
        {
            CurHP = MaxHP = (int)((float)MaxHP * fStat_Compensate);
            CurAP *= fStat_Compensate;
            CurDP *= fStat_Compensate;
        }

        //기타 스킬.
        Accurate = BaseAccurate = DBData_Base.Accurate;
        Evade = BaseEvade = DBData_Base.Evade;
        CriticalRate = BaseCriticalRate = DBData_Base.Cr_Rate;
        CriticalDamage = BaseCriticalDamage = DBData_Base.Cr_Dmg;

        MoveSpeed = TankerPushSpeed = BaseMoveSpeed = DBData_Base.Speed_Move;
        AttackSpeed = BaseAttackSpeed = DBData_Base.Speed_Atk;
        if (AttackSpeed <= 0)
            AttackSpeed = BaseAttackSpeed = 0.6f;

        AttackRange = BaseAttackRange = DBData_Base.Range_Atk;
        if (AttackRange <= 0)
            AttackRange = BaseAttackRange = 1.0f;

        PushRange = BasePushRange = DBData_Base.Range_Push;
        PushedRange = BasePushedRange = DBData_Base.Range_Pushed;


        //스킬코스트.
        DB_Skill.Schema SkillData = DB_Skill.Query(DB_Skill.Field.Index, CardIndex, DB_Skill.Field.SkillType, SkillType.Active);
        SkillCost = SkillData.Cost;
        CurSkillValue = SkillData.BaseValue + (SkillData.LvAddValue * SkillLevel);

        DB_Skill.Schema SkillMaxData = DB_Skill.Query(DB_Skill.Field.Index, CardIndex, DB_Skill.Field.SkillType, SkillType.Max);
        SkillCost_Max = SkillMaxData.Cost;
        CurSkillValue_Max = SkillMaxData.BaseValue + (SkillMaxData.LvAddValue * SkillLevel);


        //버프리스트. 10개정도만 미리 생성해두자.
        for (int idx = 0; idx < 10; idx++)
        {
            BuffElement pTemp = new BuffElement();
            pTemp.InitBuffElement();
            BuffList.Add(pTemp);
        }

        SkillCoolTime_Max = SkillData.CoolTime;
        SkillCoolTime_Cur = SkillCoolTime_Max;
    }





    public ClassType    GetClassType()
    {
        if (Pawn_Type == PAWN_TYPE.STUFF)
            return ClassType.ClassType_Keeper;
        if( Pawn_Type == PAWN_TYPE.SUMMONER)
            return ClassType.ClassType_Hitter;

        return DBData_Base.ClassType;
    }

    public Grade_Type GetGradeType()
    {
        return DBData_Base.Grade_Type;
    }


    public bool IsFarAttack_Normal()
    {
        if (DBData_Base.ClassType == ClassType.ClassType_Ranger || DBData_Base.ClassType == ClassType.ClassType_Wizard)
            return true;

        return false;
    }

    public bool IsHealer()
    {
        if (DBData_Base.ClassType == ClassType.ClassType_Healer)
            return true;

        return false;
    }


    public float GetSkillValue(SkillType eSkillType)
    {
        if (eSkillType == SkillType.Max)
            return CurSkillValue_Max;
        else
            return CurSkillValue;
    }





    //버프 리스트에 해당 버프가 이미 있는지 확인.
    public BuffElement GetOverlapBuff(int AttackerKey, int BuffIndex)
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            if (BuffList[idx].LeaderSkill)
                continue;

            if (BuffList[idx].Attacker_Key != AttackerKey)
                continue;

            if (BuffList[idx].Index == BuffIndex)
                return BuffList[idx];
        }
        return null;
    }


    //버프리스트 비어있는거 받아오고 없으면 추가생성.
    public BuffElement GetEmptyBuffElement()
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (BuffList[idx].BuffActive)
                continue;

            return BuffList[idx];
        }

        //없으면 여기까지 온다.
        BuffElement pTemp = new BuffElement();
        pTemp.InitBuffElement();
        BuffList.Add(pTemp);

        return pTemp;
    }



    public  bool        SilenceMode;
    public  bool        AirborneMode;
    public  bool        PreAirborneMode;
    public  bool        HardTankingMode;
    public  float       HardTinkingLeftTime;

    public  bool        FreezeMode;
    [HideInInspector]
    public  bool        PreFreezeMode;
    
    public  bool        StunMode;
    [HideInInspector]
    public  bool        PreStunMode;
    
    public  bool        ReflectDmgMode;
    public  float       ReflectAddValue;
    
    public  bool        ChargingUpMode;
    public  float       ChargingAddValue;

    public  bool        SkillShieldMode;
    public  float       SkillShieldValue;



    //갱신.
    public void BuffValueUpdate(ref bool GoodBuff, ref bool BadBuff)
    {
        SilenceMode = false;
        PreAirborneMode = AirborneMode;
        AirborneMode = false;
        HardTankingMode = false;
        HardTinkingLeftTime = 0.0f;
        FreezeMode = false;
        StunMode = false;
        ChargingUpMode = false;
        ChargingAddValue = 0;
        SkillShieldMode = false;
        ReflectDmgMode = false;
        ReflectAddValue = 0;

        CurAP = BaseAP + EquipAP;
        CurDP = BaseDP + EquipDP;
        MoveSpeed = BaseMoveSpeed;
        AttackSpeed = BaseAttackSpeed;
        Accurate = BaseAccurate;
        Evade = BaseEvade;
        CriticalRate = BaseCriticalRate;
        CriticalDamage = BaseCriticalDamage;

        bool SpeedCheck = false;
        float SpeedValue_Fast = 0.0f;
        float SpeedValue_Slow = 0.0f;


        bool IsActive_BuffGood = false;
        bool IsActive_BuffBad = false;

        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive || BuffList[idx].BuffBaseData == null)
                continue;

            float BuffValue = BuffList[idx].CurValue;

            switch (BuffList[idx].BuffBaseData.BUFF_KIND)
            {
                case BUFF_KIND.SILENCE:
                    SilenceMode = true;
                    break;
                case BUFF_KIND.AIRBORN:
                    AirborneMode = true;
                    break;
                case BUFF_KIND.FREEZE:
                    FreezeMode = true;
                    PreFreezeMode = true;
                    break;
                case BUFF_KIND.STUN:
                    StunMode = true;
                    PreStunMode = true;
                    break;
                case BUFF_KIND.ATT_UP:
                case BUFF_KIND.ATT_DOWN:
                case BUFF_KIND.BERSERKER_ATT_UP:
                    CurAP += BaseAP * BuffValue;
                    if (CurAP <= 0.0f)
                        CurAP = 0.0f;
                    break;
                case BUFF_KIND.DEF_UP:
                case BUFF_KIND.DEF_DOWN:
                case BUFF_KIND.BERSERKER_DP_DOWN:
                    CurDP += BaseDP * BuffValue;
                    if (CurDP <= 0.0f)
                        CurDP = 0.0f;
                    break;

                case BUFF_KIND.SLOW:
                    SpeedCheck = true;
                    SpeedValue_Slow += (1.0f - BuffValue);
                    break;

                case BUFF_KIND.FAST:
                    SpeedCheck = true;
                    SpeedValue_Fast += (BuffValue - 1.0f);
                    break;

                case BUFF_KIND.ACCURATE_UP:
                case BUFF_KIND.ACCURATE_DOWN:
                    Accurate = BaseAccurate * BuffValue;
                    break;

                case BUFF_KIND.EVADERATE_UP:
                case BUFF_KIND.EVADERATE_DOWN:
                    Evade = BaseEvade * BuffValue;
                    break;

                case BUFF_KIND.CRITICALRATE_UP:
                case BUFF_KIND.CRITICALRATE_DOWN:
                    CriticalRate += BaseCriticalRate * BuffValue;
                    break;
                
                case BUFF_KIND.CRITICALDMG_UP:
                case BUFF_KIND.CRITICALDMG_DOWN:
                    CriticalDamage += BaseCriticalDamage * BuffValue;
                    break;

                case BUFF_KIND.SKILL_SHIELD:
                    SkillShieldMode = true;
                    SkillShieldValue = 1.0f + BuffValue;  
                    break;

                case BUFF_KIND.HARD_TANKING:
                    HardTankingMode = true;
                    HardTinkingLeftTime = BuffList[idx].MaxLifeTime - BuffList[idx].CurLifeTime;
                    if (HardTinkingLeftTime <= 0.0f)
                        HardTinkingLeftTime = 0.0f;
                    break;

                case BUFF_KIND.CHARGING_UP:
                    ChargingUpMode = true;
                    ChargingAddValue += BuffValue;
                    break;

                case BUFF_KIND.REFLECT:
                    ReflectDmgMode = true;
                    ReflectAddValue = BuffValue;
                    break;
            }

            if (!BuffList[idx].LeaderSkill)
            {
                switch (BuffList[idx].BuffBaseData.BUFF_KIND)
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
                        IsActive_BuffBad = true;
                        break;

                    case BUFF_KIND.HEALING_CONSIST:
                    case BUFF_KIND.TOTEM_HEALING_CONSIST:
                        break;

                    default:
                        IsActive_BuffGood = true;
                        break;
                }
            }
        }

        if (SpeedCheck)
        {
            float SpeedResultValue = SpeedValue_Fast - SpeedValue_Slow;

            if (SpeedResultValue < 0.0f)       //합산값이 0보다 작으면 슬로우.
            {
                if (SpeedResultValue <= -1.0f)     //감소값이 1.0보다 크면
                    SpeedResultValue = 0.1f;
                else
                    SpeedResultValue = 1.0f + SpeedResultValue;
            }
            else
                SpeedResultValue = 1.0f + SpeedResultValue;


            MoveSpeed = BaseMoveSpeed * SpeedResultValue;
            if (MoveSpeed < 0.1f)
                MoveSpeed = 0.1f;

            AttackSpeed = BaseAttackSpeed - (BaseAttackSpeed * (SpeedResultValue - 1.0f));
            if (AttackSpeed < 0.1f)
                AttackSpeed = 0.1f;
        }


        //버프서클 체크.
        GoodBuff = IsActive_BuffGood;
        BadBuff = IsActive_BuffBad;
    }



    public void ClearBuffList()
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            BuffList[idx].BuffActive = false;
        }
    }



}
