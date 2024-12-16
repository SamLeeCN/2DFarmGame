using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class NPCFunction : MonoBehaviour
{
    public InventoryDataSO shopDataTemplate;
    public InventoryDataSO shopData;
    public int coins;

    void Start()
    {
        shopData=Instantiate(shopDataTemplate);
    }

    void Update()
    {

    }

    public void OpenShop()
    {
        UIManager.Instance.OpenShopPanel(shopData, this);
    }

    public NpcFunctionSaveData GetSaveData()
    {
        NpcFunctionSaveData saveData = new NpcFunctionSaveData();
        if (shopData != null)
            saveData.shop = shopData.items;
        saveData.coins = coins;
        return saveData;
    }

    public void LoadSaveData(NpcFunctionSaveData saveData)
    {
        if (shopData != null)
            shopData.items = saveData.shop;
        coins = saveData.coins;
    }
}

public class NpcFunctionSaveData
{
    public List<InventoryItem> shop;
    public int coins;
}
