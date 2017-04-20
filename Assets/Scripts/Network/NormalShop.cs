using UnityEngine;
using System.Collections;
using Common.Packet;
using System.Collections.Generic;
using System;
using UnityEngine.Purchasing;

public enum eNormalShopItemType
{
    NSI_NONE,
    NSI_GOLD,
    NSI_HEART,
    NSI_CHEST,
    NSI_RUBBY,
    NSI_PACKAGE,
}

public class ProductItems
{
    public bool m_bUseRemainCount;
    public bool m_bUseEventTime;
    public int  m_nRemainCount;
    public DateTime m_EventStartTime;
    public DateTime m_EventEndTime;
    public DB_ProductMain.Schema m_ProductData;
}

public class NormalShop : Node 
{
    public delegate void OnCreatNormalShopItem();
    public OnCreatNormalShopItem onCreatNormalShopItem;

    public delegate void OnOpenBoxItem(int boxIndex, int gold, List<CBoxResult> boxResultList);
    public OnOpenBoxItem onOpenBoxItem;

    public delegate void OnPurchaseProductItem();
    public OnPurchaseProductItem onPurchaseProductItem;

    private Dictionary<eNormalShopItemType, List<DB_NormalShop.Schema>>  m_dicNormalShopData = new Dictionary<eNormalShopItemType, List<DB_NormalShop.Schema>>();
    private Dictionary<eNormalShopItemType, List<ProductItems>> m_dicProductData = new Dictionary<eNormalShopItemType, List<ProductItems>>();

    public eNormalShopItemType m_eCurrentTabType;

