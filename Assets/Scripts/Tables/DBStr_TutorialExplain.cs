using System;
using UnityEngine;

public class DBStr_TutorialExplain : TableBase<DBStr_TutorialExplain, DBStr_TutorialExplain.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string ContentsName;
		public string ContentsExplain;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string ContentsName = "ContentsName";
		public static string ContentsExplain = "ContentsExplain";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_TutorialExplainScriptableObject");
			if (asset != null)
			{
				DBStr_TutorialExplainScriptableObject scriptableObject = asset as DBStr_TutorialExplainScriptableObject;
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
				DBStr_TutorialExplainScriptableObject scriptableObject = asset as DBStr_TutorialExplainScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
