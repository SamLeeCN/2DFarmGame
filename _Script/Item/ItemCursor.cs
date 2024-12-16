using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    public class ItemCursor : MonoBehaviour
    {

        public InventoryItem item = new InventoryItem { itemId = 0, amount = 0};
        public InventoryItem itemToBe = new InventoryItem { itemId = 0, amount = 0 };
        public bool showItemToBe = false;
        public SlotUI currentlyGettingSlot;

        [SerializeField] private bool isBelongToPlayer = true;
        [SerializeField] private NPCFunction belongToNpcFunction;

        [Header("UI Field")]
        private Image image;
        [SerializeField] private TextMeshProUGUI amountText;
        
        public bool IsShowing { get { return image.IsActive(); } 
            private set { 
                image.enabled = value;
                amountText.enabled = value;
                if (!value){
                    item.RemoveItem();
                    isBelongToPlayer = true;
                    currentlyGettingSlot = null;
                }
            } 
        }

        public bool isDragging = false;

        private List<GivingDetails> leftMouseGivingList = new List<GivingDetails>();

        private class GivingDetails
        {
            public GivingDetails(SlotUI slot) 
            { 
                this.slot = slot;
            }
            public SlotUI slot;
            public int givingAmount = 0;
        }

        private bool isLeftMouseGivingLastFrame = false;
        private bool IsLeftMouseGiving
            => !isDragging && IsShowing && Input.GetMouseButton(0);

        
        private bool isRightMouseGivingLastFrame = false;
        public bool IsRightMouseGiving =>!isDragging && IsShowing && Input.GetMouseButton(1) && !Input.GetMouseButton(0);

        private SlotUI hangingSlot;
        private SlotUI hangingSlotLastFrame;

        private SlotUI draggingSlot;

        private float rightMouseDownTimer;
        private float rightMouseShiftIntervalTimer;
        
        void Start()
        {
            image = GetComponent<Image>();
        }

        private void Update()
        {

            if (rightMouseDownTimer < Settings.longPressThreshold) rightMouseDownTimer += Time.deltaTime;
            if (rightMouseShiftIntervalTimer < Settings.rightMouseFastShiftInterval) rightMouseShiftIntervalTimer += Time.deltaTime;

            UpdateHangingSlot();

            InteractionControl();
            
            
            isLeftMouseGivingLastFrame = IsLeftMouseGiving;
            isRightMouseGivingLastFrame = IsRightMouseGiving;
            hangingSlotLastFrame = hangingSlot;

            if (IsShowing)
            {
                transform.position = Input.mousePosition;
            }
        }
        
        
        public void UpdateUI()
        {
            
            SetUpItem(item.itemId, item.amount);
            
        }

        private void SetUpItem(int itemId, int amount)
        {
            if (showItemToBe)
            {
                itemToBe.CheckItemEmpty();
                if (itemToBe.itemId == 0)
                {
                    amountText.text = string.Empty;
                    return;
                }
                image.sprite = itemToBe.Deatail.inventoryIcon;
                if (itemToBe.Deatail.IsStackable) amountText.text = itemToBe.amount.ToString();
                else amountText.text = string.Empty;

                return;
            }
            item.itemId = itemId;
            item.amount = amount;
            item.CheckItemEmpty();
            if (item.itemId == 0)
            {
                IsShowing = false;
                amountText.text = string.Empty;
                return;
            }
            else
            {
                IsShowing = true;
            }
            
            image.sprite = item.Deatail.inventoryIcon;
            if (item.Deatail.IsStackable) amountText.text = item.amount.ToString();
            else amountText.text = string.Empty;
        }

        public void SwapWithSlot(SlotUI slot)
        {
            if (slot.slotType == SlotType.Craft && slot.slotIndex == Settings.craftRowNum * Settings.craftColNum)
                return;
            ItemDetail dragItemDetail;
            if (item.itemId != 0) dragItemDetail = item.Deatail;
            else dragItemDetail = null;

            ItemDetail slotItemDetail;
            if (slot.CurrentItem.itemId != 0) slotItemDetail = slot.CurrentItem.Deatail;
            else slotItemDetail = null;


            if (item.itemId!=0 && !isBelongToPlayer && slot.slotType != SlotType.Shop)
            {// Buy
                if (item.amount * dragItemDetail.price > GameManager.Instance.playerControler.coins)
                {
                    return;
                }
                else
                {
                    GameManager.Instance.playerControler.coins -= dragItemDetail.price * item.amount;
                    belongToNpcFunction.coins += dragItemDetail.price * item.amount;
                    EventHandler.CallTradeEvent();
                    isBelongToPlayer = true;
                }
            }

            if (item.itemId != 0 && isBelongToPlayer && slot.slotType == SlotType.Shop)
            {// Sell
                
                if (item.amount * dragItemDetail.SellPrice > slot.inventoryUI.currentOpenNpcShop.coins)
                {
                    return;
                }
                else
                {
                    
                    GameManager.Instance.playerControler.coins += dragItemDetail.SellPrice * item.amount;
                    slot.inventoryUI.currentOpenNpcShop.coins -= dragItemDetail.SellPrice * item.amount;
                    EventHandler.CallTradeEvent();

                    isBelongToPlayer = false;
                    belongToNpcFunction = slot.inventoryUI.currentOpenNpcShop;
                }
            }

            currentlyGettingSlot = null;
            int tmpId = item.itemId;
            int tmpAmount = item.amount;
            item.itemId = slot.CurrentItem.itemId;
            item.amount = slot.CurrentItem.amount;
            slot.CurrentItem.itemId = tmpId;
            slot.CurrentItem.amount = tmpAmount;

            if (slot.slotType == SlotType.Shop)
            {
                isBelongToPlayer = false;
            }
            
            OnSlotOperationEnd(slot);

            if (slot.slotType == SlotType.Craft)
                CraftPanel.Instance.ShowCraftResult();

        }
        
        public void GetFromSlot(SlotUI slot, int amount)
        {
            if (item.itemId != 0 && slot.CurrentItem.itemId != item.itemId) return;

            if (slot.CurrentItem.itemId == 0) return;


            if (!isBelongToPlayer && slot.slotType != SlotType.Shop) return;

            if (item.itemId == 0 && slot.slotType == SlotType.Shop)
            {
                isBelongToPlayer = false;
                belongToNpcFunction = slot.inventoryUI.currentOpenNpcShop;
            }


            ItemDetail dragItemDetail;
            if (item.itemId != 0) dragItemDetail = item.Deatail;
            else dragItemDetail = null;

            ItemDetail slotItemDetail;
            if (slot.CurrentItem.itemId != 0) slotItemDetail = slot.CurrentItem.Deatail;
            else slotItemDetail = null;

            currentlyGettingSlot = slot;
            
            int getAmount = amount;
            if (amount + item.amount > slotItemDetail.maxStackAmount)
            {
                getAmount = slotItemDetail.maxStackAmount - item.amount;
            }

            item.itemId = slot.CurrentItem.itemId;
            item.amount += getAmount;
            
            if (slot.slotType == SlotType.Craft && slot.slotIndex == Settings.CraftSlotNum)
                CraftPanel.Instance.ClearCraftTable();

            slot.CurrentItem.amount -= getAmount;
            OnSlotOperationEnd(slot);
            if (slot.CurrentItem.itemId == 0) currentlyGettingSlot = null;

            if (slot.slotType == SlotType.Craft)
                CraftPanel.Instance.ShowCraftResult();
        }

        public void GiveToSlot(SlotUI slot, int amount)
        {
            if (slot.slotType == SlotType.Craft && slot.slotIndex == Settings.CraftSlotNum) return;
            if (slot.CurrentItem.itemId!=0 && slot.CurrentItem.itemId != item.itemId) return;
            if (item.itemId == 0) return;

            ItemDetail dragItemDetail;
            if (item.itemId!=0) dragItemDetail = item.Deatail;
            else dragItemDetail = null;

            ItemDetail slotItemDetail;
            if (slot.CurrentItem.itemId != 0) slotItemDetail = slot.CurrentItem.Deatail;
            else slotItemDetail = null;

            currentlyGettingSlot = null;
            
            int putAmount = amount;
            if (slot.CurrentItem.itemId!=0 && slot.CurrentItem.amount + item.amount > slotItemDetail.maxStackAmount)
            {

                putAmount = slotItemDetail.maxStackAmount - slot.CurrentItem.amount;

            }

            
            if (!isBelongToPlayer && slot.slotType != SlotType.Shop)
            {// Buy
                if (putAmount * dragItemDetail.price > GameManager.Instance.playerControler.coins)
                {
                    putAmount = GameManager.Instance.playerControler.coins / dragItemDetail.price;
                }
                GameManager.Instance.playerControler.coins -= putAmount * dragItemDetail.price;
                belongToNpcFunction.coins += putAmount * dragItemDetail.price;
                EventHandler.CallTradeEvent();
            }
            
            if (isBelongToPlayer && slot.slotType == SlotType.Shop)
            {// Sell
                
                if (putAmount * dragItemDetail.SellPrice  > slot.inventoryUI.currentOpenNpcShop.coins)
                {
                    putAmount = slot.inventoryUI.currentOpenNpcShop.coins / dragItemDetail.SellPrice;
                }
                GameManager.Instance.playerControler.coins += putAmount * dragItemDetail.SellPrice;
                slot.inventoryUI.currentOpenNpcShop.coins -= putAmount * dragItemDetail.SellPrice;
                EventHandler.CallTradeEvent();
            }
            slot.CurrentItem.itemId = item.itemId;
            slot.CurrentItem.amount += putAmount;
            item.amount -= putAmount;
            OnSlotOperationEnd(slot);

            if (slot.slotType == SlotType.Craft)
                CraftPanel.Instance.ShowCraftResult();
        }

        public void DropItem(int amount)
        {
            if (item.itemId==0) return;
            //TODO:Judge tile droppable and instantiate

            item.amount -= amount;
            item.CheckItemEmpty();
            UpdateUI();
            if (item.itemId == 0)
                IsShowing = false;
        }
        private void OnSlotOperationEnd(SlotUI slot)
        {
            slot.CurrentItem.CheckItemEmpty();
            item.CheckItemEmpty();
            slot.UpdateUI();
            UpdateUI();
            if (item.itemId == 0)
                IsShowing = false;
        }

        /*private void DropItem(int amount)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint
                    (new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            EventHandler.CallInstantiateItemOnWorldEvent(CurrentItem.itemId, amount, pos);
            EventHandler.CallInventoryDataUpdateEvent(inventoryData);
            inventoryData.DeclineItemAtIndex(slotIndex, amount);
        }

        private void ItemDropControl()
        {
            if (!DragItemReference.IsDragging) return;

            if (InputManager.Instance.DropEachItemInput)
            {
                if (!IsEmpty)
                {
                    DropItem(1);
                }
            }
        }*/


        private void AddToLeftMouseGivingList(SlotUI slot)
        {
            if (slot.slotType == SlotType.Craft && slot.slotIndex == Settings.CraftSlotNum) return;

            slot.isBeingGiven = true;
            slot.showItemToBe = true;
            GivingDetails givingDetails = new GivingDetails(slot);
            leftMouseGivingList.Add(givingDetails);
            OnGivingListChange();

        }


        /*private void RemoveFromLongPressSelectedSlots(SlotUI slot)
        {
            foreach (SlotUI tmpSlot in longPressSelectedSlots)
            {
                if (tmpSlot == slot)
                {
                    slot.isLongPressSelected = false;
                    slot.showItemToBe = false;
                    slot.UpdateUI();
                    longPressSelectedSlots.Remove(slot);
                    break;
                }
            }
            OnLongPressSelectionChange();
        }*/

        private void ClearLeftMouseGivingList()
        {
            foreach (GivingDetails givingDetails in leftMouseGivingList)
            {

                givingDetails.slot.isBeingGiven = false;
                givingDetails.slot.showItemToBe = false;
            }
            leftMouseGivingList.Clear();
        }

        private void OnLeftMouseGivingStart()
        {
            ItemToBeBecomeItem();
            showItemToBe = true;
        }
        private void OnLeftMouseGiving()
        {
            if (CursorManager.Instance.isUiHit)
            {
                SlotUI targetSlot = CursorManager.Instance.uiRayCastResults[0].gameObject.GetComponent<SlotUI>();

                if (targetSlot == null || targetSlot.isBeingGiven) return;
                if (targetSlot.CurrentItem.itemId == 0 || targetSlot.CurrentItem.itemId == item.itemId)
                {
                    AddToLeftMouseGivingList(targetSlot);
                }
                OnGivingListChange();
            }
        }
        private void OnGivingListChange()
        {
            if (leftMouseGivingList.Count == 0) return;
            ItemDetail itemDetail = item.Deatail;
            int itemGiveToEachSlot = item.amount/leftMouseGivingList.Count;
            ItemToBeBecomeItem();
            
            foreach (GivingDetails givingDetails in leftMouseGivingList)
            {
                givingDetails.slot.ItemToBeBecomeItem();
                givingDetails.slot.itemToBe.itemId = item.itemId;
                int giveAmount = itemGiveToEachSlot;
                if (giveAmount + givingDetails.slot.CurrentItem.amount > itemDetail.maxStackAmount)
                    giveAmount = itemDetail.maxStackAmount - givingDetails.slot.CurrentItem.amount;
                givingDetails.slot.itemToBe.amount += giveAmount;
                givingDetails.givingAmount = giveAmount;
                itemToBe.amount -= giveAmount;
                if (leftMouseGivingList.Count > 1)
                    givingDetails.slot.UpdateUI();
            }
            if (leftMouseGivingList.Count > 1)
                UpdateUI();
        }

        private void OnLeftMouseGivingEnd()
        {
            foreach (GivingDetails givingDetails in leftMouseGivingList)
            {
                GiveToSlot(givingDetails.slot, givingDetails.givingAmount);
                givingDetails.slot.ClearItemToBe();
            }
            ClearItemToBe();
            ClearLeftMouseGivingList();
        }

        public void ClearItemToBe()
        {
            itemToBe.RemoveItem();
            showItemToBe = false;
            UpdateUI();
        }

        public void ItemToBeBecomeItem()
        {
            itemToBe.itemId = item.itemId;
            itemToBe.amount = item.amount;
        }

        private void OnRightMouseGivingStart()
        {
            if (hangingSlot == null) return;
            if (hangingSlot == null || currentlyGettingSlot == hangingSlot) return;
            if (hangingSlot.CurrentItem.itemId == 0 || hangingSlot.CurrentItem.itemId == item.itemId)
            {
                GiveToSlot(hangingSlot, 1);
            }
        }
        

        private void RightMouseGetSlotItem()
        {
            if (hangingSlot == null) return;
            //Take item from the slot if drag item is not dragging or dragging current slot
            if (item.itemId == 0 || currentlyGettingSlot == hangingSlot)
                GetFromSlot(hangingSlot, 1);
        }

        private void RightMouseShiftItem()
        {
            if (IsShowing || !Input.GetKey(KeyCode.LeftShift)) return;
            if (hangingSlot == null) return;

            if (!hangingSlot.inventoryUI.fastShiftInventory.FastShiftToHere(hangingSlot, 1) && hangingSlot.inventoryUI.secondFastShiftInventory != null)
                hangingSlot.inventoryUI.secondFastShiftInventory.FastShiftToHere(hangingSlot, 1);
            
        }

        private void LeftMouseGetOrSwap()
        {
            if (hangingSlot == null) return;
            if (hangingSlot.slotType == SlotType.Display) return;
            if (isDragging) return;
            hangingSlot.CurrentItem.CheckItemEmpty();
            isDragging = false;

            if (IsShowing)
            {
                if (hangingSlot.slotType == SlotType.Craft && hangingSlot.slotIndex == Settings.CraftSlotNum)
                    return;
                if (hangingSlot.CurrentItem.itemId != 0 && hangingSlot.CurrentItem.itemId != item.itemId)
                    SwapWithSlot(hangingSlot);

            }
            else if (BagPanel.Instance.IsOpen)
            {

                GetFromSlot(hangingSlot, hangingSlot.CurrentItem.amount);
            }
        }

        private void LeftMouseSelectActionBarSlot()
        {
            if (hangingSlot == null) return;
            if (hangingSlot.slotType == SlotType.Display) return;
            hangingSlot.CurrentItem.CheckItemEmpty();
            isDragging = false;

            if (!IsShowing && !BagPanel.Instance.IsOpen) 
                hangingSlot.inventoryUI.SelectSlot(hangingSlot.slotIndex);
        }

        private void LeftMouseFastCraft()
        {
            if (hangingSlot == null) return;
            if (hangingSlot.slotType == SlotType.Craft && hangingSlot.slotIndex == Settings.CraftSlotNum)
            {
                //Fast Craft All
                int fastCraftTime = 0;
                while (hangingSlot.CurrentItem.itemId != 0)
                {
                    bool isFirstSucceed = hangingSlot.inventoryUI.fastShiftInventory.FastShiftToHere(hangingSlot, hangingSlot.CurrentItem.amount);
                    bool isSecondSuceed = hangingSlot.inventoryUI.secondFastShiftInventory != null;

                    if (!isFirstSucceed && hangingSlot.inventoryUI.secondFastShiftInventory != null)
                        isSecondSuceed = hangingSlot.inventoryUI.secondFastShiftInventory.FastShiftToHere(hangingSlot, hangingSlot.CurrentItem.amount);

                    fastCraftTime++;

                    if (!isFirstSucceed && !isSecondSuceed) break;
                    if (fastCraftTime >= Settings.fastCraftLimitedTimes) break;
                }
            }
        }

        private void LeftMouseShift()
        {
            if (hangingSlot == null) return;
            if (hangingSlot.slotType == SlotType.Craft && hangingSlot.slotIndex == Settings.CraftSlotNum) return;
            if (!hangingSlot.inventoryUI.fastShiftInventory.FastShiftToHere(hangingSlot, hangingSlot.CurrentItem.amount) && hangingSlot.inventoryUI.secondFastShiftInventory != null)
                hangingSlot.inventoryUI.secondFastShiftInventory.FastShiftToHere(hangingSlot, hangingSlot.CurrentItem.amount);
        }

        private void UpdateHangingSlot()
        {
            if (CursorManager.Instance.isUiHit)
                hangingSlot = CursorManager.Instance.uiRayCastResults[0].gameObject.GetComponent<SlotUI>();
            else
                hangingSlot = null;
        }

        private void OnSlotDraggingBegin()
        {
            if (hangingSlot == null) return;
            if (hangingSlot.slotType == SlotType.Display) return;
            isDragging = true;
            draggingSlot = hangingSlot;
            GetFromSlot(hangingSlot, hangingSlot.CurrentItem.amount);
        }
        private void OnSlotDragging()
        {
            //Nothing needs to be done;
        }

        private void OnSlotDraggingEnd()
        {
            if (draggingSlot.slotType == SlotType.Display) return;
            if (draggingSlot.slotType == SlotType.Craft && draggingSlot.slotIndex == Settings.CraftSlotNum) return;
            if (hangingSlot == null) return;

            


            if (hangingSlot.CurrentItem.itemId == 0 || hangingSlot.CurrentItem.itemId == item.itemId)
            {
                GiveToSlot(hangingSlot, item.amount);
            }
            else
            {
                SwapWithSlot(hangingSlot);
                GiveToSlot(draggingSlot, item.amount);
            }

            if (hangingSlot.slotType == SlotType.Craft) CraftPanel.Instance.ShowCraftResult();
            isDragging = false;
        } 

        private SlotUI GetCurrentEnteringSlot()
        {
            if (hangingSlot != hangingSlotLastFrame && hangingSlot != null) return hangingSlot;
            else return null;
        }

        private SlotUI GetCurrentExitingSlot()
        {
            if (hangingSlot != hangingSlotLastFrame && hangingSlotLastFrame != null) return hangingSlot;
            else return null;
        }

        private void InteractionControl()
        {
            if (!CursorManager.Instance.isUiHit) return;


            if (Input.GetMouseButtonDown(0))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    LeftMouseFastCraft();
                }
                else
                {
                    
                    if (item.itemId != 0 && (hangingSlot.CurrentItem.itemId == 0|| hangingSlot.CurrentItem.itemId == item.itemId))
                    {
                        if (!isDragging && IsShowing) OnLeftMouseGivingStart();
                    }
                    else if (item.itemId == 0 && hangingSlot.CurrentItem.itemId != 0)
                    {
                        if(BagPanel.Instance.IsOpen) OnSlotDraggingBegin();
                        else LeftMouseSelectActionBarSlot();
                    }
                }
            }

            if (Input.GetMouseButton(0)){
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    LeftMouseShift();
                }
                else
                {
                    if (IsLeftMouseGiving) OnLeftMouseGiving();
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {

                }
                else
                {
                    
                    if (item.itemId == 0 || hangingSlot.CurrentItem.itemId != item.itemId)
                    {
                        if (!isDragging) LeftMouseGetOrSwap();                     
                    }
                    
                    if (isLeftMouseGivingLastFrame && !IsLeftMouseGiving) OnLeftMouseGivingEnd();

                    if (hangingSlot == draggingSlot) isDragging = false;

                    if (isDragging)
                    {
                        OnSlotDraggingEnd();
                    }

                }
                
            }

            if (Input.GetMouseButton(0)) return; //BAN ALL THE RIGHT MOUSE INTERACTION WHILE HOLDING LEFT MOUSE


            if (Input.GetMouseButtonDown(1))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    RightMouseShiftItem();
                    rightMouseDownTimer = 0;
                }
                else
                {
                    if (item.itemId == 0 || currentlyGettingSlot == hangingSlot)
                        RightMouseGetSlotItem();
                    else OnRightMouseGivingStart();
                }
            }

            if (Input.GetMouseButton(1))
            {
                if (Input.GetKey(KeyCode.LeftShift) && item.itemId == 0)
                {
                    
                    if (rightMouseDownTimer >= Settings.longPressThreshold && rightMouseShiftIntervalTimer >= Settings.rightMouseFastShiftInterval)
                    {
                        RightMouseShiftItem();
                        rightMouseShiftIntervalTimer = 0;
                    }
                }
                else
                {
                    SlotUI currentEnteringSlot = GetCurrentEnteringSlot();
                    if (currentEnteringSlot != null) GiveToSlot(currentEnteringSlot, 1);
                }
            }
            

            if (Input.GetMouseButtonUp(1))
            {

            }
        }
    }
}