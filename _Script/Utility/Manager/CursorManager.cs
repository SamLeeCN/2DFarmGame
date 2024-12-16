using Farm.CropNamespace;
using Farm.InventoryNamespace;
using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class CursorManager : Singleton<CursorManager> 
{
    [SerializeField]Sprite normalSprite, toolSprite, commoditySprite, seedSprite, invalidSprite;
    Sprite currentSprite;
    [SerializeField]private Image cursorImage;
    [SerializeField]private RectTransform cursorCanvas;
    [SerializeField] private Image buildCursorImage;
    

    private InventoryDataSO selectedInventoryData;
    private int selectedInventorySlotIndex;
    private ItemDetail selectedItemDetail;
    private Camera mainCam;
    private Grid currentGrid;
    Vector3 mouseWorldPos;
    Vector3Int mouseGridPos;
    private IntelligentBool cursorEnable = new IntelligentBool(true);
    private IntelligentBool isCursorValid = new IntelligentBool(true);
    
    public Camera uiCamera; // Assign the camera used for the UI, typically the main camera
    public EventSystem eventSystem; // Assign the EventSystem in the scene
    public GraphicRaycaster UiGraphicRaycaster; // Assign the Canvas's GraphicRaycaster

    public List<RaycastResult> uiRayCastResults = new List<RaycastResult>();
    public bool isUiHit;

    void Start()
    {

        buildCursorImage = cursorCanvas.Find("BuildCursor").GetComponent<Image>();
        if (buildCursorImage == null)
        {
            Debug.LogError("BuildCursor not found!!!");
        }
        buildCursorImage.gameObject.SetActive(false);
        buildCursorImage.rectTransform.localScale = new Vector3(Settings.uiScaleToWorldFactor, Settings.uiScaleToWorldFactor, 1);

        currentSprite = normalSprite;
        mainCam = Camera.main;
        uiCamera = Camera.main;
        eventSystem = EventSystem.current;
        UiGraphicRaycaster = UIManager.Instance.mainCanvas.transform.GetComponent<GraphicRaycaster>();
    }

    void Update()
    {
        if (cursorImage == null || !cursorEnable.GetValue) return;
        cursorImage.transform.position = Input.mousePosition;
        if (!isUiHit)
        {
            CheckCursorValid();
            MoveBuildCursor();
            if (isCursorValid.GetValue)
            {
                SetCursorSprite(currentSprite);
                cursorImage.color = new Color(1, 1, 1, 1);
                buildCursorImage.color = new Color(1, 1, 1, 0.5f);
                CheckPlayerClickedWorld();
            }
            else
            {
                SetCursorSprite(invalidSprite);
                cursorImage.color = new Color(1, 0, 0, 0.5f);
                buildCursorImage.color = new Color(1, 0, 0, 0.5f);
            }
        }
        else
        {
            buildCursorImage.enabled = false;
        }

        isUiHit = RaycastHitUI();
    }

    private void OnEnable()
    {
        EventHandler.ActionBarItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
    }

    private void OnDisable()
    {
        EventHandler.ActionBarItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }


    private void OnBeforeSceneUnloadEvent(GameSceneSO sceneToUnload, bool isLoadData)
    {
        cursorEnable.SetValue(false, "SceneLoad");
    }

    private void OnAfterSceneLoadEvent(bool doTeleport, bool isFirstLoad)
    {
        currentGrid = FindObjectOfType<Grid>();
        cursorEnable.SetValue(true, "SceneLoad");
    }

    private void OnItemSelectedEvent(InventoryDataSO inventoryDataSO, int index, bool isSelected)
    {
        ItemDetail itemDetail = inventoryDataSO.items[index].Deatail;
        if (isSelected && itemDetail!=null)
        {
            currentSprite = itemDetail.itemType switch
            { 
                ItemType.Seed => seedSprite,
                ItemType.Commodity=>commoditySprite,
                ItemType.ChopTool=>toolSprite,
                ItemType.WaterTool=>toolSprite,
                ItemType.ReapTool=>toolSprite,
                ItemType.HoeTool=>toolSprite,
                ItemType.CollectTool=>toolSprite,
                ItemType.Furniture => toolSprite,
                _ =>normalSprite
            };
            selectedItemDetail = itemDetail;
            selectedInventoryData = inventoryDataSO;
            selectedInventorySlotIndex = index;

            if (itemDetail.itemType == ItemType.Furniture)
            {
                buildCursorImage.gameObject.SetActive(true);
                buildCursorImage.sprite = itemDetail.onWorldSprite;
                buildCursorImage.SetNativeSize();
            }
            else
            {
                buildCursorImage.gameObject.SetActive(false);
            }
        }
        else
        {
            currentSprite = normalSprite;
            selectedItemDetail = null;
            selectedInventoryData = null;
            selectedInventorySlotIndex = -1;
            buildCursorImage.gameObject.SetActive(false);
        }

        
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
        
        if (currentGrid == null) return;
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMouseGridPos(mouseGridPos);
        Crop currentCrop = GridMapManager.Instance.GetCropObjectByWorldPos(mouseWorldPos);


        if (currentTile != null)
        {
            if (selectedItemDetail != null)
            {
                CropDetails currentCropDetails = CropManager.Instance.GetCropDetails(currentTile.seedItemId);
                switch (selectedItemDetail.itemType)
                {
                    case ItemType.Seed:
                        if(currentTile.daysSinceDug  > -1 && currentTile.seedItemId == -1) isCursorValid.SetValue(true, "SelectedItem");
                        else isCursorValid.SetValue(false, "SelectedItem");
                        break;
                    case ItemType.Commodity:
                        if (!currentTile.canDropItem) isCursorValid.SetValue(false, "SelectedItem");
                        else isCursorValid.SetValue(true, "SelectedItem");
                        break;
                    case ItemType.HoeTool:
                        if (!currentTile.canDig) isCursorValid.SetValue(false, "SelectedItem");
                        else isCursorValid.SetValue(true, "SelectedItem");
                        break;
                    case ItemType.WaterTool:
                        if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1) isCursorValid.SetValue(true, "SelectedItem");
                        else isCursorValid.SetValue(false, "SelectedItem");
                        break;
                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        if (currentCrop != null && currentCrop.CanBeHarvested && currentCrop.cropDetails.CheckToolAvailable(selectedItemDetail.itemId))
                            isCursorValid.SetValue(true, "SelectedItem");
                        else
                            isCursorValid.SetValue(false, "SelectedItem");
                        break;

                    case ItemType.CollectTool:
                        if (currentCropDetails != null && currentCropDetails.CheckToolAvailable(selectedItemDetail.itemId))
                        {
                            if (currentTile.hasGrownDays >= currentCropDetails.TotalGrowthDays) isCursorValid.SetValue(true, "SelectedItem");
                            else isCursorValid.SetValue(false, "SelectedItem");
                        }
                        else
                        {
                            isCursorValid.SetValue(false, "SelectedItem");
                        }
                        break;
                    case ItemType.ReapTool:
                        if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, selectedItemDetail))
                            isCursorValid.SetValue(true, "SelectedItem");
                        else
                            isCursorValid.SetValue(false, "SelectedItem");
                        break;
                    case ItemType.Furniture:
                        if (currentTile.canPlaceFurniture)
                            isCursorValid.SetValue(true, "SelectedItem");
                        else
                            isCursorValid.SetValue(false, "SelectedItem");
                        break;
                }
            }
            else
            {
                isCursorValid.SetValue(true, "SelectedItem");
            }
        }
        else
        {
            if (selectedItemDetail != null)
            {
                if (selectedItemDetail.itemType == ItemType.ReapTool && GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, selectedItemDetail))
                {
                    isCursorValid.SetValue(true, "SelectedItem");
                }
                else
                {
                    isCursorValid.SetValue(false, "SelectedItem");
                }
                
            }
            else
            {
                isCursorValid.SetValue(true, "SelectedItem");
            }
        }
    }
    
    private void SetCursorSprite(Sprite sprite)
    {
        cursorImage.sprite = sprite;
    }

    
    
    private void CheckPlayerClickedWorld()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, selectedInventoryData, selectedInventorySlotIndex);
        }
    }

    private void MoveBuildCursor()
    {
        buildCursorImage.enabled = true;
        buildCursorImage.rectTransform.position = Input.mousePosition;
    }

    private bool RaycastHitUI()
    {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        uiRayCastResults.Clear();
        UiGraphicRaycaster.Raycast(pointerData, uiRayCastResults);
        
        if (uiRayCastResults.Count > 0)
        {
            return true;
        }

        return false;
    }

}
