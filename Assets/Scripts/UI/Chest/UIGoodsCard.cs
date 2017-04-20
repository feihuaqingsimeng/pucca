using UnityEngine;
using UnityEngine.UI;

public class UIGoodsCard : MonoBehaviour
{
    public Image m_GoodsTypeBackgroundImage;
    public Image m_GoodsTypeImage;
    public Image m_GoodsTypeFrameImage;
    public Text m_GoodsValueText;
    public Text m_GoodsTotalValueText;
    public Image m_SliderGoodsTypeImage;

    // Use this for initialization

    // Update is called once per frame

    public void SetGoods(Goods_Type goodsType, int goodsValue)
    {
        if (m_GoodsTypeImage != null)
        {
            m_GoodsTypeImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType);
            m_GoodsTypeImage.SetNativeSize();
        }

        if (m_SliderGoodsTypeImage != null)
        {
            m_SliderGoodsTypeImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType, true);
            m_SliderGoodsTypeImage.SetNativeSize();
        }

        if (m_GoodsTypeBackgroundImage != null)
        {
            m_GoodsTypeBackgroundImage.enabled = (goodsType == Goods_Type.Gold);
        }

        if (m_GoodsTypeFrameImage != null)
        {
            m_GoodsTypeFrameImage.gameObject.SetActive(goodsType == Goods_Type.Gold);
        }

        if (m_GoodsValueText != null)
        {
            m_GoodsValueText.text = string.Format("+{0}", Languages.ToString(goodsValue));
        }

        if (m_GoodsTotalValueText != null)
        {
            m_GoodsTotalValueText.text = Languages.ToString(Kernel.entry.account.GetValue(goodsType));
        }
    }
}
