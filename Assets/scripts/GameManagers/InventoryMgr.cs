using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;

public enum InventoryContainerType
{
    Bag,
    Box,
    Shop,
}

#region 物品管理器
//用于储存以及得到所有的物品

//主要有根据id得到物品的方法
#endregion
public class InventoryMgr : Singleton<InventoryMgr>
{
    private ItemDataList_SO itemDataList_SO; // 所有物品列表 （只读，只在Editor中修改）
    
    private InventoryTab_SO playerInventory_SO; //玩家物品栏列表
    

    public InventoryMgr()
    {
        string path = "Assets/GameData/Inventory/ItemDataList_SO.asset";
        itemDataList_SO = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO; //得到文件

        path = "Assets/GameData/Inventory/InventoryTabs/PlayerInventoryTab_SO.asset";
        playerInventory_SO = AssetDatabase.LoadAssetAtPath(path, typeof(InventoryTab_SO)) as InventoryTab_SO; //得到文件

        EventMgr.Instance.AddEventListener<GameObject>("PickUpItems", pickUpItemToBag); //监听捡起物品事件
        EventMgr.Instance.AddEventListener<SlotType>("UpdateUIByInventoryInfo", UpdateUIByInventoryInfo);
        
    }

    /// <summary>
    /// 更改指定数据列表中的某个数据
    /// </summary>
    /// <param name="inventoryContainerType">指定的容器类型</param>
    /// <param name="id">物品id</param>
    /// <param name="amount">物品数量</param>
    /// <param name="index">下标</param>
    public void UpdateListInfo(InventoryContainerType inventoryContainerType, int id, int amount, int index)
    {
        switch (inventoryContainerType)
        {
            case InventoryContainerType.Bag:
                InventoryItem newItem = new InventoryItem
                    {itemID = id, amount = amount};
                playerInventory_SO.inventoryItemList[index] = newItem; //更新列表中数据
                break;
            case InventoryContainerType.Box: //更新对应列表的数据
                break;
            case InventoryContainerType.Shop:
                break;
        }

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
    /// 通过物品ID找到背包已有物品位置
    /// </summary>
    /// <param name="itemID">物品ID</param>
    /// <returns>若有返回序号，若无返回-1</returns>
    public int GetIndexByItemID(int itemID)
    {
        for (int i = 0; i < playerInventory_SO.inventoryItemList.Count; i++)
        {
            if (playerInventory_SO.inventoryItemList[i].itemID == itemID)
                return i;
        }
        return -1;
    }
    
    /// <summary>
    /// 通过物品ID查找背包中物品数量
    /// 只返回第一个找到的数量，并不是全部的数量！
    /// </summary>
    /// <param name="itemID">物品ID</param>
    /// <returns>返回物品数量，若0则表示没有改物品</returns>
    public int GetQuantityByItemID(int itemID)
    {
        for (int i = 0; i < playerInventory_SO.inventoryItemList.Count; i++)
        {
            if (playerInventory_SO.inventoryItemList[i].itemID == itemID)
                return playerInventory_SO.inventoryItemList[i].amount;
        }
        return 0;
    }
    
    /// <summary>
    /// 更新指定UI上所有格子
    /// </summary>
    /// <param name="slotType"></param>
    private void UpdateUIByInventoryInfo(SlotType slotType)
    {
        switch (slotType)
        {            
            case SlotType.ToolBarSlot:
                List<InventoryItem> inventoryList = playerInventory_SO.inventoryItemList;
                
                for (int i = 0; i < Settings.toolBarCapacity; i++)
                {
                    if (inventoryList[i].amount > 0) //如果有物品，更新至现有信息
                    {
                        ItemDetails itemDetails = InventoryMgr.Instance.GetItemDetails(inventoryList[i].itemID);
                        EventMgr.Instance.EventTrigger("ToolBarUIUpdate", new[]{i, inventoryList[i].itemID, inventoryList[i].amount}); //更新第i位的ui

                    }
                    else //若无，清空
                    {
                        EventMgr.Instance.EventTrigger("ToolBarUIUpdateEmpty", i); //清空第i位的ui
                    }
                }
                break;
            case SlotType.BagSlot:
                inventoryList = playerInventory_SO.inventoryItemList;
                
                for (int i = 0; i < Settings.BagCapacity; i++)
                {
                    if (inventoryList[i].amount > 0) //如果有物品，更新至现有信息
                    {
                        ItemDetails itemDetails = InventoryMgr.Instance.GetItemDetails(inventoryList[i].itemID);
                        EventMgr.Instance.EventTrigger("BagUIUpdate", new[]{i, inventoryList[i].itemID, inventoryList[i].amount}); //更新第i位的ui
                        
                    }
                    else //若无，清空
                    {
                        EventMgr.Instance.EventTrigger("BagUIUpdateEmpty", i); //清空第i位的ui
                    }
                }
                break;

            case SlotType.BoxSlot:
                break;
            case SlotType.ShopSlot:
                break;
        }
        
    }
    
    /// <summary>
    /// 拾取物品加入物品栏
    /// </summary>
    /// <param name="itemObj"></param>
    private void pickUpItemToBag(GameObject itemObj)
    {
        Item item = itemObj.GetComponent<Item>();
        if(item is null) return;
        
        int index = GetIndexByItemID(item.itemID); //得到已有物品位置或者第一个空位
        AddItemToBagListAtIndex(item.itemID,index,1); //添加一个（后可用参数改）
        
        if (item != null && item.itemDetails.canBePickedUp)
        {
            PoolMgr.Instance.PushObj(item.name, itemObj); //利用缓存池管理散落在地上的物品
        }
        
        EventMgr.Instance.EventTrigger("pickUpItemToBag",item.itemID); //触发加入背包事件
    }
    
    /// <summary>
    /// 检查背包是否有空位
    /// </summary>
    /// <returns>是否有空位</returns>
    private bool CheckBagListCapacity()
    {
        foreach (InventoryItem item in playerInventory_SO.inventoryItemList)
        {
            if (item.itemID == 0) return true; //发现第一个空位的时候返回
        }
        return false; //没有空位返回false
    }



    /// <summary>
    /// 在指定背包序号位置添加物品
    /// </summary>
    /// <param name="itemID">物品ID</param>
    /// <param name="index">序号</param>
    /// <param name="amount">数量</param>
    private void AddItemToBagListAtIndex(int itemID, int index, int amount)
    {
        if (index == -1 && CheckBagListCapacity()) //背包没有这个物品 同时背包有空位
        {
            InventoryItem newItem = new InventoryItem {itemID = itemID, amount = amount};
            for (int i = 0; i < playerInventory_SO.inventoryItemList.Count; i++)
            {
                if (playerInventory_SO.inventoryItemList[i].itemID == 0) //找到第一个空位
                {
                    playerInventory_SO.inventoryItemList[i] = newItem; //直接更新物品栏信息
                    break;
                }
            }
        }
        else if(index != -1)  //背包有这个物品
        {
            int currentAmount = playerInventory_SO.inventoryItemList[index].amount + amount; //增加数量
            InventoryItem item = new InventoryItem {itemID = itemID, amount = currentAmount};
            playerInventory_SO.inventoryItemList[index] = item; //更新物品栏信息
        }
        //没有这个物品而且背包满了则不添加
    }
}
