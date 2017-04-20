using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public enum DOCSPIN_TABLE_KIND
{
    Data_DB_Buff = 0,
    Data_DB_CARD,
    Data_DB_PVEMobData,
    Data_DB_Skill,
    Data_DB_StageMob,
    Data_DB_StageReward
}



public class LoadDocspinTable
{
    public static void LoadDocspinTableData(DOCSPIN_TABLE_KIND kind)
    {
        switch(kind)
        {
            case DOCSPIN_TABLE_KIND.Data_DB_Buff:
                LoadData_DB_Buff();
                break;
            
            case DOCSPIN_TABLE_KIND.Data_DB_CARD:
                LoadData_DB_Card();
                break;
            
            case DOCSPIN_TABLE_KIND.Data_DB_PVEMobData:
                LoadData_DB_PVEMobData();
                break;

            case DOCSPIN_TABLE_KIND.Data_DB_Skill:
                LoadData_DB_Skill();
                break;

            case DOCSPIN_TABLE_KIND.Data_DB_StageMob:
                LoadData_DB_StageMob();
                break;

            case DOCSPIN_TABLE_KIND.Data_DB_StageReward:
                LoadData_DB_StageReward();
                break;

        }
    }



    //서브 데이터 로드부분.
    public static void LoadData_DB_Card()
    {
        DocsPin.DocsData pDocsData = DocsPin.DocsRoot.findData("Data_DB_Card");
        if(pDocsData == null)
            return;

        for(int idx = 0; idx < DB_Card.instance.schemaList.Count; idx++)
        {
            int TableIndex = DB_Card.instance.schemaList[idx].Index;
            string TableName = TableIndex.ToString();

            DB_Card.Schema Data_Card = DB_Card.Query(DB_Card.Field.Index, TableIndex);

            Data_Card.Accurate = pDocsData.get<float>(TableName, "Accurate");
            Data_Card.Evade = pDocsData.get<float>(TableName, "Evade");
            Data_Card.Cr_Rate = pDocsData.get<float>(TableName, "Cr_Rate");
            Data_Card.Cr_Dmg = pDocsData.get<float>(TableName, "Cr_Dmg");
            Data_Card.Speed_Move = pDocsData.get<float>(TableName, "Speed_Move");
            Data_Card.Speed_Atk = pDocsData.get<float>(TableName, "Speed_Atk");
            Data_Card.Range_Atk = pDocsData.get<float>(TableName, "Range_Atk");
            Data_Card.Range_Pushed = pDocsData.get<int>(TableName, "Range_Pushed");
            Data_Card.Range_Push = pDocsData.get<int>(TableName, "Range_Push");
            Data_Card.LvBase_Hp = pDocsData.get<int>(TableName, "LvBase_Hp");
            Data_Card.LvBase_Ap = pDocsData.get<int>(TableName, "LvBase_Ap");
            Data_Card.LvBase_Dp = pDocsData.get<int>(TableName, "LvBase_Dp");
            Data_Card.Acc_HP = pDocsData.get<int>(TableName, "Acc_HP");
            Data_Card.Weapon_AP = pDocsData.get<int>(TableName, "Weapon_AP");
            Data_Card.Armor_DP = pDocsData.get<int>(TableName, "Armor_DP");
            Data_Card.SizeRate = pDocsData.get<float>(TableName, "SizeRate");
        }
    }





    public static void LoadData_DB_Buff()
    {
        DocsPin.DocsData pDocsData = DocsPin.DocsRoot.findData("Data_DB_Buff");
        if (pDocsData == null)
            return;

        for (int idx = 0; idx < DB_Buff.instance.schemaList.Count; idx++)
        {
            int TableIndex = DB_Buff.instance.schemaList[idx].Index;
            string TableName = TableIndex.ToString();

            DB_Buff.Schema Data_Buff = DB_Buff.Query(DB_Buff.Field.Index, TableIndex);
            Data_Buff.BUFF_GOOD = GetBoolValue(pDocsData.get<string>(TableName, "BUFF_GOOD"));
            Data_Buff.BUFF_KIND = (BUFF_KIND)GetEnumValue("BUFF_KIND", pDocsData.get<string>(TableName, "BUFF_KIND"));
            Data_Buff.BaseValue = pDocsData.get<float>(TableName, "BaseValue");
            Data_Buff.LvAddValue = pDocsData.get<float>(TableName, "LvAddValue");
            Data_Buff.TickDelay = pDocsData.get<float>(TableName, "TickDelay");
            Data_Buff.MaxTime = pDocsData.get<float>(TableName, "MaxTime");
            Data_Buff.AddMaxTime = pDocsData.get<float>(TableName, "AddMaxTime");
            Data_Buff.Overlap_Check = GetBoolValue(pDocsData.get<string>(TableName, "Overlap_Check"));
            Data_Buff.ACTIVE_EFFECT = pDocsData.get<string>(TableName, "ACTIVE_EFFECT");
            Data_Buff.HIT_EFFECT = pDocsData.get<string>(TableName, "HIT_EFFECT");

        }


    }


