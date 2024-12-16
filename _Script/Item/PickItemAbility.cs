using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    [RequireComponent(typeof(Inventory))]
    public class PickItemAbility : MonoBehaviour
    {
        private ItemOnWorld itemToPick;
        private Inventory inventory;
        [SerializeField] private bool isPlayer = false;
        private void Start()
        {
            inventory = GetComponent<Inventory>();
            isPlayer = gameObject.CompareTag("Player");
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            itemToPick = collision.GetComponent<ItemOnWorld>();
            if (itemToPick == null || itemToPick.isFlyingToPicker == true || itemToPick.canPick==false) return;

            if (!isPlayer)
            {
                inventory.PickUpItem(itemToPick);
                
                return;
            }
            //Player
            if (Settings.autoPick)
            {
                inventory.PickUpItem(itemToPick);
            }
            else
            {
                //TODO: Inform player in trigger area
            }
            
            
        }
    }
}