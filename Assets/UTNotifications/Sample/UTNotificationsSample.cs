using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace UTNotifications
{
    /// <summary>
    /// The sample showing how to use different <c>UTNotifications</c> features.
    /// </summary>
    public class UTNotificationsSample : MonoBehaviour
    {
    //public
        /// <summary>
        /// Shows how you can initialize the UTNotifications
        /// </summary>
        public void Start()
        {
            //Please see the API Reference for the detailed information: http://universal-tools.github.io/UTNotifications/html/annotated.html

            Manager notificationsManager = Manager.Instance;

            notificationsManager.OnSendRegistrationId += SendRegistrationId;
            notificationsManager.OnNotificationClicked += OnNotificationClicked;
            notificationsManager.OnNotificationsReceived += OnNotificationsReceived;    //Let's handle incoming notifications (not only push ones)

            if (string.IsNullOrEmpty(m_webServerAddress))
            {
                m_webServerAddress = PlayerPrefs.GetString(m_webServerAddressOptionName, "");
            }

            if (!string.IsNullOrEmpty(m_webServerAddress))
            {
                Initialize();
            }
        }

        /// <summary>
        /// Draws the sample UI.
        /// </summary>
        public void OnGUI()
        {
            int height = Screen.height / 10;
            int offsetX = 8;
            int offsetY = height / 16;
            int offsetTitle = 32;

            if (!ShowReceivedNotification(true, height, offsetX, offsetY, offsetTitle) && !ShowReceivedNotification(false, height, offsetX, offsetY, offsetTitle))
            {
                ShowMenu(height, offsetX, offsetY, offsetTitle);
            }
        }

    //protected
        /// <summary>
        /// Shows the received/clicked notification.
        /// </summary>
        /// <returns><c>true</c>, if received notification was shown, <c>false</c> otherwise.</returns>
        protected bool ShowReceivedNotification(bool clicked, int height, int offsetX, int offsetY, int offsetTitle)
        {
            ReceivedNotification notification;
            if (clicked)
            {
                if (m_clickedNotification != null)
                {
                    notification = m_clickedNotification;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (m_receivedNotifications.Count > 0)
                {
                    notification = m_receivedNotifications[0];
                }
                else
                {
                    return false;
                }
            }

            const int itemsCount = 3;

            GUI.Box(new Rect(offsetX, (Screen.height - ((height + offsetY) * itemsCount + offsetTitle)) / 2, Screen.width - offsetX * 2, (height + offsetY) * itemsCount + offsetTitle), (clicked ? m_notificationClickedTitle : m_notificationReceivedTitle) + " [id=" + notification.id + "]");
            int y = (Screen.height - ((height + offsetY) * itemsCount + offsetTitle)) / 2 + offsetTitle;
            
            string userData = "";
            if (notification.userData != null)
            {
                foreach (KeyValuePair<string, string> it in notification.userData)
                {
                    userData += it.Key + "=" + it.Value + " ";
                }
            }
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), (string.IsNullOrEmpty(notification.notificationProfile) ? "" : "Profile: " + notification.notificationProfile + "\n") + notification.title + "\n" + notification.text + (string.IsNullOrEmpty(userData) ? "" : "\n" + userData) + "\n" + m_clickToHide))
            {
                HideNotification(notification.id);
                if (clicked)
                {
                    m_clickedNotification = null;
                }
                else
                {
                    m_receivedNotifications.RemoveAt(0);
                }
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_hideAllText))
            {
                m_clickedNotification = null;
                HideAll();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_cancelAllText))
            {
                m_clickedNotification = null;
                CancelAll();
            }
            y += height + offsetY;

            return true;
        }

        /// <summary>
        /// Shows the sample menu.
        /// </summary>
        protected void ShowMenu(int height, int offsetX, int offsetY, int offsetTitle)
        {
            GUI.Box(new Rect(offsetX, offsetX, Screen.width - offsetX * 2, Screen.height - offsetX * 2), m_title);
            int y = offsetY + offsetTitle;
            
            string address = GUI.TextField(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_webServerAddress);
            if (address != m_webServerAddress)
            {
                m_webServerAddress = address;
                PlayerPrefs.SetString(m_webServerAddressOptionName, m_webServerAddress);
            }
            y += height + offsetY;
            
            if (string.IsNullOrEmpty(m_webServerAddress))
            {
                GUI.Label(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_pleaseEnterWebServerAddress);
                return;
            }
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_notifyAllText))
            {
                NotifyAll();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_initializeText))
            {
                Initialize();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_localNotificationText))
            {
                CreateLocalNotification();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_scheduledNotificationText))
            {
                CreateScheduledNotifications();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_cancelRepeatingNotificationText))
            {
                CancelRepeatingScheduledNotification();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_incrementBadgeText))
            {
                IncrementBadge();
            }
            y += height + offsetY;
            
            if (GUI.Button(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_cancelAllText))
            {
                CancelAll();
            }
            y += height + offsetY;
            
            if (GUI.Toggle(new Rect(offsetX * 2, y, Screen.width - offsetX * 4, height), m_notificationsEnabled, m_notificationsEnabledText) != m_notificationsEnabled)
            {
                SetNotificationsEnabled(!m_notificationsEnabled);
            }
            y += height + offsetY;
        }
        
        /// <summary>
        /// Initialize the <c>Manager</c>.
        /// </summary>
        protected void Initialize()
        {
            //We would like to handle incoming notifications and don't want to increment the push notifications id
            Manager notificationsManager = Manager.Instance;
            bool result = notificationsManager.Initialize(true, 0, false);
            Debug.Log("UTNotifications Initialize: " + result);

            m_notificationsEnabled = notificationsManager.NotificationsEnabled();
            notificationsManager.SetBadge(0);
        }

        /// <summary>
        /// Creates the local notification.
        /// </summary>
        protected void CreateLocalNotification()
        {
            Dictionary<string, string> userData = new Dictionary<string, string>();
            userData.Add("user", "data");
            Manager.Instance.PostLocalNotification("Local", "Notification", LocalNotificationId, userData, "demo_notification_profile");
        }

        /// <summary>
        /// Creates 2 scheduled notifications: single one and repeating one.
        /// </summary>
        protected void CreateScheduledNotifications()
        {
            Manager.Instance.ScheduleNotification(15, "Scheduled", "Notification", ScheduledNotificationId, null, "demo_notification_profile", 1);

            Dictionary<string, string> userData = new Dictionary<string, string>();
            //(Android only) shows a notification with a random cat image
            userData.Add("image_url", "http://thecatapi.com/api/images/get?format=src&type=png&size=med");
            Manager.Instance.ScheduleNotificationRepeating(DateTime.Now.AddSeconds(5), 25, "Scheduled Repeating", "Notification", RepeatingNotificationId, userData, "demo_notification_profile", 1);
        }

        /// <summary>
        /// Cancels the previously created repeating scheduled notification.
        /// </summary>
        protected void CancelRepeatingScheduledNotification()
        {
            Manager.Instance.CancelNotification(RepeatingNotificationId);
        }

        /// <summary>
        /// Increments the app icon badge value, when supported.
        /// </summary>
        protected void IncrementBadge()
        {
            Manager.Instance.SetBadge(Manager.Instance.GetBadge() + 1);
        }

        /// <summary>
        /// Cancels all the previously created notifications.
        /// </summary>
        protected void CancelAll()
        {
            Manager.Instance.CancelAllNotifications();
            Manager.Instance.SetBadge(0);
            m_receivedNotifications.Clear();
        }

        /// <summary>
        /// Enables/disables notifications.
        /// </summary>
        protected void SetNotificationsEnabled(bool enabled)
        {
            Manager.Instance.SetNotificationsEnabled(enabled);
            m_notificationsEnabled = Manager.Instance.NotificationsEnabled();
        }

        /// <summary>
        /// Hides the specified notification.
        /// </summary>
        protected void HideNotification(int id)
        {
            Manager.Instance.HideNotification(id);
        }

        /// <summary>
        /// Hides all the notifications.
        /// </summary>
        protected void HideAll()
        {
            Manager.Instance.HideAllNotifications();
            m_receivedNotifications.Clear();
        }

        /// <summary>
        /// A wrapper for the <c>SendRegistrationId(string userId, string providerName, string registrationId)</c> coroutine
        /// </summary>
        protected void SendRegistrationId(string providerName, string registrationId)
        {
            string userId = GenerateDeviceUniqueIdentifier();
            StartCoroutine(SendRegistrationId(userId, providerName, registrationId));
        }

        /// <summary>
        /// Sends the received push notifications registrationId to the demo server
        /// </summary>
        protected IEnumerator SendRegistrationId(string userId, string providerName, string registrationId)
        {
            if (string.IsNullOrEmpty(m_webServerAddress))
            {
                m_initializeText = m_initializeTextOriginal + "\nUnable to send the registrationId: please fill the running demo server address";
                yield break;
            }

            m_initializeText = m_initializeTextOriginal + "\nSending registrationId...\nPlease make sure the example server is running as " + m_webServerAddress;

            WWWForm wwwForm = new WWWForm();
            
            wwwForm.AddField("uid", userId);
            wwwForm.AddField("provider", providerName);
            wwwForm.AddField("id", registrationId);
            
            WWW www = new WWW(m_webServerAddress + "/register", wwwForm);
            yield return www;

            if (www.error != null)
            {
                m_initializeText = m_initializeTextOriginal + "\n" + www.error + " " + www.text;
            }
            else
            {
                m_initializeText = m_initializeTextOriginal + "\n" + www.text;
            }
        }

        /// <summary>
        /// A wrapper for the <c>NotifyAll(string title, string text, string notificationProfile, int badgeNumber)</c> coroutine.
        /// </summary>
        protected void NotifyAll()
        {
            StartCoroutine(NotifyAll(PushNotificationId, "Hello!", "From " + SystemInfo.deviceModel, "demo_notification_profile", 1));
        }

        /// <summary>
        /// Requests the DemoServer to notify all the registered devices with push notifications.
        /// </summary>
        protected IEnumerator NotifyAll(int id, string title, string text, string notificationProfile, int badgeNumber)
        {
            m_notifyAllText = m_notifyAllTextOriginal + "\nSending...\nPlease make sure the example server is running as " + m_webServerAddress;

            title = WWW.EscapeURL(title);
            text = WWW.EscapeURL(text);

            string noCache = "&_NO_CACHE=" + UnityEngine.Random.value;

            WWW www = new WWW(string.Format("{0}/notify?id={1}&title={2}&text={3}&badge={4}{5}{6}", m_webServerAddress, id, title, text, badgeNumber, (string.IsNullOrEmpty(notificationProfile) ? "" : "&notification_profile=" + notificationProfile), noCache));
            yield return www;

            if (www.error != null)
            {
                m_notifyAllText = m_notifyAllTextOriginal + "\n" + www.error + " " + www.text;
            }
            else
            {
                m_notifyAllText = m_notifyAllTextOriginal + "\n" + www.text;
            }
        }

        /// <summary>
        /// Handles click on a notification.
        /// </summary>
        protected void OnNotificationClicked(ReceivedNotification notification)
        {
            m_clickedNotification = notification;
        }

        /// <summary>
        /// Handles the received notifications.
        /// </summary>
        protected void OnNotificationsReceived(IList<ReceivedNotification> receivedNotifications)
        {
            m_receivedNotifications.AddRange(receivedNotifications);
        }

        /// <summary>
        /// Address of the running demo server. You can replace the default value by your demo server address (f.e. <c>"http://192.168.2.102:8080"</c>).
        /// </summary>
        protected string m_webServerAddress = "";

        protected const int LocalNotificationId = 1;
        protected const int ScheduledNotificationId = 2;
        protected const int RepeatingNotificationId = 3;
        protected const int PushNotificationId = 4;

    //private
