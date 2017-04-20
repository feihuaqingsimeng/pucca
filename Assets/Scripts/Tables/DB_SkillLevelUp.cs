using System;
using UnityEngine;

public class DB_SkillLevelUp : TableBase<DB_SkillLevelUp, DB_SkillLevelUp.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Grade_Type Grade_Type;
		public int Skill_Level;
		public Goods_Type Goods_Type;
		public int Count;
		public int Need_Gold;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Grade_Type = "Grade_Type";
		public static string Skill_Level = "Skill_Level";
		public static string Goods_Type = "Goods_Type";
		public static string Count = "Count";
		public static string Need_Gold = "Need_Gold";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SkillLevelUpScriptableObject");
			if (asset != null)
			{
				DB_SkillLevelUpScriptableObject scriptableObject = asset as DB_SkillLevelUpScriptableObject;
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
				DB_SkillLevelUpScriptableObject scriptableObject = asset as DB_SkillLevelUpScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
