using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class GoodsEndPosition
{
    public Goods_Type m_eGoodsType;
    public Vector3 m_vecGoodsPosition;
}

[System.Serializable]
public class GoodsRewardAnimationData
{
    public Goods_Type       m_eRewardGoodsType;

    public bool m_bUseBaseEndPosition = true;
    public float m_fEndXPos = 0.0f;
    public float m_fEndYPos = 0.0f;
}

public class UIGoodsRewardAnimation : MonoBehaviour 
{
    public  List<GoodsEndPosition>              m_listGoodsEndPosition;

    public  UIGoodsRewardAnimationObject        m_goodsRewardObejct;
    private List<UIGoodsRewardAnimationObject>  m_listGoodsRewardAnimObj = new List<UIGoodsRewardAnimationObject>();

    public Transform m_ParentTrans;
    private bool     m_bAllAnimEnd;

    //** 보상애니메이션 오브젝트 생성 및 세팅
    public void Setting(Vector3 startPos, List<GoodsRewardAnimationData> listGoodsAnimData)
    {
        if(listGoodsAnimData == null)
            return;

        for(int i = 0; i < listGoodsAnimData.Count; i++)
        {
            UIGoodsRewardAnimationObject newRewardObject = Instantiate<UIGoodsRewardAnimationObject>(m_goodsRewardObejct);
            UIUtility.SetParent(newRewardObject.transform, m_ParentTrans);
            m_listGoodsRewardAnimObj.Add(newRewardObject);

            GoodsRewardAnimationData animData = listGoodsAnimData[i];

            GoodsEndPosition findTypePos = null;
            if (animData.m_bUseBaseEndPosition)
            {
                findTypePos = m_listGoodsEndPosition.Find(item => item.m_eGoodsType == animData.m_eRewardGoodsType);
            }
            else
            {
                findTypePos = new GoodsEndPosition();
                findTypePos.m_vecGoodsPosition = new Vector3(animData.m_fEndXPos, animData.m_fEndYPos);
            }
            newRewardObject.Setting(startPos, animData.m_eRewardGoodsType, findTypePos, GetAnimationName(i));

            m_listGoodsRewardAnimObj.Add(newRewardObject);
        }

        CompletAnim(false);
        AllAnimStart();
    }

    //** 모든 애니메이션 시작
    public void AllAnimStart()
    {
        for (int i = 0; i < m_listGoodsRewardAnimObj.Count; i++)
        {
            UIGoodsRewardAnimationObject animObejct = m_listGoodsRewardAnimObj[i];

            animObejct.StartAnim();
        }

        StartCoroutine(CheckAllAnimEnd());
    }

    //** 모든 오브젝트들의 애니메이션이 끝났는지 체크
    public IEnumerator CheckAllAnimEnd()
    {
        bool allNotComplet = true;

        while (allNotComplet)
        {
            allNotComplet = false;

            for(int i = 0; i < m_listGoodsRewardAnimObj.Count; i++)
            {
                UIGoodsRewardAnimationObject animObejct = m_listGoodsRewardAnimObj[i];

                if (!animObejct.m_bAnimationEnd)
                    allNotComplet = true;
            }
            yield return null;
        }

        AllAnimEnd();   
    }

    //** 모든 오브젝트들이 애니메이션이 끝나고 난 후
    public void AllAnimEnd()
    {
        for (int i = 0; i < m_listGoodsRewardAnimObj.Count; i++)
            Destroy(m_listGoodsRewardAnimObj[i].gameObject);

        m_listGoodsRewardAnimObj.Clear();

        CompletAnim(true);
    }

    private void CompletAnim(bool isComplet)
    {
        StopTouch(!isComplet);
        m_bAllAnimEnd = isComplet;
        this.gameObject.SetActive(!isComplet);
    }

    //** 화면 프리징!
    private void StopTouch(bool stop)
    {
        CanvasManager.Instance.GetAllCanvasBlock(stop);
    }

    private string GetAnimationName(int count)
    {
        switch(count)
        {
            case 0: return "GoodsRewardAnim_00";
            case 1: return "GoodsRewardAnim_01";
            case 2: return "GoodsRewardAnim_02";
            case 3: return "GoodsRewardAnim_03";
            default: return "GoodsRewardAnim_00";
        }
    }
}
