using System;
using UnityEngine;

public class DB_LegendBox : TableBase<DB_LegendBox, DB_LegendBox.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Card_Index;
		public int Get_Type;
		public int SumProb;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Card_Index = "Card_Index";
		public static string Get_Type = "Get_Type";
		public static string SumProb = "SumProb";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_LegendBoxScriptableObject");
			if (asset != null)
			{
				DB_LegendBoxScriptableObject scriptableObject = asset as DB_LegendBoxScriptableObject;
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
				DB_LegendBoxScriptableObject scriptableObject = asset as DB_LegendBoxScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
