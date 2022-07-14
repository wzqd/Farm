using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToolBar : BasePanel
{
    public List<Slot> ToolBarSlots; //所有的格子
    private InventoryTab_SO playerInventoryTab_SO; //玩家物品栏列表

    protected override void Awake()
    {
        base.Awake(); //覆写虚函数

        string path = "Assets/GameData/Inventory/InventoryTabs/PlayerInventoryTab_SO.asset";
        playerInventoryTab_SO = AssetDatabase.LoadAssetAtPath(path, typeof(InventoryTab_SO)) as InventoryTab_SO; //得到文件


        
        for (int i = 0; i < Settings.toolBarCapacity; i++)
        {
            ToolBarSlots.Add(GetUIComponent<Button>("Slot (" + i  +")").GetComponent<Slot>()); //得到所有slot （保证命名从0开始）

            ToolBarSlots[i].slotIndex = i; //设置下标
        }

        UpdateInventoryUI(playerInventoryTab_SO.inventoryItemList); //更新所有格子
    }

    public override void Show()
    {
        //调整UI位置
        (transform as RectTransform).sizeDelta = new Vector2(Settings.playerToolBarWidth, Settings.playerToolBarHeight); 
        
        EventMgr.Instance.AddEventListener<int>("pickUpItemToBag", pickUpItemToBag); //添加拾取物品的事件
        
    }
 
    /// <summary>
    /// 更新所有物品栏UI信息
    /// </summary>
    /// <param name="inventoryList"></param>
    private void UpdateInventoryUI(List<InventoryItem> inventoryList)
    {
        for (int i = 0; i < Settings.toolBarCapacity; i++)
        {
            if (inventoryList[i].amount > 0) //如果有物品，更新至现有信息
            {
                ItemDetails itemDetails = InventoryMgr.Instance.GetItemDetails(inventoryList[i].itemID);
                ToolBarSlots[i].UpdateSlot(itemDetails, inventoryList[i].amount);
            }
            else //若无，清空
            {
                ToolBarSlots[i].UpdateEmptySlot();
            }
        }
    }

    /// <summary>
    /// 更新拾取物品的信息
    /// </summary>
    /// <param name="itemID"></param>
    private void pickUpItemToBag(int itemID) //将SO和物品栏下标一一对应
    {
        ItemDetails itemDetails = InventoryMgr.Instance.GetItemDetails(itemID);//得到要捡起的物品信息
        int index = InventoryMgr.Instance.GetIndexByItemID(itemID);//得到要捡起的物品下标
        int quantity = InventoryMgr.Instance.GetQuantityByItemID(itemID); //得到要捡起物品的数量
        ToolBarSlots[index].UpdateSlot(itemDetails, quantity);

    }
}
