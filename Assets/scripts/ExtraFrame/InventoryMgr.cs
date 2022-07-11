using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#region 物品管理器
//用于储存以及得到所有的物品

//主要有根据id得到物品的方法
#endregion
public class InventoryMgr : Singleton<InventoryMgr>
{
    private ItemDataList_SO itemDataList_SO; // 所有物品列表
    private InventoryTab_SO playerInventoryTab_SO; //玩家物品栏列表
    

    public InventoryMgr()
    {
        string path = "Assets/GameData/Inventory/ItemDataList_SO.asset";
        itemDataList_SO = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO; //得到文件

        path = "Assets/GameData/Inventory/InventoryTabs/PlayerInventoryTab_SO.asset";
        playerInventoryTab_SO = AssetDatabase.LoadAssetAtPath(path, typeof(InventoryTab_SO)) as InventoryTab_SO; //得到文件
        
        EventMgr.Instance.AddEventListener<GameObject>("PickUpItems", addItemToBag); //监听捡起物品事件
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

    /// <summary>
    /// 拾取物品加入物品栏
    /// </summary>
    /// <param name="itemObj"></param>
    private void addItemToBag(GameObject itemObj)
    {
        Item item = itemObj.GetComponent<Item>();
        if (item != null && item.itemDetails.canBePickedUp)
        {
            PoolMgr.Instance.PushObj(item.name, itemObj); //利用缓存池管理散落在地上的物品
        }
    }
    
}
