using System;
using UnityEngine;

public class DB_AreaPvP : TableBase<DB_AreaPvP, DB_AreaPvP.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public TEXT_UI TEXT_UI;
		public int Need_Heart;
		public int Need_Rank_Point;
		public int Degrade_Rank_Point;
		public string Battle_Bg;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string TEXT_UI = "TEXT_UI";
		public static string Need_Heart = "Need_Heart";
		public static string Need_Rank_Point = "Need_Rank_Point";
		public static string Degrade_Rank_Point = "Degrade_Rank_Point";
		public static string Battle_Bg = "Battle_Bg";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_AreaPvPScriptableObject");
			if (asset != null)
			{
				DB_AreaPvPScriptableObject scriptableObject = asset as DB_AreaPvPScriptableObject;
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
				DB_AreaPvPScriptableObject scriptableObject = asset as DB_AreaPvPScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
