using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BundleLoadExample : MonoBehaviour
{
    //public string path = "http://www.setsuodu.com/ABs/ABs";
    public string path;

    void Awake ()
    {
        path = "http://www.setsuodu.com/ABs";
        //path = "file://" + Application.dataPath;
        //Debug.Log(Application.persistentDataPath + "/ABs");
    }
	
	void Update ()
    {
		
	}

    public void LoadBundle()
    {
        //Caching.CleanCache();
        //StartCoroutine(LoadFromAssetBundle("JiJia_1", "assetbundleprefab", "/StreamingAssets/"));
        StartCoroutine(LoadFromAssetBundle("Pikachu", "pikachu", "/"));
    }

    IEnumerator DownloadBundle()
    {
        WWW www = WWW.LoadFromCacheOrDownload(path, 0);

        yield return www;

        if (www.error != null)
        {
            Debug.Log(www.error);
            Debug.Log("error");
        }
        else
        {
            AssetBundle myLoadedAssetBundle = www.assetBundle;
            var asset = myLoadedAssetBundle.mainAsset;
            //yield return Instantiate(asset);
            //myLoadedAssetBundle.Unload(false);
            GameObject.Instantiate(asset);
            Debug.Log("loaded");
        }
    }

    string assetBundleName = "ABs";



    IEnumerator LoadFromAssetBundle(string name, string assetbundleName, string strPath)
    {
        List<AssetBundle> assetBundleList = new List<AssetBundle>();
        //WWW www = WWW.LoadFromCacheOrDownload(path + "/StreamingAssets/StreamingAssets", 0);
        WWW www = WWW.LoadFromCacheOrDownload(path + "/ABs", 0);

        while (!www.isDone)
        {
            Debug.Log("Loading AssetBundleManifest");
            yield return null;
        }
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }
        AssetBundle abManifet = www.assetBundle;
        assetBundleList.Add(abManifet);
        AssetBundleManifest abManifest = abManifet.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        foreach (var n in abManifest.GetAllDependencies(assetbundleName.ToLower()))
        {
            WWW wwwABM = WWW.LoadFromCacheOrDownload(path + strPath + n, 0);
            yield return wwwABM;

            while (!wwwABM.isDone)
            {
                Debug.Log("Loading Dependencies ");
                yield return null;
            }
            if (!string.IsNullOrEmpty(wwwABM.error))
            {
                Debug.Log(wwwABM.error);
                yield break;
            }
            AssetBundle ab_temp = wwwABM.assetBundle;
            assetBundleList.Add(ab_temp);
        }

        WWW wwwPrefab = WWW.LoadFromCacheOrDownload(path + strPath + assetbundleName.ToLower(), 0);
        yield return wwwPrefab;
        while (!wwwPrefab.isDone)
        {
            Debug.Log("Loading Prefab");
            yield return null;
        }

        if (!string.IsNullOrEmpty(wwwPrefab.error))
        {
            Debug.Log(wwwPrefab.error);
            yield break;
        }
        else
        {
            AssetBundle abPrefab = wwwPrefab.assetBundle;
            assetBundleList.Add(abPrefab);
            GameObject g = abPrefab.LoadAsset<GameObject>(name);
            Instantiate(g);
        }
        foreach (var abl in assetBundleList)
        {
            if (abl != null)
            {
                abl.Unload(false);
            }
        }
    }
}
