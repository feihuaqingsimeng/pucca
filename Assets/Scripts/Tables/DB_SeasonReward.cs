using System;
using UnityEngine;

public class DB_SeasonReward : TableBase<DB_SeasonReward, DB_SeasonReward.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Ruby_Obtain;
		public int Rank_Min;
		public int Rank_Max;
		public Rank_Type Rank_Type;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Ruby_Obtain = "Ruby_Obtain";
		public static string Rank_Min = "Rank_Min";
		public static string Rank_Max = "Rank_Max";
		public static string Rank_Type = "Rank_Type";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SeasonRewardScriptableObject");
			if (asset != null)
			{
				DB_SeasonRewardScriptableObject scriptableObject = asset as DB_SeasonRewardScriptableObject;
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
				DB_SeasonRewardScriptableObject scriptableObject = asset as DB_SeasonRewardScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
