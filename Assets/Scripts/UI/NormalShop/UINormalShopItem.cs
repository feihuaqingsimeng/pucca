using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Common.Packet;

[System.Serializable]
public class GoodsTypeItem
{
    [HideInInspector]
    public Price_BuyType    m_ePayType;

    public GameObject       m_GoodsItemObject;

    public Text             m_ItemName;
    public Text             m_ItemPriceCount;

    public Image            m_ItemIcon;
    public Image            m_ItemPriceIcon;
    public Image            m_BackGround;
}

[System.Serializable]
public class ChestTypeItem
{
    [HideInInspector]
    public Price_BuyType    m_ePayType;

    public GameObject       m_ChestItemObject;

    public Text             m_ItemName;
    public Text             m_ItemAreaName;
    public Text             m_ItemDec;
    public Text             m_ItemPriceCount;

    public Image            m_ItemIcon;
    public Image            m_ItemPriceIcon;
    public Image            m_BackGround;
}

[System.Serializable]
public class PackageTypeItem
{
    [HideInInspector]
    public Price_BuyType    m_ePayType;

    public GameObject       m_PackageItemObject;

    public Text             m_ItemName;
    public Text             m_ItemPriceCount;
    public Text             m_ItemDec;

    public Image            m_ItemIcon;
    public Image            m_Background;
}

public class UINormalShopItem : MonoBehaviour 
{
     //shopItemIcon
    [SerializeField]
    private Sprite[]                 m_GoldSprites;
    [SerializeField]
    private Sprite[]                 m_HeartSprites;
    [SerializeField]
    private Sprite[]                 m_RubbySprites;
    [SerializeField]
    private Sprite[]                 m_PackageSprites;

    [SerializeField]
    private Sprite                   m_GoldItemBackGround;
    [SerializeField]
    private Sprite                   m_HeartItemBackGround;
    [SerializeField]
    private Sprite                   m_RubbyItemBackGround;
    [SerializeField]
    private Sprite[]                 m_BoxItemBackGround;
    [SerializeField]
    private Sprite[]                 m_PackageBackGround;
    [SerializeField]
    private Sprite                   m_CashIcon;

    public  GoodsTypeItem            m_GoodsTypeItem;
    public  ChestTypeItem            m_ChestTypeItem;
    public  PackageTypeItem          m_PackageTypeItem;

    private int                      m_nShopItemIndex;

    [HideInInspector]
    public ProductItems             m_ProductItem;

    private eNormalShopItemType      m_curItemType;

    private void Awake()
    {
        Button goodsTypeItemButton      = m_GoodsTypeItem.m_GoodsItemObject.GetComponent<Button>();
        Button chestTypeItemButton      = m_ChestTypeItem.m_ChestItemObject.GetComponent<Button>();
        Button packageTypeItemButton    = m_PackageTypeItem.m_PackageItemObject.GetComponent<Button>();

        if (goodsTypeItemButton != null)
            goodsTypeItemButton.onClick.AddListener(OnClickItem);

        if (chestTypeItemButton != null)
            chestTypeItemButton.onClick.AddListener(OnClickItem);

        if (packageTypeItemButton != null)
            packageTypeItemButton.onClick.AddListener(OnClickItem);
    }

    private void OnDisable()
    {
        
    }

    //** BaseUI Setting
    public void SetUI(eNormalShopItemType ItemType, DB_NormalShop.Schema data, int itemNum)
    {
        if (data == null)
            return;

        m_nShopItemIndex = data.Index;

        m_GoodsTypeItem.m_GoodsItemObject.SetActive(ItemType == eNormalShopItemType.NSI_GOLD || ItemType == eNormalShopItemType.NSI_HEART);
        m_ChestTypeItem.m_ChestItemObject.SetActive(ItemType == eNormalShopItemType.NSI_CHEST);
        m_PackageTypeItem.m_PackageItemObject.SetActive(false);

        m_curItemType = ItemType;

        switch (ItemType)
        {
            case eNormalShopItemType.NSI_GOLD:
            case eNormalShopItemType.NSI_HEART: SetGoodsItem(ItemType, data, null, itemNum); break;
            case eNormalShopItemType.NSI_CHEST: SetChestItem(data, itemNum); break;
        }
    }

