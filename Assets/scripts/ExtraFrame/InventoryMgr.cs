using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#region 物品管理器
//用于储存以及得到所有的物品

//主要有根据id得到物品的方法
#endregion
public class InventoryMgr : SingletonMono<InventoryMgr>
{
    /// <summary>
    /// 物品列表
    /// </summary>
    private ItemDataList_SO itemDataList_SO;

    private void Awake()
    {
        string path = "Assets/GameData/Inventory/ItemDataList_SO.asset";
        itemDataList_SO = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
    }

    /// <summary>
    /// 根据ID获得物品信息类
    /// </summary>
    /// <param name="ID">物品id</param>
    /// <returns></returns>
    public ItemDetails GetItemDetails(int ID)
    {
        return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
    }
    
    
}
