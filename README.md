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

## Unity API与各平台路径

> Windows

API     | 路径     | 权限
------- | ------- | -------
Application.persistentDataPath  | C:\Users\user\AppData\LocalLow\CompanyName\ProductName | 可读写 |
Application.temporaryCachePath  | C:\Users\user\AppData\Local\Temp\CompanyName\ProductName | 可读写 |
Application.streamingAssetsPath | \Assets\StreamingAssets\ | 只读 |
Application.dataPath            | \Assets\ | 只读 |

> Android

API     | 路径     | 权限
------- | ------- | -------
Application.persistentDataPath  | /storage/emulated/0/Android/data/PackageName/files | 可读写 |
Application.temporaryCachePath  | /storage/emulated/0/Android/data/PackageName/cache | 可读写 |
Application.streamingAssetsPath | jar:file:///data/app/ProductName/base.apk!/assets | 只读 |
Application.dataPath            | /data/app/PackageName-1/base.apk | 只读 |

> iOS

API     | 路径     | 权限
------- | ------- | -------
Application.persistentDataPath  | /var/mobile/Containers/Data/Application/app sandbox/Documents | 可读写 |
Application.temporaryCachePath  | /var/mobile/Containers/Data/Application/app sandbox/Library/Caches | 可读写 |
Application.streamingAssetsPath | /var/containers/Bundle/Application/app sandbox/test.app/Data/Raw | 只读 |
Application.dataPath            | /var/containers/Bundle/Application/app sandbox/xxx.app/Data | 只读 |

## LoadFromCacheOrDownload 各平台的缓存地址

平台    | 路径
------- | ---
Windows | C:\Users\user\AppData\LocalLow\Unity\CompanyName_ProductName
Android | \Android\data\PackageName\files\UnityCache\Shared
iOS     | 
