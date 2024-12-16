using Farm.InventoryNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    
    private ItemSourceSO dataBase;
    private List<ItemDetail> itemSourceList;
    private ListView itemSourceListView;
    private Button addBtn;
    private ItemDetail activeItem;
    private const string itemIconString = "ItemIcon";
    private const string itemNameString = "ItemName";
    private const string itemIdString = "ItemId";

    private ScrollView itemDetailView;
    private Button deleteBtn;
    private VisualElement onWorldSpritePreview;
    private IntegerField idField;
    private TextField nameField;
    private EnumField itemTypeField;
    private ObjectField inventoryIconField;
    private ObjectField onWorldSpriteField;
    private TextField descriptionField;
    private IntegerField useRadiusField;
    private IntegerField maxStackAmountField;
    private Toggle canPickToggle;
    private Toggle canDropToggle;
    private Toggle canCarryToggle;
    private Toggle hasSpecificPrefabToggle;
    private ObjectField specificPrefabField;
    private IntegerField priceField;
    private Slider sellPercentageSlider;

    private VisualTreeAsset itemRowTemplate;

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

        /*// VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);*/

        // Import UXML
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Script/Editor/UIBuilder/ItemEditor/ItemEditor.uxml");
        
        // Instantiate UXML
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        itemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Script/Editor/UIBuilder/ItemEditor/ItemRowTemplate.uxml");
        VisualElement itemListContainer = root.Q<VisualElement>("ItemListContainer");
        itemSourceListView = itemListContainer.Q<ListView>("ItemSourceListView");
        addBtn = itemListContainer.Q<Button>("AddBtn");
        addBtn.clicked += OnAddBtnClicked;

        itemDetailView = root.Q<ScrollView>("ItemDetailView");
        deleteBtn = itemDetailView.Q<Button>("DeleteBtn");
        deleteBtn.clicked += OnDeleteBtnClicked;
        onWorldSpritePreview = root.Q<VisualElement>("OnWorldSpritePreview");
        idField = itemDetailView.Q<IntegerField>("IdField");
        nameField = itemDetailView.Q<TextField>("NameField");
        itemTypeField = itemDetailView.Q<EnumField>("ItemTypeField");
        itemTypeField.Init(ItemType.None); //IMPORTANT
        inventoryIconField = itemDetailView.Q<ObjectField>("InventoryIconField");
        // Make sure it to be UIElements.ObjectField instead of Search.ObjectField !!!
        onWorldSpriteField = itemDetailView.Q<ObjectField>("OnWorldSpriteField");
        descriptionField = itemDetailView.Q<TextField>("DescriptionField");
        useRadiusField = itemDetailView.Q<IntegerField>("UseRadiusField");
        maxStackAmountField = itemDetailView.Q<IntegerField>("MaxStackAmountField");
        canPickToggle = itemDetailView.Q<Toggle>("CanPickToggle");
        canDropToggle = itemDetailView.Q<Toggle>("CanDropToggle");
        canCarryToggle = itemDetailView.Q<Toggle>("CanCarryToggle");
        hasSpecificPrefabToggle = itemDetailView.Q<Toggle>("HasSpecificPrefabToggle");
        specificPrefabField = itemDetailView.Q<ObjectField>("SpecificPrefabField");
        priceField = itemDetailView.Q<IntegerField>("PriceField");
        sellPercentageSlider = itemDetailView.Q<Slider>("SellPercentageSlider");


        SetItemDetailViewVisivle(false);
        LoadDataBase();
        GenerateListView();
    }

    private void SetItemDetailViewVisivle(bool status)
    {
        itemDetailView.visible = status;
        inventoryIconField.visible = status;
        onWorldSpriteField.visible = status;
    }

    private void OnAddBtnClicked()
    {
        ItemDetail newItem = new ItemDetail();
        newItem.itemName = "Unnamed Item";
        newItem.itemId = itemSourceList[itemSourceList.Count - 1].itemId + 1;
        itemSourceList.Add(newItem);
        itemSourceListView.selectedIndex = itemSourceList.Count - 1;
        itemSourceListView.Rebuild();
    }

    private void OnDeleteBtnClicked()
    {
        itemSourceList.Remove(activeItem);
        itemSourceListView.Rebuild();
        SetItemDetailViewVisivle(false);
    }

    private void LoadDataBase()
    {
        string[] guids = AssetDatabase.FindAssets($"t:{nameof(ItemSourceSO)}");
        if(guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            dataBase = AssetDatabase.LoadAssetAtPath<ItemSourceSO>(path);
            itemSourceList = dataBase.itemDetailList;
            EditorUtility.SetDirty(dataBase); //IMPORTANT
        }
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () =>
        {
            return itemRowTemplate.CloneTree();
        };

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemSourceList.Count)
            {
                if (itemSourceList[i].inventoryIcon != null)
                    e.Q<VisualElement>(itemIconString).style.backgroundImage = itemSourceList[i].inventoryIcon ?
                        itemSourceList[i].inventoryIcon.texture:null;
                e.Q<Label>(itemNameString).text = itemSourceList[i].itemName;
                e.Q<Label>(itemIdString).text = itemSourceList[i].itemId.ToString();
            }
        };
        itemSourceListView.fixedItemHeight = 60;
        itemSourceListView.itemsSource = itemSourceList;
        itemSourceListView.makeItem = makeItem;
        itemSourceListView.bindItem = bindItem;
        itemSourceListView.Sort(CompareItemId);
        itemSourceListView.selectionChanged += OnSelectedItemChanged;
    }

    private int CompareItemId(VisualElement x, VisualElement y)
    {
        int x_id = int.Parse(x.Q<Label>(itemIdString).text);
        int y_id = int.Parse(y.Q<Label>(itemIdString).text);
        return x_id - y_id;
    }

    private void OnSelectedItemChanged(IEnumerable<object> select)
    {
        if (select != null && select.Any())
            activeItem = (ItemDetail) select.First();
        GetItemDetail();
        SetItemDetailViewVisivle(true);
    }

    private void GetItemDetail()
    {
        itemDetailView.MarkDirtyRepaint();

        idField.value = activeItem.itemId;
        idField.RegisterValueChangedCallback(evt =>
        { 
            activeItem.itemId = evt.newValue;
            itemSourceListView.Rebuild();
        });

        nameField.value = activeItem.itemName;
        nameField.RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemSourceListView.Rebuild();
        });

        itemTypeField.value = activeItem.itemType;
        itemTypeField.RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });
        
        inventoryIconField.value = activeItem.inventoryIcon;
        inventoryIconField.RegisterValueChangedCallback(evt =>
        {
            activeItem.inventoryIcon = (Sprite)evt.newValue;
            itemSourceListView.Rebuild();
        });

        Sprite tmpSprite = activeItem.onWorldSprite;
        onWorldSpriteField.value = tmpSprite;
        onWorldSpritePreview.style.backgroundImage = tmpSprite ? tmpSprite.texture : null;
        onWorldSpriteField.RegisterValueChangedCallback(evt =>
        {
            Sprite evtSprite = (Sprite)evt.newValue;
            activeItem.onWorldSprite = evtSprite;
            onWorldSpritePreview.style.backgroundImage = evtSprite ? evtSprite.texture : null;
        });

        descriptionField.value = activeItem.description;
        descriptionField.RegisterValueChangedCallback(evt =>
        {
            activeItem.description = evt.newValue;
        });

        useRadiusField.value = activeItem.useRadius;
        useRadiusField.RegisterValueChangedCallback(evt =>
        {
            activeItem.useRadius = evt.newValue;
        });

        maxStackAmountField.value = activeItem.maxStackAmount;
        maxStackAmountField.RegisterValueChangedCallback(evt =>
        {
            activeItem.maxStackAmount = evt.newValue;
        });

        canPickToggle.value = activeItem.canPick;
        canPickToggle.RegisterValueChangedCallback(evt =>
        {
            activeItem.canPick = evt.newValue;
        });

        canDropToggle.value = activeItem.canDrop;
        canDropToggle.RegisterValueChangedCallback(evt =>
        {
            activeItem.canDrop = evt.newValue;
        });

        canCarryToggle.value = activeItem.canCarry;
        canCarryToggle.RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarry = evt.newValue;
        });

        specificPrefabField.value = activeItem.specificPrefab;
        specificPrefabField.RegisterValueChangedCallback(evt =>
        {
            activeItem.specificPrefab = (GameObject)evt.newValue;
        });

        hasSpecificPrefabToggle.value = activeItem.hasSpecificPrefab;
        hasSpecificPrefabToggle.RegisterValueChangedCallback(evt =>
        {
            activeItem.hasSpecificPrefab = evt.newValue;
        });

        priceField.value = activeItem.price;
        priceField.RegisterValueChangedCallback(evt =>
        {
            activeItem.price = evt.newValue;
        });

        sellPercentageSlider.value = activeItem.sellPricePercent;
        sellPercentageSlider.RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPricePercent = evt.newValue;
        });
    }
}
