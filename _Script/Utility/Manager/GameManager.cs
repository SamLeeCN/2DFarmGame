using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class GameManager : Singleton<GameManager>
{
    public Character playerCharacter;
    public PlayerControler playerControler;
    public Transform PlayerTransform => playerControler.transform;
    public Inventory playerInventory;
    public bool isPlayerDead;
    public bool isGameOver = false;

    #region Save and Load
    public PlayerSaveData GetPlayerSaveData()
    {
        return playerControler.GetSaveData();
    }

    public void RestorePlayerSaveData(PlayerSaveData playerSaveData)
    {
        playerControler.LoadSaveData(playerSaveData);
    }
    #endregion
    private void Awake()
    {
        
    }
    public void SetPlayerEnability(bool enability)
    {
        playerCharacter.gameObject.SetActive(enability);
    }

    public void JudgeAndUpdateGameRunningStateByPanel()
    {
        if (ShopPanel.Instance.IsOpen || BagPanel.Instance.IsOpen || DialoguePanel.Instance.IsOpen || SaveLoadPanel.Instance.IsOpen)
        {
            EventHandler.CallUpdateGameRunningStateEvent(GameRunningState.Pause, "RegularPanel");
        }
        else 
        {
            EventHandler.CallUpdateGameRunningStateEvent(GameRunningState.GamePlay, "RegularPanel");
        }
    }

    
}
