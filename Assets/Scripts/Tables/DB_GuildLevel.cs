using System;
using UnityEngine;

public class DB_GuildLevel : TableBase<DB_GuildLevel, DB_GuildLevel.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int GulidLevel;
		public int Max_Exp;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string GulidLevel = "GulidLevel";
		public static string Max_Exp = "Max_Exp";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_GuildLevelScriptableObject");
			if (asset != null)
			{
				DB_GuildLevelScriptableObject scriptableObject = asset as DB_GuildLevelScriptableObject;
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
				DB_GuildLevelScriptableObject scriptableObject = asset as DB_GuildLevelScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
