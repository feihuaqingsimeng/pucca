using System;
using UnityEngine;

public class DB_StrangeShopProperty : TableBase<DB_StrangeShopProperty, DB_StrangeShopProperty.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Grade_Type Grade_Type;
		public Goods_Type Goods_Type;
		public int Price_Base;
		public int Price_Add;
		public int Buylimit;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Grade_Type = "Grade_Type";
		public static string Goods_Type = "Goods_Type";
		public static string Price_Base = "Price_Base";
		public static string Price_Add = "Price_Add";
		public static string Buylimit = "Buylimit";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_StrangeShopPropertyScriptableObject");
			if (asset != null)
			{
				DB_StrangeShopPropertyScriptableObject scriptableObject = asset as DB_StrangeShopPropertyScriptableObject;
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
				DB_StrangeShopPropertyScriptableObject scriptableObject = asset as DB_StrangeShopPropertyScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
