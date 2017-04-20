using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UTNotifications;
using System;

public class NotificationManager : Singleton<NotificationManager>
{

    #region Properties
    //public int NotificationId
    //{
    //    get;
    //    private set;
    //}

    /*
    public int postLocalNotificationId
    {
        get;
        private set;
    }

    public int scheduleNotificationId
    {
        get;
        private set;
    }

    public int scheduleNotificationRepeatingId
    {
        get;
        private set;
    }
    */

    

    //** Push Enable
    // edit - dotsoft
    public bool Push_On
    {
        get
        {
            return Manager.Instance.NotificationsEnabled();
        }

        set
        {
            Manager.Instance.SetNotificationsEnabled(value);
        }
    }

    /*
    private const string STR_PUSH_ON_KEY = "PUSH_ONOFF_KEY";
    private bool m_bPush_On;
    public bool Push_On
    {
        get 
        {
            if(PlayerPrefs.HasKey(STR_PUSH_ON_KEY))
            {
                int onValue = PlayerPrefs.GetInt(STR_PUSH_ON_KEY); 
                m_bPush_On = onValue == 0 ? true : false;
            }
                
            return m_bPush_On; 
        }
        set
        {
            m_bPush_On = value;

            int pushValue = value ? 0 : 1;
            PlayerPrefs.SetInt(STR_PUSH_ON_KEY, pushValue);
            PlayerPrefs.Save();
        }
    }
    */

    private NotificationChecker m_NotificationChecker;
    #endregion

    #region MonoBehaviour
    protected override void Awake()
    {
        base.Awake();
        Manager.Instance.OnNotificationClicked += OnNotificationClickedHandler;
        Manager.Instance.OnNotificationsReceived += OnNotificationsReceivedHandler;
        Manager.Instance.Initialize(true);

        // 각 알림에 쓰일 체커 추가
        m_NotificationChecker = this.gameObject.AddComponent<NotificationChecker>();
        //Manager.Instance.CancelAllNotifications();
    }

    // Use this for initialization

    // Update is called once per frame

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Manager.Instance.OnNotificationClicked -= OnNotificationClickedHandler;
        Manager.Instance.OnNotificationsReceived -= OnNotificationsReceivedHandler;
    }

    //** 종료 (외부 강제 종료는 안먹힘 정상적으로 종료했을 경우)
    void OnApplicationQuit()
    {
        SaveNotification();

        /*
#if UNITY_IPHONE
        ScheduleNotification(10, "OnApplicationQuit", "앱 종료시 푸쉬 테스트", 100);
#endif
        */
    }
    
    //** 실행하고 있는 앱이 정지되었을 경우 (홈버튼을 눌러서 내렸을 경우 등..)
    void OnApplicationPause()
    {
        SaveNotification();

        /*
#if UNITY_IPHONE
        ScheduleNotification(10, "OnApplicationPause", "앱 강제 종료 푸쉬 테스트", 101);
#endif
        */
    }

    //** 로컬 푸쉬 저장
    void SaveNotification()
    {
        CancelAllNotifications();

        if (!Push_On)
            return;

        if (m_NotificationChecker != null)
            m_NotificationChecker.SaveNotificationType();
    }
    #endregion

    public void ScheduleNotificationRepeating(DateTime firstTriggerDateTime, int intervalSeconds, string title, string text, int id, IDictionary<string, string> userData = null)
    {
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(text))
        {
            Manager.Instance.ScheduleNotificationRepeating(firstTriggerDateTime,
                                                           intervalSeconds,
                                                           title,
                                                           text,
                                                           id,
                                                           userData);
        }
    }

    public void ScheduleNotificationRepeating(int firstTriggerInSeconds, int intervalSeconds, string title, string text, int id, IDictionary<string, string> userData = null)
    {
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(text))
        {
            Manager.Instance.ScheduleNotificationRepeating(firstTriggerInSeconds,
                                                           intervalSeconds,
                                                           title,
                                                           text,
                                                           id,
                                                           userData);
        }
    }

    public void ScheduleNotification(DateTime triggerDateTime, string title, string text, int id, IDictionary<string, string> userData = null)
    {
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(text))
        {
            Manager.Instance.ScheduleNotification(triggerDateTime,
                                                  title,
                                                  text,
                                                  id,
                                                  userData);
        }
    }

    public void ScheduleNotification(int triggerInSeconds, string title, string text, int id, IDictionary<string, string> userData = null)
    {
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(text))
        {
            Manager.Instance.ScheduleNotification(triggerInSeconds,
                                                  title,
                                                  text,
                                                  id,
                                                  userData);
        }
    }

    public void PostLocalNotification(string title, string text, int id, IDictionary<string, string> userData = null)
    {
        if (!string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(text))
        {
            Manager.Instance.PostLocalNotification(title,
                                                   text,
                                                   id,
                                                   userData);
        }
    }

    public void CancelNotification(int id)
    {
        Manager.Instance.CancelNotification(id);
    }

    public void CancelAllNotifications()
    {
        Manager.Instance.CancelAllNotifications();
    }

    void OnNotificationClickedHandler(ReceivedNotification notification)
    {
        Log(notification);
    }

    void OnNotificationsReceivedHandler(IList<ReceivedNotification> receivedNotifications)
    {
        if (Debug.logger.logEnabled)
        {
            if (receivedNotifications != null && receivedNotifications.Count > 0)
            {
                for (int i = 0; i < receivedNotifications.Count; i++)
                {
                    Log(receivedNotifications[i]);
                }
            }
            else Debug.LogError("receivedNotifications is null or empty.");
        }
    }

    void Log(ReceivedNotification notification)
    {
        if (Debug.logger.logEnabled)
        {
            if (notification != null)
            {
                string userData = string.Empty;
                if (notification.userData != null && notification.userData.Count > 0)
                {
                    foreach (var item in notification.userData)
                    {
                        userData = string.Format("{0}\nKey : {1}, Value : {2}", userData, item.Key, item.Value);
                    }
                }
                else userData = "userData is null or empty.";
                Debug.LogFormat("title : {0}\ntext : {1}\nid : {2}\nuserData : {3}\nnotificationProfile : {4}\nbadgeNumber : {5}",
                                notification.title,
                                notification.text,
                                notification.id,
                                userData,
                                notification.notificationProfile,
                                notification.badgeNumber);
            }
            else Debug.LogError("notification is null.");
        }
    }
}
