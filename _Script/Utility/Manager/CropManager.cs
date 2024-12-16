using Farm.InventoryNamespace;
using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.CropNamespace{
    public class CropManager : Singleton<CropManager>
    {
        public CropSourceSO cropSourceSO;
        private Transform cropParent;
        private Grid currentGrid;

        private void OnEnable()
        {
            EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
            EventHandler.DisplaySeedEvent += OnDisplaySeedEvent;
            
        }

        private void OnDisable()
        {
            EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
            EventHandler.DisplaySeedEvent -= OnDisplaySeedEvent;
            
        }

        private void OnAfterSceneLoadEvent(bool doTeleport, bool isFirstLoad)
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnDisplaySeedEvent(int itemId, TileDetails tileDetails)
        {
            
            CropDetails cropDetails = GetCropDetails(itemId);
            if (cropDetails != null && SeasonAvailable(cropDetails) && tileDetails.seedItemId == -1) 
            {
                tileDetails.seedItemId = itemId;
                tileDetails.hasGrownDays = 0;
                DisplayCropPlant(tileDetails, cropDetails);
            }
            else if(tileDetails.seedItemId != -1)
            {
                DisplayCropPlant(tileDetails, cropDetails);
            }
        }

        public bool PlantSeed(int itemId, TileDetails tileDetails)
        {
            
            CropDetails cropDetails = GetCropDetails(itemId);
            if (cropDetails != null && SeasonAvailable(cropDetails) && tileDetails.seedItemId == -1)
            {
                tileDetails.seedItemId = itemId;
                tileDetails.hasGrownDays = 0;
                DisplayCropPlant(tileDetails, cropDetails);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void DisplayCropPlant(TileDetails tileDetails, CropDetails cropDetails)
        {
            
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;
            // Calculate which stage the crop is currently in
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.hasGrownDays >= dayCounter)
                {
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }
            // Get the prefab of current stage
            GameObject cropPrefab  = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];
            Vector3 pos = new Vector3(tileDetails.tileCoordinate.x + 0.5f, tileDetails.tileCoordinate.y + 0.5f, 0);
            if (cropParent == null)
            {
                cropParent = GameObject.FindWithTag("CropParent").transform;
            }
            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;
            cropInstance.GetComponent<Crop>().Init(cropDetails, tileDetails);
            
        }

        public CropDetails GetCropDetails(int itemId)
        {
            return cropSourceSO.cropDetailsList.Find(c => c.seedId == itemId);
        }

        public bool SeasonAvailable(CropDetails cropDetails)
        {
            for (int i = 0; i < cropDetails.seasonsToPlant.Length; i++)
            {
                if (cropDetails.seasonsToPlant[i] == TimeManager.Instance.GameSeason)
                {
                    return true;
                }
            }
            return false;
        }
    }
}