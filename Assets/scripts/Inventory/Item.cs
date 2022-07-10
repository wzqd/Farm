using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 物品类
/// </summary>
public class Item : MonoBehaviour
{
    public int itemID; //物品id
    public ItemDetails itemDetails; //物品信息

    private SpriteRenderer spriteRenderer; //物品图片
    private BoxCollider2D itemCollider; //物品碰撞器

    private void Awake() //开始得信息
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        itemCollider = GetComponent<BoxCollider2D>();
        
        
    }

    private void Start()
    {
        if (itemID != 0)
        {
            Init(itemID); //初始化信息
        }
    }

    private void Init(int ID) //用id初始化物品（物品需要有id否则报错）
    {
        itemID = ID;

        itemDetails = InventoryMgr.Instance.GetItemDetails(itemID); //得到物品数据

        if (itemDetails != null)
        {
            //赋予图片
            spriteRenderer.sprite = itemDetails.itemSpriteInWorld != null
                ? itemDetails.itemSpriteInWorld
                : itemDetails.itemIcon;

            //修改碰撞体尺寸，使其贴合图片 (对应物品要有图片否则报错）
            //*********以后可以改成固定大小，并且添加吸附效果****************
            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y); 
            itemCollider.size = newSize;
            itemCollider.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y); //因为锚点不在中心，要调整偏移
        }
    }
}
