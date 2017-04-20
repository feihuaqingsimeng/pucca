using System;
using UnityEngine;

public class DB_TreasureDetectBoxReward : TableBase<DB_TreasureDetectBoxReward, DB_TreasureDetectBoxReward.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Drop_IslandNum;
		public Grade_Type Grade_Type;
		public int Card_Index;
		public int DropProb;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Drop_IslandNum = "Drop_IslandNum";
		public static string Grade_Type = "Grade_Type";
		public static string Card_Index = "Card_Index";
		public static string DropProb = "DropProb";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TreasureDetectBoxRewardScriptableObject");
			if (asset != null)
			{
				DB_TreasureDetectBoxRewardScriptableObject scriptableObject = asset as DB_TreasureDetectBoxRewardScriptableObject;
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
				DB_TreasureDetectBoxRewardScriptableObject scriptableObject = asset as DB_TreasureDetectBoxRewardScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
