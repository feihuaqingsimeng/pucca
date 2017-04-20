using System;
using UnityEngine;

public class DB_SecretExchangeSlot : TableBase<DB_SecretExchangeSlot, DB_SecretExchangeSlot.Schema>
{
	[Serializable]
	public class Schema
	{
		public int index;
		public int SlotOrder;
		public Grade_Type Grade_Type;
		public ClassType ClassType;
		public int Need_Qty_Slot;
		public int SlotType;
	}

	public static class Field
	{
		public static string index = "index";
		public static string SlotOrder = "SlotOrder";
		public static string Grade_Type = "Grade_Type";
		public static string ClassType = "ClassType";
		public static string Need_Qty_Slot = "Need_Qty_Slot";
		public static string SlotType = "SlotType";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SecretExchangeSlotScriptableObject");
			if (asset != null)
			{
				DB_SecretExchangeSlotScriptableObject scriptableObject = asset as DB_SecretExchangeSlotScriptableObject;
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
				DB_SecretExchangeSlotScriptableObject scriptableObject = asset as DB_SecretExchangeSlotScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
