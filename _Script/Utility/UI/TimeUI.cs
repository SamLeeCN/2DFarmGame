using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class TimeUI : MonoBehaviour
{
    [Header("UI Field")]
    [SerializeField] private Button settingBtn;
    [SerializeField] private TextMeshProUGUI dateTxt;
    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private List<Image> litUpTimerPieces;
    [SerializeField] private GameObject dayTimeRotateIcon;
    [SerializeField] private Image seasonImage;
    [Header("Resource")]
    [SerializeField] private List<Sprite> seasonIconSprites;
    
    private int GameSecond => TimeManager.Instance.gameSecond;
    private int GameMinute => TimeManager.Instance.gameMinute;
    private int GameHour => TimeManager.Instance.gameHour;
    private int GameDay => TimeManager.Instance.gameDay;
    private int GameMonth => TimeManager.Instance.gameMonth;
    private int GameYear => TimeManager.Instance.gameYear;

    private Season GameSeason => TimeManager.Instance.GameSeason;

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        settingBtn.onClick.AddListener(OnSettingBtnClick);
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        settingBtn.onClick.RemoveListener(OnSettingBtnClick);
    }

    private void OnSettingBtnClick()
    {
        SaveLoadPanel.Instance.ToggeleSaveLoadPanel();
    }

    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        UpdateUI();
    }


    public void UpdateUI()
    {
        timeTxt.text = GameHour.ToString("00") + ":" + GameMinute.ToString("00");
        float dayPercentage = (GameHour + (float)GameMinute / 60) / 24;
        Vector3 rotateTarget = new Vector3(0, 0, -90 + dayPercentage * 360);
        dayTimeRotateIcon.transform.DORotate(rotateTarget, 0.2f);



        int partTimeHour = GameHour - (GameHour / 6) * 6;
        for (int i = 0; i < 6; i++)
        {
            if (i <= partTimeHour)
            {
                litUpTimerPieces[i].enabled = true;
                litUpTimerPieces[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                litUpTimerPieces[i].enabled = false;
            }
            if (i == partTimeHour)
            {
                litUpTimerPieces[i].color = new Color(1, 1, 1, (float)GameMinute / 60);
            }
        }

        dateTxt.text = TimeManager.Instance.GetDateString(GameMonth, GameDay, GameYear);
        seasonImage.sprite = seasonIconSprites[(int)GameSeason];
    }
}
