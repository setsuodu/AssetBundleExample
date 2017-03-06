using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepARConfig
{
    public static readonly string apiUrl = "http://192.168.199.208:3805/v1_0/app/enroll"; //API网络请求地址
    public static readonly string downloadUrl = "http://www.setsuodu.com/ABs/Android/"; //文件下载请求地址

    public static readonly string bundlePath = "file://" + Application.persistentDataPath; //persistentDataPath是U3d跨平台存在的目录

}
