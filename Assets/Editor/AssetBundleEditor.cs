using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundleEditor : EditorWindow
{
    private static string _targetPath;
    private static string targetPath
    {
        get
        {
            if (string.IsNullOrEmpty(_targetPath))
            {
                _targetPath = Application.dataPath + "/Sources";
            }
            return _targetPath;
        }
        set
        {
            _targetPath = value;
        }
    }
    private static string _outputPath;
    private static string outputPath
    {
        get
        {
            if (string.IsNullOrEmpty(_outputPath))
            {
                _outputPath = Application.streamingAssetsPath + "/" + patchName;
            }
            return _outputPath;
        }
        set
        {
            _outputPath = value;
        }
    }
    private static string _patchName;
    private static string patchName
    {
        get
        {
            if (string.IsNullOrEmpty(_patchName))
            {
                _patchName = "v1"; //补丁包名称
            }
            return _patchName;
        }
        set
        {
            _patchName = value;
        }
    }
    private static BuildTarget buildTarget = BuildTarget.Android;

    void OnGUI()
    {
        EditorGUILayout.Space();
        buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("输出平台：", buildTarget);

        EditorGUILayout.Space();
        patchName = EditorGUILayout.TextField("补丁版本：", patchName);

        EditorGUILayout.Space();
        if (GUILayout.Button("使用默认路径", GUILayout.Width(200)))
        {
            _targetPath = "";
            _outputPath = "";
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("资源路径：");
        targetPath = EditorGUILayout.TextField(targetPath);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("输出路径：");
        outputPath = EditorGUILayout.TextField(outputPath);

        EditorGUILayout.Space();
        if (GUILayout.Button("自动标记", GUILayout.Width(200)))
        {
            SetAssetBundleLabels();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("资源打包", GUILayout.Width(200)))
        {
            BuildAssetBundles();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("清空输出目录", GUILayout.Width(200)))
        {
            DeleteAssetBundle();
        }
    }

    [MenuItem("AssetBundle/Open Window")]
    static void AddWindow()
    {
        // 创建窗口
        Rect rect = new Rect(0, 0, 640, 320);
        AssetBundleEditor window = (AssetBundleEditor)EditorWindow.GetWindowWithRect(typeof(AssetBundleEditor), rect, true, "AssetBundle");
        window.Show();
    }

    #region 标记

    /*
     * 思路
     * 1. 找到资源保存的文件夹
     * 2. 遍历里面每个场景文件夹
     * 3. 遍历场景文件夹里的所有文件系统
     * 4. 如果访问的是文件夹:继续访问里面所有的文件系统,直到找到 文件(递归)
     * 5. 找到文件,修改他的 assetbundle labels
     * 6. 用 AssetImporter 类 修改名称和后缀
     * 7. 保存对应的文件夹名和具体路径
     * */

    //[MenuItem("AssetBundle/Set AssetBundle Labels")]
    private static void SetAssetBundleLabels()
    {
        // 当前选中物体
        //Debug.Log(Selection.activeObject.name);

        // 移除所有没有使用的标记
        AssetDatabase.RemoveUnusedAssetBundleNames();

        // 1. 找到资源所在的文件夹
        DirectoryInfo directoryInfo = new DirectoryInfo(targetPath);
        DirectoryInfo[] typeDirectories = directoryInfo.GetDirectories(); //资源类型
        
        // 2. 遍历里面每个子文件夹
        foreach (DirectoryInfo childDirectory in typeDirectories)
        {
            string typeDirectory = targetPath + "/" + childDirectory.Name;
            DirectoryInfo sceneDirectoryInfo = new DirectoryInfo(typeDirectory);
            //Debug.Log("<color=red>" + sceneDirectory + "</color>");

            // 错误检测
            if (sceneDirectoryInfo == null)
            {
                Debug.LogError(typeDirectory + "不存在");
                return;
            }
            else
            {
                Dictionary<string, string> namePathDict = new Dictionary<string, string>();

                // 3. 遍历子文件夹里的所有文件系统
                string typeName = Path.GetFileName(typeDirectory);
                //Debug.Log(typeName);

                onSceneFileSystemInfo(sceneDirectoryInfo, typeName, namePathDict);

                //onWriteConfig(typeName, namePathDict);
            }
        }
        AssetDatabase.Refresh();
        Debug.LogWarning("设置成功");
    }
    
    /// <summary>
    /// 记录配置文件
    /// </summary>
    private static void onWriteConfig(string typeName, Dictionary<string, string> namePathDict)
    {
        string path = Application.streamingAssetsPath + "/" + typeName + "Record.txt";
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

    private static void onSceneFileSystemInfo(FileSystemInfo fileSystemInfo, string typeName, Dictionary<string, string> namePathDict)
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
                // 4. 如果找到的是文件夹, 递归直到没有文件夹
                //Debug.Log("强转失败，是文件夹");
                onSceneFileSystemInfo(tempfileInfo, typeName, namePathDict);
            }
            else
            {
                // 5. 找到文件, 修改他的 AssetLabels
                //Debug.Log("是文件");
                setLables(fileInfo, typeName, namePathDict);
            }
        }
    }

    /// <summary>
    /// 修改资源文件的 assetbundle labels
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <param name="typeName"></param>
    private static void setLables(FileInfo fileInfo, string typeName, Dictionary<string, string> namePathDict)
    {
        // 忽视unity自身生成的meta文件
        if (fileInfo.Extension == ".meta") return;

        string bundleName = getBundleName(fileInfo, typeName); //sofa_1.mat
        //Debug.Log(bundleName);

        int index = fileInfo.FullName.IndexOf("Assets");
        string assetPath = fileInfo.FullName.Substring(index); //Assets/Sources/Materials/sofa_1.mat
        //Debug.Log(assetPath);

        // 6. 修改名称和后缀
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        assetImporter.assetBundleName = bundleName;
        if (fileInfo.Extension == ".unity")
        {
            assetImporter.assetBundleVariant = "u3d"; //场景文件
        }
        else
        {
            assetImporter.assetBundleVariant = "assetbundle"; //资源文件
        }

        // 添加到字典
        string folderName = "";
        if (bundleName.Contains("/"))
        {
            folderName = bundleName.Split('/')[1];
        }
        else
        {
            folderName = bundleName;
        }

        string bundlePath = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant;
        if (!namePathDict.ContainsKey(folderName))
            namePathDict.Add(folderName, bundlePath);
    }

    /// <summary>
    /// 获取包名
    /// </summary>
    /// <param name="fileInfo">文件信息</param>
    /// <param name="typeName">资源类型</param>
    /// <returns></returns>
    private static string getBundleName(FileInfo fileInfo, string typeName)
    {
        string windowPath = fileInfo.FullName;
        string unityPath = windowPath.Replace(@"\", "/"); //转斜杠 C:/Users/Administrator/Documents/GitHub/AssetBundleExample/Assets/Sources/Textures/trash_2.jpg
        string bundlePath = Path.GetFileNameWithoutExtension(unityPath);
        //Debug.Log(fileInfo + " + " + typeName + " = " + bundlePath); //sofa_3.mat

        /*
        if (bundlePath.Contains("/"))
        {
            string[] temp = bundlePath.Split('/');
            return bundlePath + "/" + temp[0];
        }
        else
        {
            return bundlePath;
        }
        */

        string result = Path.Combine(typeName, bundlePath);
        //Debug.Log(result);

        return result;
    }
    
    #endregion

    #region 打包

    //[MenuItem("AssetBundle/Build AssetBundles")]
    static void BuildAssetBundles()
    {
        //BuildAssetBundleOptions option = BuildAssetBundleOptions.None;
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        BuildPipeline.BuildAssetBundles(outputPath, 0, buildTarget);
        AssetDatabase.Refresh();
    }

    #endregion

    #region 删除

    //[MenuItem("AssetBundle/Delete All")]
    static void DeleteAssetBundle()
    {
        string srcPath = Application.streamingAssetsPath;

        try
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //返回目录中所有文件和子目录
            foreach (FileSystemInfo i in fileinfo)
            {
                if (i is DirectoryInfo) //判断是否文件夹
                {
                    DirectoryInfo subdir = new DirectoryInfo(i.FullName);
                    subdir.Delete(true); //删除子目录和文件
                }
                else
                {
                    File.Delete(i.FullName); //删除指定文件
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

        /*
        if (!Directory.Exists(outputPath))
        {
            Debug.Log("目标文件夹不存在");
            return;
        }
        Directory.Delete(outputPath, true);
        File.Delete(outputPath + ".meta");
        */

        AssetDatabase.Refresh();
    }

    #endregion
}
