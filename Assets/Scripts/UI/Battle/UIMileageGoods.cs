using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Common.Util;

public class UIMileageGoods : MonoBehaviour
{
    public Image GoodsCardImage;
    public Text GoodsCount;


    public void InitMileageGoodsCard(eGoodsType type, int nCount = 1)
    {
        GoodsCardImage.sprite = TextureManager.GetGoodsTypeSprite(type);

        if (nCount <= 0)
            GoodsCount.gameObject.SetActive(false);
        else
        {
            GoodsCount.gameObject.SetActive(true);
            GoodsCount.text = Languages.GetNumberComma(nCount);
        }
    }

}
