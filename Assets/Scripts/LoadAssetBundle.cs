using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadAssetBundle : MonoBehaviour
{
    private string cloudUrl, localUrl = "";
    public string fileName, assetName;

    void Awake()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        localUrl = "file://" + Application.persistentDataPath + "/" + fileName;
        cloudUrl = "http://www.setsuodu.com/ABs/" + fileName;
#elif UNITY_EDITOR
        //特别标注，PC上"file:// + Application.persistentDataPath"或"file:// + Application.temporaryCachePath"会导致无法读取。要"///"
        //"file:// + Application.streamingAssetsPath"能读
        localUrl = "file:///" + Application.persistentDataPath + "/" + fileName;
        cloudUrl = "http://www.setsuodu.com/ABs/" + fileName;
#endif
    }

    void Start()
    {
        StartCoroutine(LoadAsset());
        Debug.Log("Start");
    }

    public IEnumerator LoadAsset()
    {
        WWW www;
        www = new WWW(cloudUrl);
        Debug.Log("cloudUrl : " + cloudUrl);
        yield return www;
        if (www.error != null) Debug.Log(www.error);
        else
        {
            SaveAssetBundle(www.bytes, fileName);
        }

        www = new WWW(localUrl);
        Debug.Log("localUrl : " + localUrl);
        yield return www;
        if (www.error != null) Debug.Log(www.error);
        else
        {
            AssetBundle bundle = www.assetBundle;

            GameObject go = bundle.LoadAsset(assetName, typeof(GameObject)) as GameObject;
            Instantiate(go);
        }
    }

    private void SaveAssetBundle(byte[] byteData, string fileName)
    {
        string savePath = Application.persistentDataPath;
        Debug.Log(Application.persistentDataPath);

        FileStream fs = new FileStream(savePath + "/" + fileName, FileMode.Create);
        fs.Seek(0,SeekOrigin.Begin);
        fs.Write(byteData, 0, byteData.Length);
        fs.Close();
    }

}
