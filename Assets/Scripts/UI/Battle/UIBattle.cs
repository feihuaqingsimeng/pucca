using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class UIBattle : UIObject
{
    private BattleManager   BattleMng;

    public  Transform       HPGaugeParent;

    public  UseSkillPanel   UseSkillPanel_Hero;
    public  UseSkillPanel   UseSkillPanel_Enemy;

    public  Transform       BuffInfoList_Hero;
    public  Transform       BuffInfoList_Enemy;


    //공용.
    public  Text            BattleMsg;

    public  Text            BattleTimeText;

    //PVP용 서브 UI.
    public  UISubBattle_PVP SubUI_PVP;

    //PVE용 서브 UI.
    public  UISubBattle_PVE SubUI_PVE;


    //배틀카드.
    public  BattleCard[]    BattleCardList;






    //콤보.
    public  GameObject          ComboPointPrefab;
    public  Transform           ComboParent_Hero;
    public  Transform           ComboParent_Enemy;
    public  GameObject          ComboMax_Hero;
    public  GameObject          ComboMax_Hero_Effect;
    public  GameObject          ComboMax_Enemy;
    [HideInInspector]
    public  ComboPointInfo[]    ComboBubbleObj_Hero;
    [HideInInspector]
    public  ComboPointInfo[]    ComboBubbleObj_Enemy;




    //경고알람.
    public  GameObject          WaveAlert_Normal;
    private Animation           AniWaveAlert_Normal;
    public  GameObject          WaveAlert_Boss;
    private Animation           AniWaveAlert_Boss;



    //버프정보.
    private bool                BuffInfoPanel_ShowMode;
    private bool                BuffInfoPanel_ScreenMode;
    public  GameObject          BuffInfoPanel;
    public  GameObject          BuffInfoPannel_ENG;
    public  Button              ShowBuffInfoButton;


    public  Button              BattleExitButton;


    public  Button              AutoPlayButton;
    public  GameObject          AutoPlayImage_On;
    public  GameObject          AutoPlayImage_Off;



    protected override void Awake()
    {
        base.Awake();

        BattleMng = GameObject.Find("BattleManager").GetComponent<BattleManager>();

        ComboBubbleObj_Hero = new ComboPointInfo[10];
        ComboBubbleObj_Enemy = new ComboPointInfo[10];
        for (int idx = 0; idx < 20; idx++)
        {
            GameObject StarPoint = Instantiate(ComboPointPrefab) as GameObject;

            if (idx < 10)
            {
                StarPoint.transform.parent = ComboParent_Hero;
                ComboBubbleObj_Hero[idx] = StarPoint.GetComponent<ComboPointInfo>();
            }
            else
            {
                StarPoint.transform.parent = ComboParent_Enemy;
                ComboBubbleObj_Enemy[idx - 10] = StarPoint.GetComponent<ComboPointInfo>();
            }
            StarPoint.transform.localPosition = Vector3.zero;
            StarPoint.transform.localScale = Vector3.one;
        }


        AniWaveAlert_Normal = WaveAlert_Normal.transform.FindChild("AlertMark").GetComponent<Animation>();
        WaveAlert_Normal.SetActive(false);
        AniWaveAlert_Boss = WaveAlert_Boss.transform.FindChild("AlertMark").GetComponent<Animation>();
        WaveAlert_Boss.SetActive(false);


        //버프정보.
        ShowBuffInfoButton.onClick.AddListener(ShowBuffInfoPanel);
        BuffInfoPanel.SetActive(false);
        BuffInfoPanel_ShowMode = false;
        BuffInfoPanel_ScreenMode = false;


        BattleExitButton.onClick.AddListener(BattleMng.OnButton_BattleExit);

        if (Kernel.entry.battle.CurBattleKind == BATTLE_KIND.PVE_BATTLE)
        {
            AutoPlayButton.gameObject.SetActive(true);
            AutoPlayButton.onClick.AddListener(SetAutoPlayButton);

            InitAutoPlayButton();
        }
        else
        {
            AutoPlayButton.gameObject.SetActive(false);
        }

        if (Kernel.languageCode != LanguageCode.Korean)
        {
            BuffInfoPanel = BuffInfoPannel_ENG;
        }
    }

    // Max Level 표기
    private void SetLevelMax(Text levelText, int maxLevel, int level)
    {
        TextLevelMaxEffect m_LevelMaxEffect = null;

        if (levelText != null)
            m_LevelMaxEffect = levelText.GetComponent<TextLevelMaxEffect>();

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.MaxValue = maxLevel;

        if (m_LevelMaxEffect != null)
            m_LevelMaxEffect.Value = level;
    }

    // Name And Level Text Center
    private void SetTextCenter(Text levelText, Text nameText, RectTransform parentRect)
    {
        levelText.rectTransform.sizeDelta = new Vector2(levelText.preferredWidth, levelText.preferredHeight);
        nameText.rectTransform.sizeDelta = new Vector2(nameText.preferredWidth, nameText.preferredHeight);

        float parentSizeX = levelText.rectTransform.sizeDelta.x + nameText.rectTransform.anchoredPosition.x + nameText.rectTransform.sizeDelta.x;
        parentRect.sizeDelta = new Vector2(parentSizeX, parentRect.sizeDelta.y);
        parentRect.anchoredPosition = new Vector2(-(float)(parentSizeX * 0.5f), parentRect.anchoredPosition.y);
    }

    public void SetBattleUI()
    {
        SubUI_PVP.gameObject.SetActive(true);
        SubUI_PVE.gameObject.SetActive(false);

        if (BattleMng.CurBattleKind == BATTLE_KIND.TEST_BATTLE)
            return;

        //유저정보 받아와서 세팅.
        SubUI_PVP.Hero_Name.text = string.Format("{0:S}", Kernel.entry.account.name);
        SubUI_PVP.Hero_Level.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), Kernel.entry.account.level);
        SubUI_PVP.Hero_GuildName.text = Kernel.entry.account.guildName;
        SubUI_PVP.Hero_RankPoint.text = Languages.GetNumberComma(Kernel.entry.account.rankingPoint);

        SetLevelMax(SubUI_PVP.Hero_Level, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), Kernel.entry.account.level);
        SetTextCenter(SubUI_PVP.Hero_Level, SubUI_PVP.Hero_Name, SubUI_PVP.Hero_NameLevelParent);

        //** ColorMaxCheck
        bool maxLevelH = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit) <= Kernel.entry.account.level;

        SubUI_PVP.Hero_GuildFlag.SetGuildEmblem(Kernel.entry.account.guildEmblem);

        //적정보.
        SubUI_PVP.Enemy_Name.text = string.Format("{0:S}", Kernel.entry.battle.PVP_User.m_sUserName);
        SubUI_PVP.Enemy_Level.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), (int)Kernel.entry.battle.PVP_User.m_byLevel);
        SubUI_PVP.Enemy_GuildName.text = Kernel.entry.battle.PVP_User.m_sGuildName;
        SubUI_PVP.Enemy_RankPoint.text = Languages.GetNumberComma(Kernel.entry.battle.PVP_User.m_iRankingPoint);

        SetLevelMax(SubUI_PVP.Enemy_Level, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), (int)Kernel.entry.battle.PVP_User.m_byLevel);
        SetTextCenter(SubUI_PVP.Enemy_Level, SubUI_PVP.Enemy_Name, SubUI_PVP.Enemy_NameLevelParent);

        //** ColorMaxCheck
        bool maxLevelE = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit) <= Kernel.entry.battle.PVP_User.m_byLevel;

        SubUI_PVP.Enemy_GuildFlag.gameObject.SetActive(true);
        SubUI_PVP.Enemy_GuildFlag.SetGuildEmblem(Kernel.entry.battle.PVP_User.m_sGuildEmble);
        //SubUI_PVP.Enemy_GuildFlag.flagImage.sprite = TextureManager.GetSprite(SpritePackingTag.Guild, "ui_img_guild_blue");
    }





    public void SetBattleUI_PVE()
    {
        SubUI_PVP.gameObject.SetActive(false);
        SubUI_PVE.gameObject.SetActive(true);

        //유저정보 받아와서 세팅.
        SubUI_PVE.Hero_Name.text = BattleMng.AreaName;

        //웨이브 표시기.
        SubUI_PVE.InitSubBattleUI_PVE(BattleMng, BattleMng.PVE_BossCardIndex);
    }






    public void SetRevengeBattleUI()
    {
        SubUI_PVP.gameObject.SetActive(true);
        SubUI_PVE.gameObject.SetActive(false);

        if (BattleMng.CurBattleKind == BATTLE_KIND.TEST_BATTLE)
            return;

        //유저정보 받아와서 세팅.
        SubUI_PVP.Hero_Name.text = string.Format("{0:S}", Kernel.entry.account.name);
        SubUI_PVP.Hero_Level.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), Kernel.entry.account.level);
        SubUI_PVP.Hero_GuildName.text = Kernel.entry.account.guildName;
        SubUI_PVP.Hero_RankPoint.text = Languages.GetNumberComma(Kernel.entry.account.rankingPoint);

        SubUI_PVP.Hero_GuildFlag.SetGuildEmblem(Kernel.entry.account.guildEmblem);

        SetLevelMax(SubUI_PVP.Hero_Level, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), Kernel.entry.account.level);
        SetTextCenter(SubUI_PVP.Hero_Level, SubUI_PVP.Hero_Name, SubUI_PVP.Hero_NameLevelParent);

        //적정보.
        SubUI_PVP.Enemy_Name.text = string.Format("{0:S}", Kernel.entry.battle.RevengeMatchInfoData.m_sUserName);
        SubUI_PVP.Enemy_Level.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), (int)Kernel.entry.battle.RevengeMatchInfoData.m_byLevel);

        BattleLog EnmeyLog = null;
        BattleLogUtility.TryGetBattleLog(Kernel.entry.battle.RevengeMatchInfoData.m_sDeckInfo, out EnmeyLog);

        SubUI_PVP.Enemy_GuildName.text = EnmeyLog.GuildName;
        SubUI_PVP.Enemy_RankPoint.text = Languages.GetNumberComma(EnmeyLog.RankPoint);

        SubUI_PVP.Enemy_GuildFlag.gameObject.SetActive(true);
        SubUI_PVP.Enemy_GuildFlag.SetGuildEmblem(EnmeyLog.GuildEmblem);
        //SubUI_PVP.Enemy_GuildFlag.flagImage.sprite = TextureManager.GetSprite(SpritePackingTag.Guild, "ui_img_guild_blue");

        SetLevelMax(SubUI_PVP.Enemy_Level, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), (int)Kernel.entry.battle.RevengeMatchInfoData.m_byLevel);
        SetTextCenter(SubUI_PVP.Enemy_Level, SubUI_PVP.Enemy_Name, SubUI_PVP.Enemy_NameLevelParent);
    }



    protected override void Update ()
    {
        CheckBuffInfoPanel();

        //전투시간.
        int NowTime = (int)(BattleMng.MaxBattleTime - BattleMng.CurBattleTime);

        int leftTime_M = NowTime / 60;
        int leftTime_S = NowTime % 60;

        BattleTimeText.text = string.Format("{0:00}:{1:00}", leftTime_M, leftTime_S);

        for(int idx = 0; idx < ComboBubbleObj_Hero.Length; idx++)
        {
            ComboBubbleObj_Hero[idx].ShowComboPoint(BattleMng.CurComboCount_H > idx);
        }
        if (BattleMng.CurComboCount_H == BattleMng.MaxComboCount)
        {
            ComboMax_Hero.SetActive(true);

            if (Kernel.entry.tutorial.TutorialActive)
            {
                if (Kernel.entry.tutorial.GroupNumber == 10 && Kernel.entry.tutorial.CurIndex < 17)
                    ComboMax_Hero_Effect.SetActive(false);
                else
                    ComboMax_Hero_Effect.SetActive(true);
            }
            else
                ComboMax_Hero_Effect.SetActive(true);

        }
        else
            ComboMax_Hero.SetActive(false);


        for (int idx = 0; idx < ComboBubbleObj_Enemy.Length; idx++)
        {
            ComboBubbleObj_Enemy[idx].ShowComboPoint(BattleMng.CurComboCount_E > idx);
        }
        if (BattleMng.CurComboCount_E == BattleMng.MaxComboCount)
            ComboMax_Enemy.SetActive(true);
        else
            ComboMax_Enemy.SetActive(false);



        //웨이브 알람 끄기.
        if(WaveAlert_Normal.activeInHierarchy)
        {
            if(AniWaveAlert_Normal.isPlaying == false)
                WaveAlert_Normal.SetActive(false);
        }

        if(WaveAlert_Boss.activeInHierarchy)
        {
            if(AniWaveAlert_Boss.isPlaying == false)
                WaveAlert_Boss.SetActive(false);
        }
	}




    public void ShowBattleMsg(string szMsg)
    {
        BattleMsg.GetComponent<FadeUguiText>().SetFadeText(szMsg, 1.0f);
    }







    //버프정보패널.
    public void ShowBuffInfoPanel()
    {
        if (!BuffInfoPanel_ShowMode)
        {
            if (BuffInfoPanel_ScreenMode)
            {
                BuffInfoPanel_ScreenMode = false;
                return;
            }
            else
            {
                BuffInfoPanel.SetActive(true);
                BuffInfoPanel_ShowMode = true;
                BuffInfoPanel_ScreenMode = false;

                BattleMng.BattlePause();
            }
        }
    }


    public void CheckBuffInfoPanel()
    {
        if (!BuffInfoPanel_ShowMode)
            return;

        bool m_Clicked = false;
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            m_Clicked = (t.phase == TouchPhase.Began);
        }
        else
        {
            m_Clicked = Input.GetMouseButtonDown(0);
        }

        if (m_Clicked)
        {
            BuffInfoPanel.SetActive(false);
            BuffInfoPanel_ShowMode = false;
            BuffInfoPanel_ScreenMode = true;

            BattleMng.BattleResume();

        }

    }





    public void InitAutoPlayButton()
    {
        if (Kernel.entry.battle.AutoPlayBattle)
        {
            AutoPlayImage_On.SetActive(true);
            AutoPlayImage_Off.SetActive(false);
        }
        else
        {
            AutoPlayImage_On.SetActive(false);
            AutoPlayImage_Off.SetActive(true);
        }

    }




    public void SetAutoPlayButton()
    {
        Kernel.entry.battle.AutoPlayBattle = !Kernel.entry.battle.AutoPlayBattle;

        if (Kernel.entry.battle.AutoPlayBattle)
        {
            AutoPlayImage_On.SetActive(true);
            AutoPlayImage_Off.SetActive(false);
        }
        else
        {
            AutoPlayImage_On.SetActive(false);
            AutoPlayImage_Off.SetActive(true);
        }

    }








}
