using System;
using UnityEngine;

public class DB_PVEMobData : TableBase<DB_PVEMobData, DB_PVEMobData.Schema>
{
	[Serializable]
	public class Schema
	{
		public int MobIndex;
		public int Card_Index;
		public int Level_Base;
		public int Level_Skill;
		public int Level_Weapon;
		public int Level_Armor;
		public int Level_Acc;
	}

	public static class Field
	{
		public static string MobIndex = "MobIndex";
		public static string Card_Index = "Card_Index";
		public static string Level_Base = "Level_Base";
		public static string Level_Skill = "Level_Skill";
		public static string Level_Weapon = "Level_Weapon";
		public static string Level_Armor = "Level_Armor";
		public static string Level_Acc = "Level_Acc";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_PVEMobDataScriptableObject");
			if (asset != null)
			{
				DB_PVEMobDataScriptableObject scriptableObject = asset as DB_PVEMobDataScriptableObject;
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
				DB_PVEMobDataScriptableObject scriptableObject = asset as DB_PVEMobDataScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
