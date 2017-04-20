using System;
using UnityEngine;

public class DB_FranchiseExpansionInfo : TableBase<DB_FranchiseExpansionInfo, DB_FranchiseExpansionInfo.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int BuildingNum;
		public TEXT_UI TEXT_UI;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string BuildingNum = "BuildingNum";
		public static string TEXT_UI = "TEXT_UI";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_FranchiseExpansionInfoScriptableObject");
			if (asset != null)
			{
				DB_FranchiseExpansionInfoScriptableObject scriptableObject = asset as DB_FranchiseExpansionInfoScriptableObject;
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
				DB_FranchiseExpansionInfoScriptableObject scriptableObject = asset as DB_FranchiseExpansionInfoScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
