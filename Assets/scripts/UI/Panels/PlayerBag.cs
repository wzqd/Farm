using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBag : BasePanel
{
    public List<Slot> BagSlots;  //背包格子
    private InventoryTab_SO playerInventoryTab_SO; //玩家物品栏列表

    private Slot currentSelectedSlot; //现在被选中的格子
    
   public override void Show()
   {
      (transform as RectTransform).sizeDelta = new Vector2( Settings.playerBagWidth, Settings.playerBagHeight);
              
      EventMgr.Instance.AddEventListener<int>("pickUpItemToBag", pickUpItemToBag); //添加拾取物品的事件
   }
    protected override void Awake()
    {
        base.Awake(); //覆写虚函数

        string path = "Assets/GameData/Inventory/InventoryTabs/PlayerInventoryTab_SO.asset";
        playerInventoryTab_SO = AssetDatabase.LoadAssetAtPath(path, typeof(InventoryTab_SO)) as InventoryTab_SO; //得到文件

        
        for (int i = 0; i < Settings.BagCapacity; i++)
        {
            BagSlots.Add(GetUIComponent<Button>("Slot (" + i  +")").GetComponent<Slot>()); //得到背包slot
            
            BagSlots[i].slotIndex = i; //设置下标
            BagSlots[i].slotType = SlotType.BagSlot; //设置格子类型
        }

        EventMgr.Instance.AddEventListener<int>("BagSlotHighlight",BagSlotHighlight);
        
        UpdateInventoryUI(playerInventoryTab_SO.inventoryItemList); //更新所有格子
    }
    
 
    /// <summary>
    /// 更新所有物品栏UI信息
    /// </summary>
    /// <param name="inventoryList"></param>
    private void UpdateInventoryUI(List<InventoryItem> inventoryList)
    {
        for (int i = 0; i < Settings.BagCapacity; i++)
        {
            if (inventoryList[i].amount > 0) //如果有物品，更新至现有信息
            {
                ItemDetails itemDetails = InventoryMgr.Instance.GetItemDetails(inventoryList[i].itemID);
                BagSlots[i].UpdateSlot(itemDetails, inventoryList[i].amount);
            }
            else //若无，清空
            {
                BagSlots[i].UpdateEmptySlot();
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
}