    public void SetUI(eNormalShopItemType ItemType, ProductItems data, int itemNum)
    {
        if (data == null)
            return;

        m_ProductItem = data;

        m_GoodsTypeItem.m_GoodsItemObject.SetActive(ItemType == eNormalShopItemType.NSI_RUBBY);
        m_ChestTypeItem.m_ChestItemObject.SetActive(false);
        m_PackageTypeItem.m_PackageItemObject.SetActive(ItemType == eNormalShopItemType.NSI_PACKAGE);

        m_curItemType = ItemType;

        switch (ItemType)
        {
            case eNormalShopItemType.NSI_RUBBY: SetGoodsItem(ItemType, null, data.m_ProductData, itemNum); break;
            case eNormalShopItemType.NSI_PACKAGE: SetPackageItem(data, itemNum); break;
        }
    }

    //** GoodsType Item Setting
    private void SetGoodsItem(eNormalShopItemType shopType, DB_NormalShop.Schema normaltem, DB_ProductMain.Schema productItem, int itemNum)
    {
        m_GoodsTypeItem.m_ePayType = normaltem != null ? normaltem.Price_BuyType : Price_BuyType.None;

        //Image
        Goods_Type goodsType = Kernel.entry.normalShop.FindeNormalShopItemTypeToGoodsType(shopType);
        m_GoodsTypeItem.m_ItemIcon.sprite = TextureManager.GetGoodsTypeSprite(goodsType);

        switch (goodsType)
        {
            case Goods_Type.Heart:
                m_GoodsTypeItem.m_ItemName.text     = Languages.ToString(TEXT_UI.SHOP_HEART_TITLE, normaltem.Offer_Count);
                m_GoodsTypeItem.m_BackGround.sprite = m_HeartItemBackGround;
                m_GoodsTypeItem.m_ItemIcon.sprite   = m_HeartSprites[itemNum];
                break;
            case Goods_Type.Ruby:
                DB_ProductName.Schema packageName   = DB_ProductName.Query(DB_ProductName.Field.Index, productItem.Index);
                m_GoodsTypeItem.m_ItemName.text     = Languages.ToString(packageName.TEXT_UI);
                m_GoodsTypeItem.m_BackGround.sprite = m_RubbyItemBackGround;
                if (m_RubbySprites.Length <= itemNum) return;
                m_GoodsTypeItem.m_ItemIcon.sprite   = m_RubbySprites[itemNum];
                break;
            case Goods_Type.Gold:
            default:
                m_GoodsTypeItem.m_ItemName.text     = Languages.ToString(TEXT_UI.SHOP_GOLD_TITLE, normaltem.Offer_Count);
                m_GoodsTypeItem.m_BackGround.sprite = m_GoldItemBackGround;
                m_GoodsTypeItem.m_ItemIcon.sprite   = m_GoldSprites[itemNum];
                break;
        }

        //FixSize
        m_GoodsTypeItem.m_ItemIcon.SetNativeSize();
        m_GoodsTypeItem.m_ItemPriceIcon.SetNativeSize();

        if (normaltem != null)
        {
            m_GoodsTypeItem.m_ItemPriceIcon.sprite  = TextureManager.GetGoodsTypeSprite(ConvertPayTypetoGoodsType(normaltem.Price_BuyType));
            m_GoodsTypeItem.m_ItemPriceCount.text   = normaltem.Need_Count.ToString();
        }
        else if (productItem != null)
        {
            string productId = IAPManager.GetProductId(productItem);
            if (!string.IsNullOrEmpty(productId))
            {
                UnityEngine.Purchasing.Product product = IAPManager.Instance.FindProduct(productId);

                if (product != null)
                    m_GoodsTypeItem.m_ItemPriceCount.text = product.metadata.localizedPriceString;
            }
        }

        m_GoodsTypeItem.m_ItemPriceIcon.gameObject.SetActive(normaltem != null && productItem == null);

        //GridSize
        RectTransform itemRect = m_GoodsTypeItem.m_GoodsItemObject.GetComponent<RectTransform>();
    }

