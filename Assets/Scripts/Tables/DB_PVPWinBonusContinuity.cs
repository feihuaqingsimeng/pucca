using System;
using UnityEngine;

public class DB_PVPWinBonusContinuity : TableBase<DB_PVPWinBonusContinuity, DB_PVPWinBonusContinuity.Schema>
{
	[Serializable]
	public class Schema
	{
		public int index;
		public int Wins_Count;
		public Goods_Type Goods_Type;
		public int RewardCount;
	}

	public static class Field
	{
		public static string index = "index";
		public static string Wins_Count = "Wins_Count";
		public static string Goods_Type = "Goods_Type";
		public static string RewardCount = "RewardCount";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_PVPWinBonusContinuityScriptableObject");
			if (asset != null)
			{
				DB_PVPWinBonusContinuityScriptableObject scriptableObject = asset as DB_PVPWinBonusContinuityScriptableObject;
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
				DB_PVPWinBonusContinuityScriptableObject scriptableObject = asset as DB_PVPWinBonusContinuityScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
