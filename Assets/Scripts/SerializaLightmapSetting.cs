using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SerializaLightmapSetting : MonoBehaviour
{
    public TextAsset data;

    public void Save()
    {
        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        Debug.Log(meshRenderers.Length);

        List<RendererLightmapSettins> settings = new List<RendererLightmapSettins>();
        RendererLightmapSettinsList list = new RendererLightmapSettinsList();

        foreach (MeshRenderer renderer in meshRenderers)
        {
            //Debug.Log(renderer.name + ":" + renderer.lightmapIndex + "/" + renderer.lightmapScaleOffset.z);

            // 烘焙完记录下来，加载时读取
            //renderer.lightmapIndex = lightmapIndex;
            //renderer.lightmapScaleOffset = lightmapScaleOffset;

            RendererLightmapSettins setting = new RendererLightmapSettins();
            setting.lightmapIndex = renderer.lightmapIndex;
            setting.lightmapScaleOffset = renderer.lightmapScaleOffset;
            setting.realtimeLightmapIndex = renderer.realtimeLightmapIndex;
            setting.realtimeLightmapScaleOffset = renderer.realtimeLightmapScaleOffset;
            settings.Add(setting);
            //string json = JsonUtility.ToJson(setting);
            //Debug.Log(json);
        }

        list.settings = settings;
        string json = JsonUtility.ToJson(list);
        Debug.Log(json);

        // 写入本地文本
        string fileName = gameObject.name + "_Settings.json";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        StreamWriter streamWriter = new StreamWriter(filePath, false, System.Text.Encoding.UTF8);
        streamWriter.Write(json);
        streamWriter.Close();
    }

    public void Load()
    {
        if (data == null) return;
        RendererLightmapSettinsList list = JsonUtility.FromJson<RendererLightmapSettinsList>(data.text);
        List<RendererLightmapSettins> settings = list.settings;
        Debug.Log(settings.Count);

        MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            meshRenderers[i].lightmapIndex = settings[i].lightmapIndex;
            meshRenderers[i].lightmapScaleOffset = settings[i].lightmapScaleOffset;
            meshRenderers[i].realtimeLightmapIndex = settings[i].realtimeLightmapIndex;
            meshRenderers[i].realtimeLightmapScaleOffset = settings[i].realtimeLightmapScaleOffset;
            Debug.Log(meshRenderers[i].lightmapScaleOffset + "\n" + settings[i].lightmapScaleOffset);
        }
    }
}

[System.Serializable]
public class RendererLightmapSettins
{
    public int lightmapIndex;
    public Vector4 lightmapScaleOffset;
    public int realtimeLightmapIndex;
    public Vector4 realtimeLightmapScaleOffset;
}

[System.Serializable]
public class RendererLightmapSettinsList
{
    public List<RendererLightmapSettins> settings;
}

#if UNITY_EDITOR
[CustomEditor(typeof(SerializaLightmapSetting))]
public class SerLightmapSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //显示默认所有参数

        SerializaLightmapSetting demo = (SerializaLightmapSetting)target;

        if (GUILayout.Button("Save"))
        {
            demo.Save();
        }

        if (GUILayout.Button("Load"))
        {
            demo.Load();
        }
    }
}
#endif
