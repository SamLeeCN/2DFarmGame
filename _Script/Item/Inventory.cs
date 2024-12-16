using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    public class Inventory : MonoBehaviour
    {
        
        [SerializeField]private InventoryType inventoryType = InventoryType.Single;
        public InventoryDataSO bagData;

        private void Awake()
        {
            if (gameObject.CompareTag("Player")) inventoryType = InventoryType.Player;
            //FIXME:Debug
            
            if (gameObject.CompareTag("Player")) 
            {
                bagData = Instantiate(InventoryManager.Instance.bagDataTemplate);
                InventoryManager.Instance.playerInventoryData = bagData; 
            }
        }
        private void Start()
        {
            
        }
        private void OnEnable()
        {
            EventHandler.DropItemEvent += OnDropItemEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent -= OnDropItemEvent;
        }

        
        public void PickUpItem(ItemOnWorld itemToPick)
        {
            int initAmount = itemToPick.amount;
            int leftAmountOnWorld = initAmount;
            switch (inventoryType)
            {
                case InventoryType.Single:
                    leftAmountOnWorld = bagData.AddItem(itemToPick.itemId, leftAmountOnWorld);
                    itemToPick.BePickedUp(leftAmountOnWorld, transform);
                    break;
                case InventoryType.Player:
                    //FIXME:Debug
                    leftAmountOnWorld = bagData.AddItem(itemToPick.itemId, leftAmountOnWorld);
                    itemToPick.BePickedUp(leftAmountOnWorld, transform);
                    break;
            }            
        }

        private void OnDropItemEvent(Vector3 endWorldPos, InventoryDataSO inventoryData, int slotIndex)
        {
            if (inventoryData != bagData) return;
            //TODO:Dropped item bouncing to endWorldPos


            inventoryData.DeclineItemAtIndex(slotIndex, 1);
            EventHandler.CallInventoryDataUpdateEvent(inventoryData, slotIndex);
        }



        /*private void DropItem(int amount)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint
                    (new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            EventHandler.CallInstantiateItemOnWorldEvent(CurrentItem.itemId, amount, pos);
            EventHandler.CallInventoryDataUpdateEvent(inventoryData);
            inventoryData.DeclineItemAtIndex(slotIndex, amount);
        }*/
    }
}
