using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    public class WorldItemManager : Singleton<WorldItemManager>
    {
        public ItemOnWorld itemOnWorldPrefab;
        public ItemOnWorld bounceItemPrefab;
        [SerializeField] private Transform itemOnWorldParent;
        public Dictionary<string, List<WorldItemData>> sceneItemDataDict = new Dictionary<string, List<WorldItemData>>();
        public Dictionary<string, List<WorldFurnitureData>> sceneFurnitureDataDict = new Dictionary<string, List<WorldFurnitureData>>();
        private void Start()
        {
            itemOnWorldParent = GameObject.FindGameObjectWithTag("ItemOnWorldParent").transform;
        }
        private void OnEnable()
        {
            EventHandler.InstantiateItemOnWorldEvent += OnInstantiateItemOnWorldEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.DropItemEvent += OnDropItemEvent;
        }


        private void OnDisable()
        {
            EventHandler.InstantiateItemOnWorldEvent -= OnInstantiateItemOnWorldEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }

        private void OnBeforeSceneUnloadEvent(GameSceneSO sceneToUnload, bool isLoadData)
        {
            if (isLoadData) return;
            GetSceneItemFromScene(sceneToUnload.sceneName);
            GetSceneFurnitureFromScene(sceneToUnload.sceneName);
        }

        private void OnAfterSceneLoadEvent(bool doTeleport, bool isFirstLoad)
        {
            itemOnWorldParent = GameObject.FindGameObjectWithTag("ItemOnWorldParent").transform;
            RecreateAllItemsOfScene(SceneLoadManager.Instance.currentScene.sceneName, isFirstLoad);
            RecreateAllFurnituresOfScene(SceneLoadManager.Instance.currentScene.sceneName, isFirstLoad);
        }

        private void OnInstantiateItemOnWorldEvent(int itemId, int amount, Vector3 pos)
        {
            ItemOnWorld newItemOnWorld = Instantiate(itemOnWorldPrefab,pos,Quaternion.identity,itemOnWorldParent);
            newItemOnWorld.itemId = itemId;
            newItemOnWorld.amount = amount;
        }

        private void OnDropItemEvent(Vector3 endWorldPos, InventoryDataSO inventoryData, int slotIndex)
        {
            Transform playerTrans = GameManager.Instance.playerCharacter.transform;
            ItemOnWorld newBouceItemOnWorld = Instantiate(bounceItemPrefab, playerTrans.position, Quaternion.identity, itemOnWorldParent);
            ItemDetail itemDetails = inventoryData.items[slotIndex].Deatail;
            int itemId = itemDetails.itemId;
            newBouceItemOnWorld.itemId = itemId;
            newBouceItemOnWorld.amount = 1;
            Vector2 dir = (endWorldPos - playerTrans.position).normalized;
            ItemBounce itemBounce = newBouceItemOnWorld.GetComponent<ItemBounce>();
            itemBounce.InitBoucnceItem(endWorldPos, dir);
        }
        public void GenerateItemOnWorld(Vector3 worldPos, ItemDetail itemDetails)
        {
            ItemOnWorld newItemOnWorld = Instantiate(bounceItemPrefab, worldPos, Quaternion.identity, itemOnWorldParent);
            int itemId = itemDetails.itemId;
            newItemOnWorld.itemId = itemId;
            newItemOnWorld.amount = 1;
        }

        public void GetSceneItemFromScene(string sceneName)
        {
            List<WorldItemData> worldItemDatas = new List<WorldItemData>();
            ItemOnWorld[] worldItemList;
            worldItemList = FindObjectsOfType<ItemOnWorld>();
            foreach(ItemOnWorld worldItem in worldItemList)
            {
                WorldItemData sceneItemData = new WorldItemData()
                {
                    itemId = worldItem.itemId,
                    pos = new SerilizableVector3(worldItem.gameObject.transform.position),
                    hasSpecificPrefab = worldItem.CurrentItemDetail.hasSpecificPrefab,
                };
                
                worldItemDatas.Add(sceneItemData);
            }
            

            if (sceneItemDataDict.ContainsKey(sceneName))
            {
                sceneItemDataDict[sceneName] = worldItemDatas;
            }
            else
            {
                sceneItemDataDict.Add(sceneName, worldItemDatas);
            }

        }

        public void GetSceneFurnitureFromScene(string sceneName)
        {
            List<WorldFurnitureData> worldFurnitureDatas = new List<WorldFurnitureData>();
            Furniture[] worldFurnitureList;
            worldFurnitureList = FindObjectsOfType<Furniture>();
            foreach (Furniture worldFurniture in worldFurnitureList)
            {
                WorldFurnitureData sceneFurnitureData = new WorldFurnitureData()
                {
                    itemId = worldFurniture.itemId,
                    pos = new SerilizableVector3(worldFurniture.gameObject.transform.position),
                };
                Chest chest = worldFurniture.GetComponent<Chest>();
                if (chest != null)
                    sceneFurnitureData.chestIndex = chest.index;
                worldFurnitureDatas.Add(sceneFurnitureData);
            }

            if (sceneFurnitureDataDict.ContainsKey(sceneName))
            {
                sceneFurnitureDataDict[sceneName] = worldFurnitureDatas;
            }
            else
            {
                sceneFurnitureDataDict.Add(sceneName, worldFurnitureDatas);
            }

        }

        private void RecreateAllItemsOfScene(string sceneName, bool isFirstLoad)
        {
            
            List<WorldItemData> worldItemDatas= new List<WorldItemData>();
            ItemOnWorld[] worldItemList;
            if (!isFirstLoad) {
                
                worldItemList = FindObjectsOfType<ItemOnWorld>();
                foreach (ItemOnWorld worldItem in worldItemList)
                {
                    Destroy(worldItem.gameObject);
                }
            }
            
            if (sceneItemDataDict.TryGetValue(sceneName, out worldItemDatas))
            {
                
                if (worldItemDatas != null)
                {
                    foreach (WorldItemData worldItemData in worldItemDatas)
                    {
                        ItemOnWorld newItem;
                        if (!worldItemData.hasSpecificPrefab) 
                        {
                            newItem = Instantiate(itemOnWorldPrefab, worldItemData.pos.ToVector3(), Quaternion.identity, itemOnWorldParent);

                        }
                        else
                        {
                            GameObject newItemGamObject = Instantiate(InventoryManager.Instance.GetItemDetails(worldItemData.itemId).specificPrefab, worldItemData.pos.ToVector3(), Quaternion.identity, itemOnWorldParent);
                            newItem = newItemGamObject.GetComponent<ItemOnWorld>();
                        }
                        if (newItem!= null)
                            newItem.Init(worldItemData.itemId);
                    }
                }
                
            }
        }

        private void RecreateAllFurnituresOfScene(string sceneName, bool isFirstLoad)
        {
            List<WorldFurnitureData> worldFurnitureDatas = new List<WorldFurnitureData>();
            Furniture[] worldFurnitureList;
            if (!isFirstLoad)
            {
                worldFurnitureList = FindObjectsOfType<Furniture>();
                foreach (Furniture worldFurniture in worldFurnitureList)
                {
                    Destroy(worldFurniture.gameObject);
                }
            }

            if (sceneFurnitureDataDict.TryGetValue(sceneName, out worldFurnitureDatas))
            {
                if (worldFurnitureDatas != null)
                {
                    foreach (WorldFurnitureData worldFurnitureData in worldFurnitureDatas)
                    {
                        BluePrintDetails bluePrintDetails = InventoryManager.Instance.GetBluePrintDetails(worldFurnitureData.itemId);
                        GameObject newFurniture = Instantiate(bluePrintDetails.buildPrefab, worldFurnitureData.pos.ToVector3(), Quaternion.identity, itemOnWorldParent);
                        newFurniture.GetComponent<Furniture>().itemId = worldFurnitureData.itemId;
                        
                        if (worldFurnitureData.chestIndex != -1)
                        {
                            Chest chest = newFurniture.GetComponent<Chest>();
                            chest.InitChest(worldFurnitureData.chestIndex);
                        }
                    }
                }

            }
        }

        public void BuildFurniture(InventoryDataSO inventoryData, int slotIndex, Vector3 mouseWorldPos)
        {
            int id = inventoryData.items[slotIndex].itemId;
            BluePrintDetails bluePrint = InventoryManager.Instance.GetBluePrintDetails(id);
            GameObject builtItem = Instantiate(bluePrint.buildPrefab, mouseWorldPos, Quaternion.identity, itemOnWorldParent);
            Chest chest = builtItem.GetComponent<Chest>();
            if (chest!= null)
            {
                chest.InitChest();
            }
            inventoryData.DeclineItemAtIndex(slotIndex, 1);
            EventHandler.CallInventoryDataUpdateEvent(inventoryData, slotIndex);
        }

    }
}