using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class ItemEditor : EditorWindow
{
    private ItemDataList_SO itemDataBase;
    private List<ItemDetails> itemList = new List<ItemDetails>();
    private VisualTreeAsset itemRowTemplate; //itemRow的模板文件
    private ListView itemListView; //左侧的列表视图
    
    
    [MenuItem("CustomEditors/ItemEditor")]
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemEditor.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UIBuilder/ItemTemplate.uxml"); //得到模板

        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView"); //Q是得到元素的方法
        
        LoadDataBase(); //加载SO
        GenerateListView();  //加载左侧视图

    }

    private void LoadDataBase()
    {
        string[] dataArray = AssetDatabase.FindAssets("ItemDataList_SO");
        if (dataArray.Length > 1)
        {
            string path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            itemDataBase = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
        }

        itemList = itemDataBase.itemDetailsList;
        EditorUtility.SetDirty(itemDataBase); //标记为dirty才能保存数据
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => itemRowTemplate.CloneTree();
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].itemIcon != null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;
                e.Q<Label>("Name").text = itemList[i] == null ? "No ITEM" : itemList[i].itemName;
            }
        };
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        

    }
}