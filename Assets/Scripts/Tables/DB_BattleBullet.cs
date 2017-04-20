using System;
using UnityEngine;

public class DB_BattleBullet : TableBase<DB_BattleBullet, DB_BattleBullet.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Bullet_ID;
		public BATTLE_BULLET_TYPE BATTLE_BULLET_TYPE;
		public float MoveSpeed;
		public int MaxHitCount;
		public string BulletEffectName;
		public string HitEffectName;
		public float Offset_X;
		public float Offset_Y;
	}

	public static class Field
	{
		public static string Bullet_ID = "Bullet_ID";
		public static string BATTLE_BULLET_TYPE = "BATTLE_BULLET_TYPE";
		public static string MoveSpeed = "MoveSpeed";
		public static string MaxHitCount = "MaxHitCount";
		public static string BulletEffectName = "BulletEffectName";
		public static string HitEffectName = "HitEffectName";
		public static string Offset_X = "Offset_X";
		public static string Offset_Y = "Offset_Y";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_BattleBulletScriptableObject");
			if (asset != null)
			{
				DB_BattleBulletScriptableObject scriptableObject = asset as DB_BattleBulletScriptableObject;
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
				DB_BattleBulletScriptableObject scriptableObject = asset as DB_BattleBulletScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
