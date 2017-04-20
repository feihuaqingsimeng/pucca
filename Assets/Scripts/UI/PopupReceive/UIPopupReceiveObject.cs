using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIPopupReceiveObject : MonoBehaviour 
{
    public Image    m_RewardIcon;
    public Text     m_RewardCount;

    //** UI 세팅
    public void SetUI(Goods_Type goodsType, int count)
    {
        if (m_RewardIcon != null)
            m_RewardIcon.sprite = TextureManager.GetGoodsTypeSprite(goodsType);

        //기획팀 스트링 작업 필요
        if (m_RewardCount != null)
            m_RewardCount.text = Languages.ToString(TEXT_UI.GOODS_RECEIVE, Languages.ToString(goodsType), count);
    }
}
