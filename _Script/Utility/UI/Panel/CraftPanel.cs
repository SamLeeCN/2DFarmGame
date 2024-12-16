using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class CraftPanel : UISingleton<CraftPanel>
{
    int craftXcorMin = 2;
    int craftYcorMin = 2;
    int bluePrintXcorMin = 2;
    int bluePrintYcorMin = 2;
    int craftXcorMax = 0;
    int craftYcorMax = 0;
    int bluePrintXcorMax = 0;
    int bluePrintYcorMax = 0;


    public void ShowCraftResult()
    {
        InventoryDataSO craftData = InventoryManager.Instance.craftTableInventoryData;
        craftData.items[Settings.CraftSlotNum].RemoveItem();
        foreach (BluePrintDetails bluePrint in InventoryManager.Instance.bluePrintSource.bluePrintDataList)
        {
            GetCorInfo(craftData.items, bluePrint);
            if (JudgeCraftMatch(craftData.items, bluePrint))
            {
                craftData.items[Settings.CraftSlotNum].itemId = bluePrint.id;
                craftData.items[Settings.CraftSlotNum].amount = bluePrint.amount;
                break;
            }
        }
        InventoryManager.Instance.craftTable.UpdateUI();
    }

    public void ClearCraftTable()
    {
        InventoryDataSO craftData = InventoryManager.Instance.craftTableInventoryData;
        List<InventoryItem> currentCraftItems = craftData.items;
        BluePrintDetails bluePrintDetails 
            = InventoryManager.Instance.GetBluePrintDetails
            (currentCraftItems[Settings.CraftSlotNum].itemId);

        GetCorInfo(currentCraftItems, bluePrintDetails);
        
        int currentColNum = bluePrintXcorMax - bluePrintXcorMin + 1;
        int currentRowNum = bluePrintYcorMax - bluePrintYcorMin + 1;

        for (int xOffset = 0; xOffset < currentColNum; xOffset++)
        {
            for (int yOffset = 0; yOffset < currentRowNum; yOffset++)
            {
                int craftItemIndex = (craftYcorMin + yOffset) * Settings.craftColNum + craftXcorMin + xOffset;
                int bluePrintItemIndex = (bluePrintYcorMin + yOffset) * Settings.craftColNum + bluePrintXcorMin + xOffset;
                currentCraftItems[craftItemIndex].amount -= bluePrintDetails.requireItems[bluePrintItemIndex].amount;
                currentCraftItems[craftItemIndex].CheckItemEmpty();
            }
        }

        InventoryManager.Instance.craftTable.UpdateUI();
    }
    
    private void GetCorInfo(List<InventoryItem> currentCraftItems, BluePrintDetails bluePrintDetails)
    {
        craftXcorMin = 2;
        craftYcorMin = 2;
        bluePrintXcorMin = 2;
        bluePrintYcorMin = 2;
        craftXcorMax = 0;
        craftYcorMax = 0;
        bluePrintXcorMax = 0;
        bluePrintYcorMax = 0;

        for (int i = 0; i < Settings.CraftSlotNum; i++)
        {
            int xCor = i % Settings.craftColNum;
            int yCor = i / Settings.craftColNum;

            if (currentCraftItems[i].itemId != 0)
            {
                if (xCor < craftXcorMin) craftXcorMin = xCor;
                if (yCor < craftYcorMin) craftYcorMin = yCor;
                if (xCor > craftXcorMax) craftXcorMax = xCor;
                if (yCor > craftYcorMax) craftYcorMax = yCor;
            }

            if (bluePrintDetails.requireItems[i].itemId != 0)
            {
                if (xCor < bluePrintXcorMin) bluePrintXcorMin = xCor;
                if (yCor < bluePrintYcorMin) bluePrintYcorMin = yCor;
                if (xCor > bluePrintXcorMax) bluePrintXcorMax = xCor;
                if (yCor > bluePrintYcorMax) bluePrintYcorMax = yCor;
            }
        }
    }

    private bool JudgeCraftMatch(List<InventoryItem> currentCraftItems,BluePrintDetails bluePrintDetails)
    {
        if (bluePrintXcorMax - bluePrintXcorMin != craftXcorMax - craftXcorMin) return false;
        if (bluePrintYcorMax - bluePrintYcorMin != craftYcorMax - craftYcorMin) return false;

        int currentColNum = bluePrintXcorMax - bluePrintXcorMin + 1;
        int currentRowNum = bluePrintYcorMax - bluePrintYcorMin + 1;

        for (int xOffset = 0; xOffset < currentColNum; xOffset++)
        {
            for (int yOffset = 0; yOffset < currentRowNum; yOffset++)
            {
                int craftItemIndex = (craftYcorMin + yOffset) * Settings.craftColNum + craftXcorMin + xOffset;
                int bluePrintItemIndex = (bluePrintYcorMin + yOffset) * Settings.craftColNum + bluePrintXcorMin + xOffset;
                if (currentCraftItems[craftItemIndex].itemId != bluePrintDetails.requireItems[bluePrintItemIndex].itemId) return false;
                if (currentCraftItems[craftItemIndex].amount < bluePrintDetails.requireItems[bluePrintItemIndex].amount) return false;
            }
        }

        return true;
    }
}
