using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于存储所有的常量
/// </summary>
public class Settings
{
    //淡入淡出数据
    public const float fadeDuration = 0.35f; //淡入淡出的时间
    public const float fadeAlpha = 0.45f; //淡入目标透明度
    
    //UI大小数据
    public const int playerToolBarWidth = 222; //物品栏宽度
    public const int playerToolBarHeight = 24; //物品栏高度

    public const float playerBagWidth = 249.5f;
    public const float playerBagHeight = 200f;
    
    //背包数据
    public const int toolBarCapacity = 10; //物品栏大小
    public const int BagCapacity = 30; //背包大小

    //游戏时间 （每个单位对应的数量级）
    public const float secondThreshold = 1f;
    public const int secondHold = 6; //6秒一分钟 （即现实一分钟为游戏一小时）
    public const int minuteHold = 60; //60分钟一小时
    public const int hourHold = 24; //24小时一天
    public const int dayHold = 30; //30天一季度
    public const int seasonHold = 4; //4季度一年

}
