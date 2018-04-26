﻿using System.IO;
using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>  
/// 把Resource下的资源打包成.unity3d 到StreamingAssets目录下  
/// </summary>  
public class Builder : Editor
{
    public static string sourcePath = Application.dataPath + "/Resources";
    const string AssetBundlesOutputPath = "Assets/StreamingAssets";

    [MenuItem("Tools/AssetBundle Build")]
    public static void BuildAssetBundle()
    {
        ClearAssetBundlesName();

        Pack(sourcePath);

        string outputPath = Path.Combine(AssetBundlesOutputPath, Platform.GetPlatformFolder(EditorUserBuildSettings.activeBuildTarget));
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        //根据BuildSetting里面所激活的平台进行打包  
        BuildPipeline.BuildAssetBundles(outputPath, 0, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
        Debug.Log("打包完成");
    }

    /// <summary>  
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包  
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下  
    /// </summary>  
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
    }

    static void Pack(string source)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;
        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                Pack(files[i].FullName);
            }
            else
            {
                if (!files[i].Name.EndsWith(".meta"))
                {
                    file(files[i].FullName);
                }
            }
        }
    }

    static void file(string source)
    {
        string _source = Replace(source);
        string _assetPath = "Assets" + _source.Substring(Application.dataPath.Length);
        string _assetPath2 = _source.Substring(Application.dataPath.Length + 1);
        //Debug.Log (_assetPath);
        //Assets/Resources/Materials/shadow_1.mat
        string[] arr = _assetPath.Split('/');
        string suffix = "." + arr[arr.Length - 2].Substring(0, 3).ToLower();
        //Debug.Log(suffix);

        //在代码中给资源设置AssetBundleName
        AssetImporter assetImporter = AssetImporter.GetAtPath(_assetPath);
        string assetName = _assetPath2.Substring(_assetPath2.IndexOf("/") + 1);

        //变更后缀，父文件夹的头三个字母小写
        //assetName = assetName.Replace(Path.GetExtension(assetName), ".unity3d");
        assetName = assetName.Replace(Path.GetExtension(assetName), suffix);
        //Debug.Log(assetName);
        //Materials/shadow_1.unity3d

        assetImporter.assetBundleName = assetName;
    }

    static string Replace(string s)
    {
        return s.Replace("\\", "/");
    }
}

public class Platform
{
    public static string GetPlatformFolder(BuildTarget target)
    {
        switch (target)
        {
            case BuildTarget.Android:
                return "Android";
            case BuildTarget.iOS:
                return "IOS";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "Windows";
            default:
                return null;
        }
    }
}
