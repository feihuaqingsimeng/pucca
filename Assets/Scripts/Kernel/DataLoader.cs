using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEngine.UI;
using UnityEngine;




public class DataLoader : Singleton<DataLoader>
{
    public enum DownloadState
    {
        Idle,
        DownloadVersion,
        DownloadAssetBundle,
    }

    public DownloadState downloadState
    {
        get;
        private set;
    }

    public static string DataURL_Base = "http://cdn.huanji.yileweb.com/pucca/PuccaLive/";
    public static string AddURL_Develop = "DEV/";
    public static string AddURL_QA = "QA/";
    public static string AddURL_ADHOC = "ADHOC/";

    public static string DataURL_Release = "http://cdn.huanji.yileweb.com/pucca/PuccaLive/";

    public static string DATA_TABLE_URL_VERSION = "DataVersionList.xml";
    public static string DATA_TABLE_URL_BASE_WINDOW = "PC/";
    public static string DATA_TABLE_URL_BASE_ANDROID = "Android/";
    public static string DATA_TABLE_URL_BASE_IOS = "IOS/";

    private const string TableRootDirectoryName = "DataTable";


    public bool isDownloading { get; private set; }

    public List<DownLoadRetryInfo> m_listRetryInfo = new List<DownLoadRetryInfo>();

    //데이터 리스트 생성용.
    public List<DataTableVerInfo> m_listDataTableVerInfo = new List<DataTableVerInfo>();
    public List<DataTableVerInfo> m_listDownLoadInfo = new List<DataTableVerInfo>();
    public List<DataTableVerInfo> m_listSelfLoadInfo = new List<DataTableVerInfo>();

    public bool isLoadComplete
    {
        get;
        private set;
    }

    public int totalDownloadFileCount
    {
        get;
        private set;
    }

    public int downloadFileCount
    {
        get;
        private set;
    }

    public delegate void LoadEndDelegate();
    public LoadEndDelegate pEndDelegate = null;

    protected override void Awake()
    {
        base.Awake();

        DBStr_UI_BuiltInScriptableObject strUIBuiltInAsset = Resources.Load<DBStr_UI_BuiltInScriptableObject>("TableData/DBStr_UI_BuiltInScriptableObject");
        if (strUIBuiltInAsset != null)
        {
            DBStr_UI_BuiltIn.instance.SetSchemaList(strUIBuiltInAsset.m_SchemaList);
            DBStr_UI_BuiltIn.instance.GenerateCache(DBStr_UI_BuiltIn.Field.TEXT_UI, DBStr_UI_BuiltIn.Field.LanguageCode);
        }
    }

    public IEnumerator LoadData()
    {
        downloadState = DownloadState.Idle;

        //버전 파일 로드.
        yield return StartCoroutine(CheckVersion());

        //데이터 파일 로드.
        yield return StartCoroutine(StartDownLoadDataTables());
        m_listRetryInfo.Clear();

        //로딩종료.
        pEndDelegate();

        downloadState = DownloadState.Idle;
    }




    //버전 파일 로드.
    public IEnumerator CheckVersion()
    {
        downloadState = DownloadState.DownloadVersion;
        yield return StartCoroutine(DownLoadDataTable(GetAssetBaseURL() + DATA_TABLE_URL_VERSION, this.GetType().GetMethod("ParserDataTableVerInfo"), "dataversioninfo"));
    }


    //버전 파일 파싱.
    public void ParserDataTableVerInfo(object _StrData)
    {
        string szData = (string)_StrData;

        XmlDocument pXmlDoc = new XmlDocument();
        pXmlDoc.LoadXml(szData);

        XmlNode pRoot = pXmlDoc.DocumentElement;
        if (pRoot.HasChildNodes)
        {
            XmlNodeList pNodeList = pRoot.ChildNodes[0].ChildNodes;
            for (int idx = 0; idx < pNodeList.Count; idx++)
            {
                XmlNode pTempNode = pNodeList[idx];
                XmlNodeList pNodeData = pTempNode.ChildNodes;
                DataTableVerInfo pAddTable = new DataTableVerInfo();
                pAddTable.m_strFileName = pNodeData[0].InnerXml;
                pAddTable.ver = Convert.ToInt32(pNodeData[1].InnerXml);
                m_listDataTableVerInfo.Add(pAddTable);
            }
        }

        MakeDownLoadList();

        GC.Collect();
    }




