using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 路径
/// </summary>
public class PathUtil
{

    /// <summary>
    /// 获取assetbundle的输出目录
    /// </summary>
    /// <returns></returns>
    public static string GetAssetBundleOutPath()
    {
        string outPath = getPlatformPath() + "/" + getPlatformName();
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);
        return outPath;
    }
    /// <summary>
    /// 自动获取对应平台的路径
    /// </summary>
    /// <returns></returns>
    private static string getPlatformPath()
    {
        switch (Application.platform)
        {

            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return Application.streamingAssetsPath;

            case RuntimePlatform.Android:
                return Application.persistentDataPath;
            default:
                return null;
        }
    }

    /// <summary>
    /// 获取对应平台名字
    /// </summary>
    /// <returns></returns>
    private static string getPlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "Windows";

            case RuntimePlatform.Android:
                return "Android";
            default:
                return null;
        }
    }
    /// <summary>
    /// 获取www协议的路径
    /// </summary>
    public static string GetWWWPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "file:///" + GetAssetBundleOutPath();

            case RuntimePlatform.Android:
                return "jar:file://" + GetAssetBundleOutPath();
            default:
                return null;
        }
    }
}
