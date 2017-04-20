using System;
using UnityEngine;

public class DBStr_Character : TableBase<DBStr_Character, DBStr_Character.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Char_Index;
		public string StringData;
	}

	public static class Field
	{
		public static string Char_Index = "Char_Index";
		public static string StringData = "StringData";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_CharacterScriptableObject");
			if (asset != null)
			{
				DBStr_CharacterScriptableObject scriptableObject = asset as DBStr_CharacterScriptableObject;
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
				DBStr_CharacterScriptableObject scriptableObject = asset as DBStr_CharacterScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
