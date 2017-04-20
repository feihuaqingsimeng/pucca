using System;
using UnityEngine;

public class DB_Tutorial : TableBase<DB_Tutorial, DB_Tutorial.Schema>
{
	[Serializable]
	public class Schema
	{
		public int GroupIndex;
		public int Index;
		public int NextIndex;
		public TUTORIAL_TYPE TUTORIAL_TYPE;
		public int StringIndex;
		public string TargetName;
		public int WaitSeq;
		public int ExplainID;
	}

	public static class Field
	{
		public static string GroupIndex = "GroupIndex";
		public static string Index = "Index";
		public static string NextIndex = "NextIndex";
		public static string TUTORIAL_TYPE = "TUTORIAL_TYPE";
		public static string StringIndex = "StringIndex";
		public static string TargetName = "TargetName";
		public static string WaitSeq = "WaitSeq";
		public static string ExplainID = "ExplainID";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TutorialScriptableObject");
			if (asset != null)
			{
				DB_TutorialScriptableObject scriptableObject = asset as DB_TutorialScriptableObject;
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
				DB_TutorialScriptableObject scriptableObject = asset as DB_TutorialScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
