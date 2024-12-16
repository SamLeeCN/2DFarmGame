
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 

public enum ItemType
{
    None,
    Seed,Commodity,Furniture,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,
    ReapableScenery
}
public enum InventoryType
{
    Single,Player
}

public enum SlotType
{
    PlayerBag,PlayerActionBar,NPCBag,Chest,Shop,Display,Craft
}
public enum PlayerActionEnum
{
    Default, Carry, Hoe, Break, Water, Chop, Reap, Collect
}
public enum PlayerPartEnum
{
    Body, Hair, Arm, Tool
}

public enum Season
{
    Spring, Summer, Autumn, Winter
}

public enum SceneType
{
    Location, Menu, Inventoy
}
public enum TeleportType
{
    Trans, Pos
}
public enum PersistentType
{
    ReadWrite, DoNotPersist
}

public enum GridType { 
    Diggable, DropItem, PlaceFurniture, NpcObstacle
}


public enum ParticleEffectType
{
    Tree01LeavesFalling, Tree02LeavesFalling, RockBreak
}

public enum GameRunningState
{
    GamePlay, Pause
}


public enum ButtonType
{
    Key, Mouse, Controller
}

public enum LightShift
{
    Dawn, Noon, Dusk, MidNight
}

public enum Weather
{
    Sunny, Cloudy, Rainy, Snowy
}

public enum SoundName
{
    None, FootStepSoft, FootStepHard,
    Axe, Pickaxe, Hoe, Reap, Water, Basket, Chop,
    PickUp, Plant, TreeFalling, Rustle,
    AmbientCountryside1, AmbientCountryside2, AmbientIndoor1,
    //BGM
    MusicCalm1, MusicCalm3, MusicOutside
}

public enum NPCName
{
    None, Mayor, Girl01
}