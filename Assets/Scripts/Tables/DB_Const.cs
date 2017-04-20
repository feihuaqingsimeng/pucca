using System;
using UnityEngine;

public class DB_Const : TableBase<DB_Const, DB_Const.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Const_IndexID Const_IndexID;
		public float Const_Value;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Const_IndexID = "Const_IndexID";
		public static string Const_Value = "Const_Value";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_ConstScriptableObject");
			if (asset != null)
			{
				DB_ConstScriptableObject scriptableObject = asset as DB_ConstScriptableObject;
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
				DB_ConstScriptableObject scriptableObject = asset as DB_ConstScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
