using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: A collection of all item data
//*****************************************
namespace Farm.InventoryNamespace{
    [CreateAssetMenu(menuName = "DataSO/ItemDataList")]
    public class ItemSourceSO : ScriptableObject
    {
        public List<ItemDetail> itemDetailList;
    }
}