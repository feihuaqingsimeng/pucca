using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Reflection;
using System;

public class OptionGameInfo : MonoBehaviour 
{
    //** Text
    public Text     m_GameVersion_Text;
    public Text     m_AccountNumber_Text;
    public Text     m_AccountCopy_Text;
    public Text     m_QA_Text;
    public Text     m_Community_Text;
    public Text     m_LinkAccountCheck_Text;
    public Text     m_Agreement_Text;
    public Text     m_Coupon_Text;
    public Text     m_LinkAccount_Text;
    public Text     m_DropOutAccount_Text;
    public Text     m_LogOut_Text;
    public Text     m_MakeSupport_Text;

    //** TextField
    public Text     m_GameVersionValue_Text;
    public Text     m_AccountNumberValue_Text;

    //** Button
    public Button   m_Copy_Button;
    public Button   m_QA_Button;
    public Button   m_Community_Button;
    public Button   m_LinkAccountCheck_Button;
    public Button   m_Agreement_Button;
    public Button   m_Coupon_Button;
    public Button   m_LinkAccount_Button;
    public Button   m_DropOutAccount_Button;
    public Button   m_LogOut_Button;
    public Button   m_MakeSupport_Button;

    //** Copy
    

    //** 계정 연동 콜백 함수 연결
    public void LinkAccountLinkFuc()
    {
        Kernel.facebookManager.onLogIn          += OnFacebookLink;
        Kernel.facebookManager.onLogOut         += OnLickBreakAccount;
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        Kernel.gpgsManager.onSignIn             += OnGPGSLink;
        Kernel.gpgsManager.onSignOut            += OnLickBreakAccount;
#endif
        Kernel.gamecenterManager.onSignIn       += OnGameCenterLink;
        Kernel.entry.account.onLinkLogInResult  += OnLickAccountResult;
    }

    //** 계정 연동 콜백 함수 해제
    public void ClearAccountLinkFuc()
    {
        Kernel.facebookManager.onLogIn          -= OnFacebookLink;
        Kernel.facebookManager.onLogOut         -= OnLickBreakAccount;
#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
        Kernel.gpgsManager.onSignIn             -= OnGPGSLink;
        Kernel.gpgsManager.onSignOut            -= OnLickBreakAccount;
#endif
        Kernel.gamecenterManager.onSignIn       -= OnGameCenterLink;
        Kernel.entry.account.onLinkLogInResult  -= OnLickAccountResult;
    }

    //** UI Init
    public void SetUIInit()
    {
        // 텍스트 입력
        m_GameVersion_Text.text         = Languages.ToString(TEXT_UI.GAME_VERSION);
        m_AccountNumber_Text.text       = Languages.ToString(TEXT_UI.MEMBER_NUMBER);
        m_AccountCopy_Text.text         = Languages.ToString(TEXT_UI.COPY);
        m_QA_Text.text                  = Languages.ToString(TEXT_UI.CUSTOMER_QUESTION);
        m_Community_Text.text           = Languages.ToString(TEXT_UI.COMMUNITY);
        m_LinkAccountCheck_Text.text    = Languages.ToString(TEXT_UI.LINKED_ACCOUNT_CHECK);
        m_Agreement_Text.text           = Languages.ToString(TEXT_UI.AGREEMENT);
        m_Coupon_Text.text              = Languages.ToString(TEXT_UI.COUPON_INPUT);
        m_LinkAccount_Text.text         = Languages.ToString(TEXT_UI.ACCOUNT_LINK);
        m_DropOutAccount_Text.text      = Languages.ToString(TEXT_UI.ACCOUNT_ESCAPE);
        m_LogOut_Text.text              = Languages.ToString(TEXT_UI.LOG_OUT);
        m_MakeSupport_Text.text         = Languages.ToString(TEXT_UI.MAKING_SUPPORT);

        m_GameVersionValue_Text.text = Kernel.GetVersionText();
        m_AccountNumberValue_Text.text = Kernel.uid;

        // 버튼 연결
        m_Copy_Button.onClick.AddListener(OnClickCopyButton);
        m_QA_Button.onClick.AddListener(OnClickQAButton);
        m_Community_Button.onClick.AddListener(OnClickCommunityButton);
        m_LinkAccountCheck_Button.onClick.AddListener(OnClickLinkAccountCheckButton);
        m_Agreement_Button.onClick.AddListener(OnClickAgreementButton);
        m_Coupon_Button.onClick.AddListener(OnClickCouponButton);
        m_LinkAccount_Button.onClick.AddListener(OnClickLinkAccountButton);
        m_DropOutAccount_Button.onClick.AddListener(OnClickDropOutAccountButton);
        m_LogOut_Button.onClick.AddListener(OnClickLogOutButton);
        m_MakeSupport_Button.onClick.AddListener(OnClickMakeSupportButton);

        SetUIActiveInit();
    }

