using System;
using UnityEngine;

public class DB_TreasureSearch : TableBase<DB_TreasureSearch, DB_TreasureSearch.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Need_AccountLevel;
		public int BoxGet_Link;
		public int Open_Time;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Need_AccountLevel = "Need_AccountLevel";
		public static string BoxGet_Link = "BoxGet_Link";
		public static string Open_Time = "Open_Time";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TreasureSearchScriptableObject");
			if (asset != null)
			{
				DB_TreasureSearchScriptableObject scriptableObject = asset as DB_TreasureSearchScriptableObject;
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
				DB_TreasureSearchScriptableObject scriptableObject = asset as DB_TreasureSearchScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
