using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;


public class UIControl : MonoBehaviour
{
    private bool bagIsShown;
    private bool bagIsOpened;

    private GameObject playertoolbar;
    private CanvasGroup playerToolBarCanvasGroup;
    private CanvasGroup playerBagCanvasGroup;
    
    
    
    void Start()
    {
        ShowPlayerToolBar();
        ShowDragPanel();
        
    }

    // Update is called once per frame
    void Update()
    {
        OpenClosePlayerBag();
    }

    private void ShowPlayerToolBar()
    {
        UIMgr.Instance.ShowPanel<PlayerToolBar>("PlayerToolBar", E_PanelLayer.Bot,(bar) =>
        {
            //得到CanvasGroup用于隐藏
            playerToolBarCanvasGroup = bar.GetComponent<CanvasGroup>();
            EventMgr.Instance.EventTrigger("UpdateUIByInventoryInfo", SlotType.ToolBarSlot);//触发更新UI
        });
        
    }

    private void ShowDragPanel()
    {
        UIMgr.Instance.ShowPanel<DragPanel>("DragPanel", E_PanelLayer.Top);
    }

    /// <summary>
    /// 开关玩家背包
    /// </summary>
    private void OpenClosePlayerBag()
    {
        if (Input.GetKeyDown(KeyCode.E)) //之后可以设置键位
        {
            if (!bagIsShown) //如果还未加载，加载面板
            {
                UIMgr.Instance.ShowPanel<PlayerBag>("PlayerBag", E_PanelLayer.Mid, (bag) =>
                {
                    //得到CanvasGroup用于隐藏
                    playerBagCanvasGroup = bag.GetComponent<CanvasGroup>();
                    EventMgr.Instance.EventTrigger("UpdateUIByInventoryInfo", SlotType.BagSlot); //触发更新UI
                });
                
                InactivePanel(playerToolBarCanvasGroup); //隐藏物品栏
                bagIsShown = true;
                bagIsOpened = true;
                return;
            }

            if (bagIsOpened) //如果正打开，关闭
            {
                InactivePanel(playerBagCanvasGroup); //隐藏面板
                ActivePanel(playerToolBarCanvasGroup); //显示物品栏
                bagIsOpened = false;
            }
            else //如果关闭，则打开
            {
                ActivePanel(playerBagCanvasGroup); //显示面板
                InactivePanel(playerToolBarCanvasGroup); //隐藏物品栏
                bagIsOpened = true;
            }
        }
    }
    private void ActivePanel(CanvasGroup panel) //利用canvasGroup隐藏，可以在隐藏时调用代码
    {
        panel.alpha = 1;
        panel.interactable = true;
        panel.blocksRaycasts = true;
    }
    private void InactivePanel(CanvasGroup panel)
    {
        panel.alpha = 0;
        panel.interactable = false;
        panel.blocksRaycasts = false;
    }
}
