using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    public Text text;
    MyZip zip = new MyZip();
    string directory;
    float testProgressOverall, testProgress;

    void Start()
    {
        text = text.GetComponent<Text>();
        StartCoroutine(Copy());
        Debug.Log(Application.persistentDataPath);
        Debug.Log(Application.dataPath + "/ABs");
    }
    
    
    public void CompressZip()
    {
        //放进Editor中
        //zip.ZipFolder(Application.persistentDataPath + "/original", Application.persistentDataPath + "/zip/test.zip");
        zip.ZipFolder(Application.dataPath + "/ABs", Application.streamingAssetsPath + "/test.zip");
    }

    public void DecompressZip()
    {
        //Runtime中运行
        zip.UnZipFile(Application.persistentDataPath + "/zip/test.zip", Application.persistentDataPath + "/unzip");
    }

    void Update()
    {
        if ((testProgressOverall != zip.progressOverall) || (testProgress != zip.progress))
        {
            testProgressOverall = zip.progressOverall;
            testProgress = zip.progress;
            text.text = string.Format("总进度: {0}%, 单个文件进度: {1}%", zip.progressOverall, zip.progress);
        }
    }

    IEnumerator Copy()
    {
#if UNITY_EDITOR || UNITY_IPHONE
        string path = "file://" + Application.streamingAssetsPath + "/test.pdf";
#else
        string path =  Application.streamingAssetsPath + "/test.pdf";
#endif
        WWW www = new WWW(path);
        while (!www.isDone)
        {
            yield return www;
        }

        if (!Directory.Exists(Application.persistentDataPath + "/original"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/original");
        }

        if (File.Exists(Application.persistentDataPath + "/original/test.pdf"))
        {
            File.Delete(Application.persistentDataPath + "/original/test.pdf");
        }

        using (FileStream stream = File.Create(Application.persistentDataPath + "/original/test.pdf"))
        {
            stream.Write(www.bytes, 0, www.bytes.Length);
            stream.Close();
        }

        if (File.Exists(Application.persistentDataPath + "/original/test.pdf"))
        {
            FileInfo fileInfo = new FileInfo(Application.persistentDataPath + "/original/test.pdf");
            //Debug.LogError(fileInfo.Length.ToString());
        }
    }
}
