/*
 async works: 
    download a zip file → decompress zip and get ABs → loadABs by DownloadOrFromCache
*/
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleLoader : MonoBehaviour
{
    public string CloudURL = "http://www.setsuodu.com/ABs/";
    public string bundleURL, fileName, assetName, zipName, ZipURL;
    AssetBundle bundle;
    WWW download, downloadOrCache;
    public bool isDownloadZip, isDecompress, isDownloadOrCache = false;
    float percent, progress;
    public Text mDownload, mDecompress, mDownloadOrCache;

    MyZip zip = new MyZip();
    float testProgressOverall, testProgress;

    public void getZipURL(string url)
    {
        ZipURL = "http://192.168.0.227:9010/" + url;
    }

    // Listen the state of async works
    void Update()
    {
        if (isDownloadZip)
        {
            percent = download.progress * 100;
            mDownload.text = "Download: " + percent.ToString("f0") + "%";
        }

        if ((testProgressOverall != zip.progressOverall) || (testProgress != zip.progress) && !isDecompress)
        {
            testProgressOverall = zip.progressOverall;
            testProgress = zip.progress;
            //mDecompress.text = string.Format("总进度: {0}%, 单个文件进度: {1}%", zip.progressOverall, zip.progress);
            mDecompress.text = string.Format("Progress: {0}%", zip.progressOverall);
            if (testProgress == 100)
            {
                isDecompress = true;
            }
        }

        if (progress < 100f && isDownloadOrCache)
        {
            if (downloadOrCache.error == null)
            {
                progress = downloadOrCache.progress * 100;
                //Debug.Log("progress : " + progress.ToString("f0") + "%");
                mDownloadOrCache.text = "DownloadOrCache: " + progress.ToString("f0") + "%";
            }
            else if (downloadOrCache.error != null)
            {
                mDownloadOrCache.text = downloadOrCache.error;
            }
        }
    }

    IEnumerator WriteZip()
    {
        Stream outStream = File.Create(Application.persistentDataPath + "/test.zip");
        //download = new WWW(CloudURL + zipName + ".zip"); getZipURL
        //Debug.Log(CloudURL + zipName + ".zip");
        download = new WWW(ZipURL);
        Debug.Log(ZipURL);

        yield return download;

        if (download.error != null)
        {
            Debug.Log(download.error);
        }
        else
        {
            if (download.isDone)
            {
                byte[] buffer = download.bytes;
                outStream.Write(buffer, 0, buffer.Length);
                outStream.Close();

                isDownloadZip = true;
                Debug.Log("down");
            }
        }
    }

    // 解压
    IEnumerator Zip()
    {
        StartCoroutine(WriteZip());
        yield return new WaitUntil(functionDownload); zip.UnZipFile(Application.persistentDataPath + "/" + zipName + ".zip", Application.persistentDataPath + "/");
    }

    void DecompressZip()
    {
        StartCoroutine(Zip());
    }

    private IEnumerator DownloadAndCache(string _fileName, string _assetName, int version = 1)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("jar:file://" + Application.persistentDataPath + "/" + _fileName);
        //bundleURL = "jar: file://" + Application.dataPath + "!/assets/" + _fileName; //只读
        bundleURL = "file://" + Application.persistentDataPath + "/" + _fileName;

#elif UNITY_IPHONE && !UNITY_EDITOR
        bundleURL = "file://" + Application.dataPath + "/Raw/" + _fileName;
#else
        bundleURL = "file://" + Application.streamingAssetsPath + "/" + _fileName; 
        //bundleURL = "file://" + Application.persistentDataPath + "/" + _fileName;
        //bundleURL = "file://" + Application.temporaryCachePath + "/" + _fileName; //不行，报错
#endif
        Debug.Log("AssetBundleUrl: " + bundleURL);

        downloadOrCache = WWW.LoadFromCacheOrDownload(bundleURL, version);
        yield return downloadOrCache;

        if (downloadOrCache.error != null)
        {
            Debug.Log("error : " + downloadOrCache.error);
        }
        else
        {
            AssetBundle bundle = downloadOrCache.assetBundle;
            GameObject go = Instantiate(bundle.LoadAsset(_assetName)) as GameObject;
            go.AddComponent<DoubleClickShow>();
            go.AddComponent<TouchSwipe>();
        }
    }

    public void Spawn()
    {
        if (isDownloadOrCache) return;
        else
        {
            StartCoroutine(DownloadAndCache("tripod", "tripodprefab"));
            isDownloadOrCache = true;
        }
    }

    IEnumerator TaskRows()
    {
        DecompressZip();
        yield return new WaitUntil(functionDecompress);
        Spawn();
    }

    public void DoTaskRows()
    {
        StartCoroutine(TaskRows());
    }

    bool functionDownload()
    {
        return isDownloadZip;
    }

    bool functionDecompress()
    {
        return isDecompress;
    }
}
