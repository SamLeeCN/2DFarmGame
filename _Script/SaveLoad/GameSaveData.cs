using Farm.InventoryNamespace;
using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.SaveLoad{
    [System.Serializable]
    public class GameSaveData
    {
        public string sceneName;
        //TimeManager
        public TimeSaveData timeSaveData;
        //SceneLoadManager
        public Dictionary<string, bool> isSceneFirstLoadDict;
        // GameManager
        public PlayerSaveData playerSaveData;
        // WorldItemManager
        public Dictionary<string, List<WorldItemData>> sceneItemDataDict;
        public Dictionary<string, List<WorldFurnitureData>> sceneFurnitureDataDict;
        // GridMapManager
        public Dictionary<string, TileDetails> tileDetailsDict;
        // InventoryManager
        public Dictionary<int, List<InventoryItem>> chestDict;
        // NpcManager
        public Dictionary<string, NpcData> npcDataDict;
    }

    
}