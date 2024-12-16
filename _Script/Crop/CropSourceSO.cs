using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: A collection of all crop data
//*****************************************
namespace Farm.CropNamespace{
    [CreateAssetMenu(menuName = "DataSO/CropDataList")]
    public class CropSourceSO : ScriptableObject
    {
        public bool GenerateCropNameBtn = false;
        public List<CropDetails> cropDetailsList;

#if UNITY_EDITOR

        private void OnValidate()
        {
            GenerateCropName();
        }

        private void GenerateCropName()
        {
            if (!GenerateCropNameBtn) return;
            foreach (CropDetails cropDetails in cropDetailsList)
            {
                cropDetails.cropName = InventoryManager.Instance.GetItemDetails(cropDetails.seedId).itemName;
            }
            GenerateCropNameBtn = false;
        }
#endif
    }
}