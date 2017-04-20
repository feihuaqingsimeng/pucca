using System;
using UnityEngine;

public class DB_Soul : TableBase<DB_Soul, DB_Soul.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Card_List_Link;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Card_List_Link = "Card_List_Link";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SoulScriptableObject");
			if (asset != null)
			{
				DB_SoulScriptableObject scriptableObject = asset as DB_SoulScriptableObject;
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
				DB_SoulScriptableObject scriptableObject = asset as DB_SoulScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
