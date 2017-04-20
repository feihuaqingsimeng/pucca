using System;
using UnityEngine;

public class DBStr_TrainingHall : TableBase<DBStr_TrainingHall, DBStr_TrainingHall.Schema>
{
	[Serializable]
	public class Schema
	{
		public int Index;
		public string Training_Name;
		public string Training_Desc_1;
		public string Training_Desc_2;
		public string Training_Req_Desc_1;
		public string Training_Req_Desc_2;
		public string Icon_Image;
	}

	public static class Field
	{
		public static string Index = "Index";
		public static string Training_Name = "Training_Name";
		public static string Training_Desc_1 = "Training_Desc_1";
		public static string Training_Desc_2 = "Training_Desc_2";
		public static string Training_Req_Desc_1 = "Training_Req_Desc_1";
		public static string Training_Req_Desc_2 = "Training_Req_Desc_2";
		public static string Icon_Image = "Icon_Image";
	}

	public override bool FromAssetBundle(AssetBundle assetBundle)
	{
		if (assetBundle != null)
		{
			UnityEngine.Object asset = assetBundle.LoadAsset("DBStr_TrainingHallScriptableObject");
			if (asset != null)
			{
				DBStr_TrainingHallScriptableObject scriptableObject = asset as DBStr_TrainingHallScriptableObject;
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
				DBStr_TrainingHallScriptableObject scriptableObject = asset as DBStr_TrainingHallScriptableObject;
				if (scriptableObject != null)
				{
					return instance.SetSchemaList(scriptableObject.m_SchemaList);
				}
			}
		}

		return false;
	}
}
