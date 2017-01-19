using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CreateAssetBundle : Editor
{
    [MenuItem("Example/Build Android")]
    static void BuildAll()
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
    static void BuildABs()
    {
        string outputPath = Application.dataPath + "/ABs";
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        // Put the bundles in a folder called "ABs" within the Assets folder.
        BuildPipeline.BuildAssetBundles("Assets/ABs", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }

    [MenuItem("Example/Clean Cache")]
    static void CleanCache()
    {
        Caching.CleanCache();
    }
}
