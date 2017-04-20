using System;
using UnityEngine;

public class DBStr_Tutorial : TableBase<DBStr_Tutorial, DBStr_Tutorial.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string StringData;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string StringData = "StringData";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_TutorialScriptableObject");
			if (asset != null)
			{
				DBStr_TutorialScriptableObject scriptableObject = asset as DBStr_TutorialScriptableObject;
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
				DBStr_TutorialScriptableObject scriptableObject = asset as DBStr_TutorialScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
