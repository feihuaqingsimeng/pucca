using Common.Packet;
using Common.Util;
using System.Collections.Generic;

public class StrangeShop : Node
{
    List<CStrangeShopItem> m_StrangeShopItemList = new List<CStrangeShopItem>();

    public List<CStrangeShopItem> strangeShopItemList
    {
        get
        {
            return m_StrangeShopItemList;
        }
    }

    int m_NewItemCount;

    public int newItemCount
    {
        get
        {
            return m_NewItemCount;
        }

        private set
        {
            if (m_NewItemCount != value)
            {
                m_NewItemCount = value;

                if (onChangedNewItemCount != null)
                {
                    onChangedNewItemCount(m_NewItemCount);
                }
            }
        }
    }

    public delegate void OnUpdateStrangeShopItemList(List<CStrangeShopItem> strangeShopItemList);
    public OnUpdateStrangeShopItemList onUpdateStrangeShopItemList;

    public delegate void OnStrangeShopItemBuyResult(CStrangeShopItem strangeShopItem, CCardInfo cardInfo, eBuyCount buyCount);
    public OnStrangeShopItemBuyResult onStrangeShopItemBuyResult;

    public delegate void OnChangedNewItemCount(int newItemCount);
    public OnChangedNewItemCount onChangedNewItemCount;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_ACK>(RCV_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_ACK>(RCV_PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_ACK);

        return base.OnCreate();
    }

    public bool Buyable(long sequence, eBuyCount buyCount, out Result_Define.eResult result)
    {
        result = Result_Define.eResult.SUCCESS;

        Goods_Type goodsType;
        int goodsValue;
        if (TryGetPrice(sequence, buyCount, out goodsType, out goodsValue))
        {
            switch (goodsType)
            {
                case Goods_Type.Gold:
                    if (Kernel.entry.account.gold < goodsValue)
                    {
                        result = Result_Define.eResult.NOT_ENOUGH_GOLD;
                    }
                    break;
                case Goods_Type.Ruby:
                    if (Kernel.entry.account.ruby < goodsValue)
                    {
                        result = Result_Define.eResult.NOT_ENOUGH_RUBY;
                    }
                    break;
            }
        }

        return (result == Result_Define.eResult.SUCCESS);
    }

    public bool TryGetPrice(long sequence, eBuyCount buyCount, out Goods_Type goodsType, out int goodsValue)
    {
        goodsValue = 0;
        CStrangeShopItem strangeShopItem = FindStrangeShopItem(sequence);
        if (strangeShopItem != null)
        {
            DB_Card.Schema card = DB_Card.Query(DB_Card.Field.Index, strangeShopItem.m_iCardIndex);
            if (card != null)
            {
                DB_Goods.Schema goods = DB_Goods.Query(DB_Goods.Field.Index, strangeShopItem.m_ePayType);
                if (goods != null)
                {
                    DB_StrangeShopProperty.Schema strangeShopProperty = DB_StrangeShopProperty.Query(DB_StrangeShopProperty.Field.Grade_Type, card.Grade_Type,
                                                                                                     DB_StrangeShopProperty.Field.Goods_Type, goods.Goods_Type);
                    if (strangeShopProperty != null)
                    {
                        goodsType = goods.Goods_Type;

                        goodsValue = strangeShopProperty.Price_Base;

                        for (int i = strangeShopItem.m_iRemainAmount; i < strangeShopProperty.Buylimit; i++)
                        {
                            goodsValue = goodsValue + strangeShopProperty.Price_Add;
                        }

                        int addPrice = goodsValue;
                        for (int i = 1; i < (int)buyCount; i++)
                        {
                            goodsValue = (goodsValue + strangeShopProperty.Price_Add);
                            addPrice += goodsValue;
                        }

                        goodsValue = addPrice;

                        return true;
                    }
                }
            }
        }

        goodsType = Goods_Type.None;
        LogError("Failed to try get CStrangeShopItem price. (sequence : {0})", sequence);

        return false;
    }

    public CStrangeShopItem FindStrangeShopItem(long sequence)
    {
        if (m_StrangeShopItemList != null && m_StrangeShopItemList.Count > 0)
        {
            return m_StrangeShopItemList.Find(item => item.m_Sequence == sequence);
        }

        return null;
    }

    #region REQ
    public void REQ_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_SYN()
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_SYN());
    }

    public void REQ_PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_SYN(long sequence, eBuyCount buyCount)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_SYN()
            {
                m_Sequence = sequence,
                m_eBuyCount = buyCount,
            });
    }

    public void REQ_PACKET_CG_SHOP_CLEAR_STRANGE_SHOP_NEW_FLAG_SYN()
    {
        if (m_StrangeShopItemList != null
            && m_StrangeShopItemList.Count > 0)
        {
            for (int i = 0; i < m_StrangeShopItemList.Count; i++)
            {
                m_StrangeShopItemList[i].m_bIsNew = false;
            }
        }

        newItemCount = 0;

        Kernel.networkManager.WebRequest(new PACKET_CG_SHOP_CLEAR_STRANGE_SHOP_NEW_FLAG_SYN());
    }
    #endregion

    #region RCV
    void RCV_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_ACK(PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_ACK packet)
    {
        int newItemCount = 0;
        m_StrangeShopItemList.Clear();
        if (packet.m_StrangeShopItemList != null)
        {
            for (int i = 0; i < packet.m_StrangeShopItemList.Count; i++)
            {
                CStrangeShopItem strangeShopItem = packet.m_StrangeShopItemList[i];
                if (strangeShopItem != null)
                {
                    if (strangeShopItem.m_bIsNew)
                    {
                        newItemCount++;
                    }

                    m_StrangeShopItemList.Add(strangeShopItem);
                }
            }
        }

        this.newItemCount = newItemCount;

        if (onUpdateStrangeShopItemList != null)
        {
            onUpdateStrangeShopItemList(m_StrangeShopItemList);
        }
    }

    void RCV_PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_ACK(PACKET_CG_SHOP_BUY_STRANGE_SHOP_ITEM_ACK packet)
    {
        CStrangeShopItem strangeShopItem = FindStrangeShopItem(packet.m_Sequence);
        if (strangeShopItem != null)
        {
            strangeShopItem.m_iRemainAmount = packet.m_iRemainCardAmount;
        }
        else LogError("CStrangeShopItem could not be found. (sequence : {0})", packet.m_Sequence);

        entry.account.SetValue(packet.m_ePayType, packet.m_iRemainGoods);

        if (packet.m_BuyCardInfo != null)
        {
            entry.character.UpdateCardInfo(packet.m_BuyCardInfo);
        }

        if (packet.m_BuySoulInfo != null)
        {
            entry.character.UpdateSoulInfo(packet.m_BuySoulInfo);
        }

        if (onStrangeShopItemBuyResult != null)
        {
            onStrangeShopItemBuyResult(strangeShopItem, packet.m_BuyCardInfo, (eBuyCount)packet.m_eBuyCount);
        }
    }
    #endregion
}
