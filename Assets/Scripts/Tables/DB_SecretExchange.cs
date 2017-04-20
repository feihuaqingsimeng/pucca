using System;
using UnityEngine;

public class DB_SecretExchange : TableBase<DB_SecretExchange, DB_SecretExchange.Schema>
{
	[Serializable]
	public class Schema
	{
		public int index;
		public int SlotType_SecretExchangeSlot;
		public int GetType_LegendBox;
		public string Icon_Name;
	}

	public static class Field
	{
		public static string index = "index";
		public static string SlotType_SecretExchangeSlot = "SlotType_SecretExchangeSlot";
		public static string GetType_LegendBox = "GetType_LegendBox";
		public static string Icon_Name = "Icon_Name";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SecretExchangeScriptableObject");
			if (asset != null)
			{
				DB_SecretExchangeScriptableObject scriptableObject = asset as DB_SecretExchangeScriptableObject;
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
				DB_SecretExchangeScriptableObject scriptableObject = asset as DB_SecretExchangeScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
