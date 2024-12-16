using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//*****************************************

namespace CropNameSpace{
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedItemId;
        public int hasGrownDay;
        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += OnGenearateCropEvent;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= OnGenearateCropEvent;
        }

        private void OnGenearateCropEvent()
        {
            GenerateCrop();
        }

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        private void GenerateCrop()
        {
            if(currentGrid == null)
            {
                currentGrid = FindObjectOfType<Grid>();
            }
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);

            if (seedItemId > 0)
            {
                TileDetails tileDetails = GridMapManager.Instance.GetTileDetailsOnMouseGridPos(cropGridPos);

                
                
                if (tileDetails==null){
                    tileDetails = new TileDetails();
                    tileDetails.tileCoordinate.x = cropGridPos.x;
                    tileDetails.tileCoordinate.y = cropGridPos.y;
                }
                
                tileDetails.daysSinceWatered = -1;
                tileDetails.seedItemId = seedItemId;
                tileDetails.hasGrownDays = hasGrownDay;
                
                GridMapManager.Instance.UpdataTileDetail(tileDetails);
            }

            
        }

    }
}