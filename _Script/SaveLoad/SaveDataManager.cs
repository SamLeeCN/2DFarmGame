using Farm.InventoryNamespace;
using Farm.SaveLoad;
using GridMapNamespace;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.SaveLoad{
    public class SaveDataManager : Singleton<SaveDataManager>
    {
        public string jsonFolder ;
        public Dictionary<int, DataSlotDataDetails> dataSlotDataDict = new Dictionary<int, DataSlotDataDetails>();
        public int CurrentMaxSlotIndex { 
            get { 
                if (dataSlotDataDict.Count == 0) return 0;
                else return dataSlotDataDict.Keys.Max();
            }
        }

        private void Awake()
        {
            jsonFolder = $"{Application.persistentDataPath}/SaveData/";
            RestoreDataSlotDataDict();
        }

        public void Save(int index = 0, string saveDataName = "QuickSave")
        {

            SaveData("Save" + index);
            DataSlotDataDetails currentDataSlotData = new DataSlotDataDetails();
            currentDataSlotData.dataName = saveDataName;
            currentDataSlotData.sceneName = SceneLoadManager.Instance.currentScene.sceneName;
            currentDataSlotData.gameSecond = TimeManager.Instance.gameSecond;
            currentDataSlotData.gameMinute = TimeManager.Instance.gameMinute;
            currentDataSlotData.gameHour = TimeManager.Instance.gameHour;
            currentDataSlotData.gameDay = TimeManager.Instance.gameDay;
            currentDataSlotData.gameMonth = TimeManager.Instance.gameMonth;
            currentDataSlotData.gameYear = TimeManager.Instance.gameYear;

            if (!dataSlotDataDict.ContainsKey(index))
                dataSlotDataDict.Add(index, currentDataSlotData);
            else
                dataSlotDataDict[index] = currentDataSlotData;

            SaveDataSlotDataDict();

            EventHandler.CallAfterSaveDataModifiedEvent();
        }

        public void Load(int index = 0)
        {
            LoadData("Save" + index);
        }

        public void Delete(int index = 0)
        {
            if (index == 0) return;
            DeleteData("Save" + index);
            if (dataSlotDataDict.ContainsKey(index))
                dataSlotDataDict.Remove(index);
            EventHandler.CallAfterSaveDataModifiedEvent();
        }
        
        private void SaveDataSlotDataDict()
        {
            var resultPath = jsonFolder + "DataSlotDataDict" + ".json";
            var jsonData = JsonConvert.SerializeObject(dataSlotDataDict, Formatting.Indented);
            if (!File.Exists(resultPath))
                Directory.CreateDirectory(jsonFolder);
            File.WriteAllText(resultPath, jsonData);
        }

        private void RestoreDataSlotDataDict()
        {
            var resultPath = jsonFolder + "DataSlotDataDict" + ".json";
            if (!File.Exists(resultPath)) return;
            var stringData = File.ReadAllText(resultPath);
            dataSlotDataDict = JsonConvert.DeserializeObject<Dictionary<int, DataSlotDataDetails>>(stringData);
        }

        private void SaveData(string saveName)
        {
            GameSaveData gameSaveData = new GameSaveData();
            string currentSceneName = SceneLoadManager.Instance.currentScene.sceneName;
            gameSaveData.sceneName = currentSceneName;

            gameSaveData.timeSaveData = TimeManager.Instance.GetTimeSaveData();

            gameSaveData.isSceneFirstLoadDict= SceneLoadManager.Instance.isSceneFirstLoadDict;

            gameSaveData.playerSaveData = GameManager.Instance.GetPlayerSaveData();

            WorldItemManager.Instance.GetSceneItemFromScene(currentSceneName);
            gameSaveData.sceneItemDataDict = WorldItemManager.Instance.sceneItemDataDict;
            WorldItemManager.Instance.GetSceneFurnitureFromScene(currentSceneName);
            gameSaveData.sceneFurnitureDataDict = WorldItemManager.Instance.sceneFurnitureDataDict;

            gameSaveData.tileDetailsDict = GridMapManager.Instance.tileDetailsDict;

            gameSaveData.chestDict = InventoryManager.Instance.chestDict;

            NPCManager.Instance.UpdateNpcDataDict();
            gameSaveData.npcDataDict = NPCManager.Instance.npcDataDict;

            var resultPath = jsonFolder + saveName + ".json";
            var jsonData = JsonConvert.SerializeObject(gameSaveData, Formatting.Indented);
            if (!File.Exists(resultPath))
                Directory.CreateDirectory(jsonFolder);
            File.WriteAllText(resultPath, jsonData);
        }

        private void LoadData(string saveName)
        {
            GameSaveData gameSaveData;
            var resultPath = jsonFolder + saveName + ".json";
            var stringData = File.ReadAllText(resultPath);
            gameSaveData = JsonConvert.DeserializeObject<GameSaveData>(stringData);

            GameManager.Instance.RestorePlayerSaveData(gameSaveData.playerSaveData);
            TimeManager.Instance.RestoreTimeSaveData(gameSaveData.timeSaveData);
            NPCManager.Instance.npcDataDict = gameSaveData.npcDataDict;
            NPCManager.Instance.RestoreNpcDataFromDict();
            WorldItemManager.Instance.sceneItemDataDict = gameSaveData.sceneItemDataDict;
            WorldItemManager.Instance.sceneFurnitureDataDict = gameSaveData.sceneFurnitureDataDict;
            InventoryManager.Instance.chestDict = gameSaveData.chestDict;
            GridMapManager.Instance.tileDetailsDict = gameSaveData.tileDetailsDict;
            SceneLoadManager.Instance.isSceneFirstLoadDict = gameSaveData.isSceneFirstLoadDict;
            
            EventHandler.CallSceneLoadEvent(SceneLoadManager.Instance.GetSceneSOBySceneName(gameSaveData.sceneName), false, false, true);
        }

        private void DeleteData(string saveName)
        {
            var resultPath = jsonFolder + saveName + ".json";
            if (File.Exists(resultPath))
            {
                File.Delete(resultPath);
            }
        }
        
    }

    
}