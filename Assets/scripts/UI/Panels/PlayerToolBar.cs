using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerToolBar : BasePanel
{
    public List<Slot> ToolBarSlots; //所有的格子
    private InventoryTab_SO playerInventory_SO; //玩家物品栏列表
    private InventoryContainerType inventoryContainerType = InventoryContainerType.Bag; //列表类型
    
    private Slot currentSelectedSlot; //现在被选中的格子

    protected override void Awake()
    {
        base.Awake(); //覆写虚函数

        string path = "Assets/GameData/Inventory/InventoryTabs/PlayerInventoryTab_SO.asset";
        playerInventory_SO = AssetDatabase.LoadAssetAtPath(path, typeof(InventoryTab_SO)) as InventoryTab_SO; //得到文件


        
        for (int i = 0; i < Settings.toolBarCapacity; i++)
        {
            ToolBarSlots.Add(GetUIComponent<Button>("Slot (" + i  +")").GetComponent<Slot>()); //得到所有slot （保证命名从0开始）

            ToolBarSlots[i].slotIndex = i; //设置下标
            ToolBarSlots[i].slotType = SlotType.ToolBarSlot; //设置格子类型
        }
        
        EventMgr.Instance.AddEventListener<int>("ToolBarSlotHighlight", ToolBarSlotHighlight);
        EventMgr.Instance.AddEventListener<Slot[]>("ToolBarSlotEndDrag", ToolBarSlotEndDrag);
        EventMgr.Instance.AddEventListener<Slot>("ToolBarSlotBeginDrag", ToolBarSlotBeginDrag);
        
        EventMgr.Instance.AddEventListener<int[]>("ToolBarUIUpdate", ToolBarUIUpdate); //更新第i位的ui
        EventMgr.Instance.AddEventListener<int>("ToolBarUIUpdateEmpty", ToolBarUIUpdateEmpty); //更新第i位的ui
        EventMgr.Instance.AddEventListener<int>("ToolBarUIUpdateQuantityEmpty", ToolBarUIUpdateQuantityEmpty); //更新第i位的ui


        EventMgr.Instance.AddEventListener<int>("CleanSlotHighlight", CleanSlotHighlight);
        EventMgr.Instance.AddEventListener("CleanCurrentSlotHighlight", CleanCurrentSlotHighlight);
    }

    public override void Show()
    {
        //调整UI位置
        (transform as RectTransform).sizeDelta = new Vector2(Settings.playerToolBarWidth, Settings.playerToolBarHeight); 
        
        EventMgr.Instance.AddEventListener<int>("pickUpItemToBag", pickUpItemToBag); //添加拾取物品的事件
        
    }

    /// <summary>
    /// 更新UI信息
    /// </summary>
    /// <param name="info"></param>
    private void ToolBarUIUpdate(int[] info) //info第一个位列表下标，第二个为id，第三个为数量
    {
        ToolBarSlots[info[0]].UpdateSlot(InventoryMgr.Instance.GetItemDetails(info[1]), info[2]);
    }

    /// <summary>
    /// 清空UI信息
    /// </summary>
    /// <param name="index"></param>
    private void ToolBarUIUpdateEmpty(int index)
    {
        ToolBarSlots[index].UpdateEmptySlot();
    }
    
    /// <summary>
    /// 清空UI上数量
    /// </summary>
    /// <param name="index"></param>
    private void ToolBarUIUpdateQuantityEmpty(int index)
    {
        ToolBarSlots[index].EmptySlotQuantity();
    }

    /// <summary>
    /// 更新拾取物品的UI信息
    /// </summary>
    /// <param name="itemID"></param>
    private void pickUpItemToBag(int itemID) //将SO和物品栏下标一一对应
    {
        ItemDetails itemDetails = InventoryMgr.Instance.GetItemDetails(itemID);//得到要捡起的物品信息
        int index = InventoryMgr.Instance.GetIndexByItemID(itemID);//得到要捡起的物品下标
        if (index >= Settings.toolBarCapacity) return; //如果超出物品栏大小，直接返回
        int quantity = InventoryMgr.Instance.GetQuantityByItemID(itemID); //得到要捡起物品的数量
        ToolBarSlots[index].UpdateSlot(itemDetails, quantity);
    }

    private void ToolBarSlotHighlight(int index)
    {
        if (currentSelectedSlot is null) //第一次选中
        {
            currentSelectedSlot = ToolBarSlots[index];
            currentSelectedSlot.isSelected = true;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(true);
        }

        if (ToolBarSlots[index] != currentSelectedSlot) //如果点选了新的格子
        {
            currentSelectedSlot.isSelected = false;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(false); //取消之前的选中
            
            currentSelectedSlot = ToolBarSlots[index]; //赋值为新的格子
            currentSelectedSlot.isSelected = true;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(true); //选中新的格子
        }

        if (ToolBarSlots[index].itemAmount> 0 && ToolBarSlots[index].itemDetails.canBeLifted) //如果高亮的是可以被举起的物品 
        { //（并且数量大于0，因为itemDetails只有覆盖是才会更新）
            
            EventMgr.Instance.EventTrigger("PlayerLiftItem", ToolBarSlots[index].itemDetails); //触发事件让玩家举起
        }
        else
        {
            EventMgr.Instance.EventTrigger("PlayerCancelLiftItem"); //触发事件取消举起
        }
        
        
    }
    
    /// <summary>
    /// 上面格子开始拖拽
    /// </summary>
    /// <param name="slot"></param>
   private void ToolBarSlotBeginDrag(Slot slot)
    {
        ToolBarSlots[slot.slotIndex].UpdateEmptySlot(); //UI上清空对应id格子UI
        
    }
   
   
    /// <summary>
    /// 格子结束拖拽时停在上面
    /// </summary>
    private void ToolBarSlotEndDrag(Slot[] twoSlots) //第一个为起始slot，第二个为最终slot
    {

        int startIndex = twoSlots[0].slotIndex;
        int startSlotItemID = twoSlots[0].itemDetails.itemID;
        int startSlotAmount = twoSlots[0].itemAmount;
        
        int endIndex = twoSlots[1].slotIndex;
        int endSlotItemID = twoSlots[1].itemDetails.itemID; //暂存数量和id
        int endSlotAmount = twoSlots[1].itemAmount;

        if (startSlotItemID == endSlotItemID || endSlotAmount == 0) //两个是同一种东西 或者 后者是空格子（空格子不能通过inventoryMgr得details
        {
            //更新列表数据
            UpdateBagList(startSlotItemID, startSlotAmount + endSlotAmount, endIndex);
            ToolBarSlots[endIndex].UpdateSlot(twoSlots[0].itemDetails, startSlotAmount + endSlotAmount); //更新ui
            EventMgr.Instance.EventTrigger("BagUIUpdate", new[] {endIndex, startSlotItemID, startSlotAmount});//更新背包ui
            
            InventoryItem emptyItem = new InventoryItem {itemID = 0, amount = 0};
            playerInventory_SO.inventoryItemList[startIndex] = emptyItem;//清空原先列表数据（数据同步）
        
            twoSlots[0].EmptySlotQuantity();//清空原先格子数量
            EventMgr.Instance.EventTrigger("BagUIUpdateQuantityEmpty", startIndex); //清空背包中物品栏数量（UI不同步）
            
        }
        else //两个不同东西
        {
            
            UpdateBagList(startSlotItemID, startSlotAmount, endIndex); //更新放下时的数据
            ToolBarSlots[endIndex].UpdateSlot(twoSlots[0].itemDetails, startSlotAmount); //更新放下时ui

            EventMgr.Instance.EventTrigger("BagUIUpdate", new[] {endIndex, startSlotItemID, startSlotAmount});

            switch (twoSlots[0].slotType) //判断原来的类型,并且更新原来的格子和数据
            {
                case SlotType.ToolBarSlot:
                    InventoryMgr.Instance.UpdateListInfo(InventoryContainerType.Bag, endSlotItemID, endSlotAmount,
                         startIndex);
                    EventMgr.Instance.EventTrigger("ToolBarUIUpdate", new[] {startIndex, endSlotItemID, endSlotAmount});

                    EventMgr.Instance.EventTrigger("BagUIUpdate", new[] {startIndex, endSlotItemID, endSlotAmount});
                    break;
                // case SlotType.BagSlot:不需要，因为是同一个数据结构（也不可能同时开启）
                //     break;
                case SlotType.BoxSlot:
                    InventoryMgr.Instance.UpdateListInfo(InventoryContainerType.Box, endSlotItemID, endSlotAmount,
                        startIndex);
                    //调用对应面板里的事件（还没写）
                    break;
                case SlotType.ShopSlot:
                    InventoryMgr.Instance.UpdateListInfo(InventoryContainerType.Shop, endSlotItemID, endSlotAmount,
                        startIndex);
                    //调用对应面板里的事件（还没写）
                    break;
            }
        }
    }

    /// <summary>
    /// 清除对应格子的高亮
    /// </summary>
    private void CleanSlotHighlight(int index)
    {
        ToolBarSlots[index].isSelected = false;
        ToolBarSlots[index].slotHighlight.gameObject.SetActive(false); //取消选中，清除高亮图片
        currentSelectedSlot = null;
        
        EventMgr.Instance.EventTrigger("PlayerCancelLiftItem"); //触发事件取消举起
    }    
    
    /// <summary>
    /// 清除现有格子的高亮
    /// </summary>
    private void CleanCurrentSlotHighlight()
    {
        print(currentSelectedSlot);
        if (currentSelectedSlot is not null)
        {
            currentSelectedSlot.isSelected = false;
            currentSelectedSlot.slotHighlight.gameObject.SetActive(false); //取消选中，清除高亮图片
            currentSelectedSlot = null;
        }
        
        print(currentSelectedSlot);
        EventMgr.Instance.EventTrigger("PlayerCancelLiftItem"); //触发事件取消举起
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
