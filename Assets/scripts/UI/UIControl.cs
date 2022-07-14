using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    private InventoryTab_SO playerInventoryTab_SO; //玩家物品栏列表

    void Start()
    {
        ShowPlayerToolBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShowPlayerToolBar()
    {
        UIMgr.Instance.ShowPanel<PlayerToolBar>("PlayerToolBar", E_PanelLayer.Bot);

    }
}
