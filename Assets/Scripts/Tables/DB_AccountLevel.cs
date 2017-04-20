using System;
using UnityEngine;

public class DB_AccountLevel : TableBase<DB_AccountLevel, DB_AccountLevel.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int AccountLevel;
		public int Need_AccountExp;
		public int Total_AccountExp;
		public int Reward_TrainingPoint;
		public int Reward_Heart;
		public int Reward_Ruby;
		public int Max_Heart;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string AccountLevel = "AccountLevel";
		public static string Need_AccountExp = "Need_AccountExp";
		public static string Total_AccountExp = "Total_AccountExp";
		public static string Reward_TrainingPoint = "Reward_TrainingPoint";
		public static string Reward_Heart = "Reward_Heart";
		public static string Reward_Ruby = "Reward_Ruby";
		public static string Max_Heart = "Max_Heart";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_AccountLevelScriptableObject");
			if (asset != null)
			{
				DB_AccountLevelScriptableObject scriptableObject = asset as DB_AccountLevelScriptableObject;
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
				DB_AccountLevelScriptableObject scriptableObject = asset as DB_AccountLevelScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
