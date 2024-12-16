using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class UIManager : Singleton<UIManager>
{
    public Canvas mainCanvas => GetComponent<Canvas>();
    public GameObject actionBar;
    public GameObject timeUI;
    
    void Start()
    {

    }

    void Update()
    {
        UIControl();
    } 

    public void UIControl()
    {
        if(InputManager.Instance.BagToggleInput)
        {
            if (!DialoguePanel.Instance.IsOpen){
                
                if (!BagPanel.Instance.IsOpen)
                {
                    BagPanel.Instance.PosToMultiInventoryMode();
                    BagPanel.Instance.IsOpen = true;
                    CraftPanel.Instance.IsOpen = true;
                    InventoryManager.Instance.BagPanelSetUp();
                }
                else
                {
                    BagPanel.Instance.IsOpen = false;
                    CraftPanel.Instance.IsOpen = false;
                    ShopPanel.Instance.IsOpen = false;
                    ChestPanel.Instance.IsOpen = false;
                    if (ChestPanel.Instance.currentChest!=null)
                        ChestPanel.Instance.currentChest.isOpen = false;
                    
                }
            }
        }
    }

    public void OpenShopPanel(InventoryDataSO inventoryDataSO, NPCFunction npcFunction)
    {
        InventoryManager.Instance.shop.currentOpenNpcShop = npcFunction;
        InventoryManager.Instance.shop.SetUpUI(inventoryDataSO);
        ShopPanel.Instance.IsOpen = true;
        BagPanel.Instance.PosToMultiInventoryMode();
        BagPanel.Instance.IsOpen = true;
        CraftPanel.Instance.IsOpen = false;

        InventoryManager.Instance.ShopPanelSetUp();
    }

    public void OpenChestPanel(InventoryDataSO inventoryDataSO, Chest currentChest)
    {
        ChestPanel.Instance.currentChest = currentChest;
        InventoryManager.Instance.chest.SetUpUI(inventoryDataSO);
        ChestPanel.Instance.IsOpen = true;
        BagPanel.Instance.PosToMultiInventoryMode();
        BagPanel.Instance.IsOpen = true;
        CraftPanel.Instance.IsOpen = false;

        InventoryManager.Instance.ChestPanelSetUp();
    }

    public void SetUIEnability(bool enability)
    {
        if (!enability) {
            ChestPanel.Instance.IsOpen = false;
            BagPanel.Instance.IsOpen = false;
            CraftPanel.Instance.IsOpen = false;
            ShopPanel.Instance.IsOpen = false;
            actionBar.SetActive(false);
            timeUI.SetActive(false);
        }
        else
        {
            actionBar.SetActive(true);
            timeUI.SetActive(true);
        }

    }
}
