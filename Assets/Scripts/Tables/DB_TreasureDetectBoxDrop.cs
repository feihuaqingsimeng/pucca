using System;
using UnityEngine;

public class DB_TreasureDetectBoxDrop : TableBase<DB_TreasureDetectBoxDrop, DB_TreasureDetectBoxDrop.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int IslandNum;
		public int BoxIndex;
		public int DropProb;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string IslandNum = "IslandNum";
		public static string BoxIndex = "BoxIndex";
		public static string DropProb = "DropProb";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TreasureDetectBoxDropScriptableObject");
			if (asset != null)
			{
				DB_TreasureDetectBoxDropScriptableObject scriptableObject = asset as DB_TreasureDetectBoxDropScriptableObject;
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
				DB_TreasureDetectBoxDropScriptableObject scriptableObject = asset as DB_TreasureDetectBoxDropScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
