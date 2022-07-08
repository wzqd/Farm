using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 可淡化物体接口
/// </summary>
public interface IFader
{
    public void FadeIn(string objectName);  //淡入
    public void FadeOut(string objectName); //淡出
    
}

#region 淡化管理器
//管理所有可淡化的物体

//主要有登记淡化物体，和监听对外的淡出淡入事件
#endregion
public class FaderMgr : SingletonMono<FaderMgr>
{
    /// <summary>
    /// 所有可淡化物体的字典
    /// 键：物体名
    /// 值：物体脚本
    /// </summary>
    private Dictionary<string, IFader> faderDict = new Dictionary<string, IFader>();

    private void Awake()
    {
        //添加对于物体淡化的监听
        EventMgr.Instance.AddEventListener<string>("TriggerFadeIn", TriggerFadeIn); 
        EventMgr.Instance.AddEventListener<string>("TriggerFadeOut", TriggerFadeOut);
    }

    /// <summary>
    /// 登记可淡化物体
    /// </summary>
    /// <param name="objectName">物体名</param>
    /// <param name="obj">物体脚本</param>
    public void AddFaderRegistry(string objectName, IFader obj)
    {
        faderDict.Add(objectName,obj);
    }
    
    /// <summary>
    /// 触发对应物体的淡入
    /// </summary>
    /// <param name="objectName">物体名</param>
    private void TriggerFadeIn(string objectName)
    {
        if (faderDict.ContainsKey(objectName))
        {
            faderDict[objectName].FadeIn(objectName);
        }
    }
    /// <summary>
    /// 触发对应物体的淡出
    /// </summary>
    /// <param name="objectName">物体名</param>
    private void TriggerFadeOut(string objectName)
    {
        if (faderDict.ContainsKey(objectName))
        {
            faderDict[objectName].FadeOut(objectName);
        }
    }
    
    /// <summary>
    /// 清空字典，切场景使用
    /// </summary>
    public void Clear()
    {
        faderDict.Clear();
    }
}
