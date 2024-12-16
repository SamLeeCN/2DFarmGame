using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.CropNamespace{
    [System.Serializable]
    public class CropDetails
    {
        public string cropName;
        public int seedId;

        [Header("Growth Days for Different Period")]
        public int[] growthDays;

        public int TotalGrowthDays
        {
            get
            {
                int amount = 0;
                foreach (int days in growthDays)
                {
                    amount += days;
                }
                return amount;
            }
        }

        [Header("Prefab for Different Period")]
        public GameObject[] growthPrefabs;

        [Header("Sprite for Different Period")]
        public Sprite[] growthSprites;

        [Header("Seasons to Plant")]
        public Season[] seasonsToPlant;

        [Space]
        [Header("Harvest Toll Id")]
        public int[] harvestToolIds;
        public int transferCropId;

        [Header("Harvest Requiring Tool Using Times")]
        public int[] requireToolUsingTimes;

        [Space]
        [Header("Fruit Information")]
        public int[] productItemids;
        public int[] productMinCount;
        public int[] productMaxCount;
        [SerializeField]
        public float spawnRadius;

        [Header("Regrow")]
        public int daysToRegrow;
        public int timesToRegrow;

        [Header("Option")]
        public bool generateAtPlayerPosition;
        public bool hasAnimation;
        public bool hasParticleEffect;

        [Header("Particle Effect")]
        public ParticleEffectType particleEffectType;
        public Vector3 particleEffectPosOffset;

        [Header("Sound Effect")]
        public SoundName soundEffectName = SoundName.Reap;

        public bool CheckToolAvailable(int toolId)
        {
            foreach (int availableId in harvestToolIds)
            {
                if (availableId == toolId)
                    return true;
            }
            return false;
        }

        public int GetTotalRequireCount(int toolId)
        {
            for(int i  = 0; i < harvestToolIds.Length; i++)
            {
                if (harvestToolIds[i] == toolId)
                {
                    return requireToolUsingTimes[i];
                }
            }
            return -1;
        }
    }
}