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
        
        EventMgr.Instance.AddEventListener<GameObject>("PickUpItems", addItemToInventory); //监听捡起物品事件
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
    private void addItemToInventory(GameObject itemObj)
    {
        Item item = itemObj.GetComponent<Item>();
        if(item is null) return;
        
        int index = GetIndexByItemID(item.itemID); //得到已有物品位置或者第一个空位
        AddItemToInventoryAtIndex(item.itemID,index,1);
        
        if (item != null && item.itemDetails.canBePickedUp)
        {
            PoolMgr.Instance.PushObj(item.name, itemObj); //利用缓存池管理散落在地上的物品
        }
    }
    
    /// <summary>
    /// 检查背包是否有空位
    /// </summary>
    /// <returns>是否有空位</returns>
    private bool CheckBagCapacity()
    {
        foreach (InventoryItem item in playerInventoryTab_SO.inventoryItemList)
        {
            if (item.itemID == 0) return true; //发现第一个空位的时候返回
        }
        return false; //没有空位返回false
    }

    /// <summary>
    /// 通过物品ID找到背包已有物品位置
    /// </summary>
    /// <param name="itemID">物品ID</param>
    /// <returns>若有返回序号，若无返回-1</returns>
    private int GetIndexByItemID(int itemID)
    {
        for (int i = 0; i < playerInventoryTab_SO.inventoryItemList.Count; i++)
        {
            if (playerInventoryTab_SO.inventoryItemList[i].itemID == itemID)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 在指定背包序号位置添加物品
    /// </summary>
    /// <param name="itemID">物品ID</param>
    /// <param name="index">序号</param>
    /// <param name="amount">数量</param>
    private void AddItemToInventoryAtIndex(int itemID, int index, int amount)
    {
        if (index == -1 && CheckBagCapacity()) //背包没有这个物品 同时背包有空位
        {
            InventoryItem newItem = new InventoryItem {itemID = itemID, amount = amount};
            for (int i = 0; i < playerInventoryTab_SO.inventoryItemList.Count; i++)
            {
                if (playerInventoryTab_SO.inventoryItemList[i].itemID == 0) //找到第一个空位
                {
                    playerInventoryTab_SO.inventoryItemList[i] = newItem; //直接更新物品栏信息
                    break;
                }
            }
        }
        else if(index != -1)  //背包有这个物品
        {
            int currentAmount = playerInventoryTab_SO.inventoryItemList[index].amount + amount; //增加数量
            InventoryItem item = new InventoryItem {itemID = itemID, amount = currentAmount};
            playerInventoryTab_SO.inventoryItemList[index] = item; //更新物品栏信息
        }
        //没有这个物品而且背包满了则不添加
        
    }
}
