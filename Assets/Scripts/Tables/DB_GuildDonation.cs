using System;
using UnityEngine;

public class DB_GuildDonation : TableBase<DB_GuildDonation, DB_GuildDonation.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Goods_Type Goods_Type;
		public int DonationPrice;
		public int GulidPoint_Obtain;
		public int GulidExp_Obtain;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Goods_Type = "Goods_Type";
		public static string DonationPrice = "DonationPrice";
		public static string GulidPoint_Obtain = "GulidPoint_Obtain";
		public static string GulidExp_Obtain = "GulidExp_Obtain";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_GuildDonationScriptableObject");
			if (asset != null)
			{
				DB_GuildDonationScriptableObject scriptableObject = asset as DB_GuildDonationScriptableObject;
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
				DB_GuildDonationScriptableObject scriptableObject = asset as DB_GuildDonationScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
