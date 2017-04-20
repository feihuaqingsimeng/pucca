using UnityEngine;
using System.Collections;

//** 가맹점 방 안에서의 캐릭터 움직임.
public class FranchiseCharAI : MonoBehaviour 
{
    private const float F_SPACE_ROOM_SIZE   = 50.0f;    // 방의 양옆의 공간
    private const float F_CHAR_SIZE         = 80.0f;   // 캐릭터 사이즈
    private const float F_MOVE_SPEED        = 5.0f;     // 움직임 속도
    private const float F_WAIT_SPEED        = 0.05f;    // 휴식 속도
    private const float F_MIN_WAIT_TIME     = 5.0f;     // 기다림의 최저 시간
    private const float F_MAX_WAIT_TIME     = 10.0f;    // 기다림의 최고 시간
    private const float F_MIN_RUN_RANGE     = 50.0f;    // 달릴 수 있는 최소 거리

    private SkeletonAnimation m_skeletonAnim;
    public SkeletonAnimation SkeletonAnim
    {
        get { return m_skeletonAnim; }
        set { m_skeletonAnim = value; }
    }

    private RectTransform       m_rtrsChar;

    private float m_moveRange;      //** 움직일 수 있는 절반 사이즈
    private float m_roomWidth;      //** 방 사이즈 ( 양옆 공간을 제외시킨)
    public  float RoomWidth
    {
        get { return m_roomWidth; }
        set 
        {
            m_roomWidth = value - (F_SPACE_ROOM_SIZE * 2);
            m_moveRange = m_roomWidth * 0.5f;
        }
    }

    //** 현재 애니 번호
    private int m_nCurrentAnimNum;

    public void OnEnable()
    {
        if (SkeletonAnim == null)
            return;
    }

    //** 기본 세팅
    public void Setting(int charIndex, float withSize)
    {
        if (SkeletonAnim == null)
            SkeletonAnim = GetComponent<SkeletonAnimation>();

        if (m_rtrsChar == null)
            m_rtrsChar = GetComponent<RectTransform>();

        RoomWidth = withSize;

        SetSkeletonCharater(charIndex);

        StartNextAnimation();
    }

    //** 캐릭터 스파인 세팅
    private bool SetSkeletonCharater(int charIndex)
    {
        if (SkeletonAnim == null)
            return false;

        if (charIndex > 0)
        {
            DB_Card.Schema charData = DB_Card.Query(DB_Card.Field.Index, charIndex);

            if (charData == null)
            {
                Debug.LogError(string.Format("[UIFranchiseRoom] SetSkeletonCharater : charData(DB_Card index = {0}) is Null or Zero", charIndex));
                return false;
            }

            string identificationName = charData.IdentificationName;
            string assetPath = "Spines/Character/" + identificationName + "/" + identificationName + "_SkeletonData";
            SkeletonDataAsset skeletonDataAsset = Resources.Load<SkeletonDataAsset>(assetPath);
            if (skeletonDataAsset != null)
            {
                SkeletonAnim.skeletonDataAsset = skeletonDataAsset;
                SkeletonAnim.initialSkinName = identificationName;
                CharAnimSetting("wait");

                return true;
            }
            else
            {
                Debug.LogError(assetPath);
            }
        }

        return false;
    }

    //** 캐릭터 애니메이션 세팅
    private void CharAnimSetting(string animName)
    {
        SkeletonAnim.AnimationName = animName;
        SkeletonAnim.loop = true;
        SkeletonAnim.Reset();
    }

    //** 애니메이션
    private void StartNextAnimation()
    {
        string animationName = GetAnimationName();

        if (SkeletonAnim.AnimationName != animationName)
            CharAnimSetting(animationName);

        if (animationName.Equals("run"))
            StartCoroutine(MoveAnimation(RandomPosition()));
        else
            StartCoroutine(WaitAnimation(RandomNumber(F_MIN_WAIT_TIME, F_MAX_WAIT_TIME)));
    }

    //** 랜덤 번호에 따른 애니메이션 이름
    private string GetAnimationName()
    {
        // 달리기 다음은 무조건 휴식임.
        if(m_nCurrentAnimNum == 1)
            m_nCurrentAnimNum = Random.Range(2, 4);
        else
            m_nCurrentAnimNum = Random.Range(1, 4);

        switch (m_nCurrentAnimNum)
        {
            case 1: return "run";
            case 2: return "wait";
            case 3: return "pose";
            default: return "wait";
        }
    }

    //** 랜덤 값 반환
    private float RandomNumber(float min, float max)
    {
        return Random.Range(min, max);
    }

    //** 랜덤 위치 반환
    private float RandomPosition()
    {
        float ranmdomRange = 0;
        float distance = 0;

        while (distance < F_MIN_RUN_RANGE)
        {
            ranmdomRange = RandomNumber(-m_moveRange, m_moveRange);
            distance = Mathf.Abs(m_rtrsChar.anchoredPosition.x - ranmdomRange);
        }

        return ranmdomRange;
    }

    //** 움직이는 애니메이션 (run)
    private IEnumerator MoveAnimation(float movePosition)
    {
        float currentCharXPosition = m_rtrsChar.anchoredPosition.x;

        float fnextXPos = 0.0f;

        bool isRight = (currentCharXPosition - movePosition) < 0;
        float size = isRight ? -F_CHAR_SIZE : F_CHAR_SIZE;

        m_rtrsChar.transform.localScale = new Vector3(size, F_CHAR_SIZE, 1.0f);

        while (isRight ? currentCharXPosition < movePosition : currentCharXPosition > movePosition)
        {
            fnextXPos = isRight ? m_rtrsChar.anchoredPosition.x + F_MOVE_SPEED : m_rtrsChar.anchoredPosition.x - F_MOVE_SPEED;
            m_rtrsChar.anchoredPosition = new Vector2(fnextXPos, m_rtrsChar.anchoredPosition.y);
            currentCharXPosition = m_rtrsChar.anchoredPosition.x;

            yield return null;
        }

        m_rtrsChar.anchoredPosition = new Vector2(movePosition, m_rtrsChar.anchoredPosition.y);

        StartNextAnimation();
    }

    // 기다리는 애니메이션 (pose, wait)
    private IEnumerator WaitAnimation(float waitTime)
    {
        float currentTime = 0.0f;

        while (currentTime < waitTime)
        {
            currentTime += F_WAIT_SPEED;
            yield return null;
        }

        StartNextAnimation();
    }
}