    public static void LoadData_DB_PVEMobData()
    {
        DocsPin.DocsData pDocsData = DocsPin.DocsRoot.findData("Data_DB_PVEMobData");
        if (pDocsData == null)
            return;

        for (int idx = 0; idx < DB_PVEMobData.instance.schemaList.Count; idx++)
        {
            int TableIndex = DB_PVEMobData.instance.schemaList[idx].MobIndex;
            string TableName = TableIndex.ToString();

            DB_PVEMobData.Schema Data_PVEMobData = DB_PVEMobData.Query(DB_PVEMobData.Field.MobIndex, TableIndex);
            Data_PVEMobData.Card_Index = pDocsData.get<int>(TableName, "Card_Index");
            Data_PVEMobData.Level_Base = pDocsData.get<int>(TableName, "Level_Base");
            Data_PVEMobData.Level_Skill = pDocsData.get<int>(TableName, "Level_Skill");
            Data_PVEMobData.Level_Weapon = pDocsData.get<int>(TableName, "Level_Weapon");
            Data_PVEMobData.Level_Armor = pDocsData.get<int>(TableName, "Level_Armor");
            Data_PVEMobData.Level_Acc = pDocsData.get<int>(TableName, "Level_Acc");
        }
    }

    public static void LoadData_DB_Skill()
    {
        DocsPin.DocsData pDocsData = DocsPin.DocsRoot.findData("Data_DB_Skill");
        if (pDocsData == null)
            return;

        int MaxDocsCount = pDocsData.getRowCount();

        for (int idx = 0; idx < MaxDocsCount; idx++)
        {
            string TableName = idx.ToString();
            int Skill_Index = pDocsData.get<int>(TableName, "Index");
            SkillType eSkillType = (SkillType)GetEnumValue("SkillType", pDocsData.get<string>(TableName, "SkillType"));

            DB_Skill.Schema Data_Skill = DB_Skill.Query(DB_Skill.Field.Index, Skill_Index, DB_Skill.Field.SkillType, eSkillType);
            Data_Skill.SKILL_KIND = (SKILL_KIND)GetEnumValue("SKILL_KIND", pDocsData.get<string>(TableName, "SKILL_KIND"));
            Data_Skill.Cost = pDocsData.get<int>(TableName, "Cost");
            Data_Skill.BaseValue = pDocsData.get<float>(TableName, "BaseValue");
            Data_Skill.LvAddValue = pDocsData.get<float>(TableName, "LvAddValue");
            Data_Skill.BuffNumber_1 = pDocsData.get<int>(TableName, "BuffNumber_1");
            Data_Skill.BuffNumber_2 = pDocsData.get<int>(TableName, "BuffNumber_2");
            Data_Skill.SKILLACTIVE_ACTION = (SKILLACTIVE_ACTION)GetEnumValue("SKILLACTIVE_ACTION", pDocsData.get<string>(TableName, "SKILLACTIVE_ACTION"));
            Data_Skill.SKILLACTIVE_VALUETYPE = (SKILLACTIVE_VALUETYPE)GetEnumValue("SKILLACTIVE_VALUETYPE", pDocsData.get<string>(TableName, "SKILLACTIVE_VALUETYPE"));
            Data_Skill.ActiveCheckValue = pDocsData.get<float>(TableName, "ActiveCheckValue");
            Data_Skill.RangeCheck = GetBoolValue(pDocsData.get<string>(TableName, "RangeCheck"));
            Data_Skill.AreaPawnCount = pDocsData.get<int>(TableName, "AreaPawnCount");
            Data_Skill.AreaRange_Min = pDocsData.get<float>(TableName, "AreaRange_Min");
            Data_Skill.AreaRange_Max = pDocsData.get<float>(TableName, "AreaRange_Max");
            Data_Skill.SKILL_ALLY_TYPE = (SKILL_ALLY_TYPE)GetEnumValue("SKILL_ALLY_TYPE", pDocsData.get<string>(TableName, "SKILL_ALLY_TYPE"));
            Data_Skill.SKILL_TARGET_TYPE = (SKILL_TARGET_TYPE)GetEnumValue("SKILL_TARGET_TYPE", pDocsData.get<string>(TableName, "SKILL_TARGET_TYPE"));
            Data_Skill.HitCount = pDocsData.get<int>(TableName, "HitCount");
            Data_Skill.SKILL_EFFECT_TYPE = (SKILL_EFFECT_TYPE)GetEnumValue("SKILL_EFFECT_TYPE", pDocsData.get<string>(TableName, "SKILL_EFFECT_TYPE"));
            Data_Skill.MakeCount = pDocsData.get<int>(TableName, "MakeCount");
            Data_Skill.MakeLength = pDocsData.get<float>(TableName, "MakeLength");
            Data_Skill.AnimationKey = pDocsData.get<string>(TableName, "AnimationKey");
            Data_Skill.Effect_Active = pDocsData.get<string>(TableName, "Effect_Active");
            Data_Skill.Effect_Hit = pDocsData.get<string>(TableName, "Effect_Hit");

        }
    }


