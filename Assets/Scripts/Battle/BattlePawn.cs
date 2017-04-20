using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum PAWN_TYPE
{
    PAWN = 0,
    STUFF,
    SUMMONER
}


public enum PAWN_ANIMATION_KIND
{
    IDLE = 0,
    ATTACK_WAIT,
    ATTACK_PUSH,
    MOVE,
    MOVE_CHARGING,
    MOVE_BACK,
    KNOCKBACK,
    HIT,
    ATTACK_KEEPER,
    ATTACK_1,
    WIN,
    DIE,
    SKILL_0,
    SKILL_1,
    SKILL_2,
    SKILL_4,
    SKILL_5,
    SKILL_MAX,
    AIRBORNE,
    FREEZE,
    STUN,
    PHASE_WAIT,
}

public enum KNOCKBACK_TYPE
{
    KNOCKBACK_SLIDE = 0,
    KNOCKBACK_JUMP,
    KNOCKBACK_THROW,
    KNOCKBACK_SKILL,
    KNOCKBACK_DIE
}


public struct PAWN_ANIMATION_NAME
{
    public const string P_ANI_NAME_IDLE = "wait";
    public const string P_ANI_NAME_MOVE = "run";
    public const string P_ANI_NAME_KNOCKBACK = "knock_back";
    public const string P_ANI_NAME_HIT = "hit";
    public const string P_ANI_NAME_BACK = "back";
    public const string P_ANI_NAME_ATT = "attack";
    public const string P_ANI_NAME_ATT_1 = "attack1";
    public const string P_ANI_NAME_WIN = "win";
    public const string P_ANI_NAME_DIE = "die";
    public const string P_ANI_NAME_SKILL_0 = "skill1";
    public const string P_ANI_NAME_SKILL_1 = "skill2";
    public const string P_ANI_NAME_SKILL_2 = "skill3";
    public const string P_ANI_NAME_SKILL_4 = "skill4";
    public const string P_ANI_NAME_SKILL_5 = "skill5";
    public const string P_ANI_NAME_AIRBORNE = "airborne";
    public const string P_ANI_NAME_CHARGING = "push";
    public const string P_ANI_NAME_SKILL_MAX = "maxskill";
}



public class BattlePawn : BattleBaseData 
{
    //허수아비모드.
    public  bool                    DummyMode;

    [HideInInspector]
    public  bool                    BossPawn;

    [HideInInspector]
    public  BattleManager           BattleMng;

    [HideInInspector]
    public  BattleAI                AI_Module; //AI모듈.

    //특수액션.
    [HideInInspector]
    public  SpecialActionManager    SpecialActionMng;
    [HideInInspector]
    public  bool                    SpecialActionMode;
    
    [HideInInspector]
    public  List<BattleBullet>      ThrowBattleBullet = new List<BattleBullet>();
    [HideInInspector]
    public  BattleBullet            HealMagicBullet;

    [HideInInspector]
    public  BattleSkillManager      SkillManager;




    [HideInInspector]
    public  PAWN_ANIMATION_KIND     ePawnAniState;

    //스파인 애니메이션.
    [HideInInspector]
    public  SkeletonAnimation       PawnSpine;
    private Material                SpineMaterial = null;
    private MeshRenderer            SpineMeshRender;

    //모션 애니메이션.
    [HideInInspector]
    public  Animation               MotionAnimation = null;

    private bool                    bWaitChangeMotion_Idle;
    private float                   fCurTime_IdleMotion;       //히트, 넉백의 모션 정상화.
    private float                   fMaxTime_IdleMotion;


    //투사체 위치 오브젝트.
    private GameObject              PosObj_ThrowBase;

    //피격 위치 오브젝트.
    private GameObject              PosObj_HitBase;

    //스킬사용 헤드업 이펙트.
    private GameObject              PosObj_HeadUpEffect;

    //체력바 헤드업 위치.
    private GameObject              PosObj_HeadUpHPGauge;


    //그림자.
    private GameObject              ShadowObject;

    //사운드.
    private AudioSource             PawnAudioSource;

    //HP게이지.
    [HideInInspector]
    public  BattleGauge_HP          HPGaugeObject;


    //기본 데이터.
    private int         PawnKey;            //폰키 - 전투제어용 고유키.
    [HideInInspector]
    public  bool        LeaderPawn;         //리더폰.
    [HideInInspector]
    public  bool        HeroTeam;
    [HideInInspector]
    public  float       LookDirection;
    [HideInInspector]
    public  float       MoveDirection;


    //객체제어용.
    [HideInInspector]
    public  Transform       PawnTransform;
    [HideInInspector]
    public  float           FirstPos_Y;
    [HideInInspector]
    public  float           FirstPos_Z;
    private string          AttackMotionName;
    private string          BackMotionName;


    //강제이동관련 데이터.
    [HideInInspector]
    public  bool            bRangeMoveMode;
    private bool            bRangeMovePositionMode;
    private float           fRangeMove_Cur;
    private float           fRangeMove_Max;
    private float           fRangeMove_PosX;

    //색상변경 관련 데이터.
    private bool            bChangeColorMode = false;
    private bool            bChangeEnd = false;
    private float           fCurTime_ColorChange;
    private float           fMaxTime_ColorChange;

    //충격시 일시정지.
    private bool            bWaitHitDelay;
    private float           fCurTime_HitDelay;
    private float           fMaxTime_HitDelay;


    //타격대 공격 후 대기.
    [HideInInspector]
    public  bool            bHeadingDelay;
    [HideInInspector]
    public  float           fCurTime_HeadingDelay;
    [HideInInspector]
    public  float           fMaxTime_HeadingDelay;


    //일반 공격(근접딜러 제외) 딜레이.
    [HideInInspector]
    public  bool            bAttackReady;
    [HideInInspector]
    public  float           fCurTime_AttackWait;
    private float           fMaxTime_AttackWait;


    //밀어내기.
    //사용변수.
    [HideInInspector]
    public bool KnockbackMode;
    [HideInInspector]
    public KNOCKBACK_TYPE KnockbackKind;
    [HideInInspector]
    public bool bModule_Push;
    private Vector3 pPush_NextPos;
    private float fPush_BaseLength;
    private float fPush_CurPower;
    private float fPush_StopPower;
    private float fPush_Dir;
    private float fBreakPower = 2.0f;

    //점프.
    //사용변수.
    [HideInInspector]
    public bool bModule_Jump;
    private float fJump_Gravity = 9.8f;
    private float fJump_CurPower;
    private float fJump_Speed = 2.0f;

    private int nJumpCount = 0;

    //스킬사용 대기.
    [HideInInspector]
    public  bool            WaitUseSkillMode;

    //스킬 AI 제어용.
    [HideInInspector]
    public  bool            Skill_AI_Mode;

    [HideInInspector]
    public  int             Check_HitCount;     //맞은 횟수.
    [HideInInspector]
    public  int             Check_MissCount;    //회피 횟수.
    [HideInInspector]
    public  float           Check_TimeCount;    //시간 제어.
    [HideInInspector]
    public  bool            Check_AreaMode;
    [HideInInspector]
    public  float           Check_Area_Pos_Min;
    [HideInInspector]
    public  float           Check_Area_Pos_Max;
    [HideInInspector]
    public  float           Check_AreaInPawnCount;




    //스스로 삭제.
    [HideInInspector]
    public  bool            ActiveStuffObject;
    [HideInInspector]
    public  bool            ActiveDummyPawn;
    [HideInInspector]
    public  float           SelfDestroyTime;
    private float           CurDestroyTime;

    private BattlePawn      SummonerPawn;


    //투사체 경로.
    private string          DirBattleBullet_Throw = "Prefabs/Battle/ThrowObject/BattleBullet_Throw";
    private string          DirBattleBullet_Magic = "Prefabs/Battle/ThrowObject/BattleBullet_Magic";
    private string          DirBattleBullet_Heal = "Prefabs/Battle/ThrowObject/BattleBullet_Heal";

    //총 발사 이펙트.
    private GameObject      GunFireEffect;  

    //객체용.
    public int              StuffIndex;
    public DB_Skill.Schema  StuffSkillData;
    public BattlePawn       StuffBasePawn;

    public  bool            FollowStuff;
    public  BattlePawn      FollowTarget;

    public  Vector3         StuffDestroyPos;


    public  bool            AttackStuff;
    public  float           CurStuffAttack_Time;
    public  float           MaxStuffAttack_Time;

    public  SkillType       Stuff_SkillType;



    //버프 이펙트.
    public  GameObject      BuffCircle_Good;
    public  GameObject      BuffCircle_Bad;
    
    [HideInInspector]
    public  GameObject      TankerPushEffect;


    //PVE.
    [HideInInspector]
    public  int             PVE_GroupIndex;



    //외부 거리체크용 변수.
    [HideInInspector]
    public  float           TargetLengthValue;


    //스킬 사용 턴.
    [HideInInspector]
    public  bool            SkillUseTurn;


    [HideInInspector]
    public  AudioClip       LoopSoundClip;


    ////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  스크립트 시작.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////