    //다운로드 할 리스트 만들기.
    public void MakeDownLoadList()
    {
        totalDownloadFileCount = 0;

        //        string verStr = "_ver";
        foreach (DataTableVerInfo info in m_listDataTableVerInfo)
        {
            // Localization
            string fileName = info.m_strFileName;
            if (fileName.StartsWith("localdb"))
            {
                string languageCode = Kernel.languageCode.ToString();
                if (!fileName.EndsWith(languageCode, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            if (PlayerPrefs.HasKey(fileName))
            {
                //기존 저장된 파일 버전을 받아온다.
                //string szVerKey = PlayerPrefs.GetString(key, "0");  //기본 스트링은 "0.0.0".
                //int localVer = Convert.ToInt32(szVerKey);
                int localVer = PlayerPrefs.GetInt(fileName);

                //버전 파일에서 가져온 버전과 같을때는 로컬에서 불러들임.
                if (info.ver == localVer)
                {
                    /*
                    string strFileNameBuffer = info.m_strFileName;
                    if (CheckFile(strFileNameBuffer))
                    {
                        m_listSelfLoadInfo.Add(info);
                        continue;
                    }
                    */
                }
                else if (info.ver < localVer)   //낮은 버전의 테이블이면 업데이트 리스트에 추가.
                {
                    Debug.LogWarning("낮은 버전의 테이블 : " + info.m_strFileName);
                }
                else
                {
                    totalDownloadFileCount++;
                }
            }
            else
            {
                totalDownloadFileCount++;
            }

            //받아와야할 리스트에 추가.
            m_listDownLoadInfo.Add(info);
        }

        //totalDownloadFileCount = m_listDownLoadInfo.Count;
    }






    //로딩 실패시 재요청.
    public DownLoadRetryInfo GetRetryInfo(string strUrl)
    {
        foreach (DownLoadRetryInfo info in m_listRetryInfo)
        {
            if (info.m_strUrl == strUrl)
                return info;
        }

        return null;
    }



    //테이블 받아오기.
    public IEnumerator DownLoadDataTable(string url, MethodInfo func = null, string strSaveKey = null, string strVer = "")
    {
        url = url + "?" + "timeTicks:" + DateTime.Now.Ticks;
        Debug.Log("Download File : " + url);

        using (WWW www = new WWW(url))
        {
            yield return www;

            if (www.error == null)
            {
                string _str = null;
                using (MemoryStream stream = new MemoryStream(www.bytes))
                {
                    using (TextReader tr = new StreamReader(stream))
                    {
                        _str = tr.ReadToEnd();
                        tr.Close();
                    }
                    stream.Close();
                }

                if (strSaveKey != null)
                {
                    string strFileNameBuffer = strSaveKey;
                    this.WriteStringToFile(_str, strFileNameBuffer);
                    PlayerPrefs.SetString(strSaveKey + "_ver", strVer.ToString());  //정상적으로 받았으면 버전 저장.
                    PlayerPrefs.Save();
                }

                if (func != null)
                {
                    func.Invoke(this, new string[] { _str });
                }
            }
            else
            {
                DownLoadRetryInfo info = GetRetryInfo(url);
                if (info == null)
                {
                    info = new DownLoadRetryInfo();
                    info.m_strUrl = url;
                    info.m_nRetryCount = 1;
                    m_listRetryInfo.Add(info);
                }
                else
                {
                    info.m_nRetryCount++;
                }

                if (info.m_nRetryCount >= 3)
                {
                    //3번 이상 재로딩.
                    yield return null;  //실패로 간주.
                }
                else
                {
                    StartCoroutine(DownLoadDataTable(url, func, strSaveKey));
                    yield return null;
                }

                Debug.Log("receive Error : " + www.error);
            }
        }
    }



    public IEnumerator DownLoadAssetbundle(string url, int ver, string strSaveKey = null)
    {
        //Debug.Log("Download Bundle : " + url + " : Ver_" + ver.ToString());

        AssetBundle bundle = AssetBundleManager.getAssetBundle(url, ver);
        if (!bundle)
        {
            yield return StartCoroutine(AssetBundleManager.downloadAssetBundle(url, ver));
            bundle = AssetBundleManager.getAssetBundle(url, ver);
        }

        if (!string.IsNullOrEmpty(strSaveKey))
        {
            PlayerPrefs.SetInt(strSaveKey, ver);
            PlayerPrefs.Save();
        }

        if (strSaveKey.StartsWith("localdb"))
        {
            Load_LocalData(bundle, Kernel.languageCode);
            AssetBundleManager.Unload(url, ver, false);
        }
        else
        {
            switch (strSaveKey)
            {
                case "gamedb":
                    Load_GameData(bundle);
                    AssetBundleManager.Unload(url, ver, false);
                    break;
                /*
            case "scriptableobject":
                Load_GameData(bundle);
                AssetBundleManager.Unload(url, ver, false);
                break;
                */
                case "puccasound":
                    Kernel.soundManager.LoadSoundData(bundle);
                    AssetBundleManager.Unload(url, ver, false);
                    break;

                case "pucca_pvp_field":
                    Kernel.entry.battle.AssetBundleURL_PVP_Field = url;
                    Kernel.entry.battle.AssetBundleVer_PVP_Field = ver;
                    break;

                case "pucca_pve_field":
                    Kernel.entry.battle.AssetBundleURL_PVE_Field = url;
                    Kernel.entry.battle.AssetBundleVer_PVE_Field = ver;
                    break;

                default:
                    break;
            }
        }

        /*

        using (WWW asset = WWW.LoadFromCacheOrDownload(url, ver))
        {
            // 프로그래스 바로 운용하려는 경우에는 아래와 같이 업데이트단에서 저 값으로 프로그래스를 돌리면된다. 
            //asset.progress
            yield return asset;
            if (asset.error != null)
            {
                Debug.Log("error: " + asset.error.ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(strSaveKey))
                {
                    PlayerPrefs.SetInt(strSaveKey, ver);
                    PlayerPrefs.Save();
                }

                switch (strSaveKey)
                {
                    case "scriptableobject":
                        Load_GameData(asset.assetBundle);
                        break;

                    case "puccasound":
                        Kernel.soundManager.LoadSoundData(asset.assetBundle);
                        break;
                }
            }
            asset.assetBundle.Unload(false);

            asset.Dispose();
        }
         */
    }




    public string GetAssetBaseURL()
    {
        string strURL = "";
        switch (Kernel.gameServerType)
        {
            case GAME_SERVER_TYPE.TYPE_DEV:
            case GAME_SERVER_TYPE.TYPE_MANUAL:
                strURL = DataURL_Base + AddURL_Develop;
                break;

            case GAME_SERVER_TYPE.TYPE_QA:
                strURL = DataURL_Base + AddURL_QA;
                break;

            case GAME_SERVER_TYPE.TYPE_ADHOC:
                strURL = DataURL_Base + AddURL_ADHOC;
                break;

            case GAME_SERVER_TYPE.TYPE_RELEASE:
                strURL = DataURL_Release;
                break;

            case GAME_SERVER_TYPE.TYPE_BUILD_MODE:
#if UNITY_EDITOR
                strURL = DataURL_Base + AddURL_Develop;
#else
                strURL = DataURL_Base + AddURL_QA;
#endif
                break;
        }

        return strURL;
    }




    //테이블 다운로드를 시작한다.
    public IEnumerator StartDownLoadDataTables()
    {
        yield return StartCoroutine(NetDataLoad());
    }




    //다운로드 리스트에 있는 파일들 받아오기.
    public IEnumerator NetDataLoad()
    {
        downloadFileCount = 0;

        isDownloading = true;

        downloadState = DownloadState.DownloadAssetBundle;

        foreach (DataTableVerInfo info in m_listDownLoadInfo)
        {
#if UNITY_ANDROID
            string fileName = GetAssetBaseURL() + DATA_TABLE_URL_BASE_ANDROID + info.m_strFileName + ".asset" + "?" + "timeTicks:" + DateTime.Now.Ticks; //경로를 파일이름으로 자름.
#elif UNITY_STANDALONE_WIN
            string fileName = GetAssetBaseURL() + DATA_TABLE_URL_BASE_WINDOW + info.m_strFileName + ".asset"; //경로를 파일이름으로 자름.
#elif UNITY_IOS
            string fileName = GetAssetBaseURL() + DATA_TABLE_URL_BASE_IOS + info.m_strFileName + ".asset"; //경로를 파일이름으로 자름.
#else
            string fileName = GetAssetBaseURL() + DATA_TABLE_URL_BASE_WINDOW + info.m_strFileName + ".asset"; //경로를 파일이름으로 자름.
#endif


            //            MethodInfo mi = t.GetMethod("ParserDataTableLoader");
            //            yield return StartCoroutine(DownLoadDataTable(fileName, mi, info.m_strFileName, info.ver.ToString()));
            //downloadFileCount++;
            Debug.LogFormat("{0} - {1} ({2}/{3})", fileName, info.ver, downloadFileCount, totalDownloadFileCount);
            yield return StartCoroutine(DownLoadAssetbundle(fileName, info.ver, info.m_strFileName));
            downloadFileCount++;
        }

        isDownloading = false;
        Debug.Log("[DownLoad Complete!!]");
    }






    //데이터테이블 로더. 각 클래스별 로더로 분사시킬것.
    public void ParserDataTableLoader(object _StrData)
    {
        string szData = (string)_StrData;

        XmlDocument pXmlDoc = new XmlDocument();
        pXmlDoc.LoadXml(szData);

        XmlNode pRoot = pXmlDoc.DocumentElement;
        if (pRoot.HasChildNodes)
        {
            XmlNodeList pNodeList = pRoot.ChildNodes;
            for (int idx = 0; idx < pNodeList.Count; idx++)
            {
                XmlNode pTempNode = pNodeList[idx];
            }
        }
    }

    public void Load_LocalData(AssetBundle assetBundle, LanguageCode languageCode)
    {
        if (assetBundle != null)
        {
            DBStr_UI.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_UI), languageCode));
            DBStr_UI.instance.GenerateCache();

            DBStr_Network.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_Network), languageCode));
            DBStr_Network.instance.GenerateCache(DBStr_Network.Field.IndexID);

            DBStr_Skill.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_Skill), languageCode));
            DBStr_Skill.instance.GenerateCache(DBStr_Skill.Field.Skill_Index, DBStr_Skill.Field.SkillType);

            DBStr_Character.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_Character), languageCode));
            DBStr_Character.instance.GenerateCache();

            DBStr_Stage.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_Stage), languageCode));
            DBStr_Stage.instance.GenerateCache(DBStr_Stage.Field.Stage_Id);

            DBStr_AchieveString.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_AchieveString), languageCode));
            DBStr_AchieveString.instance.GenerateCache(DBStr_AchieveString.Field.Achieve_Group);

            DBStr_DailyAchieveString.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_DailyAchieveString), languageCode));
            DBStr_DailyAchieveString.instance.GenerateCache();

            DBStr_Tutorial.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_Tutorial), languageCode));
            DBStr_Tutorial.instance.GenerateCache(DBStr_Tutorial.Field.Index);

            DBStr_TutorialExplain.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_TutorialExplain), languageCode));
            DBStr_TutorialExplain.instance.GenerateCache(DBStr_TutorialExplain.Field.Index);

            DBStr_Tip.instance.FromAssetBundle(assetBundle, string.Format("{0}_{1}ScriptableObject", typeof(DBStr_Tip), languageCode));
        }
    }

    public void Load_GameData(AssetBundle pAssetBundle)
    {
        //데이터테이블.

        #region 기타
        DB_AreaPvP.instance.FromAssetBundle(pAssetBundle);
        DB_AreaPvP.instance.GenerateCache();

        DB_AccountLevel.instance.FromAssetBundle(pAssetBundle);
        DB_AccountLevel.instance.GenerateCache(DB_AccountLevel.Field.AccountLevel);

        DB_Goods.instance.FromAssetBundle(pAssetBundle);
        DB_Goods.instance.GenerateCache(DB_Goods.Field.Index);
        DB_Goods.instance.GenerateCache(DB_Goods.Field.Goods_Type, false);

        DB_TreasureSearch.instance.FromAssetBundle(pAssetBundle);
        DB_TreasureSearch.instance.GenerateCache();

        DB_BoxGet.instance.FromAssetBundle(pAssetBundle);
        DB_BoxGet.instance.GenerateCache();

        DB_Sound.instance.FromAssetBundle(pAssetBundle);
        DB_Sound.instance.GenerateCache();
        #endregion

        #region 전투
        DB_BattleBullet.instance.FromAssetBundle(pAssetBundle);
        DB_BattleBullet.instance.GenerateCache(DB_BattleBullet.Field.Bullet_ID, DB_BattleBullet.Field.BATTLE_BULLET_TYPE);

        DB_Buff.instance.FromAssetBundle(pAssetBundle);
        DB_Buff.instance.GenerateCache();

        DB_Skill.instance.FromAssetBundle(pAssetBundle);
        DB_Skill.instance.GenerateCache(DB_Skill.Field.Index, DB_Skill.Field.SkillType);

        DB_SkillLevelUp.instance.FromAssetBundle(pAssetBundle);
        DB_SkillLevelUp.instance.GenerateCache(DB_SkillLevelUp.Field.Grade_Type, DB_SkillLevelUp.Field.Skill_Level);

        DB_SummonCardData.instance.FromAssetBundle(pAssetBundle);
        DB_SummonCardData.instance.GenerateCache(DB_SummonCardData.Field.SummonIndex);

        #endregion

        #region 카드
        DB_Card.instance.FromAssetBundle(pAssetBundle);
        DB_Card.instance.GenerateCache();

        DB_CardLevelUp.instance.FromAssetBundle(pAssetBundle);
        DB_CardLevelUp.instance.GenerateCache(DB_CardLevelUp.Field.Grade_Type, DB_CardLevelUp.Field.CardLevel);

        DB_EquipLevelUp.instance.FromAssetBundle(pAssetBundle);
        DB_EquipLevelUp.instance.GenerateCache(DB_EquipLevelUp.Field.Goods_Type);

        DB_Soul.instance.FromAssetBundle(pAssetBundle);
        DB_Soul.instance.GenerateCache(DB_Soul.Field.Card_List_Link);
        #endregion

        #region 길드
        DB_EmblemPattern.instance.FromAssetBundle(pAssetBundle);
        DB_EmblemPattern.instance.GenerateCache();

        DB_GuildCardRequest.instance.FromAssetBundle(pAssetBundle);
        DB_GuildCardRequest.instance.GenerateCache();

        DB_GuildDonation.instance.FromAssetBundle(pAssetBundle);
        DB_GuildDonation.instance.GenerateCache();

        DB_GuildLevel.instance.FromAssetBundle(pAssetBundle);
        DB_GuildLevel.instance.GenerateCache(DB_GuildLevel.Field.GulidLevel);

        DB_GuildShopList.instance.FromAssetBundle(pAssetBundle);
        DB_GuildShopList.instance.GenerateCache();
        #endregion

        #region 모험
        DB_StagePVE.instance.FromAssetBundle(pAssetBundle);
        DB_StagePVE.instance.GenerateCache(DB_StagePVE.Field.Index);

        DB_StageMob.instance.FromAssetBundle(pAssetBundle);
        DB_StageMob.instance.GenerateCache(DB_StageMob.Field.GroupIndex);

        DB_StageReward.instance.FromAssetBundle(pAssetBundle);
        DB_StageReward.instance.GenerateCache();

        DB_PVEMobData.instance.FromAssetBundle(pAssetBundle);
        DB_PVEMobData.instance.GenerateCache(DB_PVEMobData.Field.MobIndex);
        #endregion

        #region 랭킹
        DB_SeasonReward.instance.FromAssetBundle(pAssetBundle);
        DB_SeasonReward.instance.GenerateCache();

        DB_DailyReward.instance.FromAssetBundle(pAssetBundle);
        DB_DailyReward.instance.GenerateCache();
        #endregion

        #region 업적
        DB_AchieveList.instance.FromAssetBundle(pAssetBundle);
        DB_AchieveList.instance.GenerateCache(DB_AchieveList.Field.Achieve_Group, DB_AchieveList.Field.Achieve_Step);

        DB_DailyAchieveList.instance.FromAssetBundle(pAssetBundle);
        DB_DailyAchieveList.instance.GenerateCache();
        #endregion

        #region 일반상점
        DB_NormalShop.instance.FromAssetBundle(pAssetBundle);
        DB_NormalShop.instance.GenerateCache();
        #endregion

        #region 수상한 상점
        DB_StrangeShopProperty.instance.FromAssetBundle(pAssetBundle);
        DB_StrangeShopProperty.instance.GenerateCache(DB_StrangeShopProperty.Field.Grade_Type, DB_StrangeShopProperty.Field.Goods_Type);
        #endregion

        #region 비밀 거래
        DB_SecretExchange.instance.FromAssetBundle(pAssetBundle);
        DB_SecretExchange.instance.GenerateCache();
        DB_SecretExchangeSlot.instance.FromAssetBundle(pAssetBundle);
        DB_SecretExchangeSlot.instance.GenerateCache();
        DB_LegendBox.instance.FromAssetBundle(pAssetBundle);
        DB_LegendBox.instance.GenerateCache();
        #endregion

        #region 가맹점
        DB_FranchiseStructure.instance.FromAssetBundle(pAssetBundle);
        DB_FranchiseStructure.instance.GenerateCache(DB_FranchiseStructure.Field.BuildingNum, DB_FranchiseStructure.Field.FloorNum);
        DB_FranchiseExpansion.instance.FromAssetBundle(pAssetBundle);
        DB_FranchiseExpansion.instance.GenerateCache();
        DB_FranchiseExpansionInfo.instance.FromAssetBundle(pAssetBundle);
        DB_FranchiseExpansionInfo.instance.GenerateCache();
        #endregion

        #region 보물탐색(AR)
        DB_TreasureDetectMap.instance.FromAssetBundle(pAssetBundle);
        DB_TreasureDetectMap.instance.GenerateCache(DB_TreasureDetectMap.Field.Index);
        DB_TreasureDetectBoxGet.instance.FromAssetBundle(pAssetBundle);
        DB_TreasureDetectBoxGet.instance.GenerateCache(DB_TreasureDetectBoxGet.Field.Index);
        #endregion

        #region 튜토리얼
        DB_Tutorial.instance.FromAssetBundle(pAssetBundle);
        DB_Tutorial.instance.GenerateCache(DB_Tutorial.Field.Index, DB_Tutorial.Field.GroupIndex);
        DB_TutorialGroup.instance.FromAssetBundle(pAssetBundle);
        DB_TutorialGroup.instance.GenerateCache(DB_TutorialGroup.Field.Index);
        #endregion

        #region IAP
        DB_ProductMain.instance.FromAssetBundle(pAssetBundle);
        DB_ProductMain.instance.GenerateCache(DB_ProductMain.Field.Index);
        DB_ProductMain.instance.GenerateCache(DB_ProductMain.Field.AppStoreProductId, false);
        DB_ProductMain.instance.GenerateCache(DB_ProductMain.Field.PlayStoreProductId, false);
        DB_ProductMain.instance.GenerateCache(DB_ProductMain.Field.PackageId, false);

        DB_ProductPackage.instance.FromAssetBundle(pAssetBundle);
        DB_ProductPackage.instance.GenerateCache(DB_ProductPackage.Field.PackageId);

        DB_ProductName.instance.FromAssetBundle(pAssetBundle);
        DB_ProductName.instance.GenerateCache(DB_ProductName.Field.Index);

        // 로그인 이후로 이동
        // Kernel.iapManager.InitializePurchasing();

        DB_Package_BoxGet.instance.FromAssetBundle(pAssetBundle);
        DB_Package_BoxGet.instance.GenerateCache(DB_Package_BoxGet.Field.Index);
        #endregion

        /*
        DB_BoxReward.instance.Load(pAssetBundle.LoadAsset("DB_BoxRewardScriptableObject"));
        DB_BoxReward.instance.GenerateCache();
        DBStr_Tutorial.instance.Load(pAssetBundle.LoadAsset("DBStr_TutorialScriptableObject"));
        DBStr_Tutorial.instance.GenerateCache();
        */



        //Docspin 데이터 로드.
        if (Kernel.UseClientTable)
        {
            LoadDocspinTable.LoadDocspinTableData(DOCSPIN_TABLE_KIND.Data_DB_CARD);
            LoadDocspinTable.LoadDocspinTableData(DOCSPIN_TABLE_KIND.Data_DB_Buff);
            LoadDocspinTable.LoadDocspinTableData(DOCSPIN_TABLE_KIND.Data_DB_PVEMobData);
            LoadDocspinTable.LoadDocspinTableData(DOCSPIN_TABLE_KIND.Data_DB_Skill);
            LoadDocspinTable.LoadDocspinTableData(DOCSPIN_TABLE_KIND.Data_DB_StageMob);
            LoadDocspinTable.LoadDocspinTableData(DOCSPIN_TABLE_KIND.Data_DB_StageReward);
        }



        isLoadComplete = true;
    }

    public void WriteStringToFile(string str, string filename)
    {
        string path = Path.Combine(Application.temporaryCachePath, filename);
        using (FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(str);
                sw.Close();
            }
            file.Close();
        }
    }

    #region
    public bool CheckFile(string fileName)
    {
        string path = pathForDocumentsFile(fileName);

        return File.Exists(path);
    }

    // 로컬에서 받아올 파일들 처리.
    public void LocalDataLoad()
    {
        Type t = this.GetType();

        foreach (DataTableVerInfo info in m_listSelfLoadInfo)
        {
            Debug.Log("Download SctiptableObject : [Local] - " + info.m_strFileName.ToString());

            MethodInfo mi = t.GetMethod("ParserDataTableLoader");
            LoadLocalDataTable(info.m_strFileName, mi);
        }
    }

    //데이터 로드.
    public void LoadLocalDataTable(string strKey, MethodInfo func)
    {
        string strFileNameBuffer = strKey;
        string _str = ReadStringFromFile(strFileNameBuffer);

        func.Invoke(this, new string[] { _str });
    }

    public string ReadStringFromFile(string filename)//, int lineIndex )
    {
        string path = pathForDocumentsFile(filename);

        if (File.Exists(path))
        {
            string str = null;
            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    str = sr.ReadToEnd();
                    sr.Close();
                }
                file.Close();
            }

            return str;
        }

        return null;
    }

    public string pathForDocumentsFile(string filename)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(Path.Combine(path, "Documents"), filename);
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, filename);
        }
        else
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            path = Path.Combine(path, TableRootDirectoryName);
            return Path.Combine(path, filename);
        }
    }
    #endregion
}







// code data ver info 
[System.Serializable]
public class DataTableVerInfo
{
    public string m_strFileName; 				// 파일이름 
    public int ver;						// 버전
};


public class DownLoadRetryInfo
{
    public string m_strUrl;
    public int m_nRetryCount;
};

