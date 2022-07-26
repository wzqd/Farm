using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeTeleporter : MonoBehaviour, ITeleport
{

    public Vector3 Destination { get; set; }
    private void Awake()
    {
        TeleporterMgr.Instance.AddTeleporterRegistry(this.gameObject.name, this); //向管理器登记自己
    }

    private void OnDestroy()
    {
        TeleporterMgr.Instance.RemoveTeleporterRegistry(this.gameObject.name); //向管理器取消自己的登记
    }


    public void Teleport(Vector3 destination)
    {
        EventMgr.Instance.EventTrigger("Teleport", Destination); //触发传送事件
    }
    
}
