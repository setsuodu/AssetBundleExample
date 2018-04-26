# AssetBundleExample

## Introduction

a template to show how to build and load assetbundle.

## WorkFlow

- create ABs;
- zip;
- upload;
- download;
- unzip;
- LoadAsset;

## API, platform path and permission

API     | 路径     | 保护
------- | ------- | -------
Application.persistentDataPath  | C:\Users\user\AppData\LocalLow\CompanyName\ProductName | 可读写 |
Application.temporaryCachePath  | C:\Users\user\AppData\Local\Temp\CompanyName\ProductName | 可读写 |
Application.streamingAssetsPath | \Assets\StreamingAssets\ | 只读 |
Application.dataPath            | \Assets\ | 只读 |

## LoadFromCacheOrDownload 各平台的缓存地址

平台    | 路径
------- | ---
Windows | C:\Users\user\AppData\LocalLow\Unity\CompanyName_ProductName
Android | \Android\data\PackageName\files\UnityCache\Shared
iOS     | 
