using System;
using System.Collections;
using UnityEngine;

public class LoadAbFiles : MonoBehaviour
{
    private static LoadAbFiles single = null;

    void Awake()
    {
        if (single == null)
            single = this;
    }

    void Start()
    {
        //1.直接加载
        GameObject go = loadAssetBundleFiles(Application.dataPath + "/Assets/Abs", "Abs", "capsule.ab", "capsule");
        Instantiate(go);
        
        //2.协程加载
        //wwwLoad(Application.dataPath + "/Assets/Abs", "cube.ab", "cube");
        wwwLoad(Application.dataPath + "/Assets/Abs", "Abs", "cube.ab", "cube");
    }

    public static LoadAbFiles getInstance()
    {
        if (single == null)
        {
            single = new GameObject("LoadMgr").AddComponent<LoadAbFiles>();
        }
        return single;
    }

    /// <summary>
    /// 若加载路径与打包路径不同，用这个
    /// </summary>
    /// <param name="filePath"></param>加载路径
    /// <param name="manifestBundleName"></param>须指定要加载的manifestAssetBundle
    /// <param name="abFileName"></param>要加载的ab
    /// <param name="prefabFileName"></param>预制体  无后缀
    public GameObject loadAssetBundleFiles(string filePath, string manifestBundleName, string abFileName, string prefabFileName)
    {
        /**
         * 1.首先从打包路径获取依赖
         * 这里加载的就是生成的同名文件ab
         */
        AssetBundle manifestBundle = AssetBundle.LoadFromFile(getManifestFilePath(filePath, manifestBundleName));
        /**
         * 2.获取依赖资源列表
         */
        if (manifestBundle != null)
        {
            try
            {
                AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//固定加载方式，通过 assetbundle Abs加载Abs.manifest
                manifestBundle.Unload(false);
                //获取加载ab的依赖信息，参数为ab名称，如cube.ab
                string[] dependsFile = manifest.GetAllDependencies(abFileName);
                if (dependsFile.Length > 0)
                {
                    //根据获取到的依赖信息加载所有依赖资源ab
                    AssetBundle[] dependsBundle = new AssetBundle[dependsFile.Length];
                    for (int i = 0; i < dependsFile.Length; i++)
                    {
                        String fp = generateAbsoluteFile(filePath, dependsFile[i]);
                        Debug.Log(String.Format("depends:{0}:{1}", i, dependsFile[i]));
                        dependsBundle[i] = AssetBundle.LoadFromFile(fp);
                    }
                }
            }
            catch (InvalidCastException e)
            {
                Debug.LogException(e);
            }

            /**
             * 3.最后加载ab
             * 注意这里的LoadAsset的参数是Prefab的名称，无后缀，如cube而非cube.ab或cube.prefab
             */
            AssetBundle ab = AssetBundle.LoadFromFile(generateAbsoluteFile(filePath, abFileName));
            GameObject go = ab.LoadAsset(prefabFileName) as GameObject;
            ab.Unload(false);
            return go;
        }
        return null;
    }

    /// <summary>
    /// 若打包路径和加载路径相同则使用这个
    /// </summary>
    /// <param name="filePath"></param>打包路径和加载路径
    /// <param name="abFileName"></param>加载ab文件名称
    /// <param name="prefabFileName"></param>加载预制体 无后缀
    public GameObject loadAssetBundleFiles(string filePath, string abFileName, string prefabFileName)
    {
        return loadAssetBundleFiles(filePath, "", abFileName, prefabFileName);
    }

    private string getManifestFilePath(String filePath, string manifestName)
    {
        if (String.IsNullOrEmpty(manifestName))  /*路径*/
        {
            int lastSymbol = filePath.LastIndexOf('/');
            string pathAssetBunldeName = filePath.Substring(lastSymbol);
            return (filePath + pathAssetBunldeName).Trim();
        }
        else  //name
        {
            return filePath + "/" + manifestName;
        }
    }

    private string generateAbsoluteFile(string filePath, string fileName)
    {
        return string.Concat(filePath, "/", fileName).Trim();
    }

    public void wwwLoad(string filePath, string abFileName, string prefabFileName)
    {
        wwwLoad(filePath, "", abFileName, prefabFileName);
    }

    public void wwwLoad(string filePath, string manifestName, string abFileName, string prefabFileName)
    {
        StartCoroutine(wwwGetAssetBundles(filePath, getManifestFilePath(filePath, manifestName), abFileName, prefabFileName));
    }

    IEnumerator wwwGetAssetBundles(string filePath, string manifestName, string abFileName, string prefabFileName)
    {
        //WWW www = WWW.LoadFromCacheOrDownload(getManifestFilePath(filePath), 0);
        WWW www = new WWW(manifestName);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
        }
        else
        {
            /**
             * 1.首先从打包路径获取依赖
             */
            AssetBundle manifestBundle = www.assetBundle;
            /**
             * 2.获取依赖资源列表
             */
            if (manifestBundle != null)
            {

                AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                manifestBundle.Unload(false);
                www.Dispose();
                string[] dependsFile = manifest.GetAllDependencies(abFileName);
                if (dependsFile.Length > 0)
                {
                    AssetBundle[] dependsBundle = new AssetBundle[dependsFile.Length];
                    for (int i = 0; i < dependsFile.Length; i++)
                    {
                        String fp = generateAbsoluteFile(filePath, dependsFile[i]);
                        Debug.Log(String.Format("depends:{0}:{1}", i, dependsFile[i]));
                        www = new WWW(fp);
                        yield return www;
                        if (www.error == null)
                        {
                            dependsBundle[i] = www.assetBundle;
                            www.Dispose();
                        }
                    }
                }

                /**
                 * 3.最后加载ab
                 */
                AssetBundle ab = AssetBundle.LoadFromFile(generateAbsoluteFile(filePath, abFileName));
                GameObject go = ab.LoadAsset(prefabFileName) as GameObject;
                if (go != null)
                {
                    GameObject.Instantiate(go);
                }
                ab.Unload(false);
            }
        }
    }
}
