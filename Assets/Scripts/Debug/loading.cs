using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class loading : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadStreamingAssets("tripod", "tripodprefab"));
    }

    public IEnumerator LoadStreamingAssets(string fileName, string assetName, int version = 1)
    {
        #region Platform path
#if UNITY_ANDROID && !UNITY_EDITOR
        string url = " jar: file://" + Application.dataPath + "!/assets/" + fileName;
        Debug.Log(" AndroidUrl: " +url);
#elif UNITY_IPHONE && !UNITY_EDITOR
    string url = "file://" + Application.dataPath + "/Raw/" + fileName;
#else
        string url = "file://" + Application.streamingAssetsPath + "/" + fileName; // Application.dataPath + "/StreamingAssets/" + fileName
#endif
        Debug.Log("AssetBundleUrl: " + url);
        #endregion

        // キャッシュシステムの準備が完了するのを待ちます
        while (!Caching.ready)
            yield return null;

        // 同じバージョンが存在する場合はアセットバンドルをキャッシュからロードするか、またはダウンロードしてキャッシュに格納します。
        using (WWW www = WWW.LoadFromCacheOrDownload(url, version))
        {
            yield return www;
            if (www.error != null)
            {
                throw new Exception("WWWダウンロードにエラーがありました:" + www.error);
            }

            AssetBundle bundle = www.assetBundle;
            if (assetName == "")
                Instantiate(bundle.mainAsset);
            else
                Instantiate(bundle.LoadAsset(assetName));
            // メモリ節約のため圧縮されたアセットバンドルのコンテンツをアンロード
            bundle.Unload(false);

        } // memory is freed from the web stream (www.Dispose() gets called implicitly)

        Debug.Log(Caching.IsVersionCached(url, 1));
        Debug.Log("DownloadAndCache end");
    }
}
