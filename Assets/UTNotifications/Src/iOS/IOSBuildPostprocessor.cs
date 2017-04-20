#if UNITY_EDITOR && UNITY_IOS

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UTNotifications
{
    class IOSBuildPostprocessor
    {
        [UnityEditor.Callbacks.PostProcessBuildAttribute(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
            if (target == BuildTarget.iPhone)
#else
            if (target == BuildTarget.iOS)
#endif
            {
                PatchAppController(pathToBuiltProject);
                CopyRaw(pathToBuiltProject);
            }
        }

        private static void PatchAppController(string pathToBuiltProject)
        {
            string fileName = Path.Combine(pathToBuiltProject, "Classes/UnityAppController.mm");
            List<string> appControllerLines = new List<string>(File.ReadAllLines(fileName));

            PatchIncludes(appControllerLines);
            PatchDidReceiveLocalNotification(appControllerLines);
            PatchDidReceiveRemoteNotification(appControllerLines);
            PatchDidFinishLaunchingWithOptions(appControllerLines);

            File.WriteAllLines(fileName, appControllerLines.ToArray());
        }

        private static void PatchIncludes(List<string> appControllerLines)
        {
#if UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9
			const string include = "#include \"Libraries/UTNotificationsTools.h\"";
#else
			const string include = "#include \"UTNotificationsTools.h\"";
#endif
            
            foreach (string line in appControllerLines)
            {
                if (line.Contains(include))
                {
                    return;
                }
            }
            
            int position;
            for (position = appControllerLines.Count - 1; position >= 0; --position)
            {
                if (appControllerLines[position].Trim().StartsWith("#include"))
                {
                    break;
                }
            }

            appControllerLines.Insert(position + 1, include);
        }

        private static void PatchDidReceiveLocalNotification(List<string> appControllerLines)
        {
            string[] addition =
            {
                "    // UTNotifications: handle clicks on local notifications",
                "    if ([UIApplication sharedApplication].applicationState != UIApplicationStateActive)",
                "    {",
                "        _UT_SetLocalNotificationWasClicked(notification.userInfo);",
                "    }"
            };

            PatchMethod("- (void)application:(UIApplication*)application didReceiveLocalNotification:(UILocalNotification*)notification", appControllerLines, addition);
        }

        private static void PatchDidReceiveRemoteNotification(List<string> appControllerLines)
        {
            string[] addition =
            {
                "    // UTNotifications: handle clicks on push notifications",
                "    if ([UIApplication sharedApplication].applicationState != UIApplicationStateActive)",
                "    {",
                "        _UT_SetPushNotificationWasClicked(userInfo);",
                "    }"
            };

            PatchMethod("- (void)application:(UIApplication*)application didReceiveRemoteNotification:(NSDictionary*)userInfo", appControllerLines, addition);
			PatchMethod("- (void)application:(UIApplication *)application didReceiveRemoteNotification:(NSDictionary *)userInfo fetchCompletionHandler:(void (^)(UIBackgroundFetchResult result))handler", appControllerLines, addition);
        }

        private static void PatchDidFinishLaunchingWithOptions(List<string> appControllerLines)
        {
            string[] addition =
            {
                "#if !UNITY_TVOS",
                "    // UTNotifications: handle clicks on local notifications",
                "    if (UILocalNotification* notification = [launchOptions objectForKey:UIApplicationLaunchOptionsLocalNotificationKey])",
                "    {",
                "        _UT_SetLocalNotificationWasClicked(notification.userInfo);",
                "    }",
                "",
                "    // UTNotifications: handle clicks on push notifications",
                "    if (NSDictionary* notification = [launchOptions objectForKey:UIApplicationLaunchOptionsRemoteNotificationKey])",
                "    {",
                "        _UT_SetPushNotificationWasClicked(notification);",
                "    }",
                "#endif"
            };

            PatchMethod("- (BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions", appControllerLines, addition);
        }

        private static void PatchMethod(string method, List<string> appControllerLines, string[] addition)
        {
            method = method.Trim();

            int methodHeader;
            for (methodHeader = 0; methodHeader < appControllerLines.Count; ++methodHeader)
            {
                if (appControllerLines[methodHeader].Trim() == method)
                {
                    break;
                }
            }

            string autoGeneratedPrefix = "//--> This block is auto-generated. Please don't change it!";
            string autoGeneratedPostfix = "//<-- End of auto-generated block";

			if (methodHeader >= appControllerLines.Count)
			{
				Debug.LogWarning("Can't patch Objective-C code (it may be alright). Method not found: " + method);
				return;
			}

            int braceCount = 0;
            int prefixLine = -1;
            int insertionLine = -1;
            for (int i = methodHeader; i < appControllerLines.Count; ++i)
            {
                string line = appControllerLines[i].Trim();
                if (line == autoGeneratedPrefix)
                {
					prefixLine = i;
                }
				else if (line == autoGeneratedPostfix)
                {
					appControllerLines.RemoveRange(prefixLine, i - prefixLine + 1);
					if (appControllerLines.Count > prefixLine && appControllerLines[prefixLine].Trim().Length == 0)
					{
						appControllerLines.RemoveAt(prefixLine);
					}

					break;
                }

                braceCount += CountOf(line, '{');

                int closingBracesCount = CountOf(line, '}');
                if (closingBracesCount > 0)
                {
                    braceCount -= closingBracesCount;

                    if (braceCount == 0)
                    {
                        //Method doesn't contain the patch yet.
                        break;
                    }
                }

                if (braceCount == 1 && insertionLine < 0)
                {
                    insertionLine = i + 1;
                }
            }

            //Let's patch.
			if (insertionLine < 0)
			{
				Debug.LogError("Can't patch Objective-C code. Unable to find a method start: " + method);
			}

            appControllerLines.Insert(insertionLine, autoGeneratedPrefix);
            appControllerLines.Insert(insertionLine + 1, autoGeneratedPostfix);
            appControllerLines.Insert(insertionLine + 2, "");
            appControllerLines.InsertRange(insertionLine + 1, addition);
        }

        private static int CountOf(string str, char ch)
        {
            int result = 0;
            foreach (char c in str)
            {
                if (c == ch)
                {
                    ++result;
                }
            }

            return result;
        }

        private static void CopyRaw(string pathToBuiltProject)
        {
            string inDir = Path.Combine(Settings.PluginsFullPath, "iOS/Raw");
            if (Directory.Exists(inDir))
            {
                string outDir = Path.Combine(pathToBuiltProject, "Data/Raw");
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }

                FileInfo[] fileInfos = new DirectoryInfo(inDir).GetFiles();
                if (fileInfos != null)
                {
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        if (!fileInfo.Name.EndsWith(".meta"))
                        {
                            File.Copy(fileInfo.FullName, Path.Combine(outDir, fileInfo.Name), true);
                        }
                    }
                }
            }
        }
    }
}

#endif
