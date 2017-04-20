using UnityEngine;
using System.Collections;
using System;

public class Character_Controller : MonoBehaviour
{
    public delegate void OnDelegate(Collision2D other);
    public OnDelegate pOnDelegate = null;

    public int CharID = 0;
    private int CloneCharID = 0;

    public SkeletonAnimation skeletonAnimation;

    public string AniKey = "";
    private string CloneAniKey = "";
    public string PathName = "Spine/Character/";
    Vector3 CharScale = Vector3.zero;

    // Animation 처리
    public bool IsAniamtion = false;
    float PosX = 0;
    float InitPosX = 0;

    float CharSpeed = 0.0f;
    float moveTime = 0.0f;
    float currentMoveTime = 0.0f;
    float changeValue = 0.0f;
    public float Distance = 0.0f;

    public SpriteRenderer ElementSprite1 = null;
    public SpriteRenderer ElementSprite2 = null;

    void Awake()
    {
        if (skeletonAnimation == null)
        {
            skeletonAnimation = gameObject.GetComponentInChildren<SkeletonAnimation>();
            if (CharScale == Vector3.zero)
                CharScale = skeletonAnimation.gameObject.transform.parent.gameObject.transform.localScale;
        }

        SpriteRenderer[] pSpriteRendererList = gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer pSpriteRenderer in pSpriteRendererList)
        {
            if (pSpriteRenderer.name == "ElementSprite1")
            {
                ElementSprite1 = pSpriteRenderer;
                ElementSprite1.gameObject.SetActive(false);
            }
            else if (pSpriteRenderer.name == "ElementSprite2")
            {
                ElementSprite2 = pSpriteRenderer;
                ElementSprite2.gameObject.SetActive(false);
            }
        }
    }

    void Start()
    {
    }

    public bool IsAniKey(string AniKey)
    {
        if (AniKey == "wait" ||
            AniKey == "attack1" ||
            AniKey == "die" ||
            AniKey == "knock_back" ||
            AniKey == "hit" ||
            AniKey == "run" ||
            AniKey == "win")
            return true;
        return false;
    }
    void OnEventListener(Spine.AnimationState state, int trackIndex, Spine.Event e)
    {
        Debug.Log(trackIndex + " " + ": Event ");
    }
    public void OnStartListener(Spine.AnimationState state, int trackIndex)
    {
        //Debug.Log(trackIndex + " " + ": start ");
    }

    public void OnEndListener(Spine.AnimationState state, int trackIndex)
    {
        //Debug.Log(trackIndex + " " + ": end");
    }

    public void OnCompleteDelegate(Spine.AnimationState state, int trackIndex, int loopCount)
    {
        //Debug.Log(trackIndex + " " + ": complete [" + loopCount + " loops]");
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (pOnDelegate != null)
            pOnDelegate(other);
    }

    public void SetInfor(int CharID)
    {
        this.CharID = CharID;
        CloneCharID = CharID;

        if (skeletonAnimation == null)
            return;

        if (ElementSprite1 != null)
            ElementSprite1.gameObject.SetActive(false);
        if (ElementSprite2 != null)
            ElementSprite2.gameObject.SetActive(false);

        string CharSpineName = "";
        string CharName = "";
        float CharSpineSize = 1.0f;
        /*
        PC_Base.Schema CharData = DataManager.GetInstance().GetPCBaseData(CharID);
        if (CharData == null)
        {
            MOB_Base.Schema pMOBBaseData = DataManager.GetInstance().GetMOBBaseData(CharID);
            if (pMOBBaseData == null)
            {
                skeletonAnimation.gameObject.SetActive(false);

                if (gameObject.transform.FindChild("Shadow") != null)
                    gameObject.transform.FindChild("Shadow").gameObject.SetActive(false);
                return;
            }
            else
            {
                skeletonAnimation.gameObject.SetActive(true);
                if (gameObject.transform.FindChild("Shadow") != null)
                    gameObject.transform.FindChild("Shadow").gameObject.SetActive(true);

                Char_View.Schema pCharViewData = DataManager.GetInstance().GetCharViewData_Direct(pMOBBaseData.Char_View_ID_Link);

                CharSpineName = pCharViewData.SpineData_Name;
                CharName = pCharViewData.IdentificationName;
                if (pMOBBaseData.SizeRate != 0)
                    CharSpineSize = pMOBBaseData.SizeRate;

                // SpineCharBaseLoby 에서만 사용
                if (ElementSprite1 != null)
                {
                    if (pMOBBaseData.ElementType1 != ElementType1.ElementType_None)
                    {
                        ElementSprite1.gameObject.SetActive(true);
                        ElementSprite1.sprite = TextureManager.GetSprite(SpritePackingTag.Element, StringHelper.GetElementType(pMOBBaseData.ElementType1)); //UIHelper.LoadSprite(StringHelper.GetElementType(pMOBBaseData.ElementType1));
                    }
                }

                if (ElementSprite2 != null)
                {
                    if (MOBBaseData.ElementType2 != eElementType.None)
                    {
                        ElementSprite2.gameObject.SetActive(true);
                        ElementSprite2.sprite = UIHelper.LoadSprite(StringHelper.GetElementType(MOBBaseData.ElementType2));
                    }
                }
            }
        }
        else
        {
            skeletonAnimation.gameObject.SetActive(true);

            if(gameObject.transform.FindChild("Shadow") != null)
                gameObject.transform.FindChild("Shadow").gameObject.SetActive(true);

            Char_View.Schema pCharViewData = DataManager.GetInstance().GetCharViewData(CharID);

            CharSpineName = pCharViewData.SpineData_Name;
            CharName = pCharViewData.IdentificationName;
            if (CharData.SizeRate != 0)
                CharSpineSize = (float)CharData.SizeRate;
        }
        */
        try
        {
            skeletonAnimation.skeletonDataAsset = (SkeletonDataAsset)Resources.Load(PathName + CharSpineName);
        }
        catch (Exception error)
        {
            Debug.LogWarning(error);
            Debug.LogError("File Not Find : " + CharSpineName);
        }
        //        skeletonAnimation.skeletonDataAsset.scale = CharSpineSize;
        skeletonAnimation.initialSkinName = CharName;
        //skeletonAnimation.gameObject.transform.parent.gameObject.transform.localScale = new Vector3(CharScale.x * CharSpineSize, CharScale.y * CharSpineSize, CharScale.y * CharSpineSize);

        Canvas pCanvas = gameObject.transform.root.GetComponent<Canvas>();
        if (pCanvas != null)
        {
            MeshRenderer pMeshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();
            if (pMeshRenderer != null)
                pMeshRenderer.sortingOrder = pCanvas.sortingOrder + 2;

            SpriteRenderer pSpriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
            if (pSpriteRenderer != null)
                pSpriteRenderer.sortingOrder = pCanvas.sortingOrder + 1;
        }

        skeletonAnimation.Reset();
        //skeletonAnimation.state.SetAnimation(0, "run", true);

        skeletonAnimation.state.Start += OnStartListener;
        skeletonAnimation.state.End += OnEndListener;
        skeletonAnimation.state.Event += OnEventListener;
        skeletonAnimation.state.Complete += OnCompleteDelegate;
    }

    public void SetCharAnimation(string AniKey, bool Loop = true)
    {
        if (CharID == 0)
            return;

        if (skeletonAnimation == null)
            SetInfor(CharID);

        if (IsAniKey(AniKey) == true)
        {
            this.AniKey = AniKey;
            CloneAniKey = AniKey;
            skeletonAnimation.Reset();
            skeletonAnimation.state.SetAnimation(0, AniKey, Loop);
        }
    }

    public void SetMovePosion(float PosX, float moveTime)
    {
        InitPosX = gameObject.transform.localPosition.x;
        this.PosX = PosX;
        this.moveTime = moveTime;
        IsAniamtion = true;
        currentMoveTime = 0;
    }

    public void SetSpeed(float Speed)
    {
        CharSpeed = Speed;
        Distance = gameObject.transform.localPosition.x;
        IsAniamtion = true;
        currentMoveTime = 0;
    }

    void FixedUpdate()
    //void LateUpdate()
    {
        if (CloneCharID != CharID)
            SetInfor(CharID);

        if (CloneAniKey != AniKey)
            SetCharAnimation(AniKey);

        if (IsAniamtion == true)
        {
            currentMoveTime += Time.deltaTime;

            if (CharSpeed <= 0)
            {
                if (currentMoveTime > moveTime)
                    currentMoveTime = moveTime;

                if (currentMoveTime >= moveTime)
                {
                    IsAniamtion = false;

                    if (AniKey == "die")
                        return;
                    else if (AniKey == "run")
                        SetCharAnimation("wait", true);
                    return;
                }

                if (AniKey == "run" &&
                    currentMoveTime != 0.0f && moveTime != 0.0f)
                {
                    changeValue = currentMoveTime / moveTime;
                    //changeValue = Mathf.Sin(changeValue * Mathf.PI * 0.5f);
                    Distance = Mathf.Lerp(InitPosX, PosX, changeValue);

                    transform.localPosition = new Vector3(Distance, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
                }
            }
            else
            {
                if (AniKey == "run")
                {
                    Distance = Distance + (Time.deltaTime * CharSpeed);
                    transform.localPosition = new Vector3(Distance, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
                }
            }
        }
    }
}
