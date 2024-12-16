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
    public class Crop : MonoBehaviour
    {
        public CropDetails cropDetails;
        public TileDetails tileDetails;
        private int harvestActionCount = 0;
        private Animator animator;

        public bool CanBeHarvested => tileDetails.hasGrownDays >= cropDetails.TotalGrowthDays;

        public void Init(CropDetails cropDetails, TileDetails tileDetails)
        {
            this.cropDetails = cropDetails;
            this.tileDetails = tileDetails;
        }

        public void ProcessToolAction(ItemDetail toolItemDetails, TileDetails tileDetails)
        {
            animator = GetComponentInChildren<Animator>();
            this.tileDetails = tileDetails;
            int requireActionAmount = cropDetails.GetTotalRequireCount(toolItemDetails.itemId);
            if (requireActionAmount == -1) return;

            // Click Counter
            if (harvestActionCount < requireActionAmount)
            {
                harvestActionCount++;

                //Sound Effect
                if (cropDetails.soundEffectName!= SoundName.None)
                {
                    EventHandler.CallSoundEffectEvent(cropDetails.soundEffectName, transform.position);
                }


                // Particle Effect
                if (cropDetails.hasParticleEffect)
                {
                    EventHandler.CallParticleEffectEvent(cropDetails.particleEffectType, transform.position + cropDetails.particleEffectPosOffset);
                }

                // Perform Animation
                if (cropDetails.hasAnimation && animator != null)
                {
                    float playerX = GameManager.Instance.playerCharacter.transform.position.x;
                    if (transform.position.x < playerX)
                    {
                        animator.SetTrigger("RotateLeft");
                    }
                    else
                    {
                        animator.SetTrigger("RotateRight");
                    }
                }
            }

            if (harvestActionCount >= requireActionAmount)
            {
                // Perform Animation
                if (cropDetails.hasAnimation && animator != null)
                {
                    float playerX = GameManager.Instance.playerCharacter.transform.position.x;
                    if (transform.position.x < playerX)
                    {
                        animator.SetTrigger("FallLeft");
                    }
                    else
                    {
                        animator.SetTrigger("FallRight");
                    }
                    StartCoroutine(HarvestAfterAnimaton());
                }
                else
                {
                    SpawnHarvestCropItems();
                }
            }
        }

        private IEnumerator HarvestAfterAnimaton()
        {
            
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("END"))
            {
                yield return null;
            }

            SpawnHarvestCropItems();

            // The following code would continue to execute even though the gameobject has been destroyed!
            if (cropDetails.transferCropId > 0)
            {
                CreateTransferCrop();
            }

            
        }

        private void SpawnHarvestCropItems()
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

                if (tileDetails != null)
                {
                    tileDetails.hasRegrowTimes++;
                    // be able to regrow
                    if (cropDetails.timesToRegrow > 0 && tileDetails.hasRegrowTimes < cropDetails.timesToRegrow)
                    {
                        tileDetails.hasGrownDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
                    }
                    else// not able to regrow
                    {
                        // restore tile property
                        tileDetails.hasRegrowTimes = -1;
                        tileDetails.hasGrownDays = -1;
                        tileDetails.seedItemId = -1;
                    }
                    Destroy(gameObject);
                    EventHandler.CallRefreshMapEvent();
                }
            }
        }

        private void CreateTransferCrop()
        {
            tileDetails.seedItemId = cropDetails.transferCropId;
            tileDetails.hasGrownDays = 0;
            EventHandler.CallRefreshMapEvent();
        }
    }
}