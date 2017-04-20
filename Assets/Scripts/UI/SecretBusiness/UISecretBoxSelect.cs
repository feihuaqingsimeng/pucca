using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UISecretBoxSelect : UIObject 
{
    private const int               N_SCROLL_ITEMS_GROUP_COUNT  = 3;

    [Space(10)]
    public  UIScrollRect            m_Scroll;
    public  GameObject              m_Parent;
    public  UISecretBoxSelectItem   m_BoxItem;
    private List<RectTransform>     m_listCopyBoxItems = new List<RectTransform>();

    protected override void Awake()
    {
        CreateItems();

        base.Awake();
    }

    protected override void OnDestroy()
    {
        SetInit();

        base.OnDestroy();
    }

    private void SetInit()
    {
        if (m_listCopyBoxItems != null && m_listCopyBoxItems.Count > 0)
        {
            for (int i = 0; i < m_listCopyBoxItems.Count; i++)
                Destroy(m_listCopyBoxItems[i].gameObject);

            m_listCopyBoxItems.Clear();
        }
    }

    //** 박스 선택 아이템들 제작
    private void CreateItems()
    {
        List<SecretBoxData> listBoxDatas = Kernel.entry.secretBusiness.GetAllBoxDatas();

        if (listBoxDatas == null)
            return;

        for (int i = 0; i < listBoxDatas.Count; i++)
        {
            UISecretBoxSelectItem boxItem = Instantiate<UISecretBoxSelectItem>(m_BoxItem);
            UIUtility.SetParent(boxItem.transform, m_Parent.transform);

            boxItem.SetItem(listBoxDatas[i]);

            m_listCopyBoxItems.Add(boxItem.GetComponent<RectTransform>());
        }

        UIUtility.SetReposition(m_Scroll.content, m_listCopyBoxItems, 20, true, 20.0f, 20.0f, N_SCROLL_ITEMS_GROUP_COUNT);
    }
}
