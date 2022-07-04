using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class SceneTree : MonoBehaviour, IFader
{
    private SpriteRenderer[] spriteRenderersInChildren;
    private Collider2D treeTrigger;

    private void Awake()
    {
        spriteRenderersInChildren = GetComponentsInChildren<SpriteRenderer>();
        treeTrigger = GetComponent<Collider2D>();
        
        FaderMgr.Instance.AddFaderRegistry(this.gameObject.name, this); //向管理器登记自己 （每次换场景也要登记）
    }


    /// <summary>
    /// 淡出
    /// </summary>
    public void FadeOut(string treeName)
    {
        Color fadeColor = new Color(1f, 1f, 1f, 1f);
        foreach (var sr in spriteRenderersInChildren)
        {
            sr.DOColor(fadeColor, Settings.fadeDuration); //不用dotween可以使用协程（计时）慢慢改变透明度
        }
    }

    /// <summary>
    /// 淡入
    /// </summary>
    public void FadeIn(string treeName)
    {
        Color fadeColor = new Color(1f, 1f, 1f, Settings.fadeAlpha);
        foreach (var sr in spriteRenderersInChildren)
        {
            sr.DOColor(fadeColor, Settings.fadeDuration); //不用dotween可以使用协程（计时）慢慢改变透明度
        }
    }
}
