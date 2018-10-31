using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(PortalRoom))]
public class PortalRoomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //显示默认所有参数

        PortalRoom demo = (PortalRoom)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Equal"))
        {
            demo.Equal();
        }
        if (GUILayout.Button("NotEqual"))
        {
            demo.NotEqual();
        }
        GUILayout.EndHorizontal();

        //PortalRoom.stencilTest = (CompareFunction)EditorGUILayout.EnumPopup("Stencil Test", CompareFunction.Equal);
    }
}
#endif

public class PortalRoom : MonoBehaviour
{
    public Material[] materials;

    //public static CompareFunction stencilTest;

    public void Equal()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
    }

    public void NotEqual()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual);
        }
    }

    void OnDestroy()
    {
        foreach (var mat in materials)
        {
            mat.SetInt("_StencilTest", (int)CompareFunction.Equal);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "MainCamera")
        {
            return;
        }
        Debug.Log("Inside the room");

        if (PortalDoor.instance.portalAccess == ARPortalAccess.允许 && PortalController.instance.portalStatus == ARPortalPlayer.在门外)
        {
            // 只有从门走，才能进入
            PortalController.instance.portalStatus = ARPortalPlayer.在门内;
            PortalDoor.instance.portalAccess = ARPortalAccess.允许;

            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.NotEqual); //进去了，就NotEqual
            }
        }
        else if (PortalDoor.instance.portalAccess == ARPortalAccess.允许 && PortalController.instance.portalStatus == ARPortalPlayer.在墙里)
        {
            // 墙里返回屋内
            PortalController.instance.portalStatus = ARPortalPlayer.在门内;
            PortalDoor.instance.portalAccess = ARPortalAccess.允许;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "MainCamera")
        {
            return;
        }
        Debug.Log("Outside of room");

        if (PortalDoor.instance.portalAccess == ARPortalAccess.允许 && PortalController.instance.portalStatus == ARPortalPlayer.在门内)
        {
            // 只有从门走，才能出去
            PortalController.instance.portalStatus = ARPortalPlayer.在门外;
            PortalDoor.instance.portalAccess = ARPortalAccess.不允许;

            foreach (var mat in materials)
            {
                mat.SetInt("_StencilTest", (int)CompareFunction.Equal); //在门外，就Equal
            }
        }
    }
}
