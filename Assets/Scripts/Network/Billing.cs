using UnityEngine;
using System.Collections;
using Common.Packet;

public class Billing : Node
{
    public delegate void OnPurchaseCheckResult();
    public OnPurchaseCheckResult onPurchaseCheckResult;

    public delegate void OnPurchaseResult();
    public OnPurchaseResult onPurchaseResult;

    public override Node OnCreate()
    {
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_BILLING_BUY_ITEM_GOOGLE_ACK>(RCV_PACKET_CG_BILLING_BUY_ITEM_GOOGLE_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_BILLING_BUY_ITEM_APPLE_ACK>(RCV_PACKET_CG_BILLING_BUY_ITEM_APPLE_ACK);
        entry.packetBroadcaster.AddPacketListener<PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_ACK>(RCV_PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_ACK);

        return base.OnCreate();
    }

    #region REQ
    public void REQ_PACKET_CG_BILLING_BUY_ITEM_GOOGLE_SYN(int productIndex, string productId, int packageId, string receipt)
    {
        Log("productIndex : {0}\nproductId : {1}\npackageId : {2}\nreceipt : {3}", productIndex, productId, packageId, receipt);

        Kernel.networkManager.WebRequest(new PACKET_CG_BILLING_BUY_ITEM_GOOGLE_SYN()
            {
                m_iShopIndex = productIndex,
                ProductID = productId,
                PackageID = packageId,
                PurchaseToken = receipt,
            });
    }

    public void REQ_PACKET_CG_BILLING_BUY_ITEM_APPLE_SYN(int productIndex, string productId, int packageId, string receipt)
    {
        Log("productIndex : {0}\nproductId : {1}\npackageId : {2}\nreceipt : {3}", productIndex, productId, packageId, receipt);

        Kernel.networkManager.WebRequest(new PACKET_CG_BILLING_BUY_ITEM_APPLE_SYN()
        {
            m_iShopIndex = productIndex,
            m_sReceipt = receipt,
            ProductID = productId,
            PackageID = packageId,
        });
    }

    public void REQ_PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_SYN(int productMainIndex)
    {
        Kernel.networkManager.WebRequest(new PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_SYN()
        {
            m_iShopIndex = productMainIndex
        });
    }

    #endregion

    #region RCV
    void RCV_PACKET_CG_BILLING_BUY_ITEM_GOOGLE_ACK(PACKET_CG_BILLING_BUY_ITEM_GOOGLE_ACK protocol)
    {
        var shopItemType = protocol.m_eShopItemType;
        entry.account.SetValue(protocol.m_ReceiveGoods.m_eGoodsType, protocol.m_ReceiveGoods.m_iTotalAmount);
        Kernel.iapManager.ConfirmPendingPurchase(protocol.ProductID);

        if (onPurchaseResult != null)
        {
            onPurchaseResult();
        }
    }

    void RCV_PACKET_CG_BILLING_BUY_ITEM_APPLE_ACK(PACKET_CG_BILLING_BUY_ITEM_APPLE_ACK protocol)
    {
        var shopItemType = protocol.m_eShopItemType;
        entry.account.SetValue(protocol.m_ReceiveGoods.m_eGoodsType, protocol.m_ReceiveGoods.m_iTotalAmount);
        Kernel.iapManager.ConfirmPendingPurchase(protocol.ProductID);

        if (onPurchaseResult != null)
        {
            onPurchaseResult();
        }
    }

    void RCV_PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_ACK(PACKET_CG_BILLING_CHECK_ITEM_PURCHASABLE_ACK packet)
    {
        if (packet.Result != Result_Define.eResult.SUCCESS)
            UIAlerter.Alert(string.Format("{0}", packet.Result), UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));

        if (onPurchaseCheckResult != null)
            onPurchaseCheckResult();
    }
    #endregion
}
