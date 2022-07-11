using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有物品信息
/// </summary>
[CreateAssetMenu(fileName = "ItemDataList_SO", menuName = "Inventory/ItemDataList")]
public class ItemDataList_SO : ScriptableObject //可使用itemEditor进行配置
{
    public List<ItemDetails> itemDetailsList;
}
