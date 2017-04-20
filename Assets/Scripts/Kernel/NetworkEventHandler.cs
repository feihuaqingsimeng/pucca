using Common.Packet;
using UnityEngine;

public class NetworkEventHandler : Singleton<NetworkEventHandler>
{

    // Use this for initialization

    // Update is called once per frame

    void OnEnable()
    {
        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException += OnNetworkException;
        }

        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onUserInfoResult += OnUserInfoResult;
            Kernel.entry.ranking.onGuildInfoResult += OnGuildInfoResult;
            Kernel.entry.account.onLevelUpdate += OnLevelUpdate;
            Kernel.entry.revengeBattle.onStartedRevengeMatch += OnStartedRevengeMatch;
        }

        if (Kernel.achieveManager != null)
        {
            Kernel.achieveManager.onCompleteAchieve += OnCompleteAchieve;
            Kernel.achieveManager.onCompleteDailyAchieve += OnCompleteDailyAchieve;
        }
    }

    void OnDisable()
    {
        if (Kernel.networkManager != null)
        {
            Kernel.networkManager.onException -= OnNetworkException;
        }

        if (Kernel.entry != null)
        {
            Kernel.entry.ranking.onUserInfoResult -= OnUserInfoResult;
            Kernel.entry.ranking.onGuildInfoResult -= OnGuildInfoResult;
            Kernel.entry.account.onLevelUpdate -= OnLevelUpdate;
            Kernel.entry.revengeBattle.onStartedRevengeMatch -= OnStartedRevengeMatch;
        }

        if (Kernel.achieveManager != null)
        {
            Kernel.achieveManager.onCompleteAchieve -= OnCompleteAchieve;
            Kernel.achieveManager.onCompleteDailyAchieve -= OnCompleteDailyAchieve;
        }
    }

    void OnCompleteAchieve(int achieveIndex, int achieveGroup, byte achieveStep)
    {
        UIAchieveNotification achieveNotification = Kernel.uiManager.Get<UIAchieveNotification>(UI.AchieveNotification, true, false);
        if (achieveNotification != null)
        {
            achieveNotification.OnAchieveComplete(achieveIndex, achieveGroup, achieveStep);

            if (!achieveNotification.gameObject.activeSelf)
            {
                Kernel.uiManager.Open(UI.AchieveNotification);
                Kernel.soundManager.PlayUISound(SOUND.SND_UI_ACHIEVEMENT_COMPLETED);
            }
        }
    }

    void OnCompleteDailyAchieve(int achieveIndex)
    {
        UIAchieveNotification achieveNotification = Kernel.uiManager.Get<UIAchieveNotification>(UI.AchieveNotification, true, false);
        if (achieveNotification != null)
        {
            achieveNotification.OnDailyAchieveComplete(achieveIndex);

            if (!achieveNotification.gameObject.activeSelf)
            {
                Kernel.uiManager.Open(UI.AchieveNotification);
                Kernel.soundManager.PlayUISound(SOUND.SND_UI_ACHIEVEMENT_COMPLETED);
            }
        }
    }

    void OnLevelUpdate(byte level)
    {
        Kernel.soundManager.PlayUISound(SOUND.SND_UI_ACCOUNT_LEVELUP);
        Kernel.uiManager.Open(UI.LevelUp);
    }

    void OnGuildInfoResult(CGuildBase guildBase)
    {
        if (guildBase != null)
        {
            UIGuildInfo guildInfo = Kernel.uiManager.Get<UIGuildInfo>(UI.GuildInfo, true, false);
            if (guildInfo != null)
            {
                guildInfo.SetGuildInfo(guildBase);
                Kernel.uiManager.Open(UI.GuildInfo);
            }
        }
    }

    void OnUserInfoResult(UserInfo userInfo)
    {
        if (userInfo != null)
        {
            UIUserInfo uiUserInfo = Kernel.uiManager.Get<UIUserInfo>(UI.UserInfo, true, false);
            if (uiUserInfo != null)
            {
                uiUserInfo.SetUserInfo(userInfo);
                Kernel.uiManager.Open(UI.UserInfo);
            }
        }
    }

    void OnStartedRevengeMatch(long sequence)
    {
        CRevengeMatchInfo MatchInfo = Kernel.entry.revengeBattle.FindRevengeMatchInfo(sequence);
        if (MatchInfo == null)
            return;

        Kernel.entry.battle.CurBattleKind = BATTLE_KIND.REVENGE_BATTLE;
        Kernel.entry.battle.RevengeMatchInfoData = MatchInfo;
        Kernel.sceneManager.LoadScene(Scene.Battle, true);
    }

    public static void OnNetworkException(Result_Define.eResult errorCode, string networkError = null, ePACKET_CATEGORY category = 0, byte index = 0)
    {
        if (!string.IsNullOrEmpty(networkError))
        {
            // Result_Define.eResult.SUCCESS : Invalid errorCode.
            OnCriticalNetworkException(Result_Define.eResult.SUCCESS, networkError);
        }
        else
        {
            // UINicknameEditor.cs
            if (category == ePACKET_CATEGORY.CG_AUTH &&
                index == (byte)eCG_AUTH.CREATE_NICKNAME_ACK)
            {
                return;
            }
            else if (category == ePACKET_CATEGORY.CG_BILLING &&
                     index == (byte)eCG_BILLING.BUY_ITEM_GOOGLE_ACK)
            {
                if (Kernel.iapManager.purchaseProcessing)
                {
                    Kernel.iapManager.purchaseProcessing = false;
                }
            }

            if (Kernel.sceneManager.isSceneLoading || Kernel.sceneManager.activeSceneObject.scene == Scene.TitleScene)
            {
                // string.Empty : Invalid networkError.
                OnCriticalNetworkException(errorCode, string.Empty);
            }
            else
            {
                string errorMsg = string.Empty;
                DBStr_Network.Schema strNetwork = DBStr_Network.Query(DBStr_Network.Field.IndexID, errorCode);
                errorMsg = (strNetwork != null) ? string.Format(strNetwork.StringData, errorCode) : errorCode.ToString();
                if (strNetwork != null && strNetwork.IsSysMsg)
                {
                    UINotificationCenter.Enqueue(errorMsg);
                }
                else
                {
                    UIAlerter.Alert(errorMsg, UIAlerter.Composition.Confirm);
                }
            }
        }
    }

    static void OnCriticalNetworkException(Result_Define.eResult errorCode, string networkError = null)
    {
        string errorMsg = networkError;
        if (string.IsNullOrEmpty(errorMsg))
        {
            DBStr_Network.Schema strNetwork = DBStr_Network.Query(DBStr_Network.Field.IndexID, errorCode);
            errorMsg = (strNetwork != null) ? string.Format(strNetwork.StringData, errorCode) : errorCode.ToString();
        }
        else
        {
            errorMsg = Languages.ToString(TEXT_UI.NETWORK_FAILED);
        }

        Debug.LogFormat("NetworkException errorCode: {0}, msg: {1}", errorCode, (networkError != null) ? networkError : errorMsg);

        UIAlerter.Alert(errorMsg,
                        UIAlerter.Composition.Confirm,
                        delegate(UIAlerter.Response response, object[] args)
                        {
                            Kernel.Reload();
                        },
                        Languages.ToString(TEXT_UI.NOTICE_WARNING));
    }
}
