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

    private void OpenClosePlayerBag()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!bagIsShown) //如果还未加载，加载面板
            {
                UIMgr.Instance.ShowPanel<PlayerBag>("PlayerBag", E_PanelLayer.Mid);
                bagIsShown = true;
                bagIsOpened = true;
                return;
            }

            if (bagIsOpened) //如果正打开，关闭
            {
                UIMgr.Instance.GetPanel<PlayerBag>("PlayerBag").gameObject.SetActive(false);
                bagIsOpened = false;
            }
            else //如果关闭，则打开
            {
                UIMgr.Instance.GetPanel<PlayerBag>("PlayerBag").gameObject.SetActive(true);
                bagIsOpened = true;
            }
            
        }

        
    }
}
