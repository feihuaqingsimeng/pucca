using System;
using UnityEngine;

public class DB_FranchiseExpansion : TableBase<DB_FranchiseExpansion, DB_FranchiseExpansion.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Expansion_Type;
		public int ReqAccountLevel;
		public int ReqBuildingNum;
		public int ReqFloorNum;
		public Goods_Type Goods_Type;
		public int ReqCount;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Expansion_Type = "Expansion_Type";
		public static string ReqAccountLevel = "ReqAccountLevel";
		public static string ReqBuildingNum = "ReqBuildingNum";
		public static string ReqFloorNum = "ReqFloorNum";
		public static string Goods_Type = "Goods_Type";
		public static string ReqCount = "ReqCount";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_FranchiseExpansionScriptableObject");
			if (asset != null)
			{
				DB_FranchiseExpansionScriptableObject scriptableObject = asset as DB_FranchiseExpansionScriptableObject;
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
				DB_FranchiseExpansionScriptableObject scriptableObject = asset as DB_FranchiseExpansionScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
