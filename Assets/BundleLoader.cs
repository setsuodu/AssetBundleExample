using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleLoader : MonoBehaviour {

    public string bundleURL = "http://www.setsuodu.com/ABs/pikachu";
    public string assetName = "pikachu";
    AssetBundle bundle;

    #region LoadFromCacheOrDownload下载或缓存方法
    void Start()
    {
        StartCoroutine(LoadBD());
    }

    private IEnumerator LoadBD()
    {
        WWW download = WWW.LoadFromCacheOrDownload(bundleURL, 0);
        yield return download;

        if (download.error != null)
        {
            Debug.Log(download.error);
        }
        else
        {
            if (!download.isDone)
            {
                Debug.Log(download.progress);
            }
            else
            {
                AssetBundle bundle = download.assetBundle;
                GameObject go = Instantiate(bundle.LoadAsset(assetName)) as GameObject;
            }
        }
    }
    #endregion

    #region WWW直接下载方法
    /*
    IEnumerator Start()
    {
        WWW www = new WWW(bundleURL);
        yield return www;
        if (www.error != null)
        {
            throw new System.Exception("There an error : " + www.error);
        }
        else
        {
            bundle = www.assetBundle;
        }
        bundleOBJ(wwwGameObject);
    }

    public void Spawn()
    {
        if (assetName == "")
        {
            Instantiate(bundle.mainAsset);
        }
        else
        {
            Instantiate(bundle.LoadAsset(assetName));
        }
    }

    public GameObject wwwGameObject;
    GameObject bundleOBJ(GameObject go)
    {
        if (assetName == "")
        {
            go = Instantiate(bundle.mainAsset) as GameObject;
        }
        else
        {
            go = Instantiate(bundle.LoadAsset(assetName)) as GameObject;
        }
        return go;
    }
    */
    #endregion
}
