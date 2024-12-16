using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Progress;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace
{
    public class SlotUI : MonoBehaviour, /*IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler,*/ IPointerEnterHandler, IPointerMoveHandler, IPointerExitHandler
    {
        public InventoryContainerUI inventoryUI;
        public SlotType slotType;
        public InventoryDataSO inventoryData;
        public int slotIndex;
        [property: SerializeField]
        public bool IsEmpty { get { return inventoryData.items[slotIndex].itemId == 0; } }
        [property: SerializeField]
        public bool IsSelected {
            get { return highLight.activeInHierarchy; }
            set { highLight.SetActive(value); }
        }
        public InventoryItem CurrentItem { get { return inventoryData.items[slotIndex]; } }
        public InventoryItem itemToBe = new InventoryItem { itemId = 0, amount = 0 }; 
        public bool showItemToBe = false;
        public InventoryItem displayItem;

        private bool isShowingItemTip;

        public ItemCursor ItemCursorReference => InventoryManager.Instance.itemCursor;
        [Header("UI Field")]
        [SerializeField] private GameObject highLight;
        [SerializeField] Image itemIconImg;
        [SerializeField] TextMeshProUGUI itemAmountTxt;

        public bool isBeingGiven = false;

        private void OnEnable()
        {
            EventHandler.InventoryDataUpdateEvent += OnInventoryDataUpdateEvent;
        }

        private void OnDisable()
        {
            EventHandler.InventoryDataUpdateEvent -= OnInventoryDataUpdateEvent;
            if (isShowingItemTip)
            {
                InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);
                isShowingItemTip = false;
            }
        }

        

        private void Start()
        {
            IsSelected = false;
        }

        private void Update()
        {

        }
        private void OnInventoryDataUpdateEvent(InventoryDataSO inventoryData, int index)
        {
            if (this.inventoryData == null) return;
            if (CurrentItem != inventoryData.items[index]) return;
            UpdateUI();
        }
        public void SetBasicInfo(SlotType slotType, InventoryContainerUI inventoryUI, InventoryDataSO inventoryData, int slotIndex)
        {
            this.slotType = slotType;
            this.inventoryUI = inventoryUI;
            this.inventoryData = inventoryData;
            this.slotIndex = slotIndex;
        }

        public void UpdateUI()
        {
            if (slotType == SlotType.Display) return;
            SetUpItem(inventoryData, slotIndex);
        }

        private void SetUpItem(InventoryDataSO inventoryData, int index)
        {
            if (showItemToBe)
            {
                ItemDetail itemToBeDetails = itemToBe.Deatail;
                if (itemToBe.itemId == 0 || itemToBe.amount == 0)
                {
                    itemIconImg.enabled = false;
                    itemAmountTxt.text = string.Empty;
                    return;
                }
                itemIconImg.enabled = true;
                itemAmountTxt.enabled = true;
                itemIconImg.sprite = itemToBeDetails.inventoryIcon;
                if (itemToBeDetails.IsStackable) itemAmountTxt.text = itemToBe.amount.ToString();
                else itemAmountTxt.text = string.Empty;
                return;
            }

            
            this.inventoryData = inventoryData;
            slotIndex = index;
            if (inventoryData == null) return;

            if (CurrentItem.itemId == 0 || CurrentItem.amount == 0)
            {
                itemIconImg.enabled = false;
                itemAmountTxt.text = string.Empty;
                return;
            }
            ItemDetail CurrentItemDetails = CurrentItem.Deatail;
            itemIconImg.enabled = true;
            itemAmountTxt.enabled = true;
            itemIconImg.sprite = CurrentItemDetails.inventoryIcon;
            if (CurrentItemDetails.IsStackable) itemAmountTxt.text = CurrentItem.amount.ToString();
            else itemAmountTxt.text = string.Empty;
        }

        public void SetUpDisplayItem(int itemId, int amount)
        {
            if (itemId == 0)
            {
                itemIconImg.enabled = false;
                itemAmountTxt.text = string.Empty;
                return;
            }


            itemIconImg.enabled = true;
            itemAmountTxt.enabled = true;
            itemIconImg.sprite = InventoryManager.Instance.GetItemDetails(itemId).inventoryIcon;
            if (InventoryManager.Instance.GetItemDetails(itemId).IsStackable) itemAmountTxt.text = amount.ToString();
            else itemAmountTxt.text = string.Empty;
        }

        /*public void OnPointerClick(PointerEventData eventData)
        {
            if (slotType == SlotType.Display) return;
            if (DragItemReference.isMouseDownDragging) return;
            CurrentItem.CheckItemEmpty();
            DragItemReference.isMouseDownDragging = false;
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                //Left Mouse Fast Shift (Right Mouse Fast Shift Written in DragItem)
                if (!DragItemReference.IsDragging && Input.GetKey(KeyCode.LeftShift))
                {
                    if (slotType==SlotType.Craft && slotIndex == Settings.CraftSlotNum)
                    {
                        //Fast Craft All
                        int fastCraftTime = 0;
                        while (CurrentItem.itemId != 0)
                        {
                            bool isFirstSucceed = inventoryUI.fastShiftInventory.FastShiftToHere(this, CurrentItem.amount);
                            bool isSecondSuceed = inventoryUI.secondFastShiftInventory != null;

                            if (!isFirstSucceed && inventoryUI.secondFastShiftInventory != null)
                                isSecondSuceed = inventoryUI.secondFastShiftInventory.FastShiftToHere(this, CurrentItem.amount);

                            fastCraftTime++;

                            if (!isFirstSucceed && !isSecondSuceed) break;
                            if (fastCraftTime >= Settings.fastCraftLimitedTimes) break;
                        }
                    }
                    else
                    {
                        if (!inventoryUI.fastShiftInventory.FastShiftToHere(this, CurrentItem.amount) && inventoryUI.secondFastShiftInventory != null)
                            inventoryUI.secondFastShiftInventory.FastShiftToHere(this, CurrentItem.amount);
                    }
                    return;
                }

                //Put item in the slot if no items there or same id items there
                if (DragItemReference.IsDragging)
                {
                    if (slotType == SlotType.Craft && slotIndex == Settings.CraftSlotNum)
                        return;
                    if (CurrentItem.itemId != 0 && CurrentItem.itemId != DragItemReference.item.itemId)
                        DragItemReference.SwapWithSlot(this);  
                    
                }
                else if (BagPanel.Instance.IsOpen)
                {

                    DragItemReference.GetFromSlot(this, CurrentItem.amount);
                }
                else
                {
                    inventoryUI.SelectSlot(slotIndex);
                }
                

            }
            
        }*/

        /*
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (Input.GetKey(KeyCode.LeftShift)) return;
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (ItemCursorReference.IsShowing) return;
            CurrentItem.CheckItemEmpty();
            ItemCursorReference.isDragging = true;
            if (CurrentItem.itemId == 0) return;
            if (slotType == SlotType.Display) return;
            
            ItemCursorReference.GetFromSlot(this, CurrentItem.amount);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (CurrentItem.itemId == 0) return;
            if (slotType == SlotType.Display) return;
            
            ItemCursorReference.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (slotType == SlotType.Display) return;
            if (slotType == SlotType.Craft && slotIndex == Settings.CraftSlotNum) return;
            
            if (!ItemCursorReference.isDragging) return;


            SlotUI targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
            if (targetSlot == null) return;

            if (targetSlot.CurrentItem.itemId == 0 || targetSlot.CurrentItem.itemId == ItemCursorReference.item.itemId)
            {
                ItemCursorReference.GiveToSlot(targetSlot, ItemCursorReference.item.amount);
            }
            else
            {
                ItemCursorReference.SwapWithSlot(targetSlot);
                ItemCursorReference.GiveToSlot(this, ItemCursorReference.item.amount);
            }

            if (targetSlot.slotType == SlotType.Craft) CraftPanel.Instance.ShowCraftResult();
            ItemCursorReference.isDragging = false;
        }*/



        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotType == SlotType.Display)
            {
                ShowItemTip(displayItem);
                return;
            }
            if (!showItemToBe&&CurrentItem.itemId!=0)
            {
                ShowItemTip(CurrentItem);
            }
            /*if (ItemCursorReference.IsRightMouseGiving)
            {
                ItemCursorReference.GiveToSlot(this, 1);
            }*/
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (slotType == SlotType.Display)
            {
                ShowItemTip(displayItem);
                return;
            }
            if (CurrentItem.itemId != 0)
            {
                ShowItemTip(CurrentItem);
            }
            else
            {
                InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);
            isShowingItemTip = false;
        }

        private void ShowItemTip(InventoryItem inventoryItem)
        {
            isShowingItemTip = true;
            InventoryManager.Instance.itemToolTip.SetUpUI(InventoryManager.Instance.GetItemDetails(inventoryItem.itemId));
        }

        public void ClearItemToBe()
        {
            itemToBe.RemoveItem();
            showItemToBe = false;
            UpdateUI();
        }

        public void ItemToBeBecomeItem()
        {
            itemToBe.itemId = CurrentItem.itemId;
            itemToBe.amount = CurrentItem.amount;
        }
    }
}

