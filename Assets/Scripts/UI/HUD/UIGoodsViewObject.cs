using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGoodsViewObject : MonoBehaviour 
{
    [HideInInspector]
    public Goods_Type m_eGoodsType;

    public GameObject m_GoodsContainer;
    public Image m_GoodsImage;
    public Text m_GoodsText;

    public void SetUI(Goods_Type goodsType)
    {
        m_eGoodsType = goodsType;

        m_GoodsImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType);
        m_GoodsImage.SetNativeSize();
        m_GoodsText.text = Kernel.entry.account.GetValue(goodsType).ToString();
    }

    public void UpdateGoods(int value)
    {
        if (m_GoodsText != null)
            m_GoodsText.text = value.ToString();
    }
}