    //** ChestType Item Setting
    private void SetChestItem(DB_NormalShop.Schema data, int itemNum)
    {
        DB_BoxGet.Schema boxData = DB_BoxGet.Query(DB_BoxGet.Field.Index, data.BoxGet_Link);

        if(boxData == null)
            return;

        m_ChestTypeItem.m_ePayType              = data.Price_BuyType;

        //Text
        m_ChestTypeItem.m_ItemName.text         = Languages.ToString(boxData.TEXT_UI) + " " + Languages.ToString(TEXT_UI.BOX);
        m_ChestTypeItem.m_ItemDec.text          = Languages.ToString(TEXT_UI.SHOP_BOX_BUY_INFO);
        m_ChestTypeItem.m_ItemPriceCount.text   = data.Need_Count.ToString();

        int m_nPVPArea = Kernel.entry.account.currentPvPArea;
        string areaEnumName = "AREA_" + m_nPVPArea.ToString();
        TEXT_UI areaName = (TEXT_UI)Enum.Parse(typeof(TEXT_UI), areaEnumName);
        m_ChestTypeItem.m_ItemAreaName.text = Languages.ToString(areaName);

        //Image
        m_ChestTypeItem.m_ItemIcon.sprite       = TextureManager.GetSprite(SpritePackingTag.Chest, boxData.Box_IdentificationName);
        m_ChestTypeItem.m_BackGround.sprite     = m_BoxItemBackGround[itemNum];

        //PayTypeIcon
        bool existGoodsType = ConvertPayTypetoGoodsType(data.Price_BuyType) != Goods_Type.None;

        if (existGoodsType)
            m_ChestTypeItem.m_ItemPriceIcon.sprite = TextureManager.GetGoodsTypeSprite(ConvertPayTypetoGoodsType(data.Price_BuyType));
        else
            m_GoodsTypeItem.m_ItemPriceIcon.sprite = m_CashIcon;

        //FixSize
        m_ChestTypeItem.m_ItemIcon.SetNativeSize();
        m_ChestTypeItem.m_ItemPriceIcon.SetNativeSize();

        //GirdSize
        RectTransform itemRect = m_ChestTypeItem.m_ChestItemObject.GetComponent<RectTransform>();
    }

    private void SetPackageItem(ProductItems data, int itemNum)
    {
        DB_ProductPackage.Schema package = DB_ProductPackage.Query(DB_ProductPackage.Field.PackageId, data.m_ProductData.PackageId);
        DB_ProductName.Schema packageName = DB_ProductName.Query(DB_ProductName.Field.Index, data.m_ProductData.Index);

        if(packageName != null)
            m_PackageTypeItem.m_ItemName.text = Languages.ToString(packageName.TEXT_UI);

        string productId = IAPManager.GetProductId(data.m_ProductData);
        if (!string.IsNullOrEmpty(productId))
        {
            UnityEngine.Purchasing.Product product = IAPManager.Instance.FindProduct(productId);

            if (product != null)
                m_PackageTypeItem.m_ItemPriceCount.text = product.metadata.localizedPriceString;
        }
        

        m_PackageTypeItem.m_ItemDec.gameObject.SetActive(true);

        m_PackageTypeItem.m_ItemIcon.sprite     = m_PackageSprites[itemNum];
        m_PackageTypeItem.m_Background.sprite   = m_PackageBackGround[itemNum];
        m_PackageTypeItem.m_ItemIcon.SetNativeSize();

        SetUpdatePackageDec(data);
    }

    public void SetUpdatePackageDec(ProductItems data)
    {
        // 조건 설명 Dec
        string decString = "";

        List<string> listDecString = new List<string>();

        if (data.m_ProductData.SaleStartTime != string.Empty)
            listDecString.Add(string.Format("<color=#FF4949FF>{0}</color>", Languages.ToString(TEXT_UI.SALES_RAMNANT_TIME)) + "\n" + data.m_ProductData.SaleEndTime);

        if (data.m_ProductData.LevelLimit > 0)
            listDecString.Add(Languages.ToString(TEXT_UI.BUY_LIMIT_ACCOUNT_LEVEL, string.Format("<color=#FF4949FF>{0}</color>", data.m_ProductData.LevelLimit)));


        if (data.m_bUseRemainCount)
            listDecString.Add(Languages.ToString(TEXT_UI.BUY_LIMIT_COUNT, string.Format("<color=#FF4949FF>{0}</color>", data.m_nRemainCount)));

        for (int i = 0; i < listDecString.Count; i++)
        {
            decString += listDecString[i];

            if (listDecString.Count - 1 > i)
                decString += "\n";
        }

        m_PackageTypeItem.m_ItemDec.text = decString;
    }

