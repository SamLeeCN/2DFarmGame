using Farm.InventoryNamespace;
using Farm.SaveLoad;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class DataPageFliper : MonoBehaviour
{
    [SerializeField] private DataSlotContainer dataSlotContainer;
    private Dictionary<int, DataSlotDataDetails> DataDetailDataDict => SaveDataManager.Instance.dataSlotDataDict;
    public int CurrentMaxSlotIndex => SaveDataManager.Instance.CurrentMaxSlotIndex;
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
    void Start()
    {
        pageUpBtn.onClick.AddListener(OnPageUp);
        pageDownBtn.onClick.AddListener(OnPageDown);
    }

    void Update()
    {
        ShortCutControl();
    } 

    public void SetUpUI(DataSlotContainer dataSlotContainer, int startIndex, int low, int high, bool isSaveMode)
    {
        gameObject.SetActive(true);

        this.dataSlotContainer = dataSlotContainer;
        eachPageAmount = this.dataSlotContainer.slots.Count;
        ultimateStartIndex = low == -1 ? 0 : low;
        ultimateEndIndex = high == -1 ? CurrentMaxSlotIndex : high;

        if (isSaveMode)
            totalAmount = CurrentMaxSlotIndex + 2;
        else
            totalAmount = CurrentMaxSlotIndex + 1;

        maxPage = (totalAmount - 1) / eachPageAmount + 1;
        if (maxPage <= 1) gameObject.SetActive(false);

        currentPage = GetPageByIndex(startIndex);

        UpdateUI();

        if (currentPage > maxPage) OnPageUp();
    }

    private int GetPageByIndex(int startIndex)
    {
        return (startIndex - ultimateStartIndex) / eachPageAmount + 1;
    }

    private int GetIndexByPage(int page)
    {
        return ultimateStartIndex + (page - 1) * eachPageAmount;
    }

    public void UpdateUI()
    {
        pageTxt.text = currentPage.ToString() + "/" + maxPage.ToString();
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

    private void OnPageUp()
    {
        if (currentPage <= 1) return;

        currentPage--;

        RefreshContainerUI();
        UpdateUI();
    }
    private void OnPageDown()
    {
        if (currentPage >= maxPage) return;

        currentPage++;

        RefreshContainerUI();
        UpdateUI();
    }

    private void RefreshContainerUI()
    {
        dataSlotContainer.SetUpUI(GetIndexByPage(currentPage));
    }
}
