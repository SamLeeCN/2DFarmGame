using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class TabPanels : MonoBehaviour
{
    public GameObject[] panels;
    public Button[] tabs;
    public GameObject currentShowingPanel;
    public bool isInitialized = false;

    private void Awake()
    {
        if (!isInitialized) Initialize();
    }

    public void Initialize()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            int index = i;
            tabs[i].onClick.AddListener(() => ShowPanel(index));
        }

        currentShowingPanel = panels[0];
        ShowPanel(0);
        isInitialized = true;
    }

    public void ShowPanel(int index)
    {
        panels[index].transform.SetAsLastSibling();
        currentShowingPanel = panels[index];
    }

}
