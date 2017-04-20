using System;
using UnityEngine;

public class DB_AchieveList : TableBase<DB_AchieveList, DB_AchieveList.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Achieve_Group;
		public int Achieve_Step;
		public Achieve_Type Achieve_Type;
		public int Terms_COUNT;
		public Goods_Type Goods_Type;
		public int Goods_Obtain;
		public int List_LineUp;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Achieve_Group = "Achieve_Group";
		public static string Achieve_Step = "Achieve_Step";
		public static string Achieve_Type = "Achieve_Type";
		public static string Terms_COUNT = "Terms_COUNT";
		public static string Goods_Type = "Goods_Type";
		public static string Goods_Obtain = "Goods_Obtain";
		public static string List_LineUp = "List_LineUp";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_AchieveListScriptableObject");
			if (asset != null)
			{
				DB_AchieveListScriptableObject scriptableObject = asset as DB_AchieveListScriptableObject;
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
				DB_AchieveListScriptableObject scriptableObject = asset as DB_AchieveListScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
