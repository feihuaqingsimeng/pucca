using System;
using UnityEngine;

public class DB_EquipLevelUp : TableBase<DB_EquipLevelUp, DB_EquipLevelUp.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Goods_Type Goods_Type;
		public int AddCount_10;
		public int AddCount_20;
		public int AddCount_30;
		public int BaseGold;
		public int Gold_10;
		public int Gold_20;
		public int Gold_30;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Goods_Type = "Goods_Type";
		public static string AddCount_10 = "AddCount_10";
		public static string AddCount_20 = "AddCount_20";
		public static string AddCount_30 = "AddCount_30";
		public static string BaseGold = "BaseGold";
		public static string Gold_10 = "Gold_10";
		public static string Gold_20 = "Gold_20";
		public static string Gold_30 = "Gold_30";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_EquipLevelUpScriptableObject");
			if (asset != null)
			{
				DB_EquipLevelUpScriptableObject scriptableObject = asset as DB_EquipLevelUpScriptableObject;
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
				DB_EquipLevelUpScriptableObject scriptableObject = asset as DB_EquipLevelUpScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
