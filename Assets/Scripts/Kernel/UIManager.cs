using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    Dictionary<UI, UIObject> m_UIObjectDictionary = new Dictionary<UI, UIObject>();
    Stack<UIObject> m_History = new Stack<UIObject>();
    GameObject m_BlurFX;

    GameObject blurFX
    {
        get
        {
            if (m_BlurFX == null)
            {
                CreateBlurFX();
            }

            return m_BlurFX;
        }
    }

    public delegate void OnOpen(UI ui);
    public OnOpen onOpen;

    public delegate void OnClose(UI ui);
    public OnClose onClose;

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.onStartLoadScene += OnStartLoadScene;
        }
    }

    void OnDisable()
    {
        if (Kernel.sceneManager)
        {
            Kernel.sceneManager.onStartLoadScene -= OnStartLoadScene;
        }
    }

    string GetAssetPath(UI ui)
    {
        string assetPath = "Prefabs/UI/";
        switch (ui)
        {
            case UI.HUD:
            case UI.Shortcut:
                assetPath = assetPath + "HUD/";
                break;
            case UI.Lobby:
                assetPath = assetPath + "Lobby/";
                break;
            case UI.Title:
                assetPath = assetPath + "Title/";
                break;
            case UI.Deck:
            case UI.CharCardOption:
            case UI.CardInfo:
            case UI.DeckEditPopup:
                assetPath = assetPath + "Deck/";
                break;
            case UI.Battle:
            case UI.BattleStart:
            case UI.BattleResult:
            case UI.RevengeResult:
            case UI.MileageDirector:
                assetPath = assetPath + "Battle/";
                break;
            case UI.ChestInfo:
            case UI.ChestDirector:
                assetPath = assetPath + "Chest/";
                break;
            case UI.Guild:
            case UI.GuildEditor:
            case UI.GuildEnter:
            case UI.GuildMemberList:
            case UI.GuildCardRequest:
            case UI.GuildReceiveCardInfo:
            case UI.GuildIntroduceEdit:
            case UI.GuildInfo:
                assetPath = assetPath + "Guild/";
                break;
            case UI.GuildDonation:
                assetPath = assetPath + "Guild/GuildDonation/";
                break;
            case UI.GuildShop:
            case UI.GuildShopHelp:
                assetPath = assetPath + "Guild/GuildShop/";
                break;
            case UI.Ranking:
            case UI.RankingDailyRewardInfo:
            case UI.RankingSeasonRewardInfo:
                assetPath = assetPath + "Ranking/";
                break;
            case UI.Adventure:
            case UI.AdventureInfo:
            case UI.AdventureSweep:
            case UI.AdventureResult:
                assetPath = assetPath + "Adventure/";
                break;
            case UI.Treasure:
                assetPath = assetPath + "Treasure/";
                break;
            case UI.LevelUp:
                assetPath = assetPath + "LevelUp/";
                break;
            case UI.Promotion:
                assetPath = assetPath + "Promotion/";
                break;
            case UI.Post:
                assetPath = assetPath + "Post/";
                break;
            case UI.Achieve:
            case UI.AchieveNotification:
                assetPath = assetPath + "Achieve/";
                break;
            case UI.NormalShop:
            case UI.PackageInfo:
                assetPath = assetPath + "NormalShop/";
                break;
            case UI.StrangeShop:
            case UI.StrangeShopDirector:
            case UI.StrangeShopOption:
                assetPath = assetPath + "StrangeShop/";
                break;
            case UI.SecretBoxSelect:
            case UI.SecretBusiness:
            case UI.SecretCardHelp:
            case UI.SecretCardInfo:
                assetPath = assetPath + "SecretBusiness/";
                break;
            case UI.RevengeBattle:
                assetPath = assetPath + "RevengeBattle/";
                break;
            case UI.Franchise:
            case UI.FranchiseInfo:
                assetPath = assetPath + "Franchise/";
                break;
            case UI.Option:
            case UI.LanguageSeletion:
            case UI.CouponCheck:
            case UI.DropOutAccount:
            case UI.SupportCheck:
                assetPath = assetPath + "Option/";
                break;
            case UI.Detect:
            case UI.DetectAR:
            case UI.DetectChestDirection:
            case UI.DetectManual:
            case UI.DetectPopup:
                assetPath = assetPath + "Detect/";
                break;
            case UI.Tutorial:
                assetPath = assetPath + "Tutorial/";
                break;
            case UI.Notice:
                assetPath = assetPath + "Notice/";
                break;


        }

        return assetPath + "UI" + ui.ToString();
    }

    void OnAnimationEvent(UIObject comp, string triggerName)
    {
        if (!comp)
        {
            return;
        }

        if (string.Equals("Popup_open_ani", triggerName, System.StringComparison.OrdinalIgnoreCase))
        {
            comp.gameObject.SetActive(true);
        }
        else if (string.Equals("Popup_Close_ani", triggerName))
        {
            comp.gameObject.SetActive(false);

            // To Close().
            /*
            if (comp.blurOut)
            {
                m_History.Pop();
                BlurOut();
            }
            */
        }
    }

    void OnStartLoadScene(Scene scene)
    {
        List<UI> destroyables = new List<UI>();
        foreach (var item in m_UIObjectDictionary)
        {
            if (item.Value.destroyable)
            {
                if (destroyables == null)
                {
                    destroyables = new List<UI>();
                }

                destroyables.Add(item.Key);
            }
        }

        foreach (var item in destroyables)
        {
            Close(item, true);
        }

        if (m_BlurFX != null)
        {
            m_BlurFX.gameObject.SetActive(false);
        }
    }

    void BlurOut()
    {
        //Debug.Log(System.Environment.StackTrace);
        if (m_BlurFX == null)
        {
            CreateBlurFX();
        }

        bool blurOut = false;
        if (m_History.Count > 0)
        {
            UIObject target = m_History.Peek();
            if (target && target.blurOut)
            {
                int siblingIndex = target.transform.GetSiblingIndex();
                if (siblingIndex > 0)
                {
                    siblingIndex--;
                }

                UIUtility.SetParent(m_BlurFX.transform, target.transform.parent, siblingIndex);
                blurOut = true;
            }
        }

        if (m_BlurFX.activeSelf != blurOut)
        {
            m_BlurFX.SetActive(blurOut);
        }
    }

    public void Toggle(UI ui, bool forceLoad = false)
    {
        UIObject comp = Get(ui, forceLoad);
        if (comp != null)
        {
            if (comp.gameObject.activeSelf)
            {
                Close(ui);
            }
            else
            {
                Open(ui);
            }
        }
    }

    public void Close(UI ui, bool destroyImmediate = false)
    {
        UIObject comp;
        if (m_UIObjectDictionary.TryGetValue(ui, out comp))
        {
            if (comp.IsAnimation("Popup_Close_ani"))
                return;

            if (destroyImmediate)
            {
                //m_History.Pop();
                m_UIObjectDictionary.Remove(ui);
                DestroyImmediate(comp.gameObject);

                //
                if (comp.blurOut)
                {
                    // InvalidOperationExcpt, 임시 처리
                    if (m_History.Count > 0)
                    {
                        m_History.Pop();
                        BlurOut();
                    }
                }
            }
            else
            {
                if (!comp.IsAnimation("Popup_Close_ani"))
                    comp.SetTrigger("Popup_Close_ani");

                if (comp.blurOut)
                {
                    // InvalidOperationExcpt, 임시 처리
                    if (m_History.Count > 0)
                    {
                        m_History.Pop();
                        BlurOut();
                    }
                }
            }

            if (onClose != null)
            {
                onClose(ui);
            }
        }
    }

    public T Open<T>(UI ui, int siblingIndex = -1, int sortingOrder = -1) where T : UIObject
    {
        return Open(ui, siblingIndex, sortingOrder) as T;
    }

    public UIObject Open(UI ui, int siblingIndex = -1, int sortingOrder = -1)
    {
        UIObject comp = Get(ui, true, true);
        if (comp != null)
        {
            sortingOrder = (sortingOrder != -1) ? sortingOrder : comp.sortingOrder;
            sortingOrder = sortingOrder * 100;

            UIUtility.SetParent(comp.transform, Kernel.canvasManager.GetCanvas(sortingOrder).transform, siblingIndex);

            if (comp.CheckAnimator())
            {
                if (!comp.gameObject.activeSelf)
                {
                    if (comp.blurOut)
                    {
                        bool duplicated = false;
                        if (m_History.Count > 0)
                        {
                            UIObject temp = m_History.Peek();
                            if (temp != null)
                            {
                                duplicated = (temp.ui == ui);
                            }
                        }
                        if (!duplicated)
                        {
                            m_History.Push(comp);
                            BlurOut();
                        }
                    }

                    comp.gameObject.SetActive(true);
                    comp.SetTrigger("Popup_open_ani");
                }
            }
            else
            {
                if (comp.blurOut)
                {
                    bool duplicated = false;
                    if (m_History.Count > 0)
                    {
                        UIObject temp = m_History.Peek();
                        if (temp != null)
                        {
                            duplicated = (temp.ui == ui);
                        }
                    }
                    if (!duplicated)
                    {
                        m_History.Push(comp);
                        BlurOut();
                    }
                }

                if (!comp.gameObject.activeSelf)
                {
                    comp.gameObject.SetActive(true);
                    comp.SetTrigger("Popup_open_ani");
                }
            }

            if (onOpen != null)
            {
                onOpen(ui);
            }
        }

        return comp;
    }

    public T Get<T>(UI ui, bool forceLoad = false, bool activeSelf = true) where T : UIObject
    {
        return Get(ui, forceLoad, activeSelf) as T;
    }

    public UIObject Get(UI ui, bool forceLoad = false, bool activeSelf = true)
    {
        UIObject comp;
        if (!m_UIObjectDictionary.TryGetValue(ui, out comp))
        {
            if (forceLoad)
            {
                comp = Load(ui, activeSelf);
            }
        }

        return comp;
    }

    UIObject Load(UI ui, bool activeSelf)
    {
        UIObject comp = null;
        if (!m_UIObjectDictionary.TryGetValue(ui, out comp))
        {
            string assetPath = GetAssetPath(ui);
            if (!string.IsNullOrEmpty(assetPath))
            {
                GameObject gameObject = Resources.Load(assetPath) as GameObject;
                if (gameObject != null)
                {
                    comp = gameObject.GetComponent<UIObject>();
                    if (comp != null)
                    {
                        comp = Instantiate<UIObject>(comp);
                        comp.gameObject.layer = 5;
                        comp.ui = ui;
                        comp.onAnimationEvent += OnAnimationEvent;
                        comp.gameObject.SetActive(activeSelf);

                        m_UIObjectDictionary.Add(ui, comp);
                    }
                    else Debug.LogError("");
                }
                else Debug.LogError("");
            }
            else Debug.LogError("");
        }
        else Debug.LogWarning("");

        return comp;
    }

    GameObject CreateBlurFX()
    {
        if (m_BlurFX == null)
        {
            m_BlurFX = new GameObject();
            UIUtility.SetParent(m_BlurFX.transform, transform);

            Image image = m_BlurFX.AddComponent<Image>();
            image.color = new Color32(0, 0, 0, 208); //new Color(0f, 0f, 0f, 0.5f);
            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        }

        return m_BlurFX;
    }
}
