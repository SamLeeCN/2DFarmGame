using Farm.CropNamespace;
using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class Character : MonoBehaviour
{
    [Header("Properties")]
    public float basicWalkingSpeed = 10;
    public float basicRunningSpeed = 10;
    public float currentWalkingSpeed;
    public float currentRunningSpeed;
    void Start()
    {
        //FIXME: replace with new codes
        currentWalkingSpeed = basicWalkingSpeed;
        currentRunningSpeed = basicRunningSpeed;
    }

    void Update()
    {

    } 


    
}
