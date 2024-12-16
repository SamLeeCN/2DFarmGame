using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    [CreateAssetMenu(menuName = "DataSO/InventoryDataSO")]
    public class InventoryDataSO : ScriptableObject
    {
        public List<InventoryItem> items = new List<InventoryItem>();


        public int GetEmptyIndex(int indexRangeLow = -1, int indexRangeHigh = -1)
        {
            int fromIndex = indexRangeLow == -1 ? 0 : indexRangeLow;
            int toIndex = indexRangeHigh == -1 ? items.Count - 1 : indexRangeHigh;

            for (int i = fromIndex; i <= toIndex; i++)
            {
                if (items[i].itemId == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<int> GetItemIndexes(int id, int indexRangeLow = -1, int indexRangeHigh = -1)
        {
            int fromIndex = indexRangeLow == -1 ? 0 : indexRangeLow;
            int toIndex = indexRangeHigh == -1 ? items.Count - 1 : indexRangeHigh;

            List<int> result = new List<int>();
            for (int i = fromIndex; i <= toIndex; i++)
            {
                if (items[i].itemId == id)
                {
                    result.Add(i);
                }
            }
            return result;
        }
        public int CountItemAmount(int id, int indexRangeLow = -1, int indexRangeHigh = -1)
        {
            int fromIndex = indexRangeLow == -1 ? 0 : indexRangeLow;
            int toIndex = indexRangeHigh == -1 ? items.Count - 1 : indexRangeHigh;

            int result = 0;
            for (int i = fromIndex; i <= toIndex; i++)
            {
                if (items[i].itemId == id)
                {
                    result += items[i].amount;
                }
            }
            return result;
        }
        public int AddItemSimulation(int id, int amount, bool addWhenExceedStackRange = true, int indexRangeLow = -1, int indexRangeHigh = -1)
        {
            
            int leftAmountToAdd = amount;
            List<int> currentItemIndexes = GetItemIndexes(id, indexRangeLow, indexRangeHigh);


            if (currentItemIndexes.Count > 0)
            {
                foreach (int currentItemIndex in currentItemIndexes)
                {
                    leftAmountToAdd = AddItemAtIndexSimulation(currentItemIndex, id, leftAmountToAdd, addWhenExceedStackRange);
                    if (leftAmountToAdd == 0)
                    {
                        return 0;
                    }
                }
            }

            int emptyIndex = GetEmptyIndex(indexRangeLow, indexRangeHigh);
            while (emptyIndex != -1 && leftAmountToAdd > 0)
            {
                leftAmountToAdd = AddItemAtIndexSimulation(emptyIndex, id, leftAmountToAdd, addWhenExceedStackRange);
                emptyIndex = GetEmptyIndex(emptyIndex + 1, indexRangeHigh);
            }
            return leftAmountToAdd;
        }

        /// <summary>
        /// return left item amount to add.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddItem(int id, int amount, bool addWhenExceedStackRange = true, int indexRangeLow = -1, int indexRangeHigh = -1)
        {
            
            int leftAmountToAdd = amount;
            List<int> currentItemIndexes = GetItemIndexes(id, indexRangeLow, indexRangeHigh);


            if (currentItemIndexes.Count > 0)
            {
                foreach (int currentItemIndex in currentItemIndexes)
                {
                    leftAmountToAdd = AddItemAtIndex(currentItemIndex, id, leftAmountToAdd, addWhenExceedStackRange);
                    if (leftAmountToAdd == 0)
                    {
                        return 0;
                    }
                }
            }

            int emptyIndex = GetEmptyIndex(indexRangeLow, indexRangeHigh);
            while (emptyIndex != -1 && leftAmountToAdd > 0)
            {
                leftAmountToAdd = AddItemAtIndex(emptyIndex, id, leftAmountToAdd , addWhenExceedStackRange);
                emptyIndex = GetEmptyIndex(emptyIndex, indexRangeHigh);
            }
            
            return leftAmountToAdd;
        }


        public int AddItemAtIndexSimulation(int index, int id, int addAmount, bool addWhenExceedStackRange = true)
        {
            if (id == 0) return 0;
            int leftAmountToAdd = addAmount;
            InventoryItem item = items[index];
            if (item.itemId != 0 && item.itemId != id) return leftAmountToAdd;

            int itemMaxStack = InventoryManager.Instance.GetItemDetails(id).maxStackAmount;

            if (item.itemId == 0)
            {
                if (leftAmountToAdd > itemMaxStack)
                {
                    leftAmountToAdd -= itemMaxStack;
                }
                else
                {
                    leftAmountToAdd = 0;
                }
            }
            else
            {
                if (item.amount + leftAmountToAdd > itemMaxStack)
                {
                    if (addWhenExceedStackRange)
                    {
                        leftAmountToAdd = leftAmountToAdd - (itemMaxStack - item.amount);
                    }

                }
                else
                {
                    leftAmountToAdd = 0;
                }
            }
            return leftAmountToAdd;
        }
        /// <summary>
        /// return left item amount to add.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddItemAtIndex(int index, int id, int addAmount , bool addWhenExceedStackRange = true)
        {
            if (id == 0) return 0;
            int leftAmountToAdd = addAmount;
            InventoryItem item = items[index];
            if (item.itemId != 0 && item.itemId != id) return leftAmountToAdd;

            int itemMaxStack = InventoryManager.Instance.GetItemDetails(id).maxStackAmount;

            if (item.itemId == 0)
            {
                item.itemId = id;
                if (leftAmountToAdd > itemMaxStack)
                {
                    leftAmountToAdd -= itemMaxStack;
                    item.amount = itemMaxStack;
                }
                else
                {
                    item.amount += leftAmountToAdd;
                    leftAmountToAdd = 0;
                }
            }
            else
            {
                if (item.amount + leftAmountToAdd > itemMaxStack)
                {
                    if (addWhenExceedStackRange)
                    {
                        leftAmountToAdd = leftAmountToAdd - (itemMaxStack - item.amount);
                        item.amount = itemMaxStack;
                    }

                }
                else
                {
                    item.amount += leftAmountToAdd;
                    leftAmountToAdd = 0;
                }
            }
            EventHandler.CallInventoryDataUpdateEvent(this, index);
            return leftAmountToAdd;
        }
        /// <summary>
        /// return left item amount to decline.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int DeclineItem(int id, int declineAmount, bool declineWhenNotEnough = false, int indexRangeLow = -1, int indexRangeHigh = -1)
        {
            int leftAmountToDecline = declineAmount;
            if (CountItemAmount(id, indexRangeLow, indexRangeHigh) < leftAmountToDecline && !declineWhenNotEnough) return leftAmountToDecline;

            List<int> currentItemIndexes = GetItemIndexes(id, indexRangeLow, indexRangeHigh);

            if (currentItemIndexes.Count > 0)
            {
                foreach (int currentItemIndex in currentItemIndexes)
                {
                    leftAmountToDecline = DeclineItemAtIndex(currentItemIndex, leftAmountToDecline, true);
                    if (leftAmountToDecline == 0) break;
                }
            }
            
            return leftAmountToDecline;
        }
        /// <summary>
        /// return left item amount to decline.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int DeclineItemAtIndex(int index, int declineAmount, bool declineWhenNotEnough = false)
        {
            int leftAmountToDecline = declineAmount;
            InventoryItem item = items[index];
            if (declineAmount > item.amount && !declineWhenNotEnough) return leftAmountToDecline;

            if (leftAmountToDecline >= item.amount)
            {
                leftAmountToDecline = leftAmountToDecline - item.amount;
                items[index].RemoveItem();
            }
            else
            {
                items[index].amount -= leftAmountToDecline;
                leftAmountToDecline = 0;
            }
            EventHandler.CallInventoryDataUpdateEvent(this, index);
            return leftAmountToDecline;
        }
    }
}