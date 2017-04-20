using System;
using UnityEngine;

public class DB_Sound : TableBase<DB_Sound, DB_Sound.Schema>
{
	[Serializable]
	public class Schema
	{
		public SOUND SOUND;
		public string sndName;
		public SOUND_TYPE SOUND_TYPE;
	}

	public static class Field
	{
		public static string SOUND = "SOUND";
		public static string sndName = "sndName";
		public static string SOUND_TYPE = "SOUND_TYPE";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DB_SoundScriptableObject");
			if (asset != null)
			{
				DB_SoundScriptableObject scriptableObject = asset as DB_SoundScriptableObject;
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
				DB_SoundScriptableObject scriptableObject = asset as DB_SoundScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
