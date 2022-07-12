using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    // Start is called before the first frame update
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
        UIMgr.Instance.ShowPanel<PlayerToolBar>("PlayerToolBar",E_PanelLayer.Bot, (bar) =>
        {
            //调整UI位置
            (bar.transform as RectTransform).sizeDelta = new Vector2(Settings.playerToolBarWidth, Settings.playerToolBarHeight); 
        });
    }
}
