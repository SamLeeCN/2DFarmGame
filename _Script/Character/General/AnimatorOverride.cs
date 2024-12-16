using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;
    public SpriteRenderer holdItem;
    public List<AnimatorOverrideInfo> animatorOverrrideInfos;
    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();


    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();

        foreach (Animator animator in animators)
        {
            animatorNameDict.Add(animator.name, animator);
        }
    }
    private void OnEnable()
    {
        EventHandler.ActionBarItemSelectedEvent += OnActionBarItemSelectedEvent;
        EventHandler.InventoryDataUpdateEvent += OnInventoryDataUpdateEvent;
    }
    private void OnDisable()
    {
        EventHandler.ActionBarItemSelectedEvent -= OnActionBarItemSelectedEvent;
        EventHandler.InventoryDataUpdateEvent -= OnInventoryDataUpdateEvent;
    }

    private void OnInventoryDataUpdateEvent(InventoryDataSO inventoryData, int index)
    {
        if (inventoryData.items[index] == InventoryManager.Instance.CurrentSelectedItem)
        {
            if (inventoryData.items[index].amount == 0)
                EventHandler.CallActionBarItemSelectedEvent(inventoryData, index, false);
        }
    }

    private void OnActionBarItemSelectedEvent(InventoryDataSO inventoryData, int index, bool isSlected)
    {
        int currentItemId = inventoryData.items[index].itemId;
        if (currentItemId == 0)
        {
            SwitchAnimator(PlayerActionEnum.Default);
            holdItem.enabled = false;
            return;
        }
        ItemDetail currentItemDetail = InventoryManager.Instance.GetItemDetails(currentItemId);


        holdItem.sprite = currentItemDetail.onWorldSprite;
        holdItem.enabled = isSlected;
        PlayerActionEnum currentPlayerActionEnum;
        if (isSlected) {
            switch (currentItemDetail.itemType)
            {
                case ItemType.HoeTool:
                    currentPlayerActionEnum = PlayerActionEnum.Hoe;
                    holdItem.enabled = false;
                    break;
                case ItemType.BreakTool:
                    currentPlayerActionEnum = PlayerActionEnum.Break;
                    holdItem.enabled = false;
                    break;
                case ItemType.ChopTool:
                    currentPlayerActionEnum = PlayerActionEnum.Chop;
                    holdItem.enabled = false;
                    break;
                case ItemType.ReapTool:
                    currentPlayerActionEnum = PlayerActionEnum.Reap;
                    holdItem.enabled = false;
                    break;
                case ItemType.WaterTool:
                    currentPlayerActionEnum = PlayerActionEnum.Water;
                    holdItem.enabled = false;
                    break;
                case ItemType.CollectTool:
                    currentPlayerActionEnum = PlayerActionEnum.Collect;
                    holdItem.enabled = false;
                    break;
                default:
                    currentPlayerActionEnum = PlayerActionEnum.Carry;
                    holdItem.enabled = true;
                    break;
            }
        }
        else
        {
            currentPlayerActionEnum = PlayerActionEnum.Default;
        }

        
        
        SwitchAnimator(currentPlayerActionEnum);
    }

    public void SwitchAnimator(PlayerActionEnum playerActionEnum)
    {
        foreach (var animatorOverrideInfo in animatorOverrrideInfos) 
        { 
            if(animatorOverrideInfo.playerActionEnum == playerActionEnum)
            {
                animatorNameDict[animatorOverrideInfo.playerPartEnum.ToString()].runtimeAnimatorController = animatorOverrideInfo.overrideController;
            }
        }

    }
}
