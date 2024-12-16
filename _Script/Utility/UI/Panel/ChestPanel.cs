using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class ChestPanel : UISingleton<ChestPanel>
{
    public Chest currentChest;
    public InventoryContainerUI inventoryContainer => InventoryManager.Instance.chest;


}
