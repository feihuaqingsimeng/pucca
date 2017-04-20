using System;
using UnityEngine;

public class DB_PVPWinBonusMileage : TableBase<DB_PVPWinBonusMileage, DB_PVPWinBonusMileage.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int Mileage_Count;
		public int RewardArea;
		public int Get_Type_MileageReward;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Mileage_Count = "Mileage_Count";
		public static string RewardArea = "RewardArea";
		public static string Get_Type_MileageReward = "Get_Type_MileageReward";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_PVPWinBonusMileageScriptableObject");
			if (asset != null)
			{
				DB_PVPWinBonusMileageScriptableObject scriptableObject = asset as DB_PVPWinBonusMileageScriptableObject;
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
				DB_PVPWinBonusMileageScriptableObject scriptableObject = asset as DB_PVPWinBonusMileageScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