    public static void LoadData_DB_StageMob()
    {
        DocsPin.DocsData pDocsData = DocsPin.DocsRoot.findData("Data_DB_StageMob");
        if (pDocsData == null)
            return;

        for (int idx = 0; idx < DB_StageMob.instance.schemaList.Count; idx++)
        {
            int TableIndex = DB_StageMob.instance.schemaList[idx].GroupIndex;
            string TableName = TableIndex.ToString();

            DB_StageMob.Schema Data_StageMob = DB_StageMob.Query(DB_StageMob.Field.GroupIndex, TableIndex);
            Data_StageMob.NextGroup = pDocsData.get<int>(TableName, "NextGroup");
            Data_StageMob.SpawnDelay = pDocsData.get<float>(TableName, "SpawnDelay");
            Data_StageMob.StatRevision = pDocsData.get<float>(TableName, "StatRevision");
            Data_StageMob.LeaderIndex = pDocsData.get<int>(TableName, "LeaderIndex");
            Data_StageMob.BossIndex = pDocsData.get<int>(TableName, "BossIndex");
            Data_StageMob.MobIndex_1 = pDocsData.get<int>(TableName, "MobIndex_1");
            Data_StageMob.MobIndex_2 = pDocsData.get<int>(TableName, "MobIndex_2");
            Data_StageMob.MobIndex_3 = pDocsData.get<int>(TableName, "MobIndex_3");
            Data_StageMob.MobIndex_4 = pDocsData.get<int>(TableName, "MobIndex_4");
            Data_StageMob.MobIndex_5 = pDocsData.get<int>(TableName, "MobIndex_5");

        }
    }


    public static void LoadData_DB_StageReward()
    {
        DocsPin.DocsData pDocsData = DocsPin.DocsRoot.findData("Data_DB_StageReward");
        if (pDocsData == null)
            return;

        for (int idx = 0; idx < DB_StageReward.instance.schemaList.Count; idx++)
        {
            int TableIndex = DB_StageReward.instance.schemaList[idx].Index;
            string TableName = TableIndex.ToString();

            DB_StageReward.Schema Data_StageReward = DB_StageReward.Query(DB_StageReward.Field.Index, TableIndex);
            Data_StageReward.Reward_Group = pDocsData.get<int>(TableName, "Reward_Group");
            Data_StageReward.Reward_Slot = pDocsData.get<int>(TableName, "Reward_Slot");
            Data_StageReward.Goods_Type = (Goods_Type)GetEnumValue("Goods_Type", pDocsData.get<string>(TableName, "Goods_Type"));
            Data_StageReward.Count_Min = pDocsData.get<int>(TableName, "Count_Min");
            Data_StageReward.Count_Max = pDocsData.get<int>(TableName, "Count_Max");
            Data_StageReward.FixDropProb = pDocsData.get<float>(TableName, "FixDropProb");

        }

    }














    public static int GetEnumValue(string strEnumKind, string strEnumValue)
    {
        if (strEnumValue == null)
            return 0;

        Type EnumType = Type.GetType(strEnumKind);
        return (int)System.Enum.Parse(EnumType, strEnumValue);
    }

    public static bool GetBoolValue(string strBoolData)
    {
        if (strBoolData == null)
            return false;

        return (bool)System.Boolean.Parse(strBoolData);
    }


}
