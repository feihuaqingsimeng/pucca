using Common.Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIStrangeShop : UIObject
{
    public Text m_DescriptionText;
    public List<UIStrangeShopObject> m_StrangeShopObjectList;
    public Text m_RemainTimeText;

    // Use this for initialization

    // Update is called once per frame

    protected override void OnEnable()
    {
        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException += OnNetworkException;
        }

        if (Kernel.entry != null)
        {
            Kernel.entry.strangeShop.onUpdateStrangeShopItemList += OnUpdateStrangeShopItemList;
            Kernel.entry.strangeShop.onStrangeShopItemBuyResult += OnStrangeShopItemBuyResult;

            OnUpdateStrangeShopItemList(Kernel.entry.strangeShop.strangeShopItemList);
        }
    }

    protected override void OnDisable()
    {
        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException -= OnNetworkException;
        }

        if (Kernel.entry != null)
        {
            Kernel.entry.strangeShop.onUpdateStrangeShopItemList -= OnUpdateStrangeShopItemList;
            Kernel.entry.strangeShop.onStrangeShopItemBuyResult -= OnStrangeShopItemBuyResult;
        }
    }

    void OnNetworkException(Result_Define.eResult result, string error, ePACKET_CATEGORY packetCategory, byte packetIndex)
    {
        if (result == Result_Define.eResult.ENDED_BUY_TIME &&
            packetCategory == ePACKET_CATEGORY.CG_SHOP &&
            packetIndex == (byte)eCG_SHOP.BUY_STRANGE_SHOP_ITEM_ACK)
        {
            Kernel.entry.strangeShop.REQ_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_SYN();
        }
    }

    void OnStrangeShopItemBuyResult(CStrangeShopItem strangeShopItem, CCardInfo cardInfo, Common.Util.eBuyCount buyCount)
    {
        switch (UnityEngine.Random.Range(0, 2))
        {
            case 0:
                SetDescriptionText(TEXT_UI.STRANGE_NPC_YES01);
                break;
            case 1:
                SetDescriptionText(TEXT_UI.STRANGE_NPC_YES02);
                break;
            case 2:
                SetDescriptionText(TEXT_UI.STRANGE_NPC_YES03);
                break;
        }

        if (strangeShopItem != null && m_StrangeShopObjectList != null && m_StrangeShopObjectList.Count > 0)
        {
            UIStrangeShopObject strangeShopObject = m_StrangeShopObjectList.Find(item => item.sequence == strangeShopItem.m_Sequence);
            if (strangeShopObject != null)
            {
                strangeShopObject.SetStrangeShopItem(strangeShopItem);
            }
        }

        if (cardInfo != null)
        {
            UIStrangeShopDirector strangeShopDirector = Kernel.uiManager.Get<UIStrangeShopDirector>(UI.StrangeShopDirector, true, false);
            if (strangeShopDirector != null)
            {
                strangeShopDirector.SetStrangeShopItem(strangeShopItem, cardInfo, buyCount);
                Kernel.uiManager.Open(UI.StrangeShopDirector);
            }
        }
    }

    void OnUpdateStrangeShopItemList(List<CStrangeShopItem> strangeShopItemList)
    {
        SetDescriptionText(UnityEngine.Random.Range(0, 1) != 0 ? TEXT_UI.STRANGE_NPC_HI01 : TEXT_UI.STRANGE_NPC_HI02);

        if (strangeShopItemList != null && strangeShopItemList.Count > 0)
        {
            // m_byShopOrder 오름차순 정렬
            strangeShopItemList.Sort(delegate(CStrangeShopItem lhs, CStrangeShopItem rhs)
            {
                if (lhs != null && rhs != null)
                {
                    return lhs.m_byShopOrder.CompareTo(rhs.m_byShopOrder);
                }
                else return -1;
            });

            for (int i = 0; i < m_StrangeShopObjectList.Count; i++)
            {
                UIStrangeShopObject strangeShopObject = m_StrangeShopObjectList[i];
                if (strangeShopObject != null)
                {
                    CStrangeShopItem strangeShopItem = (i < strangeShopItemList.Count) ? strangeShopItemList[i] : null;
                    if (strangeShopItem != null)
                    {
                        strangeShopObject.SetStrangeShopItem(strangeShopItem);
                    }
                    else
                    {
                        strangeShopObject.gameObject.SetActive(false);
                    }
                }
            }
        }

        StopCoroutine("Timer");
        StartCoroutine("Timer");
    }

    public void SetDescriptionText(TEXT_UI value)
    {
        m_DescriptionText.text = Languages.ToString(value);
    }

    // To Strangeshop.Update().
    IEnumerator Timer()
    {
        bool requestStrangeShopItemList = false;
        DateTime tomorrow = DateTime.Today.AddDays(1);

        while (!requestStrangeShopItemList)
        {
            TimeSpan ts = tomorrow - TimeUtility.currentServerTime;
            if (ts.TotalSeconds > 0)
            {
                m_RemainTimeText.text = Languages.ToString(TEXT_UI.REMAINING, string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds));
            }
            else
            {
                Kernel.entry.strangeShop.REQ_PACKET_CG_SHOP_GET_STRANGE_SHOP_LIST_SYN();
                requestStrangeShopItemList = true;
            }

            yield return 0;
        }
    }
}
