using Common.Packet;
using UnityEngine;
using UnityEngine.UI;

public class UILoading : UIObject
{
    public Image m_BackgroundImage; // 로딩 검은색 배경
    public Image m_LoadingImage; // 랜덤 이미지
    public Image m_TipImage;
    public Text m_TipText;
    public GameObject CloudLoading_Obj;
    private UIWorldCloud CloudLoadingManager;

    private Scene NextScene;


    // Use this for initialization

    // Update is called once per frame

    protected override void Awake()
    {
        base.Awake();

        CloudLoadingManager = CloudLoading_Obj.GetComponent<UIWorldCloud>();
        Kernel.sceneManager.onLoadSceneCloudEvent = new SceneManager.OnLoadSceneCloudEvent(ShowCloudLoading_Join);

        if (m_BackgroundImage)
        {
            m_BackgroundImage.gameObject.SetActive(false);
        }

        if (CloudLoading_Obj)
        {
            CloudLoading_Obj.gameObject.SetActive(false);
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();

        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.onStartLoadScene += OnStartLoadScene;
            Kernel.sceneManager.onCompleteLoadScene += OnCompleteLoadScene;
        }

        if (Kernel.networkManager)
        {
            Kernel.networkManager.onException += OnNetworkException;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.onStartLoadScene -= OnStartLoadScene;
            Kernel.sceneManager.onCompleteLoadScene -= OnCompleteLoadScene;
        }

        if (Kernel.networkManager)
        {
            Kernel.networkManager.onException -= OnNetworkException;
        }

        RefreshTip();
    }

    void RefreshTip()
    {
        string tipMsg = string.Empty;
        if (DBStr_Tip.instance.schemaList != null && DBStr_Tip.instance.schemaList.Count > 0)
        {
            int randomIndex = Random.Range(0, DBStr_Tip.instance.schemaList.Count - 1);
            tipMsg = DBStr_Tip.instance.schemaList[randomIndex].Tip;
        }

        m_TipText.text = tipMsg;
        UIUtility.FitSizeToContent(m_TipText);
    }

    void OnNetworkException(Common.Packet.Result_Define.eResult result, string error, ePACKET_CATEGORY category, byte index)
    {
        if (category != ePACKET_CATEGORY.CG_GAME)
        {
            return;
        }

        switch ((eCG_GAME)index)
        {
            case eCG_GAME.PVE_RESULT_ACK:
                Kernel.entry.adventure.onStartPVE_Battle -= LoadScene;
                break;
            case eCG_GAME.PVP_RESULT_ACK:
                Kernel.entry.battle.onLoadBattleScene -= LoadScene;
                break;
            case eCG_GAME.START_PVP_MATCHING_ACK:
                ShowCloudLoading_Exit();
                break;
        }
    }

    void OnStartLoadScene(Scene scene)
    {
        switch (scene)
        {
            case Scene.Battle:
                break;

            default:
                m_LoadingImage.sprite = TextureManager.GetSprite(SpritePackingTag.Loading, "loading_" + Random.Range(0, 4).ToString());
                m_BackgroundImage.gameObject.SetActive(true);
                RefreshTip();
                break;
        }
    }

    void OnCompleteLoadScene(Scene scene)
    {
        switch (scene)
        {
            case Scene.Battle:
                ShowCloudLoading_Exit();
                break;

            default:
                m_BackgroundImage.gameObject.SetActive(false);
                break;
        }
    }




    /////////////////////추가.

    //구름 로딩.
    public void ShowCloudLoading_Join(Scene eNextScene) //도입부.
    {
        CloudLoading_Obj.gameObject.SetActive(true);
        CloudLoadingManager.CloseCloudEffect();
        CloudLoadingManager.LoadingPreEventEnd_Cloud = new UIWorldCloud.LoadingPreEventCallback_Cloud(ShowCloudLoading_Wait);
        NextScene = eNextScene;
    }


    public void ShowCloudLoading_Wait()             //로딩중 상태. 통신도 여기에 처리.
    {
        CloudLoadingManager.ShowCloudLoadingText();

        switch (NextScene)
        {
            case Scene.Battle:
                switch (Kernel.entry.battle.CurBattleKind)
                {
                    case BATTLE_KIND.PVP_BATTLE:
                        Kernel.entry.battle.onLoadBattleScene += LoadScene;
                        Kernel.entry.battle.REQ_PACKET_CG_GAME_START_PVP_MATCHING_SYN();
                        break;

                    case BATTLE_KIND.PVE_BATTLE:
                        Kernel.entry.adventure.onStartPVE_Battle += LoadScene;
                        Kernel.entry.adventure.REQ_PACKET_CG_GAME_START_PVE_SYN();
                        break;

                    case BATTLE_KIND.REVENGE_BATTLE:
                        LoadScene();
                        break;
                }
                break;
        }
    }

    // 임시 처리
    void LoadSceneByPvP()
    {
        Kernel.entry.battle.onLoadBattleScene -= LoadScene;
        LoadScene();
    }

    // 임시 처리
    void LoadSceneByPvE()
    {
        Kernel.entry.adventure.onStartPVE_Battle -= LoadScene;
        LoadScene();
    }

    public void LoadScene() //통신 종료시 씬전환.
    {
        Kernel.sceneManager.LoadScene(NextScene);
    }


    public void ShowCloudLoading_Exit()             //로딩 종료.
    {
        CloudLoadingManager.OpenCloudEffect();
    }
}
