using System;
using UnityEngine;

public class DB_FranchiseStructure : TableBase<DB_FranchiseStructure, DB_FranchiseStructure.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int BuildingNum;
		public int FloorNum;
		public Goods_Type Goods_Type;
		public int RewardCount;
		public int CoolTime_Sec;
		public int Expansion_Type;
		public bool isRoof;
		public string ResourceName;
		public int CardId;
		public int isTopFloor;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string BuildingNum = "BuildingNum";
		public static string FloorNum = "FloorNum";
		public static string Goods_Type = "Goods_Type";
		public static string RewardCount = "RewardCount";
		public static string CoolTime_Sec = "CoolTime_Sec";
		public static string Expansion_Type = "Expansion_Type";
		public static string isRoof = "isRoof";
		public static string ResourceName = "ResourceName";
		public static string CardId = "CardId";
		public static string isTopFloor = "isTopFloor";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_FranchiseStructureScriptableObject");
			if (asset != null)
			{
				DB_FranchiseStructureScriptableObject scriptableObject = asset as DB_FranchiseStructureScriptableObject;
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
				DB_FranchiseStructureScriptableObject scriptableObject = asset as DB_FranchiseStructureScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
