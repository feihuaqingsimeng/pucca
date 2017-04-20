using System;
using UnityEngine;

public class DB_ProductName : TableBase<DB_ProductName, DB_ProductName.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public TEXT_UI TEXT_UI;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string TEXT_UI = "TEXT_UI";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_ProductNameScriptableObject");
			if (asset != null)
			{
				DB_ProductNameScriptableObject scriptableObject = asset as DB_ProductNameScriptableObject;
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
				DB_ProductNameScriptableObject scriptableObject = asset as DB_ProductNameScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
