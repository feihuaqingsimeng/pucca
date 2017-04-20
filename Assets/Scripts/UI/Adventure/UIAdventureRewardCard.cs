using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Util;

public class UIAdventureRewardCard : MonoBehaviour
{
    public GameObject m_GoldCard;
    public GameObject m_ItemFrame;

    public  Image   GoodsImageforCardType;
    public  Image   GoodsImageforGoodsType;
    public  Text    GoodsCount;

    public void InitRewardCard(eGoodsType type, int nCount = 1)
    {
        DB_Goods.Schema GoodData = DB_Goods.Query(DB_Goods.Field.Index, (int)type);

        bool isGoldType = type == eGoodsType.Gold;
        bool isNeedFrameType = type == eGoodsType.SweepTicket || type == eGoodsType.TreasureDetectMapBlack 
            || type == eGoodsType.TreasureDetectMapCoconut || type == eGoodsType.TreasureDetectMapIce 
            || type == eGoodsType.TreasureDetectMapLake || type == eGoodsType.TreasureDetectMapTerrapin; 

        m_GoldCard.SetActive(isGoldType);
        m_ItemFrame.SetActive(isNeedFrameType);
        GoodsImageforCardType.gameObject.SetActive(!isNeedFrameType && !isGoldType);
        GoodsImageforGoodsType.gameObject.SetActive(isNeedFrameType && !isGoldType);

        if (!isNeedFrameType && !isGoldType)
            GoodsImageforCardType.sprite = TextureManager.GetGoodsTypeSprite(GoodData.Goods_Type);
        else
            GoodsImageforGoodsType.sprite = TextureManager.GetGoodsTypeSprite(GoodData.Goods_Type);

        GoodsImageforGoodsType.SetNativeSize();

        if (nCount <= 1)
            GoodsCount.gameObject.SetActive(false);
        else
        {
            GoodsCount.gameObject.SetActive(true);
            GoodsCount.text = "x" + Languages.GetNumberComma(nCount);
        }
    }

    public void InitRewardCard(Goods_Type type, int nCount = 1)
    {
        bool isGoldType = type == Goods_Type.Gold;
        bool isNeedFrameType = type == Goods_Type.SweepTicket || type == Goods_Type.TreasureDetectMap_Black
            || type == Goods_Type.TreasureDetectMap_Coconut || type == Goods_Type.TreasureDetectMap_Ice
            || type == Goods_Type.TreasureDetectMap_Lake || type == Goods_Type.TreasureDetectMap_Terrapin; 

        m_GoldCard.SetActive(isGoldType);
        m_ItemFrame.SetActive(isNeedFrameType);
        GoodsImageforCardType.gameObject.SetActive(!isNeedFrameType && !isGoldType);
        GoodsImageforGoodsType.gameObject.SetActive(isNeedFrameType && !isGoldType);

        if (!isNeedFrameType && !isGoldType)
            GoodsImageforCardType.sprite = TextureManager.GetGoodsTypeSprite(type);
        else
            GoodsImageforGoodsType.sprite = TextureManager.GetGoodsTypeSprite(type);

        GoodsImageforGoodsType.SetNativeSize();

        if (nCount <= 1)
            GoodsCount.gameObject.SetActive(false);
        else
        {
            GoodsCount.gameObject.SetActive(true);
            GoodsCount.text = "x" + Languages.GetNumberComma(nCount);
        }
    }

}
