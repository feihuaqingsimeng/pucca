using System;
using UnityEngine;

public class DB_DailyAchieveList : TableBase<DB_DailyAchieveList, DB_DailyAchieveList.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Achieve_Type Achieve_Type;
		public int Terms_Count;
		public Goods_Type Goods_Type;
		public int Goods_Obtain;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Achieve_Type = "Achieve_Type";
		public static string Terms_Count = "Terms_Count";
		public static string Goods_Type = "Goods_Type";
		public static string Goods_Obtain = "Goods_Obtain";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_DailyAchieveListScriptableObject");
			if (asset != null)
			{
				DB_DailyAchieveListScriptableObject scriptableObject = asset as DB_DailyAchieveListScriptableObject;
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
				DB_DailyAchieveListScriptableObject scriptableObject = asset as DB_DailyAchieveListScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
