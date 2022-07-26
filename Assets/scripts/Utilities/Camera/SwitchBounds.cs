using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/// <summary>
/// 切换Cinemachine上视角的边界
/// </summary>
public class SwitchBounds : MonoBehaviour
{
    private void Start()
    {
        EventMgr.Instance.AddEventListener("SwitchCameraBounds", SwitchConfineShape);
    }

    /// <summary>
    /// 切换相机边界 （每次切换场景时调用）
    /// </summary>
    private void SwitchConfineShape()
    {
        PolygonCollider2D confineShape =
            GameObject.FindGameObjectWithTag("BoundsConfine").GetComponent<PolygonCollider2D>(); //找到碰撞器，（记得添加标签）
        
        CinemachineConfiner confine = GetComponent<CinemachineConfiner>(); //得到cinemachine脚本

        confine.m_BoundingShape2D = confineShape; 
        
        confine.InvalidatePathCache(); //每次改变边界时要清空缓存（cinemachine的需要）
    }
}
