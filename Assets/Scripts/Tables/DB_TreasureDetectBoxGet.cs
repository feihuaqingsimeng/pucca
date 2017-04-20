using System;
using UnityEngine;

public class DB_TreasureDetectBoxGet : TableBase<DB_TreasureDetectBoxGet, DB_TreasureDetectBoxGet.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public TEXT_UI TEXT_UI;
		public int Gold_Min;
		public int Gold_Max;
		public int Total_Type;
		public int Total_Count;
		public int C_Type_Min;
		public int C_Type_Max;
		public int C_Count_Min;
		public int C_Count_Max;
		public int B_Type_Min;
		public int B_Type_Max;
		public int B_Count_Min;
		public int B_Count_Max;
		public int A_Type_Min;
		public int A_Type_Max;
		public int A_Count_Min;
		public int A_Count_Max;
		public int S_Type_Min;
		public int S_Type_Max;
		public int S_Count_Min;
		public int S_Count_Max;
		public int SS_Type_Min;
		public int SS_Type_Max;
		public int SS_Count_Min;
		public int SS_Count_Max;
		public string Box_IdentificationName;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string TEXT_UI = "TEXT_UI";
		public static string Gold_Min = "Gold_Min";
		public static string Gold_Max = "Gold_Max";
		public static string Total_Type = "Total_Type";
		public static string Total_Count = "Total_Count";
		public static string C_Type_Min = "C_Type_Min";
		public static string C_Type_Max = "C_Type_Max";
		public static string C_Count_Min = "C_Count_Min";
		public static string C_Count_Max = "C_Count_Max";
		public static string B_Type_Min = "B_Type_Min";
		public static string B_Type_Max = "B_Type_Max";
		public static string B_Count_Min = "B_Count_Min";
		public static string B_Count_Max = "B_Count_Max";
		public static string A_Type_Min = "A_Type_Min";
		public static string A_Type_Max = "A_Type_Max";
		public static string A_Count_Min = "A_Count_Min";
		public static string A_Count_Max = "A_Count_Max";
		public static string S_Type_Min = "S_Type_Min";
		public static string S_Type_Max = "S_Type_Max";
		public static string S_Count_Min = "S_Count_Min";
		public static string S_Count_Max = "S_Count_Max";
		public static string SS_Type_Min = "SS_Type_Min";
		public static string SS_Type_Max = "SS_Type_Max";
		public static string SS_Count_Min = "SS_Count_Min";
		public static string SS_Count_Max = "SS_Count_Max";
		public static string Box_IdentificationName = "Box_IdentificationName";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_TreasureDetectBoxGetScriptableObject");
			if (asset != null)
			{
				DB_TreasureDetectBoxGetScriptableObject scriptableObject = asset as DB_TreasureDetectBoxGetScriptableObject;
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
				DB_TreasureDetectBoxGetScriptableObject scriptableObject = asset as DB_TreasureDetectBoxGetScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
