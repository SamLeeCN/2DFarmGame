using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[System.Serializable]
public class ScheduleDetails : IComparable<ScheduleDetails>
{
    public int hour, minute, day;
    public int priority;
    public Season season;
    public string targetScene;
    public Vector2Int targetGridCoordinate;
    public AnimationClip animationClipAtStop;
    public bool isInteractable;

    public int Time => (hour * 60) + minute;

    public bool isSpecial = false;

    public int delayFromCurrentTime = 1;

    public ScheduleDetails(int hour, int minute, int day, int priority, Season season, string targetScene, Vector2Int targetGridCoordinate, AnimationClip animationClipAtStop, bool isInteractable)
    {
        this.hour = hour;
        this.minute = minute;
        this.day = day;
        this.priority = priority;
        this.season = season;
        this.targetScene = targetScene;
        this.targetGridCoordinate = targetGridCoordinate;
        this.animationClipAtStop = animationClipAtStop;
        this.isInteractable = isInteractable;
    }

    public ScheduleDetails(ScheduleDetails schedule)
    {
        hour = schedule.hour;
        minute = schedule.minute;
        day = schedule.day;
        priority = schedule.priority;
        season = schedule.season;
        targetScene = schedule.targetScene;
        targetGridCoordinate = schedule.targetGridCoordinate;
        animationClipAtStop = schedule.animationClipAtStop;
        isInteractable = schedule.isInteractable;
        isSpecial = schedule.isSpecial;
        delayFromCurrentTime = schedule.delayFromCurrentTime;
        targetGridCoordinate = schedule.targetGridCoordinate;
        animationClipAtStop = schedule.animationClipAtStop;
    }
    #region Save and Load
    public ScheduleDetails(ScheduleDetaildsSaveData saveData)
    {
        hour = saveData.hour;
        minute = saveData.minute;
        day = saveData.day;
        priority = saveData.priority;
        season = (Season)saveData.season;
        targetScene = saveData.targetScene;
        targetGridCoordinate = new Vector2Int(saveData.targetGridCoordinateX, saveData.targetGridCoordinateY);
        //animationClipAtStop = (AnimationClip)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(saveData.animationClipAtStopGUID), typeof(AnimationClip));
        isInteractable = saveData.isInteractable;
        isSpecial = saveData.isSpecial;
        delayFromCurrentTime = saveData.delayFromCurrentTime;
    }
   
    public ScheduleDetaildsSaveData GetSaveData()
    {
        ScheduleDetaildsSaveData saveData = new ScheduleDetaildsSaveData();
        saveData.hour = hour;
        saveData.minute = minute;
        saveData.day = day;
        saveData.priority = priority;
        saveData.season = (int)season;
        saveData.targetScene = targetScene;
        saveData.targetGridCoordinateX = targetGridCoordinate.x;
        saveData.targetGridCoordinateY = targetGridCoordinate.y;
        //saveData.animationClipAtStopGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animationClipAtStop));
        saveData.isInteractable = isInteractable;
        saveData.isSpecial = isSpecial;
        saveData.delayFromCurrentTime = delayFromCurrentTime;
        return saveData;
    }
    #endregion
    public int CompareTo(ScheduleDetails other)
    {
        if (Time == other.Time)
        {
            if (priority > other.priority)
            {
                return 1;
            }
            else if (priority < other.priority)
            {
                return -1;
            }
            else
            {
                if (day < other.day)
                {
                    return -1;
                }
                else if (day > other.day)
                {
                    return 1;
                }
                else
                {
                    if (season < other.season)
                    {
                        return -1;
                    }
                    else if (season > other.season)
                    {
                        return 1;
                    }
                }
            }
        }
        else if (Time < other.Time)
        {
            return -1;
        }
        else if (Time > other.Time)
        {
            return 1; 
        }

        return 0;
    }

    public void UpdateStartTime()
    {
        if (!isSpecial) return;
        TimeSpan time = TimeManager.Instance.GameTime + new TimeSpan(0, delayFromCurrentTime, 0);
        hour = time.Hours;
        minute = time.Minutes;
    }
    
}

public class ScheduleDetaildsSaveData
{
    public int hour, minute, day;
    public int priority;
    public int season;
    public string targetScene;
    public int targetGridCoordinateX, targetGridCoordinateY;
    public int animationInstanceID;
    public bool isInteractable;
    public bool isSpecial = false;
    public int delayFromCurrentTime = 1;
}
