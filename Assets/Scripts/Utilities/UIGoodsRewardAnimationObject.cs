using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGoodsRewardAnimationObject : MonoBehaviour
{
    private const float F_GOODS_NORMAL_SIZE = 1.0f;
    private const float F_GOODS_MAX_SIZE_   = 1.5f;
    private const float F_CARD_NORMAL_SIZE  = 0.36f;
    private const float F_CARD_MAX_SIZE     = 0.5f;
    private const float F_EXP_NORMAL_SIZE   = 0.5f;
    private const float F_EXP_MAX_SIZE      = 1.0f;

    private const float F_ANIMATION_SPEED   = 5.0f;

    public GameObject   m_EffectObject;

    public Image        m_RewardImage;
    public Animation    m_RewardAnim;

    private Transform   m_trsImage;
    private Transform   m_trsReward;
    private Vector3     m_SelectGoodsEndPosition;

    private string      m_AnimName;
    public  bool        m_bAnimationEnd;

    //** 기본 세팅
    public void Setting(Vector3 pos, Goods_Type goodsType, GoodsEndPosition goodsData, string animName)
    {
        if (m_trsReward == null)
            m_trsReward = GetComponent<Transform>();

        if (m_trsImage == null)
            m_trsImage = m_RewardImage.GetComponent<Transform>();

        m_trsReward.position = new Vector3(pos.x, pos.y, pos.z);
        m_trsReward.localScale = Vector3.one;
        m_RewardImage.sprite = TextureManager.GetGoodsTypeSprite(goodsType);
        m_RewardImage.SetNativeSize();

        m_AnimName = animName;

        float goodsSize = GetGoodsSize(goodsType, false);
        m_trsImage.localScale = new Vector3(goodsSize, goodsSize, 1.0f);

        if (goodsData == null)
        {
            Debug.LogError(string.Format("[UIFranchiseRewardAnimation] Setting : findTypePos(GoodsEndPosition : {0} type) is Null", goodsType));
            return;
        }
        m_SelectGoodsEndPosition = goodsData.m_vecGoodsPosition;
    }

    public void StartAnim()
    {
        CompletAnimation(false);
        StartCoroutine(CheckAnimComplet());
    }

    //** 애니메이션 완료 체크
    private IEnumerator CheckAnimComplet()
    {
        m_RewardAnim.Play(m_AnimName);

        Kernel.soundManager.PlayUISound(SOUND.SND_UI_FRANCHISE_GETITEM);

        while (m_RewardAnim.isPlaying)
            yield return null;

        StartCoroutine(StartAnimation());
    }

    //** 보상 애니메이션 스타트!!!!
    private IEnumerator StartAnimation()
    {
        Vector3 currentPosition = m_trsReward.localPosition;
        Vector2 endPosition = m_SelectGoodsEndPosition;

        Vector3 currentImgPosition = m_trsImage.localPosition;
        Vector3 endImagePosition = Vector3.zero;

        float m_fElapsedTime = 0.0f;
        float fValue = 0.0f;

        float fnextXPos = 0.0f;
        float fnextYPos = 0.0f;

        float fImgNextXPos = 0.0f;
        float fImgNextYPos = 0.0f;

        while (currentPosition.x != endPosition.x || currentPosition.y != endPosition.y)
        {
            m_fElapsedTime += Time.deltaTime;

            if (m_fElapsedTime > F_ANIMATION_SPEED)
                m_fElapsedTime = F_ANIMATION_SPEED;

            fValue = m_fElapsedTime / F_ANIMATION_SPEED;

            fnextXPos = EaseOutCubic(currentPosition.x, endPosition.x, fValue);
            fnextYPos = EaseOutCubic(currentPosition.y, endPosition.y, fValue);

            fImgNextXPos = EaseOutCubic(currentImgPosition.x, endImagePosition.x, fValue);
            fImgNextYPos = EaseOutCubic(currentImgPosition.y, endImagePosition.y, fValue);

            m_trsReward.localPosition = new Vector3(fnextXPos, fnextYPos, 1);
            m_trsImage.localPosition = new Vector3(fImgNextXPos, fImgNextYPos, 1);

            currentPosition = m_trsReward.localPosition;
            currentImgPosition = m_trsImage.localPosition;

            yield return null;
        }
        m_trsReward.localPosition = new Vector3(endPosition.x, endPosition.y, 1);
        m_trsImage.localPosition = new Vector3(endImagePosition.x, endImagePosition.y, 1);

        CompletAnimation(true);
    }

    //** 완료되었는가? 에 대한 세팅
    private void CompletAnimation(bool complet)
    {
        m_trsReward.gameObject.SetActive(!complet);
        m_EffectObject.SetActive(!complet);
        m_bAnimationEnd = complet;
    }

    //** 이동 에니메이션
    private float EaseOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    //** Goods 사이즈 구하기
    public float GetGoodsSize(Goods_Type m_eRevGoodsType, bool maxSize)
    {
        if ( //골드, 루비, 하트, 길드 포인트, 복수 포인트, 랭킹 포인트, 별, 트레이닝 포인트, 스마일 포인트, 친구 포인트
            m_eRevGoodsType == Goods_Type.Gold || m_eRevGoodsType == Goods_Type.Ruby || m_eRevGoodsType == Goods_Type.Heart
            || m_eRevGoodsType == Goods_Type.GuildPoint || m_eRevGoodsType == Goods_Type.RevengePoint || m_eRevGoodsType == Goods_Type.RankingPoint
            || m_eRevGoodsType == Goods_Type.StarPoint || m_eRevGoodsType == Goods_Type.TrainingPoint || m_eRevGoodsType == Goods_Type.SmilePoint || m_eRevGoodsType == Goods_Type.FriendPoint
            || m_eRevGoodsType == Goods_Type.SweepTicket || m_eRevGoodsType == Goods_Type.GuildExp
            )
        {
            return maxSize ? F_GOODS_MAX_SIZE_ : F_GOODS_NORMAL_SIZE;
        }
        else if(m_eRevGoodsType == Goods_Type.AccountExp)
        {
            return maxSize ? F_EXP_MAX_SIZE : F_EXP_NORMAL_SIZE;
        }
        else // 나머지 카드형태
        {
            return maxSize ? F_CARD_MAX_SIZE : F_CARD_NORMAL_SIZE;
        }
    }
}
