using Common.Packet;
using UnityEngine;

public class UIStrangeShopDirector : UIObject
{
    public UIStrangeShopRewardCard m_StrangeShopRewardCard;
    public GameObject m_FX;
    bool m_Clicked;

    // Use this for initialization

    // Update is called once per frame
    protected override void Update()
    {
        // Copy from UIChestDirector.cs
        m_Clicked = false;
        if (Application.isMobilePlatform && Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            m_Clicked = (t.phase == TouchPhase.Began);
        }
        else
        {
            m_Clicked = Input.GetMouseButtonDown(0);
        }

        if (m_Clicked)
        {
            if (Kernel.uiManager != null)
            {
                Kernel.uiManager.Close(UI.StrangeShopDirector);
            }
        }
    }

    protected override void OnEnable()
    {
        m_StrangeShopRewardCard.gameObject.SetActive(true);
        m_FX.SetActive(true);
    }

    protected override void OnDisable()
    {
        m_StrangeShopRewardCard.gameObject.SetActive(false);
        m_FX.SetActive(false);
    }

    public void SetOnlyAct(CCardInfo cardInfo, int count)
    {
        if (cardInfo != null)
            m_StrangeShopRewardCard.SetCardInfo(cardInfo, count);
    }

    public void SetStrangeShopItem(CStrangeShopItem strangeShopItem, CCardInfo cardInfo, Common.Util.eBuyCount buyCount)
    {
        if (strangeShopItem != null &&
            cardInfo != null)
        {
            m_StrangeShopRewardCard.SetCardInfo(cardInfo, (int)buyCount);
        }
    }

    public void SetGuildShopItem(int itemIndex, byte buyCount)
    {
        DB_GuildShopList.Schema guildShopList = DB_GuildShopList.Query(DB_GuildShopList.Field.Index, itemIndex);
        if (guildShopList != null)
        {
            switch (guildShopList.Goods_Type)
            {
                case Goods_Type.None:
                case Goods_Type.Card:
                    CCardInfo cardInfo = Kernel.entry.character.FindCardInfo(guildShopList.Card_IndexID);
                    if (cardInfo != null)
                    {
                        m_StrangeShopRewardCard.SetCardInfo(cardInfo, buyCount);
                    }
                    break;
                default:
                    m_StrangeShopRewardCard.SetGoods(guildShopList.Goods_Type, buyCount);
                    break;
            }
        }
    }
}
