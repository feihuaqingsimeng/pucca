using System;
using UnityEngine;

public class DBStr_UI : TableBase<DBStr_UI, DBStr_UI.Schema>
{
	[Serializable]
	public class Schema
	{
		public TEXT_UI TEXT_UI;
		public string StringData;
	}

	public static class Field
	{
		public static string TEXT_UI = "TEXT_UI";
		public static string StringData = "StringData";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_UIScriptableObject");
			if (asset != null)
			{
				DBStr_UIScriptableObject scriptableObject = asset as DBStr_UIScriptableObject;
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
				DBStr_UIScriptableObject scriptableObject = asset as DBStr_UIScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
