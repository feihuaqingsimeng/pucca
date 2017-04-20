using System;
using UnityEngine;

public class DB_StageReward : TableBase<DB_StageReward, DB_StageReward.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Reward_Group;
		public int Reward_Slot;
		public Goods_Type Goods_Type;
		public int Count_Min;
		public int Count_Max;
		public float FixDropProb;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Reward_Group = "Reward_Group";
		public static string Reward_Slot = "Reward_Slot";
		public static string Goods_Type = "Goods_Type";
		public static string Count_Min = "Count_Min";
		public static string Count_Max = "Count_Max";
		public static string FixDropProb = "FixDropProb";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_StageRewardScriptableObject");
			if (asset != null)
			{
				DB_StageRewardScriptableObject scriptableObject = asset as DB_StageRewardScriptableObject;
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
				DB_StageRewardScriptableObject scriptableObject = asset as DB_StageRewardScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
