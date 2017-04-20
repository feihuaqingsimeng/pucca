using System;
using UnityEngine;

public class DB_Skill : TableBase<DB_Skill, DB_Skill.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public SkillType SkillType;
		public SKILL_KIND SKILL_KIND;
		public int Cost;
		public float CoolTime;
		public float BaseValue;
		public float LvAddValue;
		public int BuffNumber_1;
		public int BuffNumber_2;
		public SKILLACTIVE_ACTION SKILLACTIVE_ACTION;
		public SKILLACTIVE_VALUETYPE SKILLACTIVE_VALUETYPE;
		public float ActiveCheckValue;
		public bool RangeCheck;
		public int AreaPawnCount;
		public float AreaRange_Min;
		public float AreaRange_Max;
		public SKILL_ALLY_TYPE SKILL_ALLY_TYPE;
		public SKILL_TARGET_TYPE SKILL_TARGET_TYPE;
		public int HitCount;
		public SKILL_EFFECT_TYPE SKILL_EFFECT_TYPE;
		public int MakeCount;
		public float MakeLength;
		public string AnimationKey;
		public string Effect_Active;
		public string Effect_Hit;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string SkillType = "SkillType";
		public static string SKILL_KIND = "SKILL_KIND";
		public static string Cost = "Cost";
		public static string CoolTime = "CoolTime";
		public static string BaseValue = "BaseValue";
		public static string LvAddValue = "LvAddValue";
		public static string BuffNumber_1 = "BuffNumber_1";
		public static string BuffNumber_2 = "BuffNumber_2";
		public static string SKILLACTIVE_ACTION = "SKILLACTIVE_ACTION";
		public static string SKILLACTIVE_VALUETYPE = "SKILLACTIVE_VALUETYPE";
		public static string ActiveCheckValue = "ActiveCheckValue";
		public static string RangeCheck = "RangeCheck";
		public static string AreaPawnCount = "AreaPawnCount";
		public static string AreaRange_Min = "AreaRange_Min";
		public static string AreaRange_Max = "AreaRange_Max";
		public static string SKILL_ALLY_TYPE = "SKILL_ALLY_TYPE";
		public static string SKILL_TARGET_TYPE = "SKILL_TARGET_TYPE";
		public static string HitCount = "HitCount";
		public static string SKILL_EFFECT_TYPE = "SKILL_EFFECT_TYPE";
		public static string MakeCount = "MakeCount";
		public static string MakeLength = "MakeLength";
		public static string AnimationKey = "AnimationKey";
		public static string Effect_Active = "Effect_Active";
		public static string Effect_Hit = "Effect_Hit";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SkillScriptableObject");
			if (asset != null)
			{
				DB_SkillScriptableObject scriptableObject = asset as DB_SkillScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}

	public override bool FromAssetBundle(AssetBundle assetBundle, string assetName)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset(assetName);

			if (asset != null)
			{
				DB_SkillScriptableObject scriptableObject = asset as DB_SkillScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
