using System;
using UnityEngine;

public class DBStr_UI_BuiltIn : TableBase<DBStr_UI_BuiltIn, DBStr_UI_BuiltIn.Schema>
{
	[Serializable]
	public class Schema
	{
		public TEXT_UI TEXT_UI;
		public LanguageCode LanguageCode;
		public string String_Data;
	}

	public static class Field
	{
		public static string TEXT_UI = "TEXT_UI";
		public static string LanguageCode = "LanguageCode";
		public static string String_Data = "String_Data";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_UI_BuiltInScriptableObject");
			if (asset != null)
			{
				DBStr_UI_BuiltInScriptableObject scriptableObject = asset as DBStr_UI_BuiltInScriptableObject;
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
				DBStr_UI_BuiltInScriptableObject scriptableObject = asset as DBStr_UI_BuiltInScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
