using Common.Packet;
using UnityEngine;
using UnityEngine.UI;

public class UIStrangeShopObject : MonoBehaviour
{
    public Button m_Button;
    public Text m_NameText;
    public UIMiniCharCard m_MiniCharCard;
    public Text m_RemainCountText;
    public Image m_GoodsTypeImage;
    public Text m_PriceText;
    public Image m_CompleteImage;
    long m_Sequence;
    Goods_Type m_GoodsType;

    int m_GoodsValue;
    int m_RemainValue;

    public long sequence
    {
        get
        {
            return m_Sequence;
        }
    }

    void Awake()
    {
        m_Button.onClick.AddListener(OnClick);
    }

    // Use this for initialization

    // Update is called once per frame

    public void SetStrangeShopItem(CStrangeShopItem strangeShopItem)
    {
        if (strangeShopItem != null)
        {
            m_Sequence = strangeShopItem.m_Sequence;

            m_NameText.text = Languages.FindCharName(strangeShopItem.m_iCardIndex);
            m_MiniCharCard.SetCardInfo(strangeShopItem.m_iCardIndex);

            //** 08.18. 수상한 상점 남은 수 : {0}로 표기.
            m_RemainValue = strangeShopItem.m_iRemainAmount;
            m_RemainCountText.text = Languages.ToString(TEXT_UI.SHOP_LIMIT_BUY, m_RemainValue);
            //m_RemainCountText.text = Languages.ToString(strangeShopItem.m_iRemainAmount);

            m_GoodsTypeImage.sprite = TextureManager.GetGoodsTypeSprite(strangeShopItem.m_ePayType);
            m_GoodsTypeImage.SetNativeSize();
            if (Kernel.entry.strangeShop.TryGetPrice(m_Sequence, Common.Util.eBuyCount.One, out m_GoodsType, out m_GoodsValue))
            {
                m_PriceText.text = Languages.ToString(m_GoodsValue);
            }
            m_CompleteImage.gameObject.SetActive(strangeShopItem.m_iRemainAmount == 0);
        }
    }

    void OnClick()
    {
        if (Kernel.entry != null)
        {
            CStrangeShopItem strangeShopItem = Kernel.entry.strangeShop.FindStrangeShopItem(m_Sequence);
            if (strangeShopItem != null)
            {
                if (strangeShopItem.m_iRemainAmount <= 0)
                {
                    SendMessageUpwards("SetDescriptionText", TEXT_UI.STRANGE_NPC_SOLDOUT01);
                    NetworkEventHandler.OnNetworkException(Result_Define.eResult.NOT_ENOUGH_BUY_COUNT);
                }
                else
                {
                    UIStrangeShopOption strangeShopOption = Kernel.uiManager.Open<UIStrangeShopOption>(UI.StrangeShopOption);
                    if (strangeShopOption != null)
                    {
                        strangeShopOption.SetStrangeShopObject(this);
                    }
                }
            }
        }
    }
}