    //초기화.
    public void InitBattlePawn(bool bHeroTeam, bool bLeader, int nCardIndex, int nPawnKey, int nLevel, int nEquipLevel_Acc, int nEquipLevel_Weapon, int nEquipLevel_Armor, int nSkillLevel, float fStat_Compensate = 1.0f)
    {
        //일반 폰.
        Pawn_Type = PAWN_TYPE.PAWN;

        BossPawn = false;

        //초기화.
        PawnTransform = transform;
        GameObject SpineObject = PawnTransform.FindChild("PawnAnimation").gameObject;
        PawnSpine = SpineObject.GetComponent<SkeletonAnimation>();
        SpineMaterial = SpineObject.GetComponent<MeshRenderer>().material;
        SpineMeshRender = SpineObject.GetComponent<MeshRenderer>();
        MotionAnimation = SpineObject.GetComponent<Animation>();
        ShadowObject = PawnTransform.FindChild("EF_Shadow").gameObject;
        BattleMng = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        PawnAudioSource = GetComponent<AudioSource>();


        PosObj_ThrowBase = PawnTransform.FindChild("ThrowPos").gameObject;
        PosObj_HitBase = PawnTransform.FindChild("HitPos").gameObject;
        PosObj_HeadUpHPGauge = PawnTransform.FindChild("HPGaugePos").gameObject;
        PosObj_HeadUpEffect = PawnTransform.FindChild("HPGaugePos").gameObject;


        //체력바 붙이기.
        if (bHeroTeam)
        {
            HPGaugeObject = null;
        }
        else
        {
            HPGaugeObject = Instantiate(BattleMng.EnemyHPGaugePrefab).GetComponent<BattleGauge_HP>();
            HPGaugeObject.transform.SetParent(BattleMng.pBattleUI.HPGaugeParent);
            HPGaugeObject.transform.localPosition = Vector3.zero;
            HPGaugeObject.transform.localScale = Vector3.one;
        }

        PawnKey = nPawnKey;
        InitBattleBaseData(nCardIndex, nLevel, nEquipLevel_Acc, nEquipLevel_Weapon, nEquipLevel_Armor, nSkillLevel, fStat_Compensate, bHeroTeam);
        HeroTeam = bHeroTeam;
        LeaderPawn = bLeader;

        if (HeroTeam)
        {
            LookDirection = -1.0f;
            MoveDirection = 1.0f;
        }
        else
        {
            LookDirection = 1.0f;
            MoveDirection = -1.0f;
        }


        //탄환 로딩.
        ThrowBattleBullet.Clear();
        if (IsFarAttack_Normal() == true)    //직업상 원거리 공격이면.
        {
            for (int idx = 0; idx < 2; idx++)   //처음에 두개만...
            {
                GameObject pBulletObject = Instantiate(Resources.Load(DirBattleBullet_Throw)) as GameObject;
                if (pBulletObject != null)
                {
                    BattleBullet pTempBulletData = pBulletObject.GetComponent<BattleBullet>();
                    pTempBulletData.InitBullet(BattleMng, this, BATTLE_BULLET_TYPE.HORIZON);
                    ThrowBattleBullet.Add(pTempBulletData);
                }
            }
        }

        if (IsHealer() == true)             //힐러면 두개의 프리팹을 보유해야한다.
        {
            GameObject pBulletObject_Att = Instantiate(Resources.Load(DirBattleBullet_Throw)) as GameObject;
            if (pBulletObject_Att != null)
            {
                for (int idx = 0; idx < 2; idx++)
                {
                    BattleBullet pTempBulletData = pBulletObject_Att.GetComponent<BattleBullet>();
                    pTempBulletData.InitBullet(BattleMng, this, BATTLE_BULLET_TYPE.HORIZON);
                    ThrowBattleBullet.Add(pTempBulletData);
                }
            }

            GameObject pBulletObject_Heal = Instantiate(Resources.Load(DirBattleBullet_Heal)) as GameObject;
            if (pBulletObject_Heal != null)
            {
                HealMagicBullet = pBulletObject_Heal.GetComponent<BattleBullet>();
                HealMagicBullet.InitBullet(BattleMng, this, BATTLE_BULLET_TYPE.MAGIC_TARGET_HEAL);
            }
        }

        GunFireEffect = null;
        if (nCardIndex == 11)    //서부뭉크.
        {
            GunFireEffect = Instantiate(Resources.Load("Effects/EF_Gunfire_fx01")) as GameObject;
            GunFireEffect.SetActive(false);
        }

        //스킬로딩.
        SkillManager = new BattleSkillManager();
        SkillManager.InitBattleSkillManager(BattleMng, this, LeaderPawn);


        //스파인 로드.
        string CharDir = "Spines/Character/" + DBData_Base.IdentificationName + "/" + DBData_Base.IdentificationName + "_SkeletonData";
        try
        {
            PawnSpine.skeletonDataAsset = (SkeletonDataAsset)Resources.Load(CharDir);
        }
        catch (System.Exception error)
        {
            Debug.LogWarning(error);
            Debug.LogError("File Not Find : " + CharDir);
            return;
        }

        PawnSpine.initialSkinName = DBData_Base.IdentificationName;

        SpineMaterial = PawnSpine.skeletonDataAsset.atlasAssets[0].materials[0];
        SpineMaterial.SetFloat("_TextureFade", 0);

        //애니메이션 이름이 없거나 다른 경우가 있어서 특수처리해준다.
        Spine.AnimationStateData pStatData = PawnSpine.skeletonDataAsset.GetAnimationStateData();

        if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_ATT_1) != null)
            AttackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_ATT_1;
        else if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_ATT) != null)
            AttackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_ATT;


        if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_BACK) != null)
            BackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_BACK;
        else if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_KNOCKBACK) != null)
            BackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_KNOCKBACK;


        //AI모듈 생성.
        AI_Module = new BattleAI();
        AI_Module.InitBattleAI(BattleMng, this);

        //특수액션 생성.
        SpecialActionMng = new SpecialActionManager();
        SpecialActionMng.InitSpecialAction(this);

        //서브 기능 데이터 초기화.
        InitSubData();

        //버프서클 이펙트.
        BuffCircle_Good.SetActive(false);
        BuffCircle_Bad.SetActive(false);


        //모션 초기화.
        PawnSpine.timeScale = 1.0f;
        SetMotion(PAWN_ANIMATION_KIND.IDLE);


        //탱커푸시 이펙트.
        if (GetClassType() == ClassType.ClassType_Keeper)
        {
            TankerPushEffect = Instantiate(Resources.Load("Effects/EF_Hit_Basic_Loop")) as GameObject;
            TankerPushEffect.transform.position = Vector3.zero;
            TankerPushEffect.transform.localScale = Vector3.one;
            TankerPushEffect.SetActive(false);
        }
        else
            TankerPushEffect = null;

        SpineMeshRender.sortingOrder = 5;


        AddLoopSoundClip();

    }



    private void InitSubData()
    {
        //색상변경.
        bChangeColorMode = false;
        bChangeEnd = true;
        fCurTime_ColorChange = 0.0f;
        fMaxTime_ColorChange = 0.2f;

        bWaitChangeMotion_Idle = false;
        fCurTime_IdleMotion = 0.0f;             //히트, 넉백의 모션 정상화시간.
        fMaxTime_IdleMotion = 0.0f;

        bWaitHitDelay = false;
        fCurTime_HitDelay = 0.0f;
        fMaxTime_HitDelay = 0.05f;


        bAttackReady = false;
        fCurTime_AttackWait = 0.0f;
        fMaxTime_AttackWait = AttackSpeed;


        bHeadingDelay = false;
        fCurTime_HeadingDelay = 0.0f;
        fMaxTime_HeadingDelay = AttackSpeed;

        WaitUseSkillMode = false;
    }






    public void InitPawnStuff(bool bHeroTeam, int nStuffIndex, int HP, float fSelfDestroyTime)
    {
        Pawn_Type = PAWN_TYPE.STUFF;
        StuffIndex = nStuffIndex;
        BossPawn = false;

        PawnTransform = transform;
        GameObject SpineObject = PawnTransform.FindChild("PawnAnimation").gameObject;
        PawnSpine = SpineObject.GetComponent<SkeletonAnimation>();
        SpineMaterial = SpineObject.GetComponent<MeshRenderer>().material;
        SpineMeshRender = SpineObject.GetComponent<MeshRenderer>();
        MotionAnimation = SpineObject.GetComponent<Animation>();
        ShadowObject = PawnTransform.FindChild("EF_Shadow").gameObject;
        BattleMng = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        PawnAudioSource = GetComponent<AudioSource>();

        PosObj_ThrowBase = PawnTransform.FindChild("ThrowPos").gameObject;
        PosObj_HitBase = PawnTransform.FindChild("HitPos").gameObject;
        PosObj_HeadUpHPGauge = PawnTransform.FindChild("HPGaugePos").gameObject;
        PosObj_HeadUpEffect = PawnTransform.FindChild("HPGaugePos").gameObject;

        switch (StuffIndex)
        {
            case 24:    //향로.
                GameObject pTempBodyPot = Instantiate(Resources.Load("Effects/EF_Healing_incense_001")) as GameObject;
                pTempBodyPot.transform.parent = transform;
                StuffDestroyPos = new Vector3(0.0f, 0.0f, 0.0f);
                pTempBodyPot.transform.localPosition = StuffDestroyPos;
                pTempBodyPot.transform.localScale = Vector3.one;
                FollowStuff = false;
                AttackStuff = false;
                AddLoopSoundClip_Stuff(StuffIndex);
                break;

            case 43:    //구름.
            case 1002:
                GameObject pTempBodyCloud = Instantiate(Resources.Load("Effects/EF_Electric_Cloud_001")) as GameObject;
                pTempBodyCloud.transform.parent = transform;
                StuffDestroyPos = new Vector3(0.0f, 3.0f, 0.0f);
                pTempBodyCloud.transform.localPosition = StuffDestroyPos;
                pTempBodyCloud.transform.localScale = Vector3.one;

//                PosObj_HitBase.transform.localPosition = PosObj_HitBase.transform.localPosition + StuffDestroyPos;

                FollowStuff = true;

                AttackStuff = true;
                CurStuffAttack_Time = 0.0f;
                MaxStuffAttack_Time = 1.0f;
                break;
        }

        SpineObject.SetActive(false);

        BaseHP = HP;
        CurHP = MaxHP = HP;

        HPGaugeObject = Instantiate(BattleMng.PawnHPGaugePrefab).GetComponent<BattleGauge_HP>();
        HPGaugeObject.transform.SetParent(BattleMng.pBattleUI.HPGaugeParent);
        HPGaugeObject.transform.localPosition = Vector3.zero;
        HPGaugeObject.transform.localScale = Vector3.one;


        ActiveStuffObject = false;
        SelfDestroyTime = fSelfDestroyTime;
        CurDestroyTime = 0.0f;


        HeroTeam = bHeroTeam;
        if (HeroTeam)
        {
            LookDirection = -1.0f;
            MoveDirection = 1.0f;
        }
        else
        {
            LookDirection = 1.0f;
            MoveDirection = -1.0f;
        }

        SpineMeshRender.sortingOrder = 6;
    }


    public void ResetStuff(BattlePawn BasePawn, Vector3 SetPos, DB_Skill.Schema SkillData, SkillType eSkillType, BattlePawn TargetPawn = null)
    {
        StuffSkillData = SkillData;
        StuffBasePawn = BasePawn;
        Stuff_SkillType = eSkillType;

        List<BattlePawn> tempList = null;
        if (HeroTeam)
            tempList = BattleMng.HeroPawnList;
        else
            tempList = BattleMng.EnemyPawnList;
        

        if (ActiveStuffObject)
        {
            if (!FollowStuff)
            {
                //버프 회수.
                for (int idx = 0; idx < tempList.Count; idx++)
                {
                    if (tempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                        continue;

                    tempList[idx].RemoveBuffElement(StuffSkillData.BuffNumber_1);
                    tempList[idx].RemoveBuffElement(StuffSkillData.BuffNumber_2);
                }
            }
            BattleMng.pEffectPoolMng.SetBattleEffect(transform.position + StuffDestroyPos, BattleMng.EffectID_Bomb);
            Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_EXPLOSION_1);

        }

        ActiveStuffObject = true;

        CurHP = MaxHP = BaseHP;
        CurDestroyTime = 0.0f;
        CurStuffAttack_Time = 0.0f;

        //타입에 따른 효과 초기화.

        if (HeroTeam)
            BattleMng.HeroPawnList.Add(this);
        else
            BattleMng.EnemyPawnList.Add(this);


        if (FollowStuff)
        {
            FollowTarget = TargetPawn;
        }
        else
        {
            //좌표에 설치.
            transform.position = SetPos;

            //근처에 있으면 밀어버리고.
            BattleMng.ForceKnockback(!HeroTeam, SetPos);

            //버프 부여.
            for (int idx = 0; idx < tempList.Count; idx++)
            {
                if (tempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                tempList[idx].AddBuff_Stuff(StuffBasePawn, StuffSkillData.BuffNumber_1, Stuff_SkillType);
                tempList[idx].AddBuff_Stuff(StuffBasePawn, StuffSkillData.BuffNumber_2, Stuff_SkillType);
            }
        }
        BattleMng.pEffectPoolMng.SetBattleEffect(transform.position, BattleMng.EffectID_Smoke);
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SUMMON);

        ActiveLoopSound();
    }

    public void DestroyStuff()
    {
        ActiveStuffObject = false;

        List<BattlePawn> tempList = null;
        if (HeroTeam)
            tempList = BattleMng.HeroPawnList;
        else
            tempList = BattleMng.EnemyPawnList;

        tempList.Remove(this);
        gameObject.SetActive(false);
        HPGaugeObject.HideBattleGauge_HP();

        BattleMng.pEffectPoolMng.SetBattleEffect(transform.position + StuffDestroyPos, BattleMng.EffectID_Bomb);
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_EXPLOSION_1);

        if (!FollowStuff)
        {
            //버프 회수.
            for (int idx = 0; idx < tempList.Count; idx++)
            {
                if (tempList[idx].Pawn_Type != PAWN_TYPE.PAWN)
                    continue;

                tempList[idx].RemoveBuffElement(StuffSkillData.BuffNumber_1);
                tempList[idx].RemoveBuffElement(StuffSkillData.BuffNumber_2);
            }
        }

        StopLoopSound();
    }




















    //소환한 더미 폰.
    public void InitDummyPawn(BattlePawn pSummonerPawn, SkillType eSkillType, float fSelfDestroyTime)
    {
        //일반 폰.
        Pawn_Type = PAWN_TYPE.SUMMONER;
        SummonerPawn = pSummonerPawn;
        BossPawn = false;

        DB_SummonCardData.Schema SummonData;
        if (eSkillType == SkillType.Max)
            SummonData = DB_SummonCardData.Query(DB_SummonCardData.Field.SummonIndex, pSummonerPawn.SkillManager.DB_MaxSkill.BaseValue);
        else
            SummonData = DB_SummonCardData.Query(DB_SummonCardData.Field.SummonIndex, pSummonerPawn.SkillManager.DB_ActiveSkill.BaseValue);


        //초기화.
        PawnTransform = transform;
        GameObject SpineObject = PawnTransform.FindChild("PawnAnimation").gameObject;
        PawnSpine = SpineObject.GetComponent<SkeletonAnimation>();
        SpineMaterial = SpineObject.GetComponent<MeshRenderer>().material;
        SpineMeshRender = SpineObject.GetComponent<MeshRenderer>();
        MotionAnimation = SpineObject.GetComponent<Animation>();
        ShadowObject = PawnTransform.FindChild("EF_Shadow").gameObject;
        BattleMng = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        PawnAudioSource = GetComponent<AudioSource>();


        PosObj_ThrowBase = PawnTransform.FindChild("ThrowPos").gameObject;
        PosObj_HitBase = PawnTransform.FindChild("HitPos").gameObject;
        PosObj_HeadUpHPGauge = PawnTransform.FindChild("HPGaugePos").gameObject;
        PosObj_HeadUpEffect = PawnTransform.FindChild("HPGaugePos").gameObject;


        //체력바 붙이기.
        HPGaugeObject = Instantiate(BattleMng.PawnHPGaugePrefab).GetComponent<BattleGauge_HP>();
        HPGaugeObject.transform.SetParent(BattleMng.pBattleUI.HPGaugeParent);
        HPGaugeObject.transform.localPosition = Vector3.zero;
        HPGaugeObject.transform.localScale = Vector3.one;

        ActiveStuffObject = false;
        SelfDestroyTime = fSelfDestroyTime;
        CurDestroyTime = 0.0f;

        HeroTeam = SummonerPawn.HeroTeam;
        if (HeroTeam)
        {
            PawnKey = 15000 + SummonerPawn.GetBattlePawnKey();
            LookDirection = -1.0f;
            MoveDirection = 1.0f;
        }
        else
        {
            PawnKey = 25000 + SummonerPawn.GetBattlePawnKey();
            LookDirection = 1.0f;
            MoveDirection = -1.0f;
        }

        float SValue_HP = 0.0f;
        float SValue_AP = 0.0f;
        float SValue_DP = 0.0f;

        switch (SummonerPawn.SkillLevel)
        {
            case 1:
                SValue_HP = SummonData.HP_Lv1;
                SValue_AP = SummonData.AP_Lv1;
                SValue_DP = SummonData.DP_Lv1;
                break;
            case 2:
                SValue_HP = SummonData.HP_Lv2;
                SValue_AP = SummonData.AP_Lv2;
                SValue_DP = SummonData.DP_Lv2;
                break;
            case 3:
                SValue_HP = SummonData.HP_Lv3;
                SValue_AP = SummonData.AP_Lv3;
                SValue_DP = SummonData.DP_Lv3;
                break;
            case 4:
                SValue_HP = SummonData.HP_Lv4;
                SValue_AP = SummonData.AP_Lv4;
                SValue_DP = SummonData.DP_Lv4;
                break;
            case 5:
                SValue_HP = SummonData.HP_Lv5;
                SValue_AP = SummonData.AP_Lv5;
                SValue_DP = SummonData.DP_Lv5;
                break;
        }



        InitBattleBaseData(SummonerPawn.CardIndex, SummonerPawn.Level, SummonerPawn.EquipLevel_Acc, SummonerPawn.EquipLevel_Weapon, SummonerPawn.EquipLevel_Armor, SummonerPawn.SkillLevel, 1.0f, SummonerPawn.HeroTeam);

        

        //체력, 공격력, 방어력 제어.
        BaseHP = (int)((float)BaseHP * SValue_HP);
        BaseAP = (int)((float)BaseAP * SValue_AP);
        BaseDP = (int)((float)BaseDP * SValue_DP);

        CurHP = MaxHP = BaseHP + EquipHP;
        CurAP = BaseAP + EquipAP;
        CurDP = BaseDP + EquipDP;

        AttackRange = 1.6f;
        LeaderPawn = false;

        //탄환 로딩.
        ThrowBattleBullet = null;

        //스킬로딩.
        SkillManager = null;

        //스파인 로드.
        string CharDir = "Spines/Character/" + SummonData.SpineName + "/" + SummonData.SpineName + "_SkeletonData";
        try
        {
            PawnSpine.skeletonDataAsset = (SkeletonDataAsset)Resources.Load(CharDir);
        }
        catch (System.Exception error)
        {
            Debug.LogWarning(error);
            Debug.LogError("File Not Find : " + CharDir);
            return;
        }

        PawnSpine.initialSkinName = SummonData.SpineName;

        SpineMaterial = PawnSpine.skeletonDataAsset.atlasAssets[0].materials[0];
        SpineMaterial.SetFloat("_TextureFade", 0);

        //애니메이션 이름이 없거나 다른 경우가 있어서 특수처리해준다.
        Spine.AnimationStateData pStatData = PawnSpine.skeletonDataAsset.GetAnimationStateData();

        if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_ATT_1) != null)
            AttackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_ATT_1;
        else if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_ATT) != null)
            AttackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_ATT;


        if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_BACK) != null)
            BackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_BACK;
        else if (pStatData.skeletonData.FindAnimation(PAWN_ANIMATION_NAME.P_ANI_NAME_KNOCKBACK) != null)
            BackMotionName = PAWN_ANIMATION_NAME.P_ANI_NAME_KNOCKBACK;


        //AI모듈 생성.
        AI_Module = new BattleAI();
        AI_Module.InitBattleAI(BattleMng, this);

        //서브 기능 데이터 초기화.
        InitSubData();

        //모션 초기화.
        PawnSpine.timeScale = 1.0f;
        SetMotion(PAWN_ANIMATION_KIND.IDLE);


        SpineMeshRender.sortingOrder = 6;
    }




    public void ResetDummyPawn(BattlePawn BasePawn, Vector3 SetPos, SkillType eSkillType)
    {
        List<BattlePawn> tempList = null;
        if (HeroTeam)
            tempList = BattleMng.HeroPawnList;
        else
            tempList = BattleMng.EnemyPawnList;


        if (ActiveDummyPawn)
        {
            BattleMng.pEffectPoolMng.SetBattleEffect(transform.position, BattleMng.EffectID_Smoke);
        }

        ActiveDummyPawn = true;
        CancelInvoke("DestroyPawn");
        KnockbackMode = false;
        KnockbackKind = KNOCKBACK_TYPE.KNOCKBACK_SLIDE;

        CurHP = MaxHP = BaseHP;
        CurDestroyTime = 0.0f;
        bModule_Push = false;
        bModule_Jump = false;
        PawnSpine.gameObject.SetActive(true);
        ShadowObject.SetActive(true);


        ePawnAniState = PAWN_ANIMATION_KIND.IDLE;
        SetMotion(PAWN_ANIMATION_KIND.IDLE, true);

        //타입에 따른 효과 초기화.

        if (HeroTeam)
            BattleMng.HeroPawnList.Add(this);
        else
            BattleMng.EnemyPawnList.Add(this);

        //좌표에 소환.
        FirstPos_Y = BasePawn.FirstPos_Y;
        FirstPos_Z = BasePawn.FirstPos_Z;
        transform.position = new Vector3(SetPos.x, BasePawn.FirstPos_Y, BasePawn.FirstPos_Z);

        float fSize = ScaleSize;
        PushRange = BasePushRange;

        if (eSkillType == SkillType.Max)
            fSize *= 1.5f;

        transform.localScale = new Vector3(LookDirection * fSize, fSize, fSize);
        BattleMng.pEffectPoolMng.SetBattleEffect(transform.position, BattleMng.EffectID_Smoke);

        //근처에 있으면 밀어버리고.
        BattleMng.ForceKnockback(!HeroTeam, SetPos);
    }

    public void DestroyDummyPawn()
    {
        ActiveDummyPawn = false;

        List<BattlePawn> tempList = null;
        if (HeroTeam)
            tempList = BattleMng.HeroPawnList;
        else
            tempList = BattleMng.EnemyPawnList;

        tempList.Remove(this);
        HPGaugeObject.HideBattleGauge_HP();

        BattleMng.pEffectPoolMng.SetBattleEffect(transform.position, BattleMng.EffectID_Smoke);
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_SUMMON);

        Destroy(gameObject);
    }


















    public int GetBattlePawnKey()
    {
        return PawnKey;
    }



    //일반 렌더링 업데이트.
    void Update()
    {
        if(TankerPushEffect != null)
        {
            if (BattleMng.eBattleStateKind != BATTLE_STATE_KIND.BATTLE && TankerPushEffect.activeInHierarchy)
            {
                TankerPushEffect.SetActive(false);
            }
        }


        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            if (HPGaugeObject != null)
            {
                float fCurHP = (float)CurHP * 1.0f / (float)MaxHP;
                if (fCurHP <= 0.0f)
                    HPGaugeObject.HideBattleGauge_HP();
                else
                {
//                    Vector3 GaugePos = BattleMng.PVE_FieldCamPos.transform.position + transform.position;
                    Vector3 GaugePos = transform.position;

                    if(FollowStuff)
                        HPGaugeObject.UpdateBattleGauge_HP(GaugePos + Vector3.up * 3.5f, fCurHP);
                    else
                        HPGaugeObject.UpdateBattleGauge_HP(GaugePos + Vector3.up * 1.2f, fCurHP);
                }
            }


            if(BattleMng.eBattleStateKind == BATTLE_STATE_KIND.BATTLE && !BattleMng.GamePauseMode)
            {
                CurDestroyTime += Time.deltaTime;
                if(CurDestroyTime >= SelfDestroyTime)
                {
                    CurDestroyTime = 0.0f;
                    DestroyStuff();
                }
            }
            return;
        }


        if (Pawn_Type == PAWN_TYPE.SUMMONER)
        {
            if (BattleMng.eBattleStateKind == BATTLE_STATE_KIND.BATTLE && !BattleMng.GamePauseMode)
            {
                if (SummonerPawn == null || SummonerPawn.IsDeath())
                {
                    DestroyDummyPawn();
                    return;
                }
                else
                {
                    CurDestroyTime += Time.deltaTime;
                    if (CurDestroyTime >= SelfDestroyTime)
                    {
                        CurDestroyTime = 0.0f;
                        DestroyDummyPawn();
                        return;
                    }
                }
            }
        }



        if(FreezeMode)
            SpineMeshRender.material.color = Color.blue;
        else
            SpineMeshRender.material.color = Color.white;


        if (!bChangeEnd)        //색상변경 코드.
        {
            fCurTime_ColorChange += Time.deltaTime;
            if (fCurTime_ColorChange >= fMaxTime_ColorChange)
                Reset_Color();

            if (bChangeColorMode)
                SpineMeshRender.material.SetFloat("_TextureFade", 1);
            else
            {
                SpineMeshRender.material.SetFloat("_TextureFade", 0);
                bChangeEnd = true;
            }
        }


        //체력바.
        if (HPGaugeObject != null && HPGaugeObject.NotDestroyGauge == false)
        {
            float AddPos = 0.0f;
            if (Pawn_Type == PAWN_TYPE.SUMMONER)
                AddPos = 0.1f;

            float fCurHP = (float)CurHP * 1.0f / (float)MaxHP;
            if (fCurHP <= 0.0f)
                HPGaugeObject.HideBattleGauge_HP();
            else
            {
//                Vector3 GaugePos = BattleMng.PVE_FieldCamPos.transform.position + (PosObj_HeadUpHPGauge.transform.position + Vector3.up * AddPos);
                Vector3 camTargetPos = BattleMng.PVE_FieldCamPos.transform.position;
                Vector3 GaugePos = new Vector3(-camTargetPos.x, camTargetPos.y, camTargetPos.z) + (PosObj_HeadUpHPGauge.transform.position + Vector3.up * AddPos);
                HPGaugeObject.UpdateBattleGauge_HP(GaugePos, fCurHP);
            }
        }
    }




    //행동제어용 FixedUpdate.
    //리플레이를 위해 FixedUpdate를 사용.
    void FixedUpdate()
    {
        if (SpecialActionMode)
        {
            SpecialActionMng.UpdateSpecialAction();

            //히트 딜레이.
            if (bWaitHitDelay)
            {
                fCurTime_HitDelay += Time.deltaTime;
                if (fCurTime_HitDelay >= fMaxTime_HitDelay)
                {
                    bWaitHitDelay = false;
                    fCurTime_HitDelay = 0.0f;
                    SetHitDelay(false);
                }
            }

            //그림자처리.
            ShadowObject.transform.position = new Vector3(PawnTransform.position.x, FirstPos_Y, PawnTransform.position.z + 0.3f);
            return;
        }


        if (BattleMng.eBattleStateKind == BATTLE_STATE_KIND.BATTLE_END || BattleMng.eBattleStateKind >= BATTLE_STATE_KIND.NETWORK_BATTLE_RESULT)  //배틀 이후일때.
        {
            if (ePawnAniState == PAWN_ANIMATION_KIND.MOVE)
                SetMotion(PAWN_ANIMATION_KIND.IDLE, true);
        }


        //밀어내기 이펙트.
        if (BattleMng.eBattleStateKind != BATTLE_STATE_KIND.BATTLE && TankerPushEffect != null)
        {
            if (TankerPushEffect.activeInHierarchy)
                TankerPushEffect.SetActive(false);
        }


        if (BattleMng.CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            if (!HeroTeam)
            {
                if (PVE_GroupIndex >= BattleMng.PVE_BattleGroupIndex)
                {
                    PawnTransform.localPosition = new Vector3(PawnTransform.localPosition.x, FirstPos_Y, PawnTransform.localPosition.z);

                    //그림자처리.
                    ShadowObject.transform.position = new Vector3(PawnTransform.position.x, FirstPos_Y, PawnTransform.position.z + 0.3f);
                    return;
                }
            }
        }


        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            if (FollowStuff)
            {
                if (FollowTarget == null || FollowTarget.IsDeath())
                    DestroyStuff();
                else
                    transform.position = new Vector3(FollowTarget.transform.position.x, FollowTarget.FirstPos_Y, FollowTarget.FirstPos_Z);

                if (AttackStuff)
                {
                    CurStuffAttack_Time += Time.deltaTime;
                    if (CurStuffAttack_Time >= MaxStuffAttack_Time)
                    {
                        CurStuffAttack_Time = 0.0f;
                        FollowTarget.GetDamage_Skill(StuffBasePawn, Stuff_SkillType, StuffBasePawn.SkillManager.SkillEffData_Active.EffID_Hit);
                        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_LIGHTNING);
                    }

                }
            }
            return;
        }

        //스킬사용.
        if (bUseSkillMode && !FreezeMode && !StunMode)
        {
            fUseSkillTime_Cur += Time.deltaTime;
            if (fUseSkillTime_Cur >= fUseSkillTime_Delay)
            {
                fUseSkillTime_Cur = 0.0f;
                SetUseSkill_Action(UseSkillType);
            }

            if (bWaitChangeMotion_Idle)  //모션 초기화 코드.
            {
                fCurTime_IdleMotion += Time.deltaTime;
                if (fCurTime_IdleMotion >= fMaxTime_IdleMotion)
                {
                    fCurTime_IdleMotion = 0.0f;
                    bWaitChangeMotion_Idle = false;
                    ResetIdleMotion();
                }
            }
            return;
        }

        if (BattleMng.GamePauseMode)
            return;

        //개별쿨타임.
        if (SkillCoolTime_Cur < SkillCoolTime_Max)
        {
            SkillCoolTime_Cur += Time.deltaTime;
            if (SkillCoolTime_Cur >= SkillCoolTime_Max)
                SkillCoolTime_Cur = SkillCoolTime_Max;
        }


        //버프 갱신.
        UpdateBuffList();

        bool GoodBuff = false;
        bool BadBuff = false;
        BuffValueUpdate(ref GoodBuff, ref BadBuff);

        BuffCircle_Good.SetActive(GoodBuff);
        BuffCircle_Bad.SetActive(BadBuff);


        //에어본 오류.
        if(PreAirborneMode == true && AirborneMode == false)    //에어본 걸렸던 상태면.
        {
            if (CurHP > 0)
            {
                if (ePawnAniState == PAWN_ANIMATION_KIND.SKILL_0 
                    || ePawnAniState == PAWN_ANIMATION_KIND.SKILL_1 
                    || ePawnAniState == PAWN_ANIMATION_KIND.SKILL_5 
                    || ePawnAniState == PAWN_ANIMATION_KIND.SKILL_4
                    || ePawnAniState == PAWN_ANIMATION_KIND.SKILL_MAX)
                    SetMotion(PAWN_ANIMATION_KIND.IDLE, true);
            }
            PreAirborneMode = false;
        }



        //공격속도 갱신.
        fMaxTime_AttackWait = AttackSpeed;

        if (FreezeMode)
        {
            SetMotion(PAWN_ANIMATION_KIND.FREEZE);
        }
        else
        {
            if (PreFreezeMode)
            {
                PreFreezeMode = false;
                SetMotion(PAWN_ANIMATION_KIND.IDLE);
            }
        }

        if (StunMode)
            SetMotion(PAWN_ANIMATION_KIND.STUN);
        else
        {
            if (PreStunMode)
            {
                PreStunMode = false;
                SetMotion(PAWN_ANIMATION_KIND.IDLE);
            }
        }

        //히트 딜레이.
        if (bWaitHitDelay)
        {
            fCurTime_HitDelay += Time.deltaTime;
            if (fCurTime_HitDelay >= fMaxTime_HitDelay)
            {
                bWaitHitDelay = false;
                fCurTime_HitDelay = 0.0f;
                SetHitDelay(false);
            }
            return;
        }


        //전투중일때 대기, 이동중일때만 AI 체크.
        if (BattleMng.eBattleStateKind == BATTLE_STATE_KIND.BATTLE || BattleMng.eBattleStateKind == BATTLE_STATE_KIND.BATTLE_START)
        {
            bool bSkillActiveMotion = false;

            switch (ePawnAniState)
            {
                case PAWN_ANIMATION_KIND.IDLE:
                case PAWN_ANIMATION_KIND.MOVE:
                case PAWN_ANIMATION_KIND.MOVE_CHARGING:
                case PAWN_ANIMATION_KIND.MOVE_BACK:
                case PAWN_ANIMATION_KIND.ATTACK_WAIT:
                    bSkillActiveMotion = true;
                    break;
            }

            if (WaitUseSkillMode && bSkillActiveMotion)
            {
                SetUseSkill_Motion();
                return;
            }
        }


        if (BattleMng.eBattleStateKind == BATTLE_STATE_KIND.BATTLE)
        {
            //스킬체크.

            Check_TimeCount += Time.deltaTime;

            bool bSkillActiveMotion = false;

            switch (ePawnAniState)
            {
                case PAWN_ANIMATION_KIND.IDLE:
                case PAWN_ANIMATION_KIND.MOVE:
                case PAWN_ANIMATION_KIND.MOVE_CHARGING:
                case PAWN_ANIMATION_KIND.MOVE_BACK:
                case PAWN_ANIMATION_KIND.ATTACK_WAIT:
                    bSkillActiveMotion = true;
                    break;
            }

            if (DummyMode)
                bSkillActiveMotion = false;


            if(GetClassType() == ClassType.ClassType_Keeper)
                AI_Module.UpdatePawnAI();

            if (bSkillActiveMotion)
            {
                if (GetClassType() != ClassType.ClassType_Keeper)
                    AI_Module.UpdatePawnAI();

                if (Pawn_Type == PAWN_TYPE.PAWN)
                {
                    if (SkillManager.AI_Mode)
                    {
                        //먼저 사용하고.
                        SkillManager.CheckUseSkill();

                        //체크.
                        if (BattleMng.IsWaitUseSkillCheck(HeroTeam))
                            SkillManager.UpdateSkill_AI();
                    }
                }
            }
        }

        //모션 초기화.
        if (bWaitChangeMotion_Idle)  //모션 초기화 코드.
        {
            fCurTime_IdleMotion += Time.deltaTime;
            if (fCurTime_IdleMotion >= fMaxTime_IdleMotion)
            {
                fCurTime_IdleMotion = 0.0f;
                bWaitChangeMotion_Idle = false;
                ResetIdleMotion();
            }
        }

        if (BattleMng.eBattleStateKind == BATTLE_STATE_KIND.MOVE_START && ePawnAniState == PAWN_ANIMATION_KIND.IDLE && bRangeMoveMode)
        {
            SetMotion(PAWN_ANIMATION_KIND.MOVE);
        }



        switch (ePawnAniState)
        {
            case PAWN_ANIMATION_KIND.IDLE:
                break;

            case PAWN_ANIMATION_KIND.ATTACK_WAIT:
                break;

            case PAWN_ANIMATION_KIND.ATTACK_PUSH:
            case PAWN_ANIMATION_KIND.ATTACK_KEEPER:
                break;

            case PAWN_ANIMATION_KIND.STUN:
                break;

            case PAWN_ANIMATION_KIND.FREEZE:
                PawnSpine.timeScale = 0.0f;
                break;


            case PAWN_ANIMATION_KIND.MOVE:
            case PAWN_ANIMATION_KIND.MOVE_CHARGING:
                float fMoveSpeed = MoveSpeed;
                if (bRangeMoveMode) //강제이동 모드일때는 같은 스피드로.
                    fMoveSpeed = 4.0f;

                if (AI_Module.TankerPushMode || (AI_Module.PushTankPawn != null && AI_Module.PushTankPawn.IsDeath() == false))
                    fMoveSpeed = TankerPushSpeed;

                if (AI_Module.TankerPushEqual)
                    fMoveSpeed = 0.0f;


                if (GetClassType() == ClassType.ClassType_Keeper && (ePawnAniState == PAWN_ANIMATION_KIND.MOVE || ePawnAniState == PAWN_ANIMATION_KIND.MOVE_CHARGING))
                {
                    if (AI_Module.TankerPushMode || (AI_Module.PushTankPawn != null && AI_Module.PushTankPawn.IsDeath() == false))
                        AnimationSpeed = 0.25f;
                    else
                        AnimationSpeed = 0.5f;

                    PawnSpine.timeScale = AnimationSpeed;
                }

                float fStepRange = (fMoveSpeed * Time.deltaTime);
                float fPos_X = PawnTransform.localPosition.x + (MoveDirection * fStepRange);
                PawnTransform.localPosition = new Vector3(fPos_X, PawnTransform.localPosition.y, PawnTransform.localPosition.z);

                //강제이동.
                if (bRangeMoveMode)
                {
                    if (bRangeMovePositionMode)
                    {
                        if (fPos_X >= fRangeMove_PosX)
                        {
                            fPos_X = fRangeMove_PosX;
                            PawnTransform.localPosition = new Vector3(fPos_X, PawnTransform.localPosition.y, PawnTransform.localPosition.z);
                            bRangeMoveMode = false;
                            SetMotion(PAWN_ANIMATION_KIND.IDLE);
                        }
                    }
                    else
                    {
                        fRangeMove_Cur += fStepRange;
                        if (fRangeMove_Cur >= fRangeMove_Max)
                        {
                            fRangeMove_Cur = 0.0f;
                            bRangeMoveMode = false;
                            SetMotion(PAWN_ANIMATION_KIND.IDLE);
                        }
                    }
                }
                break;

            case PAWN_ANIMATION_KIND.MOVE_BACK:
                float fBackMoveSpeed = MoveSpeed * 0.5f;

                float fBackStepRange = (fBackMoveSpeed * Time.deltaTime);
                float fBackPos_X = PawnTransform.localPosition.x + ((MoveDirection * -1.0f) * fBackStepRange);
                PawnTransform.localPosition = new Vector3(fBackPos_X, PawnTransform.localPosition.y, PawnTransform.localPosition.z);
                break;


            case PAWN_ANIMATION_KIND.KNOCKBACK:
                break;

            case PAWN_ANIMATION_KIND.AIRBORNE:
                if (MotionAnimation["aniPawnAirborne"].speed == 1.0f && MotionAnimation.isPlaying == false)
                {
                    MotionAnimation.transform.localPosition = Vector3.zero;
                    SetMotion(PAWN_ANIMATION_KIND.IDLE);
                }
                break;
            
            case PAWN_ANIMATION_KIND.HIT:
                break;
            
            case PAWN_ANIMATION_KIND.ATTACK_1:
//            case PAWN_ANIMATION_KIND.ATTACK_KEEPER:
                break;
        }


        //시간제어.
        if (bAttackReady == false)
        {
            fCurTime_AttackWait += Time.deltaTime;
            if (fCurTime_AttackWait >= fMaxTime_AttackWait)
            {
                bAttackReady = true;
                fCurTime_AttackWait = 0.0f;
            }
        }


        //AI모듈에 밀어내기 관련 좌표갱신.
        AI_Module.UpdatePushPosition();

        //모션 처리.
        UpdateKnockback();
        
        //그림자처리.
        ShadowObject.transform.position = new Vector3(PawnTransform.position.x, FirstPos_Y, PawnTransform.position.z + 0.3f);

    }










    //모션 설정.
    public void SetMotion(PAWN_ANIMATION_KIND eMotion, bool bForceChange = false)
    {
        if (SpecialActionMode)
            return;

        if (ePawnAniState == eMotion && bForceChange == false)
            return;

        if (ePawnAniState == PAWN_ANIMATION_KIND.DIE)
            return;


        if (!bForceChange)
        {
            if (FreezeMode)
            {
                if (eMotion == PAWN_ANIMATION_KIND.DIE)
                    ePawnAniState = eMotion;
                else
                {
                    ePawnAniState = PAWN_ANIMATION_KIND.FREEZE;
                    PawnSpine.timeScale = 0.0f;
                    return;
                }
            }
            else
            {
                if (ePawnAniState == PAWN_ANIMATION_KIND.FREEZE)
                    PawnSpine.timeScale = AnimationSpeed;

                ePawnAniState = eMotion;
            }


            if (AirborneMode)
            {
                if (eMotion != PAWN_ANIMATION_KIND.AIRBORNE && eMotion != PAWN_ANIMATION_KIND.DIE)
                    return;
            }
        }
        else
        {
            ePawnAniState = eMotion;
        }
    
        bool bLoop = false;
        bool bBackMove = false;

        MotionAnimation.Stop();
        MotionAnimation.transform.localPosition = Vector3.zero;
        AnimationSpeed = 1.0f;
        switch (ePawnAniState)
        {
            case PAWN_ANIMATION_KIND.PHASE_WAIT:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_IDLE;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.IDLE:
            case PAWN_ANIMATION_KIND.STUN:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_IDLE;
                AI_Module.TankerPushMode = false;
                AI_Module.PushTankPawn = null;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.ATTACK_WAIT:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_IDLE;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.ATTACK_PUSH:
                PawnSpine.AnimationName = BackMotionName;
                break;

            case PAWN_ANIMATION_KIND.MOVE_CHARGING:
                switch (CardIndex)
                {
                    case 1002:
                        PawnSpine.AnimationName = "win_x";
                        break;

                    case 1005:
                        PawnSpine.AnimationName = "run";
                        break;

                    default:
                        PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_CHARGING;
                        break;
                }

                bLoop = true;

                transform.localScale = new Vector3(LookDirection * ScaleSize, ScaleSize, ScaleSize);
                break;

            case PAWN_ANIMATION_KIND.MOVE:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_MOVE;
                bLoop = true;

                transform.localScale = new Vector3(LookDirection * ScaleSize, ScaleSize, ScaleSize);
                break;

            case PAWN_ANIMATION_KIND.MOVE_BACK:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_MOVE;
                bLoop = true;
                if (GetClassType() == ClassType.ClassType_Keeper)
                    AnimationSpeed = 0.5f;
                bBackMove = true;
                break;

            case PAWN_ANIMATION_KIND.KNOCKBACK:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_KNOCKBACK;
                ChangeTextureColor();
                break;

            case PAWN_ANIMATION_KIND.HIT:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_HIT;
                ChangeTextureColor();
                break;

            case PAWN_ANIMATION_KIND.ATTACK_KEEPER:
                switch (CardIndex)
                {
                    case 1002:
                        PawnSpine.AnimationName = "skill1";
                        break;

                    case 1005:
                        PawnSpine.AnimationName = "skill2";
                        break;

                    default:
                        PawnSpine.AnimationName = "attack1";
                        break;
                }
                break;

            case PAWN_ANIMATION_KIND.ATTACK_1:
                PawnSpine.AnimationName = AttackMotionName;
                break;

            case PAWN_ANIMATION_KIND.FREEZE:
                PawnSpine.timeScale = 0.0f;
                break;

            case PAWN_ANIMATION_KIND.WIN:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_WIN;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.DIE:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_DIE;

                if (SkillUseTurn)
                {
                    SkillUseTurn = false;
                    BattleMng.SetWaitUseSkillCheck(HeroTeam, true);
                }

                CurHP = 0;
                AI_Module.TankerPushMode = false;
                if (TankerPushEffect != null)
                    TankerPushEffect.SetActive(false);
                SetKnockBack(null, KNOCKBACK_TYPE.KNOCKBACK_DIE);

                if (HPGaugeObject != null)
                    HPGaugeObject.HideBattleGauge_HP();
                break;

            case PAWN_ANIMATION_KIND.SKILL_0:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_0;
                break;

            case PAWN_ANIMATION_KIND.SKILL_1:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_1;
                break;

            case PAWN_ANIMATION_KIND.SKILL_4:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_4;
                break;

            case PAWN_ANIMATION_KIND.SKILL_5:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_5;
                break;

            case PAWN_ANIMATION_KIND.SKILL_MAX:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_MAX;
                break;

            case PAWN_ANIMATION_KIND.AIRBORNE:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_AIRBORNE;
                bLoop = true;
                break;

        }
        PawnSpine.Reset();
        Spine.TrackEntry pCurAniData = PawnSpine.state.GetCurrent(0);
        pCurAniData.Loop = bLoop;
        PawnSpine.timeScale = AnimationSpeed;

        if(bBackMove)
            transform.localScale = new Vector3((LookDirection * -1.0f) * ScaleSize, ScaleSize, ScaleSize);
        else
            transform.localScale = new Vector3(LookDirection * ScaleSize, ScaleSize, ScaleSize);


        bool bNotIdleLink = false;
        if (ePawnAniState == PAWN_ANIMATION_KIND.KNOCKBACK)
            bNotIdleLink = true;

        if (bLoop == false && !bNotIdleLink)
        {
            bWaitChangeMotion_Idle = true;
            fCurTime_IdleMotion = 0.0f;       //히트, 넉백의 모션 정상화시간.
            fMaxTime_IdleMotion = pCurAniData.endTime;
        }

    }










    public float SetMotion_Manual(PAWN_ANIMATION_KIND eMotion)
    {
        bool bLoop = false;
        bool bBackMove = false;

        MotionAnimation.Stop();
        MotionAnimation.transform.localPosition = Vector3.zero;
        AnimationSpeed = 1.0f;
        switch (eMotion)
        {
            case PAWN_ANIMATION_KIND.PHASE_WAIT:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_IDLE;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.IDLE:
            case PAWN_ANIMATION_KIND.STUN:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_IDLE;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.ATTACK_WAIT:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_IDLE;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.ATTACK_PUSH:
                PawnSpine.AnimationName = BackMotionName;
                break;

            case PAWN_ANIMATION_KIND.MOVE_CHARGING:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_CHARGING;
                bLoop = true;

                transform.localScale = new Vector3(LookDirection * ScaleSize, ScaleSize, ScaleSize);
                break;

            case PAWN_ANIMATION_KIND.MOVE:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_MOVE;
                bLoop = true;

                transform.localScale = new Vector3(LookDirection * ScaleSize, ScaleSize, ScaleSize);
                break;

            case PAWN_ANIMATION_KIND.MOVE_BACK:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_MOVE;
                bLoop = true;
                if (GetClassType() == ClassType.ClassType_Keeper)
                    AnimationSpeed = 0.5f;
                bBackMove = true;
                break;

            case PAWN_ANIMATION_KIND.KNOCKBACK:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_KNOCKBACK;
                ChangeTextureColor();
                break;

            case PAWN_ANIMATION_KIND.HIT:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_HIT;
                ChangeTextureColor();
                break;

            case PAWN_ANIMATION_KIND.ATTACK_KEEPER:
                PawnSpine.AnimationName = "attack1";
                break;

            case PAWN_ANIMATION_KIND.ATTACK_1:
                PawnSpine.AnimationName = AttackMotionName;
                break;

            case PAWN_ANIMATION_KIND.FREEZE:
                PawnSpine.timeScale = 0.0f;
                break;

            case PAWN_ANIMATION_KIND.WIN:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_WIN;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.DIE:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_DIE;
                break;

            case PAWN_ANIMATION_KIND.SKILL_0:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_0;
                break;

            case PAWN_ANIMATION_KIND.SKILL_1:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_1;
                break;

            case PAWN_ANIMATION_KIND.SKILL_2:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_2;
                bLoop = true;
                break;

            case PAWN_ANIMATION_KIND.SKILL_4:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_4;
                break;

            case PAWN_ANIMATION_KIND.SKILL_5:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_5;
                break;

            case PAWN_ANIMATION_KIND.SKILL_MAX:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_SKILL_MAX;
                break;

            case PAWN_ANIMATION_KIND.AIRBORNE:
                PawnSpine.AnimationName = PAWN_ANIMATION_NAME.P_ANI_NAME_AIRBORNE;
                bLoop = true;
                break;

        }
        PawnSpine.Reset();
        Spine.TrackEntry pCurAniData = PawnSpine.state.GetCurrent(0);
        pCurAniData.Loop = bLoop;
        PawnSpine.timeScale = AnimationSpeed;

        if (bBackMove)
            transform.localScale = new Vector3((LookDirection * -1.0f) * ScaleSize, ScaleSize, ScaleSize);
        else
            transform.localScale = new Vector3(LookDirection * ScaleSize, ScaleSize, ScaleSize);


        return pCurAniData.endTime;
    }








    public void ResetIdleMotion()
    {
        SetMotion(PAWN_ANIMATION_KIND.IDLE);
    }



    public BattleBullet FindFreeBullet()
    {
        for (int idx = 0; idx < ThrowBattleBullet.Count; idx++)
        {
            if (ThrowBattleBullet[idx].ActiveBullet)
                continue;

            return ThrowBattleBullet[idx];
        }

        //없으면 생성.
        GameObject pBulletObject = Instantiate(Resources.Load(DirBattleBullet_Throw)) as GameObject;
        if (pBulletObject != null)
        {
            BattleBullet pTempBulletData = pBulletObject.GetComponent<BattleBullet>();
            pTempBulletData.InitBullet(BattleMng, this, BATTLE_BULLET_TYPE.HORIZON);
            ThrowBattleBullet.Add(pTempBulletData);
            return pTempBulletData;
        }

        return null;
    }



    public void SetThrowAttack_Normal()
    {
        if (ThrowBattleBullet.Count == 0)
            return;


        //공격시간 초기화.
        bAttackReady = false;
        fCurTime_AttackWait = 0.0f;

        BattlePawn pTarget = AI_Module.GetNearPawnData(false);
/*
        BattlePawn pTarget = AI_Module.GetNearPawnData_Far(false);
        if (pTarget == null)
            pTarget = AI_Module.GetNearPawnData(false);
 */
        BattleBullet pTempBullet = FindFreeBullet();
        pTempBullet.ShotBullet(GetPawnThrowPosition(), pTarget);
        ShowGunFireEffect();
    }



    public void SetCastingMagic(BattlePawn Target, bool UseHealMagic)
    {
        if (UseHealMagic)
        {
            if (HealMagicBullet == null)
                return;

            HealMagicBullet.CastingMagic(Target);
        }
        else
        {
            if (ThrowBattleBullet.Count == 0)
                return;

            BattleBullet pTempBullet = FindFreeBullet();
            pTempBullet.CastingMagic(Target);
        }

        //공격시간 초기화.
        bAttackReady = false;
        fCurTime_AttackWait = 0.0f;
    }







    public void SetAreaDamage()
    {
        List<BattlePawn> tempList = null;
        if (HeroTeam)
            tempList = BattleMng.EnemyPawnList;
        else
            tempList = BattleMng.HeroPawnList;

        for (int idx = 0; idx < tempList.Count; idx++)
        {
            if(tempList[idx].IsDeath())
                continue;

            float Length = BattleUtil.GetDistance_X(PawnTransform.position.x, tempList[idx].PawnTransform.position.x);

            if (Length <= (AttackRange + 1.0f))
            {
                int DmgValue = 0;
                bool isCritical = false;
                MakeBattleDamage(this, tempList[idx], ref DmgValue, ref isCritical);
                tempList[idx].GetDamage_Normal(DmgValue, isCritical, BattleMng.EffectID_DamageNormal);

                //데미지반사.
                if (tempList[idx].ReflectDmgMode)
                    GetDamage_Reflect(DmgValue, tempList[idx].ReflectAddValue);

                if(tempList[idx].GetClassType() != ClassType.ClassType_Keeper)
                    tempList[idx].SetKnockBack(this, KNOCKBACK_TYPE.KNOCKBACK_JUMP);
            }
        }
    }


    


    //사망체크.
    public bool IsDeath()
    {
        if (CurHP <= 0 || ePawnAniState == PAWN_ANIMATION_KIND.DIE)
            return true;

        if (BattleMng.CurBattleKind == BATTLE_KIND.PVE_BATTLE && !HeroTeam)
        {
            if (PVE_GroupIndex >= BattleMng.PVE_BattleGroupIndex)
                return true;
        }


        return false;
    }


    public int GetCurHP()
    {
        return CurHP;
    }












    ////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  스킬 스크립트.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////
    public void SetUseSkill(SkillType eSkillType, ref bool IsActiveSkill)
    {
        if (SilenceMode)
            return;

        if (SkillManager == null)
        {
            Debug.Log("Error_UseKSill -> " + CardIndex.ToString() + ", " + HeroTeam.ToString());
        }

        int SkillCost = SkillManager.GetDB_Skill(eSkillType).Cost;

        if (HeroTeam)
        {
            if (BattleMng.SkillUseDelay)
                return;

            if(SkillCoolTime_Cur >= SkillCoolTime_Max)
            {
                if(eSkillType != SkillType.Leader)
                    SkillCoolTime_Cur = 0.0f;

                WaitUseSkillMode = true;
                UseSkillType = eSkillType;

                BattleMng.SkillUseDelay = true;
                BattleMng.CurSkillUseDelayTime = 0.0f;
                IsActiveSkill = true;

                if (eSkillType != SkillType.Leader)
                {
                    if (BattleMng.CurComboCount_H == BattleMng.MaxComboCount)
                    {
                        BattleMng.CurComboCount_H = 0;
                        BattleMng.UseExSkill_H = false;
                    }
                    else
                    {
                        BattleMng.CurComboCount_H += SkillCost;
                        if (BattleMng.CurComboCount_H >= BattleMng.MaxComboCount)
                        {
                            BattleMng.CurComboCount_H = BattleMng.MaxComboCount;
                            BattleMng.UseExSkill_H = true;

                            if (Kernel.entry.tutorial.TutorialActive)
                            {
                                if (Kernel.entry.tutorial.GroupNumber == 10)
                                {
                                    if (Kernel.entry.tutorial.WaitSeq == 104)
                                    {
                                        BattleMng.BattlePause();
                                        Kernel.entry.tutorial.onSetNextTutorial();
                                    }
                                }
                            }


                        }
                    }
                }
            }
            else
            {
                BattleMng.pBattleUI.ShowBattleMsg(Languages.ToString(TEXT_UI.NOT_ENOUGH_MANA));
                return;
            }
        }
        else
        {
            if (eSkillType != SkillType.Leader)
            {
                if (BattleMng.EnemySkillUseDelay)
                    return;
            }

            if(SkillCoolTime_Cur >= SkillCoolTime_Max)
            {
                if (eSkillType != SkillType.Leader)
                    SkillCoolTime_Cur = 0.0f;
                WaitUseSkillMode = true;
                UseSkillType = eSkillType;
                BattleMng.EnemySkillUseDelay = true;
                BattleMng.CurEnemySkillUseDelayTime = 0.0f;
                IsActiveSkill = true;

                if (eSkillType != SkillType.Leader)
                {
                    if (BattleMng.CurComboCount_E == BattleMng.MaxComboCount)
                    {
                        BattleMng.CurComboCount_E = 0;
                        BattleMng.UseExSkill_E = false;
                    }
                    else
                    {
                        BattleMng.CurComboCount_E += SkillCost;
                        if (BattleMng.CurComboCount_E >= BattleMng.MaxComboCount)
                        {
                            BattleMng.CurComboCount_E = BattleMng.MaxComboCount;
                            BattleMng.UseExSkill_E = true;
                        }
                    }
                }
            }
            else
                return;
        }


        switch (SkillManager.GetDB_Skill(eSkillType).SKILLACTIVE_ACTION)
        {
            case SKILLACTIVE_ACTION.MY_HIT_COUNT:
            case SKILLACTIVE_ACTION.MYTEAM_HIT_COUNT:
                Check_HitCount = 0;
                break;

            case SKILLACTIVE_ACTION.TIME:
                Check_TimeCount = 0.0f;
                break;
        }
    }




    private bool        bUseSkillMode;
    private SkillType   UseSkillType;
    private float       fUseSkillTime_Cur;
    private float       fUseSkillTime_Delay;

    //스킬 사용 모션 제어.
    public void SetUseSkill_Motion()
    {
        if (bUseSkillMode)
            return;

        bUseSkillMode = true;
        fUseSkillTime_Cur = 0.0f;
        WaitUseSkillMode = false;

        DB_Skill.Schema UseSkillData = SkillManager.GetDB_Skill(UseSkillType);

        //애니메이션.
        fUseSkillTime_Delay = 0.0f;

        if (UseSkillType == SkillType.Max)
        {
            SetMotion(PAWN_ANIMATION_KIND.SKILL_MAX);
            fUseSkillTime_Delay = 1.8f;
        }
        else
        {
            if (UseSkillData.AnimationKey.Equals("skill1"))
            {
//                fUseSkillTime_Delay = 0.5f;
                SetMotion(PAWN_ANIMATION_KIND.SKILL_0);
            }
            else if (UseSkillData.AnimationKey.Equals("skill2"))
            {
//                fUseSkillTime_Delay = 1.2f;
                SetMotion(PAWN_ANIMATION_KIND.SKILL_1);
            }
            else if (UseSkillData.AnimationKey.Equals("skill4"))
            {
//                fUseSkillTime_Delay = 1.2f;
                SetMotion(PAWN_ANIMATION_KIND.SKILL_4);
            }
            else if (UseSkillData.AnimationKey.Equals("skill5"))
            {
//                fUseSkillTime_Delay = 1.2f;
                SetMotion(PAWN_ANIMATION_KIND.SKILL_5);
            }
        }


        //사용시 얼굴 이펙트.
        Invoke("ShowUseSkillEffect", 0.2f);
        if (HeroTeam)
            BattleMng.pBattleUI.UseSkillPanel_Hero.ShowSkillPanel(this, UseSkillType);
        else
            BattleMng.pBattleUI.UseSkillPanel_Enemy.ShowSkillPanel(this, UseSkillType);

        //일시정지 이펙트.
        BattleMng.SetBattle_Pause_UseSkill(this, UseSkillData, fUseSkillTime_Delay);


        //스킬별 개별처리.
        switch (UseSkillData.Index)
        {
            case 25:
                if (UseSkillData.SkillType == SkillType.Leader)
                {
                    SkillManager.RandomActive_Success = false;
                }
                else
                {
                    int RandomValue = Random.Range(0, 100);
                    if (RandomValue < 60)
                    {
                        SkillManager.RandomActive_Success = true;
                        if (UseSkillData.SkillType == SkillType.Max)
                            BattleMng.ShowTeamSkillEffect(HeroTeam, false, SkillManager.SkillEffData_Max.SubUseEffect[0]);
                        else
                            BattleMng.ShowTeamSkillEffect(HeroTeam, false, SkillManager.SkillEffData_Active.SubUseEffect[0]);

                        Invoke("RandomSound_Success", 1.0f);
                    }
                    else
                    {
                        SkillManager.RandomActive_Success = false;
                        if (UseSkillData.SkillType == SkillType.Max)
                            BattleMng.ShowTeamSkillEffect(HeroTeam, false, SkillManager.SkillEffData_Max.SubUseEffect[1]);
                        else
                            BattleMng.ShowTeamSkillEffect(HeroTeam, false, SkillManager.SkillEffData_Active.SubUseEffect[1]);

                        Invoke("RandomSound_Fail", 1.0f);
                    }
                }
                break;
        }

    }


    private void RandomSound_Success()
    {
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_RANDOM_SUCCESS);
    }

    private void RandomSound_Fail()
    {
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_RANDOM_FAIL);
    }


    //스킬 시연 이펙트 표시.
    public void ShowUseSkillEffect()
    {
        BattleMng.pEffectPoolMng.SetBattleEffect(GetPawnPosition_Center(), BattleMng.EffectID_UseSkill, 1.0f, 2.0f);

        if(UseSkillType == SkillType.Max)
            BattleMng.pEffectPoolMng.SetBattleEffect(GetPawnPosition_Center(), BattleMng.EffectID_MaxSkill, 1.0f, 0.1f);
    
        Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_ACT);
    }





    //스킬 동작.
    public void SetUseSkill_Action(SkillType eSkillType)
    {
        bUseSkillMode = false;
        BattleMng.SetBattle_Resume_UseSkill();

        if (eSkillType != SkillType.Leader && SpecialActionMng.IsSpecialActionSkill())
            SpecialActionMng.StartSpecialAction(eSkillType);
        else
            SkillManager.UseSkill(eSkillType);
    }








    //버프관련.
    public void AddBuff(BattlePawn  Attacker, SkillType eSkill)
    {
        DB_Skill.Schema TempSkillData = null;
        SkillEffectData TempEffectData = null;

        bool bLeader = false;
        if (eSkill == SkillType.Leader)
            bLeader = true;

        TempSkillData = Attacker.SkillManager.GetDB_Skill(eSkill);
        TempEffectData = Attacker.SkillManager.GetSkillEffectData(eSkill);

        bool GoodBuff = false;
        bool bOverlapMode = false;
        if (TempSkillData.BuffNumber_1 != 0)
        {
            DB_Buff.Schema tempBuffData_1 = DB_Buff.Query(DB_Buff.Field.Index, TempSkillData.BuffNumber_1);

            BuffElement pTempElement_0 = null;
            if (bLeader || !tempBuffData_1.Overlap_Check)
            {
                pTempElement_0 = GetEmptyBuffElement();
            }
            else
            {
                pTempElement_0 = GetOverlapBuff(Attacker.GetBattlePawnKey(), TempSkillData.BuffNumber_1);
                if (pTempElement_0 == null)
                    pTempElement_0 = GetEmptyBuffElement();
                else
                    bOverlapMode = true;
            }

            pTempElement_0.AddBuffElement(Attacker, this, bOverlapMode, TempSkillData.BuffNumber_1, Attacker.SkillLevel, TempEffectData.EffID_Buff1_Active, TempEffectData.EffID_Buff1_Hit, eSkill, bLeader);

            if (tempBuffData_1.BUFF_GOOD)
                GoodBuff = true;
        }

        bOverlapMode = false;
        if (TempSkillData.BuffNumber_2 != 0)
        {
            DB_Buff.Schema tempBuffData_2 = DB_Buff.Query(DB_Buff.Field.Index, TempSkillData.BuffNumber_2);

            BuffElement pTempElement_1 = null;

            if (bLeader || !tempBuffData_2.Overlap_Check)
            {
                pTempElement_1 = GetEmptyBuffElement();
            }
            else
            {
                pTempElement_1 = GetOverlapBuff(Attacker.GetBattlePawnKey(), TempSkillData.BuffNumber_2);
                if (pTempElement_1 == null)
                    pTempElement_1 = GetEmptyBuffElement();
                else
                    bOverlapMode = true;
            }

            pTempElement_1.AddBuffElement(Attacker, this, bOverlapMode, TempSkillData.BuffNumber_2, Attacker.SkillLevel, TempEffectData.EffID_Buff2_Active, TempEffectData.EffID_Buff2_Hit, eSkill, bLeader);

            if (tempBuffData_2.BUFF_GOOD)
                GoodBuff = true;
        }

        if(bLeader && GoodBuff)
            BattleMng.pEffectPoolMng.SetBattleEffect(GetPawnPosition_Center() + Vector3.up * 1.5f, BattleMng.EffectID_UseSkill_Leader, 2.0f);
    }


    public void AddBuff_Stuff(BattlePawn Attacker, int BuffIndex, SkillType eSkill)
    {
        bool bOverlapMode = false;

        if (BuffIndex != 0)
        {
            DB_Buff.Schema tempBuffData = DB_Buff.Query(DB_Buff.Field.Index, BuffIndex);

            BuffElement pTempElement = null;
            if (tempBuffData.Overlap_Check)
            {
                pTempElement = GetOverlapBuff(Attacker.GetBattlePawnKey(), BuffIndex);
                if (pTempElement == null)
                    pTempElement = GetEmptyBuffElement();
                else
                    bOverlapMode = true;
            }
            else
                pTempElement = GetEmptyBuffElement();

            int nEffectHit_ID = -1;
            switch (BuffIndex)
            {
                case 24:
                    nEffectHit_ID = BattleMng.EffectID_HealNormal;
                    break;
            }


            pTempElement.AddBuffElement(Attacker, this, bOverlapMode, BuffIndex, Attacker.SkillLevel, -1, nEffectHit_ID, eSkill);

        }
    }


    public void AddBuff_Ignore(BattlePawn Attacker, SkillType eSkill, int IgnoreBuffIndex)
    {
        DB_Skill.Schema TempSkillData = null;
        SkillEffectData TempEffectData = null;

        bool bLeader = false;
        if (eSkill == SkillType.Leader)
            bLeader = true;

        TempSkillData = Attacker.SkillManager.GetDB_Skill(eSkill);
        TempEffectData = Attacker.SkillManager.GetSkillEffectData(eSkill);

        bool GoodBuff = false;
        bool bOverlapMode = false;
        if (TempSkillData.BuffNumber_1 != 0 && TempSkillData.BuffNumber_1 != IgnoreBuffIndex)
        {
            DB_Buff.Schema tempBuffData_1 = DB_Buff.Query(DB_Buff.Field.Index, TempSkillData.BuffNumber_1);

            BuffElement pTempElement_0 = null;
            if (bLeader || !tempBuffData_1.Overlap_Check)
            {
                pTempElement_0 = GetEmptyBuffElement();
            }
            else
            {
                pTempElement_0 = GetOverlapBuff(Attacker.GetBattlePawnKey(), TempSkillData.BuffNumber_1);
                if (pTempElement_0 == null)
                    pTempElement_0 = GetEmptyBuffElement();
                else
                    bOverlapMode = true;
            }

            pTempElement_0.AddBuffElement(Attacker, this, bOverlapMode, TempSkillData.BuffNumber_1, Attacker.SkillLevel, TempEffectData.EffID_Buff1_Active, TempEffectData.EffID_Buff1_Hit, eSkill, bLeader);

            if (tempBuffData_1.BUFF_GOOD)
                GoodBuff = true;
        }

        bOverlapMode = false;
        if (TempSkillData.BuffNumber_2 != 0 && TempSkillData.BuffNumber_2 != IgnoreBuffIndex)
        {
            DB_Buff.Schema tempBuffData_2 = DB_Buff.Query(DB_Buff.Field.Index, TempSkillData.BuffNumber_2);

            BuffElement pTempElement_1 = null;

            if (bLeader || !tempBuffData_2.Overlap_Check)
            {
                pTempElement_1 = GetEmptyBuffElement();
            }
            else
            {
                pTempElement_1 = GetOverlapBuff(Attacker.GetBattlePawnKey(), TempSkillData.BuffNumber_2);
                if (pTempElement_1 == null)
                    pTempElement_1 = GetEmptyBuffElement();
                else
                    bOverlapMode = true;
            }

            pTempElement_1.AddBuffElement(Attacker, this, bOverlapMode, TempSkillData.BuffNumber_2, Attacker.SkillLevel, TempEffectData.EffID_Buff2_Active, TempEffectData.EffID_Buff2_Hit, eSkill, bLeader);

            if (tempBuffData_2.BUFF_GOOD)
                GoodBuff = true;
        }

        if (bLeader && GoodBuff)
            BattleMng.pEffectPoolMng.SetBattleEffect(GetPawnPosition_Center() + Vector3.up * 1.5f, BattleMng.EffectID_UseSkill_Leader, 2.0f);
    }




    public void UpdateBuffList()
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            BuffList[idx].UpdateBuffElement();
        }
             

    }


    //버프 삭제.
    public void RemoveBuffElement(int RemoveBuffIdx)
    {
        if (RemoveBuffIdx == 0)
            return;

        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            if (BuffList[idx].Index == RemoveBuffIdx)
            {
                BuffList[idx].ReleaseBuffElement();
            }
        }
    }


    public void RemoveBuffElement(BUFF_KIND eKind)
    {
        if (AirborneMode)
            ReleaseAireborne();

        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            if (BuffList[idx].BuffBaseData.BUFF_KIND == eKind)
            {
                BuffList[idx].ReleaseBuffElement();
            }
        }
    }


    public void RemoveBadBuff()
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            if (BuffList[idx].BuffBaseData.BUFF_KIND == BUFF_KIND.BERSERKER_DP_DOWN)
                continue;

            if (BuffList[idx].BuffBaseData.BUFF_GOOD == false)
            {
                BuffList[idx].ReleaseBuffElement();
            }
        }
    }

    public void RemoveGoodBuff()
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            if (BuffList[idx].BuffBaseData.BUFF_KIND == BUFF_KIND.BERSERKER_DP_DOWN)
                continue;

            if (BuffList[idx].BuffBaseData.BUFF_GOOD == true)
            {
                BuffList[idx].ReleaseBuffElement();
            }
        }
    }



    public void RemoveBuff_Manual(BUFF_KIND eRemoveBuff)
    {
        for (int idx = 0; idx < BuffList.Count; idx++)
        {
            if (!BuffList[idx].BuffActive)
                continue;

            if (BuffList[idx].BuffBaseData.BUFF_KIND == eRemoveBuff)
                BuffList[idx].ReleaseBuffElement();
        }
    }






    ////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  기능 스크립트.
    //
    ////////////////////////////////////////////////////////////////////////////////////////////
    public Vector3 GetPawnPosition_Center()
    {
        return PawnTransform.position + new Vector3(0.0f, 0.5f * ScaleSize, 0.0f);
    }

    public Vector3 GetPawnPosition_Ground()
    {
        return PawnTransform.position + new Vector3(0.0f, 0.0f, 0.0f);
    }

    public Vector3 GetPawnThrowPosition()
    {
        return PosObj_ThrowBase.transform.position;
    }

    public Vector3 GetPawnHitPosition()
    {
        return PosObj_HitBase.transform.position + Vector3.up * (0.5f * ScaleSize);
    }


    public void SetEffectToPawn(int Effect_ID)
    {
        BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, Effect_ID);
    }

    public EffectPoolElement SetAttachEffectToPawn(int Effect_ID, bool LoopEffect, bool bGroundEffect = false, bool bHeadEffect = false, float EffectDir = 1.0f)
    {
        if (LoopEffect)
        {
            if (bGroundEffect)
                return BattleMng.pEffectPoolMng.SetBattleEffect_ForceLoop(transform, true, Effect_ID);
            else if (bHeadEffect)
                return BattleMng.pEffectPoolMng.SetBattleEffect_ForceLoop(PosObj_HeadUpEffect.transform, true, Effect_ID);
            else
                return BattleMng.pEffectPoolMng.SetBattleEffect_ForceLoop(PosObj_HitBase.transform, true, Effect_ID);
        }
        else
        {
            if (bGroundEffect)
                return BattleMng.pEffectPoolMng.SetBattleEffect_Follow(transform, true, Effect_ID, EffectDir);
            else if (bHeadEffect)
                return BattleMng.pEffectPoolMng.SetBattleEffect_Follow(PosObj_HeadUpEffect.transform, true, Effect_ID, EffectDir);
            else
                return BattleMng.pEffectPoolMng.SetBattleEffect_Follow(PosObj_HitBase.transform, true, Effect_ID,EffectDir);
        }
    }


    public void GetDamage_Normal(int DamageCount, bool Critical, int Effect_ID = -1)
    {
        if (SpecialActionMode)
            return;

        if (DamageCount <= 0)
        {
            BattleMng.pEffectPoolMng.SetMissCount(PosObj_HitBase.transform.position);
            return;
        }

        if (Pawn_Type == PAWN_TYPE.STUFF)
            DamageCount = 1;

        if (HeroTeam && BattleMng.HardTankingMode_Hero)
        {
            if (BattleMng.HardTankingPawn_Hero != this)
            {
                BattleMng.HardTankingPawn_Hero.GetDamage_Normal((int)((float)DamageCount * 0.5f), Critical, Effect_ID);

                //데미지반사.
                if (BattleMng.HardTankingPawn_Hero.ReflectDmgMode)
                    GetDamage_Reflect(DamageCount, BattleMng.HardTankingPawn_Hero.ReflectAddValue);
                return;
            }
        }

        if (!HeroTeam && BattleMng.HardTankingMode_Enemy)
        {
            if (BattleMng.HardTankingPawn_Enemy != this)
            {
                BattleMng.HardTankingPawn_Enemy.GetDamage_Normal((int)((float)DamageCount * 0.5f), Critical, Effect_ID);

                //데미지반사.
                if (BattleMng.HardTankingPawn_Enemy.ReflectDmgMode)
                    GetDamage_Reflect(DamageCount, BattleMng.HardTankingPawn_Enemy.ReflectAddValue);
                return;
            }
        }


        //치트.
        if (!BattleMng.SuperMode)
            CurHP -= DamageCount;

        if (BattleMng.OneKillMode_Hero && !HeroTeam)
            CurHP = 0;
        if (BattleMng.OneKillMode_Enemy && HeroTeam)
            CurHP = 0;
        //
        //        CurHP -= DamageCount;

        if(AirborneMode)
            RemoveBuffElement(BUFF_KIND.AIRBORN);

        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                DestroyStuff();
            }

        }
        else
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                SetMotion(PAWN_ANIMATION_KIND.DIE);
            }

            //스킬체크용.
            Check_HitCount++;
        }

        Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_0);

        BattleMng.pEffectPoolMng.SetDamageCount(PosObj_HitBase.transform.position, DamageCount, HeroTeam, Critical);

        if (Effect_ID != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, Effect_ID);
    }

    public void GetDotDamage(int BaseDmg, float BuffDmgValue, int EffectID)
    {
        if (SpecialActionMode)
            return;

        int DmgCount = (int)(BaseDmg * BuffDmgValue);

        if (Pawn_Type == PAWN_TYPE.STUFF)
            DmgCount = 1;

        //스킬쉴드.
        if (SkillShieldMode)
        {
            DmgCount = (int)((float)DmgCount * SkillShieldValue);
            RemoveBuffElement(BUFF_KIND.SKILL_SHIELD);
        }

        if (DmgCount <= 0)
            DmgCount = 1;

        CurHP -= DmgCount;

        if (AirborneMode)
            RemoveBuffElement(BUFF_KIND.AIRBORN);

        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                DestroyStuff();
            }
        }
        else
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                SetMotion(PAWN_ANIMATION_KIND.DIE);
            }
        }

