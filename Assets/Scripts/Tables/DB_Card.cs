using System;
using UnityEngine;

public class DB_Card : TableBase<DB_Card, DB_Card.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string IdentificationName;
		public Grade_Type Grade_Type;
		public ClassType ClassType;
		public int Drop_Area;
		public AIType AIType;
		public float Accurate;
		public float Evade;
		public float Cr_Rate;
		public float Cr_Dmg;
		public float Speed_Move;
		public float Speed_Atk;
		public float Range_Atk;
		public int Range_Pushed;
		public int Range_Push;
		public int LvBase_Hp;
		public int LvBase_Ap;
		public int LvBase_Dp;
		public int Acc_HP;
		public int Weapon_AP;
		public int Armor_DP;
		public float SizeRate;
		public float BP_SkillValue;
		public float BP_SkillLvValue;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string IdentificationName = "IdentificationName";
		public static string Grade_Type = "Grade_Type";
		public static string ClassType = "ClassType";
		public static string Drop_Area = "Drop_Area";
		public static string AIType = "AIType";
		public static string Accurate = "Accurate";
		public static string Evade = "Evade";
		public static string Cr_Rate = "Cr_Rate";
		public static string Cr_Dmg = "Cr_Dmg";
		public static string Speed_Move = "Speed_Move";
		public static string Speed_Atk = "Speed_Atk";
		public static string Range_Atk = "Range_Atk";
		public static string Range_Pushed = "Range_Pushed";
		public static string Range_Push = "Range_Push";
		public static string LvBase_Hp = "LvBase_Hp";
		public static string LvBase_Ap = "LvBase_Ap";
		public static string LvBase_Dp = "LvBase_Dp";
		public static string Acc_HP = "Acc_HP";
		public static string Weapon_AP = "Weapon_AP";
		public static string Armor_DP = "Armor_DP";
		public static string SizeRate = "SizeRate";
		public static string BP_SkillValue = "BP_SkillValue";
		public static string BP_SkillLvValue = "BP_SkillLvValue";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_CardScriptableObject");
			if (asset != null)
			{
				DB_CardScriptableObject scriptableObject = asset as DB_CardScriptableObject;
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
				DB_CardScriptableObject scriptableObject = asset as DB_CardScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
