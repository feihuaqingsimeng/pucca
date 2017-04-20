using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum eReceivePopupType
{
    RT_NONE,
    RT_ONE,
    RT_MORE,
}

public class UIPopupReceive : UIObject 
{
    public UIPopupReceiveOne    m_OneReceive;
    public UIPopupRecevieMore   m_MoreReceive;
    public UIPopupReceiveObject m_ReceiveObject;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_ReceiveObject.gameObject.SetActive(false);
    }

    //** Data 및 UI 세팅
    public void SetData(eReceivePopupType popupType, List<Goods_Type> goodsType, List<int> count, string strTitle, string strSubTitle = "")
    {
        m_OneReceive.gameObject.SetActive(popupType == eReceivePopupType.RT_ONE);
        m_MoreReceive.gameObject.SetActive(popupType == eReceivePopupType.RT_MORE);

        switch (popupType)
        {
            case eReceivePopupType.RT_NONE :
                return;
            case eReceivePopupType.RT_ONE :
                m_OneReceive.SetData(goodsType[0], count[0], strTitle, strSubTitle, this);
                m_OneReceive.m_OKButton.onClick.AddListener(OnCloseButtonClick);
                break;
            // 모두 받기의 경우에는 같은 재화끼리 묶어야하므로 일이 하나 더 있음.
            case eReceivePopupType.RT_MORE :
                Dictionary<Goods_Type, int> dicGoods = SetSeparateGoods(goodsType, count);
                List<Goods_Type> goodsTypeList = GetGoodsTypeList(dicGoods);
                List<int> countList = GetCountList(dicGoods);
                m_MoreReceive.SetData(goodsTypeList, countList, strTitle, strSubTitle, this);
                m_MoreReceive.m_OKButton.onClick.AddListener(OnCloseButtonClick);
                break; 
        }
    }

    //** 같은 종류의 재화끼리 묶음.
    private Dictionary<Goods_Type, int> SetSeparateGoods(List<Goods_Type> goodsType, List<int> count)
    {
        Dictionary<Goods_Type, int> dicGoods = new Dictionary<Goods_Type, int>();

        for (int i = 0; i < goodsType.Count; i++)
        {
            if (dicGoods.ContainsKey(goodsType[i]))
            {
                dicGoods[goodsType[i]] += count[i];
                continue;
            }

            dicGoods.Add(goodsType[i], count[i]);
        }

        return dicGoods;
    }

    //** Key(GoodsType) 값만 List로 반환
    private List<Goods_Type> GetGoodsTypeList(Dictionary<Goods_Type, int> dicGoods)
    {
        List<Goods_Type> newGoodsType = new List<Goods_Type>();

        foreach (KeyValuePair<Goods_Type, int> keyValue in dicGoods)
            newGoodsType.Add(keyValue.Key);

        return newGoodsType;
    }

    //** Value(Count) 값만 List로 반환
    private List<int> GetCountList(Dictionary<Goods_Type, int> dicGoods)
    {
        List<int> newCount = new List<int>();

        foreach (KeyValuePair<Goods_Type, int> keyValue in dicGoods)
            newCount.Add(keyValue.Value);

        return newCount;
    }

    //** 모두 초기화
    protected override void OnDisable()
    {
        m_OneReceive.SetInit();
        m_MoreReceive.SetInit();
 	    base.OnDisable();
    }
}
