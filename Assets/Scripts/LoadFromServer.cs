using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] AssetBundleManifest manifest = null;
    [SerializeField] AssetBundle bundle = null;

    void Start()
    {
        StartCoroutine(LoadManifest());

        //GameObject go = Resources.Load<GameObject>("Portal");
        //Instantiate(go);
    }

    #region 加载远程

    public Dictionary<string, Material> matDictionary = new Dictionary<string, Material>();

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
    IEnumerator RemoteBundle(string fileName)
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();
        string bundleName = string.Format("prefabs/{0}.assetbundle", fileName);
        //Debug.Log(bundleName);

        // 注意：GetAllDependencies会返回直接和间接关联的AssetBundle
        // 1. 加载依赖包
        string[] dependence = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependence.Length; i++)
        {
            string url0 = Path.Combine(baseURL, dependence[i]); //拼接依赖的路径
            Debug.Log("<color=blue>" + url0 + "</color>\n" + manifest.GetAssetBundleHash(dependence[i]));

            WWW temp0 = WWW.LoadFromCacheOrDownload(url0, manifest.GetAssetBundleHash(dependence[i]));
            yield return temp0;
            AssetBundle depend = temp0.assetBundle;
            bundleCacheList.Add(depend);

            // 注意：这里不需要手动LoadAsset
            // 只需要加载AssetBundle即可
            // Asset会在加载其它关联Asset时自动加载

            if (dependence[i].Contains("materials"))
            {
                //Debug.Log(dependence[i]);
                string key = dependence[i].Split('/')[1].Split('.')[0];
                mat = depend.LoadAsset<Material>(key);
                matDictionary.Add(key, mat); //16个
            }
        }

        // 2. 加载prefab自己
        string url1 = Path.Combine(baseURL, bundleName);
        Debug.Log("<color=green>" + url1 + "</color>");
        WWW temp1 = WWW.LoadFromCacheOrDownload(url1, manifest.GetAssetBundleHash(bundleName));
        yield return temp1;
        AssetBundle bundle = temp1.assetBundle;
        bundleCacheList.Add(bundle);

        // 实例化
        Object asset = bundle.LoadAsset(fileName);
        GameObject go = Instantiate(asset) as GameObject;
        if (!go.GetComponent<FixShader>())
            go.AddComponent<FixShader>();

        // FixShader
        for (int i = 0; i < go.GetComponentsInChildren<MeshRenderer>().Length; i++)
        {
            Material[] mats = go.GetComponentsInChildren<MeshRenderer>()[i].materials;
            for (int t = 0; t < mats.Length; t++)
            {
                Debug.Log(mats[t].name);
                string key = mats[t].name.Split(' ')[0];
                Material mat = matDictionary[key];
                go.GetComponentsInChildren<MeshRenderer>()[i].materials[t] = mat;
            }
        }

        yield return new WaitForEndOfFrame(); //必须加

        // 全部包释放掉
        for (int i = bundleCacheList.Count - 1; i >= 0; i--)
        {
            //bundleCacheList[i].Unload(true); //释放AssetBundle文件内存镜像同时销毁所有已经Load的Assets内存对象
            //bundleCacheList[i].Unload(false);  //释放AssetBundle文件内存镜像
            bundleCacheList[i] = null;
            bundleCacheList.Remove(bundleCacheList[i]); //List中移除
        }
    }

    public void LoadAsset()
    {
        StartCoroutine(RemoteBundle("portal")); // prefab/bundleName
    }

    public void ClearCache()
    {
        Caching.ClearCache();
    }

    #endregion

    #region 各种类型加载测试

    public Texture2D t2d = null;
    public Material mat = null;
    public GameObject sp;
    public Texture2D lightmap = null;

    [ContextMenu("LoadTexture2D")]
    void LoadTexture2D()
    {
        StartCoroutine(OnLoadTexture2D());
    }

    IEnumerator OnLoadTexture2D()
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();

        string fileName = "bed";
        string bundleName = string.Format("textures/{0}.assetbundle", fileName); // textures/bed.assetbundle
        string[] dependence = manifest.GetAllDependencies(bundleName);
        Debug.Log("dependence:" + dependence.Length);

        string url1 = Path.Combine(baseURL, bundleName);
        Debug.Log(url1);

        WWW temp1 = WWW.LoadFromCacheOrDownload(url1, manifest.GetAssetBundleHash(bundleName));
        yield return temp1;
        bundle = temp1.assetBundle;
        //Debug.Log(bundle.GetAllAssetNames().Length);
        //Debug.Log(bundle.GetAllAssetNames()[0]);

        t2d = bundle.LoadAsset<Texture2D>("bed");
        bundleCacheList.Add(bundle);

        yield return new WaitForEndOfFrame();
        for (int i = bundleCacheList.Count - 1; i >= 0; i--)
        {
            bundleCacheList[i].Unload(false);  //释放AssetBundle文件内存镜像
            bundleCacheList[i] = null;
            bundleCacheList.Remove(bundleCacheList[i]); //List中移除
        }
    }

    [ContextMenu("LoadMaterial")]
    void LoadMaterial()
    {
        StartCoroutine(OnLoadMaterial());
    }

    IEnumerator OnLoadMaterial()
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();

        string fileName = "bed";
        string bundleName = string.Format("materials/{0}.assetbundle", fileName);

        string[] dependence = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependence.Length; i++)
        {
            string url0 = Path.Combine(baseURL, dependence[i]); //拼接依赖的路径
            Debug.Log("<color=blue>" + url0 + "</color>\n" + manifest.GetAssetBundleHash(dependence[i]));

            WWW temp0 = WWW.LoadFromCacheOrDownload(url0, manifest.GetAssetBundleHash(dependence[i]));
            yield return temp0;
            AssetBundle depend = temp0.assetBundle;

            // 注意：这里不需要手动LoadAsset
            // 只需要加载AssetBundle即可
            // Asset会在加载其它关联Asset时自动加载
            bundleCacheList.Add(depend);
        }

        string url1 = Path.Combine(baseURL, bundleName);
        Debug.Log("<color=green>" + url1 + "</color>");

        WWW temp1 = WWW.LoadFromCacheOrDownload(url1, manifest.GetAssetBundleHash(bundleName));
        yield return temp1;
        bundle = temp1.assetBundle;

        mat = bundle.LoadAsset<Material>(fileName);
        bundleCacheList.Add(bundle);

        sp.GetComponent<MeshRenderer>().material = mat;

        yield return new WaitForEndOfFrame();
        for (int i = bundleCacheList.Count - 1; i >= 0; i--)
        {
            bundleCacheList[i].Unload(false);  //释放AssetBundle文件内存镜像
            bundleCacheList[i] = null;
            bundleCacheList.Remove(bundleCacheList[i]); //List中移除
        }
    }

    [ContextMenu("SetLightmap")]
    void SetLightmap()
    {
        Debug.Log(LightmapSettings.lightmaps.Length); //1
        LightmapData[] lightmapData = new LightmapData[LightmapSettings.lightmaps.Length];
        if (LightmapSettings.lightmaps.Length == 0)
        {
            lightmapData = new LightmapData[1];
        }
        for (int i = 0; i < lightmapData.Length; i++)
        {
            lightmapData[i] = new LightmapData();
            lightmapData[i].lightmapColor = Resources.Load<Texture2D>("Lightmap-0_comp_dark"); //string_maps[i]
        }
        LightmapSettings.lightmaps = lightmapData;
    }

    [ContextMenu("LoadLightmap")]
    void LoadLightmap()
    {
        StartCoroutine(OnLoadLighmap());
    }

    IEnumerator OnLoadLighmap()
    {
        List<AssetBundle> bundleCacheList = new List<AssetBundle>();

        string fileName = "Lightmap-0_comp_light";
        string bundleName = string.Format("lightmap/{0}.assetbundle", fileName);

        string[] dependence = manifest.GetAllDependencies(bundleName);
        for (int i = 0; i < dependence.Length; i++)
        {
            string url0 = Path.Combine(baseURL, dependence[i]); //拼接依赖的路径
            Debug.Log("<color=blue>" + url0 + "</color>\n" + manifest.GetAssetBundleHash(dependence[i]));

            WWW temp0 = WWW.LoadFromCacheOrDownload(url0, manifest.GetAssetBundleHash(dependence[i]));
            yield return temp0;
            AssetBundle depend = temp0.assetBundle;

            // 注意：这里不需要手动LoadAsset
            // 只需要加载AssetBundle即可
            // Asset会在加载其它关联Asset时自动加载
            bundleCacheList.Add(depend);
        }

        string url1 = Path.Combine(baseURL, bundleName);
        Debug.Log("<color=green>" + url1 + "</color>");

        WWW temp1 = WWW.LoadFromCacheOrDownload(url1, manifest.GetAssetBundleHash(bundleName));
        yield return temp1;
        bundle = temp1.assetBundle;
        Debug.Log(bundle.GetAllAssetNames().Length + ":" + bundle.GetAllAssetNames()[0]);

        lightmap = bundle.LoadAsset<Texture2D>(fileName);
        bundleCacheList.Add(bundle);

        yield return new WaitForEndOfFrame();
        for (int i = bundleCacheList.Count - 1; i >= 0; i--)
        {
            bundleCacheList[i].Unload(false);  //释放AssetBundle文件内存镜像
            bundleCacheList[i] = null;
            bundleCacheList.Remove(bundleCacheList[i]); //List中移除
        }
    }

    #endregion
}

public enum BundlePlatform
{
    android = 0,
    ios = 1
}
