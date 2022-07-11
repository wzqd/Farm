using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品栏信息
/// </summary>
[CreateAssetMenu(fileName = "InventoryTab_SO", menuName = "Inventory/InventoryTab")]
public class InventoryTab_SO : ScriptableObject
{
    public List<InventoryItem> inventoryItemList;
}
