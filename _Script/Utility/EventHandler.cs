using Farm.Dialogue;
using Farm.InventoryNamespace;
using GridMapNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class EventHandler : MonoBehaviour
{
    public static event Action<InventoryDataSO, int> InventoryDataUpdateEvent;
    public static void CallInventoryDataUpdateEvent(InventoryDataSO inventoryData, int index)
    {
        InventoryDataUpdateEvent?.Invoke(inventoryData, index);
    }

    public static event Action<int, int, Vector3> InstantiateItemOnWorldEvent;
    public static void CallInstantiateItemOnWorldEvent(int itemID, int amount,Vector3 pos)
    {
        InstantiateItemOnWorldEvent?.Invoke(itemID, amount, pos);
    }
    
    public static event Action<InventoryDataSO, int, bool> ActionBarItemSelectedEvent;
    public static void CallActionBarItemSelectedEvent(InventoryDataSO inventoryData, int index, bool isSelected)
    {
        ActionBarItemSelectedEvent?.Invoke(inventoryData, index, isSelected);
    }

    public static event Action<int, int, int, Season> GameMinuteEvent;
    public static void CallGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
    }

    public static event Action<int, int, int, int, Season> GameHourEvent;
    public static void CallGameHourEvent(int hour, int day, int month, int year, Season season)
    {
        GameHourEvent?.Invoke(hour, day, month, year, season);
    }

    public static event Action<int, Season> GameDayEvent;
    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }

    public static event Action<GameSceneSO, bool> BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent(GameSceneSO gameSceneSO, bool isLoadData)
    {
        BeforeSceneUnloadEvent?.Invoke(gameSceneSO, isLoadData);
    }

    public static event Action<GameSceneSO, bool , bool, bool > SceneLoadEvent;
    public static void CallSceneLoadEvent(GameSceneSO sceneToLoad, bool doTeleport, bool doFadeScreen = true, bool isLoadData = false)
    {
        SceneLoadEvent?.Invoke(sceneToLoad, doTeleport, doFadeScreen, isLoadData);
    }

    public static event Action<bool, bool> AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent(bool doTeleport, bool isFirstLoad)
    {
        AfterSceneLoadEvent?.Invoke(doTeleport, isFirstLoad);
    }

    public static event Action NewGameEvent;
    public static void CallNewGameEvent()
    {
        NewGameEvent?.Invoke();
    }

    public static event Action<float> ScreenFadeInEvent;
    public static void CallScreenFadeInEvent(float screenFadeDuration)
    {
        ScreenFadeInEvent?.Invoke(screenFadeDuration);
    }

    public static event Action<float> ScreenFadeOutEvent;
    public static void CallScreenFadeOutEvent(float screenFadeDuration)
    {
        ScreenFadeOutEvent?.Invoke(screenFadeDuration);
    }

    public static event Action<Vector3, InventoryDataSO, int> MouseClickedEvent;
    public static void CallMouseClickedEvent(Vector3 mouseWorldPos, InventoryDataSO inventoryData, int slotIndex)
    {
        MouseClickedEvent?.Invoke(mouseWorldPos, inventoryData, slotIndex);
    }

    public static event Action<Vector3, InventoryDataSO, int> ExcuteClickWorldActionEvent;
    /// <summary>
    /// called after animation is performed
    /// </summary>
    /// <param name="mouseWorldPos"></param>
    /// <param name="itemDetail"></param>
    public static void CallExcuteClickWorldActionEvent(Vector3 mouseWorldPos, InventoryDataSO inventoryData, int slotIndex)
    {
        ExcuteClickWorldActionEvent?.Invoke(mouseWorldPos, inventoryData, slotIndex);
    }

    public static event Action<Vector3, InventoryDataSO, int> DropItemEvent;
    public static void CallDropItemEvent(Vector3 endWorldPos, InventoryDataSO inventoryData, int slotIndex)
    {
        DropItemEvent?.Invoke(endWorldPos, inventoryData, slotIndex);
    }

    public static event Action<int, TileDetails> DisplaySeedEvent;
    public static void CallDisplaySeedEvent(int seedId, TileDetails tileDetails)
    {
        DisplaySeedEvent?.Invoke(seedId, tileDetails);
    }

    public static event Action RefreshMapEvent;
    public static void CallRefreshMapEvent()
    {
        RefreshMapEvent?.Invoke();
    }


    public static event Action<ParticleEffectType, Vector3> ParticleEffectEvent;
    public static void CallParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action<SoundName, Vector3> SoundEffectEvent;

    public static void CallSoundEffectEvent(SoundName soundName, Vector3 pos)
    {
        SoundEffectEvent?.Invoke(soundName, pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> DialogueEvent;
    public static void CallDialogueEvent(DialoguePiece dialoguePiece)
    {
        DialogueEvent?.Invoke(dialoguePiece);
    }

    public static event Action<GameRunningState, string> UpdateGameRunningStateEvent;
    public static void CallUpdateGameRunningStateEvent(GameRunningState gameRunningState, string key)
    {
        UpdateGameRunningStateEvent?.Invoke(gameRunningState, key);
    }

    public static event Action TradeEvent;
    public static void CallTradeEvent()
    {
        TradeEvent?.Invoke();
    }

    public static event Action AfterSaveDataModifiedEvent;
    public static void CallAfterSaveDataModifiedEvent()
    {
        AfterSaveDataModifiedEvent?.Invoke();
    }

}
