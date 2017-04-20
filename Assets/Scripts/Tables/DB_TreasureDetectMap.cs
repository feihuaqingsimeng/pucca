using System;
using UnityEngine;

public class DB_TreasureDetectMap : TableBase<DB_TreasureDetectMap, DB_TreasureDetectMap.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int IslandNum;
		public int UnlockAccountLevel;
		public Goods_Type Goods_Type;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string IslandNum = "IslandNum";
		public static string UnlockAccountLevel = "UnlockAccountLevel";
		public static string Goods_Type = "Goods_Type";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TreasureDetectMapScriptableObject");
			if (asset != null)
			{
				DB_TreasureDetectMapScriptableObject scriptableObject = asset as DB_TreasureDetectMapScriptableObject;
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
				DB_TreasureDetectMapScriptableObject scriptableObject = asset as DB_TreasureDetectMapScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
