using System;
using UnityEngine;

public class DB_Buff : TableBase<DB_Buff, DB_Buff.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public bool BUFF_GOOD;
		public BUFF_KIND BUFF_KIND;
		public float BaseValue;
		public float LvAddValue;
		public float TickDelay;
		public float MaxTime;
		public float AddMaxTime;
		public bool Overlap_Check;
		public string ACTIVE_EFFECT;
		public string HIT_EFFECT;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string BUFF_GOOD = "BUFF_GOOD";
		public static string BUFF_KIND = "BUFF_KIND";
		public static string BaseValue = "BaseValue";
		public static string LvAddValue = "LvAddValue";
		public static string TickDelay = "TickDelay";
		public static string MaxTime = "MaxTime";
		public static string AddMaxTime = "AddMaxTime";
		public static string Overlap_Check = "Overlap_Check";
		public static string ACTIVE_EFFECT = "ACTIVE_EFFECT";
		public static string HIT_EFFECT = "HIT_EFFECT";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_BuffScriptableObject");
			if (asset != null)
			{
				DB_BuffScriptableObject scriptableObject = asset as DB_BuffScriptableObject;
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
				DB_BuffScriptableObject scriptableObject = asset as DB_BuffScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