//        Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_1);
        BattleMng.pEffectPoolMng.SetDamageCount_Dot(PosObj_HitBase.transform.position, DmgCount);

        if (EffectID != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, EffectID);

    }


    public void GetDamage_Skill(BattlePawn Attacker, SkillType eSkillType, int HitEff_ID = -1)
    {
        if (SpecialActionMode)
            return;

        if (Attacker == this)
            return;

        if (HeroTeam && BattleMng.HardTankingMode_Hero)
        {
            if (BattleMng.HardTankingPawn_Hero != this)
            {
                BattleMng.HardTankingPawn_Hero.GetDamage_Skill(Attacker, eSkillType, HitEff_ID);
                return;
            }
        }

        if (!HeroTeam && BattleMng.HardTankingMode_Enemy)
        {
            if (BattleMng.HardTankingPawn_Enemy != this)
            {
                BattleMng.HardTankingPawn_Enemy.GetDamage_Skill(Attacker, eSkillType, HitEff_ID);
                return;
            }
        }


        int DamageCount = 0;
        bool Critical = false;
        MakeBattleSkillDamage(Attacker, this, eSkillType, ref DamageCount, ref Critical);

        if (HardTankingMode)
            DamageCount = (int)((float)DamageCount * 0.5f);

        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            DamageCount = 1;
            Critical = false;
        }

        //스킬쉴드.
        if (SkillShieldMode)
        {
            DamageCount = (int)((float)DamageCount * SkillShieldValue);
            RemoveBuffElement(BUFF_KIND.SKILL_SHIELD);
        }

        if (DamageCount <= 0)
            DamageCount = 1;


        //치트.
        if (!BattleMng.SuperMode)
            CurHP -= DamageCount;

        if (BattleMng.OneKillMode_Hero && !HeroTeam)
            CurHP = 0;
        if (BattleMng.OneKillMode_Enemy && HeroTeam)
            CurHP = 0;

        if (AirborneMode)
            RemoveBuffElement(BUFF_KIND.AIRBORN);

