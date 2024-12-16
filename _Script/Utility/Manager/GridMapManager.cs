using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Farm.CropNamespace;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace GridMapNamespace
{
    public class GridMapManager : Singleton<GridMapManager>
    {
        public List<MapDataSO> mapDataList;
        public MapDataSO currentMapData;
        public Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();
        public Grid currentGrid;
        private List<ReapItem> reapItemInRadius;
        private Season currentSeason;

        [SerializeField]private RuleTile dugTile;
        [SerializeField]private RuleTile wateredTile;
        public Tilemap digTileMap;
        public Tilemap waterTileMap;
        
        private void OnEnable()
        {
            EventHandler.ExcuteClickWorldActionEvent += OnExcuteClickWorldActionEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent += OnGameDayEvent;
            EventHandler.RefreshMapEvent += OnRefreshMapEvent;
        }

        private void OnDisable()
        {
            EventHandler.ExcuteClickWorldActionEvent -= OnExcuteClickWorldActionEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.GameDayEvent -= OnGameDayEvent;
            EventHandler.RefreshMapEvent -= OnRefreshMapEvent;
        }

        private void OnRefreshMapEvent()
        {
            RefreshMap();
        }

        private void OnGameDayEvent(int day, Season season)
        {
            currentSeason = season;

            foreach (var tile in tileDetailsDict.Values)
            {
                if (tile.daysSinceWatered > -1)
                {
                    tile.daysSinceWatered = -1;
                }
                if (tile.daysSinceDug > -1)
                {
                    tile.daysSinceDug++;
                }
                // Erase the dug hole after 5 days of dug
                if(tile.daysSinceDug > 5 && tile.seedItemId == -1)
                {
                    tile.daysSinceDug = -1;
                    tile.canDig = true;
                    tile.hasGrownDays = -1;
                }
                // Plants grow
                if (tile.seedItemId != -1)
                {
                    tile.hasGrownDays++;
                }

            }
            RefreshMap();
        }

        private void OnAfterSceneLoadEvent(bool doTeleport, bool isFirstLoad)
        {
            currentGrid = FindObjectOfType<Grid>(); 
            digTileMap = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTileMap = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            if (isFirstLoad)
            {
                EventHandler.CallGenerateCropEvent();
                
            }

            RefreshMap();
        }

        void Start()
        {
            // FIXME: Need to be Save, instead of reset every time when player start the game
            foreach (MapDataSO mapData in mapDataList)
            {
                InitTileDetailsDict(mapData);
            }
        }

        public void InitTileDetailsDict(MapDataSO mapDataSO)
        {
            foreach (TileProperty tileProperty in mapDataSO.tileProperties)
            {
                TileDetails tileDetails = new TileDetails()
                {
                    tileCoordinate = tileProperty.tileCoordinate,
                };
                string key = GetTileDetailDictKey(mapDataSO.gameScene.sceneName, tileDetails.tileCoordinate.x, tileDetails.tileCoordinate.y);
                if (tileDetailsDict.ContainsKey(key))
                {
                    tileDetails = tileDetailsDict[key];
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NpcObstacle:
                        tileDetails.isNpcObstacle = tileProperty.boolTypeValue;
                        break;
                }

                if (tileDetailsDict.ContainsKey(key))
                {
                    tileDetails = tileDetailsDict[key];
                }
                else
                {
                    tileDetailsDict.Add(key, tileDetails);
                }
            }
        }

        public static string GetTileDetailDictKey(string sceneName, int tileCoordinateX, int tileCoordinateY)
        {
            return "scene" + sceneName + "__x" + tileCoordinateX + "__y" + tileCoordinateY;
        }

        public TileDetails GetTileDetailsOnMouseGridPos(Vector3Int mouseGridPos)
        {
            string key = GetTileDetailDictKey(SceneLoadManager.Instance.GetCurrentSceneName(), mouseGridPos.x, mouseGridPos.y);
            if (tileDetailsDict.ContainsKey(key))
            {
                return tileDetailsDict[key];
            }
            else
            {
                return null;
            }
        }

        private void OnExcuteClickWorldActionEvent(Vector3 mouseWorldPos, InventoryDataSO inventoryData, int slotIndex)
        {
            Vector3Int mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            TileDetails currentTileDetails = GetTileDetailsOnMouseGridPos(mouseGridPos);
            ItemDetail itemDetail = inventoryData.items[slotIndex].Deatail;
            Crop currrentCrop = GetCropObjectByWorldPos(mouseWorldPos);
            if (currentTileDetails != null)
            {
                switch (itemDetail.itemType)
                {
                    case ItemType.Seed:
                        if (inventoryData.items[slotIndex].amount > 0 && currentTileDetails.seedItemId == -1)
                        {
                            if (CropManager.Instance.PlantSeed(itemDetail.itemId, currentTileDetails))
                            {
                                inventoryData.DeclineItemAtIndex(slotIndex, 1);
                                EventHandler.CallSoundEffectEvent(SoundName.Plant, GameManager.Instance.PlayerTransform.position);
                            }
                        }
                        
                        break;
                    case ItemType.Commodity:
                        if (inventoryData.items[slotIndex].amount > 0)
                        {
                            EventHandler.CallDropItemEvent(mouseWorldPos, GameManager.Instance.playerInventory.bagData, slotIndex);
                        } 
                        break;
                    case ItemType.HoeTool:
                        SetDugGround(currentTileDetails);
                        currentTileDetails.canDig = false;
                        currentTileDetails.daysSinceDug = 0;
                        currentTileDetails.canDropItem = false;
                        EventHandler.CallSoundEffectEvent(SoundName.Hoe, GameManager.Instance.PlayerTransform.position);
                        break;
                    case ItemType.WaterTool:
                        SetWateredGround(currentTileDetails);
                        currentTileDetails.daysSinceWatered = 0;
                        EventHandler.CallSoundEffectEvent(SoundName.Water, GameManager.Instance.PlayerTransform.position);
                        break;

                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        if (currrentCrop != null)
                        {   
                            // Find crop by crop collision box
                            currrentCrop.ProcessToolAction(itemDetail, currrentCrop.tileDetails);
                        }
                        break;
                    case ItemType.CollectTool:
                        if (currrentCrop != null)
                        {
                            // Find crop by tileDetails
                            currrentCrop.ProcessToolAction(itemDetail, currentTileDetails);
                        }
                        break;
                    case ItemType.Furniture:
                        WorldItemManager.Instance.BuildFurniture(inventoryData, slotIndex, mouseWorldPos);
                        break;
                }
                
                UpdataTileDetail(currentTileDetails);
            }
            
            if (itemDetail.itemType == ItemType.ReapTool)
            {
                for (int i = 0; i < reapItemInRadius.Count; i++)
                {
                    reapItemInRadius[i].SpawnHarvestCropItems();
                    Destroy(reapItemInRadius[i].gameObject);
                }
            }
        }

        private void SetDugGround(TileDetails tileDetails)
        {
            Vector3Int tilePos = new Vector3Int(tileDetails.tileCoordinate.x, tileDetails.tileCoordinate.y, 0);
            if (digTileMap != null)
            {
                digTileMap.SetTile(tilePos, dugTile);
            }
        }

        private void SetWateredGround(TileDetails tileDetails)
        {
            Vector3Int tilePos = new Vector3Int(tileDetails.tileCoordinate.x, tileDetails.tileCoordinate.y, 0);
            if (waterTileMap != null)
            {
                waterTileMap.SetTile(tilePos, wateredTile);
            }
        }

        public void UpdataTileDetail(TileDetails tileDetails)
        {
            string key = GetTileDetailDictKey(SceneLoadManager.Instance.GetCurrentSceneName(), tileDetails.tileCoordinate.x, tileDetails.tileCoordinate.y);
            if (tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }

        private void DisplayMap(string sceneName)
        {
            foreach(var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;
                if (key.StartsWith("scene"+sceneName+"__x"))
                {
                    
                    if (tileDetails.daysSinceDug > -1)
                    {
                        SetDugGround(tileDetails);
                    }
                    if (tileDetails.daysSinceWatered > -1)
                    {
                        SetWateredGround(tileDetails);
                    }
                    if(tileDetails.seedItemId > -1)
                    {
                        EventHandler.CallDisplaySeedEvent(tileDetails.seedItemId, tileDetails);
                    }
                }
            }
        }

        private void RefreshMap()
        {
            if (digTileMap != null)
            {
                digTileMap.ClearAllTiles();
            }
            if (waterTileMap != null)
            {
                waterTileMap.ClearAllTiles();
            }

            foreach (Crop crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneLoadManager.Instance.GetCurrentSceneName());
        }

        public Crop GetCropObjectByWorldPos(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                {
                    currentCrop = colliders[i].GetComponent<Crop>();
                }
            }
            return currentCrop;
        }

        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos, ItemDetail toolItemDetails)
        {
            reapItemInRadius = new List<ReapItem>();
            Collider2D[] colls = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, toolItemDetails.useRadius, colls);

            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i] != null)
                    {
                        if (colls[i].GetComponent<ReapItem>())
                        {
                            var item = colls[i].GetComponent<ReapItem>();
                            reapItemInRadius.Add(item);
                        }
                    }
                }
            }
            return reapItemInRadius.Count > 0;
        }

        public bool GetGridDimensions(string sceneName,out Vector2Int gridDimensions, out Vector2Int gridOrigin)
        {
            gridDimensions = Vector2Int.zero;
            gridOrigin = Vector2Int.zero;

            foreach (var mapData in mapDataList)
            {
                if (mapData.gameScene.sceneName == sceneName)
                {
                    gridDimensions.x = mapData.gridWidth;
                    gridDimensions.y = mapData.gridHeight;

                    gridOrigin.x = mapData.originX;
                    gridOrigin.y = mapData.originY;
                    return true;
                }
            }
            return false;
        }
    }
}

