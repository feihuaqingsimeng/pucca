using System;
using UnityEngine;

public class DB_EmblemPattern : TableBase<DB_EmblemPattern, DB_EmblemPattern.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string Icon_Name;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Icon_Name = "Icon_Name";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_EmblemPatternScriptableObject");
			if (asset != null)
			{
				DB_EmblemPatternScriptableObject scriptableObject = asset as DB_EmblemPatternScriptableObject;
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
				DB_EmblemPatternScriptableObject scriptableObject = asset as DB_EmblemPatternScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
