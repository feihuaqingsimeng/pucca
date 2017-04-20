using Common.Packet;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRevengeBattle : UIObject
{
    public Text m_EmptyText;
    public ScrollRect m_ScrollRect;
    List<UIRevengeBattleObject> m_RevengeBattleObjectList = new List<UIRevengeBattleObject>();
    public float m_Padding;
    public float m_Spacing;

    protected override void Awake()
    {
        if (Kernel.entry != null)
        {
            GameObject go = Resources.Load<GameObject>("Prefabs/UI/RevengeBattle/UIRevengeBattleObject");
            if (go != null)
            {
                UIRevengeBattleObject comp = go.GetComponent<UIRevengeBattleObject>();
                if (comp != null)
                {
                    for (int i = 0; i < Kernel.entry.data.GetValue<int>(Const_IndexID.Const_Revenge_Match_List); i++)
                    {
                        UIRevengeBattleObject item = Instantiate<UIRevengeBattleObject>(comp);
                        if (item != null)
                        {
                            item.gameObject.name = i.ToString();
                            UIUtility.SetParent(item.transform, m_ScrollRect.content);
                            item.rectTransform.anchoredPosition = Vector2.zero;
                            m_RevengeBattleObjectList.Add(item);
                        }
                    }
                }
            }
        }
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.revengeBattle.onUpdatedRevengeMatchInfoList += OnUpdatedRevengeMatchInfoList;

            OnUpdatedRevengeMatchInfoList(Kernel.entry.revengeBattle.revengeMatchInfoList);
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.entry != null)
        {
            Kernel.entry.revengeBattle.onUpdatedRevengeMatchInfoList -= OnUpdatedRevengeMatchInfoList;
        }
    }

    void OnUpdatedRevengeMatchInfoList(List<CRevengeMatchInfo> revengeMatchInfoList)
    {
        for (int i = 0; i < m_RevengeBattleObjectList.Count; i++)
        {
            UIRevengeBattleObject revengeBattleObject = m_RevengeBattleObjectList[i];
            if (revengeBattleObject != null)
            {
                CRevengeMatchInfo revengeMatchInfo = null;
                if (revengeMatchInfoList != null &&
                    revengeMatchInfoList.Count > i)
                {
                    revengeMatchInfo = revengeMatchInfoList[i];
                }

                if (revengeMatchInfo != null)
                {
                    revengeBattleObject.SetRevengeMatchInfo(revengeMatchInfo);
                    UIUtility.SetParent(revengeBattleObject.rectTransform, m_ScrollRect.content);
                    revengeBattleObject.gameObject.SetActive(true);
                }
                else
                {
                    UIUtility.SetParent(revengeBattleObject.rectTransform, transform);
                    revengeBattleObject.gameObject.SetActive(false);
                }
            }
        }

        BuildLayout();


        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 600)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }


    }

    void BuildLayout()
    {
        float y = -m_Padding;
        bool isEmpty = true;
        for (int i = 0; i < m_RevengeBattleObjectList.Count; i++)
        {
            UIRevengeBattleObject revengeBattleObject = m_RevengeBattleObjectList[i];
            if (revengeBattleObject != null
                && revengeBattleObject.gameObject.activeSelf)
            {
                RectTransform rectTransform = revengeBattleObject.rectTransform;
                if (rectTransform != null)
                {
                    if (isEmpty)
                    {
                        isEmpty = false;
                    }

                    rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
                    y = y - rectTransform.sizeDelta.y - m_Spacing;
                }
            }
        }

        y = Mathf.Abs(y - m_Spacing) + m_Padding;
        m_ScrollRect.content.sizeDelta = new Vector2(m_ScrollRect.content.sizeDelta.x, y);
        m_EmptyText.gameObject.SetActive(isEmpty);
    }
}
