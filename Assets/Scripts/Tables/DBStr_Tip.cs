using System;
using UnityEngine;

public class DBStr_Tip : TableBase<DBStr_Tip, DBStr_Tip.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string Tip;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Tip = "Tip";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_TipScriptableObject");
			if (asset != null)
			{
				DBStr_TipScriptableObject scriptableObject = asset as DBStr_TipScriptableObject;
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
				DBStr_TipScriptableObject scriptableObject = asset as DBStr_TipScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
