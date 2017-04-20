using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Packet;
using Common.Util;

public class UIStrangeShopRewardCard : MonoBehaviour
{
    public UICharCard m_CharCard;
    public Text m_CharCardCountText;
    public UIGoodsCard m_GoodsCard;

    // Use this for initialization

    // Update is called once per frame

    public void SetCardInfo(CCardInfo cardInfo, int cardCount)
    {
        if (cardInfo != null)
        {
            m_CharCard.cid = cardInfo.m_Cid;
            // 임시 처리
            m_CharCard.isNew = !Kernel.entry.character.IsDirected(cardInfo.m_iCardIndex);
            Kernel.entry.character.Directed(cardInfo.m_iCardIndex);
            m_CharCardCountText.text = string.Format("x{0}", Languages.ToString(cardCount));
        }

        m_CharCard.gameObject.SetActive(true);
        m_GoodsCard.gameObject.SetActive(false);
    }

    public void SetGoods(Goods_Type goodsType, int goodsValue)
    {
        m_GoodsCard.SetGoods(goodsType, goodsValue);

        m_CharCard.gameObject.SetActive(false);
        m_GoodsCard.gameObject.SetActive(true);
    }
}
