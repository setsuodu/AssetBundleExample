/*
 * 流程：
 1.访问API，解析得到：
    a.当前官方MD5码。
    b.本平台ABs的zip包下载地址。[←]
 2.访问本地文件夹，是否有文件存在：
    2-1.存在，比对MD5码：
        2-1-1.MD5一致，不操作。
        2-1-2.MD5不同，下载，覆盖文件。
    2-2.不存在，下载，直接写入文件。
 3.解压
 4.响应：LoadAsset(), Instanciate()
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class md5 : MonoBehaviour
{
    //API路径，网络资源下载地址，本地读写路径
    private string apiUrl, cloudUrl, localUrl;
    public string fileName;
    public Text mDebug;

    void Start ()
    {
        //apiUrl = "http://www.setsuodu.com/ABs/" + "api.json";
        //cloudUrl = "http://www.setsuodu.com/ABs/" + fileName + ".zip";
        apiUrl = "http://localhost/ABs/" + "api.json";
        cloudUrl = "http://localhost/ABs/" + fileName + ".zip";
        localUrl = Application.streamingAssetsPath + "/" + fileName + ".zip";
        getFileHash(localUrl);

        StartCoroutine(getJSON());
    }

    void Update()
    {
        if (isDebug)
        {
            if (www.progress < 1)
            {
                Debug.Log("111");
                mDebug.text = (www.progress * 100f).ToString("f0") + "%";
            }
            else if (www.progress >= 1)
            {
                Debug.Log("222 : " + myZip.progressOverall);
                mDebug.text = myZip.progressOverall.ToString() + "%";
            }
        }
    }

    //计算某文件MD5码
    public static string getFileHash(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open);
            int len = (int)fs.Length;
            byte[] data = new byte[len];
            fs.Read(data, 0, len);
            fs.Close();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(data);
            string fileMD5 = "";
            foreach (byte b in result)
            {
                fileMD5 += Convert.ToString(b, 16);
            }
            Debug.Log(fileMD5);
            return fileMD5;
        }

        catch (FileNotFoundException e)
        {
            Debug.Log(e.Message);
            return "";
        }
    }

    WWW www; //放在外部方便被访问
    bool isDebug = false; //控制下载进程显示开关

    //访问api得到官方MD5值，网络资源下载地址（从json获取比较好，便于大量文件管理）
    IEnumerator getJSON()
    {
        www = new WWW(apiUrl);
        Debug.Log(apiUrl);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            APIJson data = JsonUtility.FromJson<APIJson>(www.text);
            //Debug.Log("hash : " + data.hash + " zip : " + data.zip); //得到官方MD5值，网络资源下载地址

            string filePath = Application.temporaryCachePath + "/" + fileName + ".zip";
            Debug.Log(filePath); //方便找本地路径

            //文件已经存在
            if (File.Exists(filePath))
            {
                //执行MD5对比
                if (data.hash == getFileHash(filePath)) //结果相同，跳过
                {
                    Debug.Log("当前为最新版本");
                }
                else //结果不同，下载，覆盖写入。
                {
                    Debug.Log("有新版本，请更新");
                    www = new WWW(data.zip);
                    isDebug = true; //显示下载进度
                    yield return www;
                    if (www.isDone)
                    {
                        Debug.Log("is Done");
                        isDebug = false; //下载完成，不在刷新下载进度
                    }

                    FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                    fs.Write(www.bytes, 0, www.bytesDownloaded);
                    fs.Close();

                    //UnZip(); //覆盖完成，解压
                    Invoke("UnZip", 1); //等待一下再执行
                }
            }
            //文件不存在，直接下载、写入
            else
            {
                www = new WWW(data.zip);
                isDebug = true; //显示下载进度
                yield return www;
                if (www.isDone)
                {
                    Debug.Log("is Done");
                    isDebug = false; //下载完成，不在刷新下载进度
                }

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                fs.Write(www.bytes, 0, www.bytesDownloaded);
                fs.Close();

                //UnZip(); //首次下载完成，解压
                Invoke("UnZip", 1); //等待一下再执行
            }
            //存在问题：
            //不要每次解压；
            //但解压中途退出，导致不再解压!!!
        }
        else
        {
            Debug.Log(www.error);
        }
    }

    #region zip解压缩

    MyZip myZip = new MyZip();
    public string zipUrl;

    public void UnZip()
    {
        isDebug = true;
        string inputPath = Application.temporaryCachePath + "/" + fileName + ".zip";
        string outputPath = Application.temporaryCachePath;
        //input path, output path
        myZip.UnZipFile(inputPath, outputPath);
    }

    #endregion

    #region JSON解析

    [Serializable]
    public class APIJson
    {
        //要与api.json里的键名保持一致
        public string hash;
        public string zip;
    }

    #endregion
}
