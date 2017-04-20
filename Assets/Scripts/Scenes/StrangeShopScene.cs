using Common.Packet;
using System.Collections;
using System.Collections.Generic;

public class StrangeShopScene : SceneObject
{

    // Use this for initialization

    // Update is called once per frame

    protected override void OnDisable()
    {
        Kernel.entry.strangeShop.REQ_PACKET_CG_SHOP_CLEAR_STRANGE_SHOP_NEW_FLAG_SYN();
    }

    public override IEnumerator Preprocess()
    {
        completed = false;

        Kernel.entry.strangeShop.onUpdateStrangeShopItemList += OnUpdateStrangeShopItemList;

        Kernel.entry.strangeShop.REQ_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_SYN();

        return base.Preprocess();
    }

    void OnUpdateStrangeShopItemList(List<CStrangeShopItem> strangeShopItemList)
    {
        Kernel.entry.strangeShop.onUpdateStrangeShopItemList -= OnUpdateStrangeShopItemList;

        Kernel.uiManager.Open(UI.StrangeShop);

        completed = true;
    }
}
