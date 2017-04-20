using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPopupRecevieMore : MonoBehaviour 
{
    private UIPopupReceive              m_Owner;
    private List<UIPopupReceiveObject>  m_listItemObject = new List<UIPopupReceiveObject>();
    public  GameObject                  m_ItemParent;

    public Text     m_MainTitle;
    public Text     m_SubTitle;
    public Text     m_OKButtonText;

    public Button   m_OKButton;

    public ScrollRect m_ScrollRect;

    //** 초기화
    public void SetInit()
    {
        if (m_listItemObject == null)
            return;

        for(int i = 0; i < m_listItemObject.Count; i++)
        {
            m_listItemObject[i].gameObject.transform.parent = null;
            GameObject.Destroy(m_listItemObject[i].gameObject);
        }

        m_listItemObject.Clear();

        m_ScrollRect.content.anchoredPosition = Vector2.zero;
    }

    //** 데이터 & UI 세팅
    public void SetData(List<Goods_Type> goodsType, List<int> count, string strTitle, string strSubTitle, UIPopupReceive owner)
    {
        m_Owner = owner;
        //owner.m_ReceiveObject.SetUI(goodsType, count);

        if (m_MainTitle != null)
        {
            m_MainTitle.gameObject.SetActive(!strTitle.Equals(""));
            m_MainTitle.text = strTitle;
        }

        if(m_SubTitle != null)
        {
            m_SubTitle.gameObject.SetActive(!strSubTitle.Equals(""));
            m_SubTitle.text = strSubTitle;
        }

        m_OKButtonText.text = Languages.ToString(TEXT_UI.OK);

        //Copy
        for (int i = 0; i < goodsType.Count; i++)
        {
            UIPopupReceiveObject m_ItemObject = Instantiate<UIPopupReceiveObject>(owner.m_ReceiveObject);
            UIUtility.SetParent(m_ItemObject.transform, m_ItemParent.transform);
            m_ItemObject.gameObject.SetActive(true);

            m_ItemObject.SetUI(goodsType[i], count[i]);
            m_listItemObject.Add(m_ItemObject);
        }

        ScrollRectReSize();
    }

    public void ScrollRectReSize()
    {
        if (m_listItemObject.Count <= 0 || m_listItemObject == null)
            return;

        RectTransform lastRect = m_listItemObject[0].gameObject.GetComponent<RectTransform>();

        int itemCount = m_listItemObject.Count;
        float spacing = m_ScrollRect.content.gameObject.GetComponent<GridLayoutGroup>().spacing.y;

        float y = (itemCount * lastRect.rect.height) + (itemCount * spacing);

        m_ScrollRect.content.sizeDelta = new Vector2(m_ScrollRect.content.sizeDelta.x, y);
    }
}
