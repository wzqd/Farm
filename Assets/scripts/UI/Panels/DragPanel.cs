using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class DragPanel : BasePanel
{
    [SerializeField]private Image dragImage;

    protected override void Awake()
    {
        base.Awake();
        dragImage = GetUIComponent<Image>("DragItem");
        dragImage.enabled = false; //初始不显示图片
        
        EventMgr.Instance.AddEventListener("SlotOnDrag",SlotOnDrag);
        EventMgr.Instance.AddEventListener<Slot>("SlotBeginDrag",SlotBeginDrag);
        EventMgr.Instance.AddEventListener("SlotEndDrag",SlotEndDrag);
    }

    private void SlotOnDrag()
    {
        dragImage.gameObject.transform.position = Input.mousePosition;
    }

    private void SlotBeginDrag(Slot slot)
    {
        dragImage.sprite = slot.itemImage.sprite;
        dragImage.SetNativeSize();
        dragImage.enabled = true;
        
        slot.isSelected = true;
    }

    private void SlotEndDrag()
    {
        dragImage.enabled = false;
    }
    
}
