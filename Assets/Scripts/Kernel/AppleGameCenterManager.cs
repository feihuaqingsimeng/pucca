using System;
// using U3DXT.iOS.GameKit;
using UnityEngine;

public class AppleGameCenterManager : Singleton<AppleGameCenterManager>
{
    // https://docs.unity3d.com/ScriptReference/SocialPlatforms.GameCenter.GameCenterPlatform.html

        /*
    public class AppleGameCenterAuth
    {
        public string PlayerID { get; set; }
        public string BundleID { get; set; }
        public string PublicKeyUrl { get; set; }
        public string Signature { get; set; }
        public string Salt { get; set; }
        public ulong Timestamp { get; set; }
    }

    public bool isAuth
    {
        get;
        private set;
    }

    public delegate void OnSignIn(string id);
    public OnSignIn onSignIn;

    public void SignIn()
    {
        GameKitXT.AuthenticateLocalPlayer();
    }

    void OnEnable()
    {
        GameKitXT.LocalPlayerAuthenticated += HandleAppleLocalPlayerAuthenticated;
        GameKitXT.LocalPlayerAuthenticationFailed += HandleAppleLocalPlayerAuthenticationFailed;
    }

    void OnDisable()
    {
        GameKitXT.LocalPlayerAuthenticated -= HandleAppleLocalPlayerAuthenticated;
        GameKitXT.LocalPlayerAuthenticationFailed -= HandleAppleLocalPlayerAuthenticationFailed;
    }

    

    void HandleAppleLocalPlayerAuthenticationFailed(object sender, U3DXT.Core.U3DXTErrorEventArgs e)
    {
        Debug.Log("(social) not authentificated " + e.description);

        UIAlerter.Alert(Languages.ToString(TEXT_UI.LOGIN_ERROR_LOGIN),
                                    UIAlerter.Composition.Confirm,
                                    null,
                                    Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }

    void HandleAppleLocalPlayerAuthenticated(object sender, EventArgs e)
    {
        Debug.Log("(social) authentificated");

        if (onSignIn != null)
        {
            GameKitXT.localPlayer.gkLocalPlayer.GenerateIdentityVerificationSignature((_publicKeyUrl, _signature, _salt, _creationDateSpan, _nserror) =>
            {
                Debug.Log("(social) on verif code");
                if (_nserror != null) Debug.Log("(social) error?" + _nserror.ToString());
                Debug.Log("(social) _publicKeyUrl:" + _publicKeyUrl.ToString());
                Debug.Log("(social) _signature:" + _signature.ToString());
                Debug.Log("(social) _salt:" + _salt.ToString());

                AppleGameCenterAuth auths = new AppleGameCenterAuth();
                auths.PlayerID = GameKitXT.localPlayer.playerID;
                auths.BundleID = Application.bundleIdentifier;
                auths.PublicKeyUrl = _publicKeyUrl.ToString();
                auths.Signature = _signature.ToString();
                auths.Salt = _salt.ToString();
                auths.Timestamp = _creationDateSpan;

                string auth_token = JsonUtility.ToJson(auths);

                onSignIn(auth_token);
            });
        }
    }
    */

    /*
    public void GetServerAuthCode(Action<AppleGameCenterAuth> callback)
    {
        if (isAuth)
        {
            GameKitXT.localPlayer.gkLocalPlayer.GenerateIdentityVerificationSignature((_publicKeyUrl, _signature, _salt, _creationDateSpan, _nserror) =>
            {
                Debug.Log("(social) on verif code");
                if (_nserror != null) Debug.Log("(social) error?" + _nserror.ToString());
                Debug.Log("(social) _publicKeyUrl:" + _publicKeyUrl.ToString());
                Debug.Log("(social) _signature:" + _signature.ToString());
                Debug.Log("(social) _salt:" + _salt.ToString());

                AppleGameCenterAuth auths = new AppleGameCenterAuth();
                auths.PlayerID = GameKitXT.localPlayer.playerID;
                auths.BundleID = Application.bundleIdentifier;
                auths.PublicKeyUrl = _publicKeyUrl.ToString();
                auths.Signature = _signature.ToString();
                auths.Salt = _salt.ToString();
                auths.Timestamp = _creationDateSpan;

                callback(auths);
            });
        }
    }
    */

    public delegate void OnSignIn(string id);
    public OnSignIn onSignIn;

    public void SignIn()
    {
        // authenticate user:
        Social.localUser.Authenticate(OnAuthenticate);
    }

    void OnAuthenticate(bool success)
    {
        // handle success or failure
        Debug.Log("gamecenter login success : " + success);

        if (success)
        {
            if (onSignIn != null)
            {
                onSignIn(Social.localUser.id);
            }
        }
        else
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.LOGIN_ERROR_LOGIN),
                                    UIAlerter.Composition.Confirm,
                                    null,
                                    Languages.ToString(TEXT_UI.NOTICE_WARNING));
        }
    }

    /*
    public delegate void OnRetrieveVerificationSignature(string publicKeyUrl);
    public OnRetrieveVerificationSignature onRetrieveVerificationSignature;

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        GameCenterManager.OnAuthFinished += OnAuthenticate;
        GameCenterManager.OnPlayerSignatureRetrieveResult += OnPlayerSignatureRetrieveResult;
    }

    void OnDisable()
    {
        GameCenterManager.OnAuthFinished -= OnAuthenticate;
        GameCenterManager.OnPlayerSignatureRetrieveResult -= OnPlayerSignatureRetrieveResult;
    }

    public void Authenticate()
    {
        GameCenterManager.Init();
    }

    void OnAuthenticate(SA.Common.Models.Result result)
    {
        if (result != null)
        {
            Debug.LogFormat("Error : {0}\nHasError : {1}\nIsFailed : {2}\nIsSucceeded : {3}\n", result.Error, result.HasError, result.IsFailed, result.IsSucceeded);
            if (result.IsSucceeded)
            {
                GameCenterManager.RetrievePlayerSignature();
            }
        }
    }

    void OnPlayerSignatureRetrieveResult(GK_PlayerSignatureResult result)
    {
        if (result != null)
        {
            Debug.LogFormat("PublicKeyUrl : {0}", result.PublicKeyUrl);
            if (result.IsSucceeded)
            {
                if (onRetrieveVerificationSignature != null)
                {
                    onRetrieveVerificationSignature(result.PublicKeyUrl);
                }
            }
        }
    }
    */
}
