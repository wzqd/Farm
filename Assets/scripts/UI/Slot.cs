using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [Header("组件获取")]
    [SerializeField] private Image itemImage; //物品图片
    [SerializeField] private TextMeshProUGUI amountText; //数量文字
    [SerializeField] private Image slotHighlight; //高亮边框图片
    [SerializeField] private Button button; //按钮组件
    [Header("格子类型")]
    public SlotType slotType; //格子枚举类型
    public bool isSelected; //是否被选中

    [Header("物品信息")]
    public ItemDetails itemDetails;
    public int itemAmount;


    private void Start()
    {
        isSelected = false;
        if (itemDetails.itemID == 0)
        {
            UpdateEmptySlot();
        }
    }

    /// <summary>
    /// 更新格子UI和信息
    /// </summary>
    /// <param name="item">ItemDetails</param>
    /// <param name="amount">持有数量</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        itemDetails = item;
        itemImage.sprite = item.itemIcon;
        itemAmount = amount;
        amountText.text = amount.ToString();
        if (itemAmount == 0) //如果没有则不显示数字
            amountText.enabled = false;
        button.interactable = true;
    }

    /// <summary>
    /// 将Slot更新为空
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected)
            isSelected = false;
        if (itemAmount == 0)//如果没有则不显示数字
            amountText.enabled = false;
        itemImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }
}
