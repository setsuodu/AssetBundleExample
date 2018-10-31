using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PortalController))]
public class DemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); //显示默认所有参数

        PortalController demo = (PortalController)target;

        if (GUILayout.Button("Reset"))
        {
            demo.Reset();
        }
    }
}

#endif

/*
 * PortalController 代表所有门内实例的管理
 * TriggerEnter 触发优先级 门 == 墙 > 房间
 */
public class PortalController : UnitySingletonClass<PortalController>
{
    public ARPortalPlayer portalStatus = ARPortalPlayer.在门外;
    [SerializeField] GameObject avatar; //控制NPC行为树
    [Header("----- 角色随机出生点 -----"), SerializeField] Transform[] spawnPoints;

    void Start()
    {
        //avatar = AvatarController.instance.gameObject;
        //gameObject.SetActive(false); //AR扫描后显示
    }

    // 面朝玩家(摄像机)
    public void Reset()
    {
        /*
        Vector3 camera_pos = Camera.main.transform.position + Vector3.down;
        transform.position = camera_pos + Camera.main.transform.forward * 3; //玩家面前3米处

        Vector3 lookPos = transform.position - Camera.main.transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos); //面朝玩家
        */
    }

    public void SetAvatarActive()
    {
        if(!avatar.activeSelf)
            avatar.SetActive(true);

        ///TODO:查询角色作息表，确定出生点
        Vector3 position = spawnPoints[0].position;
        Quaternion rotation = Quaternion.identity;
        avatar.transform.SetPositionAndRotation(position, rotation);
        ///TODO:切换、启动行为树
    }
}

public enum ARPortalPlayer
{
    在门外 = 0,
    在门内 = 1,
    在墙里 = 2,
}
public enum ARPortalAccess
{
    不允许 = 0,
    允许 = 1,
}