    public bool m_bFirstParchaseCallBack     = false;    // 첫번째 구매 인중 결과 (영수증까지 나왔는지 체크 후 콜백)
    public bool m_bSeconsParchaseCallBack    = false;    // 두번째 구매 인증 결과 (서버에 구매했다고 보고 후 콜백)
    public bool m_bFirstParchaseSucess       = true;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_READ_CASH_SHOP_BUY_COUNT_ACK>(REV_PACKET_CG_READ_CASH_SHOP_BUY_COUNT_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_ACK>(REV_PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_ACK>(REV_PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_ACK);

        return base.OnCreate();
    }

    //** All ShopData Setting
    public void SetShopData()
    {
        if (m_dicNormalShopData != null && m_dicNormalShopData.Count > 0)
            return;

        if (IAPManager.Instance != null && IAPManager.Instance.onPurchaseResult == null)
            IAPManager.Instance.onPurchaseResult += FirstPurchaseResultCallBack;

        if (Kernel.entry.billing != null && Kernel.entry.billing.onPurchaseResult == null)
            Kernel.entry.billing.onPurchaseResult += SeconsPurchaseResultCallBack;

        //Normal Items
        List<DB_NormalShop.Schema> shopTable = DB_NormalShop.instance.schemaList;

        if (shopTable == null || m_dicNormalShopData == null)
            return;

        for (int i = 0; i < shopTable.Count; i++)
        {
            DB_NormalShop.Schema normalItem = shopTable[i];
            eNormalShopItemType shopType = eNormalShopItemType.NSI_NONE;

            if (normalItem.BoxGet_Link > 0)
                shopType = eNormalShopItemType.NSI_CHEST;
            else
                shopType = FindeGoodsTypeTypeToNormalShopItemType(normalItem.Goods_Type);

            if (m_dicNormalShopData.ContainsKey(shopType))
            {
                m_dicNormalShopData[shopType].Add(shopTable[i]);
                continue;
            }

            List<DB_NormalShop.Schema> listShopData = new List<DB_NormalShop.Schema>();
            listShopData.Add(shopTable[i]);
            m_dicNormalShopData.Add(shopType, listShopData);
        }

        if (m_dicProductData != null && m_dicProductData.Count > 0)
            return;

        // Product Items
        List<DB_ProductMain.Schema> productTable = DB_ProductMain.instance.schemaList;

        if (productTable == null || m_dicProductData == null)
            return;

        for (int i = 0; i < productTable.Count; i++)
        {
            DB_ProductMain.Schema product = productTable[i];
            eNormalShopItemType shopType = eNormalShopItemType.NSI_NONE;

            DB_ProductPackage.Schema itemTalbe = DB_ProductPackage.Query(DB_ProductPackage.Field.PackageId, product.PackageId);

            if (itemTalbe == null)
                continue;

            if (product.ItemProductType == ItemProductType.Single)
                shopType = FindeGoodsTypeTypeToNormalShopItemType(itemTalbe.Goods_Type);
            else
                shopType = eNormalShopItemType.NSI_PACKAGE;

            ProductItems newProductItem         = new ProductItems();
            newProductItem.m_bUseRemainCount    = product.AccountLimit > 0;
            newProductItem.m_nRemainCount       = product.AccountLimit;
            newProductItem.m_bUseEventTime      = product.SaleStartTime != string.Empty;
            newProductItem.m_EventStartTime     = newProductItem.m_bUseEventTime ? GetStringToDataTime(product.SaleStartTime) : DateTime.MinValue;
            newProductItem.m_EventEndTime       = newProductItem.m_bUseEventTime ? GetStringToDataTime(product.SaleEndTime) : DateTime.MinValue;

            newProductItem.m_ProductData = product;

            if (m_dicProductData.ContainsKey(shopType))
            {
                m_dicProductData[shopType].Add(newProductItem);
                continue;
            }

            List<ProductItems> newProductList = new List<ProductItems>();
            newProductList.Add(newProductItem);
            m_dicProductData.Add(shopType, newProductList); 
        }

        // 유료화 상품 정보 불러오기
        REQ_PACKET_CG_READ_CASH_SHOP_BUY_COUNT_SYN();
    }

    public DateTime GetStringToDataTime(string time)
    {
        string[] splitString = time.Split('-');
        DateTime newDateTime = new DateTime(int.Parse(splitString[0]), int.Parse(splitString[1]), int.Parse(splitString[2]));

        return newDateTime;
    }

    //** NormalItemBuy
    public void BuyNormalItem(int shopItemIndex)
    {
        DB_NormalShop.Schema normalShopData = DB_NormalShop.Query(DB_NormalShop.Field.Index, shopItemIndex);

        if (normalShopData == null)
            return;

        if(normalShopData.BoxGet_Link > 0)
            REQ_PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_SYN(shopItemIndex);
        else
            REQ_PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_SYN(shopItemIndex);
    }

    //** ProductItemBuy
    public void BuyProductItem(int productIndex)
    {
#if UNITY_EDITOR
        UIAlerter.Alert("테스트 계정으로 모바일에서 확인 가능합니다.", UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
        return;
#endif

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
        IAPManager.Instance.Purchase(productIndex);
#endif
    }

    //** 첫번째 구매 인증 후 콜백
    private void FirstPurchaseResultCallBack(bool purchaseProcessing, Product product, PurchaseFailureReason failureReason)
    {
        m_bFirstParchaseSucess = failureReason == 0;
        m_bFirstParchaseCallBack = true;

        if (failureReason != 0)
            UIAlerter.Alert(string.Format("PurchaseFailureReason : {0}", failureReason), UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }

    //** 두번째 구매 인증 후 콜백
    private void SeconsPurchaseResultCallBack()
    {
        m_bSeconsParchaseCallBack = true;

        LastResultPurchase();
    }

    //** 모든 구매 인증이 완료됨.
    private void LastResultPurchase()
    {
        if (onPurchaseProductItem != null)
            onPurchaseProductItem();

        m_bFirstParchaseCallBack = false;
        m_bSeconsParchaseCallBack = false;
    }

    //** NormalShopItemType => GoodsType
    public Goods_Type FindeNormalShopItemTypeToGoodsType(eNormalShopItemType type)
    {
        switch (type)
        {
            case eNormalShopItemType.NSI_GOLD:   return Goods_Type.Gold;
            case eNormalShopItemType.NSI_HEART:  return Goods_Type.Heart;
            case eNormalShopItemType.NSI_RUBBY:  return Goods_Type.Ruby;
            case eNormalShopItemType.NSI_CHEST:
            case eNormalShopItemType.NSI_PACKAGE:
            case eNormalShopItemType.NSI_NONE:
            default: return Goods_Type.None;
        }
    }

    //** GoodsType => NormalShopItemType
    public eNormalShopItemType FindeGoodsTypeTypeToNormalShopItemType(Goods_Type type, bool chest = false, bool package = false)
    {
        switch (type)
        {
            case Goods_Type.Gold:   return eNormalShopItemType.NSI_GOLD;
            case Goods_Type.Heart:  return eNormalShopItemType.NSI_HEART;
            case Goods_Type.Ruby:   return eNormalShopItemType.NSI_RUBBY;
        }

        if (chest)
            return eNormalShopItemType.NSI_CHEST;

        if (package)
            return eNormalShopItemType.NSI_PACKAGE;

        return eNormalShopItemType.NSI_NONE;

    }

    //** Find All NormalItems Data 
    public Dictionary<eNormalShopItemType, List<DB_NormalShop.Schema>> FindAllNormalTypeData()
    {
        return m_dicNormalShopData;
    }

    //** Find All ProductItems Data 
    public Dictionary<eNormalShopItemType, List<ProductItems>> FindAllProductTypeData()
    {
        return m_dicProductData;
    }

    //** Find Normal Data 
    public List<DB_NormalShop.Schema> FindNormalData(eNormalShopItemType goodsType)
    {
        return m_dicNormalShopData.ContainsKey(goodsType) ? m_dicNormalShopData[m_eCurrentTabType] : null;
    }

    //** Find Product Data
    public List<ProductItems> FindProductData(eNormalShopItemType productType)
    {
        return m_dicProductData.ContainsKey(productType) ? m_dicProductData[productType] : null;
    }

#region Packet
    #region Req
    //** Cash Item 정보 요청
    public void REQ_PACKET_CG_READ_CASH_SHOP_BUY_COUNT_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_READ_CASH_SHOP_BUY_COUNT_SYN());
    }

    //** BoxItem Buy
    public void REQ_PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_SYN(int shopItemIndex)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_SYN()
            {
                m_iShopIndex = shopItemIndex
            });
    }

    //** NormalItem Buy
    public void REQ_PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_SYN(int shopItemIndex)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_SYN()
            {
                m_iShopIndex = shopItemIndex
            });
    }

    #endregion

    #region Rev
    //** Cash Item 정보 결과
    public void REV_PACKET_CG_READ_CASH_SHOP_BUY_COUNT_ACK(PACKET_CG_READ_CASH_SHOP_BUY_COUNT_ACK packet)
    {
        if (packet.m_CashShopBuyCountList == null)
            return;

        List<CCashShopBuyCount> packetProductItems = packet.m_CashShopBuyCountList;

        if(m_dicProductData == null)
            return;

        for (int i = 0; i < packetProductItems.Count; i++)
        {
            CCashShopBuyCount product = packetProductItems[i];

            if (!product.m_bIsPay)
            {
                if (Kernel.entry != null)
                    Kernel.entry.billing.REQ_PACKET_CG_BILLING_BUY_ITEM_GOOGLE_SYN(product.m_iShopIndex, "", 0, "");
            }

            ProductItems productItem = null;

            // 루비 먼저 체크
            if(m_dicProductData.ContainsKey(eNormalShopItemType.NSI_RUBBY))
                productItem = m_dicProductData[eNormalShopItemType.NSI_RUBBY].Find(item => item.m_ProductData.Index == product.m_iShopIndex);

            // 루비가 아니면 패키지 체크
            if (productItem == null)
            {
                if(m_dicProductData.ContainsKey(eNormalShopItemType.NSI_PACKAGE))
                    productItem = m_dicProductData[eNormalShopItemType.NSI_PACKAGE].Find(item => item.m_ProductData.Index == product.m_iShopIndex);
            }

            if (productItem == null)
                continue;

            // 남은 횟수 계산
            if (!productItem.m_bUseRemainCount)
                continue;

            productItem.m_nRemainCount = productItem.m_nRemainCount - product.m_iBuyCount;
        }
    }

    //** BoxItem Buy Result
    public void REV_PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_ACK(PACKET_CG_SHOP_BUY_NORMAL_SHOP_BOX_ACK packet)
    {
        if (packet == null)
            return;

        entry.account.gold = packet.m_iTotalGold;
        entry.account.ruby = packet.m_iRemainRuby;

        for (int i = 0; i < packet.m_CardList.Count; i++)
            entry.character.UpdateCardInfo(packet.m_CardList[i]);

        for (int i = 0; i < packet.m_SoulList.Count; i++)
            entry.character.UpdateSoulInfo(packet.m_SoulList[i]);

        if(onOpenBoxItem != null)
            onOpenBoxItem(packet.m_iBoxIndex, packet.m_iEarnGold, packet.m_BoxResultList);
    }

    //** NormalItem Buy Result
    public void REV_PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_ACK(PACKET_CG_SHOP_BUY_NORMAL_SHOP_ITEM_ACK packet)
    {
        if (packet == null)
            return;

        entry.account.ruby = packet.m_iRemainRuby;

        Kernel.entry.account.SetValue(packet.m_ReceiveGoods.m_eGoodsType, packet.m_ReceiveGoods.m_iTotalAmount);
    }

    #endregion
#endregion

}

