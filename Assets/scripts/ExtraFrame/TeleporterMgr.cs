using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 传送器物体接口
/// </summary>
public interface ITeleport
{
    public void Teleport(Vector3 destination);  //传送
    public Vector3 Destination { get; set;} //目标点

}

#region 传送器管理器
//管理所有传送点

//主要有登记传送点，和监听对外的传送事件
#endregion
public class TeleporterMgr : Singleton<TeleporterMgr>
{
    /// <summary>
    /// 所有传送点字典
    /// 键：物体名
    /// 值：物体脚本
    /// </summary>
    private Dictionary<string, ITeleport> teleporterDict = new Dictionary<string, ITeleport>();

    public TeleporterMgr()
    {
        //添加对于物体淡化的监听
        EventMgr.Instance.AddEventListener<string>("TriggerTeleport", TriggerTeleport);
    }

    /// <summary>
    /// 登记传送点
    /// </summary>
    /// <param name="objectName">物体名</param>
    /// <param name="obj">物体脚本</param>
    public void AddTeleporterRegistry(string objectName, ITeleport obj)
    {
        teleporterDict.Add(objectName,obj);
    }    
    
    /// <summary>
    /// 取消登记传送点
    /// </summary>
    /// <param name="objectName">物体名</param>
    public void RemoveTeleporterRegistry(string objectName)
    {
        teleporterDict.Remove(objectName);
    }
    
    /// <summary>
    /// 触发对应传送点传送
    /// </summary>
    /// <param name="objectName">物体名</param>
    private void TriggerTeleport(string objectName)
    {
        if (teleporterDict.ContainsKey(objectName))
        {
            ITeleport teleporter = teleporterDict[objectName];
            teleporter.Teleport(teleporter.Destination);
        }
    }
    /// <summary>
    /// 清空字典，切场景使用
    /// </summary>
    public void Clear()
    {
        teleporterDict.Clear();
    }
}
