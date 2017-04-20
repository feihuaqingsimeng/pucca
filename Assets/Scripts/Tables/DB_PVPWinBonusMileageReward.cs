using System;
using UnityEngine;

public class DB_PVPWinBonusMileageReward : TableBase<DB_PVPWinBonusMileageReward, DB_PVPWinBonusMileageReward.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Goods_Type Goods_Type;
		public int Count_min;
		public int Count_MAX;
		public int Get_Type;
		public int SumProb;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Goods_Type = "Goods_Type";
		public static string Count_min = "Count_min";
		public static string Count_MAX = "Count_MAX";
		public static string Get_Type = "Get_Type";
		public static string SumProb = "SumProb";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_PVPWinBonusMileageRewardScriptableObject");
			if (asset != null)
			{
				DB_PVPWinBonusMileageRewardScriptableObject scriptableObject = asset as DB_PVPWinBonusMileageRewardScriptableObject;
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
				DB_PVPWinBonusMileageRewardScriptableObject scriptableObject = asset as DB_PVPWinBonusMileageRewardScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
