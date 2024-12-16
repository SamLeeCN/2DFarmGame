using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class SaveLoadPanel : UISingleton<SaveLoadPanel>
{
    public Button savePanelBtn;
    public Button loadPanelBtn;
    public GameObject savePanel;
    public GameObject loadPanel;
    public TabPanels tabPanels;
    public DataSlotContainer dataSlotContainer;

    public GameObject background;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;
    }

    private void OnAfterSceneLoadEvent(bool arg1, bool arg2)
    {
        IsOpen = false;
    }

    private void Awake()
    {
        
        savePanelBtn.onClick.AddListener(() => SetPanel(true));
        loadPanelBtn.onClick.AddListener(() => SetPanel(false));
        tabPanels.Initialize();
        SetPanel(tabPanels.currentShowingPanel == savePanel);
    }

    public void SetPanel(bool isSaveMode)
    {
        dataSlotContainer.isSaveMode = isSaveMode;
        dataSlotContainer.UpdateUI();
    }

    public void ToggeleSaveLoadPanel()
    {
        if (IsOpen) CloseSaveLoadPanel();
        else ShowSaveLoadPanel();
    }

    private void ShowSaveLoadPanel()
    {
        background.SetActive(true);
        SetPanel(tabPanels.currentShowingPanel == savePanel);
        savePanelBtn.gameObject.SetActive(true);
        loadPanelBtn.gameObject.SetActive(true);
        IsOpen = true;
    }

    private void CloseSaveLoadPanel()
    {
        IsOpen = false;
    }

    public void ShowLoadPanelOnMainMenu()
    {
        SetPanel(false);
        background.SetActive(false);
        savePanelBtn.gameObject.SetActive(false);
        loadPanelBtn.gameObject.SetActive(false);
    }
}
