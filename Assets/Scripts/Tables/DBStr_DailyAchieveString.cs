using System;
using UnityEngine;

public class DBStr_DailyAchieveString : TableBase<DBStr_DailyAchieveString, DBStr_DailyAchieveString.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string TITLE_STRING;
		public string CONTENT_STRING;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string TITLE_STRING = "TITLE_STRING";
		public static string CONTENT_STRING = "CONTENT_STRING";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_DailyAchieveStringScriptableObject");
			if (asset != null)
			{
				DBStr_DailyAchieveStringScriptableObject scriptableObject = asset as DBStr_DailyAchieveStringScriptableObject;
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
				DBStr_DailyAchieveStringScriptableObject scriptableObject = asset as DBStr_DailyAchieveStringScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
