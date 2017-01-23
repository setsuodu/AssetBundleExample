using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using UnityEngine;
using LitJson;

public class License : MonoBehaviour
{
    [HideInInspector]
    public string mLicenseKey;
    public int targetSetsCount;
    public List<string> setsId, setsVersion, targets, targetId, feature, effect;

    string url = "http://192.168.0.227:9011/v1/app/enroll";
    private readonly string[] NFTDataExts = { "fset", "fset3", "iset" };

    string twPath;
    Thread mThread;

    void Start()
    {
        mThread = new Thread(run);
        StartCoroutine(GetJson());
    }

    object lockd = new object();
    void run()
    {
        //int index = 0;
        lock (lockd)
        {
            while (true)
            {

#if UNITY_EDITOR
                Debug.Log("Hello Editor!");
#elif UNITY_ANDROID
                if(Application.platform == RuntimePlatform.Android)
                {
                    //PluginFunctions.loadArMark(twPath); //java层读取markers.dat
                }
#endif
                Thread.Sleep(10000);
                break;
            }
        }
    }

    IEnumerator GetJson()
    {
        JsonData data = new JsonData();
        data["license"] = mLicenseKey;
        byte[] postBytes = Encoding.Default.GetBytes(data.ToJson());

        Dictionary<string, string> header = new Dictionary<string, string>();
        header["Content-Type"] = "application/json";
        header.Add("CLEARANCE", "I_AM_AR_MASTER");

        WWW www = new WWW(url, postBytes, header);
        yield return www;

        if (www.isDone && www.error == null)
        {
            // UTF8文字列として取得する
            string text = www.text;
            Debug.Log(text);
            // バイナリデータとして取得する
            byte[] results = www.bytes;
            Debug.Log(results);
        }

        JsonData complete = JsonMapper.ToObject(www.text);
        JsonData targetSets = complete["data"]["targetSets"];

        targetSetsCount = targetSets.Count;
        string twContent = targetSetsCount.ToString(); //文本的内容，先遍历生成文本，在写入

        for (int i = 0; i < targetSets.Count; i++)
        {
            setsId.Add(targetSets[i]["setsId"].ToString()); //setsId
            setsVersion.Add(targetSets[i]["setsVersion"].ToString()); //setsVersion
            twContent += "\n";
            twContent += "/";
            twContent += targetSets[i]["setsId"].ToString();
            twContent += "/";
            twContent += targetSets[i]["setsId"].ToString();
            twContent += "\n";
            twContent += "NFT";
            twContent += "\n";
            twContent += "FILTER 15.0";

            for (int s = 0; s < targetSets[i]["targets"].Count; s++)
            {
                targetId.Add(targetSets[i]["targets"][s]["targetId"].ToString()); //targetId
                effect.Add(targetSets[i]["targets"][s]["effect"].ToString()); ; //effect
                gameObject.SendMessage("getZipURL", effect[0]);

                twContent += "\n";
                twContent += "UID ";
                twContent += targetSets[i]["targets"][s]["targetId"].ToString();

                for (int t = 0; t < targetSets[i]["targets"][s]["feature"].Count; t++)
                {
                    feature.Add(targetSets[i]["targets"][s]["feature"][t].ToString()); //feature
                }
            }
        }

        //写入文本文件
        Debug.Log(twContent);
        twPath = Application.temporaryCachePath + "/" + "markers.dat";
        Debug.Log(twPath); //便于Debug找到文件夹
        TextWriter textWriter = new StreamWriter(twPath, false);
        textWriter.Write(twContent);
        textWriter.Close();

        //创建文件夹
        for (int i = 0; i < setsId.Count; i++)
        {
            string dir = Application.temporaryCachePath + "/" + setsId[i];
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            for (int t = 0; t < feature.Count; t++)
            {
                string urlMarker = "http://192.168.0.227:9010" + feature[t];
                string filePath = dir + "/" + setsId[0] + "." + NFTDataExts[t];
                Debug.Log(urlMarker);

                WWW download = new WWW(urlMarker);
                yield return download;

                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                fs.Write(download.bytes, 0, download.bytesDownloaded);
                fs.Close();
            }
        }

        mThread.Start();
    }


}
