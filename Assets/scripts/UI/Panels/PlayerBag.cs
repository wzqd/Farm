using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class PlayerBag : BasePanel
{
    public List<Slot> BagSlots;  //背包格子
    private InventoryTab_SO playerInventory_SO; //玩家物品栏列表

    private Slot currentSelectedSlot; //现在被选中的格子
    private InventoryContainerType inventoryContainerType = InventoryContainerType.Bag; //列表类型
    
   public override void Show()
   {
      (transform as RectTransform).sizeDelta = new Vector2( Settings.playerBagWidth, Settings.playerBagHeight);
              
      EventMgr.Instance.AddEventListener<int>("pickUpItemToBag", pickUpItemToBag); //添加拾取物品的事件
   }
    protected override void Awake()
    {
        base.Awake(); //覆写虚函数

        string path = "Assets/GameData/Inventory/InventoryTabs/PlayerInventoryTab_SO.asset";
        playerInventory_SO = AssetDatabase.LoadAssetAtPath(path, typeof(InventoryTab_SO)) as InventoryTab_SO; //得到文件

        
        for (int i = 0; i < Settings.BagCapacity; i++)
        {
            BagSlots.Add(GetUIComponent<Button>("Slot (" + i  +")").GetComponent<Slot>()); //得到背包slot
            
            BagSlots[i].slotIndex = i; //设置下标
            BagSlots[i].slotType = SlotType.BagSlot; //设置格子类型
        }

        EventMgr.Instance.AddEventListener<int>("BagSlotHighlight",BagSlotHighlight);
        EventMgr.Instance.AddEventListener<Slot[]>("BagSlotEndDrag", BagSlotEndDrag);
        EventMgr.Instance.AddEventListener<Slot>("BagSlotBeginDrag", BagSlotBeginDrag);
        
        EventMgr.Instance.AddEventListener<int[]>("BagUIUpdate", BagUIUpdate); //更新第i位的ui
        EventMgr.Instance.AddEventListener<int>("BagUIUpdateEmpty", BagUIUpdateEmpty); //更新第i位的ui
        EventMgr.Instance.AddEventListener<int>("BagUIUpdateQuantityEmpty", BagUIUpdateQuantityEmpty); //更新第i位的ui
        
        
    }
    
 
    /// <summary>
    /// 更新UI信息
    /// </summary>
    /// <param name="info"></param>
    private void BagUIUpdate(int[] info) //info第一个位列表下标，第二个为id，第三个为数量
    {
        BagSlots[info[0]].UpdateSlot(InventoryMgr.Instance.GetItemDetails(info[1]), info[2]);
    }

    /// <summary>
    /// 清空UI信息
    /// </summary>
    /// <param name="index"></param>
    private void BagUIUpdateEmpty(int index)
    {
        BagSlots[index].UpdateEmptySlot();
    }

    /// <summary>
    /// 清空UI上数量
    /// </summary>
    /// <param name="index"></param>
    private void BagUIUpdateQuantityEmpty(int index)
    {
        BagSlots[index].EmptySlotQuantity();
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
        BagSlots[index].UpdateSlot(itemDetails, quantity);
    }

    /// <summary>
    /// 背包格子点选高亮
    /// </summary>
    /// <param name="index"></param>
    private void BagSlotHighlight(int index)
    {
        if (currentSelectedSlot is null) //第一次选中
        {
            currentSelectedSlot = BagSlots[index];
            currentSelectedSlot.isSelected = true;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(true);
        }

        if (BagSlots[index] != currentSelectedSlot) //如果点选了新的格子
        {
            currentSelectedSlot.isSelected = false;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(false); //取消之前的选中
            currentSelectedSlot = BagSlots[index]; //赋值为新的格子
            currentSelectedSlot.isSelected = true;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(true); //选中新的格子
        }
    }

    /// <summary>
    /// 背包中开始拖拽
    /// </summary>
    /// <param name="slot"></param>
    private void BagSlotBeginDrag(Slot slot)
    {
        BagSlots[slot.slotIndex].UpdateEmptySlot(); //UI上清空对应id格子
    }
    
    
    /// <summary>
    /// 背包中结束拖拽
    /// </summary>
    /// <param name="twoSlots"></param>
    private void BagSlotEndDrag(Slot[] twoSlots)
    {
        int endIndex = twoSlots[1].slotIndex;
        int startIndex = twoSlots[0].slotIndex;

        if (twoSlots[0].itemDetails.itemID == twoSlots[1].itemDetails.itemID|| twoSlots[1].itemDetails.itemID == 0) //两个是同一种东西 或者 后者是空格子
        {
            //更新自己列表数据
            UpdateBagList (twoSlots[0].itemDetails.itemID, twoSlots[0].itemAmount + twoSlots[1].itemAmount, endIndex);
            BagSlots[endIndex].UpdateSlot(twoSlots[0].itemDetails, twoSlots[0].itemAmount + twoSlots[1].itemAmount); //更新ui
            
            InventoryItem emptyItem = new InventoryItem {itemID = 0, amount = 0};
            playerInventory_SO.inventoryItemList[twoSlots[0].slotIndex] = emptyItem;//清空原先列表数据
        
            twoSlots[0].EmptySlotQuantity();//清空原先格子数量
            if (startIndex < Settings.toolBarCapacity)
            {
                EventMgr.Instance.EventTrigger("ToolBarUIUpdateQuantityEmpty", startIndex); //清空背包中物品栏数量（UI不同步）
            }
        }
        else //两个不同东西
        {
            int endSlotItemID = twoSlots[1].itemDetails.itemID; //暂存数量和id
            int endSlotAmount = twoSlots[1].itemAmount;
            
            //更新自己列表数据
            UpdateBagList(twoSlots[0].itemDetails.itemID, twoSlots[0].itemAmount, endIndex);
            BagSlots[endIndex].UpdateSlot(twoSlots[0].itemDetails, twoSlots[0].itemAmount); //更新自己ui

            //更新原先格子所在列表数据和所在UI
            switch (twoSlots[0].slotType) //判断原来的类型
            {
                // case SlotType.ToolBarSlot: 不需要，因为是同一个数据结构（也不可能同时开启）
                //     break;
                case SlotType.BagSlot: 
                    InventoryMgr.Instance.UpdateListInfo(InventoryContainerType.Bag, endSlotItemID,endSlotAmount, startIndex);
                    EventMgr.Instance.EventTrigger("BagUIUpdate", new[] {startIndex, endSlotItemID, endSlotAmount});
                    
                    if (startIndex < Settings.toolBarCapacity) //如果在起始时物品栏部分
                    {
                        EventMgr.Instance.EventTrigger("ToolBarUIUpdate", new[] {startIndex, endSlotItemID, endSlotAmount});
                    }

                    break;
                case SlotType.BoxSlot:
                    InventoryMgr.Instance.UpdateListInfo(InventoryContainerType.Box, endSlotItemID,endSlotAmount, startIndex);
                    //调用对应面板里的事件（还没写）
                    break;
                case SlotType.ShopSlot:
                    InventoryMgr.Instance.UpdateListInfo(InventoryContainerType.Shop, endSlotItemID,endSlotAmount, startIndex);
                    //调用对应面板里的事件（还没写）
                    break;
            }
        }
        
    }
    
    
    /// <summary>
    /// 更新背包数据
    /// 封装了一遍inventoryMgr中的方法
    /// </summary>
    /// <param name="id"></param>
    /// <param name="amount"></param>
    /// <param name="index"></param>
    private void UpdateBagList(int id, int amount, int index)
    {
        InventoryMgr.Instance.UpdateListInfo(inventoryContainerType, id, amount, index);
    }
}