//
//        CurHP -= DamageCount;

        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                DestroyStuff();
            }
        }
        else
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                SetMotion(PAWN_ANIMATION_KIND.DIE);
            }

            //스킬체크용.
            Check_HitCount++;
        }

        SetHitSound(Attacker.CardIndex, true);


        BattleMng.pEffectPoolMng.SetDamageCount(PosObj_HitBase.transform.position, DamageCount, HeroTeam, Critical);

        SetDelayHit();

        if (HitEff_ID != -1)
        {
            switch (Attacker.SkillManager.DB_ActiveSkill.Index)
            {
                case 34:        //그라운드 이펙트.
                case 46:
                    BattleMng.pEffectPoolMng.SetBattleEffect(transform.position, HitEff_ID);
                    break;

                default:
                    BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, HitEff_ID);
                    break;
            }

            BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);
        }



        //데미지반사.
        if (ReflectDmgMode)
        {
            Attacker.GetDamage_Reflect(DamageCount, ReflectAddValue);
        }
    }




    public void GetDamage_Skill_Manual(int Damage, SkillType eSkillType, int HitEff_ID = -1)
    {
        if (SpecialActionMode)
            return;

        if (HeroTeam && BattleMng.HardTankingMode_Hero)
        {
            if (BattleMng.HardTankingPawn_Hero != this)
            {
                BattleMng.HardTankingPawn_Hero.GetDamage_Skill_Manual(Damage, eSkillType, HitEff_ID);
                return;
            }
        }

        if (!HeroTeam && BattleMng.HardTankingMode_Enemy)
        {
            if (BattleMng.HardTankingPawn_Enemy != this)
            {
                BattleMng.HardTankingPawn_Enemy.GetDamage_Skill_Manual(Damage, eSkillType, HitEff_ID);
                return;
            }
        }


        int DamageCount = Damage;
        bool Critical = false;

        if (HardTankingMode)
            DamageCount = (int)((float)DamageCount * 0.5f);

        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            DamageCount = 1;
            Critical = false;
        }

        //스킬쉴드.
        if (SkillShieldMode)
        {
            DamageCount = (int)((float)DamageCount * SkillShieldValue);
            RemoveBuffElement(BUFF_KIND.SKILL_SHIELD);
        }

        if (DamageCount <= 0)
            DamageCount = 1;


        //치트.
        if (!BattleMng.SuperMode)
            CurHP -= DamageCount;

        if (BattleMng.OneKillMode_Hero && !HeroTeam)
            CurHP = 0;
        if (BattleMng.OneKillMode_Enemy && HeroTeam)
            CurHP = 0;

        if (AirborneMode)
            RemoveBuffElement(BUFF_KIND.AIRBORN);

        //
        //        CurHP -= DamageCount;

        if (Pawn_Type == PAWN_TYPE.STUFF)
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                DestroyStuff();
            }
        }
        else
        {
            if (CurHP <= 0)
            {
                CurHP = 0;
                SetMotion(PAWN_ANIMATION_KIND.DIE);
            }

            //스킬체크용.
            Check_HitCount++;
        }

        Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_0);
        BattleMng.pEffectPoolMng.SetDamageCount(PosObj_HitBase.transform.position, DamageCount, HeroTeam, Critical);
        SetDelayHit();

        BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, HitEff_ID);
        BattleMng.pCamShake.SetShake(CAM_SHAKE_PRESET.SHAKE_MIDDLE);
    }



    public void GetDamage_Reflect(float DamageValue, float ReflectValue)
    {
        if (DamageValue <= 0.0f)
            return;

        int nDmgCount = (int)(DamageValue * ReflectValue);
        CurHP -= nDmgCount;
        if (CurHP <= 0)
        {
            CurHP = 0;
            SetMotion(PAWN_ANIMATION_KIND.DIE);
        }

        Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_0);

        BattleMng.pEffectPoolMng.SetDamageCount_Reflect(PosObj_HitBase.transform.position, nDmgCount);

        if (BattleMng.EffectID_DamageNormal != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, BattleMng.EffectID_DamageNormal);

    }










    public void GetHeal_Normal(int HealCount, bool Critical, int Effect_ID = -1)
    {
        if (Pawn_Type == PAWN_TYPE.STUFF)
            return;

        CurHP += HealCount;
        if (CurHP >= MaxHP)
            CurHP = MaxHP;

        Kernel.soundManager.PlaySound(SOUND.SFX_HEAL);

        BattleMng.pEffectPoolMng.SetHealCount(PosObj_HitBase.transform.position, HealCount, HeroTeam, Critical);

        if (Effect_ID != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, Effect_ID);
    }

    public void GetHeal_SkillManual(int HealCount, SkillType eSkillType, int Effect_ID = -1)
    {
        if (Pawn_Type == PAWN_TYPE.STUFF)
            return;

        CurHP += HealCount;
        if (CurHP >= MaxHP)
            CurHP = MaxHP;

        Kernel.soundManager.PlaySound(SOUND.SFX_HEAL);

        BattleMng.pEffectPoolMng.SetHealCount(PosObj_HitBase.transform.position, HealCount, HeroTeam, false);

        if (Effect_ID != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, Effect_ID);
    }

    public void GetDotHeal(float HealValue, int EffectID)
    {
        if (Pawn_Type == PAWN_TYPE.STUFF)
            return;

        int HealCount = (int)(MaxHP * HealValue);
        CurHP += HealCount;
        if (CurHP >= MaxHP)
            CurHP = MaxHP;

        Kernel.soundManager.PlaySound(SOUND.SFX_HEAL);
        BattleMng.pEffectPoolMng.SetHealCount_Dot(PosObj_HitBase.transform.position, HealCount);

        if (EffectID != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, EffectID);

    }




    public void GetHeal_Skill(BattlePawn Healer, BattlePawn Target, SkillType eSkillType, int HitEff_ID = -1)
    {
        if (Pawn_Type == PAWN_TYPE.STUFF)
            return;

        int HealCount = 0;
        bool Critical = false;
        MakeBattleSkillHealCount(Healer, Target, eSkillType, ref HealCount, ref Critical);

        CurHP += HealCount;
        if (CurHP >= MaxHP)
            CurHP = MaxHP;

        Kernel.soundManager.PlaySound(SOUND.SFX_HEAL);

        BattleMng.pEffectPoolMng.SetHealCount(PosObj_HitBase.transform.position, HealCount, HeroTeam, Critical);

        if (HitEff_ID != -1)
            BattleMng.pEffectPoolMng.SetBattleEffect(PosObj_HitBase.transform.position, HitEff_ID);
    }



    public void MakeBattleDamage(BattlePawn Attacker, BattlePawn Defender, ref int Damage, ref bool Critical, bool NoCritical = false)
    {
//        (자신)총 체력 - [{상대방}총 공격력 x {상대방}총 공격력/({상대방}총 공격력 + {자신}총 방어력)] x {속성 상성 체크} x {치명타 발생 체크} x { 피해, 치유 체크}
        if (NoCritical)
            Critical = false;
        else
        {
            int CriticalRate = (int)(Attacker.CriticalRate * 100.0f);
            Critical = false;
            int RandomValue = Random.Range(0, 100);
            if (CriticalRate >= RandomValue)
                Critical = true;
        }

        float TempDamage = Attacker.CurAP * Attacker.CurAP / (Attacker.CurAP + Defender.CurDP);
        if (Critical)
            Damage = (int)(TempDamage * (1.0f + Attacker.CriticalDamage));
        else
            Damage = (int)TempDamage;


        if (IsEvadeDamage(Attacker, Defender) == true && Critical == false)
            Damage = 0;

    }



    public void MakeBattleSkillDamage(BattlePawn Attacker, BattlePawn Defender, SkillType eSkillType, ref int Damage, ref bool Critical)
    {
        //        (자신)총 체력 - [{상대방}총 공격력 x {상대방}총 공격력/({상대방}총 공격력 + {자신}총 방어력)] x {속성 상성 체크} x {치명타 발생 체크} x { 피해, 치유 체크}
        int CriticalRate = (int)(Attacker.CriticalRate * 100.0f);
        Critical = false;
        int RandomValue = Random.Range(0, 100);
        if (CriticalRate >= RandomValue)
            Critical = true;

        float TempDamage = Attacker.CurAP * Attacker.CurAP / (Attacker.CurAP + Defender.CurDP);
        TempDamage *= Attacker.GetSkillValue(eSkillType);

        if (Critical)
            Damage = (int)(TempDamage * (1.0f + Attacker.CriticalDamage));
        else
            Damage = (int)TempDamage;

        if (Damage <= 0)
            Damage = 1;
    }



    public void MakeBattleHealCount(BattlePawn Healer, BattlePawn Target, ref int HealCount, ref bool Critical)
    {
        //힐 공식 필요.
        int CriticalRate = (int)(Healer.CriticalRate * 100.0f);
        Critical = false;
        int RandomValue = Random.Range(0, 100);
        if (CriticalRate >= RandomValue)
            Critical = true;

        float TempHealValue = Healer.CurAP;
        if (Critical)
            HealCount = (int)(TempHealValue * (1.0f + Healer.CriticalDamage)); //1.5는 나중에 삭제.
        else
            HealCount = (int)TempHealValue;
    }

    public void MakeBattleSkillHealCount(BattlePawn Healer, BattlePawn Target, SkillType eSkillType, ref int HealCount, ref bool Critical)
    {
        //힐 공식 필요.
        int CriticalRate = (int)(Healer.CriticalRate * 100.0f);
        Critical = false;
        int RandomValue = Random.Range(0, 100);
        if (CriticalRate >= RandomValue)
            Critical = true;

        float TempHealValue = Healer.CurAP;

        TempHealValue *= Healer.GetSkillValue(eSkillType);

        if (Critical)
            HealCount = (int)(TempHealValue * (1.0f + Healer.CriticalDamage)); //1.5는 나중에 삭제.
        else
            HealCount = (int)TempHealValue;
    }


    public bool IsEvadeDamage(BattlePawn Attacker, BattlePawn Defender)
    {
            //회피율 계산.

        float CalHitRate = Attacker.Accurate - Defender.Evade;
        if (CalHitRate >= 0.95f)
            CalHitRate = 0.95f;
        if (CalHitRate <= 0.1f)
            CalHitRate = 0.1f;

        float RandomValue = Random.Range(0.0f, 1.0f);

        if (RandomValue > CalHitRate)
            return true;

        return false;
    }




    public void ShowGunFireEffect()
    {
        if (GunFireEffect == null)
            return;

        Invoke("ActiveGunFireEffect", 0.2f);
    }

    private void ActiveGunFireEffect()
    {
        //기존 삭제가 들어오면 지우고.
        CancelInvoke("HideGunFireEffect");
        CancelInvoke("ActiveGunFireEffect");

        //일단 껐다가 다시 그린다.
        HideGunFireEffect();

        GunFireEffect.transform.position = GetPawnThrowPosition();
        GunFireEffect.transform.localScale = new Vector3(MoveDirection, 1.0f, 1.0f);
        GunFireEffect.SetActive(true);

        Invoke("HideGunFireEffect", 1.0f);
    }

    private void HideGunFireEffect()
    {
        GunFireEffect.SetActive(false);
    }
    











    //폰 일시정지.
    public void SetPause(bool bPause)
    {
        if (bPause)
        {
            PawnSpine.timeScale = 0.0f;
            if (ThrowBattleBullet != null)
            {
                for (int idx = 0; idx < ThrowBattleBullet.Count; idx++)
                {
                    ThrowBattleBullet[idx].PauseBullet();
                }
            }
            SetAirborneAniPause(true);
        }
        else
        {
            PawnSpine.timeScale = AnimationSpeed;
            if (ThrowBattleBullet != null)
            {
                for (int idx = 0; idx < ThrowBattleBullet.Count; idx++)
                {
                    ThrowBattleBullet[idx].ResumeBullet();
                }
            }
            SetAirborneAniPause(false);
        }
    }


    public void SetAirborneAniPause(bool bPause)
    {
        if (bPause)
            MotionAnimation["aniPawnAirborne"].speed = 0.0f;
        else
            MotionAnimation["aniPawnAirborne"].speed = 1.0f;
    }



    //특정 길이만큼 강제이동 명령.
    public void SetMove_MoveRange(float fRange)
    {
        bRangeMoveMode = true;
        bRangeMovePositionMode = false;
        fRangeMove_Cur = 0.0f;
        fRangeMove_Max = fRange;
        SetMotion(PAWN_ANIMATION_KIND.MOVE, true);
    }

    public void SetMove_MovePosition(float Pos_X)
    {
        bRangeMoveMode = true;
        bRangeMovePositionMode = true;
        fRangeMove_PosX = Pos_X;
        SetMotion(PAWN_ANIMATION_KIND.MOVE, true);
    }


    //특정 길이만큼 강제이동중인가?
    public bool IsRangeMoveMode()
    {
        return bRangeMoveMode;
    }








    //넉백.
    public void SetKnockBack(BattlePawn pAttacker, KNOCKBACK_TYPE eKnockbackType)
    {
        if (ePawnAniState == PAWN_ANIMATION_KIND.WIN)
            return;

        if (SpecialActionMode)
            return;


        KnockbackMode = true;

        float fDir = -1.0f;
        if (!HeroTeam)
            fDir = 1.0f;

        float fPower = 0.0f;
        float fJumpPower = 0.0f;
        float fTempPower = 3.0f;

        ClassType eClassType;

        if (pAttacker != null)
        {
            fTempPower = 1.0f * (100.0f + pAttacker.PushRange) / (100.0f + PushedRange);
            eClassType = pAttacker.GetClassType();
        }
        else
            eClassType = ClassType.ClassType_Keeper;

        KnockbackKind = eKnockbackType;
        if (CurHP <= 0 || ePawnAniState == PAWN_ANIMATION_KIND.DIE)
            KnockbackKind = KNOCKBACK_TYPE.KNOCKBACK_DIE;

        switch (KnockbackKind)
        {
            case KNOCKBACK_TYPE.KNOCKBACK_SLIDE:
                if (GetClassType() == ClassType.ClassType_Keeper)
                    fTempPower *= 0.5f;

                fJumpPower = 0.0f;
                break;

            case KNOCKBACK_TYPE.KNOCKBACK_JUMP:
                if (GetClassType() == ClassType.ClassType_Keeper)    //내가 탱커면...
                {
                    fTempPower *= 2.0f;
                    fJumpPower = 1.5f;
                }
                else
                {
                    if (GetClassType() == ClassType.ClassType_Hitter)
                        fTempPower *= 2.0f;
                    fJumpPower = 3.0f;
                }
                break;

            case KNOCKBACK_TYPE.KNOCKBACK_THROW:
                fJumpPower = 0.0f;
                break;

            case KNOCKBACK_TYPE.KNOCKBACK_SKILL:
                if (GetClassType() == ClassType.ClassType_Keeper)    //내가 탱커면...
                {
                    fTempPower *= 5.0f;
                    fJumpPower = 3.5f;
                }
                else
                {
                    fTempPower *= 5.0f;
                    fJumpPower = 7.0f;
                }
                break;

            case KNOCKBACK_TYPE.KNOCKBACK_DIE:
                fPower = 5.0f;
                fJumpPower = 3.0f;
                break;
        }



        if(fJumpPower == 0.0f)
        {
            switch (GetClassType())
            {
                case ClassType.ClassType_Keeper:
                    switch (KnockbackKind)
                    {
                        case KNOCKBACK_TYPE.KNOCKBACK_SLIDE:
                            if (eClassType == ClassType.ClassType_Keeper)// || eClassType == ClassType.ClassType_Hitter)
                            {
                                SetMotion(PAWN_ANIMATION_KIND.HIT);
                            }
                            else
                            {
                                ChangeTextureColor();
                                ShakeMonster();
                            }
                            break;

                        case KNOCKBACK_TYPE.KNOCKBACK_THROW:
                            ChangeTextureColor();
                            ShakeMonster();
                            break;

                        default:
                            SetMotion(PAWN_ANIMATION_KIND.HIT);
                            break;
                    }
                    break;

                case ClassType.ClassType_Hitter:
                    switch (KnockbackKind)
                    {
                        case KNOCKBACK_TYPE.KNOCKBACK_SLIDE:
                            if (eClassType == ClassType.ClassType_Keeper || eClassType == ClassType.ClassType_Hitter)
                            {
                                SetMotion(PAWN_ANIMATION_KIND.ATTACK_PUSH);
                            }
                            else
                            {
                                ChangeTextureColor();
                                ShakeMonster();
                            }
                            break;

                        case KNOCKBACK_TYPE.KNOCKBACK_THROW:
                            if(ePawnAniState != PAWN_ANIMATION_KIND.KNOCKBACK)
                                SetMotion(PAWN_ANIMATION_KIND.ATTACK_PUSH);
                            ChangeTextureColor();
                            break;

                        default:
                            SetMotion(PAWN_ANIMATION_KIND.HIT);
                            break;
                    }
                    break;

                default:
                    SetMotion(PAWN_ANIMATION_KIND.HIT);
                    break;
            }

            if (GetClassType() == ClassType.ClassType_Keeper && KnockbackKind == KNOCKBACK_TYPE.KNOCKBACK_SLIDE)
            {
                if (eClassType == ClassType.ClassType_Keeper || eClassType == ClassType.ClassType_Hitter)
                {
                    SetMotion(PAWN_ANIMATION_KIND.HIT);
                }
                else
                {
                    ChangeTextureColor();
                    ShakeMonster();
                }
            }
            else if (GetClassType() == ClassType.ClassType_Hitter && KnockbackKind == KNOCKBACK_TYPE.KNOCKBACK_SLIDE)
            {
                if (eClassType == ClassType.ClassType_Keeper || eClassType == ClassType.ClassType_Hitter)
                {
                    SetMotion(PAWN_ANIMATION_KIND.ATTACK_PUSH);
                }
                else
                {
                    ChangeTextureColor();
                    ShakeMonster();
                }
            }
            else
                SetMotion(PAWN_ANIMATION_KIND.HIT);
        }
        else
        {
            SetMotion(PAWN_ANIMATION_KIND.KNOCKBACK);
            SetModule_Jump(fJumpPower);
        }

        //밀림.
        fPower = Settings.Util.GetRound(fTempPower, 2);
        SetModule_Push(fPower, fDir);
    }



    public void UpdateKnockback()
    {
        UpdateModule_Push();
        UpdateModule_Jump();

        if (KnockbackMode)
        {
            if (!bModule_Push && !bModule_Jump)
            {
                if (KnockbackKind == KNOCKBACK_TYPE.KNOCKBACK_DIE)
                {
                    KnockbackMode = false;
                    Invoke("DestroyPawn", 0.5f);
                    return;
                }



                if (IsDeath() == false)
                {
                    switch (ePawnAniState)
                    {
                        case PAWN_ANIMATION_KIND.ATTACK_PUSH:
                            break;

                        default:
                            SetMotion(PAWN_ANIMATION_KIND.IDLE);
                            break;
                    }
                }
                KnockbackMode = false;
            }
        }
    }


    public void DestroyPawn()
    {
        PawnSpine.gameObject.SetActive(false);
        ShadowObject.SetActive(false);

        if (Pawn_Type == PAWN_TYPE.SUMMONER)
        {
            DestroyDummyPawn();
            return;
        }
        else
            BattleMng.pEffectPoolMng.SetBattleEffect(transform.position, BattleMng.EffectID_Die);

        
        if(SkillManager != null)
        {
            //토템.
            if(SkillManager.SummonStuffObject != null)
            {
                if(SkillManager.SummonStuffObject.GetComponent<BattlePawn>().ActiveStuffObject)
                    SkillManager.SummonStuffObject.GetComponent<BattlePawn>().DestroyStuff();
            }
            if (SkillManager.SummonStuffObject_Max != null)
            {
                if (SkillManager.SummonStuffObject_Max.GetComponent<BattlePawn>().ActiveStuffObject)
                    SkillManager.SummonStuffObject_Max.GetComponent<BattlePawn>().DestroyStuff();
            }
        }

    }



    //에어본.
    public void SetAirborne(BattlePawn pAttacker)
    {
        if (ePawnAniState == PAWN_ANIMATION_KIND.WIN || ePawnAniState == PAWN_ANIMATION_KIND.DIE)
            return;

        SetMotion(PAWN_ANIMATION_KIND.AIRBORNE);

        MotionAnimation.Stop();
        MotionAnimation.Play("aniPawnAirborne");
    }

    public void ReleaseAireborne()
    {
        MotionAnimation.Stop();
        if (CurHP <= 0)
            SetMotion(PAWN_ANIMATION_KIND.DIE, true);
        else
            SetMotion(PAWN_ANIMATION_KIND.HIT, true);
    }








    public void SetDelayHit()
    {
        //죽었거나 승리모션중이면 리턴.
        if (ePawnAniState == PAWN_ANIMATION_KIND.WIN || ePawnAniState == PAWN_ANIMATION_KIND.DIE)
            return;

        //대기중이나 이동중일땐 히트모션.
        switch(ePawnAniState)
        {
            case PAWN_ANIMATION_KIND.IDLE:
            case PAWN_ANIMATION_KIND.MOVE:
            case PAWN_ANIMATION_KIND.MOVE_CHARGING:
            case PAWN_ANIMATION_KIND.MOVE_BACK:
            case PAWN_ANIMATION_KIND.ATTACK_WAIT:
            case PAWN_ANIMATION_KIND.ATTACK_KEEPER:
                if (GetClassType() == ClassType.ClassType_Keeper)
                    SetHitDelay(true);
                else
                    SetMotion(PAWN_ANIMATION_KIND.HIT, true);
                break;

            default:
                SetHitDelay(true);
                break;
        }

        ShakeMonster();
    }




    private bool    PushHitMode;
    public void SetPushHit(BattlePawn pAttacker)
    {
        if (ePawnAniState == PAWN_ANIMATION_KIND.WIN || ePawnAniState == PAWN_ANIMATION_KIND.DIE)
            return;

        SetMotion(PAWN_ANIMATION_KIND.HIT, true);
        SetHitDelay(true);
        ShakeMonster();

//        PushHitMode = true;
        fBreakPower = 0.0f;
        float fTempPower = 1.0f * (100.0f + pAttacker.PushRange) / (100.0f + PushedRange);

        SetModule_Push(fTempPower, LookDirection);
    }





    //밀어내기.
    public void SetModule_Push(float fPower, float fDir)
    {
        bModule_Push = true;
        fPush_Dir = fDir;
        fPush_CurPower = fPower * 3.0f;
        fPush_StopPower = 0.0f;
        fBreakPower = fPush_CurPower * 0.5f;
        Vector3 pBasePos = PawnTransform.position;
        pPush_NextPos = new Vector3(pBasePos.x + (fPush_Dir * fPush_CurPower), FirstPos_Y, pBasePos.z);
        fPush_BaseLength = BattleUtil.GetDistance_X(pBasePos.x, pPush_NextPos.x);
    }

    public void UpdateModule_Push()
    {
        if (!bModule_Push)
            return;

        //X축 이동.
        float fCalPos_X = PawnTransform.position.x + (fPush_CurPower * fPush_Dir * Time.deltaTime);

        float fLeftWallPos = BattleMng.pBattleFieldMng.WallPos_Left;
        if (fPush_Dir < 0 && fCalPos_X <= fLeftWallPos)
        {
            fCalPos_X = fLeftWallPos;
            fPush_CurPower = 0;
        }

        float fRightWallPos = BattleMng.pBattleFieldMng.WallPos_Right;
        if (BattleMng.CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            fRightWallPos = 5000.0f;
        }

        if (fPush_Dir > 0 && fCalPos_X >= fRightWallPos)
        {
            fCalPos_X = fRightWallPos;
            fPush_CurPower = 0;
        }

        //정의.
        PawnTransform.position = new Vector3(fCalPos_X, PawnTransform.position.y, PawnTransform.position.z);

        //X축 파워 가감.
        if (fPush_CurPower > 0.0f)
        {
            if (fJump_CurPower <= 0)
            {
                fPush_StopPower += (Time.deltaTime * fBreakPower);
                fPush_CurPower -= fPush_StopPower;

                if (fPush_CurPower <= 0.0f)
                {
                    fPush_CurPower = 0.0f;
                    bModule_Push = false;
                }

            }
        }
        else
        {
            fPush_CurPower = 0.0f;
            bModule_Push = false;
        }
    }





    //점프.
    public void SetModule_Jump(float fPower = 2.0f)
    {
        if (nJumpCount > 3 && bModule_Jump)
            return;

        nJumpCount++;
        bModule_Jump = true;
        fJump_CurPower = fPower;
    }


    public void UpdateModule_Jump()
    {
        //Y축 이동.
        float fCalPos_Y = 0.0f;
        if (bModule_Jump)
        {
            fCalPos_Y = transform.position.y + fJump_CurPower * (Time.deltaTime * fJump_Speed);
            fJump_CurPower -= fJump_Gravity * (Time.deltaTime * fJump_Speed);

            if (fCalPos_Y <= FirstPos_Y)
            {
                fCalPos_Y = FirstPos_Y;
                fJump_CurPower = 0.0f;
                bModule_Jump = false;
                nJumpCount = 0;
            }
        }
        else
        {
            fCalPos_Y = FirstPos_Y;
            nJumpCount = 0;
        }


        PawnTransform.position = new Vector3(PawnTransform.position.x, fCalPos_Y, PawnTransform.position.z);
    }





    //히트 딜레이.
    public void SetHitDelay(bool bDelay)
    {
        fCurTime_HitDelay = 0.0f;
        if (bDelay)
        {
            bWaitHitDelay = true;
            SetPause(true);
        }
        else
        {
            bWaitHitDelay = false;
            SetPause(false);
        }
    }




    //피격시 흔들기.
    private bool bShakeMonsterMode;
    private float nShakeMonsterTimeMS;
    private int nShakeCount;
    public void ShakeMonster()
    {
        bShakeMonsterMode = true;
        nShakeMonsterTimeMS = 0.0f;
        nShakeCount = 0;
    }

    public void UpdateShakeMonster()
    {
        if (!bShakeMonsterMode)
            return;

        if (nShakeCount > 4)
        {
            bShakeMonsterMode = false;
            nShakeCount = 0;
            PawnSpine.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else
        {
            nShakeMonsterTimeMS += Time.deltaTime;
            if (nShakeMonsterTimeMS >= 0.03f)
            {
                nShakeMonsterTimeMS = 0.0f;
                nShakeCount++;
                if (nShakeCount % 2 == 0)
                    PawnSpine.transform.localPosition = new Vector3(0.0f, 0.05f, 0.0f);
                else
                    PawnSpine.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
        }
    }








    //대열의 가장 앞으로 순간이동.
    public void SetTeleport(BattlePawn Attacker)
    {
        BattlePawn FrontPawn = Attacker.AI_Module.GetNearPawnData(false);

        transform.position = FrontPawn.transform.position;
    }



    public void ForceMove_X(float xPos)
    {
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
    }










    //피격시 색상변경.
    public void ChangeTextureColor()
    {
        bChangeColorMode = true;
        bChangeEnd = false;
        fCurTime_ColorChange = 0.0f;            //색상변경 시간.
    }


    //변경된 색상 정상화.
    void Reset_Color()
    {
        bChangeColorMode = false;
        fCurTime_ColorChange = 0.0f;
    }







    public void SetTargetLengthValue(float TargetPos_X)
    {
        TargetLengthValue = BattleUtil.GetDistance_X(transform.position.x, TargetPos_X);
    }













    //사운드.
    public void SetHitSound(int AttackerIndex, bool SkillSound)
    {
        if (SkillSound)
        {
            switch (AttackerIndex)
            {
                case 3:     //가루.
                case 30:    //밥루스.
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_HIT_PUNCH);
                    break;

                case 51:    //장뚱.
                case 57:    //주방장뚱.
                    Kernel.soundManager.PlaySound(SOUND.SFX_BICHEOLJO);
                    break;

                case 54:    //해머루스.
                    Kernel.soundManager.PlaySound(SOUND.SFX_SKILL_LIGHTNING);
                    Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_1);
                    break;

                default:
                    Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_0);
                    break;
            }
        }
        else
        {
            switch (AttackerIndex)
            {
                default:
                    Kernel.soundManager.PlaySound(SOUND.SFX_DAMAGE_0);
                    break;
            }
        }
    }






    public void AddLoopSoundClip()
    {
        LoopSoundClip = null;

        switch (CardIndex)
        {
            case 1:     //뿌까.
            case 3:     //가루.
            case 27:    //아뵤.
            case 41:    //작열이.
            case 42:    //파이야.
            case 45:    //돌격스미스.
            case 53:    //또베.
            case 54:    //해머루스.
            case 56:    //라이더뿌까.
                LoopSoundClip = Kernel.soundManager.GetAudioClip(SOUND.SFX_SKILL_CAST_RUN);
                break;
        }
    }

    public void AddLoopSoundClip_Stuff(int StuffIndex)
    {
        LoopSoundClip = null;

        switch (StuffIndex)
        {
            case 24:    //수노인 토템.
                LoopSoundClip = Kernel.soundManager.GetAudioClip(SOUND.SFX_SKILL_CAST_AURA);
                break;
        }
    }


    public void ActiveLoopSound()
    {
        if (Kernel.soundManager.SFX_On == false)
            return;

        if (LoopSoundClip == null)
            return;

        PawnAudioSource.loop = true;
        PawnAudioSource.clip = LoopSoundClip;
        PawnAudioSource.Play();
    }

    public void StopLoopSound()
    {
        if (Kernel.soundManager.SFX_On == false)
            return;

        if (LoopSoundClip == null)
            return;

        PawnAudioSource.Stop();
        PawnAudioSource.loop = false;
        PawnAudioSource.clip = null;
    }

}
