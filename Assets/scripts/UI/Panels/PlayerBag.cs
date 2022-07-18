using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
        
    }
    
 
    private void BagUIUpdate(int[] info) //info第一个位列表下标，第二个为id，第三个为数量
    {
        BagSlots[info[0]].UpdateSlot(InventoryMgr.Instance.GetItemDetails(info[1]), info[2]);
    }

    private void BagUIUpdateEmpty(int index)
    {
        BagSlots[index].UpdateEmptySlot();
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
        
        InventoryItem emptyItem = new InventoryItem {itemID = 0, amount = 0};
        playerInventory_SO.inventoryItemList[slot.slotIndex] = emptyItem;//清空列表数据
    }
    
    
    /// <summary>
    /// 背包中结束拖拽
    /// </summary>
    /// <param name="twoSlots"></param>
    private void BagSlotEndDrag(Slot[] twoSlots)
    {
        int endIndex = twoSlots[1].slotIndex;

        if (twoSlots[0].itemDetails.itemID == twoSlots[1].itemDetails.itemID) //两个是同一种东西
        {
            //更新列表数据
            UpdateBagList (twoSlots[0].itemDetails.itemID, twoSlots[0].itemAmount + twoSlots[1].itemAmount, endIndex);
            
        }
        else //两个不同东西
        {
            UpdateBagList(twoSlots[0].itemDetails.itemID, twoSlots[0].itemAmount, endIndex);
        }
        
        BagSlots[endIndex].UpdateSlot(twoSlots[0].itemDetails, twoSlots[0].itemAmount); //更新ui
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
