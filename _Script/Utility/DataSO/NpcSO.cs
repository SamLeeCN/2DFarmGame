using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[CreateAssetMenu(fileName = "New NpcSO", menuName = "DataSO/NpcSO")]
public class NpcSO : ScriptableObject
{
    public string npcName;
    public ScheduleDataListSO regularSchedule;
}
