using System;
using UnityEngine;

public class DB_GuildCardRequest : TableBase<DB_GuildCardRequest, DB_GuildCardRequest.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public Grade_Type Grade_Type;
		public int GulidLevel;
		public int Get_limit;
		public int Donation_limit;
		public int EXP_Obtain;
		public int Gold_Obtain;
		public int GulidPoint_Obtain;
		public int GulidExp_Obtain;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Grade_Type = "Grade_Type";
		public static string GulidLevel = "GulidLevel";
		public static string Get_limit = "Get_limit";
		public static string Donation_limit = "Donation_limit";
		public static string EXP_Obtain = "EXP_Obtain";
		public static string Gold_Obtain = "Gold_Obtain";
		public static string GulidPoint_Obtain = "GulidPoint_Obtain";
		public static string GulidExp_Obtain = "GulidExp_Obtain";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_GuildCardRequestScriptableObject");
			if (asset != null)
			{
				DB_GuildCardRequestScriptableObject scriptableObject = asset as DB_GuildCardRequestScriptableObject;
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
				DB_GuildCardRequestScriptableObject scriptableObject = asset as DB_GuildCardRequestScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
