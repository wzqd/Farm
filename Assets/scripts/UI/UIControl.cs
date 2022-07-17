using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class UIControl : MonoBehaviour
{
    private bool bagIsShown;
    private bool bagIsOpened;
    
    
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
        UIMgr.Instance.ShowPanel<PlayerToolBar>("PlayerToolBar", E_PanelLayer.Bot);
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
                UIMgr.Instance.ShowPanel<PlayerBag>("PlayerBag", E_PanelLayer.Mid);
                UIMgr.Instance.GetPanel<PlayerToolBar>("PlayerToolBar").gameObject.SetActive(false); //隐藏物品栏
                bagIsShown = true;
                bagIsOpened = true;
                return;
            }

            if (bagIsOpened) //如果正打开，关闭
            {
                UIMgr.Instance.GetPanel<PlayerBag>("PlayerBag").gameObject.SetActive(false); //隐藏面板
                UIMgr.Instance.GetPanel<PlayerToolBar>("PlayerToolBar").gameObject.SetActive(true); //显示物品栏
                bagIsOpened = false;
            }
            else //如果关闭，则打开
            {
                UIMgr.Instance.GetPanel<PlayerBag>("PlayerBag").gameObject.SetActive(true); //显示面板
                UIMgr.Instance.GetPanel<PlayerToolBar>("PlayerToolBar").gameObject.SetActive(false); //隐藏物品栏
                bagIsOpened = true;
            }
            
        }

        
    }
}
