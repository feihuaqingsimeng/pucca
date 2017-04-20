using Common.Packet;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UILobby : UIObject
{
    //Promotion
    public RectTransform[] m_trPanel; //[0] : Left, [1] : Bottom, [2] : Bottom_2(Chest) [3] : Right 

    public Image m_BackgroundImage;
    public  Transform   m_BackgroundAddOn_Parent;
    private GameObject  m_BackgroundAddOn_Object;

    public List<UILobbyCharacterSummary> m_CharacterSummaryList;
    public List<UIChest> m_ChestList;
    public Button m_MailButton;
    public Button m_DeckButton;
    public Button m_ShopButton;
    public Button m_AdventureButton;
    public Button m_MatchButton;
    public Text m_MatchButtonHeartText;
    public Button m_RankingButton;
    public Button m_TreasureButton;
    public Button m_RevengeButton;
    public Button m_FranchiseButton;
    public Button m_DetectButton;
    public Button m_GuildButton;
    public Button m_NoticeButton;

    public Image m_PostNewButton;
    public Image m_FranchiseNewButton;

    public GameObject m_ChestUIObject;

    protected override void Awake()
    {
        base.Awake();

        m_MailButton.onClick.AddListener(OnMailButtonClicked);
        m_DeckButton.onClick.AddListener(OnDeckButtonClicked);
        m_ShopButton.onClick.AddListener(OnShopButtonClicked);
        m_AdventureButton.onClick.AddListener(OnAdventureButtonClicked);
        m_MatchButton.onClick.AddListener(OnMatchButtonClicked);
        m_RankingButton.onClick.AddListener(OnRankingButtonClicked);
        m_TreasureButton.onClick.AddListener(OnTreasureButtonClicked);
        m_RevengeButton.onClick.AddListener(OnRevengeButtonClick);
        m_FranchiseButton.onClick.AddListener(OnFranchiseButtonClick);
        m_DetectButton.onClick.AddListener(OnDetectButtonClick);
        m_GuildButton.onClick.AddListener(OnGuildButtonClick);
        m_NoticeButton.onClick.AddListener(OnNoticeButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestListUpdate += OnChestListUpdate;
            Kernel.entry.treasure.onActiveTreasureBoxCallback += OnActiveTreasureButton;
            Kernel.entry.post.onActivePostCallback += OnActivePostNewButton;
            Kernel.entry.post.PostInit();
            Kernel.entry.post.LinkUpdate(true);
            Kernel.entry.franchise.onActiveFranchiseRewardCallback += OnActiveFranchiseNewButton;

            Renewal();
            OnCheckPromotion();

            Kernel.entry.tutorial.onUpdateTutorialUI += SetLobby_TutorialState;
            Kernel.entry.tutorial.onResetLobbyUI += ResetLobbyUI;

/*
            if (Kernel.entry.tutorial.GroupNumber != 10000)
            {
                Kernel.entry.tutorial.REQ_PACKET_CG_GAME_COMPLETE_TUTORIAL_SYN(10000);
                Kernel.entry.tutorial.GroupNumber = 10000;
                Kernel.entry.tutorial.TutorialActive = false;
            }
 */
            if (Kernel.entry.tutorial.TutorialActive)
            {
                switch (Kernel.entry.tutorial.GroupNumber)
                {
                    case 10:
                        if (Kernel.entry.tutorial.WaitSeq == 106)
                            Kernel.entry.tutorial.onSetNextTutorial();
                        break;

                    case 30:
                        if (Kernel.entry.tutorial.WaitSeq == 305)
                            Kernel.entry.tutorial.onSetNextTutorial();
                        break;

                    case 40:
                        if (Kernel.entry.tutorial.WaitSeq == 404)
                            Kernel.entry.tutorial.onSetNextTutorial();
                        break;

                    case 60:
                        if (Kernel.entry.tutorial.WaitSeq == 601)
                            Kernel.entry.tutorial.onSetNextTutorial();
                        break;
                }
            }
            else
            {
                if (Kernel.entry.account.TutorialGroup != 0 && Kernel.entry.account.TutorialGroup != 10000)
                {
                    DB_TutorialGroup.Schema TutorialGroupData = DB_TutorialGroup.Query(DB_TutorialGroup.Field.Index, Kernel.entry.account.TutorialGroup);
                    if (Kernel.entry.account.level >= TutorialGroupData.ActiveLevel)
                    {
                        if (Kernel.entry.tutorial.onStartTutorial == null)
                            Kernel.uiManager.Open(UI.Tutorial);
                        else
                            Kernel.entry.tutorial.onStartTutorial(Kernel.entry.account.TutorialGroup);
                    }
                }
            }

            SetLobby_TutorialState();
  
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.entry != null)
        {
            Kernel.entry.chest.onChestListUpdate -= OnChestListUpdate;
            Kernel.entry.treasure.onActiveTreasureBoxCallback -= OnActiveTreasureButton;
            Kernel.entry.post.onActivePostCallback -= OnActivePostNewButton;
            Kernel.entry.post.LinkUpdate(false);
            Kernel.entry.franchise.onActiveFranchiseRewardCallback -= OnActiveFranchiseNewButton;

            Kernel.entry.tutorial.onUpdateTutorialUI -= SetLobby_TutorialState;
            Kernel.entry.tutorial.onResetLobbyUI -= ResetLobbyUI;

        }
    }

    private void OnCheckPromotion()
    {
        if (Kernel.entry.battle.fluctuationPvpArea == 0)
        {
            int FieldNumber = Kernel.entry.account.currentPvPArea - 1;
            m_BackgroundImage.sprite = TextureManager.GetSprite(SpritePackingTag.Lobby, string.Format("img_Lobby_Background_{0}", FieldNumber));
            m_BackgroundAddOn_Object = Instantiate(Resources.Load("Prefabs/UI/Lobby/LobbyAddOn_" + FieldNumber.ToString())) as GameObject;
            m_BackgroundAddOn_Object.transform.SetParent(m_BackgroundAddOn_Parent);

            if (FieldNumber == 3)    //해적선 위일때 배경 바다를 움직이기위해 애니메이션 재생.
                m_BackgroundImage.GetComponent<Animation>().Play();
            else
                m_BackgroundImage.GetComponent<Animation>().Stop();
            return;
        }

        //** Promotion Start
        UIPromotion promotion = (UIPromotion)Kernel.uiManager.Open(UI.Promotion);
        promotion.SetInit(m_trPanel[0], m_trPanel[1], m_trPanel[2], m_trPanel[3], m_BackgroundImage, m_BackgroundAddOn_Parent, m_BackgroundAddOn_Object);
        promotion.StartAct();
    }

    void OnChestListUpdate(List<CRewardBox> rewardBoxList)
    {
        if (rewardBoxList != null)
        {
            for (int i = 0; i < rewardBoxList.Count; i++)
            {
                // Warning, NullRefExcpt!
                m_ChestList[rewardBoxList[i].m_byBoxOrder - 1].rewardBox = rewardBoxList[i];
            }
        }
    }

    private void OnActiveTreasureButton(bool active)
    {
        //튜토리얼.
        if (Kernel.entry.account.TutorialGroup <= 80)
        {
            m_TreasureButton.gameObject.SetActive(false);
            return;
        }

        if (m_TreasureButton != null && m_TreasureButton.gameObject.activeSelf != active)
            m_TreasureButton.gameObject.SetActive(active);
    }

    private void OnActivePostNewButton(bool active)
    {
        if (m_PostNewButton == null)
            return;


        if (active)
        {
            UIPost postUI = UIManager.Instance.Get(UI.Post) as UIPost;

            if (postUI != null && postUI.active)
                return;
        }
        m_PostNewButton.gameObject.SetActive(active);
    }

    private void OnActiveFranchiseNewButton(bool active)
    {
        if (m_FranchiseNewButton == null)
            return;

        if (m_FranchiseButton != null && m_FranchiseNewButton.gameObject.activeSelf != active)
            m_FranchiseNewButton.gameObject.SetActive(active);
    }

    void Renewal()
    {
        if (Kernel.entry == null)
        {
            return;
        }

        Debug.Log("currentPvPArea : " + Kernel.entry.account.currentPvPArea);
        int currentPvPArea = Kernel.entry.account.currentPvPArea;
        //m_BackgroundImage.sprite = TextureManager.GetSprite(SpritePackingTag.Lobby, string.Format("img_Lobby_Background_{0}", currentPvPArea - 1));

        DB_AreaPvP.Schema areaPvP = DB_AreaPvP.Query(DB_AreaPvP.Field.Index, currentPvPArea);
        if (areaPvP != null)
        {
            m_MatchButtonHeartText.text = (-areaPvP.Need_Heart).ToString();
        }

        CDeckData deckData = Kernel.entry.character.FindMainDeckData();
        if (deckData != null)
        {
            for (int i = 0; i < m_CharacterSummaryList.Count; i++)
            {
                // Warning, NullRefExcpt!
                long cid = deckData.m_CardCidList[i];

                m_CharacterSummaryList[i].gameObject.SetActive(cid > 0);
                m_CharacterSummaryList[i].cid = cid;
            }
        }
        Kernel.entry.normalShop.SetShopData();
        Kernel.entry.post.onActivePostCallback(Kernel.entry.post.newPostCheckExist);
        OnChestListUpdate(Kernel.entry.chest.rewardBoxList);

        Kernel.entry.franchise.REQ_PACKET_CG_READ_FRANCHISE_BUILDING_INFO_SYN();
        Kernel.entry.franchise.onActiveFranchiseRewardCallback(Kernel.entry.franchise.CheckRewardCount() > 0);
    }

    void OnMailButtonClicked()
    {
        if (Kernel.uiManager)
        {
            UIPost post = Kernel.uiManager.Get<UIPost>(UI.Post, true, false);

            if (post)
                Kernel.uiManager.Open(UI.Post);
        }
    }

    void OnDeckButtonClicked()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.LoadScene(Scene.Deck);
        }
    }

    void OnShopButtonClicked()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.LoadScene(Scene.NormalShop);
            Kernel.entry.normalShop.m_eCurrentTabType = eNormalShopItemType.NSI_PACKAGE;
        }
    }

    void OnAdventureButtonClicked()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.LoadScene(Scene.Adventure);
        }

    }

    void OnGuildButtonClick()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.LoadScene(Scene.Guild);
        }
    }

    void OnNoticeButtonClick()
    {
        string noticeURL = "http://eula.db.mseedgames.co.kr/pucca/PuccaNotice.html";
        Application.OpenURL(noticeURL);
    }
    
    void OnTreasureButtonClicked()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.LoadScene(Scene.Treasure);
        }
    }

    void OnMatchButtonClicked()
    {
        if (Kernel.entry != null)
        {
            int currentPvPArea = Kernel.entry.account.currentPvPArea;
            DB_AreaPvP.Schema areaPvP = DB_AreaPvP.Query(DB_AreaPvP.Field.Index, currentPvPArea);

            SoundDataInfo.CancelSound(m_MatchButton.gameObject);

            if (Kernel.entry.chest.isSlotFull)
            {
                UIAlerter.Alert(Languages.ToString(TEXT_UI.BOX_FULL), UIAlerter.Composition.Confirm_Cancel, OnResponse);
            }
            else if (Kernel.entry.account.heart < areaPvP.Need_Heart)
            {
                DBStr_Network.Schema schema = DBStr_Network.Query(DBStr_Network.Field.IndexID, 103);    //하트부족 메시지.
                UINotificationCenter.Enqueue(schema.StringData);
            }
            else
            {
                OnResponse(UIAlerter.Response.Confirm);
                SoundDataInfo.RevertSound(m_MatchButton.gameObject);
            }
        }
    }

    void OnRankingButtonClicked()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.LoadScene(Scene.Ranking);
        }
    }

    void OnRevengeButtonClick()
    {
        if (Kernel.sceneManager != null)
        {
            Kernel.sceneManager.LoadScene(Scene.RevengeBattle);
        }
    }

    void OnFranchiseButtonClick()
    {
        if (Kernel.sceneManager != null)
        {
            Kernel.sceneManager.LoadScene(Scene.Franchise);
        }
    }



    void OnDetectButtonClick()
    {
        if (Kernel.sceneManager != null)
        {
            Kernel.sceneManager.LoadScene(Scene.Detect);
        }
    }

    void OnResponse(UIAlerter.Response response, params object[] args)
    {
        if (response != UIAlerter.Response.Confirm)
        {
            return;
        }

        Kernel.entry.battle.CurBattleKind = BATTLE_KIND.PVP_BATTLE;
        Kernel.sceneManager.LoadScene(Scene.Battle, true);
    }







    //튜토리얼 기능 추가.
    public void SetLobby_TutorialState()
    {
        if (Kernel.entry.tutorial.GroupNumber == 0 || Kernel.entry.tutorial.GroupNumber == 10000)
        {
            ResetLobbyUI();
            return;
        }

//        Kernel.uiManager.Close(UI.HUD);
        m_MailButton.gameObject.SetActive(false);
        m_DeckButton.gameObject.SetActive(false);
        m_ShopButton.gameObject.SetActive(false);
        m_AdventureButton.gameObject.SetActive(false);
        m_MatchButton.gameObject.SetActive(false);
        m_RankingButton.gameObject.SetActive(false);
        m_RevengeButton.gameObject.SetActive(false);
        m_FranchiseButton.gameObject.SetActive(false);
        m_DetectButton.gameObject.SetActive(false);
        m_GuildButton.gameObject.SetActive(false);
        m_ChestUIObject.SetActive(false);

        int tutorialGroupNum = Kernel.entry.tutorial.GroupNumber;
        int accountLevel = Kernel.entry.account.level;

        if (tutorialGroupNum >= 10)
            m_MatchButton.gameObject.SetActive(true);

        if (tutorialGroupNum >= 20)
            m_ChestUIObject.SetActive(true);

        if (tutorialGroupNum >= 30)
            m_AdventureButton.gameObject.SetActive(true);

        if (tutorialGroupNum >= 40)
            m_DeckButton.gameObject.SetActive(true);

//        if (Kernel.entry.tutorial.GroupNumber >= 50)
//            Kernel.uiManager.Open(UI.HUD);

        if (tutorialGroupNum >= 60)
        {
            m_MailButton.gameObject.SetActive(true);
            m_ShopButton.gameObject.SetActive(true);
            m_RankingButton.gameObject.SetActive(true);
            m_RevengeButton.gameObject.SetActive(true);
        }

        if (tutorialGroupNum >= 70)
            m_DetectButton.gameObject.SetActive(true);

        if (tutorialGroupNum < 80 && accountLevel >= 3)
            m_TreasureButton.gameObject.SetActive(false);


        if (tutorialGroupNum >= 90 && accountLevel >= 3)
            m_FranchiseButton.gameObject.SetActive(true);

        if (tutorialGroupNum >= 100 && accountLevel >= 4)
            m_GuildButton.gameObject.SetActive(true);
    }



    public void ResetLobbyUI()
    {
        Kernel.uiManager.Open(UI.HUD);
        m_MailButton.gameObject.SetActive(true);
        m_DeckButton.gameObject.SetActive(true);
        m_ShopButton.gameObject.SetActive(true);
        m_AdventureButton.gameObject.SetActive(true);
        m_MatchButton.gameObject.SetActive(true);
        m_RankingButton.gameObject.SetActive(true);
        m_TreasureButton.gameObject.SetActive(true);
        m_RevengeButton.gameObject.SetActive(true);
        m_FranchiseButton.gameObject.SetActive(true);
        m_DetectButton.gameObject.SetActive(true);
        m_GuildButton.gameObject.SetActive(true);
        m_ChestUIObject.SetActive(true);
    }



}
