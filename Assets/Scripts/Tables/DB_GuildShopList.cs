using System;
using UnityEngine;

public class DB_GuildShopList : TableBase<DB_GuildShopList, DB_GuildShopList.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int GulidLevel_Open;
		public int Buylimit_Base;
		public int Buylimit_Add;
		public Goods_Type Goods_Type;
		public int Card_IndexID;
		public int Need_GuildPoint;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string GulidLevel_Open = "GulidLevel_Open";
		public static string Buylimit_Base = "Buylimit_Base";
		public static string Buylimit_Add = "Buylimit_Add";
		public static string Goods_Type = "Goods_Type";
		public static string Card_IndexID = "Card_IndexID";
		public static string Need_GuildPoint = "Need_GuildPoint";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_GuildShopListScriptableObject");
			if (asset != null)
			{
				DB_GuildShopListScriptableObject scriptableObject = asset as DB_GuildShopListScriptableObject;
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
				DB_GuildShopListScriptableObject scriptableObject = asset as DB_GuildShopListScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
