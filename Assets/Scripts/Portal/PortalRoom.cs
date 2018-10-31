using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PortalRoom : MonoBehaviour
{
    public Material[] materials;
    //private BoxCollider boxCollider;
    //[SerializeField, Range(2, 10)] int horizontal = 2;
    //[SerializeField, Range(2, 10)] int vertical = 2;

    void Awake()
    {
        //boxCollider = GetComponent<BoxCollider>();
        //boxCollider.size = new Vector3(horizontal, 2, vertical);
        //boxCollider.center = new Vector3(0, 1, vertical / 2);
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
