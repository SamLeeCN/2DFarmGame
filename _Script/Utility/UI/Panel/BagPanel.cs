using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class BagPanel : UISingleton<BagPanel> 
{
    

    [SerializeField] private int xOffsetMultiInventory;
    public void PosToMultiInventoryMode()
    {
        rectTransform.localPosition = new Vector3(xOffsetMultiInventory, 0, 0);
    }

    public void PosToCenter()
    {
        rectTransform.localPosition = new Vector3(0, 0, 0);
    }
}
