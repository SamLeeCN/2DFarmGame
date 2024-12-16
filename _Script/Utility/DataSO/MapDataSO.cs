using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace GridMapNamespace
{
    [CreateAssetMenu(menuName = "DataSO/MapDataSO")]
    public class MapDataSO : ScriptableObject
    {
        public GameSceneSO gameScene;

        [Header("Map Information")]
        public int gridWidth;
        public int gridHeight;
        [Header("Left Buttom Point(Origin)")]
        public int originX;
        public int originY;
        public List<TileProperty> tileProperties;
    }

}
