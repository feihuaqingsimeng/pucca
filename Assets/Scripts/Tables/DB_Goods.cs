using System;
using UnityEngine;

public class DB_Goods : TableBase<DB_Goods, DB_Goods.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string TEXT_UI;
		public Goods_Type Goods_Type;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string TEXT_UI = "TEXT_UI";
		public static string Goods_Type = "Goods_Type";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_GoodsScriptableObject");
			if (asset != null)
			{
				DB_GoodsScriptableObject scriptableObject = asset as DB_GoodsScriptableObject;
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
				DB_GoodsScriptableObject scriptableObject = asset as DB_GoodsScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
