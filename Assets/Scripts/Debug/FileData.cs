using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileData : MonoBehaviour
{
    public List<DirectoryInfo> folderList = new List<DirectoryInfo>();

    void Start()
    {
        string path = Application.dataPath + "/ABs";
        Debug.Log(files(path).Count);
    }

    List<FileInfo> files(string path)
    {
        List<FileInfo> ignoreList = new List<FileInfo>();

        DirectoryInfo folders = new DirectoryInfo(path);
        DirectoryInfo[] dirInfo = folders.GetDirectories();
        Debug.Log(dirInfo.Length); //文件夹数，不算文件

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
        return ignoreList;
    }
}
