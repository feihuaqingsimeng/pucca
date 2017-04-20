using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public delegate void OnBuyNormalCallBack(int shopItemIndex);
public delegate void OnBuyProductCallBack(int productItemIndex);

public static class PopupBuyInfo
{
    public static void OpenPopupBuyInfo(int shopItemIndex, string buyItemName, string needGoodsCount, Goods_Type needGoodsType, OnBuyNormalCallBack buyCallBack, int remainBuyCount = -1)
    {
        UIPopupBuy popupBuy = Kernel.uiManager.Get<UIPopupBuy>(UI.PopupBuy, true, false);
        popupBuy.NormalItemBuy(shopItemIndex, Languages.ToString(TEXT_UI.BUY), buyItemName, needGoodsCount, needGoodsType, buyCallBack, remainBuyCount);
        UIManager.Instance.Open(UI.PopupBuy);
    }

    public static void OpenProductBuyPopupInfo(int productIndex, string buyItemName, string needGoodsCount, Goods_Type needGoodsType, OnBuyProductCallBack buyCallBack, int remainBuyCount = -1, int needLevel = -1)
    {
        UIPopupBuy popupBuy = Kernel.uiManager.Get<UIPopupBuy>(UI.PopupBuy, true, false);
        popupBuy.ProductItemBuy(productIndex, Languages.ToString(TEXT_UI.BUY), buyItemName, needGoodsCount, needGoodsType, buyCallBack, remainBuyCount);
        UIManager.Instance.Open(UI.PopupBuy);
    }
}

public class UIPopupBuy : UIObject 
{
    public OnBuyNormalCallBack onBuyNormalCallBack;
    public OnBuyNormalCallBack onSaveNormalBuyCallBack;

    public OnBuyProductCallBack onBuyProductCallBack;
    public OnBuyProductCallBack onSaveBuyProductCallBack;

    private int m_nShopItemIndex;
    private int m_nProductIndex;

    public Text m_MainTitle;
    public Text m_BuyDec;
    public Text m_BuyButtonText;
    public Text m_CancelButtonText;

    public Button m_BuyButton;
    public Button m_CancelButton;

    protected override void Awake()
    {
        base.Awake();
        m_BuyButton.onClick.AddListener(onClickBuy);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (onBuyNormalCallBack != null && onSaveNormalBuyCallBack != null)
        {
            onBuyNormalCallBack -= onSaveNormalBuyCallBack;
            onSaveNormalBuyCallBack = null;
        }

        if (onBuyProductCallBack != null && onSaveBuyProductCallBack != null)
        {
            onBuyProductCallBack -= onSaveBuyProductCallBack;
            onSaveBuyProductCallBack = null;
        }
    }
   

    //** Base UI Setting
    //** ps: remainBuyCount을 사용하지 않는 팝업을 사용하고 싶을 경우... remainBuyCount = -1로 두기.
    public void NormalItemBuy(int shopItemIndex, string mainTitle, string buyItemName, string needGoodsCount, Goods_Type needGoodsType, OnBuyNormalCallBack buyCallBack, int remainBuyCount = -1)
    {
        SetButtonAble(true);

        SetUI(mainTitle, buyItemName, needGoodsCount, needGoodsType, remainBuyCount);

        if (buyCallBack != null)
        {
            onBuyNormalCallBack += buyCallBack;
            onSaveNormalBuyCallBack = buyCallBack;
        }

        m_nShopItemIndex = shopItemIndex;
    }

    //** 현금 걸제하는 ProductItem 구매
    public void ProductItemBuy(int productIndex, string mainTitle, string buyItemName, string needGoodsCount, Goods_Type needGoodsType, OnBuyProductCallBack buyCallBack, int remainBuyCount = -1)
    {
        SetButtonAble(true);

        SetUI(mainTitle, buyItemName, needGoodsCount, needGoodsType, remainBuyCount);

        if (buyCallBack != null)
        {
            onBuyProductCallBack += buyCallBack;
            onSaveBuyProductCallBack = onBuyProductCallBack;
        }

        m_nProductIndex = productIndex;
    }

    public void SetUI(string mainTitle, string buyItemName, string needGoodsCount, Goods_Type needGoodsType, int remainBuyCount = -1)
    {
         //Text
        m_MainTitle.text        = Languages.ToString(TEXT_UI.BUY);

        string buyString = needGoodsType != Goods_Type.None ? Languages.ToString(needGoodsType) : "";
        m_BuyDec.text = remainBuyCount < 0
                                    ? Languages.ToString(TEXT_UI.ITEM_BUY, needGoodsCount.ToString() + buyString, buyItemName)
                                    : Languages.ToString(TEXT_UI.ITEM_LIMIT_BUY, needGoodsCount.ToString() + buyString, buyItemName, remainBuyCount);

        m_BuyButtonText.text    = Languages.ToString(TEXT_UI.BUY);
        m_CancelButtonText.text = Languages.ToString(TEXT_UI.NO);
    }

    //** 꺼지는 애니메이션 도중에 여러번 눌러지지 않도록 방지.
    public void SetButtonAble(bool able)
    {
        m_BuyButton.enabled = able;
        m_CancelButton.enabled = able;
    }

    //** Buy Click
    public void onClickBuy()
    {
        SetButtonAble(false);

        if (onBuyNormalCallBack != null)
            onBuyNormalCallBack(m_nShopItemIndex);

        if (onBuyProductCallBack != null)
        {
            onBuyProductCallBack(m_nProductIndex);
            StartCoroutine(WaitPurchase());
        }

        UIManager.Instance.Close(UI.PopupBuy);
    }

    private IEnumerator WaitPurchase()
    {
        while (!Kernel.entry.normalShop.m_bFirstParchaseCallBack)
            yield return null;

        Debug.Log("Purchase m_bFirstParchaseCallBack is Success");

        bool isSuccess = Kernel.entry.normalShop.m_bFirstParchaseSucess;
        Debug.Log(string.Format("Purchase m_bFirstParchaseSucess is {0}", isSuccess));

        if (isSuccess)
        {
            while (!Kernel.entry.normalShop.m_bSeconsParchaseCallBack)
                yield return null;

            Debug.Log("Purchase m_bSeconsParchaseCallBack is Success");
        }

        Debug.Log("Purchase is End");
    }
}
