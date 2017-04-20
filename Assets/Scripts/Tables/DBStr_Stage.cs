using System;
using UnityEngine;

public class DBStr_Stage : TableBase<DBStr_Stage, DBStr_Stage.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Stage_Id;
		public string StageName;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Stage_Id = "Stage_Id";
		public static string StageName = "StageName";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_StageScriptableObject");
			if (asset != null)
			{
				DBStr_StageScriptableObject scriptableObject = asset as DBStr_StageScriptableObject;
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
				DBStr_StageScriptableObject scriptableObject = asset as DBStr_StageScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
