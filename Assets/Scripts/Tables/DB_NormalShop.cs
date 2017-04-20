using System;
using UnityEngine;

public class DB_NormalShop : TableBase<DB_NormalShop, DB_NormalShop.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Shop_Group;
		public Goods_Type Goods_Type;
		public int Offer_Count;
		public int BoxGet_Link;
		public Price_BuyType Price_BuyType;
		public int Need_Count;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Shop_Group = "Shop_Group";
		public static string Goods_Type = "Goods_Type";
		public static string Offer_Count = "Offer_Count";
		public static string BoxGet_Link = "BoxGet_Link";
		public static string Price_BuyType = "Price_BuyType";
		public static string Need_Count = "Need_Count";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_NormalShopScriptableObject");
			if (asset != null)
			{
				DB_NormalShopScriptableObject scriptableObject = asset as DB_NormalShopScriptableObject;
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
				DB_NormalShopScriptableObject scriptableObject = asset as DB_NormalShopScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
