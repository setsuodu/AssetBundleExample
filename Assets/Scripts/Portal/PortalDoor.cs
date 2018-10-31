using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalDoor : UnitySingletonClass<PortalDoor>
{
    public ARPortalAccess portalAccess;

    void Awake()
    {
        portalAccess = ARPortalAccess.不允许;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != "MainCamera")
        {
            return;
        }

        if (PortalController.instance.portalStatus == ARPortalPlayer.在门外 && portalAccess == ARPortalAccess.不允许)
        {
            portalAccess = ARPortalAccess.允许;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != "MainCamera")
        {
            return;
        }

        if (PortalController.instance.portalStatus == ARPortalPlayer.在门外)
        {
            portalAccess = ARPortalAccess.不允许;
        }
    }
}
