using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleEditor
{
    #region 自动做标记

    //思路
    //1.找到资源保存的文件夹
    //2.遍历里面每个场景文件夹
    //3.遍历场景文件夹里的所有文件系统
    //4.如果访问的是文件夹:继续访问里面所有的文件系统,直到找到 文件(递归)
    //5.找到文件,修改他的 assetbundle labels
    //6.用 AssetImporter 类 修改名称和后缀
    //7.保存对应的文件夹名和具体路径

    [MenuItem("AssetBundle/Set AssetBundle Labels")]
    public static void SetAssetBundleLabels()
    {
        //当前选中物体
        //Debug.Log(Selection.activeObject.name);

        //移除所有没有使用的标记
        AssetDatabase.RemoveUnusedAssetBundleNames();
        //1.找到资源保存的文件夹
        string assetDirectory = Application.dataPath + "/Resources";//"../Assets/Resources";
        DirectoryInfo directoryInfo = new DirectoryInfo(assetDirectory);
        DirectoryInfo[] sceneDirectories = directoryInfo.GetDirectories();
        //2.遍历里面每个场景文件夹
        foreach (DirectoryInfo tempDirectoryInfo in sceneDirectories)
        {
            string sceneDirectory = assetDirectory + "/" + tempDirectoryInfo.Name;
            DirectoryInfo sceneDirectoryInfo = new DirectoryInfo(sceneDirectory);
            //错误检测
            if (sceneDirectoryInfo == null)
            {
                Debug.LogError(sceneDirectory + "不存在");
                return;
            }
            else
            {
                Dictionary<string, string> namePathDict = new Dictionary<string, string>();
                //3.遍历场景文件夹里的所有文件系统
                //D:\WorkSpace\UnityWorkSpace\MobaClient\AssetBundles\Assets\Res
                //D:/WorkSpace/UnityWorkSpace/MobaClient/AssetBundles/Assets/Res
                int index = sceneDirectory.LastIndexOf("/");
                string sceneName = sceneDirectory.Substring(index + 1);
                onSceneFileSystemInfo(sceneDirectoryInfo, sceneName, namePathDict);

                onWriteConfig(sceneName, namePathDict);
            }
        }//end foreach
        AssetDatabase.Refresh();
        Debug.LogWarning("设置成功");
    }//end set
    
    /// <summary>
    /// 记录配置文件
    /// </summary>
    private static void onWriteConfig(string sceneName, Dictionary<string, string> namePathDict)
    {
        string path = PathUtil.GetAssetBundleOutPath() + "/" + sceneName + "Record.txt";
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(namePathDict.Count);
                foreach (KeyValuePair<string, string> kv in namePathDict)
                    sw.WriteLine(kv.Key + " " + kv.Value);
            }
        }
    }

    private static void onSceneFileSystemInfo(FileSystemInfo fileSystemInfo, string sceneName, Dictionary<string, string> namePathDict)
    {
        if (!fileSystemInfo.Exists)
        {
            Debug.LogError(fileSystemInfo.FullName + "不存在");
            return;
        }
        DirectoryInfo directoryInfo = fileSystemInfo as DirectoryInfo;
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        foreach (var tempfileInfo in fileSystemInfos)
        {
            FileInfo fileInfo = tempfileInfo as FileInfo;
            if (fileInfo == null)
            {
                //代表强转失败,不是文件 就是文件夹
                //4.如果访问的是文件夹:继续访问里面所有的文件系统,直到找到 文件(递归)
                onSceneFileSystemInfo(tempfileInfo, sceneName, namePathDict);
            }
            else
            {
                //文件
                //5.找到文件,修改他的 assetbundle labels
                setLables(fileInfo, sceneName, namePathDict);
            }
        }
    }
    
    /// <summary>
    /// 修改资源文件的 assetbundle labels
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="sceneName"></param>
    private static void setLables(FileInfo fileInfo, string sceneName, Dictionary<string, string> namePathDict)
    {
        //忽视unity自身生成的meta文件
        if (fileInfo.Extension == ".meta")
            return;
        string bundleName = getBundleName(fileInfo, sceneName);
        int index = fileInfo.FullName.IndexOf("Assets");
        string assetPath = fileInfo.FullName.Substring(index);
        //6.用 AssetImporter 类 修改名称和后缀
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        assetImporter.assetBundleName = bundleName;
        if (fileInfo.Extension == ".unity")
            assetImporter.assetBundleVariant = "u3d";
        else
            assetImporter.assetBundleVariant = "assetbundle";
        //添加到字典
        string folderName = "";
        if (bundleName.Contains("/"))
            folderName = bundleName.Split('/')[1];
        else
            folderName = bundleName;
        string bundlePath = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant;
        if (!namePathDict.ContainsKey(folderName))
            namePathDict.Add(folderName, bundlePath);
    }
    
    /// <summary>
    /// 获取包名
    /// </summary>
    private static string getBundleName(FileInfo fileInfo, string sceneName)
    {
        string windowPath = fileInfo.FullName;
        string unityPath = windowPath.Replace(@"\", "/");
        int Index = unityPath.IndexOf(sceneName) + sceneName.Length;
        string bundlePath = unityPath.Substring(Index + 1);

        if (bundlePath.Contains("/"))
        {
            string[] temp = bundlePath.Split('/');
            return sceneName + "/" + temp[0];
        }
        else
        {
            return sceneName;
        }
    }
    
    #endregion

    #region 打包

    [MenuItem("AssetBundle/Build AssetBundles")]
    static void BuildAssetBundles()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();
        //BuildAssetBundleOptions option = BuildAssetBundleOptions.None;
        BuildPipeline.BuildAssetBundles(outPath, 0, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
    }

    #endregion

    #region 删除

    [MenuItem("AssetBundle/Delete All")]
    static void DeleteAssetBundle()
    {
        string outPath = PathUtil.GetAssetBundleOutPath();

        Directory.Delete(outPath, true);
        File.Delete(outPath + ".meta");
        AssetDatabase.Refresh();
    }

    #endregion
}
