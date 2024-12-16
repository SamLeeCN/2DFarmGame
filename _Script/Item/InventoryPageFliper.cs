using Farm.InventoryNamespace;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class InventoryPageFliper : MonoBehaviour
{
    [SerializeField] private InventoryContainerUI inventoryUI;
    [SerializeField] private InventoryDataSO inventoryData;
    [SerializeField] private int ultimateStartIndex;
    [SerializeField] private int ultimateEndIndex;

    [SerializeField] private int currentPage;
    [SerializeField] private int maxPage;
    [SerializeField] private int eachPageAmount;
    [SerializeField] private int totalAmount;
    [Header("UI Field")]
    [SerializeField] private TextMeshProUGUI pageTxt;
    [SerializeField] private Button pageUpBtn;
    [SerializeField] private Button pageDownBtn;
    private void Start()
    {
        pageUpBtn.onClick.AddListener(OnPageUp);
        pageDownBtn.onClick.AddListener(OnPageDown);
    }
    private void Update()
    {
        ShortCutControl();
    }
    public void SetUpUI(InventoryContainerUI inventoryUI,InventoryDataSO inventoryData,int currentStartIndex,int low,int high)
    {
        gameObject.SetActive(true);

        this.inventoryUI = inventoryUI;
        eachPageAmount = inventoryUI.slots.Count;
        ultimateStartIndex = low == -1 ? 0 : low;
        ultimateEndIndex = high == -1 ? inventoryData.items.Count : high;

        this.inventoryData = inventoryData;
        totalAmount = ultimateEndIndex - ultimateStartIndex;//inventoryData.items.Count;

        maxPage = (totalAmount-1)/eachPageAmount+1;
        if(maxPage <= 1) gameObject.SetActive(false);

        currentPage =GetPageByIndex(currentStartIndex);
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        pageTxt.text = currentPage.ToString() + "/" + maxPage.ToString();
    }

    private void RefreshInventoryUI()
    {
        inventoryUI.SetUpUI(inventoryData, GetIndexByPage(currentPage));
    }

    private void OnPageUp()
    {
        if (currentPage <= 1) return;

        currentPage--;
        
        RefreshInventoryUI();
        UpdateUI();
    }
    private void OnPageDown()
    {
        if(currentPage>=maxPage) return;

        currentPage++;

        RefreshInventoryUI();
        UpdateUI();
    }

    private int GetPageByIndex(int startIndex)
    {
        return (startIndex - ultimateStartIndex) / eachPageAmount + 1;
    }
    private int GetIndexByPage(int page)
    {
        return ultimateStartIndex + (page - 1) * eachPageAmount;
    }
    private void ShortCutControl()
    {
        if (InputManager.Instance.PageUpInput)
        {
            OnPageUp();
        }
        if (InputManager.Instance.PageDownInput)
        {
            OnPageDown();
        }
    }
}
