using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[CreateAssetMenu(menuName = "DataSO/BulePrintSourceSO")]
public class BluePrintSourceSO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;
    
}
[System.Serializable]
public class BluePrintDetails
{
    public int id;
    public int amount = 1;
    public InventoryItem[] requireItems = new InventoryItem[9];
    public GameObject buildPrefab;

}