using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Common.Packet;

[System.Serializable]
public class ShopToggleObject
{
    public int                  m_ToggleNum;
    public eNormalShopItemType  m_eTapType;

    public RectTransform        m_trsBackground;
    public RectTransform        m_trsNameAndIconParent;
    public RectTransform        m_trsIcon;

    public Text                 m_ToggleName;
    public Image                m_ToggleIcon;

    // Name And Level Text Center
    public void SetTabTitle(string toggleName, Sprite toggleIcon, float sizeMutiValue)
    {
        m_ToggleName.text = toggleName;
        m_ToggleIcon.sprite = toggleIcon;
        m_ToggleIcon.SetNativeSize();
        m_trsIcon.transform.localScale = new Vector2(m_trsIcon.transform.localScale.x * sizeMutiValue, m_trsIcon.transform.localScale.y * sizeMutiValue);
    }

    public void SetCenter(bool tabActive)
    {
        float IconAndNameSpace = 3.0f;

        // Name Setting
        m_ToggleName.rectTransform.sizeDelta = new Vector2(m_ToggleName.preferredWidth, m_ToggleName.preferredHeight);
        m_ToggleName.rectTransform.anchoredPosition = tabActive ? new Vector2(m_trsIcon.sizeDelta.x + IconAndNameSpace, m_ToggleName.rectTransform.anchoredPosition.y) : new Vector2(0.0f, m_ToggleName.rectTransform.anchoredPosition.y);

        // Size Cal
        float parentSizeX = tabActive ?
                m_ToggleName.rectTransform.sizeDelta.x + m_ToggleIcon.rectTransform.sizeDelta.x + IconAndNameSpace : m_ToggleName.rectTransform.sizeDelta.x;

        //Parent Size and Position Setting
        m_trsNameAndIconParent.sizeDelta = new Vector2(parentSizeX, m_trsNameAndIconParent.sizeDelta.y);
        float moveValue = (float)((m_trsBackground.sizeDelta.x * 0.5f)  - (parentSizeX * 0.5f));
        m_trsNameAndIconParent.anchoredPosition = new Vector2(moveValue, m_trsNameAndIconParent.anchoredPosition.y);
    }
}

public class UINormalShop : UIObject
{
    public  UINormalShopItem        m_ItemPrefab;

    private List<UINormalShopItem>  m_listChestItems    = new List<UINormalShopItem>();
    private List<UINormalShopItem>  m_listGoodsItems    = new List<UINormalShopItem>();
    private List<UINormalShopItem> m_listPackageItmes   = new List<UINormalShopItem>();

    public  GameObject              m_ChestType_Object;
    public  GameObject              m_GoodsType_Object;
    public  GameObject              m_PackageType_Object;

    public  UIScrollRect            m_Scroll_PackageType;

    public  ToggleSiblingGroup           m_ToggleGroup;
    public  List<ShopToggleObject>  m_listToggleObjects = new List<ShopToggleObject>();
    
    private eNormalShopItemType     m_eCurrentItemType;

  protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < m_ToggleGroup.m_Toggles.Count; i++)
            m_ToggleGroup.m_Toggles[i].onValueChanged.AddListener(OnClickToggle);

        Kernel.entry.normalShop.onCreatNormalShopItem   += ChangeSelectToggle;
        Kernel.entry.normalShop.onOpenBoxItem           += OpenBox;
        
        SetUI();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        for (int i = 0; i < m_ToggleGroup.m_Toggles.Count; i++)
            m_ToggleGroup.m_Toggles[i].onValueChanged.RemoveListener(OnClickToggle);

        Kernel.entry.normalShop.onCreatNormalShopItem   -= ChangeSelectToggle;
        Kernel.entry.normalShop.onOpenBoxItem           -= OpenBox;
    }

    //** UI Setting
    private void SetUI()
    {
        SetToggle();
        SetItemType(Kernel.entry.normalShop.m_eCurrentTabType);
    }

    //** Item Tap Type Setting
    private void SetItemType(eNormalShopItemType type)
    {
        // 현재 탭과 같은 타입일 경우 그냥 지나감. (재화 타입은 같은 프리팹이지만 데이터 및 UI변경해 줘야하므로..)
        if (m_eCurrentItemType == type && m_eCurrentItemType == eNormalShopItemType.NSI_CHEST && m_eCurrentItemType == eNormalShopItemType.NSI_PACKAGE)
            return;

        m_eCurrentItemType = type;
        CreatItems();
    }

    //** Toggle 기본 세팅
    private void SetToggle()
    {
        if (m_listToggleObjects == null)
            return;

        int count = 0;

        //** PackageItem : 1탭 / 하트 : 2탭 / 골드 : 3탭 / 루비 : 4탭

        List<ProductItems> packageItem = Kernel.entry.normalShop.FindProductData(eNormalShopItemType.NSI_PACKAGE);
        if (packageItem != null && packageItem.Count > 0)
        {
            if (m_listToggleObjects.Count - 1 < count)
                return;

            SetToggle(eNormalShopItemType.NSI_PACKAGE, count);
            count++;
        }

        foreach (eNormalShopItemType shopType in Kernel.entry.normalShop.FindAllNormalTypeData().Keys)
        {
            if(m_listToggleObjects.Count -1 < count)
                return;

            SetToggle(shopType, count);
            count++;
        }

        List<ProductItems> rubbyItem = Kernel.entry.normalShop.FindProductData(eNormalShopItemType.NSI_RUBBY);
        if (rubbyItem != null && rubbyItem.Count > 0)
        {
            if (m_listToggleObjects.Count - 1 < count)
                return;

            SetToggle(eNormalShopItemType.NSI_RUBBY, count);
            count++;
        }

        // 필요한 만큼의 탭은 활성화. 나머지는 비활성화.
        for (int i = 0; i < m_ToggleGroup.m_Toggles.Count; i++)
            m_ToggleGroup.m_Toggles[i].gameObject.SetActive(i < count);

        ChangeSelectToggle();
    }

    //** Toggle 상세 세팅
    private void SetToggle(eNormalShopItemType shopType, int toggleNum)
    {
        m_listToggleObjects[toggleNum].m_ToggleNum      = toggleNum;
        m_listToggleObjects[toggleNum].m_eTapType       = shopType;

        Sprite iconSprite = null;
        string iconName = "";
        if (shopType == eNormalShopItemType.NSI_CHEST)
        {
            iconSprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_shop_tab_box");
            iconName = Languages.ToString(TEXT_UI.BOX);
        }
        else if (shopType == eNormalShopItemType.NSI_PACKAGE) 
        {
            iconSprite = TextureManager.GetSprite(SpritePackingTag.Extras, "ui_shop_tab_package");
            iconName = Languages.ToString(TEXT_UI.SHOP_TAB_CASH_PACKAGE);
        }
        else
        {
            Goods_Type goodsType = Kernel.entry.normalShop.FindeNormalShopItemTypeToGoodsType(shopType);
            iconSprite = TextureManager.GetGoodsTypeSprite(goodsType);
            iconName = Languages.ToString(goodsType);
        }

        m_listToggleObjects[toggleNum].SetTabTitle(iconName, iconSprite, shopType == eNormalShopItemType.NSI_CHEST || shopType == eNormalShopItemType.NSI_PACKAGE ? 1.0f : 0.95f);
    }

    //** Toggle Button Click
    private void OnClickToggle(bool value)
    {
        for (int i = 0; i < m_listToggleObjects.Count; i++)
        {
            bool bisOn = m_ToggleGroup.m_Toggles[i].isOn;

            //꺼진 것은 TextRect가 Null임.
            //if (m_listToggleObjects[i].m_trsTextRect != null)
            m_listToggleObjects[i].SetCenter(bisOn);
            
            m_listToggleObjects[i].m_ToggleIcon.gameObject.SetActive(bisOn);

            if (bisOn)
            {
                Kernel.entry.normalShop.m_eCurrentTabType = m_listToggleObjects[i].m_eTapType;
                SetItemType(m_listToggleObjects[i].m_eTapType);
            }
        }
    }

    //** Toggle Change
    private void ChangeSelectToggle()
    {
        eNormalShopItemType curGoodsType = Kernel.entry.normalShop.m_eCurrentTabType;
        
        ShopToggleObject findObject = m_listToggleObjects.Find(item => item.m_eTapType == curGoodsType);

        if (findObject == null)
            return;

        m_ToggleGroup.m_Toggles[findObject.m_ToggleNum].isOn = true;
    }

    //** Item Create
    private void CreatItems()
    {
        switch (m_eCurrentItemType)
        {
            case eNormalShopItemType.NSI_GOLD  :
            case eNormalShopItemType.NSI_HEART :
                CreateNomrmalItems(m_listGoodsItems, m_GoodsType_Object, m_eCurrentItemType);
                break;
            case eNormalShopItemType.NSI_RUBBY :
                CreateProductItems(m_listGoodsItems, m_GoodsType_Object, m_eCurrentItemType);
                break;
            case eNormalShopItemType.NSI_CHEST  :
                CreateNomrmalItems(m_listChestItems, m_ChestType_Object, m_eCurrentItemType);
                break;
            case eNormalShopItemType.NSI_PACKAGE:
                CreateProductItems(m_listPackageItmes, m_Scroll_PackageType.content.gameObject, m_eCurrentItemType);
                break;
            default : break;
        }

        m_ChestType_Object.SetActive(m_eCurrentItemType == eNormalShopItemType.NSI_CHEST || m_eCurrentItemType == eNormalShopItemType.NSI_NONE);
        m_GoodsType_Object.SetActive(m_eCurrentItemType == eNormalShopItemType.NSI_HEART || m_eCurrentItemType == eNormalShopItemType.NSI_GOLD || m_eCurrentItemType == eNormalShopItemType.NSI_RUBBY);
        m_PackageType_Object.SetActive(m_eCurrentItemType == eNormalShopItemType.NSI_PACKAGE);
    }

    //** NormalItems 생성
    private void CreateNomrmalItems(List<UINormalShopItem> creatItems, GameObject parentGrid, eNormalShopItemType type)
    {
        List<DB_NormalShop.Schema> listNormalItem = Kernel.entry.normalShop.FindNormalData(type);

        if (listNormalItem == null)
            return;

        // 이미 만들어진 것이면 키고 끄는 것만.
        if (creatItems.Count > 0)
        {
            // 재화 타입은 같은 프리팹이므로 데이터 변경만.
            if (m_eCurrentItemType != eNormalShopItemType.NSI_CHEST || m_eCurrentItemType != eNormalShopItemType.NSI_NONE)
            {
                for (int i = 0; i < listNormalItem.Count; i++)
                {
                    if (creatItems.Count - 1 <= i)
                    {
                        CreateNormalItem(creatItems, listNormalItem, parentGrid, i);
                        continue;
                    }

                    creatItems[i].SetUI(m_eCurrentItemType, listNormalItem[i], i);
                    creatItems[i].gameObject.SetActive(true);
                }

                int remainCount = creatItems.Count - listNormalItem.Count;

                for (int i = 0; i < remainCount; i++)
                {
                    int key = (creatItems.Count -1) - i;
                    creatItems[key].gameObject.SetActive(false);
                }

            }
        }
        // 만들어 진것이 없으면 만들어 놓기.
        else
        {
            for (int i = 0; i < listNormalItem.Count; i++)
                CreateNormalItem(creatItems, listNormalItem, parentGrid, i);
        }
    }

    private void CreateNormalItem(List<UINormalShopItem> creatItems, List<DB_NormalShop.Schema> listNormalItem, GameObject parentGrid, int count)
    {
        UINormalShopItem shopItem = Instantiate<UINormalShopItem>(m_ItemPrefab);
        UIUtility.SetParent(shopItem.transform, parentGrid.transform);
        shopItem.SetUI(m_eCurrentItemType, listNormalItem[count], count);
        creatItems.Add(shopItem);
    }

    //** ProductItems 생성
    private void CreateProductItems(List<UINormalShopItem> creatItems, GameObject parentGrid, eNormalShopItemType type)
    {
        List<ProductItems> listProductItem = Kernel.entry.normalShop.FindProductData(type);

        if (listProductItem == null)
            return;

        // 이미 만들어진 것이면 키고 끄는 것만.
        if (creatItems.Count > 0)
        {
            // 재화 타입은 같은 프리팹이므로 데이터 변경만.
            if (m_eCurrentItemType != eNormalShopItemType.NSI_PACKAGE || m_eCurrentItemType != eNormalShopItemType.NSI_NONE)
            {
                for (int i = 0; i < listProductItem.Count; i++)
                {
                    if (creatItems.Count - 1 <= i)
                    {
                        CreateProductItem(creatItems, listProductItem, parentGrid, i);
                        continue;
                    }

                    creatItems[i].SetUI(m_eCurrentItemType, listProductItem[i], i);
                    creatItems[i].gameObject.SetActive(true);
                    }

                int remainCount = creatItems.Count - listProductItem.Count;

                for (int i = 0; i < remainCount; i++)
                {
                    int key = (creatItems.Count - 1) - i;
                    creatItems[key].gameObject.SetActive(false);
                }
                    
            }
        }
        // 만들어 진것이 없으면 만들어 놓기.
        else
        {
            for (int i = 0; i < listProductItem.Count; i++)
                CreateProductItem(creatItems, listProductItem, parentGrid, i);
        }

        RectTransform itemRect = m_ItemPrefab.m_PackageTypeItem.m_PackageItemObject.GetComponent<RectTransform>();
        GridLayoutGroup grid = m_Scroll_PackageType.content.GetComponent<GridLayoutGroup>();

        if (itemRect == null)
            return;

        float xSize = (itemRect.sizeDelta.x * listProductItem.Count) + (grid.spacing.x * (listProductItem.Count - 1));
        m_Scroll_PackageType.content.sizeDelta = new Vector2(xSize, m_Scroll_PackageType.content.sizeDelta.y);

        m_Scroll_PackageType.content.anchoredPosition = new Vector2(0.0f, m_Scroll_PackageType.content.anchoredPosition.y);
    }

    private void CreateProductItem(List<UINormalShopItem> creatItems, List<ProductItems> listProductItem, GameObject parentGrid, int count)
    {
        UINormalShopItem shopItem = Instantiate<UINormalShopItem>(m_ItemPrefab);
        UIUtility.SetParent(shopItem.transform, parentGrid.transform);
        shopItem.SetUI(m_eCurrentItemType, listProductItem[count], count);
        creatItems.Add(shopItem);
    }

    //** Buy Box
    private void OpenBox(int boxIndex, int gold, List<CBoxResult> boxResultList)
    {
        UIChestDirector chestDirector = Kernel.uiManager.Get<UIChestDirector>(UI.ChestDirector, true, false);
        if (chestDirector != null)
        {
            chestDirector.SetReward(boxIndex, gold, boxResultList);
            Kernel.uiManager.Open(UI.ChestDirector);
            chestDirector.DirectionByCoroutine();
        }
    }
}




