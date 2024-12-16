using GridMapNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace
{
    public class InventoryManager : Singleton<InventoryManager>
    {
        public InventoryContainerUI actionBar;
        public InventoryContainerUI bag;
        public InventoryContainerUI shop;
        public InventoryContainerUI craftTable;
        public InventoryContainerUI chest;
        public InventoryDataSO playerInventoryData;
        public InventoryDataSO shopInventoryData;
        public InventoryDataSO craftTableInventoryData;
        public ItemToolTip itemToolTip;

        public ItemCursor itemCursor;

        public ItemSourceSO itemSource;
        public BluePrintSourceSO bluePrintSource;

        public InventoryDataSO bagDataTemplate;
        public InventoryDataSO craftTableDataTemplate;

        private InventoryItem currentSelectedItem;
        public InventoryItem CurrentSelectedItem { get { return currentSelectedItem; } }

        public Dictionary<int, List<InventoryItem>> chestDict = new Dictionary<int, List<InventoryItem>>();

        
        public ItemDetail GetItemDetails(int id)
        {
            return itemSource.itemDetailList.Find(i=>i.itemId==id);
        }

        public BluePrintDetails GetBluePrintDetails(int itemId)
        {
            return bluePrintSource.bluePrintDataList.Find(b => b.id == itemId);
        }

        private void OnEnable()
        {
            EventHandler.ActionBarItemSelectedEvent += OnActionBarItemSelectedEvent;
        }
        private void OnDisable()
        {
            EventHandler.ActionBarItemSelectedEvent += OnActionBarItemSelectedEvent;
        }

        private void OnActionBarItemSelectedEvent(InventoryDataSO inventoryData, int index, bool isSelected)
        {
            if (isSelected)
                currentSelectedItem = inventoryData.items[index];
            else
                currentSelectedItem = null;
        }

        private void Start()
        {
            craftTableInventoryData = Instantiate(craftTableDataTemplate);
            actionBar.SetUpUI(playerInventoryData, 0);
            bag.SetUpUI(playerInventoryData, 10);
            craftTable.SetUpUI(craftTableInventoryData, 0);
        }

        

        public void ItemDraggedToSlot(SlotUI currentSlot, SlotUI targetSlot)
        {
            if (currentSlot.slotType == SlotType.Display || targetSlot.slotType == SlotType.Display) return;
            if (currentSlot.slotType == SlotType.Shop) 
            { 
                
            }
            else if (targetSlot.slotType == SlotType.Shop)
            {

            }
            else
            {
                InventoryDataSO currentInv = currentSlot.inventoryData;
                int currentIndex = currentSlot.slotIndex;
                InventoryDataSO targetInv = targetSlot.inventoryData;
                int targetIndex = targetSlot.slotIndex;
                SwapInventoryItem(currentInv, currentIndex, targetInv, targetIndex);

            }

        }

        private void SwapInventoryItem(InventoryDataSO currentInv,int currentIndex,InventoryDataSO targetInv,int targetIndex)
        {
            InventoryItem tmp = currentInv.items[currentIndex];
            currentInv.items[currentIndex] = targetInv.items[targetIndex];
            targetInv.items[targetIndex] = tmp;

            EventHandler.CallInventoryDataUpdateEvent(currentInv, currentIndex);
            EventHandler.CallInventoryDataUpdateEvent(targetInv, targetIndex);
        }


        public List<InventoryItem> GetChest(int chestIndex)
        {
            if (chestDict.ContainsKey(chestIndex)) return chestDict[chestIndex];
            else return null;
        }

        public void AddChestData(Chest chest)
        {
            int chestIndex = chest.index;
            if (!chestDict.ContainsKey(chestIndex)) 
                chestDict.Add(chestIndex, chest.chestData.items);
        }

        public void ModifyChestData(Chest chest)
        {
            int chestIndex = chest.index;
            if (chestDict.ContainsKey(chestIndex))
                chestDict[chestIndex] = chest.chestData.items;
        }

        public void DeleteChestData(Chest chest)
        {
            int chestIndex = chest.index;
            if (chestDict.ContainsKey(chestIndex))
                chestDict.Remove(chestIndex);
        }

        public int GenerateAvailableIndex()
        {
            int index = 0;
            while (chestDict.ContainsKey(index))
            {
                index++;
            }
            return index;
        }

        #region panel setup
        public void ShopPanelSetUp()
        {
            shop.fastShiftInventory = bag;
            shop.secondFastShiftInventory = actionBar;
            
            bag.fastShiftInventory = shop;
            bag.secondFastShiftInventory = actionBar;
            actionBar.fastShiftInventory = bag;
            actionBar.secondFastShiftInventory = null;
        }

        public void BagPanelSetUp()
        {
            craftTable.fastShiftInventory = bag;
            craftTable.secondFastShiftInventory = actionBar;
            
            bag.fastShiftInventory = actionBar;
            bag.secondFastShiftInventory = null;
            actionBar.fastShiftInventory = bag;
            actionBar.secondFastShiftInventory = null;
        }

        public void ChestPanelSetUp()
        {
            chest.fastShiftInventory = bag;
            chest.secondFastShiftInventory = actionBar;

            bag.fastShiftInventory = chest;
            bag.secondFastShiftInventory = actionBar;
            actionBar.fastShiftInventory = bag;
            actionBar.secondFastShiftInventory = null;
        }

        #endregion

    }
}

