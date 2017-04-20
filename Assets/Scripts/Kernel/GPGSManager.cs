#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GPGSManager : Singleton<GPGSManager>
{
    // https://github.com/playgameservices/play-games-plugin-for-unity

    public delegate void OnSignIn(string id, string serverAuthCode);
    public OnSignIn onSignIn;

    public delegate void OnSignOut();
    public OnSignOut onSignOut;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
        GooglePlayGames.OurUtils.PlayGamesHelperObject.CreateObject();
    }

    // Use this for initialization

    // Update is called once per frame

    public void Initialize()
    {
        /*
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            // enables saving game progress.
        .EnableSavedGames()
            // registers a callback to handle game invitations received while the game is not running.
        .WithInvitationDelegate(InvitationReceivedDelegate)
            // registers a callback for turn based match notifications received while the
            // game is not running.
        .WithMatchDelegate(MatchDelegate)
            // require access to a player's Google+ social graph (usually not needed)
        .RequireGooglePlus()
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        */
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();
    }

    public void SignIn()
    {
        // authenticate user:
        Social.localUser.Authenticate(OnAuthenticate);
    }

    void OnAuthenticate(bool success)
    {
        // handle success or failure
        Debug.Log("success : " + success);
        if (success)
        {
            PlayGamesPlatform.Instance.GetServerAuthCode(OnServerAuthCode);
        }
        else
        {
            UIAlerter.Alert(Languages.ToString(TEXT_UI.LOGIN_ERROR_LOGIN),
                                    UIAlerter.Composition.Confirm,
                                    null,
                                    Languages.ToString(TEXT_UI.NOTICE_WARNING));
        }
    }

    void OnServerAuthCode(CommonStatusCodes commonStatusCodes, string serverAuthCode)
    {
        Debug.LogFormat("CommonStatusCodes : {0}\nServerAuthCode : {1}\nApplicationId : {2}\nWebClientId : {3}",
                          commonStatusCodes,
                          serverAuthCode,
                          GameInfo.ApplicationId,
                          GameInfo.WebClientId);

        if (commonStatusCodes == CommonStatusCodes.Success)
        {
            if (onSignIn != null)
            {
                onSignIn(Social.localUser.id, serverAuthCode);
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

    public void SignOut()
    {
        // sign out
        if (PlayGamesPlatform.Instance.IsAuthenticated())
        {
            PlayGamesPlatform.Instance.SignOut();

            if (onSignOut != null)
            {
                onSignOut();
            }
        }
    }
}
#endif