using Farm.InventoryNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
//*****************************************
//Creator: SamLee 
//Description: 
//*****************************************
namespace Farm.InventoryNamespace{
    
    public class InventoryContainerUI : MonoBehaviour
    {
        [Header("Page Flip")]
        [SerializeField] private int indexRangeLow = -1;
        [SerializeField] private int indexRangeHigh = -1;
        [SerializeField] private InventoryPageFliper pageFliper;
        [Header("Settings")]
        [SerializeField] private InventoryDataSO inventoryData;
        [SerializeField] private int startIndex;
        [SerializeField] SlotType slotType;
        public List<SlotUI> slots;

        public NPCFunction currentOpenNpcShop;

        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private Image headerImage;
        [SerializeField] private ItemCursor DragItemReference => InventoryManager.Instance.itemCursor;

        public InventoryContainerUI fastShiftInventory;
        public InventoryContainerUI secondFastShiftInventory;

        private void Start()
        {
            SetUpPageFliper();
        }
        private void SetUpPageFliper()
        {
            if (pageFliper != null&&inventoryData!=null)
            {
                pageFliper.SetUpUI(this, inventoryData, startIndex, indexRangeLow, indexRangeHigh);
            }
        }

        private void OnEnable()
        {
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.TradeEvent += OnTradeEvent;
            if(inventoryData!=null) UpdateUI();
            RefreshCoinUI();
        }

        private void OnDisable()
        {
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.TradeEvent += OnTradeEvent;
        }

        private void OnBeforeSceneUnloadEvent(GameSceneSO sO, bool isLoadData)
        {
            if (!isLoadData) return;
            UpdateUI();
            RefreshCoinUI();
        }

        private void OnTradeEvent()
        {
            RefreshCoinUI();
        }

        private void RefreshCoinUI()
        {
            switch (slotType)
            {
                case SlotType.PlayerBag:
                    coinText.text = GameManager.Instance.playerControler.coins.ToString();
                    break;
                case SlotType.Shop:
                    if (currentOpenNpcShop != null)
                        coinText.text = currentOpenNpcShop.coins.ToString();
                    break;
                default:

                    break;
            }
        }

        

        public void SetUpUI(InventoryDataSO inventoryData, int startIndex = 0)
        {
            this.inventoryData = inventoryData;
            this.startIndex = startIndex;
            if (slotType == SlotType.Shop)
                indexRangeHigh = inventoryData.items.Count - 1;
            for (int i = 0; i < slots.Count; i++)
            {
                if (i + startIndex >= inventoryData.items.Count)
                {
                    slots[i].gameObject.SetActive(false);
                }
                else
                {
                    slots[i].gameObject.SetActive(true);
                    slots[i].SetBasicInfo(slotType,this,inventoryData,i+startIndex);
                    slots[i].UpdateUI();
                }
            }
            SetUpPageFliper();
            RefreshCoinUI();
        }

        public void UpdateUI()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].UpdateUI();
            }
        }

        public void SelectSlot(int index)
        {
            if (slotType != SlotType.PlayerActionBar) return;
            foreach (var slot in slots)
            {
                if (!slot.gameObject.activeInHierarchy) continue;
                if (slot.slotIndex == index)
                {
                    slot.IsSelected = !slot.IsSelected;
                    EventHandler.CallActionBarItemSelectedEvent(inventoryData, index, slot.IsSelected);
                }
                else
                {
                    slot.IsSelected = false;
                }
            }
        }

        public bool FastShiftToHere(SlotUI slot, int amount)
        {
            ItemDetail slotItemDetail;
            if (slot.CurrentItem.itemId != 0) slotItemDetail = slot.CurrentItem.Deatail;
            else slotItemDetail = null;

            if (slot.slotType == SlotType.Craft && slot.slotIndex == Settings.CraftSlotNum)
            {
                int simulationLeftAmount = inventoryData.AddItemSimulation(slot.CurrentItem.itemId, amount, true, indexRangeLow, indexRangeHigh);
                
                if (simulationLeftAmount!= 0)
                    return false;
            }

            if (slot.slotType == SlotType.Shop && slotType != SlotType.Shop)
            {//Buy
                if (amount * slotItemDetail.price > GameManager.Instance.playerControler.coins)
                    amount = GameManager.Instance.playerControler.coins / slotItemDetail.price;
            }
            if (slot.slotType == SlotType.PlayerBag && slotType == SlotType.Shop)
            {//Sell
                if (amount * slotItemDetail.SellPrice > currentOpenNpcShop.coins)
                    amount = currentOpenNpcShop.coins / slotItemDetail.SellPrice;
            }
            
            int leftAmountToAdd = inventoryData.AddItem(slot.CurrentItem.itemId, amount, true, indexRangeLow, indexRangeHigh);

            if (slot.slotType == SlotType.Shop && slotType != SlotType.Shop)
            {//Buy
                GameManager.Instance.playerControler.coins -= slotItemDetail.price * (amount - leftAmountToAdd);
                slot.inventoryUI.currentOpenNpcShop.coins += slotItemDetail.price * (amount - leftAmountToAdd);
                EventHandler.CallTradeEvent();
            }
            if (slot.slotType == SlotType.PlayerBag && slotType == SlotType.Shop)
            {//Sell
                GameManager.Instance.playerControler.coins += slotItemDetail.SellPrice * (amount - leftAmountToAdd);
                currentOpenNpcShop.coins -= slotItemDetail.SellPrice * (amount - leftAmountToAdd);
                EventHandler.CallTradeEvent();
            }
            slot.CurrentItem.amount -= amount - leftAmountToAdd;
            if (slot.slotType == SlotType.Craft && slot.slotIndex == Settings.CraftSlotNum) CraftPanel.Instance.ClearCraftTable();
            slot.CurrentItem.CheckItemEmpty();
            slot.UpdateUI();
            if (slot.slotType == SlotType.Craft) CraftPanel.Instance.ShowCraftResult();

            if (leftAmountToAdd == 0) return true;
            else return false;
        }
    }
}