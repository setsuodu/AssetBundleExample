using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CreateAssetBundle : Editor
{
    [MenuItem("Example/Build Android")]
    static void BuildAndroid()
    {
        string outputPath = Application.dataPath + "/ABs";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        BuildPipeline.BuildAssetBundles("Assets/ABs", BuildAssetBundleOptions.None, BuildTarget.Android);
        AssetDatabase.Refresh();
    }

    [MenuItem("Example/Build Win64")]
    static void BuildWin64()
    {
        string outputPath = Application.dataPath + "/ABs";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/ABs", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        // 压缩
        MyZip zip = new MyZip();
        zip.ZipFolder(Application.dataPath + "/ABs", Application.streamingAssetsPath + "/test.zip");

        AssetDatabase.Refresh();
    }

    [MenuItem("Example/Clean Cache")]
    static void CleanCache()
    {
        Caching.CleanCache();
    }

    static void Loader()
    {
    }
}
