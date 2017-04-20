using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICouponCheck : UIObject 
{
    //** Text
    public Text m_Title_Text;
    public Text m_Dec;
    public Text m_Ok_Text;
    public Text m_Cancle_Text;

    //** Text Field
    public InputField m_CouponField;

    //** Button
    public Button m_Ok_Button;

    protected override void Awake()
    {
        base.Awake();

        SetUIInit();
    }

    protected override void OnCloseButtonClick()
    {
        // 초기화
        m_CouponField.text = "";

        base.OnCloseButtonClick();
    }

    //** UI Init
    public void SetUIInit()
    {
        // 텍스트 입력
        m_Title_Text.text   = Languages.ToString(TEXT_UI.COUPON_INPUT);
        m_Dec.text          = Languages.ToString(TEXT_UI.COUPON_INPUT_TERM);
        m_Ok_Text.text      = Languages.ToString(TEXT_UI.OK);
        m_Cancle_Text.text  = Languages.ToString(TEXT_UI.CANCEL);

        m_Ok_Button.onClick.AddListener(OnClickOKButton);
    }

    //** 확인 버튼 클릭시
    public void OnClickOKButton()
    {

    }
}
