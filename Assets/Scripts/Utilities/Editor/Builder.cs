using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Builder
{
    [MenuItem("Builder/Build Standalone")]
    public static void BuildStandalone()
    {
        string filePath = Application.dataPath.Replace("Assets", string.Empty) + "Standalone/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string fileName = "puccawars.exe";

        BuildPipeline.BuildPlayer(
            BuildScenePaths,
            filePath + fileName,
            BuildTarget.StandaloneWindows,
            BuildOptions.None);
    }

    [MenuItem("Builder/Build Android_QA")]
    public static void BuildAndroid_QA()
    {
        PlayerSettings.Android.keystoreName = "puccawarslive.keystore";
        PlayerSettings.Android.keystorePass = "dpaTlem@1";
        PlayerSettings.Android.keyaliasName = "puccawarslive";
        PlayerSettings.Android.keyaliasPass = "dpaTlem@1";

        string filePath = Application.dataPath.Replace("Assets", string.Empty) + "Android_QA/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string fileName = "puccawars_QA.apk";

        Kernel.gameServerType = GAME_SERVER_TYPE.TYPE_QA;

        BuildPipeline.BuildPlayer(
            BuildScenePaths,
            filePath + fileName,
            BuildTarget.Android,
            BuildOptions.None);
    }


    [MenuItem("Builder/Build Android_AdHoc")]
    public static void BuildAndroid_AdHoc()
    {
        PlayerSettings.Android.keystoreName = "puccawarslive.keystore";
        PlayerSettings.Android.keystorePass = "dpaTlem@1";
        PlayerSettings.Android.keyaliasName = "puccawarslive";
        PlayerSettings.Android.keyaliasPass = "dpaTlem@1";

        string filePath = Application.dataPath.Replace("Assets", string.Empty) + "Android_ADHOC/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string fileName = "puccawars_ADHOC.apk";

        Kernel.gameServerType = GAME_SERVER_TYPE.TYPE_ADHOC;

        BuildPipeline.BuildPlayer(
            BuildScenePaths,
            filePath + fileName,
            BuildTarget.Android,
            BuildOptions.None);
    }

    [MenuItem("Builder/Build Android_Release")]
    public static void BuildAndroid_Release()
    {
        PlayerSettings.Android.keystoreName = "puccawarslive.keystore";
        PlayerSettings.Android.keystorePass = "dpaTlem@1";
        PlayerSettings.Android.keyaliasName = "puccawarslive";
        PlayerSettings.Android.keyaliasPass = "dpaTlem@1";

        string filePath = Application.dataPath.Replace("Assets", string.Empty) + "Android_RELEASE/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string fileName = "puccawars_RELEASE.apk";

        Kernel.gameServerType = GAME_SERVER_TYPE.TYPE_RELEASE;

        BuildPipeline.BuildPlayer(
            BuildScenePaths,
            filePath + fileName,
            BuildTarget.Android,
            BuildOptions.None);

        System.Diagnostics.Process.Start(filePath);
    }


    [MenuItem("Builder/Build iOS")]
    public static void BuildiOS()
    {
        string filePath = Application.dataPath.Replace("Assets", string.Empty) + "iOS/";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
        string fileName = "puccawars";

        BuildPipeline.BuildPlayer(BuildScenePaths,
                                  filePath + fileName,
                                  BuildTarget.iOS,
                                  BuildOptions.None);
    }

    static string[] BuildScenePaths
    {
        get
        {
            List<string> scenePaths = new List<string>();
            foreach (var item in EditorBuildSettings.scenes)
            {
                if (!item.enabled)
                {
                    continue;
                }

                scenePaths.Add(item.path);
            }

            return scenePaths.ToArray();
        }
    }
}