    //** Active / EnActive UI Init
    public void SetUIActiveInit()
    {
        bool isGuest = Kernel.entry.account.mainLoginType == Common.Util.eLoginType.Guest && Kernel.entry.account.subLoginType == Common.Util.eLoginType.None;

        // 게스트 => 계정연동(0), 계정탈퇴(x), 로그아웃(x)
        // 구글 등.. => 계정연동(x), 계정탈퇴(o), 로그아웃(o)
        m_LinkAccount_Button.gameObject.SetActive(isGuest);
        m_LogOut_Button.gameObject.SetActive(!isGuest);
        m_DropOutAccount_Button.gameObject.SetActive(!isGuest);
    }

    //** 복사 버튼 선택
    private void OnClickCopyButton()
    {
        // Editor 또는 PC 용 Copy
#if UNITY_EDITOR
        TextEditor te = new TextEditor();
        te.text = m_AccountNumberValue_Text.text;
        te.SelectAll();
        te.Copy();
#endif

        //Android용 Copy ClipBoard
#if UNITY_ANDROID && !UNITY_EDITOR
        CopyClipBoard(m_AccountNumberValue_Text.text);
#endif

        //IPone용 Copy ClipBoard
#if UNITY_IPHONE && !UNITY_EDITOR

#endif
        UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.COPY_FINISHED));
    }
    
    public static void CopyClipBoard(string copyStr)
    {
        AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");

        activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaObject clipboardManager = activity.Call<AndroidJavaObject>("getSystemService", "clipboard");
            //clipboardManager.Call("setText", exportData);
            AndroidJavaClass clipDataClass = new AndroidJavaClass("android.content.ClipData");
            AndroidJavaObject clipData = clipDataClass.CallStatic<AndroidJavaObject>("newPlainText", "simple text", copyStr);
            clipboardManager.Call("setPrimaryClip", clipData);
        }));
    }

    //** 고객문의 버튼 선택
    private void OnClickQAButton()
    {
        string email = "cs_puccawars_aos@mseedgames.com";

#if UNITY_IOS && !UNITY_EDITOR
        email = "cs_puccawars_ios@mseedgames.com";
#endif
        string subject = EscapeURL(" ");
        string body = EscapeURL
            (
            string.Format("1. {0} : \n2. {1} : ", Languages.ToString(TEXT_UI.CHARACTER_NAME), Languages.ToString(TEXT_UI.QUESTION_CONTENTS))
            );
       
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    //** 커뮤니티 버튼 선택
    private void OnClickCommunityButton()
    {
        if(Kernel.languageCode == LanguageCode.Korean)
            Application.OpenURL("http://cafe.naver.com/puccawars");     //뿌까 카페 주소
        else
            Application.OpenURL("https://www.facebook.com/puccawars");  //뿌까 페이스북 주소
    }

    //** 연동계정 확인 버튼 선택
    private void OnClickLinkAccountCheckButton()
    {
        TEXT_UI baseText = TEXT_UI.NONE;
        string strDec = "";

        if (Kernel.entry.account.mainLoginType == Common.Util.eLoginType.Guest && Kernel.entry.account.subLoginType == Common.Util.eLoginType.None)
            baseText = TEXT_UI.ACCOUNT_CHECK_UNLINKED;
        else
            baseText = TEXT_UI.ACCOUNT_CHECK_LINKED;

        Common.Util.eLoginType currentType = Common.Util.eLoginType.None;

        if (Kernel.entry.account.subLoginType != Common.Util.eLoginType.None)
            currentType = Kernel.entry.account.subLoginType;
        else
            currentType = Kernel.entry.account.mainLoginType;

        strDec = Languages.ToString(baseText, Languages.ToString(currentType));
        UIAlerter.Alert(strDec, UIAlerter.Composition.Confirm, null, Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }

    //** 이용약관 버튼 선택
    private void OnClickAgreementButton()
    {
        Application.OpenURL("http://eula.db.mseedgames.co.kr/pucca/PuccaGlobalClause.html");
    }

    //** 쿠폰입력 버튼 선택
    private void OnClickCouponButton()
    {
        UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.NOTICE_PREPARE));
        return;

        //if (Kernel.uiManager)
        //{
        //    UICouponCheck couponCheck = Kernel.uiManager.Get<UICouponCheck>(UI.CouponCheck, true, false);

        //    if (couponCheck)
        //        Kernel.uiManager.Open(UI.CouponCheck);
        //}
    }

    //** 계정연동 버튼 선택
    private void OnClickLinkAccountButton()
    {
        UIAccountInterconnector accountInter = UIManager.Instance.Get<UIAccountInterconnector>(UI.AccountInterconnector, true, false);

        if (accountInter == null)
        {
            Debug.LogError("[OptionGameInfo] OnClickLinkAccountButton : UIAccountInterconnector is null");
            return;
        }

        UIManager.Instance.Open(UI.AccountInterconnector);
    }

    //** 구글 계정연동 콜백 함수
    private void OnGPGSLink(string id, string serverAuthCode)
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_ACCOUNT_LINK_SYN(Common.Util.eLoginType.Google, Kernel.uid, string.Empty, serverAuthCode);
    }

    //** 페이스 북 계정연동 콜백 함수
    private void OnFacebookLink(string userId, string accessToken)
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_ACCOUNT_LINK_SYN(Common.Util.eLoginType.Facebook, Kernel.uid, accessToken, string.Empty);
    }

    //** 게임센터 계정연동 콜백 함수
    private void OnGameCenterLink(string authToken)
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_ACCOUNT_LINK_SYN(Common.Util.eLoginType.Apple, Kernel.uid, authToken, string.Empty);
    }

    //** 계정연동 결과 콜백 함수
    private void OnLickAccountResult()
    {
        UINotificationCenter.Enqueue(Languages.ToString(TEXT_UI.ACCOUNT_LINKED_FINISHED));
        Kernel.uiManager.Close(UI.AccountInterconnector);
        SetUIActiveInit();
    }

    //** 계정탈퇴 버튼 선택
    private void OnClickDropOutAccountButton()
    {
        if (Kernel.uiManager)
        {
            UIDropOutAccount dropOutAccount = Kernel.uiManager.Get<UIDropOutAccount>(UI.DropOutAccount, true, false);

            if (dropOutAccount)
                Kernel.uiManager.Open(UI.DropOutAccount);
        }
    }

    //** 로그아웃 버튼 선택
    private void OnClickLogOutButton()
    {
        //Common.Util.eLoginType linkLoginType = Common.Util.eLoginType.None;
        //string loginKey = "";

        //if (Kernel.entry.account.subLoginType != Common.Util.eLoginType.None)
        //{
        //    linkLoginType = Kernel.entry.account.subLoginType;
        //    loginKey = Kernel.entry.account.subLoginKey;
        //}
        //else
        //{
        //    linkLoginType = Kernel.entry.account.mainLoginType;
        //    loginKey = Kernel.entry.account.mainLoginKey;
        //}

        OnLickBreakAccount();

//        switch (linkLoginType)
//        {
//            case Common.Util.eLoginType.Facebook: Kernel.facebookManager.LogOut(); break;
//#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
//            case Common.Util.eLoginType.Google: Kernel.gpgsManager.SignOut(); break;
//#endif
//            case Common.Util.eLoginType.Apple: break; //GameCenterManager 필요.
//            case Common.Util.eLoginType.Guest:
//            default: break;
//        }
    }

    //** 로그아웃 콜백 함수
    private void OnLickBreakAccount()
    {
        Kernel.entry.account.REQ_PACKET_CG_AUTH_LOG_OUT_SYN();
    }

    //** 제작 지원 버튼 선택
    private void OnClickMakeSupportButton()
    {
        UISupportCheck supportPage = Kernel.uiManager.Get<UISupportCheck>(UI.SupportCheck, true, false);

        if(supportPage != null)
            Kernel.uiManager.Open(UI.SupportCheck);
    }
}
