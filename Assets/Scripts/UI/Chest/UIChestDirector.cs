using Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIChestDirector : UIObject
{
    public Image m_Image;
    public Animator m_Chest_Open_aniAnimator;
    public AnimationEventHandler m_AnimationEventHandler;
    public SkeletonAnimation m_SkeletonAnimation;
    public List<UIChestRewardCard> m_ChestRewardCardList;
    public float m_CardRotateSpeed;
    public List<RuntimeAnimatorController> m_RuntimeAnimatorControllers;
    public GameObject m_ChestOpenFX;
    public Canvas m_Canvas;

    [SerializeField]
    int m_RewardCount;
    bool m_Directing;
    int m_RotateCount;
    bool m_Clicked;

    protected override void Awake()
    {
        m_AnimationEventHandler.onAnimationEventCallback += OnAnimationEvent;

        for (int i = 0; i < m_ChestRewardCardList.Count; i++)
        {
            m_ChestRewardCardList[i].m_CardRotateSpeed = m_CardRotateSpeed;
            m_ChestRewardCardList[i].onRotateCallback += OnRotate;
        }
    }

    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        if (!m_Directing)
        {
            m_Clicked = false;
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
                if (int.Equals(m_RotateCount, m_RewardCount))
                {
                    if (Kernel.uiManager != null)
                    {
                        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 200)
                        {
                            Kernel.entry.tutorial.onSetNextTutorial();
                        }

                        Kernel.uiManager.Close(UI.ChestDirector, true);
                    }
                }
                else
                {
                    if (!EventSystem.current.currentSelectedGameObject)
                    {
                        for (int i = 0; i < m_ChestRewardCardList.Count; i++)
                        {
                            UIChestRewardCard item = m_ChestRewardCardList[i];
                            if (item && !item.rotated)
                            {
                                item.OnClick();

                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    protected override void OnEnable()
    {
        RectTransform rectTransform = m_Canvas.transform as RectTransform;
        rectTransform.anchorMin = new Vector2(.5f, .5f);
        rectTransform.anchorMax = new Vector2(.5f, .5f);
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SetReward(int boxIndex, int gold, List<CBoxResult> boxResultList, string boxSkeletonName = "")
    {
        DB_BoxGet.Schema boxGet = DB_BoxGet.Query(DB_BoxGet.Field.Index, boxIndex);
        if (boxGet != null)
            SetSkeletonAnimation(boxGet.Box_IdentificationName);
        else if (!boxSkeletonName.Equals(""))
            SetSkeletonAnimation(boxSkeletonName);

        boxResultList.Sort(delegate(CBoxResult lhs, CBoxResult rhs)
        {
            return Random.Range(-1, 1);
        });

        int randomMaxValue = gold > 0 ? boxResultList.Count + 1 : boxResultList.Count;
        int random = Random.Range(0, randomMaxValue);
        bool activeSelf;
        int index = 0;
        for (int i = 0; i < m_ChestRewardCardList.Count; i++)
        {
            if (random == i && gold > 0)
            {
                m_ChestRewardCardList[i].gold = gold;
                activeSelf = true;
            }
            else
            {
                m_ChestRewardCardList[i].boxResult = (activeSelf = boxResultList.Count > index) ? boxResultList[index++] : null;
            }

            if (activeSelf)
            {
                m_RewardCount++;
            }

            m_ChestRewardCardList[i].gameObject.SetActive(activeSelf);
        }
    }

    // Copy from UIChest.cs
    bool SetSkeletonAnimation(string identificationName)
    {
        if (!string.IsNullOrEmpty(identificationName))
        {
            string assetPath = string.Format("Spines/RewardBox/{0}/{0}_SkeletonData", identificationName);
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                m_SkeletonAnimation.skeletonDataAsset = skeletonDataAsset;
                m_SkeletonAnimation.initialSkinName = identificationName;
                m_SkeletonAnimation.AnimationName = "lock";
                m_SkeletonAnimation.loop = true;
                m_SkeletonAnimation.Reset();

                return true;
            }
            else
            {
                Debug.LogError(assetPath);
            }
        }

        return false;
    }

    public void DirectionByCoroutine()
    {
        m_Directing = true;
        StartCoroutine(Direction());
    }

    IEnumerator Direction()
    {
        float alpha = 0f;

        while (alpha <= 1f)
        {
            m_Image.color = new Color(1f, 1f, 1f, alpha);
            alpha = alpha + Time.fixedDeltaTime;

            yield return 0;
        }

        m_Chest_Open_aniAnimator.gameObject.SetActive(true);
        m_Chest_Open_aniAnimator.ResetTrigger("Normal");
        m_Chest_Open_aniAnimator.ResetTrigger("Chest_Open_ani");
        m_Chest_Open_aniAnimator.SetTrigger("Chest_Open_ani");

        Kernel.soundManager.PlayUISound(SOUND.SND_UI_CHESTBOX_ACT);

        yield break;
    }

    // -> AnimationEventHandler
    IEnumerator ActiveFX()
    {
        yield return new WaitForSeconds(0.15f);

        m_ChestOpenFX.SetActive(true);
    }

    new void OnAnimationEvent(string value)
    {
        if (string.Equals("Dropped", value))
        {
            m_SkeletonAnimation.AnimationName = "open";
            m_SkeletonAnimation.loop = false;
            m_SkeletonAnimation.Reset();
            StartCoroutine(ActiveFX());
        }
        else if (string.Equals("Chest_Open_ani Finished", value))
        {
            m_Directing = false;
        }
        else if (string.Equals("Card", value))
        {
            bool isOddNumber = m_RewardCount % 2 > 0;
            float x = -168f * Mathf.Floor(m_RewardCount / 2);
            if (!isOddNumber)
            {
                x = x + 84f;
            }
            for (int i = 0; i < m_ChestRewardCardList.Count; i++)
            {
                if (m_ChestRewardCardList[i].gameObject.activeSelf)
                {
                    m_ChestRewardCardList[i].runtimeAnimatorController = FindRuntimeAnimatorController(i, m_RewardCount);
                    m_ChestRewardCardList[i].SetTrigger(x);
                    x = x + 168f;
                }
            }
        }
    }

    RuntimeAnimatorController FindRuntimeAnimatorController(int index, int count)
    {
        if (count % 2 > 0)
        {
            switch (count)
            {
                case 1:
                    return m_RuntimeAnimatorControllers[8];
                case 3:
                    switch (index)
                    {
                        case 0:
                            return m_RuntimeAnimatorControllers[7];
                        case 1:
                            return m_RuntimeAnimatorControllers[8];
                        case 2:
                            return m_RuntimeAnimatorControllers[9];
                    }
                    break;
                case 5:
                    switch (index)
                    {
                        case 0:
                            return m_RuntimeAnimatorControllers[6];
                        case 1:
                            return m_RuntimeAnimatorControllers[7];
                        case 2:
                            return m_RuntimeAnimatorControllers[8];
                        case 3:
                            return m_RuntimeAnimatorControllers[9];
                        case 4:
                            return m_RuntimeAnimatorControllers[10];
                    }
                    break;
            }
        }
        else
        {
            switch (count)
            {
                case 2:
                    switch (index)
                    {
                        case 0:
                            return m_RuntimeAnimatorControllers[2];
                        case 1:
                            return m_RuntimeAnimatorControllers[3];
                    }
                    break;
                case 4:
                    switch (index)
                    {
                        case 0:
                            return m_RuntimeAnimatorControllers[1];
                        case 1:
                            return m_RuntimeAnimatorControllers[2];
                        case 2:
                            return m_RuntimeAnimatorControllers[3];
                        case 3:
                            return m_RuntimeAnimatorControllers[4];
                    }
                    break;
                case 6:
                    switch (index)
                    {
                        case 0:
                            return m_RuntimeAnimatorControllers[0];
                        case 1:
                            return m_RuntimeAnimatorControllers[1];
                        case 2:
                            return m_RuntimeAnimatorControllers[2];
                        case 3:
                            return m_RuntimeAnimatorControllers[3];
                        case 4:
                            return m_RuntimeAnimatorControllers[4];
                        case 5:
                            return m_RuntimeAnimatorControllers[5];
                    }
                    break;
            }
        }

        return null;
    }

    void OnRotate()
    {
        m_RotateCount++;

        if (int.Equals(m_RotateCount, m_RewardCount))
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
