using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookManager : Singleton<FacebookManager>
{
    // https://developers.facebook.com/docs/unity/examples
    // https://developers.facebook.com/docs/facebook-login/access-tokens/expiration-and-extension

    #region Variables
    // 1180628048689010
    #endregion

    #region Properties
    public bool IsInitialized
    {
        get
        {
            return FB.IsInitialized;
        }
    }
    #endregion

    #region Delegates
    public delegate void OnLogIn(string userId, string accessToken);
    public OnLogIn onLogIn;

    public delegate void OnLogOut();
    public OnLogOut onLogOut;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    // Use this for initialization

    // Update is called once per frame

    public void Initialize()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
    }

    public void LogIn()
    {
        if (!FB.IsLoggedIn)
        {
            var perms = new List<string>() { "public_profile", "email", "user_friends" };
            FB.LogInWithReadPermissions(perms, OnAuth);
        }
        else
        {
            //FB.Mobile.RefreshCurrentAccessToken(OnRefreshCurrentAccessToken);
            AccessToken accessToken = AccessToken.CurrentAccessToken;
            Debug.Log(accessToken);
            // ExpirationTime 처리
            if (onLogIn != null)
            {
                onLogIn(accessToken.UserId, accessToken.TokenString);
            }
        }
    }

    /*
    void OnRefreshCurrentAccessToken(IAccessTokenRefreshResult result)
    {
        AccessToken accessToken = AccessToken.CurrentAccessToken;
        Debug.Log(accessToken);
        if (onLogIn != null)
        {
            onLogIn(accessToken.UserId, accessToken.TokenString);
        }
    }
    */

    void OnAuth(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken accessToken = AccessToken.CurrentAccessToken;
            Debug.Log(accessToken);
            if (onLogIn != null)
            {
                onLogIn(accessToken.UserId, accessToken.TokenString);
            }
        }
        else
        {
            Debug.Log("User cancelled facebook log-in.");

        }
    }

    public void LogOut()
    {
        if (FB.IsLoggedIn)
        {
            FB.LogOut();

            if (onLogOut != null)
            {
                onLogOut();
            }
        }
    }

    void InitCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to initialize the Facebook SDK.");
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        Initialize();
    }
}
