using Farm.InventoryNamespace;
using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.CropNamespace{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        void Start()
        {

        }

        void Update()
        {

        }

        public void InitCropData(CropDetails cropDetails)
        {
            this.cropDetails = cropDetails;
        }

        public void SpawnHarvestCropItems()
        {
            for (int i = 0; i < cropDetails.productItemids.Length; i++)
            {
                int amountToProduce;
                if (cropDetails.productMinCount[i] == cropDetails.productMaxCount[i])
                {
                    amountToProduce = cropDetails.productMinCount[i];
                }
                else
                {
                    amountToProduce = Random.Range(cropDetails.productMinCount[i], cropDetails.productMaxCount[i]);
                }

                for (int j = 0; j < amountToProduce; j++)
                {
                    if (cropDetails.generateAtPlayerPosition)
                    {
                        WorldItemManager.Instance.GenerateItemOnWorld(GameManager.Instance.playerCharacter.transform.position, InventoryManager.Instance.GetItemDetails(cropDetails.productItemids[i]));
                    }
                    else
                    {
                        WorldItemManager.Instance.GenerateItemOnWorld(transform.position, InventoryManager.Instance.GetItemDetails(cropDetails.productItemids[i]));
                    }
                }
            }
        }
    }
}