using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.VolumeComponent;
using UnityEngine.InputSystem;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class TimeManager : Singleton<TimeManager>
{
    public float timeSpeed = 1.0f;

    public int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    public Season GameSeason
    {
        get{
            Season gameSeason;
            if (gameMonth < 3)
            {
                gameSeason = Season.Winter;
            }
            else if (gameMonth < 6)
            {
                gameSeason = Season.Spring;
            }
            else if (gameMonth < 9)
            {
                gameSeason = Season.Summer;
            }
            else if (gameMonth < 12)
            {
                gameSeason = Season.Autumn;
            }
            else
            {
                gameSeason = Season.Winter;
            };
            return gameSeason;
        }
    }
    private float tikTime = 0;
    private IntelligentBool isGameTimePause = new IntelligentBool(false);
    public bool IsGameTimePause
    {
        get { return isGameTimePause.GetValue; }
    }

    private int secondThreshold = 60;
    private int minuteThreshold = 60;
    private int hourThreshold = 24;

    private List<int> DayThresholdList
    {
        get
        {
            if (DateTime.IsLeapYear(gameYear))
            {
                return new List<int>() { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            }
            else
            {
                return new List<int>() { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            }
        }
    }

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.UpdateGameRunningStateEvent += OnUpdateGameRunningStateEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        EventHandler.UpdateGameRunningStateEvent -= OnUpdateGameRunningStateEvent;
    }

    private void OnBeforeSceneUnloadEvent(GameSceneSO sO, bool isLoadData)
    {
        isGameTimePause.SetValue(true, "SceneLoad");
    }

    private void OnAfterSceneLoadEvent(bool arg1, bool arg2)
    {
        isGameTimePause.SetValue(false, "SceneLoad");
    }

    private void OnUpdateGameRunningStateEvent(GameRunningState state, string key)
    {

        isGameTimePause.SetValue(state == GameRunningState.Pause, key);
    }

    void Start()
    {
        gameSecond = 0;
        gameMinute = 0;
        gameHour = 0;
        gameDay = 1;
        gameMonth = 1;
        gameYear = 2024;
    }

    void Update()
    {
        if (!isGameTimePause.GetValue && !SceneLoadManager.Instance.isSceneLoading)
        {
            tikTime += Time.deltaTime;
            while (tikTime >= Settings.gameTimeThreshold/timeSpeed)
            {
                tikTime -= Settings.gameTimeThreshold/timeSpeed;
                UpdateTime();
            }
        }

        //FIXME: Debug Time
        TimeCheat();
    } 

    private void UpdateTime()
    {
        gameSecond++;
        if(gameSecond >= secondThreshold)
        {
            gameMinute++;
            gameSecond = 0;
            if(gameMinute >= minuteThreshold)
            {
                gameHour++; 
                gameMinute = 0;
                if(gameHour >= hourThreshold)
                {
                    gameDay++;
                    gameHour = 0;
                    if (gameDay >= DayThresholdList[gameMonth - 1])
                    {
                        gameMonth++;
                        gameDay = 1;
                        if (gameMonth >= 12)
                        {
                            gameYear++;
                            gameMonth = 1;
                        }
                    }
                    EventHandler.CallGameDayEvent(gameDay, GameSeason);
                }
                EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, GameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, GameSeason);
        }
    }

    //FIXME: Debug Time
    private void TimeCheat()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            gameDay++;
            gameHour = 0;
            if (gameDay >= DayThresholdList[gameMonth - 1])
            {
                gameMonth++;
                gameDay = 1;
                if (gameMonth >= 12)
                {
                    gameYear++;
                    gameMonth = 1;
                }
            }
            EventHandler.CallGameDayEvent(gameDay, GameSeason);
            EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, GameSeason);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            gameHour++;
            gameMinute = 0;
            if (gameHour >= hourThreshold)
            {
                gameDay++;
                gameHour = 0;
                if (gameDay >= DayThresholdList[gameMonth - 1])
                {
                    gameMonth++;
                    gameDay = 1;
                    if (gameMonth >= 12)
                    {
                        gameYear++;
                        gameMonth = 1;
                    }
                }
                EventHandler.CallGameDayEvent(gameDay, GameSeason);
            }
            EventHandler.CallGameHourEvent(gameHour, gameDay, gameMonth, gameYear, GameSeason);
        }
    }


    public TimeSpan GetDawnTime()
    {
        switch (GameSeason)
        {
            case Season.Spring:
                return Settings.normalDawn;
            case Season.Summer:
                return Settings.summerDawn;
            case Season.Autumn:
                return Settings.normalDawn;
            case Season.Winter:
                return Settings.winterDawn;
        }

        return Settings.normalDawn;
    }
    public TimeSpan GetDuskTime()
    {
        switch (GameSeason)
        {
            case Season.Spring:
                return Settings.normalDusk;
            case Season.Summer:
                return Settings.summerDusk;
            case Season.Autumn:
                return Settings.normalDusk;
            case Season.Winter:
                return Settings.winterDusk;
        }

        return Settings.normalDusk;
    }
    
    public TimeSaveData GetTimeSaveData()
    {
        TimeSaveData timeSaveData = new TimeSaveData();
        timeSaveData.gameSecond = gameSecond;
        timeSaveData.gameMinute = gameMinute;
        timeSaveData.gameHour = gameHour;
        timeSaveData.gameDay = gameDay;
        timeSaveData.gameMonth = gameMonth;
        timeSaveData.gameYear = gameYear;
        return timeSaveData;
    }

    public void RestoreTimeSaveData(TimeSaveData timeSaveData)
    {
        gameSecond = timeSaveData.gameSecond;
        gameMinute = timeSaveData.gameMinute;
        gameHour = timeSaveData.gameHour;
        gameDay = timeSaveData.gameDay;
        gameMonth = timeSaveData.gameMonth;
        gameYear = timeSaveData.gameYear;
        
    }

    public string GetDateString(int gameMonth, int gameDay, int gameYear)
    {
        return Settings.monthStringList[gameMonth - 1] + " " + gameDay.ToString() + ", " + gameYear.ToString();
    }
}

public class TimeSaveData
{
    public int gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
}

