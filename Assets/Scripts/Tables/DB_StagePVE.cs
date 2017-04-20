using System;
using UnityEngine;

public class DB_StagePVE : TableBase<DB_StagePVE, DB_StagePVE.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public int NextStageID;
		public STAGE_TYPE STAGE_TYPE;
		public int Need_Heart;
		public int TimeOut;
		public int AccountEXP;
		public int MobGroup_Id;
		public int Reward_Group_Link;
		public string Stage_BG;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string NextStageID = "NextStageID";
		public static string STAGE_TYPE = "STAGE_TYPE";
		public static string Need_Heart = "Need_Heart";
		public static string TimeOut = "TimeOut";
		public static string AccountEXP = "AccountEXP";
		public static string MobGroup_Id = "MobGroup_Id";
		public static string Reward_Group_Link = "Reward_Group_Link";
		public static string Stage_BG = "Stage_BG";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_StagePVEScriptableObject");
			if (asset != null)
			{
				DB_StagePVEScriptableObject scriptableObject = asset as DB_StagePVEScriptableObject;
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
				DB_StagePVEScriptableObject scriptableObject = asset as DB_StagePVEScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
