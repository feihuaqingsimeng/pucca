using System;
using UnityEngine;

public class DB_ProductPackage : TableBase<DB_ProductPackage, DB_ProductPackage.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int ProductIndex;
		public int PackageId;
		public Goods_Type Goods_Type;
		public int GoodsValue;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string ProductIndex = "ProductIndex";
		public static string PackageId = "PackageId";
		public static string Goods_Type = "Goods_Type";
		public static string GoodsValue = "GoodsValue";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_ProductPackageScriptableObject");
			if (asset != null)
			{
				DB_ProductPackageScriptableObject scriptableObject = asset as DB_ProductPackageScriptableObject;
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
				DB_ProductPackageScriptableObject scriptableObject = asset as DB_ProductPackageScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
