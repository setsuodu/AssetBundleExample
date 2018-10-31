using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalWall : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "MainCamera")
        {
            return;
        }

        if (PortalController.instance.portalStatus == ARPortalPlayer.在门内 && PortalDoor.instance.portalAccess == ARPortalAccess.允许)
        {
            // 警告
            PortalController.instance.portalStatus = ARPortalPlayer.在墙里;
            Debug.Log("碰到墙");
            ///TODO:黑屏+警告语+箭头方向指示
        }
    }
}
