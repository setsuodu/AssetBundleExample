using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadFromServer : MonoBehaviour
{
    private string dir
    {
        get
        {
            string path = Application.persistentDataPath + "/Windows/";
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            return path;
        }
    }

    private string baseURL
    {
        get
        {
            string url = string.Format("http://www.setsuodu.com/download/AssetBundle/{0}/{1}", bundlePlatform, patchName); // ../android/v1
            return url;
        }
    }
    private string mainAssetURL
    {
        get
        {
            string url = string.Format("{0}/{1}", baseURL, patchName); // ../android/v1/v1
            //Debug.Log(url);
            return url;
        }
    }
    private string assetURL
    {
        get
        {
            string url = string.Format("{0}/{1}.assetbundle", baseURL, labelName); // ../android/v1/assetLabel.assetbundle
            return url;
        }
    }
    [SerializeField] string patchName = "v1"; //补丁名称
    [SerializeField] string labelName = "prefabs/sofa_1"; //要加载资源的 assetLabel
    [SerializeField] BundlePlatform bundlePlatform = BundlePlatform.android;
    [SerializeField] AssetBundle bundle = null;
    [SerializeField] AssetBundleManifest manifest = null;

    void Start()
    {
        StartCoroutine(LoadManifest());
    }

    #region 加载远程

    IEnumerator LoadManifest()
    {
        WWW www = WWW.LoadFromCacheOrDownload(mainAssetURL, 0); // 下载到 \Users\Administrator\AppData\LocalLow\Unity\CompanyName_ProductName
        yield return www;
        AssetBundle ab = www.assetBundle;
        manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        www.Dispose(); //释放www内存
        ab.Unload(false); //释放assetbundle
    }

    // 云端加载AssetBundle，并实例化GameObject
    IEnumerator RemoteBundle(string assetlabel)
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();
        string bundleName = string.Format("prefabs/{0}.assetbundle", assetlabel);

        // 注意：GetAllDependencies会返回直接和间接关联的AssetBundle
        // 1. 加载依赖包
        string[] dependence = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependence.Length; i++)
        {
            string url0 = Path.Combine(baseURL, dependence[i]);
            Debug.Log("<color=blue>" + url0 + "</color>");
            WWW temp0 = WWW.LoadFromCacheOrDownload(url0, manifest.GetAssetBundleHash(dependence[i]));
            //while (!temp0.isDone) { }
            yield return temp0;
            AssetBundle depend = temp0.assetBundle;
            //AssetBundle depend = AssetBundle.LoadFromFile(Path.Combine(dir, dependence[i])); //从本地读取

            //注意：这里不需要手动LoadAsset
            //只需要加载AssetBundle即可
            //Asset会在加载其它关联Asset时自动加载
            bundleCacheList.Add(depend);
        }

        // 2. 加载prefab自己
        //string url1 = baseURL + bundleName;
        string url1 = assetURL;
        Debug.Log("<color=green>" + url1 + "</color>");
        WWW temp1 = WWW.LoadFromCacheOrDownload(url1, manifest.GetAssetBundleHash(bundleName));
        yield return temp1;
        AssetBundle bundle = temp1.assetBundle;
        //AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(dir, bundleName));
        bundleCacheList.Add(bundle);

        // 实例化
        Object asset = bundle.LoadAsset(assetlabel);
        GameObject go = Instantiate(asset) as GameObject;

        // 全部包释放掉
        for (int i = bundleCacheList.Count - 1; i >= 0; i--)
        {
            bundleCacheList[i].Unload(false);  //释放AssetBundle文件内存镜像
            //bundleCacheList[i].Unload(true); //释放AssetBundle文件内存镜像同时销毁所有已经Load的Assets内存对象
            bundleCacheList[i] = null;
            bundleCacheList.Remove(bundleCacheList[i]); //List中移除
        }
    }

    public void LoadAsset()
    {
        StartCoroutine(RemoteBundle("sofa_1")); // prefab/bundleName
    }

    public void ClearCache()
    {
        Caching.ClearCache();
    }

    #endregion

    IEnumerator Download()
    {
        WWW www = WWW.LoadFromCacheOrDownload(assetURL, 0);
        yield return www;
        AssetBundle ab = www.assetBundle;
        AssetBundleManifest manifest = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest");
        Debug.Log(manifest.GetAllAssetBundles().Length); //manifest中记录了所有资源（依赖关系），共19个
        www.Dispose(); //释放www内存
        //ab.Unload(false); //这里先不释放，后面还要用

        //解析依赖列表
        /*
        string[] dependsFile = manifest.GetAllDependencies(labelName);
        //Debug.LogFormat("有{0}个依赖", dependsFile.Length);
        //加载依赖
        AssetBundle[] dependsBundle = new AssetBundle[dependsFile.Length];
        for (int i = 0; i < dependsFile.Length; i++)
        {
            //Debug.Log(dependsFile[i]);
            //materials/sofa_1.unity3d
            //models/sofa_1.unity3d
            //textures/sofa_1.unity3d
            string downloadUrl = Cdn + dependsFile[i];
            Debug.Log(string.Format("depends{0}: {1}", i, dependsFile[i]));
            WWW dwww = WWW.LoadFromCacheOrDownload(downloadUrl, manifest.GetAssetBundleHash(dependsFile[i]));
            yield return dwww;
            dependsBundle[i] = dwww.assetBundle;
            dwww.Dispose(); //资源下载，要释放
            dependsBundle[i].LoadAllAssets();
        }
        */

        /*
        string mainUrl = Cdn + labelName; //http://www.miereco.com/ABs/android/prefabs/sofa_1.unity3d
        //WWW mwww = WWW.LoadFromCacheOrDownload(mainUrl, manifest.GetAssetBundleHash(labelName));
        WWW mwww = WWW.LoadFromCacheOrDownload(url, 0);
        yield return mwww;
        AssetBundle bundle = mwww.assetBundle;
        //AssetBundleManifest mani = (AssetBundleManifest)bundle.LoadAsset("AssetBundleManifest");
        //Debug.Log(mani.GetAllAssetBundles().Length);
        mwww.Dispose(); //资源下载，要释放

        Debug.Log(bundle.GetAllAssetNames()[0]);
        if (bundle.Contains("sofa_1.prefab"))
        {
            //确认AssetBundle中有指定预置对象
            Debug.Log("ok!");
            var asset = bundle.LoadAsset("sofa_1.unity3d"); //加载指定预置对象
            Instantiate(asset); //实例化该预置对象
        }
        else
        {
            Debug.Log("failed.");
        }
        bundle.Unload(false);
        */

        Debug.Log(ab.GetAllAssetNames()[0]);
        //assets/resources/prefabs/sofa_1.prefab
        if (ab.Contains("sofa_1.prefab"))
        {
            //确认AssetBundle中有指定预置对象
            Debug.Log("ok!");
            var ob = ab.LoadAsset("sofa_1.prefab"); //加载指定预置对象
            Instantiate(ob); //实例化该预置对象
        }
        else
        {
            Debug.Log("failed.");
        }

        ab.Unload(false); //释放assetbundle
    }

    // 本地加载AssetBundle，并实例化GameObject
    public GameObject LocalBundle(string assetlabel)
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();
        string bundleName = string.Format("prefabs/{0}.prefab", assetlabel);

        //注意：GetAllDependencies会返回直接和间接关联的AssetBundle
        //1. 加载依赖包
        string[] dependence = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependence.Length; ++i)
        {
            AssetBundle depend = AssetBundle.LoadFromFile(Path.Combine(dir, dependence[i])); //从本地读取
            //注意：这里不需要手动LoadAsset
            //只需要加载AssetBundle即可
            //Asset会在加载其它关联Asset时自动加载
            bundleCacheList.Add(depend);
        }

        //2. 加载prefab自己
        AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(dir, bundleName));
        bundleCacheList.Add(bundle);

        //实例化
        var asset = bundle.LoadAsset(assetlabel);
        GameObject go = Instantiate(asset) as GameObject;

        //包括模型，贴图，材质球...全部包释放掉
        for (int i = bundleCacheList.Count - 1; i >= 0; i--)
        {
            //Debug.Log("[Remove][" + i + "]" + bundleCacheList[i].name);
            bundleCacheList[i].Unload(false);  //释放AssetBundle文件内存镜像
            //bundleCacheList[i].Unload(true); //释放AssetBundle文件内存镜像同时销毁所有已经Load的Assets内存对象
            bundleCacheList[i] = null;
            bundleCacheList.Remove(bundleCacheList[i]); //List中移除
        }
        //Debug.Log(bundleCacheList.Count); //0
        return go;
    }
}

public enum BundlePlatform
{
    android = 0,
    ios = 1
}
