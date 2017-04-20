using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Common.Packet;
using Common.Util;
using System.Linq;



public enum BATTLE_KIND
{
    PVP_BATTLE = 0,
    PVE_BATTLE,
    REVENGE_BATTLE,
    TEST_BATTLE,
}


public enum BATTLE_STATE_KIND
{
    PRE_LOADING = 0,
    INIT,
    SHOW_BATTLE_INFO,
    MOVE_START,
    TUTORIAL_PRE_BATTLE,
    BATTLE_START,
    BATTLE,
    BATTLE_END,
    PHASE_END,
    NETWORK_BATTLE_RESULT,
    SHOW_RESULT,
    BATTLE_EXIT,
    END_BATTLE_MODE
}


public enum BATTLE_RESULT_STATE
{
    NONE = 0,
    WIN,
    LOSE,
    DRAW
}


public class BattleGroupInfo
{
    public  int     GroupIndex;
    public  float   SpawnTime_Cur;
    public  float   SpawnTime_Max;

    public  DB_StageMob.Schema  Groupdata;
}




public class BattleManager : MonoBehaviour 
{
    public BATTLE_KIND          CurBattleKind;
    public bool                 OneKillMode_Hero;
    public bool                 OneKillMode_Enemy;

    public bool                 SuperMode;

    //각 파트 매니저들.
    [HideInInspector]
    public BattleFieldManager   pBattleFieldMng;
    [HideInInspector]
    public EffectPoolManager    pEffectPoolMng;
    [HideInInspector]
    public UIBattle             pBattleUI;
    [HideInInspector]
    public UIBattleStart        pBattleStartUI;

    //카메라.
    [HideInInspector]
    public GameObject           MainCameraObject;
    [HideInInspector]
    public CameraShake          pCamShake;

    //EX 카메라 연출.
    public bool                 ExCamEffect;
    public bool                 ExCamEffect_HeroTeam;
    public int                  ExCamEffect_State;
    public float                ExCamEffect_CurTime;
    public float                ExCamEffect_MaxTime;
    public float                ExCamEffect_CurSize;
    public float                ExCamEffect_CurPosX;
    public float                ExCamEffect_CurPosY;

    public float                ExCamZoomIn_Speed;
    public float                ExCamMoveIn_Speed;
    public float                ExCamZoomOut_Speed;
    public float                ExCamMoveOut_Speed;


    public Camera               MainCamera;
    public Camera               EffectCamera;
    public float                BaseCamSize = 3.6f;
    public float                MaxCamSize = 3.0f;
    public Vector3              BaseLookPosition = new Vector3(0.0f, 0.0f, 0.0f);
    public Vector3              BaseLookOffset = new Vector3(0.0f, 0.05f, -10.0f);
    public Vector3              TargetLookPosition;


    //페이드스크린.
    public FadeScreenCS         SkillFadeScreen;

    //EX 페이드 스크린.
    public FadeScreenCS         ExSkillFadeScreen;


    //몬스터리스트.
    public List<BattlePawn> HeroPawnList = new List<BattlePawn>();
    public List<BattlePawn> EnemyPawnList = new List<BattlePawn>();

    //객체데이터.
    private Transform       PawnParentTarget;


    //현재 상태 제어용.
    public  BATTLE_STATE_KIND   eBattleStateKind;
    private bool                PreLoadingComplete;
    private bool                BattleInitComplete;

    //프리로딩.
    [HideInInspector]
    public  GameObject          PawnHPGaugePrefab;
    [HideInInspector]
    public  GameObject          EnemyHPGaugePrefab;


    //공용 이펙트.
    [HideInInspector]
    public   int                EffectID_DamageNormal;  //기본 히트.
    [HideInInspector]
    public   int                EffectID_HealNormal;    //기본 힐.

    [HideInInspector]
    public   int                EffectID_UseSkill_Leader;      //기본 스킬사용.
    [HideInInspector]
    public   int                EffectID_UseSkill;      //기본 스킬사용.
    [HideInInspector]
    public   int                EffectID_MaxSkill;      //맥스스킬 버서크.
    [HideInInspector]
    public   int                EffectID_Die;
    [HideInInspector]
    public  int                 EffectID_Bomb;
    [HideInInspector]
    public  int                 EffectID_Smoke;
    [HideInInspector]
    public   Vector3            pEffectAddPos_Z = new Vector3(0.0f, 0.0f, 0.5f);        //이펙트 추가시 Z축 보정.


    //일시정지관련.
    [HideInInspector]
    public  bool                ApplicationPauseMode;
    [HideInInspector]
    public  bool                GamePauseMode;

    //리더스킬 사용부분.
    private bool                bLeaderSkillMode_Hero;
    private bool                AlreadyUseLeaderSkill_Hero;
    private bool                bLeaderSkillMode_Enemy;
    private bool                AlreadyUseLeaderSkill_Enemy;



    //버프정보 관련.
    public  GameObject              BuffInfoElementPrefab;
    [HideInInspector]
    public  List<BuffInfoElement>   BuffInfoList_Hero = new List<BuffInfoElement>();
    [HideInInspector]
    public  List<BuffInfoElement>   BuffInfoList_Enemy = new List<BuffInfoElement>();



    //폰 수.
    [HideInInspector]
    public  int                 MaxPawnCount = 5;

    //승패관련.
    private BATTLE_RESULT_STATE eBattleResultState;  //아군의 승리.

    //전투시간.
    [HideInInspector]
    public  float               MaxBattleTime = 120.0f; //2분.
    [HideInInspector]
    public  float               CurBattleTime = 0.0f;
            

    //EX스킬.
    [HideInInspector]
    public  bool                UseExSkill_H = false;
    [HideInInspector]
    public  bool                UseExSkill_E = false;

    [HideInInspector]
    public  bool                SkillUseDelay = false;
    [HideInInspector]
    public  float               CurSkillUseDelayTime;


    [HideInInspector]
    public  bool                EnemySkillUseDelay = false;
    [HideInInspector]
    public  float               CurEnemySkillUseDelayTime;


    [HideInInspector]
    public  float               MaxSkillUseDelayTime = 0.5f;



    //지역이름.
    [HideInInspector]
    public string               AreaName;
    [HideInInspector]
    public string               EnemyTeamName;

    //스킬AI Check.
    [HideInInspector]
    public  int                 Check_TeamHitCount;
    [HideInInspector]
    public  int                 Check_EnemyTeamHitCount;
    [HideInInspector]
    public  float               Check_TeamHP;
    [HideInInspector]
    public  float               Check_EnemyTeamHP;




    //콤보.
    public  int                 CurComboCount_H;
    public  int                 CurComboCount_E;
    public  int                 MaxComboCount = 10;


    //헌신 스킬용.
    public  bool                HardTankingMode_Hero;
    public  BattlePawn          HardTankingPawn_Hero;
    public  bool                HardTankingMode_Enemy;
    public  BattlePawn          HardTankingPawn_Enemy;




    [HideInInspector]
    public  Transform           PVE_FieldCamPos;
    private LerpFollowTarget    PVE_FieldCamManager; 




    //표시 가능한 버프 종류 배열.
    BUFF_KIND[] AddBuffList = new BUFF_KIND[]
        {
            BUFF_KIND.SLOW,
            BUFF_KIND.CRITICALRATE_UP,
            BUFF_KIND.CRITICALRATE_DOWN,
            BUFF_KIND.ATT_UP,
            BUFF_KIND.ATT_DOWN,
            BUFF_KIND.DEF_UP,
            BUFF_KIND.DEF_DOWN,
            BUFF_KIND.EVADERATE_UP,
            BUFF_KIND.EVADERATE_DOWN,
            BUFF_KIND.ACCURATE_UP,
            BUFF_KIND.ACCURATE_DOWN,
            BUFF_KIND.CRITICALDMG_UP,
            BUFF_KIND.CRITICALDMG_DOWN,
            BUFF_KIND.FAST,
            BUFF_KIND.POISON,
            BUFF_KIND.DOT_DAMAGE,
            BUFF_KIND.SACRIFICE,
            BUFF_KIND.SILENCE,
            BUFF_KIND.HEALING_CONSIST,
            BUFF_KIND.TOTEM_HEALING_CONSIST,
            BUFF_KIND.HARD_TANKING,
            BUFF_KIND.SKILL_SHIELD,
            BUFF_KIND.FREEZE
        };






    //PVE.
    [HideInInspector]
    public  int                     PVE_BattleGroupIndex;
    [HideInInspector]
    public  BattleGroupInfo[]       BattleGroupArray;
    [HideInInspector]
    public  int                     PVE_BossCardIndex;

    public  GameObject              PVE_BossScreenEffect;


    //배틀로그.
    [HideInInspector]
    public  BattleLog   MyBattleLog = new BattleLog();




    //수호자 밀어내기 효과.
    [HideInInspector]
    public  bool        ChargingKeeper_HeroTeam;

    [HideInInspector]
    public  bool        ChargingKeeper_EnemyTeam;



    //스킬사용 제어용.
    List<BattlePawn>    SortingBaseList = new List<BattlePawn>();
    List<BattlePawn>    SortingList_Healer = new List<BattlePawn>();
    List<BattlePawn>    SortingList_MaxCoolTime = new List<BattlePawn>();
    List<BattlePawn>    SortingList_HighSkillValue = new List<BattlePawn>();
    List<BattlePawn>    SortingList_HighComboCost = new List<BattlePawn>();
    List<BattlePawn>    SortingList_SameList = new List<BattlePawn>();
    List<BattlePawn>    SortingList_JobList = new List<BattlePawn>();
    [HideInInspector]
    public  bool        WaitUseSkillCheck_H;
    [HideInInspector]
    public  bool        WaitUseSkillCheck_E;



    //테스트모드.
    [HideInInspector]
    public  BattleTestModule    TestModuleMng;


    void Awake()
    {
        if (CurBattleKind != BATTLE_KIND.TEST_BATTLE)
            CurBattleKind = Kernel.entry.battle.CurBattleKind;
        else
        {
            //테스트모드일때.
            GameObject TestModuleObj = GameObject.Find("BattleTestModule");
            if (TestModuleObj == null)
                return;

            TestModuleMng = TestModuleObj.GetComponent<BattleTestModule>();
            if (TestModuleMng == null)
                return;
        }

        //전투 시작시 레벨.
        Kernel.entry.battle.BattleStartLevel = Kernel.entry.account.level;

        //배틀필드 매니저.
        pBattleFieldMng = GameObject.Find("MapManager").GetComponent<BattleFieldManager>();

        //메인 카메라 오브젝트.
        MainCameraObject = GameObject.Find("BattleCameraObject") as GameObject;

        //카메라효과.
        pCamShake = GameObject.Find("BattleCamera").GetComponent<CameraShake>();

        //PVE용 캠 타겟.
        PVE_FieldCamPos = GameObject.Find("PVE_FieldCamPos").transform;
        PVE_FieldCamManager = PVE_FieldCamPos.GetComponent<LerpFollowTarget>();

            //체력바 프리팹.
        PawnHPGaugePrefab = Resources.Load("Prefabs/Battle/BattleGauge_HP") as GameObject;
        EnemyHPGaugePrefab = Resources.Load("Prefabs/Battle/BattleGauge_HP_Enemy") as GameObject;


        //폰 부모 트랜스폼.
        PawnParentTarget = GameObject.Find("PawnParent").transform;
        if (PawnParentTarget == null)   //대상이 없으면 BattleManager 하위로 집어넣어 관리.
            PawnParentTarget = transform;

        //이펙트풀 매니저.
        pEffectPoolMng = GameObject.Find("EffectPoolManager").GetComponent<EffectPoolManager>();
        SetBattleReset_EffectPool();

        //PVE_보스스크린이펙트.
        PVE_BossScreenEffect.SetActive(false);



        PreLoadingComplete = false;
        BattleInitComplete = false;
        ChangeBattleState(BATTLE_STATE_KIND.PRE_LOADING);


        GamePauseMode = false;
        eBattleResultState = BATTLE_RESULT_STATE.NONE;
        AlreadyUseLeaderSkill_Hero = false;
        AlreadyUseLeaderSkill_Enemy = false;

        
        //전투시간.
        CurBattleTime = 0.0f;

        SkillUseDelay = false;
        CurSkillUseDelayTime = 0.0f;
        EnemySkillUseDelay = false;
        CurEnemySkillUseDelayTime = 0.0f;


        //콤보.
        CurComboCount_H = 0;
        CurComboCount_E = 0;

        //하드탱킹.
        HardTankingMode_Hero = false;
        HardTankingPawn_Hero = null;
        HardTankingMode_Enemy = false;
        HardTankingPawn_Enemy = null;


        //지역이름.
        switch (CurBattleKind)
        {
            case BATTLE_KIND.PVE_BATTLE:
                DBStr_Stage.Schema StageInfo = DBStr_Stage.Query(DBStr_Stage.Field.Stage_Id, Kernel.entry.adventure.SelectStageIndex);
                if (StageInfo != null)
                {
                    AreaName = StageInfo.StageName;
                    EnemyTeamName = "";
                }
                break;

            case BATTLE_KIND.REVENGE_BATTLE:
                BattleLog  EnmeyLog = null;
                BattleLogUtility.TryGetBattleLog(Kernel.entry.battle.RevengeMatchInfoData.m_sDeckInfo, out EnmeyLog);

                DB_AreaPvP.Schema RevengeAreaInfo = DB_AreaPvP.Query(DB_AreaPvP.Field.Index, EnmeyLog.currentPvPArea);
                if (RevengeAreaInfo != null)
                {
                    AreaName = Languages.ToString(RevengeAreaInfo.TEXT_UI);
                    EnemyTeamName = "";
                }
                break;

            default:
                DB_AreaPvP.Schema AreaInfo = DB_AreaPvP.Query(DB_AreaPvP.Field.Index, Kernel.entry.account.currentPvPArea);
                if (AreaInfo != null)
                {
                    AreaName = Languages.ToString(AreaInfo.TEXT_UI);
                    EnemyTeamName = "";
                }
                break;
        }
    }



