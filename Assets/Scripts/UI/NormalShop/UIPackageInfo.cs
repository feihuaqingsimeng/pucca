using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIPackageInfo : UIObject
{
    private int     m_nProductIndex;

    private UINormalShopItem    m_owner;

    public Image    m_MainPackageImage;
    public Text     m_MainItemName;
    public Text     m_MainDec;

    public UIPackageInfoItems       m_CopyPrefabs;
    public List<UIPackageInfoItems> m_listPackageItems;

    public GridLayoutGroup          m_PrentGrid;

    public Button m_PurchaseButton;

    protected override void Awake()
    {
        base.Awake();

        if (m_PurchaseButton != null)
            m_PurchaseButton.onClick.AddListener(OnClickBuy);
    }

    public void SetPackage(UINormalShopItem owner ,int productIndex,int packageID, string priceCount, string packageName, Sprite iconSprite)
    {
        m_owner = owner;

        m_nProductIndex = productIndex;
        m_MainPackageImage.sprite = iconSprite;
        m_MainPackageImage.SetNativeSize();
        m_MainItemName.text = packageName;
        m_MainDec.text = Languages.ToString(TEXT_UI.ITEM_BUY_NO_ENTER, priceCount, packageName);

        List<DB_ProductPackage.Schema> packageList = DB_ProductPackage.instance.schemaList;

        if (packageList == null)
            return;

        List<DB_ProductPackage.Schema> needPackageItems = new List<DB_ProductPackage.Schema>();
        for (int i = 0; i < packageList.Count; i++)
        {
            DB_ProductPackage.Schema package = packageList[i];

            if (package.PackageId == packageID)
                needPackageItems.Add(package);
        }

        CreateItems(needPackageItems);
    }

    private void CreateItems(List<DB_ProductPackage.Schema> listPackage)
    {

        if (m_listPackageItems.Count > 0)
            DestroyItems();

        m_CopyPrefabs.gameObject.SetActive(true);

        for (int i = 0; i < listPackage.Count; i++)
        {
            DB_ProductPackage.Schema package = listPackage[i];

            UIPackageInfoItems item = Instantiate<UIPackageInfoItems>(m_CopyPrefabs);
            UIUtility.SetParent(item.transform, m_PrentGrid.transform);
            item.SetUI(package);
            m_listPackageItems.Add(item);
        }

        RectTransform parentRect = m_PrentGrid.gameObject.GetComponent<RectTransform>();
        float ySize = (m_PrentGrid.spacing.y * listPackage.Count -1) + (m_CopyPrefabs.m_thisRecttrans.sizeDelta.y * listPackage.Count);
        parentRect.sizeDelta = new Vector2(parentRect.sizeDelta.x, ySize);
        parentRect.anchoredPosition = new Vector2(parentRect.anchoredPosition.x, 0.0f);

        m_CopyPrefabs.gameObject.SetActive(false);
    }

    private void DestroyItems()
    {
        for (int i = 0; i < m_listPackageItems.Count; i++)
            Destroy(m_listPackageItems[i].gameObject);

        m_listPackageItems.Clear();
        
    }

    private void OnClickBuy()
    {
        ProductItems productItem = m_owner.m_ProductItem;

        if (productItem == null)
            return;

        //** 구매 전 체크
        if (productItem.m_ProductData.LevelLimit < Kernel.entry.account.level && productItem.m_ProductData.LevelLimit != 0)
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.BUY_LIMIT_ACCOUNT_LEVEL, productItem.m_ProductData.LevelLimit),
                UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
            return;
        }

        if (productItem.m_bUseRemainCount && productItem.m_nRemainCount <= 0)
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.CAN_NOT_BUY_COUNT_LIMIT), UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
            return;
        }

        if (productItem.m_bUseEventTime && productItem.m_EventStartTime > TimeUtility.currentServerTime && productItem.m_EventEndTime < TimeUtility.currentServerTime)
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.CAN_NOT_BUY_COUNT_LIMIT), UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
            return;
        }

        Kernel.entry.billing.onPurchaseCheckResult += ProductItemBuy;
        Kernel.entry.billing.REQ_PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_SYN(productItem.m_ProductData.Index);
    }

    private void ProductItemBuy()
    {
        Kernel.entry.billing.onPurchaseCheckResult -= ProductItemBuy;
        m_owner.ProductItemBuy(m_nProductIndex);
        StartCoroutine(WaitPurchase());
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
