using Farm.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class DataSlotUI : MonoBehaviour
{
    public DataSlotContainer dataSlotContainer;
    public int index;
    public string saveName;
    public string saveDate;
    public string sceneName;
    public TextMeshProUGUI textUI;
    [SerializeField] private Button slotBtn;
    [SerializeField] private Button deleteBtn;

    private bool IsAutoSave => index == 0;
    private bool IsNew => index == SaveDataManager.Instance.CurrentMaxSlotIndex;
    private bool IsEmpty => !DataSlotDataDict.ContainsKey(index);

    private Dictionary<int, DataSlotDataDetails> DataSlotDataDict => SaveDataManager.Instance.dataSlotDataDict;
    private void Awake()
    {
        slotBtn.onClick.AddListener(OnDataSlotClick);
        deleteBtn.onClick.AddListener(OnDeleteClick);
    }
    void Start()
    {

    }

    void Update()
    {

    } 

    private void OnDataSlotClick()
    {
        if (dataSlotContainer.isSaveMode)
            SaveToSlot();
        else
            LoadSlot();
    }

    private void OnDeleteClick()
    {
        DeleteSlot();
    }

    private void SaveToSlot()
    {
        if (IsAutoSave) return;
        SaveDataManager.Instance.Save(index);
    }

    private void LoadSlot()
    {
        SaveDataManager.Instance.Load(index);
    }

    private void DeleteSlot()
    {
        if (IsAutoSave) return;
        SaveDataManager.Instance.Delete(index);
    }

    public void SetBasicInfo(DataSlotContainer dataSlotContainer, int index)
    {
        this.dataSlotContainer = dataSlotContainer;
        this.index = index;
    }

    public void UpdateUI()
    {
        
        if (IsEmpty)
        {
            deleteBtn.gameObject.SetActive(false);
            if(!IsAutoSave)
                textUI.text = "Click to save in new slot";
            else
                textUI.text = "Auto save with no data";
        
        }
        else
        {
            deleteBtn.gameObject.SetActive(true);
            var currentData = DataSlotDataDict[index];
            textUI.text = currentData.dataName + "\n" +
                TimeManager.Instance.GetDateString(currentData.gameMonth, currentData.gameDay, currentData.gameYear);
        }
    }
}
