using Farm.InventoryNamespace;
using Farm.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class DataSlotContainer : MonoBehaviour
{
    public bool isSaveMode = false;
    [Header("Page Flip")]
    [SerializeField] private int indexRangeLow = -1;
    [SerializeField] private int indexRangeHigh = -1;
    [SerializeField] private DataPageFliper pageFliper;

    [Header("Settings")]
    [SerializeField] private int startIndex;

    public List<DataSlotUI> slots;

    public int CurrentMaxSlotIndex => SaveDataManager.Instance.CurrentMaxSlotIndex;
    void Start()
    {
        SetUpPageFliper();
    }

    private void OnEnable()
    {
        EventHandler.AfterSaveDataModifiedEvent += UpdateUI;
        SetUpUI(startIndex);
    }
    private void OnDisable()
    {
        EventHandler.AfterSaveDataModifiedEvent -= UpdateUI;
    }
    void Update()
    {

    }

    public void SetUpUI(int startIndex = 0)
    {
        this.startIndex = startIndex;
        int showCount = isSaveMode? CurrentMaxSlotIndex + 1 : CurrentMaxSlotIndex;

        for (int i = 0; i < slots.Count; i++)
        {
            if (i + startIndex > showCount)
            {
                slots[i].gameObject.SetActive(false);
                slots[i].SetBasicInfo(this, -1);
            }
            else
            {
                slots[i].gameObject.SetActive(true);
                slots[i].SetBasicInfo(this, i + startIndex);
                slots[i].UpdateUI();
            }
        }
        SetUpPageFliper();
    }

    public void UpdateUI()
    {
        SetUpUI(startIndex);
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].UpdateUI();
        }
    }

    public void SetUpPageFliper()
    {
        if (pageFliper != null)
        {
            int showIndexHigh;
            if (isSaveMode)
            {
                showIndexHigh = indexRangeHigh + 1;
            }
            else
            {
                showIndexHigh = indexRangeHigh;
            }
            pageFliper.SetUpUI(this, startIndex, indexRangeLow, indexRangeHigh, isSaveMode);
        }
    }
}
