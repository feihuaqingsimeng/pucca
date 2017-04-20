using UnityEngine;
using UnityEngine.UI;
using Common.Packet;


public class UIMileageDirector : UIObject
{
    public UIMileageGoods       m_RewardCard;
    public GameObject           m_FX;

    //이펙트.
    public RectTransform        ShineEffect;
    private float               fRotateZ;

    //애니메이션.
    public Animation            MileageDirectorAnimation;

    private bool                TouchActive;
    bool m_Clicked;



    protected override void OnEnable()
    {
        MileageDirectorAnimation.Stop();
        MileageDirectorAnimation.Play("AniMileageDirector");

        m_RewardCard.gameObject.SetActive(false);
        m_FX.SetActive(false);
        TouchActive = false;
        Invoke("ShowRewardCard", 2.2f);
    }

    protected override void OnDisable()
    {
        m_RewardCard.gameObject.SetActive(false);
        m_FX.SetActive(false);
    }





    protected override void Update()
    {
        base.Update();

        if (ShineEffect.gameObject.activeInHierarchy)
        {
            fRotateZ -= 20.0f * Time.deltaTime;
            if (fRotateZ <= 360.0f)
                fRotateZ += 360.0f;
            ShineEffect.localRotation = Quaternion.Euler(0.0f, 0.0f, fRotateZ);
        }


        if (!TouchActive)
            return;

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
                Kernel.uiManager.Close(UI.MileageDirector);
            }
        }
    }





    private void ShowRewardCard()
    {
        m_RewardCard.gameObject.SetActive(true);
        m_FX.SetActive(true);
        Kernel.soundManager.PlayUISound(SOUND.SND_UI_PVP_RESULT_WINPOINT_REWARD);

        Invoke("SetTouchActive", 0.5f);
    }


    private void SetTouchActive()
    {
        TouchActive = true;
    }




    public CReceivedGoods RecvGoods
    {
        set
        {
            if (m_RewardCard != null)
            {
                m_RewardCard.InitMileageGoodsCard(value.m_eGoodsType, value.m_iReceivedAmount);
                //업적이 있나??
            }
        }
    }
}
