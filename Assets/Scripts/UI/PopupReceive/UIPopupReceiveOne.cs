using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPopupReceiveOne : MonoBehaviour 
{
    private UIPopupReceive          m_Owner;
    private UIPopupReceiveObject    m_ItemObject;
    public  GameObject              m_ItemParent;

    public Text     m_MainTitle;
    public Text     m_SubTitle;

    public Button   m_OKButton;
    public Text     m_OKButtonText;

    //** 초기화
    public void SetInit()
    {
        if (m_ItemObject == null)
            return;

        m_ItemObject.gameObject.transform.parent = null;
        GameObject.Destroy(m_ItemObject.gameObject);

        m_ItemObject = null;
    }

    //** 데이터 & UI 세팅
    public void SetData(Goods_Type goodsType, int count, string strTitle, string strSubTitle, UIPopupReceive owner)
    {
        m_Owner = owner;

        m_MainTitle.gameObject.SetActive(!strTitle.Equals(""));
        m_SubTitle.gameObject.SetActive(!strSubTitle.Equals(""));

        m_MainTitle.text = strTitle;
        m_SubTitle.text  = strSubTitle;

        m_OKButtonText.text = Languages.ToString(TEXT_UI.OK);

        //Copy
        m_ItemObject = Instantiate<UIPopupReceiveObject>(owner.m_ReceiveObject);
        UIUtility.SetParent(m_ItemObject.transform, m_ItemParent.transform);
        m_ItemObject.gameObject.SetActive(true);
        m_ItemObject.SetUI(goodsType, count);
    }

}
