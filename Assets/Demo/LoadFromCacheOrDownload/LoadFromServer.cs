using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadFromServer : MonoBehaviour
{
    public AssetBundleManifest manifest = null;
    private string dir
    {
        get
        {
            string path = Application.persistentDataPath + "/Windows/";
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            return path;
        }
    }
    private string absUrl
    {
        get
        {
            string url = string.Format("http://www.miereco.com/ABs/{0}/{0}", Application.platform); //Android/WindowsEditor
            return url;
        }
    }
    string rootUrl = "http://www.miereco.com/ABs/android/";
    string url = "http://www.miereco.com/ABs/android/prefabs/sofa_1.unity3d"; //prefab资源路径
    string labelName = "prefabs/sofa_1.unity3d"; //要加载的prefab的label名

    void Start()
    {

    }

    public void LoadAsset()
    {
        StartCoroutine(LoadManifest());
    }

    public void ClearCache()
    {
        Caching.ClearCache();
    }

    IEnumerator Download()
    {
        //WWW www = WWW.LoadFromCacheOrDownload(absUrl, 0);
        WWW www = WWW.LoadFromCacheOrDownload(url, 0);
        yield return www;
        AssetBundle ab = www.assetBundle;
        AssetBundleManifest manifest = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest");
        //Debug.Log(manifest.GetAllAssetBundles().Length); //manifest中记录了所有资源（依赖关系），共19个
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

        ///*
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
        //*/

        ab.Unload(false); //释放assetbundle
    }

    IEnumerator LoadManifest()
    {
        Debug.Log(absUrl);

        WWW www = WWW.LoadFromCacheOrDownload(absUrl, 0);
        yield return www;
        AssetBundle ab = www.assetBundle;
        manifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        www.Dispose(); //释放www内存
        ab.Unload(false); //释放assetbundle

        //GameObject go = LocalBundle("sofa_1");
        //GameObject go = CloudBundle("sofa_1");
        yield return CloudBundle("sofa_1");
    }

    //本地加载AssetBundle，并实例化GameObject
    public GameObject LocalBundle(string assetlabel)
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();
        string bundleName = string.Format("prefabs/{0}.pre", assetlabel);

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

    //云端加载AssetBundle，并实例化GameObject
    public IEnumerator CloudBundle(string assetlabel)
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();
        string bundleName = string.Format("prefabs/{0}.pre", assetlabel);

        //注意：GetAllDependencies会返回直接和间接关联的AssetBundle
        //1. 加载依赖包
        string[] dependence = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependence.Length; i++)
        {
            string url0 = rootUrl + dependence[i];
            Debug.Log(url0);
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

        //2. 加载prefab自己
        string url1 = rootUrl + bundleName;
        WWW temp1 = WWW.LoadFromCacheOrDownload(url1, manifest.GetAssetBundleHash(bundleName));
        //while (!temp1.isDone) { }
        yield return temp1;
        AssetBundle bundle = temp1.assetBundle;
        //AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(dir, bundleName));
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
        //return go;
    }

}
