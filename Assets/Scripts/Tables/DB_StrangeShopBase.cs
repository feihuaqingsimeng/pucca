using System;
using UnityEngine;

public class DB_StrangeShopBase : TableBase<DB_StrangeShopBase, DB_StrangeShopBase.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Goods_Type Goods_Type;
		public float Grade_C_Pb;
		public float Grade_B_Pb;
		public float Grade_A_Pb;
		public float Grade_S_Pb;
		public string Goods_Icon_Name;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Goods_Type = "Goods_Type";
		public static string Grade_C_Pb = "Grade_C_Pb";
		public static string Grade_B_Pb = "Grade_B_Pb";
		public static string Grade_A_Pb = "Grade_A_Pb";
		public static string Grade_S_Pb = "Grade_S_Pb";
		public static string Goods_Icon_Name = "Goods_Icon_Name";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_StrangeShopBaseScriptableObject");
			if (asset != null)
			{
				DB_StrangeShopBaseScriptableObject scriptableObject = asset as DB_StrangeShopBaseScriptableObject;
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
				DB_StrangeShopBaseScriptableObject scriptableObject = asset as DB_StrangeShopBaseScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
