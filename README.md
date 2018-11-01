# AssetBundleExample

## Introduction

This template shows how to build and load AssetBundle.
Add dynamic lightmap bundle loading Demo;

## Steps

1. AssetBundle/Open Window;
2. Set "输出平台" and "补丁版本", click "使用默认路径" to setup custom source path and output path;
3. click "自动标记" to set the assetbundle AssetsLabel automaticly, as the format ``FileType/FileName.assetbunle``;
4. click "资源打包" to build assetbundle package to "/StreamingAssets/";
5. copy the package(for example "v2") folder onto the server;
6. setup the "baseURL" according to the directory structure of server;
7. load main manifest at first, this file include all the dependencies and hash;
8. use filename to query hash from manifest, then ``WWW.LoadFromCacheOrDownload()`` to load the dependencies, the file;
9. unload(false) at last;

## Attention

1. if there are shaders bundle, put them to "Edit/Project Settings/Graphics/Always Included Shaders" before build assetbundle.
2. dont put standard shader into "Always Included Shaders";
3. if a meshrenderer linked a shader bundle, ``AddComponent<FixShader>()`` to the gameObject.
4. don't make some object use a same AssetsLabel, it will make unity crash while building assetbundle.
5. dynamic Lightmap load and use a json to load Int lightmapIndex and Vector4 lightmapScaleOffset. The json file must be generate after ``Generate Lighting`` when gameObject in the Hierarchy.
