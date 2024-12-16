using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class MainMenuPanel : UISingleton<MainMenuPanel>
{
    public Button newGameBtn;
    public Button loadGameBtn;
    public Button exitBtn;
    public Button quitLoadingBtn;

    public GameObject gamePanel;
    public GameObject gamePanelContent;
    
    public GameObject instructionPanel;
    public GameObject settingPanel;

    public TabPanels tabPanels;

    private void Awake()
    {
        newGameBtn.onClick.AddListener(OnNewGameBtnClick);
        loadGameBtn.onClick.AddListener(OnLoadGameBtnClick);
        exitBtn.onClick.AddListener(OnExitBtnClick);

        for (int i = 0; i < tabPanels.tabs.Length; i++)
        {
            tabPanels.tabs[i].onClick.AddListener(QuitSavePanel);
        }

        quitLoadingBtn.onClick.AddListener(QuitSavePanel);
        tabPanels.Initialize();
        QuitSavePanel();

    }

    private void GoToSavePanel()
    {
        SaveLoadPanel.Instance.dataSlotContainer.UpdateUI();
        SaveLoadPanel.Instance.IsOpen = true;
        SaveLoadPanel.Instance.ShowLoadPanelOnMainMenu();
        gamePanelContent.SetActive(false);
        quitLoadingBtn.gameObject.SetActive(true);
    }
    private void QuitSavePanel()
    {
        SaveLoadPanel.Instance.IsOpen = false;
        gamePanelContent.SetActive(true);
        quitLoadingBtn.gameObject.SetActive(false);
    }

    private void OnNewGameBtnClick()
    {
        EventHandler.CallSceneLoadEvent(SceneLoadManager.Instance.startScene, false);
        GameManager.Instance.PlayerTransform.position = Settings.playerInitialPos;
    }

    private void OnLoadGameBtnClick()
    {
        GoToSavePanel();
    }

    private void OnExitBtnClick()
    { 
        Application.Quit();
    }


}
