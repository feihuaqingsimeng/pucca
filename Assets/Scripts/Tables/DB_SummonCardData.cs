using System;
using UnityEngine;

public class DB_SummonCardData : TableBase<DB_SummonCardData, DB_SummonCardData.Schema>
{
	[Serializable]
	public class Schema
	{
		public int SummonIndex;
		public string SpineName;
		public float HP_Lv1;
		public float HP_Lv2;
		public float HP_Lv3;
		public float HP_Lv4;
		public float HP_Lv5;
		public float AP_Lv1;
		public float AP_Lv2;
		public float AP_Lv3;
		public float AP_Lv4;
		public float AP_Lv5;
		public float DP_Lv1;
		public float DP_Lv2;
		public float DP_Lv3;
		public float DP_Lv4;
		public float DP_Lv5;
	}

	public static class Field
	{
		public static string SummonIndex = "SummonIndex";
		public static string SpineName = "SpineName";
		public static string HP_Lv1 = "HP_Lv1";
		public static string HP_Lv2 = "HP_Lv2";
		public static string HP_Lv3 = "HP_Lv3";
		public static string HP_Lv4 = "HP_Lv4";
		public static string HP_Lv5 = "HP_Lv5";
		public static string AP_Lv1 = "AP_Lv1";
		public static string AP_Lv2 = "AP_Lv2";
		public static string AP_Lv3 = "AP_Lv3";
		public static string AP_Lv4 = "AP_Lv4";
		public static string AP_Lv5 = "AP_Lv5";
		public static string DP_Lv1 = "DP_Lv1";
		public static string DP_Lv2 = "DP_Lv2";
		public static string DP_Lv3 = "DP_Lv3";
		public static string DP_Lv4 = "DP_Lv4";
		public static string DP_Lv5 = "DP_Lv5";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SummonCardDataScriptableObject");
			if (asset != null)
			{
				DB_SummonCardDataScriptableObject scriptableObject = asset as DB_SummonCardDataScriptableObject;
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
				DB_SummonCardDataScriptableObject scriptableObject = asset as DB_SummonCardDataScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
