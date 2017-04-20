using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBattleStart : UIObject
{
    private CanvasGroup     pCanvasGroup;
    public  Text            BattleFieldName;
    
    //아군.
    public  RectTransform   UserNameAndLevelParent_Hero;
    public  Text            UserName_Hero;
    public  Text            UserLevel_Hero;

    public  GameObject      GuildInfoObject_Hero;
    public  UIGuildFlag     GuildFlag_Hero;
    public  Text            GuildName_Hero;

    public  Text            RankPoint_Hero;

    //적군.
    public  RectTransform   UserNameAndLevelParent_Enemy;
    public  Text            UserName_Enemy;
    public  Text            UserLevel_Enemy;

    public  GameObject      GuildInfoObject_Enemy;
    public  UIGuildFlag     GuildFlag_Enemy;
    public  Text            GuildName_Enemy;

    public  Text            RankPoint_Enemy;



    //업데이트 데이터.
    [HideInInspector]
    public  bool            ShowEventMode;
    private float           WaitHideDelayTime;
    private float           CurWaitHideDelay;
    private bool            AlphaHideMode;
    private float           AlphaHideSpeed;


    protected override void Awake()
    {
        base.Awake();

        pCanvasGroup = gameObject.GetComponent<CanvasGroup>();
        pCanvasGroup.alpha = 0.0f;
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

    public void ShowBattleStartUI(BattleManager pBattleMng)
    {
        switch (pBattleMng.CurBattleKind)
        {
            case BATTLE_KIND.PVP_BATTLE:
                //유저정보 받아와서 세팅.
                UserLevel_Hero.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), Kernel.entry.account.level);
                UserName_Hero.text = string.Format("{0:S}", Kernel.entry.account.name);
                SetLevelMax(UserLevel_Hero, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), Kernel.entry.account.level);
                SetTextCenter(UserLevel_Hero, UserName_Hero, UserNameAndLevelParent_Hero); 

                GuildFlag_Hero.SetGuildEmblem(Kernel.entry.account.guildEmblem);
                GuildName_Hero.text = Kernel.entry.account.guildName;

                RankPoint_Hero.text = Languages.GetNumberComma(Kernel.entry.account.rankingPoint);

                //적정보.
                UserLevel_Enemy.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), (int)Kernel.entry.battle.PVP_User.m_byLevel);
                UserName_Enemy.text = string.Format("{0:S}", Kernel.entry.battle.PVP_User.m_sUserName);
                SetLevelMax(UserLevel_Enemy, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), (int)Kernel.entry.battle.PVP_User.m_byLevel);
                SetTextCenter(UserLevel_Enemy, UserName_Enemy, UserNameAndLevelParent_Enemy); 

                //** ColorMaxCheck
                bool maxLevelEnemy = Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit) <= Kernel.entry.battle.PVP_User.m_byLevel;

                GuildInfoObject_Enemy.SetActive(true);
                GuildFlag_Enemy.SetGuildEmblem(Kernel.entry.battle.PVP_User.m_sGuildEmble);
                GuildName_Enemy.text = string.IsNullOrEmpty(Kernel.entry.battle.PVP_User.m_sGuildName) ? Languages.ToString(TEXT_UI.GUILD_NONE) : Kernel.entry.battle.PVP_User.m_sGuildName;
                //GuildFlag_Enemy.flagImage.sprite = TextureManager.GetSprite(SpritePackingTag.Guild, "ui_img_guild_blue");

                RankPoint_Enemy.text = Languages.GetNumberComma(Kernel.entry.battle.PVP_User.m_iRankingPoint);
                break;

            case BATTLE_KIND.PVE_BATTLE:
                //유저정보 받아와서 세팅.
                
                UserName_Hero.text = string.Format("{0:S}", Kernel.entry.account.name);

                //임시로...
                GuildInfoObject_Hero.gameObject.SetActive(false);
                RankPoint_Hero.transform.parent.gameObject.SetActive(false);

                //적정보.
                UserName_Enemy.text = string.Format("{0:S}", pBattleMng.EnemyTeamName);

                //임시로...
                GuildInfoObject_Enemy.gameObject.SetActive(false);
                RankPoint_Enemy.transform.parent.gameObject.SetActive(false);
                break;

            case BATTLE_KIND.REVENGE_BATTLE:
                //유저정보 받아와서 세팅.
                UserLevel_Hero.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), Kernel.entry.account.level);
                UserName_Hero.text = string.Format("{0:S}", Kernel.entry.account.name);
                SetLevelMax(UserLevel_Hero, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), Kernel.entry.account.level);
                SetTextCenter(UserLevel_Hero, UserName_Hero, UserNameAndLevelParent_Hero); 

                if (Kernel.entry.account.guildEmblem == string.Empty)
                {
                    GuildInfoObject_Hero.SetActive(false);
                }
                else
                {
                    GuildInfoObject_Hero.SetActive(true);
                    GuildFlag_Hero.SetGuildEmblem(Kernel.entry.account.guildEmblem);
                    GuildName_Hero.text = Kernel.entry.account.guildName;
                }
                RankPoint_Hero.text = Languages.GetNumberComma(Kernel.entry.account.rankingPoint);


                //적정보.
                UserLevel_Enemy.text = string.Format("{0}{1:D}", Languages.ToString(TEXT_UI.LV), (int)Kernel.entry.battle.RevengeMatchInfoData.m_byLevel);
                UserName_Enemy.text = string.Format("{0:S}", Kernel.entry.battle.RevengeMatchInfoData.m_sUserName);
                SetLevelMax(UserLevel_Enemy, Kernel.entry.data.GetValue<byte>(Const_IndexID.Const_Account_Level_Limit), (int)Kernel.entry.battle.RevengeMatchInfoData.m_byLevel);
                SetTextCenter(UserLevel_Enemy, UserName_Enemy, UserNameAndLevelParent_Enemy); 


                GuildInfoObject_Enemy.SetActive(false);
                RankPoint_Enemy.gameObject.SetActive(false);
                break;

            case BATTLE_KIND.TEST_BATTLE:
                return;
        }

        BattleFieldName.text = pBattleMng.AreaName;



        //이벤트 시작.
        ShowEventMode = true;
        AlphaHideMode = false;
        pCanvasGroup.alpha = 1.0f;
        WaitHideDelayTime = 3.0f;
        CurWaitHideDelay = 0.0f;
        AlphaHideSpeed = 5.0f;
    }


    protected override void Update()
    {
        if(!ShowEventMode)
            return;

        if (AlphaHideMode)
        {
            pCanvasGroup.alpha -= AlphaHideSpeed * Time.deltaTime;
            if (pCanvasGroup.alpha <= 0.0f)
            {
                pCanvasGroup.alpha = 0.0f;
                ShowEventMode = false;
                AlphaHideMode = false;
            }
        }
        else
        {
            CurWaitHideDelay += Time.deltaTime;
            if (CurWaitHideDelay >= WaitHideDelayTime)
            {
                CurWaitHideDelay = 0.0f;
                AlphaHideMode = true;
            }
        }
    }








}