#if UNITY_ANDROID
        static private string GetMd5Hash(string input)
        {
            if (input == "")
            {
                return "";
            }

            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                stringBuilder.Append(data[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }
#endif

        /// <summary>
        /// Generates the device unique identifier.
        /// </summary>
        /// <remarks>
        /// See http://forum.unity3d.com/threads/released-utnotifications-professional-cross-platform-push-notifications-and-more.333045/page-3#post-2591927 & http://forum.unity3d.com/threads/unique-identifier-details.353256/
        /// </remarks>
        static private string GenerateDeviceUniqueIdentifier()
        {
#if !UNITY_ANDROID
            return SystemInfo.deviceUniqueIdentifier;
#else
            try
            {
                string id = "";
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context");
                string TELEPHONY_SERVICE = contextClass.GetStatic<string>("TELEPHONY_SERVICE");
                AndroidJavaObject telephonyService = activity.Call<AndroidJavaObject>("getSystemService", TELEPHONY_SERVICE);
                bool noPermission = false;

                try
                {
                    id = telephonyService.Call<string>("getDeviceId");
                }
                catch
                {
                    noPermission = true;
                }

                if (id == null)
                {
                    id = "";
                }

                if (noPermission)
                {
                    AndroidJavaClass settingsSecure = new AndroidJavaClass("android.provider.Settings$Secure");
                    string ANDROID_ID = settingsSecure.GetStatic<string>("ANDROID_ID");
                    AndroidJavaObject contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
                    id = settingsSecure.CallStatic<string>("getString", contentResolver, ANDROID_ID);
                    if (id == null)
                    {
                        id = "";
                    }
                }

                if (id == "")
                {
                    string mac = "00000000000000000000000000000000";
                    try
                    {
                        StreamReader reader = new StreamReader("/sys/class/net/wlan0/address");
                        mac = reader.ReadLine();
                        reader.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    id = mac.Replace(":", "");
                }

                return GetMd5Hash(id);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return GetMd5Hash("00000000000000000000000000000000");
            }
#endif
        }

        private const string m_title = "UTNotifications Sample";
        private const string m_notificationClickedTitle = "Notification Clicked";
        private const string m_notificationReceivedTitle = "Notification Received";
        private const string m_pleaseEnterWebServerAddress = "Please enter the running demo server address above. F.e. http://192.168.2.102:8080";
        private const string m_initializeTextOriginal = "Initialize";
        private string m_initializeText = m_initializeTextOriginal;
        private const string m_notifyAllTextOriginal = "Notify all registered devices";
        private string m_notifyAllText = m_notifyAllTextOriginal;
        private const string m_localNotificationText = "Create Local Notification";
        private const string m_incrementBadgeText = "Increment the badge number, when supported";
        private const string m_scheduledNotificationText = "Create Scheduled Notifications";
        private const string m_cancelRepeatingNotificationText = "Cancel Repeating Notification";
        private const string m_hideAllText = "Hide All Notifications";
        private const string m_cancelAllText = "Cancel All Notifications\n(Also resets the badge number on iOS)";
        private const string m_notificationsEnabledText = "Notifications Enabled";
        private const string m_webServerAddressOptionName = "_UT_NOTIFICATIONS_SAMPLE_SERVER_ADDRESS";
        private bool m_notificationsEnabled;
        private string m_clickToHide = "(Click to hide)";
        private ReceivedNotification m_clickedNotification = null;
        private List<ReceivedNotification> m_receivedNotifications = new List<ReceivedNotification>();
    }
}