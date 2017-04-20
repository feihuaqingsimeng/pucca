#if UNITY_ANDROID

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace UTNotifications
{
    [InitializeOnLoad]
    public class UTNotificationsDependencies : AssetPostprocessor
    {
        static UTNotificationsDependencies()
        {
            RegisterDependencies();
        }

        public static void RegisterDependencies()
        {
            RegisterAndroidDependencies();
        }

        public static void RegisterAndroidDependencies()
        {
            Type playServicesSupport = Google.VersionHandler.FindClass("Google.JarResolver", "Google.JarResolver.PlayServicesSupport");
            if (playServicesSupport == null)
            {
                return;
            }

            svcSupport = svcSupport ?? Google.VersionHandler.InvokeStaticMethod(playServicesSupport, "CreateInstance", new object[] { "UTNotifications", EditorPrefs.GetString("AndroidSdkRoot"), "ProjectSettings" });

            Google.VersionHandler.InvokeInstanceMethod(svcSupport, "ClearDependencies", new object[] { });

            Google.VersionHandler.InvokeInstanceMethod(svcSupport, "DependOn", new object[] { "com.google.android.gms", "play-services-gcm", "LATEST" }, namedArgs: new Dictionary<string, object>()
            {
                { "packageIds", new string[] { "extra-google-m2repository" } }
            });

            Google.VersionHandler.InvokeInstanceMethod(svcSupport, "DependOn", new object[] { "com.android.support", "support-v4", GetAndroidSupportPluginVersion() }, namedArgs: new Dictionary<string, object>()
            {
                { "packageIds", new string[] { "extra-android-m2repository" } }
            });
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.Contains("IOSResolver") || asset.Contains("JarResolver"))
                {
                    RegisterDependencies();
                    break;
                }
            }
        }

        private static string GetAndroidSupportPluginVersion()
        {
            string gpgsDependenciesPath = Settings.FullPath("GooglePlayGames/Editor/GPGSDependencies.cs");

            //If Google Play Services plugin present, let's use the same version of Android Support Library to avoid inter-plugin conflicts.
            if (File.Exists(gpgsDependenciesPath))
            {
                string gpgsDependencies = File.ReadAllText(gpgsDependenciesPath);
                const string supportv4 = "\"support-v4\"";

                int supportv4Index = gpgsDependencies.IndexOf(supportv4);
                if (supportv4Index > 0)
                {
                    int versionStartSearchIndex = supportv4Index + supportv4.Length + 1;

                    bool firstQuotationFound = false;
                    for (int i = versionStartSearchIndex; i < gpgsDependencies.Length; ++i)
                    {
                        if (gpgsDependencies[i] == '\"')
                        {
                            if (!firstQuotationFound)
                            {
                                firstQuotationFound = true;
                                versionStartSearchIndex = i + 1;
                            }
                            else
                            {
                                return gpgsDependencies.Substring(versionStartSearchIndex, i - versionStartSearchIndex);
                            }
                        }
                    }
                }
            }

            return "LATEST";
        }

        private static object svcSupport;
    }
}

#endif