using Common.Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class UIDetectChestDirection : UIObject
{
    public List<UIChestRewardCard>              m_ChestRewardCardList;
    public float                                m_CardRotateSpeed;
    public GameObject                           m_ChestOpenFX;
    public List<RuntimeAnimatorController>      m_RuntimeAnimatorControllers;

    [SerializeField]
    int     m_RewardCount;
    bool    m_Directing;
    int     m_RotateCount;
    bool    m_Clicked;

    protected override void Awake()
    {
        for (int i = 0; i < m_ChestRewardCardList.Count; i++)
        {
            m_ChestRewardCardList[i].m_CardRotateSpeed = m_CardRotateSpeed;
            m_ChestRewardCardList[i].onRotateCallback += OnRotate;
        }

    }


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
                        Kernel.uiManager.Close(UI.DetectChestDirection);
                        Kernel.sceneManager.LoadScene(Scene.Detect);
                        Kernel.uiManager.Open(UI.HUD);
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

    public void SetReward(int gold, List<CBoxResult> boxResultList)
    {
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


    public void DirectionByCoroutine()
    {
        m_Directing = true;
        StartCoroutine("ActiveFX", 0.15f);
    }

    // -> AnimationEventHandler
    public void ActiveFX()
    {
        Kernel.soundManager.PlayUISound(SOUND.SND_UI_CHESTBOX_ACT);
        m_ChestOpenFX.SetActive(true);
        OnAnimationEvent();

        m_Directing = false;
    }




    new void OnAnimationEvent()
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
