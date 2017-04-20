using System;
using UnityEngine;

public class DB_CardLevelUp : TableBase<DB_CardLevelUp, DB_CardLevelUp.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Grade_Type Grade_Type;
		public int CardLevel;
		public int Count;
		public int Need_Gold;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Grade_Type = "Grade_Type";
		public static string CardLevel = "CardLevel";
		public static string Count = "Count";
		public static string Need_Gold = "Need_Gold";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_CardLevelUpScriptableObject");
			if (asset != null)
			{
				DB_CardLevelUpScriptableObject scriptableObject = asset as DB_CardLevelUpScriptableObject;
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
				DB_CardLevelUpScriptableObject scriptableObject = asset as DB_CardLevelUpScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
