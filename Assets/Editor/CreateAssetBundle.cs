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

        #region Compress
        // 压缩
        string path = Application.dataPath + "/ABs";
        DirectoryInfo folders = new DirectoryInfo(path);

        DirectoryInfo[] dirInfo = folders.GetDirectories();
        //Debug.Log(dirInfo.Length); //文件夹数，不算文件

        FileInfo[] fileInfo = folders.GetFiles();
        Debug.Log("File Compressed: " + fileInfo.Length); //文件数，不算文件夹

        //遍历文件夹
        List<FileInfo> ignoreList = new List<FileInfo>();
        FileInfo[] fileArray = folders.GetFiles();
        for (int i = 0; i < fileArray.Length; i++)
        {
            ignoreList.Add(fileArray[i]);
        }

        for (int i = 0; i < dirInfo.Length; i++)
        {
            fileArray = dirInfo[i].GetFiles();
            for (int t = 0; t < fileArray.Length; t++)
            {
                ignoreList.Add(fileArray[t]);
            }
        }
        foreach (FileInfo fi in ignoreList)
        {
            if (fi.Name.Contains(".meta"))
                fi.Delete();
        }

        MyZip zip = new MyZip();
        zip.ZipFolder(Application.dataPath + "/ABs", Application.streamingAssetsPath + "/test_android.zip");
        #endregion

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
        
        #region Compress
        // 压缩
        string path = Application.dataPath + "/ABs";
        DirectoryInfo folders = new DirectoryInfo(path);

        DirectoryInfo[] dirInfo = folders.GetDirectories();
        //Debug.Log(dirInfo.Length); //文件夹数，不算文件

        FileInfo[] fileInfo = folders.GetFiles();
        Debug.Log("File Compressed: " + fileInfo.Length); //文件数，不算文件夹

        //遍历文件夹
        List<FileInfo> ignoreList = new List<FileInfo>();
        FileInfo[] fileArray = folders.GetFiles();
        for (int i = 0; i < fileArray.Length; i++)
        {
            ignoreList.Add(fileArray[i]);
        }

        for (int i = 0; i < dirInfo.Length; i++)
        {
            fileArray = dirInfo[i].GetFiles();
            for (int t = 0; t < fileArray.Length; t++)
            {
                ignoreList.Add(fileArray[t]);
            }
        }
        foreach (FileInfo fi in ignoreList)
        {
            if (fi.Name.Contains(".meta"))
                fi.Delete();
        }

        MyZip zip = new MyZip();
        zip.ZipFolder(Application.dataPath + "/ABs", Application.streamingAssetsPath + "/test_x86.zip");
        #endregion

        AssetDatabase.Refresh();
    }

    [MenuItem("Example/Clean Cache")]
    static void CleanCache()
    {
        Caching.CleanCache();
    }
}
