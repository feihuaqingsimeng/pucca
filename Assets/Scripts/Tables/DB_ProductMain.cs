using System;
using UnityEngine;

public class DB_ProductMain : TableBase<DB_ProductMain, DB_ProductMain.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string PlayStoreProductId;
		public string AppStoreProductId;
		public int PackageId;
		public ItemProductType ItemProductType;
		public int AccountLimit;
		public int LevelLimit;
		public string SaleStartTime;
		public string SaleEndTime;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string PlayStoreProductId = "PlayStoreProductId";
		public static string AppStoreProductId = "AppStoreProductId";
		public static string PackageId = "PackageId";
		public static string ItemProductType = "ItemProductType";
		public static string AccountLimit = "AccountLimit";
		public static string LevelLimit = "LevelLimit";
		public static string SaleStartTime = "SaleStartTime";
		public static string SaleEndTime = "SaleEndTime";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_ProductMainScriptableObject");
			if (asset != null)
			{
				DB_ProductMainScriptableObject scriptableObject = asset as DB_ProductMainScriptableObject;
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
				DB_ProductMainScriptableObject scriptableObject = asset as DB_ProductMainScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
