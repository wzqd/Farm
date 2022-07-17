using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class Slot : MonoBehaviour, IPointerClickHandler,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [Header("组件获取")]
    [SerializeField] private TextMeshProUGUI amountText; //数量文字
    [SerializeField] private Button button; //按钮组件
    public Image slotHighlight; //高亮边框图片
    public Image itemImage; //物品图片
    
    [Header("格子类型")]
    public SlotType slotType; //格子枚举类型
    public bool isSelected; //是否被选中
    public int slotIndex; //格子序号

    [Header("物品信息")]
    public ItemDetails itemDetails;
    public int itemAmount;


    private void Start()
    {
        isSelected = false;
        if (itemDetails.itemID == 0)
        {
            UpdateEmptySlot(); //开始时如果没有东西，变为空格子
        }
    }

    /// <summary>
    /// 更新格子UI中的信息
    /// </summary>
    /// <param name="item">ItemDetails类信息</param>
    /// <param name="amount">持有的数量</param>
    public void UpdateSlot(ItemDetails item, int amount)
    {
        itemDetails = item;
        itemImage.sprite = item.itemIcon;
        itemAmount = amount;
        itemImage.enabled = true; //显示图片
        amountText.text = amount.ToString();
        amountText.enabled = itemAmount > 1; //如果没有或者只有一个则不显示数字
        button.interactable = true;
        
    }

    /// <summary>
    /// 将Slot变为空
    /// </summary>
    public void UpdateEmptySlot()
    {
        if (isSelected) //若已经选中，取消选中
            isSelected = false;
        amountText.enabled = false;
        itemImage.enabled = false;
        amountText.text = string.Empty;
        button.interactable = false;
    }

    /// <summary>
    /// 格子被点击时
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        switch (slotType)
        {
            case SlotType.ToolBarSlot:
                EventMgr.Instance.EventTrigger("ToolBarSlotHighlight", slotIndex); //触发工具栏高亮事件
                break;
            case SlotType.BagSlot:
                EventMgr.Instance.EventTrigger("BagSlotHighlight", slotIndex); //触发背包高亮事件（要保证格子已经分配了下标）
                break;
            case SlotType.BoxSlot:
                break;
            case SlotType.ShopSlot:
                break;
        }
    }

    /// <summary>
    /// 格子被拖拽时
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnDrag(PointerEventData eventData)
    {
        if (itemAmount == 0) return; //如果拖拽的是空格子，返回
        EventMgr.Instance.EventTrigger("SlotOnDrag");
    }

    /// <summary>
    /// 格子开始拖拽时
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnBeginDrag(PointerEventData eventData)
    {
        switch (slotType) //开始拖拽和点击一样都要触发高亮
        {
            case SlotType.ToolBarSlot:
                EventMgr.Instance.EventTrigger("ToolBarSlotHighlight", slotIndex); //触发工具栏高亮事件
                break;
            case SlotType.BagSlot:
                EventMgr.Instance.EventTrigger("BagSlotHighlight", slotIndex); //触发背包高亮事件（要保证格子已经分配了下标）
                break;
            case SlotType.BoxSlot:
                break;
            case SlotType.ShopSlot:
                break;
        }
        
        if (itemAmount == 0) return; //如果拖拽的是空格子，返回
        EventMgr.Instance.EventTrigger("SlotBeginDrag",this);
    }

    /// <summary>
    /// 格子结束拖拽时
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (itemAmount == 0) return; //如果拖拽的是空格子，返回
        EventMgr.Instance.EventTrigger("SlotEndDrag");
        Debug.Log(eventData.pointerCurrentRaycast.gameObject);
    }
}
