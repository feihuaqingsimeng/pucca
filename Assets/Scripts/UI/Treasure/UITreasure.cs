using UnityEngine;
using System.Collections;
using Common.Packet;
using UnityEngine.UI;
using System.Collections.Generic;

public class UITreasure : UIObject
{

    private const float         F_ITEMS_SPACE               = 10.0f;
    private const float         F_ITEMS_XPOSITON_ANIMATION  = -1200.0f;
    

    public  UITreasureInfo      m_CopyBoxItem;                          //Scroll 복사용 Prefab
    public  GameObject          m_ItemParent;

    private List<UITreasureInfo> m_listInfoItem = new List<UITreasureInfo>();

    private UIActiveAnimationUtility m_activeAnimation;

    protected override void Awake()
    {
        Kernel.entry.treasure.onOpenTreasureBoxCallback += OnResultTreasureBox;

        m_activeAnimation = this.gameObject.GetComponent<UIActiveAnimationUtility>();

        if (m_activeAnimation == null)
            m_activeAnimation = this.gameObject.AddComponent<UIActiveAnimationUtility>();
            

        CreateItems();
    }

    //** Create ScrollItem
    private void CreateItems()
    {
        if (m_listInfoItem != null)
            m_listInfoItem.Clear();

        int count = 1;

        foreach (TreasureInfo item in Kernel.entry.treasure.m_dicTreasureBox.Values)
        {
            if (item == null)
                continue;

            UITreasureInfo boxItem = Instantiate<UITreasureInfo>(m_CopyBoxItem);
            UIUtility.SetParent(boxItem.transform, m_ItemParent.transform);

            boxItem.SettingItem(this, item.m_treasureBoxData, count);
            m_activeAnimation.SetData(count, boxItem.gameObject);

            m_listInfoItem.Add(boxItem);

            count++;
        }

        RepositionItems();
    }

    //** Reposition ScrollItems
    private void RepositionItems()
    {
        RectTransform parentRect = m_ItemParent.GetComponent<RectTransform>();

        float parentRect_HalfHeight = parentRect.rect.height * 0.5f;

        float preItemPosY = 0.0f;

        for (int i = 0; i < m_listInfoItem.Count; i++)
        {
            RectTransform ItemRect = m_listInfoItem[i].gameObject.GetComponent<RectTransform>();

            if (i == 0)
            {
                float itemRect_HalfHeight = ItemRect.rect.height * 0.5f;
                ItemRect.localPosition = new Vector3(F_ITEMS_XPOSITON_ANIMATION, (parentRect_HalfHeight - itemRect_HalfHeight) - F_ITEMS_SPACE, 0);
            }
            else
            {
                ItemRect.localPosition = new Vector3(F_ITEMS_XPOSITON_ANIMATION, (preItemPosY - ItemRect.rect.height) - F_ITEMS_SPACE, 0);
            }
            preItemPosY = ItemRect.localPosition.y;
        }
    }

    //** ScrollItem 새로 갱신
    private void RefleshData(List<CTreasureBox> newBoxList)
    {
        foreach (CTreasureBox newItem in newBoxList)
        {
            UITreasureInfo objItem = m_listInfoItem.Find(item => Equals(item.m_iBoxIndex, newItem.m_iBoxIndex));

            if (objItem == null)
                continue;

            objItem.SettingItem(this, newItem, objItem.m_iBoxNum);
        }
    }

    //** OpenBox!!
    private void OnResultTreasureBox(int boxIndex, int gold, List<CBoxResult> boxResultList, List<CTreasureBox> newBoxList)
    {
        UIChestDirector chestDirector = Kernel.uiManager.Get<UIChestDirector>(UI.ChestDirector, true, false);
        if (chestDirector != null)
        {
            chestDirector.SetReward(boxIndex, gold, boxResultList);
            Kernel.uiManager.Open(UI.ChestDirector);
            chestDirector.DirectionByCoroutine();
        }

        RefleshData(newBoxList);
    }

    //** UITreasureInfo의 받기 버튼들을 막을 건인지?
    public void SetAbleButtons(bool isUseBlock)
    {
        if (m_listInfoItem == null)
            return;

        for (int i = 0; i < m_listInfoItem.Count; i++)
            m_listInfoItem[i].m_OpenButton.enabled = isUseBlock;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        //튜토리얼.
        if (Kernel.entry.tutorial.TutorialActive && Kernel.entry.tutorial.WaitSeq == 800)
        {
            Kernel.entry.tutorial.onSetNextTutorial();
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        Kernel.entry.treasure.onOpenTreasureBoxCallback -= OnResultTreasureBox;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
}
