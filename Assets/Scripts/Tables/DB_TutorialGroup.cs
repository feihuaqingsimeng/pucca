using System;
using UnityEngine;

public class DB_TutorialGroup : TableBase<DB_TutorialGroup, DB_TutorialGroup.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int NextIndex;
		public int PrevIndex;
		public int BattleFlag;
		public int ActiveLevel;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string NextIndex = "NextIndex";
		public static string PrevIndex = "PrevIndex";
		public static string BattleFlag = "BattleFlag";
		public static string ActiveLevel = "ActiveLevel";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TutorialGroupScriptableObject");
			if (asset != null)
			{
				DB_TutorialGroupScriptableObject scriptableObject = asset as DB_TutorialGroupScriptableObject;
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
				DB_TutorialGroupScriptableObject scriptableObject = asset as DB_TutorialGroupScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