    //** PayType to GoodsType
    private Goods_Type ConvertPayTypetoGoodsType(Price_BuyType type)
    {
        switch (type)
        {
            case Price_BuyType.None             :
            case Price_BuyType.PayCash          : 
            case Price_BuyType.PayFree          : return Goods_Type.None;
            case Price_BuyType.PayRuby          : return Goods_Type.Ruby;
            case Price_BuyType.PayFriendPoint   : return Goods_Type.FriendPoint;
            case Price_BuyType.PayHonorPoint    : return Goods_Type.RankingPoint;
            default                             : return Goods_Type.None;
        }
    }
    
    //** ClickItem
    private void OnClickItem()
    {
        if (m_curItemType == eNormalShopItemType.NSI_GOLD || m_curItemType == eNormalShopItemType.NSI_HEART || m_curItemType == eNormalShopItemType.NSI_CHEST)
        {
            string itemName = m_curItemType == eNormalShopItemType.NSI_GOLD || m_curItemType == eNormalShopItemType.NSI_HEART
                                          ? m_GoodsTypeItem.m_ItemName.text : m_ChestTypeItem.m_ItemName.text;
            string itemCount = m_curItemType == eNormalShopItemType.NSI_GOLD || m_curItemType == eNormalShopItemType.NSI_HEART
                                                ? m_GoodsTypeItem.m_ItemPriceCount.text : m_ChestTypeItem.m_ItemPriceCount.text;
            Goods_Type goodsType = ConvertPayTypetoGoodsType
                (m_curItemType == eNormalShopItemType.NSI_GOLD || m_curItemType == eNormalShopItemType.NSI_HEART || m_curItemType == eNormalShopItemType.NSI_RUBBY
                ? m_GoodsTypeItem.m_ePayType : m_ChestTypeItem.m_ePayType);

            PopupBuyInfo.OpenPopupBuyInfo(m_nShopItemIndex, itemName, itemCount, goodsType, Kernel.entry.normalShop.BuyNormalItem);
        }
        else 
        {
            if (m_curItemType == eNormalShopItemType.NSI_RUBBY)
            {
                string itemName = m_GoodsTypeItem.m_ItemName.text;
                Goods_Type goodsType = Goods_Type.None;
                PopupBuyInfo.OpenProductBuyPopupInfo(m_ProductItem.m_ProductData.Index, itemName, m_GoodsTypeItem.m_ItemPriceCount.text, goodsType, ProductItemBuy);
            }
            else if(m_curItemType == eNormalShopItemType.NSI_PACKAGE)
            {
                UIPackageInfo packageInfo = Kernel.uiManager.Get<UIPackageInfo>(UI.PackageInfo, true, false);

                if (packageInfo == null)
                    return;

                packageInfo.SetPackage(this, m_ProductItem.m_ProductData.Index, m_ProductItem.m_ProductData.PackageId, m_PackageTypeItem.m_ItemPriceCount.text, m_PackageTypeItem.m_ItemName.text, m_PackageTypeItem.m_ItemIcon.sprite);
                Kernel.uiManager.Open(UI.PackageInfo);
            }
        }
    }

    public void ProductItemBuy(int productIndex)
    {
        Kernel.entry.normalShop.onPurchaseProductItem += ProductItemBuyResult;
        Kernel.entry.normalShop.BuyProductItem(productIndex);
    }

    //** Product Item 지급 완료 결과
    private void ProductItemBuyResult()
    {
        Kernel.entry.normalShop.onPurchaseProductItem -= ProductItemBuyResult;

        if (m_curItemType == eNormalShopItemType.NSI_PACKAGE)
        {
            Kernel.uiManager.Close(UI.PackageInfo);
            UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.MAIL_PROVIDE));
        }

        if (m_curItemType == eNormalShopItemType.NSI_PACKAGE && m_ProductItem.m_bUseRemainCount)
        {
            List<ProductItems> product = Kernel.entry.normalShop.FindProductData(eNormalShopItemType.NSI_PACKAGE);

            if (product == null)
                return;

            ProductItems productItem = product.Find(item => item.m_ProductData.Index == m_ProductItem.m_ProductData.Index);

            if (productItem == null)
                return;

            if(productItem.m_nRemainCount > 0)
                productItem.m_nRemainCount--;

            SetUpdatePackageDec(productItem);
        }
    }
}
