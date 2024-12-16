using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class Settings
{
    public static float fadeDuration = 0.3f;
    public static float sceneItemFadeAlpha = 0.45f;
    public static float pickItemSpeed = 50f;
    public static bool autoPick = true;
    public static float gameTimeThreshold = 0.012f;
    public static int reapAmount = 2;
    public static float gridCellSize = 1f;
    public static float gridCellDiagonalSize = 1.41f;
    public static float pixelSize = 0.05f; // 20*20 per unit
    public static float npcAnimationBreakTime = 3f;
    public static float dialogueTextCompleteTime = 0.5f;
    public static float uiScaleToWorldFactor = 0.6f;
    public static int craftRowNum = 3;
    public static int craftColNum = 3;
    public static int CraftSlotNum => craftRowNum * craftColNum;
    public static float longPressThreshold = 0.2f;
    public static int fastCraftLimitedTimes = 64;
    public static float rightMouseFastShiftInterval = 0.1f;
    public static float doubleClickInterval = 0.1f;


    public static TimeSpan summerDawn = new TimeSpan(5, 0, 0);
    public static TimeSpan winterDawn = new TimeSpan(7, 0, 0);
    public static TimeSpan summerDusk = new TimeSpan(19, 0, 0);
    public static TimeSpan winterDusk = new TimeSpan(17, 0, 0);
    public static TimeSpan noon = new TimeSpan(12, 0, 0);
    public static TimeSpan midNight = new TimeSpan(0, 0, 0);
    public static TimeSpan normalDawn = new TimeSpan(6, 0, 0);
    public static TimeSpan normalDusk = new TimeSpan(18, 0 ,0);
    public static TimeSpan sunOnHorizonTime = new TimeSpan(1, 0, 0);

    public static float bgmMutingDuration = 0.3f;

    public static List<string> monthStringList = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "July", "Aug", "Sept", "Oct", "Nov", "Dec" };

    public static Vector3 playerInitialPos = new Vector3(2, -2, 0);

    
}
