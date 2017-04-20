using Common.Packet;
using UnityEngine.UI;
using UnityEngine;

public class UIStrangeShopOption : UIObject
{
    public Button m_1Button;
    public Image m_1GoodsTypeImage;
    public Text m_1PriceText;
    public Button m_10Button;
    public Image m_10GoodsTypeImage;
    public Text m_10PriceText;
    public Button m_50Button;
    public Image m_50GoodsTypeImage;
    public Text m_50PriceText;

    long sequence
    {
        get;
        set;
    }

    protected override void Awake()
    {
        m_1Button.onClick.AddListener(On1ButtonClick);
        m_10Button.onClick.AddListener(On10ButtonClick);
        m_50Button.onClick.AddListener(On50ButtonClick);
    }

    // Use this for initialization

    // Update is called once per frame

    public void SetStrangeShopObject(UIStrangeShopObject strangeShopObject)
    {
        if (strangeShopObject != null)
        {
            sequence = strangeShopObject.sequence;
            /*
            RectTransform rectTransform = strangeShopObject.transform as RectTransform;
            float pivotY = 0;
            Vector3 worldPos = Vector3.zero;
            if (strangeShopObject.transform.position.y < 0)
            {
                pivotY = 0;
                worldPos = rectTransform.TransformPoint(0, rectTransform.rect.yMax, 0);
            }
            else
            {
                pivotY = 1;
                worldPos = rectTransform.TransformPoint(0, rectTransform.rect.yMin, 0);
            }

            this.rectTransform.pivot = new Vector2(this.rectTransform.pivot.x, pivotY);
            this.transform.position = worldPos;
            */
            RectTransform rectTransform = strangeShopObject.transform as RectTransform;
            Vector3 worldPos = rectTransform.TransformPoint(0, rectTransform.rect.yMin - 17.5f, 0);
            this.transform.position = worldPos;

            Goods_Type goodsType;
            int price;
            if (Kernel.entry.strangeShop.TryGetPrice(strangeShopObject.sequence, Common.Util.eBuyCount.One, out goodsType, out price))
            {
                m_1GoodsTypeImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType);
                m_1GoodsTypeImage.SetNativeSize();
                m_1PriceText.text = Languages.ToString(price);
            }
            if (Kernel.entry.strangeShop.TryGetPrice(strangeShopObject.sequence, Common.Util.eBuyCount.Ten, out goodsType, out price))
            {
                m_10GoodsTypeImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType);
                m_10GoodsTypeImage.SetNativeSize();
                m_10PriceText.text = Languages.ToString(price);
            }
            if (Kernel.entry.strangeShop.TryGetPrice(strangeShopObject.sequence, Common.Util.eBuyCount.Fifty, out goodsType, out price))
            {
                m_50GoodsTypeImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType);
                m_50GoodsTypeImage.SetNativeSize();
                m_50PriceText.text = Languages.ToString(price);
            }
        }
    }

    void Buy(long sequence, Common.Util.eBuyCount buyCount)
    {
        if (Kernel.entry != null)
        {
            Result_Define.eResult result;
            if (Kernel.entry.strangeShop.Buyable(sequence, buyCount, out result))
            {
                Kernel.entry.strangeShop.REQ_PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_SYN(sequence, buyCount);
            }
            else
            {
                UIStrangeShop strangeShop = Kernel.uiManager.Get<UIStrangeShop>(UI.StrangeShop, false, true);
                if (strangeShop != null)
                {
                    strangeShop.SetDescriptionText(TEXT_UI.STRANGE_NPC_NO01);
                }
                //SendMessageUpwards("SetDescriptionText", TEXT_UI.STRANGE_NPC_NO01);
                NetworkEventHandler.OnNetworkException(result);
            }
        }
    }

    void On1ButtonClick()
    {
        Buy(sequence, Common.Util.eBuyCount.One);
        OnCloseButtonClick();
    }

    void On10ButtonClick()
    {
        Buy(sequence, Common.Util.eBuyCount.Ten);
        OnCloseButtonClick();
    }

    void On50ButtonClick()
    {
        Buy(sequence, Common.Util.eBuyCount.Fifty);
        OnCloseButtonClick();
    }
}
