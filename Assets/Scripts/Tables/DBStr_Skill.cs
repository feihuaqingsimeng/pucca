using System;
using UnityEngine;

public class DBStr_Skill : TableBase<DBStr_Skill, DBStr_Skill.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Skill_Index;
		public SkillType SkillType;
		public string Skill_Name;
		public string Skill_Desc;
		public string Skill_Tooltip;
		public string BattleText_Top;
		public string BattleText_Bottom;
	}

	public static class Field
	{
		public static string Skill_Index = "Skill_Index";
		public static string SkillType = "SkillType";
		public static string Skill_Name = "Skill_Name";
		public static string Skill_Desc = "Skill_Desc";
		public static string Skill_Tooltip = "Skill_Tooltip";
		public static string BattleText_Top = "BattleText_Top";
		public static string BattleText_Bottom = "BattleText_Bottom";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_SkillScriptableObject");
			if (asset != null)
			{
				DBStr_SkillScriptableObject scriptableObject = asset as DBStr_SkillScriptableObject;
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
				DBStr_SkillScriptableObject scriptableObject = asset as DBStr_SkillScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
