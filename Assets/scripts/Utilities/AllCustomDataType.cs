using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品信息类
/// </summary>
[System.Serializable]
public class ItemDetails
{
    public int itemID; //物品id
    public string itemName;//物品名
    public ItemType itemType; //具体类型
    
    public Sprite itemIcon;//物品图标
    public Sprite itemSpriteInWorld;//物品掉落的图片
    
    public string itemDescription; //物品描述
    
    public int itemUseRadius;//物品可使用半径
    
    public bool canBePickedUp; //是否可以被拾取
    public bool canBeDropped; //是否可被丢弃
    public bool canBeLifted;//是否可以在手中被举起
    
    public int itemPrice; //价格
    [Range(0,1)]
    public float itemSellPercentage;  //售卖返还百分比




}

/// <summary>
/// 物品栏物品结构体
/// 用结构体有初始值
/// </summary>
[System.Serializable]
public struct InventoryItem
{
    public int itemID; //物品id, 0代表没有物品
    public int amount; //物品栏中物品数量
}
