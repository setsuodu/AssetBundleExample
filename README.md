# AssetBundleExample

## Introduction

This template shows how to build and load AssetBundle.

## Steps

1. Create AssetBundles;
2. Copy all AssetBundles onto server;
3. 读取总配置文件 ABs.manifest，获取文件hash;
4. WWW.LoadFromCacheOrDownload() 或 UnityWebRequest.GetAssetBundle()，加载hash;
5. LoadAsset;
6. 实例化，释放资源Unload(false);

> 缓存物理路径

平台 | 路径
-- | --
Windows | C:\Users\user\AppData\LocalLow\Unity\CompanyName_ProductName
Android | \Android\data\PackageName\files\UnityCache\Shared
iOS     | 

## 关于Shader的AB包

1. 打包前，将shader添加到GraphicsSettins/Always Included Shaders列表
2. 加载时，动态添加到使用该材质的物体上，AddComponent(Fixshader)