    void OnEnable()
    {
        //튜토리얼정보.
        if (Kernel.entry.tutorial.TutorialActive)
            Kernel.entry.tutorial.onTutorialBattleDelegate += TutorialResume;
    }

    void OnDisable()
    {
        //튜토리얼정보.
        if (Kernel.entry.tutorial.TutorialActive)
            Kernel.entry.tutorial.onTutorialBattleDelegate -= TutorialResume;
    }
    
    
    
    
    
    
    
    
    
    
    //전투 초기화 전 프리로딩.
    public void PreloadBattle()
    {
        //전투 배경 로딩.
        switch (CurBattleKind)
        {
            case BATTLE_KIND.PVE_BATTLE:
                DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, Kernel.entry.adventure.SelectStageIndex);
                pBattleFieldMng.LoadBattleField_PVE(PVE_FieldCamPos, StageData.Stage_BG);

                MaxBattleTime = StageData.TimeOut;
                break;

            default:
                DB_AreaPvP.Schema AreaData = DB_AreaPvP.Query(DB_AreaPvP.Field.Index, Kernel.entry.account.currentPvPArea);
                pBattleFieldMng.LoadBattleField(AreaData.Battle_Bg);

                MaxBattleTime = 120.0f;
                break;
        }
        PreLoadingComplete = true;
    }









    //이펙트풀 등록.
    public void SetBattleReset_EffectPool()
    {
        pEffectPoolMng.InitEffectPoolManager();

        //기본 히트 이펙트.
        EffectID_DamageNormal = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Hit_Basic_seq") as GameObject, false,  20);

        //기본 힐 이펙트.
        EffectID_HealNormal = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Heal_Fx") as GameObject, false, 20);

        //리더버프용 이펙트.
        EffectID_UseSkill_Leader = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Battle_Crown") as GameObject, false, 20);

        //기본 스킬사용 이펙트.
        EffectID_UseSkill = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_SkillActive_seq") as GameObject, true, 5);

        //기본 스킬사용 이펙트.
        EffectID_MaxSkill = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_MaxSkill") as GameObject, true, 5);

        //사망 이펙트.(임시).
        EffectID_Die = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Die_Fx") as GameObject, true, 12);

        //사망 이펙트.(임시).
        EffectID_Bomb = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Hit_StuffBomb") as GameObject, true, 4);

        //펑 이펙트.
        EffectID_Smoke = pEffectPoolMng.AddEffectPool(Resources.Load("Effects/EF_Kagemusha_smoke_001") as GameObject, true, 4);
    }














    //전투 초기화.
    public void InitBattle()
    {
        //UI 로드.
        pBattleUI = GameObject.FindObjectOfType<UIBattle>();
        pBattleStartUI = GameObject.FindObjectOfType<UIBattleStart>();

        //폰 로드.
        switch (CurBattleKind)
        {
            case BATTLE_KIND.PVP_BATTLE:
                LoadBattlePawn_MyTeam();
                LoadBattlePawn_PVP_Enemy(false);
                break;

            case BATTLE_KIND.PVE_BATTLE:
                LoadBattlePawn_MyTeam();
                LoadBattlePawn_PVE_Enemy();
                break;

            case BATTLE_KIND.REVENGE_BATTLE:
                LoadBattlePawn_MyTeam();
                LoadBattlePawn_PVP_Enemy(true);
                break;

            case BATTLE_KIND.TEST_BATTLE:
                TestLoadBattlePawn();
                break;
        }

        BattleInitComplete = true;


        //UI설정.
        switch (CurBattleKind)
        {
            case BATTLE_KIND.PVP_BATTLE:
            case BATTLE_KIND.TEST_BATTLE:
                pBattleUI.SetBattleUI();
                break;

            case BATTLE_KIND.PVE_BATTLE:
                pBattleUI.SetBattleUI_PVE();
                break;

            case BATTLE_KIND.REVENGE_BATTLE:
                pBattleUI.SetRevengeBattleUI();
                break;
        }

        //버프리스트.
        InitBuffInfoList();

        //PVE.
        PVE_BattleGroupIndex = 0;

        //글로벌 스킬 딜레이.
        MaxSkillUseDelayTime = 1.0f;
    }












    //폰 생성.
    public void LoadBattlePawn_MyTeam()
    {
        //아군.
        BattlePawn tempPawnData = null;

        CDeckData HeroDeckData = Kernel.entry.character.FindMainDeckData();

        for (int idx = 0; idx < HeroDeckData.m_CardCidList.Count; idx++)
        {
            if (idx >= MaxPawnCount)
                break;

            long CardCID = HeroDeckData.m_CardCidList[idx];
            if (CardCID <= 0 || idx >= MaxPawnCount)
            {
                pBattleUI.BattleCardList[idx].SetEmptyCard();
                continue;
            }

            int TempPawnKey = 100 + idx;    //100은 아군.
            bool bLeader = false;
            if (CardCID == HeroDeckData.m_LeaderCid)
                bLeader = true;

            tempPawnData = GenerateBattlePawn(TempPawnKey, CardCID, true, bLeader, idx);
            if (tempPawnData == null)
            {
                pBattleUI.BattleCardList[idx].SetEmptyCard();
                continue;
            }

            //아군이면 UI도 초기화.
            pBattleUI.BattleCardList[idx].InitBattleCard(pBattleUI, tempPawnData);

            if (tempPawnData != null)
                HeroPawnList.Add(tempPawnData);

            //스킬 AI모드 끄기.
            if(Kernel.entry.battle.CurBattleKind != BATTLE_KIND.PVE_BATTLE)
                tempPawnData.SkillManager.AI_Mode = false;
        }
    }



    //적 생성 - PVP.
    public void LoadBattlePawn_PVP_Enemy(bool bRevengeMode)
    {
        //적 추가.
        BattlePawn tempPawnData = null;

        CDeckData EnemyDeckData = null;

        if (bRevengeMode)
        {
            BattleLog EnmeyLog = null;
            BattleLogUtility.TryGetBattleLog(Kernel.entry.battle.RevengeMatchInfoData.m_sDeckInfo, out EnmeyLog);

            EnemyDeckData = EnmeyLog.m_DeckData;
        }
        else
        {
            EnemyDeckData = Kernel.entry.battle.PVP_Deck;
        }


        for (int idx = 0; idx < EnemyDeckData.m_CardCidList.Count; idx++)
        {
            if (idx >= MaxPawnCount)
                break;

            long CardCID = EnemyDeckData.m_CardCidList[idx];
            if (CardCID <= 0 || idx >= MaxPawnCount)
                continue;

            int TempPawnKey = 200 + idx;
            bool bLeader = false;
            if (CardCID == EnemyDeckData.m_LeaderCid)
                bLeader = true;

            tempPawnData = GenerateBattlePawn(TempPawnKey, CardCID, false, bLeader, idx);

            if (tempPawnData == null)
                continue;

            if (tempPawnData != null)
                EnemyPawnList.Add(tempPawnData);


            //적은 스킬 AI 발동.
            tempPawnData.SkillManager.AI_Mode = true;

        }



        //배틀로그 생성.
        MyBattleLog.currentPvPArea = Kernel.entry.account.currentPvPArea;
        MyBattleLog.GuildName = Kernel.entry.guild.guildName;
        MyBattleLog.GuildEmblem = Kernel.entry.guild.guildEmblem;
        MyBattleLog.RankPoint = Kernel.entry.account.rankingPoint;

        CDeckData mainDeckData = Kernel.entry.character.FindMainDeckData();
        if (mainDeckData != null)
        {
            MyBattleLog.m_DeckData = mainDeckData;

            if (MyBattleLog.m_CardInfoList == null)
            {
                MyBattleLog.m_CardInfoList = new List<CCardInfo>();
            }
            for (int i = 0; i < mainDeckData.m_CardCidList.Count; i++)
            {
                CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(mainDeckData.m_CardCidList[i]);
                if (cardInfo != null)
                {
                    MyBattleLog.m_CardInfoList.Add(cardInfo);
                }
                else Debug.LogError(mainDeckData.m_CardCidList[i]);
            }
        }
        //MyBattleLog.m_CardInfoList = Kernel.entry.character.cardInfoList;
        //MyBattleLog.m_DeckData = Kernel.entry.character.FindMainDeckData();
    }










    //적 생성 - PVE.
    public void LoadBattlePawn_PVE_Enemy()
    {
        //적군 정보.
        DB_StagePVE.Schema StageData = DB_StagePVE.Query(DB_StagePVE.Field.Index, Kernel.entry.adventure.SelectStageIndex);
        BattlePawn tempPawnData = null;


        int nAddEnemyCount = 0;
        int CurGroupIndex = StageData.MobGroup_Id;

        //그룹 카운팅.

        int MaxGroupCount = 0;
        int TempGroupID = StageData.MobGroup_Id;
        while (true)
        {
            MaxGroupCount++;
            DB_StageMob.Schema Groupdata = DB_StageMob.Query(DB_StageMob.Field.GroupIndex, TempGroupID);
            if (Groupdata.NextGroup == 0)
                break;
            else
                TempGroupID = Groupdata.NextGroup;
        }

        //그룹 수 만큼 배열 생성.
        int nSettingCount = 0;
        BattleGroupArray = new BattleGroupInfo[MaxGroupCount];
        TempGroupID = StageData.MobGroup_Id;
        while (true)
        {
            BattleGroupArray[nSettingCount] = new BattleGroupInfo();
            BattleGroupArray[nSettingCount].Groupdata = DB_StageMob.Query(DB_StageMob.Field.GroupIndex, TempGroupID);
            BattleGroupArray[nSettingCount].GroupIndex = TempGroupID;
            BattleGroupArray[nSettingCount].SpawnTime_Cur = 0.0f;
            BattleGroupArray[nSettingCount].SpawnTime_Max = BattleGroupArray[nSettingCount].Groupdata.SpawnDelay;

            if (nSettingCount == 0)
                BattleGroupArray[nSettingCount].SpawnTime_Max = 3.0f;

            if (BattleGroupArray[nSettingCount].Groupdata.NextGroup == 0)
                break;
            else
                TempGroupID = BattleGroupArray[nSettingCount].Groupdata.NextGroup;

            nSettingCount++;
        }




        //그룹 정보를 가지고 폰생성.
        PVE_BossCardIndex = 0;
        for (int idx = 0; idx < BattleGroupArray.Length; idx++)
        {
            BattleGroupInfo pGroupInfo = BattleGroupArray[idx];
            DB_StageMob.Schema pGroupData = pGroupInfo.Groupdata;

            for (int subIdx = 0; subIdx < 5; subIdx++)
            {
                int TempMobIndex = 0;
                switch (subIdx)
                {
                    case 0: TempMobIndex = pGroupData.MobIndex_1; break;
                    case 1: TempMobIndex = pGroupData.MobIndex_2; break;
                    case 2: TempMobIndex = pGroupData.MobIndex_3; break;
                    case 3: TempMobIndex = pGroupData.MobIndex_4; break;
                    case 4: TempMobIndex = pGroupData.MobIndex_5; break;
                }

                if (TempMobIndex == 0)
                    continue;

                DB_PVEMobData.Schema TempMobData = DB_PVEMobData.Query(DB_PVEMobData.Field.MobIndex, TempMobIndex);

                int PawnID = TempMobData.Card_Index;

                //보스정보.
                bool bBoss = false;
                if (pGroupData.BossIndex - 1 == subIdx)
                {
                    PVE_BossCardIndex = PawnID;
                    bBoss = true;
                }


                int TempPawnKey = 200 + nAddEnemyCount;
                bool bLeader = false;
                if (subIdx == pGroupData.LeaderIndex - 1 && pGroupData.LeaderIndex != 0)
                    bLeader = true;

                float StatRevision = pGroupData.StatRevision;
                if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.GroupNumber == 30)
                    StatRevision = 0.1f;

                tempPawnData = MakeBattlePawn(false, bLeader, PawnID, TempMobData.Level_Base,
                    TempMobData.Level_Acc, TempMobData.Level_Weapon, TempMobData.Level_Armor,
                    TempMobData.Level_Skill, TempPawnKey, StatRevision);

                if (tempPawnData == null)
                    continue;

                InitPawnTransform(tempPawnData, false, subIdx, bBoss);

                if (tempPawnData != null)
                    EnemyPawnList.Add(tempPawnData);

                nAddEnemyCount++;

                tempPawnData.transform.localPosition = new Vector3(5000.0f, tempPawnData.transform.localPosition.y, tempPawnData.transform.localPosition.z);

                //PVE.
                tempPawnData.SetMotion(PAWN_ANIMATION_KIND.PHASE_WAIT);
                tempPawnData.PVE_GroupIndex = idx;

                tempPawnData.BossPawn = bBoss;
                if (tempPawnData.BossPawn)
                {
                    Destroy(tempPawnData.HPGaugeObject);
                    tempPawnData.HPGaugeObject = null;
                }

            }
        }
    }


















    //테스트 모드 적 로딩.
    public void TestLoadBattlePawn()
    {
        GameObject TestModuleObj = GameObject.Find("BattleTestModule");
        if (TestModuleObj == null)
            return;

        BattleTestModule TestModuleMng = TestModuleObj.GetComponent<BattleTestModule>();
        if (TestModuleMng == null)
            return;

        //아군.
        BattlePawn tempPawnData = null;

        for (int idx = 0; idx < TestModuleMng.HeroTeam.Length; idx++)
        {
            TestPawnData pTestPawnData = TestModuleMng.HeroTeam[idx];

            if (pTestPawnData.Index == 0)
                pBattleUI.BattleCardList[idx].SetEmptyCard();
            else
            {
                int TempPawnKey = 100 + idx;    //100은 아군.
                bool bLeader = false;
                if (idx == TestModuleMng.HeroLeader)
                    bLeader = true;


                tempPawnData = MakeBattlePawn(true, bLeader, pTestPawnData.Index, pTestPawnData.Level_Pawn, 
                    pTestPawnData.Level_Accessory, pTestPawnData.Level_Weapon, pTestPawnData.Level_Armor, 
                    pTestPawnData.Level_Skill, TempPawnKey);

                if (tempPawnData == null)
                {
                    pBattleUI.BattleCardList[idx].SetEmptyCard();
                    continue;
                }

                //아군이면 UI도 초기화.
                pBattleUI.BattleCardList[idx].InitBattleCard(pBattleUI, tempPawnData);

                InitPawnTransform(tempPawnData, true, idx);

                if (tempPawnData != null)
                    HeroPawnList.Add(tempPawnData);
            }
        }


        //적군.
        tempPawnData = null;
        for (int idx = 0; idx < TestModuleMng.HeroTeam.Length; idx++)
        {
            TestPawnData pTestPawnData = TestModuleMng.EnemyTeam[idx];
            if (pTestPawnData.Index != 0)
            {
                int TempPawnKey = 200 + idx;    //200은 적군.
                bool bLeader = false;
                if (idx == TestModuleMng.EnemyLeader)
                    bLeader = true;

                tempPawnData = MakeBattlePawn(false, bLeader, pTestPawnData.Index, pTestPawnData.Level_Pawn,
                    pTestPawnData.Level_Accessory, pTestPawnData.Level_Weapon, pTestPawnData.Level_Armor,
                    pTestPawnData.Level_Skill, TempPawnKey, TestModuleMng.Stat_Compensate);

                if (tempPawnData == null)
                    continue;

                InitPawnTransform(tempPawnData, false, idx);

                if (tempPawnData != null)
                    EnemyPawnList.Add(tempPawnData);

                tempPawnData.DummyMode = TestModuleMng.DummyEnemyMode;
            }
        }


        //기타 모드 추가.
        //슈퍼모드.
        SuperMode = TestModuleMng.SuperMode;

    }


















    //폰 생성.
    public BattlePawn   GenerateBattlePawn(int PawnKey, long CardCID, bool HeroTeam, bool Leader, int nAddIndex)
    {
        BattlePawn tempPawnData = MakeBattlePawn_CID(HeroTeam, Leader, CardCID, PawnKey);

        if (tempPawnData == null)
            return null;

        InitPawnTransform(tempPawnData, HeroTeam, nAddIndex);

        return tempPawnData;
    }


    //폰 트랜스폼 최초 세팅.
    public void InitPawnTransform(BattlePawn tempPawnData, bool HeroTeam, int AddIndex, bool BossMode = false)
    {
        //첫 좌표 기억.
        tempPawnData.FirstPos_Z = 10.0f;
        tempPawnData.FirstPos_Y = pBattleFieldMng.GroundPos_Up;

        //클래스에 따라 위치추가조정. 사이값은 3.
        switch (tempPawnData.GetClassType())
        {
            case ClassType.ClassType_Keeper:
                tempPawnData.FirstPos_Z = 8.0f;
                tempPawnData.FirstPos_Y += 0.03f;
                break;
            case ClassType.ClassType_Hitter:
                tempPawnData.FirstPos_Z = 2.0f;
                tempPawnData.FirstPos_Y -= 0.03f;
                break;
            default:
                tempPawnData.FirstPos_Z = 5.0f;
                break;
        }

        if (BossMode)  //보스처리.
        {
            tempPawnData.FirstPos_Z = 11.0f;
            tempPawnData.FirstPos_Y += 0.06f;
        }

        tempPawnData.FirstPos_Z -= AddIndex * 0.3f;

        //배치.
        float fCurPos_X = pBattleFieldMng.SpawnRange + (pBattleFieldMng.SpawnRange * AddIndex);
        float fCurPos_Y = pBattleFieldMng.SpawnRange;

        tempPawnData.transform.SetParent(PawnParentTarget);

        if (HeroTeam)
            tempPawnData.transform.position = new Vector3(-pBattleFieldMng.StartPosLength - fCurPos_X, fCurPos_Y, tempPawnData.FirstPos_Z);
        else
            tempPawnData.transform.position = new Vector3(pBattleFieldMng.StartPosLength + fCurPos_X, fCurPos_Y, tempPawnData.FirstPos_Z);

        //폰 사이즈.
        float fSize = tempPawnData.ScaleSize;
        tempPawnData.transform.localScale = new Vector3(tempPawnData.LookDirection * fSize, fSize, fSize);
    }





    //폰 생성.
    //CardIndex 는 캐릭터번호, PawnID 는 전투에서 사용되는 고유키.
    public BattlePawn MakeBattlePawn(bool HeroTeam, bool LeaderPawn, int CardIndex, int CardLevel, int EquipLevel_Acc, int EquipLevel_Weapon, int EquipLevel_Armor, int SkillLevel, int PawnKey, float fStat_Compensate = 1.0f)
    {
        if (CardIndex == -1)
            return null;

        //Prefab 로드.
        GameObject PawnObj = Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
        BattlePawn PawnData = PawnObj.GetComponent<BattlePawn>();


        //폰 초기화 및 스파인 로드.
        PawnData.InitBattlePawn(HeroTeam, LeaderPawn, CardIndex, PawnKey, CardLevel, EquipLevel_Acc, EquipLevel_Weapon, EquipLevel_Armor, SkillLevel, fStat_Compensate);

        return PawnData;
    }



    public BattlePawn MakeBattlePawn_CID(bool HeroTeam, bool LeaderPawn, long CID, int PawnKey)
    {
        if (CID <= 0)
            return null;
        CCardInfo cardInfo = null;
        if(HeroTeam)
            cardInfo = Kernel.entry.character.FindCardInfo(CID);
        else
        {
            if (Kernel.entry.battle.CurBattleKind == BATTLE_KIND.REVENGE_BATTLE)
            {
                BattleLog pEnemyBattleLog;
                BattleLogUtility.TryGetBattleLog(Kernel.entry.battle.RevengeMatchInfoData.m_sDeckInfo, out pEnemyBattleLog);

                for (int idx = 0; idx < pEnemyBattleLog.m_CardInfoList.Count; idx++)
                {
                    if (pEnemyBattleLog.m_CardInfoList[idx].m_Cid == CID)
                    {
                        cardInfo = pEnemyBattleLog.m_CardInfoList[idx];
                        break;
                    }
                }
            }
            else
            {
                for (int idx = 0; idx < Kernel.entry.battle.PVP_CardInfo.Count; idx++)
                {
                    if (Kernel.entry.battle.PVP_CardInfo[idx].m_Cid == CID)
                    {
                        cardInfo = Kernel.entry.battle.PVP_CardInfo[idx];
                        break;
                    }
                }
            }
        }

        if (cardInfo != null)
        {
            //Prefab 로드.
            GameObject PawnObj = Instantiate(Resources.Load("Prefabs/Battle/BattlePawn")) as GameObject;
            BattlePawn PawnData = PawnObj.GetComponent<BattlePawn>();


            //폰 초기화 및 스파인 로드.
            PawnData.InitBattlePawn(HeroTeam, LeaderPawn, cardInfo.m_iCardIndex, PawnKey, cardInfo.m_byLevel, 
                cardInfo.m_byAccessoryLV, cardInfo.m_byWeaponLV, cardInfo.m_byArmorLV, cardInfo.m_bySkill);  //레벨 받아올것.

            return PawnData;
        }
        return null;
    }




























	void Update ()
    {
        if(CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            //카메라 제어.
            float PVE_PreCamPos_X = PVE_FieldCamPos.position.x;
            float PVE_CamPos_X = 0.0f;

            bool FixFollowMode = false;
            switch(eBattleStateKind)
            {
                case BATTLE_STATE_KIND.PRE_LOADING:
                case BATTLE_STATE_KIND.INIT:
                case BATTLE_STATE_KIND.SHOW_BATTLE_INFO:
                    PVE_CamPos_X = 0.0f;
                    break;

                case BATTLE_STATE_KIND.MOVE_START:
                case BATTLE_STATE_KIND.BATTLE_START:
                    PVE_CamPos_X = BattleUtil.FrontPawnPosition(HeroPawnList) + 0.5f;
                    FixFollowMode = true;
                    break;

                case BATTLE_STATE_KIND.PHASE_END:
                    PVE_CamPos_X = BattleUtil.FrontPawnPosition(HeroPawnList) + 0.5f;
                    break;

                case BATTLE_STATE_KIND.END_BATTLE_MODE:
                case BATTLE_STATE_KIND.NETWORK_BATTLE_RESULT:
                case BATTLE_STATE_KIND.SHOW_RESULT:
                case BATTLE_STATE_KIND.BATTLE_EXIT:
                    PVE_CamPos_X = PVE_PreCamPos_X;
                    break;


                default:
                    PVE_CamPos_X = BattleUtil.FrontPawnPosition(HeroPawnList) + 0.5f;
                    break;
            }

            if (PVE_CamPos_X < 0.0f)
                PVE_CamPos_X = 0.0f;

            PVE_FieldCamManager.SetFollowTarget(PVE_CamPos_X, FixFollowMode);
            MainCameraObject.transform.position = PVE_FieldCamPos.position;


            //웨이브 타이머.
            if (eBattleStateKind == BATTLE_STATE_KIND.BATTLE)
            {
                if (PVE_BattleGroupIndex < BattleGroupArray.Length)
                {
                    BattleGroupArray[PVE_BattleGroupIndex].SpawnTime_Cur += Time.deltaTime;
                    if (BattleGroupArray[PVE_BattleGroupIndex].SpawnTime_Cur >= BattleGroupArray[PVE_BattleGroupIndex].SpawnTime_Max)
                    {
                        SetNextGroupActive();
                    }
                }
            }
        }


        //EX스킬 이펙트.
        UpdateExCamEffect();

        if (eBattleStateKind == BATTLE_STATE_KIND.BATTLE)
        {
            if (GamePauseMode)
                return;

            if(CurBattleKind != BATTLE_KIND.TEST_BATTLE && !Kernel.entry.tutorial.TutorialActive)
                CurBattleTime += Time.deltaTime;
            if (CurBattleTime >= MaxBattleTime)
            {
                CurBattleTime = MaxBattleTime;
            }

            //아군 총 히트수와 체력.
            Check_TeamHitCount = 0;
            int TotalHP_Cur = 0;
            int TotalHP_Max = 0;
            for (int idx = 0; idx < HeroPawnList.Count; idx++)
            {
                if (HeroPawnList[idx].IsDeath())
                    continue;
                if (HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                Check_TeamHitCount += HeroPawnList[idx].Check_HitCount;
                TotalHP_Cur += HeroPawnList[idx].CurHP;
                TotalHP_Max += HeroPawnList[idx].MaxHP;
            }
            Check_TeamHP = TotalHP_Cur * 1.0f / TotalHP_Max;

            //적군 총 히트수와 체력.
            Check_EnemyTeamHitCount = 0;
            TotalHP_Cur = 0;
            TotalHP_Max = 0;
            for (int idx = 0; idx < EnemyPawnList.Count; idx++)
            {
                if (EnemyPawnList[idx].IsDeath())
                    continue;
                if (EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                Check_EnemyTeamHitCount += EnemyPawnList[idx].Check_HitCount;
                TotalHP_Cur += EnemyPawnList[idx].CurHP;
                TotalHP_Max += EnemyPawnList[idx].MaxHP;
            }
            Check_EnemyTeamHP = TotalHP_Cur * 1.0f / TotalHP_Max;



            if (SkillUseDelay == true)
            {
                CurSkillUseDelayTime += Time.deltaTime;
                if (CurSkillUseDelayTime >= MaxSkillUseDelayTime)
                {
                    SkillUseDelay = false;
                    CurSkillUseDelayTime = 0.0f;
                }
            }
            else
            {
                //스킬 AI 매니저.
                //이건 자동전투 모드가 생기면 제어.

                if (Kernel.entry.battle.CurBattleKind == BATTLE_KIND.PVE_BATTLE && Kernel.entry.battle.AutoPlayBattle)
                    UpdateSkillUseManager(true);
            }


            if (EnemySkillUseDelay == true)
            {
                CurEnemySkillUseDelayTime += Time.deltaTime;
                if (CurEnemySkillUseDelayTime >= MaxSkillUseDelayTime)
                {
                    EnemySkillUseDelay = false;
                    CurEnemySkillUseDelayTime = 0.0f;
                }
            }
            else
            {
                //스킬 AI 매니저.
                UpdateSkillUseManager(false);
            }





        }

	}




    void FixedUpdate()
    {
        switch (eBattleStateKind)
        {
            case BATTLE_STATE_KIND.PRE_LOADING:
                if (PreLoadingComplete)
                    ChangeBattleState(BATTLE_STATE_KIND.INIT);
                break;

            case BATTLE_STATE_KIND.INIT:
                if (BattleInitComplete)
                {
                    if(CurBattleKind == BATTLE_KIND.PVE_BATTLE)
                        ChangeBattleState(BATTLE_STATE_KIND.MOVE_START);
                    else
                        ChangeBattleState(BATTLE_STATE_KIND.SHOW_BATTLE_INFO);
                }
                break;

            case BATTLE_STATE_KIND.SHOW_BATTLE_INFO:
                if (pBattleStartUI.ShowEventMode == false)
                    ChangeBattleState(BATTLE_STATE_KIND.MOVE_START);
                break;

            case BATTLE_STATE_KIND.MOVE_START:
                if (CheckAllPawn_FirstMoveEnd() == true)
                {
                    if (CurBattleKind == BATTLE_KIND.PVE_BATTLE)
                    {
                        SetNextGroupActive();
                    }
                    else
                    {
                        if (Kernel.entry.tutorial.TutorialActive)
                        {
                            if (Kernel.entry.tutorial.GroupNumber == 10 && Kernel.entry.tutorial.WaitSeq == 100)
                                ChangeBattleState(BATTLE_STATE_KIND.TUTORIAL_PRE_BATTLE);
                            else
                                ChangeBattleState(BATTLE_STATE_KIND.BATTLE_START);

                        }
                        else
                            ChangeBattleState(BATTLE_STATE_KIND.BATTLE_START);
                    }
                }
                break;

            case BATTLE_STATE_KIND.BATTLE_START:
                //버프리스트 갱신.
                UpdateBuffInfoList();
                break;

            case BATTLE_STATE_KIND.BATTLE:
                if(GamePauseMode)
                    return;

                //버프리스트 갱신.
                UpdateBuffInfoList();

                //하드탱킹.
                CheckHardTanker();

                //밀어내기 체크.
                CheckChargingKeeper();


                eBattleResultState = BATTLE_RESULT_STATE.NONE;

                if (CurBattleTime >= MaxBattleTime)
                    eBattleResultState = BATTLE_RESULT_STATE.DRAW;
                else
                {
                    if (CurBattleKind == BATTLE_KIND.PVE_BATTLE)     //PVE일때 전투 결과.
                    {
                        bool HeroAlive = CheckAlivePawn(true);
                        bool EnemyAlive = false;
                        if (PVE_BattleGroupIndex >= BattleGroupArray.Length)            //최종 웨이브일때.
                        {
                            EnemyAlive = CheckAlivePawn(false);

                            if (!HeroAlive && !EnemyAlive)   //비김.
                                eBattleResultState = BATTLE_RESULT_STATE.DRAW;
                            if (HeroAlive && !EnemyAlive)    //이김.
                                eBattleResultState = BATTLE_RESULT_STATE.WIN;
                            if (!HeroAlive && EnemyAlive)    //짐.
                                eBattleResultState = BATTLE_RESULT_STATE.LOSE;
                        }
                        else    //최종 웨이브가 아니니 뒷그룹은 살아있을때.
                        {
                            EnemyAlive = CheckAliveEnemyGroup();
                            if (HeroAlive)
                            {
                                if (EnemyAlive)  //현재그룹이 살아있으면...
                                    return;
                                else            //현재그룹이 다 죽었으면...
                                {
                                    //다음 웨이브 세팅.
                                    SetNextGroupActive();
                                }
                            }
                            else
                            {
                                bool AllEnemyAlive = CheckAlivePawn(false);
                                if (AllEnemyAlive)
                                    eBattleResultState = BATTLE_RESULT_STATE.LOSE;
                                else
                                    eBattleResultState = BATTLE_RESULT_STATE.DRAW;
                            }
                        }
                    }
                    else
                    {
                        bool HeroAlive = CheckAlivePawn(true);
                        bool EnemyAlive = CheckAlivePawn(false);

                        if (!HeroAlive && !EnemyAlive)   //비김.
                            eBattleResultState = BATTLE_RESULT_STATE.DRAW;
                        if (HeroAlive && !EnemyAlive)    //이김.
                            eBattleResultState = BATTLE_RESULT_STATE.WIN;
                        if (!HeroAlive && EnemyAlive)    //짐.
                            eBattleResultState = BATTLE_RESULT_STATE.LOSE;
                    }
                }

                if (eBattleResultState != BATTLE_RESULT_STATE.NONE)
                {
                    ChangeBattleState(BATTLE_STATE_KIND.BATTLE_END);

                    switch (eBattleResultState)
                    {
                        case BATTLE_RESULT_STATE.WIN:
                            SetBattleWinMotion(true);
                            break;

                        case BATTLE_RESULT_STATE.LOSE:
                            SetBattleWinMotion(false);
                            break;
                        
                        case BATTLE_RESULT_STATE.DRAW:
                            SetBattleEndMotion();
                            break;
                    }
                }
                break;

            case BATTLE_STATE_KIND.PHASE_END:
                break;

            case BATTLE_STATE_KIND.BATTLE_END:
                break;

            case BATTLE_STATE_KIND.SHOW_RESULT:
                break;

            case BATTLE_STATE_KIND.BATTLE_EXIT:
                break;

            case BATTLE_STATE_KIND.END_BATTLE_MODE:
                break;
        }
    }



    private void InvokeMethod_PlayBattle()
    {
        ChangeBattleState(BATTLE_STATE_KIND.BATTLE);
    }

    private void InvokeMethod_SendBattleResult()
    {
        ChangeBattleState(BATTLE_STATE_KIND.NETWORK_BATTLE_RESULT);
    }






















    ////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  기능제어.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////

    //전투상태 전환시 처리할 부분은 여기에 넣어준다.
    public void ChangeBattleState(BATTLE_STATE_KIND eStateKind)
    {
        eBattleStateKind = eStateKind;

        //씬 전환시 처리할 부분은 여기에 넣어준다.
        switch (eBattleStateKind)
        {
            case BATTLE_STATE_KIND.PRE_LOADING:         //전투전 세팅을 위한 대기.
                PreloadBattle();
                break;

            case BATTLE_STATE_KIND.INIT:                //전투 초기화.
                InitBattle();
                break;

            case BATTLE_STATE_KIND.SHOW_BATTLE_INFO:
                pBattleStartUI.ShowBattleStartUI(this);
                break;

            case BATTLE_STATE_KIND.MOVE_START:          //전투 시작위치로 이동.
                SetAllPawn_FirstMove();     //모든 폰에게 전투위치로 이동하도록 명령.
                break;

            case BATTLE_STATE_KIND.TUTORIAL_PRE_BATTLE:
                Kernel.entry.tutorial.onSetNextTutorial();
                break;

            case BATTLE_STATE_KIND.BATTLE_START:        //전투 시작.
                ActiveLeaderSkill_Hero();
                break;

            case BATTLE_STATE_KIND.BATTLE:              //전투 중.
                WaitUseSkillCheck_H = true;
                WaitUseSkillCheck_E = true;
                break;

            case BATTLE_STATE_KIND.PHASE_END:
                SetEndPhase();
                Invoke("SetAllPawn_BattleFormation", 1.0f);
                break;

            case BATTLE_STATE_KIND.BATTLE_END:          //전투 종료.
                ResetBuffInfoList();
                Invoke("InvokeMethod_SendBattleResult", 2.0f);
                break;

            case BATTLE_STATE_KIND.NETWORK_BATTLE_RESULT:
                SendBattleResult();
                break;


            case BATTLE_STATE_KIND.SHOW_RESULT:         //결과창 표기.
                break;

            case BATTLE_STATE_KIND.BATTLE_EXIT:         //전투 나가기.
                break;

            case BATTLE_STATE_KIND.END_BATTLE_MODE:
                break;
        }

    }





    //  전체 시작지점으로 이동.
    public void SetAllPawn_FirstMove()
    {
        //PVE.
        if (CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            int nSetCount = 0;
            for (int idx = 0; idx < HeroPawnList.Count; idx++)
            {
                if (HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                HeroPawnList[idx].SetMove_MovePosition(pBattleFieldMng.StartPosLength/2 - (pBattleFieldMng.SpawnRange + (pBattleFieldMng.SpawnRange * nSetCount)));
                nSetCount++;
            }
        }
        else
        {
            for (int idx = 0; idx < HeroPawnList.Count; idx++)
            {
                if (HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                HeroPawnList[idx].SetMove_MoveRange(pBattleFieldMng.FirstMoveLength);
            }

            for (int idx = 0; idx < EnemyPawnList.Count; idx++)
            {
                if (EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                EnemyPawnList[idx].SetMove_MoveRange(pBattleFieldMng.FirstMoveLength);
            }
        }
    }




    public void SetEndPhase()
    {

        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].IsDeath())
                continue;

            HeroPawnList[idx].RemoveBadBuff();
            HeroPawnList[idx].WaitUseSkillMode = false;
        }
    }

    public void SetAllPawn_BattleFormation()
    {
        ChangeBattleState(BATTLE_STATE_KIND.MOVE_START);
    }









    //모든 폰의 시작지점 이동이 끝났는지 체크.
    public bool CheckAllPawn_FirstMoveEnd()
    {
        bool IsMovePawn = false;

        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].IsDeath())
                continue;
            if (HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (HeroPawnList[idx].IsRangeMoveMode())
            {
                IsMovePawn = true;
                break;
            }
        }


        //PVE면 그냥 아군만 체크.
        if (CurBattleKind == BATTLE_KIND.PVE_BATTLE)
            return !IsMovePawn;



        if (IsMovePawn == false)
        {
            for (int idx = 0; idx < EnemyPawnList.Count; idx++)
            {
                if (EnemyPawnList[idx].IsDeath())
                    continue;
                if (EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                if (EnemyPawnList[idx].IsRangeMoveMode())
                {
                    IsMovePawn = true;
                    break;
                }
            }
        }

        return !IsMovePawn;
    }





    //생존한 폰이 있는지 체크.
    public bool CheckAlivePawn(bool HeroTeam)
    {
        List<BattlePawn>    TempList = null;
        if(HeroTeam)
            TempList = HeroPawnList;
        else
            TempList = EnemyPawnList;

        bool bAlive = false;
        for(int idx = 0; idx < TempList.Count; idx++)
        {
            if (TempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (TempList[idx].CurHP <= 0 || TempList[idx].ePawnAniState == PAWN_ANIMATION_KIND.DIE)
                continue;

            bAlive = true;
            break;
        }

        return bAlive;
    }



    //현재 그룹번호 이하로 생존중인 적이 있는지 체크.
    public bool CheckAliveEnemyGroup()
    {
        bool bAlive = false;
        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (EnemyPawnList[idx].CurHP <= 0 || EnemyPawnList[idx].ePawnAniState == PAWN_ANIMATION_KIND.DIE)
                continue;

            if (EnemyPawnList[idx].PVE_GroupIndex < PVE_BattleGroupIndex)
            {
                bAlive = true;
                break;
            }
        }

        return bAlive;
    }






    //리더 찾아오기.
    public BattlePawn FindBattlePawn_Leader(bool HeroTeam, bool CheckActive = true)
    {
        List<BattlePawn>    TempList = null;
        if(HeroTeam)
            TempList = HeroPawnList;
        else
            TempList = EnemyPawnList;

        for(int idx = 0; idx < TempList.Count; idx++)
        {
            if (CheckActive)
            {
                if (TempList[idx].IsDeath())
                    continue;
            }

            if(TempList[idx].LeaderPawn)
                return TempList[idx];
        }

        return null;
    }


    //보스 찾아오기.
    public BattlePawn FindBattlePawn_Boss()
    {
        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].IsDeath())
                continue;

            if (EnemyPawnList[idx].BossPawn)
                return EnemyPawnList[idx];
        }

        return null;
    }


    public BattlePawn GetPawnOnKey(int Key)
    {
        List<BattlePawn> pTempList;
        if (Key / 100 == 1)  //아군.
            pTempList = HeroPawnList;
        else
            pTempList = EnemyPawnList;

        for (int idx = 0; idx < pTempList.Count; idx++)
        {
            if (pTempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (pTempList[idx].GetBattlePawnKey() == Key)
                return pTempList[idx];
        }

        return null;
    }



    //강제 넉백.
    public void ForceKnockback(bool bHeroTeam, Vector3 pPos)
    {
        List<BattlePawn> pTempList = null;
        if (bHeroTeam)
            pTempList = HeroPawnList;
        else
            pTempList = EnemyPawnList;

        for (int idx = 0; idx < pTempList.Count; idx++)
        {
            if (pTempList[idx].IsDeath())
                continue;
            if (pTempList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            float Distance = BattleUtil.GetDistance_X(pPos.x, pTempList[idx].transform.position.x);
            if(Distance <= 1.5f)
                pTempList[idx].SetKnockBack(null, KNOCKBACK_TYPE.KNOCKBACK_SLIDE);
        }
    }









    //이긴팀에 승리모션.
    public void SetBattleWinMotion(bool HeroWin)
    {
        List<BattlePawn> TempList = null;
        if (HeroWin)
            TempList = HeroPawnList;
        else
            TempList = EnemyPawnList;

        for (int idx = 0; idx < TempList.Count; idx++)
        {
            if (TempList[idx].IsDeath())
                continue;
            if (TempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            TempList[idx].SetMotion(PAWN_ANIMATION_KIND.WIN, true);
        }

    }


    //전투 종료.
    public void SetBattleEndMotion()
    {
        List<BattlePawn> TempList = null;
        for (int nLoop = 0; nLoop < 2; nLoop++)
        {
            if (nLoop == 0)
                TempList = HeroPawnList;
            else
                TempList = EnemyPawnList;

            for (int idx = 0; idx < TempList.Count; idx++)
            {
                if (TempList[idx].IsDeath())
                    continue;
                if (TempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                TempList[idx].SetMotion(PAWN_ANIMATION_KIND.IDLE, true);
            }
        }
    }












    //스킬 사용시 전투 일시정지.
    [HideInInspector]
    public  BattlePawn      CurUseSkillPawnData;
    private DB_Skill.Schema CurUseSkillData;
    
    public void SetBattle_Pause_UseSkill(BattlePawn UsePawn, DB_Skill.Schema SkillData, float WaitTime)
    {
        if (SkillData.SkillType == SkillType.Max)
            SetExCamEffect(UsePawn.HeroTeam, UsePawn.transform.position, WaitTime);


        GamePauseMode = true;
        CurUseSkillPawnData = UsePawn;
        CurUseSkillData = SkillData;

        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            var renderer = HeroPawnList[idx].PawnSpine.gameObject.GetComponent<MeshRenderer>();
            if (HeroPawnList[idx] == CurUseSkillPawnData || (CurUseSkillPawnData.HeroTeam && CurUseSkillData.SkillType == SkillType.Leader))
            {
                renderer.sortingOrder = 21;
                continue;
            }

            if (SkillData.SkillType != SkillType.Max)
            {
                HeroPawnList[idx].SetPause(true);
                HeroPawnList[idx].SetAirborneAniPause(true);
            }
        }

        //폰들 일시정지.
        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            var renderer = EnemyPawnList[idx].PawnSpine.gameObject.GetComponent<MeshRenderer>();

            if (EnemyPawnList[idx] == CurUseSkillPawnData || (!CurUseSkillPawnData.HeroTeam && CurUseSkillData.SkillType == SkillType.Leader))
            {
                renderer.sortingOrder = 21;
                continue;
            }

            EnemyPawnList[idx].SetPause(true);
            EnemyPawnList[idx].SetAirborneAniPause(true);
        }

        pEffectPoolMng.SetPause();
        SkillFadeScreen.StartFadeScreen();
    }

    public void SetBattle_Resume_UseSkill()
    {
        GamePauseMode = false;

        //폰들 일시정지 해제.
        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            var renderer = HeroPawnList[idx].PawnSpine.gameObject.GetComponent<MeshRenderer>();

            if (HeroPawnList[idx] == CurUseSkillPawnData || (CurUseSkillPawnData.HeroTeam && CurUseSkillData.SkillType == SkillType.Leader))
            {
                if (HeroPawnList[idx].IsDeath())
                    renderer.sortingOrder = 1;
                else
                {
                    //보스면...
//                    renderer.sortingOrder = 3;
                    if(HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                        renderer.sortingOrder = 6;
                    else
                        renderer.sortingOrder = 5;
                }
            }

            HeroPawnList[idx].SetPause(false);
            HeroPawnList[idx].SetAirborneAniPause(false);
        }


        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].Pawn_Type == PAWN_TYPE.STUFF)
                continue;

            var renderer = EnemyPawnList[idx].PawnSpine.gameObject.GetComponent<MeshRenderer>();

            if (EnemyPawnList[idx] == CurUseSkillPawnData || (!CurUseSkillPawnData.HeroTeam && CurUseSkillData.SkillType == SkillType.Leader))
            {
                if (EnemyPawnList[idx].IsDeath())
                    renderer.sortingOrder = 1;
                else
                {
                    //보스면...
//                    renderer.sortingOrder = 3;
                    if (EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                        renderer.sortingOrder = 6;
                    else
                        renderer.sortingOrder = 5;
                }
            }

            EnemyPawnList[idx].SetPause(false);
            EnemyPawnList[idx].SetAirborneAniPause(false);
        }

        pEffectPoolMng.SetResume();
        SkillFadeScreen.ReleaseFadeScreen();

        SetWaitUseSkillCheck(CurUseSkillPawnData.HeroTeam, true);

        CurUseSkillPawnData = null;
        CurUseSkillData = null;

        //리더일땐 전투상황 변경해준다.
        if (bLeaderSkillMode_Hero)
        {
            bLeaderSkillMode_Hero = false;
            Invoke("ActiveLeaderSkill_Enemy", 1.0f);
            return;
        }

        if (bLeaderSkillMode_Enemy)
        {
            bLeaderSkillMode_Enemy = false;
            Invoke("InvokeMethod_PlayBattle", 1.0f);
        }
    }








    //리더스킬 사용부분.
    public void ActiveLeaderSkill_Hero()
    {
        BattlePawn pPawn = FindBattlePawn_Leader(true);

        if (pPawn != null && !AlreadyUseLeaderSkill_Hero)
        {
            bLeaderSkillMode_Hero = true;
            AlreadyUseLeaderSkill_Hero = true;
            bool CheckUse = false;
            pPawn.SetUseSkill(SkillType.Leader, ref CheckUse);
        }
        else
        {
            bLeaderSkillMode_Hero = false;
            ActiveLeaderSkill_Enemy();
        }
    }

    public void ActiveLeaderSkill_Enemy()
    {
        BattlePawn pPawn = FindBattlePawn_Leader(false);

        if (pPawn != null && !AlreadyUseLeaderSkill_Enemy)
        {
            bLeaderSkillMode_Enemy = true;
            AlreadyUseLeaderSkill_Enemy = true;
            bool CheckUse = false;
            pPawn.SetUseSkill(SkillType.Leader, ref CheckUse);
        }
        else
        {
            bLeaderSkillMode_Enemy = false;
            Invoke("InvokeMethod_PlayBattle", 1.0f);
        }
    }







    //버프정보 리스트.
    public void InitBuffInfoList()
    {
        ResetBuffInfoList();

        //버프 종류만큼 생성.
        for (int idx = 0; idx < AddBuffList.Length; idx++)
        {
            GameObject tempBuffObj_H = Instantiate(BuffInfoElementPrefab);
            tempBuffObj_H.transform.SetParent(pBattleUI.BuffInfoList_Hero);
            tempBuffObj_H.transform.localPosition = Vector3.zero;
            tempBuffObj_H.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            BuffInfoElement TempBuffDataElement_H = tempBuffObj_H.GetComponent<BuffInfoElement>();
            TempBuffDataElement_H.InitBuffInfoElement(AddBuffList[idx]);
            BuffInfoList_Hero.Add(TempBuffDataElement_H);

            GameObject tempBuffObj_E = Instantiate(BuffInfoElementPrefab);
            tempBuffObj_E.transform.SetParent(pBattleUI.BuffInfoList_Enemy);
            tempBuffObj_E.transform.localPosition = Vector3.zero;
            tempBuffObj_E.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            BuffInfoElement TempBuffDataElement_E = tempBuffObj_E.GetComponent<BuffInfoElement>();
            TempBuffDataElement_E.InitBuffInfoElement(AddBuffList[idx]);
            BuffInfoList_Enemy.Add(TempBuffDataElement_E);
        }
    }



    public void UpdateBuffInfoList()
    {
        float BuffTime = 0.0f;

        for(int idx = 0; idx < AddBuffList.Length; idx++)
        {
            //아군 버프 리스트.
            BuffInfoElement pInfo_Hero = FindBuffInfoElement(BuffInfoList_Hero, AddBuffList[idx]);
            if (pInfo_Hero == null)
                continue;

            //시간체크.
            BuffTime = GetBuffValue(HeroPawnList, AddBuffList[idx]);
            if (BuffTime > 0.0f && !pInfo_Hero.ActiveBuffInfo)
            {
                pInfo_Hero.gameObject.SetActive(true);
                pInfo_Hero.ActiveBuffInfoElement();
            }

            //시간갱신.
            pInfo_Hero.UpdateBuffInfo(BuffTime);

            
            
            //적군 버프 리스트.
            BuffInfoElement pInfo_Enemy = FindBuffInfoElement(BuffInfoList_Enemy, AddBuffList[idx]);
            if (pInfo_Enemy == null)
                continue;

            //시간체크.
            BuffTime = GetBuffValue(EnemyPawnList, AddBuffList[idx]);
            if (BuffTime > 0.0f && !pInfo_Enemy.ActiveBuffInfo)
            {
                pInfo_Enemy.gameObject.SetActive(true);
                pInfo_Enemy.ActiveBuffInfoElement();
            }

            //시간갱신.
            pInfo_Enemy.UpdateBuffInfo(BuffTime);
        }
    }


    public void ResetBuffInfoList()
    {
        //초기화.
        for (int idx = 0; idx < BuffInfoList_Hero.Count; idx++)
        {
            Destroy(BuffInfoList_Hero[idx]);
        }
        BuffInfoList_Hero.Clear();

        for (int idx = 0; idx < BuffInfoList_Enemy.Count; idx++)
        {
            Destroy(BuffInfoList_Enemy[idx]);
        }
        BuffInfoList_Enemy.Clear();
    }
    

    //버프정보 리스트에서 해당 버프의 데이터를 받아온다.
    public BuffInfoElement FindBuffInfoElement(List<BuffInfoElement> BuffInfoList, BUFF_KIND BuffKind)
    {
        for (int idx = 0; idx < BuffInfoList.Count; idx++)
        {
            if (BuffInfoList[idx].BuffInfoKind == BuffKind)
                return BuffInfoList[idx];
        }

        return null;
    }



    //몬스터 리스트에서 해당 버프가 가장 긴 녀석 기준으로 시간 받아온다.
    public float GetBuffValue(List<BattlePawn> FindList, BUFF_KIND BuffKind)
    {
        float BuffTimeValue = 0.0f;

        for (int idx = 0; idx < FindList.Count; idx++)
        {
            if (FindList[idx].IsDeath())
                continue;

            for (int sub = 0; sub < FindList[idx].BuffList.Count; sub++)
            {
                BuffElement pElement = FindList[idx].BuffList[sub];
                if (!pElement.BuffActive)
                    continue;

                if (pElement.BuffBaseData.BUFF_KIND == BuffKind)
                {
                    float LeftTime = (pElement.MaxLifeTime - pElement.CurLifeTime); //남은시간.
                    if (BuffTimeValue <= LeftTime)
                        BuffTimeValue = LeftTime;
                }
            }
        }

        return BuffTimeValue;
    }













    private float   AddExEffect_TeamLength;

    //EX 스킬 연출.
    public void SetExCamEffect(bool HeroTeam, Vector3 TaretPos, float WaitTime = 1.2f)
    {
        if (ExCamEffect)    //연속 들어올때 처리.
        {
            MainCamera.transform.position = BaseLookPosition;

            ExCamEffect_CurSize = BaseCamSize;
            MainCamera.orthographicSize = ExCamEffect_CurSize;
            EffectCamera.enabled = true;
            EffectCamera.orthographicSize = ExCamEffect_CurSize;
        }

        ExCamEffect = true;
        ExCamEffect_HeroTeam = HeroTeam;

        if (ExCamEffect_HeroTeam)
            AddExEffect_TeamLength = 1.0f;
        else
            AddExEffect_TeamLength = -1.0f;


        ExCamEffect_State = 0;
        ExCamEffect_CurSize = BaseCamSize;
        TargetLookPosition = TaretPos;
        ExCamEffect_CurTime = 0.0f;
        ExCamEffect_MaxTime = WaitTime - 0.1f;
        ExCamEffect_CurPosX = MainCamera.transform.localPosition.x;
        ExCamEffect_CurPosY = MainCamera.transform.localPosition.y;

        BaseLookPosition = MainCamera.transform.position;

        pBattleUI.HPGaugeParent.gameObject.SetActive(false);

        ExSkillFadeScreen.StartFadeScreen();
    }



    //EX 스킬 연출.
    public void UpdateExCamEffect()
    {
        if (!ExCamEffect)
            return;

        switch (ExCamEffect_State)
        {
            case 0:     //줌인.
                ExCamEffect_CurSize -= Time.deltaTime * ExCamZoomIn_Speed;
                if (ExCamEffect_CurSize <= MaxCamSize)
                {
                    ExCamEffect_CurSize = MaxCamSize;
                    ExCamEffect_State = 1;
                    ExCamEffect_CurTime = 0.0f;
                }
                MainCamera.orthographicSize = ExCamEffect_CurSize;
                EffectCamera.orthographicSize = ExCamEffect_CurSize;
                EffectCamera.enabled = false;

                ExCamEffect_CurPosX = MainCamera.transform.position.x;
                if (BaseLookPosition.x < TargetLookPosition.x)
                {
                    ExCamEffect_CurPosX += Time.deltaTime * ExCamMoveIn_Speed;
                    if (ExCamEffect_CurPosX >= TargetLookPosition.x + AddExEffect_TeamLength)
                        ExCamEffect_CurPosX = TargetLookPosition.x + AddExEffect_TeamLength;
                }
                else
                {
                    ExCamEffect_CurPosX -= Time.deltaTime * ExCamMoveIn_Speed;
                    if (ExCamEffect_CurPosX <= TargetLookPosition.x + AddExEffect_TeamLength)
                        ExCamEffect_CurPosX = TargetLookPosition.x + AddExEffect_TeamLength;
                }

                ExCamEffect_CurPosY = MainCamera.transform.position.y;
                ExCamEffect_CurPosY += Time.deltaTime * ExCamMoveIn_Speed / 2;
                if (ExCamEffect_CurPosY >= BaseLookPosition.y + BaseLookOffset.y)
                    ExCamEffect_CurPosY = BaseLookPosition.y + BaseLookOffset.y;

                MainCamera.transform.position = new Vector3(ExCamEffect_CurPosX, ExCamEffect_CurPosY, BaseLookOffset.z);

                ExSkillFadeScreen.transform.position = MainCamera.transform.position;
                break;

            case 1:     //대기.
                ExCamEffect_CurTime += Time.deltaTime;
                if (ExCamEffect_CurTime >= ExCamEffect_MaxTime)
                {
                    ExCamEffect_CurTime = 0.0f;
                    ExCamEffect_State = 2;
                    ExSkillFadeScreen.ReleaseFadeScreen();
                }
                break;

            case 2:     //줌아웃.
                ExCamEffect_CurSize += Time.deltaTime * ExCamZoomOut_Speed;
                if (ExCamEffect_CurSize >= BaseCamSize)
                {
                    ExCamEffect_CurSize = BaseCamSize;
                    ExCamEffect = false;
                }
                MainCamera.orthographicSize = ExCamEffect_CurSize;
                EffectCamera.enabled = true;
                EffectCamera.orthographicSize = ExCamEffect_CurSize;


                ExCamEffect_CurPosX = MainCamera.transform.position.x;
                if (ExCamEffect_CurPosX < BaseLookPosition.x)
                {
                    ExCamEffect_CurPosX += Time.deltaTime * ExCamMoveOut_Speed;
                    if (ExCamEffect_CurPosX >= BaseLookPosition.x)
                        ExCamEffect_CurPosX = BaseLookPosition.x;
                }
                else
                {
                    ExCamEffect_CurPosX -= Time.deltaTime * ExCamMoveOut_Speed;
                    if (ExCamEffect_CurPosX <= BaseLookPosition.x)
                        ExCamEffect_CurPosX = BaseLookPosition.x;
                }

                ExCamEffect_CurPosY = MainCamera.transform.position.y;
                ExCamEffect_CurPosY -= Time.deltaTime * ExCamMoveOut_Speed / 2;
                if (ExCamEffect_CurPosY <= BaseLookPosition.y)
                    ExCamEffect_CurPosY = BaseLookPosition.y;
                
                MainCamera.transform.position = new Vector3(ExCamEffect_CurPosX, ExCamEffect_CurPosY, BaseLookOffset.z);

                pBattleUI.HPGaugeParent.gameObject.SetActive(true);

                ExSkillFadeScreen.transform.position = MainCamera.transform.position;
                break;
        }
    }









    //팀 스킬 이펙트.
    public void ShowTeamSkillEffect(bool bHeroTeam, bool ShowTeamCenter, int Effect_ID)
    {
        Vector3 TargetPos = Vector3.zero;

        if (ShowTeamCenter)
        {
            if (bHeroTeam)
                TargetPos = new Vector3(MainCamera.transform.position.x - 3.0f, 0.03f, 0.0f);
            else
                TargetPos = new Vector3(MainCamera.transform.position.x + 3.0f, 0.03f, 0.0f);
        }
        else
        {
            if (bHeroTeam)
                TargetPos = GameObject.Find("RandomActive_Hero").transform.position;
            else
                TargetPos = GameObject.Find("RandomActive_Enemy").transform.position;
        }

        pEffectPoolMng.SetBattleEffect(TargetPos, Effect_ID, 1.0f, 2.1f);
    }




    //하드탱킹 확인.
    public void CheckHardTanker()
    {
        bool ActiveHardTanking = false;
        float LeftBuffTime = 0.0f;
        BattlePawn TempPawn = null;

        for(int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].IsDeath())
                continue;

            if(HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (HeroPawnList[idx].HardTankingMode && HeroPawnList[idx].HardTinkingLeftTime > 0.0f)
            {
                ActiveHardTanking = true;
                HardTankingMode_Hero = true;

                if (LeftBuffTime == 0.0f || LeftBuffTime < HeroPawnList[idx].HardTinkingLeftTime)
                {
                    LeftBuffTime = HeroPawnList[idx].HardTinkingLeftTime;
                    TempPawn = HeroPawnList[idx];
                }
            }
        }

        if(ActiveHardTanking)
        {
            HardTankingPawn_Hero = TempPawn;
        }
        else
        {
            HardTankingMode_Hero = false;
            HardTankingPawn_Hero = null;
        }

        //적군.
        ActiveHardTanking = false;
        LeftBuffTime = 0.0f;
        TempPawn = null;

        for(int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if(EnemyPawnList[idx].IsDeath())
                continue;

            if(EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (EnemyPawnList[idx].HardTankingMode && EnemyPawnList[idx].HardTinkingLeftTime > 0.0f)
            {
                ActiveHardTanking = true;
                HardTankingMode_Enemy = true;

                if (LeftBuffTime == 0.0f || LeftBuffTime < EnemyPawnList[idx].HardTinkingLeftTime)
                {
                    LeftBuffTime = EnemyPawnList[idx].HardTinkingLeftTime;
                    TempPawn = EnemyPawnList[idx];
                }
            }
        }


        if(ActiveHardTanking)
        {
            HardTankingPawn_Enemy = TempPawn;
        }
        else
        {
            HardTankingMode_Enemy = false;
            HardTankingPawn_Enemy = null;
        }
    }





    //하드탱킹 확인.
    public void CheckChargingKeeper()
    {
        ChargingKeeper_HeroTeam = false;
        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].IsDeath())
                continue;

            if (HeroPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (HeroPawnList[idx].GetClassType() == ClassType.ClassType_Keeper)
            {
                if (HeroPawnList[idx].AI_Module.TankerPushMode || HeroPawnList[idx].AI_Module.TankerPushEqual)
                    ChargingKeeper_HeroTeam = true;
            }
        }

        //적군.
        ChargingKeeper_EnemyTeam = false;
        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].IsDeath())
                continue;

            if (EnemyPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                continue;

            if (EnemyPawnList[idx].GetClassType() == ClassType.ClassType_Keeper)
            {
                if (EnemyPawnList[idx].AI_Module.TankerPushMode || EnemyPawnList[idx].AI_Module.TankerPushEqual)
                    ChargingKeeper_EnemyTeam = true;
            }
        }
    }




    //PVE용.
    public void SetNextGroupActive()
    {
        if (PVE_BattleGroupIndex >= BattleGroupArray.Length)
            return;

        BattleGroupInfo GroupInfo = BattleGroupArray[PVE_BattleGroupIndex];

        float BasePos_X = BattleUtil.FrontPawnPosition(HeroPawnList);

        int nActiveCount = 0;
        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].PVE_GroupIndex == PVE_BattleGroupIndex)
            {
                EnemyPawnList[idx].SetMotion(PAWN_ANIMATION_KIND.IDLE);
                float Pos_x = BasePos_X + pBattleFieldMng.StartPosLength + (pBattleFieldMng.SpawnRange + (pBattleFieldMng.SpawnRange * nActiveCount));
                EnemyPawnList[idx].ForceMove_X(Pos_x);
                nActiveCount++;
            }
        }

        PVE_BattleGroupIndex++;
        if(PVE_BattleGroupIndex == 1)   //처음이면.
            ChangeBattleState(BATTLE_STATE_KIND.BATTLE_START);

        if (PVE_BattleGroupIndex <= 1)  //첫 웨이브는 표시없음.
            return;

        if (PVE_BattleGroupIndex >= BattleGroupArray.Length)
        {
            if (GroupInfo.Groupdata.BossIndex == 0)
            {
                pBattleUI.WaveAlert_Normal.SetActive(true);
            }
            else
            {
                pBattleUI.WaveAlert_Boss.SetActive(true);
                PVE_BossScreenEffect.SetActive(true);
            }
        }
        else
            pBattleUI.WaveAlert_Normal.SetActive(true);

    }














    public void SetWaitUseSkillCheck(bool HeroTeam, bool Value)
    {
        if (HeroTeam)
        {
            if (Kernel.entry.battle.CurBattleKind == BATTLE_KIND.PVE_BATTLE)
                WaitUseSkillCheck_H = Value;
            else
                WaitUseSkillCheck_H = false;
        }
        else
            WaitUseSkillCheck_E = Value;
    }

    public bool IsWaitUseSkillCheck(bool HeroTeam)
    {
        if (HeroTeam)
            return WaitUseSkillCheck_H;
        else
            return WaitUseSkillCheck_E;
    }


    //스킬사용 제어 매니저.
    public void UpdateSkillUseManager(bool HeroTeam)
    {
        if(!IsWaitUseSkillCheck(HeroTeam))
            return;



        bool SkillActivePawn = false;

        List<BattlePawn> TempPawnList = null;
        int CurComboCount;
        if (HeroTeam)
        {
            TempPawnList = HeroPawnList;
            CurComboCount = CurComboCount_H;
        }
        else
        {
            TempPawnList = EnemyPawnList;
            CurComboCount = CurComboCount_E;
        }


        //스킬제어.
        SortingBaseList.Clear();
        SortingList_Healer.Clear();
        SortingList_MaxCoolTime.Clear();
        SortingList_HighSkillValue.Clear();
        SortingList_HighComboCost.Clear();
        SortingList_SameList.Clear();


        //발동조건.
        //1. MAX 가득찼을때.
        if (CurComboCount >= MaxComboCount)
        {
            //쿨타임이 5초 이하인 스킬중 체크.
            for (int idx = 0; idx < TempPawnList.Count; idx++)
            {
                if (TempPawnList[idx].IsDeath() || TempPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                if (TempPawnList[idx].SkillCoolTime_Max - TempPawnList[idx].SkillCoolTime_Cur <= 5.0f)  //5초이하로 남은애들만.
                    SortingBaseList.Add(TempPawnList[idx]);
                if (TempPawnList[idx].GetClassType() == ClassType.ClassType_Healer)
                    SortingList_Healer.Add(TempPawnList[idx]);
            }

            if (SortingBaseList.Count == 0)
                return;

            // - 치유사 우선 -> 맥스쿨타임 가장 긴 스킬 -> 스킬Value 높은 스킬
            // - 치유사 우선.
            if (SortingList_Healer.Count > 0)
            {
                SkillActivePawn = false;
                for(int idx = 0; idx < SortingList_Healer.Count; idx++)
                {
                    if(SortingList_Healer[idx].SkillCoolTime_Cur >= SortingList_Healer[idx].SkillCoolTime_Max)  //사용가능이 있으면.
                        SkillActivePawn = true;
                }

                if(SkillActivePawn)
                {
                    while(true)
                    {
                        int Slot = Random.Range(0, SortingList_Healer.Count);
                        if(SortingList_Healer[Slot].SkillCoolTime_Cur >= SortingList_Healer[Slot].SkillCoolTime_Max)  //사용가능이 있으면.
                        {
                            SortingList_Healer[Slot].SkillUseTurn = true;
                            SetWaitUseSkillCheck(HeroTeam, false);
                            return;
                        }
                    }
                }
            }

            //맥스쿨타임 가장 긴 스킬.
            SortingList_MaxCoolTime = (from pawn in SortingBaseList
                                       orderby pawn.SkillCoolTime_Max descending
                                       select pawn).ToList();
            for (int idx = 0; idx < SortingList_MaxCoolTime.Count; idx++)
            {
                if (SortingList_MaxCoolTime[idx].SkillCoolTime_Cur >= SortingList_MaxCoolTime[idx].SkillCoolTime_Max)  //사용가능이 있으면.
                {
                    SortingList_SameList = (from pawn in SortingBaseList
                                            where (pawn.SkillCoolTime_Cur >= pawn.SkillCoolTime_Max) && (SortingList_MaxCoolTime[idx].SkillCoolTime_Max == pawn.SkillCoolTime_Max)
                                            select pawn).ToList();

                    if (SortingList_SameList.Count > 1) //1개보다 작을 수 없음.
                    {
                        //스킬Value 높은 스킬.
                        SortingList_HighSkillValue = (from pawn in SortingList_SameList
                                                      orderby pawn.CurSkillValue_Max descending
                                                      select pawn).ToList();
                        if (SortingList_HighSkillValue.Count > 1) //1개보다 작을 수 없음.
                        {
                            int Slot = Random.Range(0, SortingList_HighSkillValue.Count);
                            SortingList_HighSkillValue[Slot].SkillUseTurn = true;
                            SetWaitUseSkillCheck(HeroTeam, false);
                            return;
                        }
                        else
                        {
                            SortingList_HighSkillValue[0].SkillUseTurn = true;
                            SetWaitUseSkillCheck(HeroTeam, false);
                            return;
                        }
                    }
                    else
                    {
                        SortingList_MaxCoolTime[idx].SkillUseTurn = true;
                        SetWaitUseSkillCheck(HeroTeam, false);
                        return;
                    }
                }
            }

            //종료.
        }
        else
        {
            //2. MAX 안찼을때.
            //쿨타임이 5초 이하인 스킬중 체크.
            for (int idx = 0; idx < TempPawnList.Count; idx++)
            {
                if (TempPawnList[idx].IsDeath() || TempPawnList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                if (TempPawnList[idx].SkillManager.ActiveCheckOK)  //스킬 조건 활성화 된 아이들만.
                    SortingBaseList.Add(TempPawnList[idx]);
                if (TempPawnList[idx].GetClassType() == ClassType.ClassType_Healer)
                    SortingList_Healer.Add(TempPawnList[idx]);
            }

            if (SortingBaseList.Count == 0)
                return;

            // - 치유사 우선 -> 맥스쿨타임 가장 긴 스킬 -> Combo Cost 높은 스킬 -> 직업군(수호->타격->공격->도사->수호) -> 랜덤.
            // - 치유사 우선.
            if (SortingList_Healer.Count > 0)
            {
                SkillActivePawn = false;
                for (int idx = 0; idx < SortingList_Healer.Count; idx++)
                {
                    if (SortingList_Healer[idx].SkillCoolTime_Cur >= SortingList_Healer[idx].SkillCoolTime_Max)  //사용가능이 있으면.
                        SkillActivePawn = true;
                }

                if (SkillActivePawn)
                {
                    while (true)
                    {
                        int Slot = Random.Range(0, SortingList_Healer.Count);
                        if (SortingList_Healer[Slot].SkillCoolTime_Cur >= SortingList_Healer[Slot].SkillCoolTime_Max)  //사용가능이 있으면.
                        {
                            SortingList_Healer[Slot].SkillUseTurn = true;
                            SetWaitUseSkillCheck(HeroTeam, false);
                            return;
                        }
                    }
                }
            }

            //맥스쿨타임 가장 긴 스킬.
            SortingList_MaxCoolTime = (from pawn in SortingBaseList
                                       orderby pawn.SkillCoolTime_Max descending
                                       select pawn).ToList();
            for (int idx = 0; idx < SortingList_MaxCoolTime.Count; idx++)
            {
                if (SortingList_MaxCoolTime[idx].SkillCoolTime_Cur >= SortingList_MaxCoolTime[idx].SkillCoolTime_Max)  //사용가능이 있으면.
                {
                    SortingList_SameList = (from pawn in SortingBaseList
                                            where (pawn.SkillCoolTime_Cur >= pawn.SkillCoolTime_Max) && (SortingList_MaxCoolTime[idx].SkillCoolTime_Max == pawn.SkillCoolTime_Max)
                                            select pawn).ToList();

                    if (SortingList_SameList.Count > 1) //1개보다 작을 수 없음.
                    {
                        //Combo Cost 높은 스킬.
                        SortingList_HighComboCost = (from pawn in SortingList_SameList
                                                      orderby pawn.CurSkillValue_Max descending
                                                      select pawn).ToList();
                        if (SortingList_HighComboCost.Count > 1) //1개보다 작을 수 없음.
                        {
                            //수호 -> 타격 -> 공격 -> 도사.
                            ClassType tempClass = ClassType.ClassType_Keeper;
                            for (int subClass = 0; subClass < 4;subClass++)
                            {
                                switch (subClass)
                                {
                                    case 0: tempClass = ClassType.ClassType_Keeper; break;
                                    case 1: tempClass = ClassType.ClassType_Hitter; break;
                                    case 2: tempClass = ClassType.ClassType_Ranger; break;
                                    case 3: tempClass = ClassType.ClassType_Wizard; break;
                                }
                                SortingList_JobList.Clear();

                                SortingList_JobList = (from pawn in SortingList_HighComboCost
                                                       where pawn.GetClassType() == tempClass
                                                       select pawn).ToList();

                                if (SortingList_JobList.Count > 1)
                                {
                                    int Slot = Random.Range(0, SortingList_JobList.Count);
                                    SortingList_JobList[Slot].SkillUseTurn = true;
                                    SetWaitUseSkillCheck(HeroTeam, false);
                                    return;
                                }
                                else if(SortingList_JobList.Count == 1)
                                {
                                    SortingList_JobList[0].SkillUseTurn = true;
                                    SetWaitUseSkillCheck(HeroTeam, false);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            SortingList_HighComboCost[0].SkillUseTurn = true;
                            SetWaitUseSkillCheck(HeroTeam, false);
                            return;
                        }
                    }
                    else
                    {
                        SortingList_MaxCoolTime[idx].SkillUseTurn = true;
                        SetWaitUseSkillCheck(HeroTeam, false);
                        return;
                    }
                }
            }
            //종료.
        }
    }


















    //통신부분.
    public void SendBattleResult()
    {
        switch (CurBattleKind)
        {
            case BATTLE_KIND.PVP_BATTLE:
                ePvpResult eResult = ePvpResult.Draw;
                int GetStarCount = 0;
                switch(eBattleResultState)
                {
                    case BATTLE_RESULT_STATE.WIN:
                        eResult = ePvpResult.Win;
                        GetStarCount = 5;
                        break;

                    case BATTLE_RESULT_STATE.LOSE:
                        eResult = ePvpResult.Lose;
                        GetStarCount = 1;
                        break;

                    case BATTLE_RESULT_STATE.DRAW:
                        eResult = ePvpResult.Draw;
                        GetStarCount = 2;
                        break;
                }


                //전투 덱.
                string strBattleLog = string.Empty;
                if(eResult == ePvpResult.Win)
                    BattleLogUtility.TryParse(MyBattleLog, out strBattleLog);

                Kernel.entry.battle.onBattleResultDelegate += RecvBattleResult;
                Kernel.entry.battle.REQ_PACKET_CG_GAME_PVP_RESULT_SYN(eResult, (int)CurBattleTime, GetStarCount, strBattleLog);
                break;




            case BATTLE_KIND.PVE_BATTLE:
                bool bWin = false;
                switch (eBattleResultState)
                {
                    case BATTLE_RESULT_STATE.WIN:
                        bWin = true;
                        break;
                }

#region 업적
                if (bWin)
                {
                    byte lastKillCount = 0;
                    if (EnemyPawnList != null && EnemyPawnList.Count > 0)
                    {
                        for (int i = 0; i < EnemyPawnList.Count; i++)
                        {
                            BattlePawn battlePawn = EnemyPawnList[i];
                            if (battlePawn != null)
                            {
                                if (battlePawn.IsDeath())
                                {
                                    lastKillCount++;
                                }
                            }
                            else Debug.LogError(string.Format("EnemyPawnList[{0}] is null.", i));
                        }
                    }
                    else Debug.LogError("EnemyPawnList is null or empty.");

                    Kernel.entry.adventure.lastKillCount = lastKillCount;
                }
#endregion

                //튜토리얼.
                if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 302)  //첫번째 지역 첫번째 버튼.
                {
                    Kernel.entry.tutorial.onSetNextTutorial();
                }
                else
                {
                    Kernel.entry.adventure.onPveResultDelegate += RecvBattleResult_PVE;
                    Kernel.entry.adventure.REQ_PACKET_CG_GAME_PVE_RESULT_SYN(bWin);
                }
                break;


            case BATTLE_KIND.REVENGE_BATTLE:
                ePvpResult eRevengeResult = ePvpResult.Win;
                switch (eBattleResultState)
                {
                    case BATTLE_RESULT_STATE.DRAW:
                        eRevengeResult = ePvpResult.Draw;
                        break;
                    case BATTLE_RESULT_STATE.LOSE:
                        eRevengeResult = ePvpResult.Lose;
                        break;
                }


                Kernel.entry.revengeBattle.onRevengeMatchResult += RecvRevengeResult;
                Kernel.entry.revengeBattle.REQ_PACKET_CG_GAME_REVENGE_MATCH_RESULT_SYN(eRevengeResult);
                break;
        }
    }


    //결과창 세팅.
    public void RecvBattleResult(PACKET_CG_GAME_PVP_RESULT_ACK packet)
    {
        Kernel.entry.battle.onBattleResultDelegate -= RecvBattleResult;

        //상자리스트.
        Kernel.entry.chest.UpdateRewardBoxList(packet.m_RewardBoxList);

        ChangeBattleState(BATTLE_STATE_KIND.SHOW_RESULT);
        UIBattleResult pResultMng = (UIBattleResult)Kernel.uiManager.Open(UI.BattleResult);
        pResultMng.InitBattleResult(this, eBattleResultState, 
            packet.m_iFluctuationRankingPoint, packet.m_iFluctuationStarPoint, packet.m_iReceiveBoxIndex,
            packet.m_iWinningStreak, packet.m_iWinningStreakReward,
            packet.m_byWinPoint, packet.m_WinPointReward);

        Kernel.soundManager.PlayUISound(eBattleResultState == BATTLE_RESULT_STATE.WIN ? SOUND.SND_UI_PVP_PVE_RESULT_CLEAR : SOUND.SND_UI_PVP_PVE_RESULT_FAIL);
    }






    //결과창 세팅_PVE.
    public void RecvBattleResult_PVE(PACKET_CG_GAME_PVE_RESULT_ACK packet)
    {
        Kernel.entry.adventure.onPveResultDelegate -= RecvBattleResult_PVE;

        UIAdventureResult pResultMng = (UIAdventureResult)Kernel.uiManager.Open(UI.AdventureResult);

        if (eBattleResultState == BATTLE_RESULT_STATE.WIN)
            pResultMng.ShowResult_Win(packet);
        else
            pResultMng.ShowResult_Lose();

        Kernel.soundManager.PlayUISound(eBattleResultState == BATTLE_RESULT_STATE.WIN ? SOUND.SND_UI_PVP_PVE_RESULT_CLEAR : SOUND.SND_UI_PVP_PVE_RESULT_FAIL);
    }





    //결과창 세팅_Revenge.
    public void RecvRevengeResult(CReceivedGoods RecvGoodsInfo)
    {
        Kernel.entry.revengeBattle.onRevengeMatchResult -= RecvRevengeResult;

        UIRevengeResult pResultMng = (UIRevengeResult)Kernel.uiManager.Open(UI.RevengeResult);

        if(RecvGoodsInfo == null)   //받을 데이터가 없으면...
            pResultMng.InitRevengeResult(this, eBattleResultState, Kernel.entry.account.revengePoint, -1);
        else
            pResultMng.InitRevengeResult(this, eBattleResultState, RecvGoodsInfo.m_iTotalAmount, RecvGoodsInfo.m_iReceivedAmount);
    }




    void OnNetworkException(Result_Define.eResult result, string error, ePACKET_CATEGORY category, byte index)
    {
        if (category != ePACKET_CATEGORY.CG_GAME)
        {
            return;
        }

        switch((eCG_GAME)index)
        {
            case eCG_GAME.PVE_RESULT_ACK:
                Kernel.entry.adventure.onPveResultDelegate -= RecvBattleResult_PVE;
                break;
        }
    }





    public void OnButton_BattleExit()
    {
        if (Kernel.entry.tutorial.TutorialActive)
            return;

        UIAlerter.Alert(Languages.ToString(TEXT_UI.BATTLE_GIVEUP_DESC),
        UIAlerter.Composition.Confirm_Cancel,
        BattleForceExit,
        Languages.ToString(TEXT_UI.NOTICE_WARNING));

        BattlePause();
    }



    public void BattlePause()
    {
        ApplicationPauseMode = true;
        Time.timeScale = 0.0f;
    }


    public void BattleResume()
    {
        Time.timeScale = 1.0f;
        ApplicationPauseMode = false;
    }


    public void BattleForceExit(UIAlerter.Response response, params object[] args)
    {
        BattleResume();

        if (response != UIAlerter.Response.Confirm)
            return;

        switch (CurBattleKind)
        {
            case BATTLE_KIND.PVP_BATTLE:
                Kernel.entry.battle.onBattleResultDelegate -= RecvBattleResult;
                Kernel.entry.battle.onBattleResultDelegate += RecvBattleResult_ForceQuit;
                Kernel.entry.battle.REQ_PACKET_CG_GAME_PVP_RESULT_SYN(ePvpResult.Lose, (int)CurBattleTime, 1, string.Empty);
                break;

            case BATTLE_KIND.REVENGE_BATTLE:
                Kernel.entry.revengeBattle.onRevengeMatchResult -= RecvRevengeResult;
                Kernel.entry.revengeBattle.onRevengeMatchResult += RecvRevengeResult_ForceQuit;
                Kernel.entry.revengeBattle.REQ_PACKET_CG_GAME_REVENGE_MATCH_RESULT_SYN(ePvpResult.Lose);
                break;

            case BATTLE_KIND.PVE_BATTLE:
                Kernel.entry.adventure.onPveResultDelegate -= RecvBattleResult_PVE;
                Kernel.entry.adventure.onPveResultDelegate += RecvBattleResult_PVE_ForceQuit;
                Kernel.entry.adventure.REQ_PACKET_CG_GAME_PVE_RESULT_SYN(false);
                break;
        }
    }





    //PVP 강제종료 패배처리.
    public void RecvBattleResult_ForceQuit(PACKET_CG_GAME_PVP_RESULT_ACK packet)
    {
        Kernel.entry.battle.onBattleResultDelegate -= RecvBattleResult_ForceQuit;
        
        //강제 종료 패배처리 하면 연승이 깨짐
        Kernel.entry.account.winningStreak = packet.m_iWinningStreak;
        Kernel.entry.account.winPoint = packet.m_byWinPoint;

        Kernel.sceneManager.LoadScene(Scene.Lobby);
    }


    //복수전 강제종료 패배처리.
    public void RecvRevengeResult_ForceQuit(CReceivedGoods RecvGoodsInfo)
    {
        Kernel.entry.revengeBattle.onRevengeMatchResult -= RecvRevengeResult_ForceQuit;
        Kernel.uiManager.Open(UI.HUD);
        Kernel.sceneManager.LoadScene(Scene.RevengeBattle);
    }


    //PVE 강제종료 패배처리.
    public void RecvBattleResult_PVE_ForceQuit(PACKET_CG_GAME_PVE_RESULT_ACK packet)
    {
        Kernel.entry.adventure.onPveResultDelegate -= RecvBattleResult_PVE_ForceQuit;
        Kernel.entry.adventure.PreSelectStageIndex = Kernel.entry.adventure.SelectStageIndex;
        Kernel.sceneManager.LoadScene(Scene.Adventure);
    }















    //어플리케이션 비활성화일때.
    void OnApplicationFocus(bool hasFocus)
    {
#if UNITY_EDITOR
        return;
#else
        if (hasFocus == false)
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.PAUSE_DESC),
                UIAlerter.Composition.Confirm,
                OnResponseCallback,
                Languages.ToString(TEXT_UI.NOTICE_WARNING));

            BattlePause();
        }
#endif
    }

    private void OnResponseCallback(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
            return;

        BattleResume();
    }
















    //튜토리얼 제어.
    public void TutorialResume()
    {
        switch (Kernel.entry.tutorial.GroupNumber)
        {
            case 10:
                switch (Kernel.entry.tutorial.CurIndex)
                {
                    case 8:
                        ChangeBattleState(BATTLE_STATE_KIND.BATTLE_START);
                        break;

                    case 11:
                    case 13:
                        BattleResume();
                        break;

                    case 15:
                        SuperMode = true;
                        BattleResume();
                        break;
                    
                    case 17:
                        SuperMode = false;
                        OneKillMode_Hero = true;
                        BattleResume();
                        break;
                }
                break;

            case 30:
                switch (Kernel.entry.tutorial.CurIndex)
                {
                    case 12:
                        Kernel.entry.adventure.onPveResultDelegate += RecvBattleResult_PVE;
                        Kernel.entry.adventure.REQ_PACKET_CG_GAME_PVE_RESULT_SYN(true);
                        break;
                }
                break;
        }
    }









    public void SetUseSkill_DummyMode(BattlePawn SkillUsePawn)
    {
        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx] == SkillUsePawn)
                continue;
            if (HeroPawnList[idx].IsDeath())
                continue;

            HeroPawnList[idx].DummyMode = true;
        }


        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx] == SkillUsePawn)
                continue;

            if (EnemyPawnList[idx].IsDeath())
                continue;

            EnemyPawnList[idx].DummyMode = true;
        }
    }


    public void ReleaseUseSkill_DummyMode()
    {
        for (int idx = 0; idx < HeroPawnList.Count; idx++)
        {
            if (HeroPawnList[idx].IsDeath())
                continue;

            HeroPawnList[idx].DummyMode = false;
        }


        for (int idx = 0; idx < EnemyPawnList.Count; idx++)
        {
            if (EnemyPawnList[idx].IsDeath())
                continue;

            EnemyPawnList[idx].DummyMode = false;
        }
    }



}
