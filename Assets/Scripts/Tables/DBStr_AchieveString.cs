using System;
using UnityEngine;

public class DBStr_AchieveString : TableBase<DBStr_AchieveString, DBStr_AchieveString.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string TITLE_STRING;
		public string CONTENT_STRING;
		public int Achieve_Group;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string TITLE_STRING = "TITLE_STRING";
		public static string CONTENT_STRING = "CONTENT_STRING";
		public static string Achieve_Group = "Achieve_Group";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_AchieveStringScriptableObject");
			if (asset != null)
			{
				DBStr_AchieveStringScriptableObject scriptableObject = asset as DBStr_AchieveStringScriptableObject;
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
				DBStr_AchieveStringScriptableObject scriptableObject = asset as DBStr_AchieveStringScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
