using System;
using UnityEngine;

public class DB_DailyReward : TableBase<DB_DailyReward, DB_DailyReward.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Ruby_Obtain;
		public int Need_RankingPoint;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Ruby_Obtain = "Ruby_Obtain";
		public static string Need_RankingPoint = "Need_RankingPoint";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_DailyRewardScriptableObject");
			if (asset != null)
			{
				DB_DailyRewardScriptableObject scriptableObject = asset as DB_DailyRewardScriptableObject;
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
				DB_DailyRewardScriptableObject scriptableObject = asset as DB_DailyRewardScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
