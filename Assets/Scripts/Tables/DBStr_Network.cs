using System;
using UnityEngine;

public class DBStr_Network : TableBase<DBStr_Network, DBStr_Network.Schema>
{
	[Serializable]
	public class Schema
	{
		public int IndexID;
		public string StringData;
		public bool IsSysMsg;
	}

	public static class Field
	{
		public static string IndexID = "IndexID";
		public static string StringData = "StringData";
		public static string IsSysMsg = "IsSysMsg";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_NetworkScriptableObject");
			if (asset != null)
			{
				DBStr_NetworkScriptableObject scriptableObject = asset as DBStr_NetworkScriptableObject;
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
				DBStr_NetworkScriptableObject scriptableObject = asset as DBStr_NetworkScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
