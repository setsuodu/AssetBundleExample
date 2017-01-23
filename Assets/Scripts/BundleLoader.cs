using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleLoader : MonoBehaviour {

    public string bundleURL = "http://www.setsuodu.com/ABs/pikachu";
    public string assetName = "";
    AssetBundle bundle;
    //public Text mDebug;

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
            Debug.Log("error : " + download.error);
            //mDebug.text = "error : " + download.error;
        }
        else
        {
            Debug.Log("progress : " + download.progress);
            AssetBundle bundle = download.assetBundle;
            GameObject go = Instantiate(bundle.LoadAsset(assetName)) as GameObject;
            go.AddComponent<DoubleClickShow>();
            go.AddComponent<TouchSwipe>();
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
