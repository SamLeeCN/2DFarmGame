using System.Collections.Generic;
using System;
using UnityEngine;
using JetBrains.Annotations;
using Newtonsoft.Json;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 

public class SerilizableVector3
{
    public float x, y, z;
    public SerilizableVector3(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

public class IntelligentBool
{
    private List<string> changeKeys = new List<string>();

    private bool defaultValue;

    public IntelligentBool(bool defaultValue)
    {
        this.defaultValue = defaultValue;
    }
    public bool GetValue
    {
        get { return changeKeys.Count == 0 ? defaultValue : !defaultValue; }
    }
    public void SetValue(bool value, string key)
    {
        if(value != defaultValue)
        {
            if (!changeKeys.Contains(key))
            {
                changeKeys.Add(key);
            }
        }
        else
        {
            if (changeKeys.Contains(key))
            {
                changeKeys.Remove(key);
            }
        }
    }
    public void PrintKeys()
    {
        
        if (changeKeys.Count > 0)
        {
            string keys = "";
            foreach (string key in changeKeys)
            {
                keys += key + ", ";
            }
            Debug.Log("Keys: " + keys);
        }
        else
        {
            Debug.Log("No key left");
        }
        
    }

    public void PrintKeyCount()
    {
        Debug.Log(changeKeys.Count);
    }
}

namespace Farm.InventoryNamespace
{
    [System.Serializable]
    public class ItemDetail
    {
        public int itemId;
        public string itemName;
        public string description;
        public Sprite inventoryIcon;
        public Sprite onWorldSprite;
        

        public ItemType itemType;
        public int useRadius;
        public bool canPick;
        public bool canDrop;
        public bool canCarry;
        public bool hasSpecificPrefab = false;
        public GameObject specificPrefab;
        public int maxStackAmount = 64;
        public bool IsStackable { get {  return maxStackAmount>1; } }
        public int price;
        public int SellPrice {  get { return (int)(price*sellPricePercent); } }
        [Range(0f, 1f)]
        public float sellPricePercent = 0.8f;
    }

    [System.Serializable]
    public class InventoryItem
    {

        public int itemId = 0;

        public int amount = 0;

        [JsonIgnore]
        public ItemDetail Deatail 
        { 
            get 
            {
                if(itemId!=0)
                {
                    return InventoryManager.Instance.GetItemDetails(itemId);
                }
                else
                {
                    return null;
                }
            } 
        }
        public void RemoveItem()
        {
            itemId = 0;
            amount = 0;
        }

        public void CheckItemEmpty()
        {
            if (amount <= 0 || itemId == 0) RemoveItem();
        }
    }
    [System.Serializable]
    public class WorldItemData
    {
        public int itemId;
        public SerilizableVector3 pos;
        public bool hasSpecificPrefab = false;
    }

   
}
namespace GridMapNamespace
{
    [System.Serializable]
    public class TileProperty
    {
        public Vector2Int tileCoordinate;
        public GridType gridType;
        public bool boolTypeValue;
    }

    [System.Serializable]
    public class TileDetails
    {
        public Vector2Int tileCoordinate;
        public bool canDig;
        public bool canDropItem;
        public bool canPlaceFurniture;
        public bool isNpcObstacle;
        public int daysSinceDug = -1;
        public int daysSinceWatered = -1;
        public int seedItemId = -1;
        public int hasGrownDays = -1;
        public int hasRegrowTimes = -1;
    }

}
[System.Serializable]
public class NPCPosition
{
    public Transform npcTrans;
    public string startScene;
    public Vector3 position;
}
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string toSceneName;
    public List<ScenePath> scenePathList;
}
[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public bool activeFromGridCell = true;
    public Vector2Int fromGridCell;
    public bool activeGoToGridCell = true;
    public Vector2Int goToGridCell;
}
[System.Serializable]
public class WorldFurnitureData 
{
    public int itemId;
    public SerilizableVector3 pos;
    public int chestIndex = -1;
}

public class DataSlotDataDetails
{
    public string dataName;
    public string sceneName;
    public int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
}


#region player animator
[Serializable]
public class AnimatorOverrideInfo
{
    public PlayerPartEnum playerPartEnum;
    public PlayerActionEnum playerActionEnum;
    public AnimatorOverrideController overrideController;
}



#endregion


