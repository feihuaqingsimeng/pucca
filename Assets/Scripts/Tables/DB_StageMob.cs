using System;
using UnityEngine;

public class DB_StageMob : TableBase<DB_StageMob, DB_StageMob.Schema>
{
	[Serializable]
	public class Schema
	{
		public int GroupIndex;
		public int NextGroup;
		public float SpawnDelay;
		public float StatRevision;
		public int LeaderIndex;
		public int BossIndex;
		public int MobIndex_1;
		public int MobIndex_2;
		public int MobIndex_3;
		public int MobIndex_4;
		public int MobIndex_5;
	}

	public static class Field
	{
		public static string GroupIndex = "GroupIndex";
		public static string NextGroup = "NextGroup";
		public static string SpawnDelay = "SpawnDelay";
		public static string StatRevision = "StatRevision";
		public static string LeaderIndex = "LeaderIndex";
		public static string BossIndex = "BossIndex";
		public static string MobIndex_1 = "MobIndex_1";
		public static string MobIndex_2 = "MobIndex_2";
		public static string MobIndex_3 = "MobIndex_3";
		public static string MobIndex_4 = "MobIndex_4";
		public static string MobIndex_5 = "MobIndex_5";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_StageMobScriptableObject");
			if (asset != null)
			{
				DB_StageMobScriptableObject scriptableObject = asset as DB_StageMobScriptableObject;
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
				DB_StageMobScriptableObject scriptableObject = asset as DB_StageMobScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
