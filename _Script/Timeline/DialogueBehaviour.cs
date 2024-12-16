using Farm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
[System.Serializable]
public class DialogueBehaviour : PlayableBehaviour
{
    private PlayableDirector director;
    public DialoguePiece dialoguePiece;
    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        EventHandler.CallDialogueEvent(dialoguePiece);
        if (Application.isPlaying)
        {
            if (dialoguePiece.hasToPause)
            {
                TimelineManager.Instance.PauseTimeline(director);
            }
            else
            {
                EventHandler.CallDialogueEvent(null);
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (Application.isPlaying)
        {
            TimelineManager.Instance.isDone = dialoguePiece.isDone;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        EventHandler.CallDialogueEvent(null);
    }

    public override void OnGraphStart(Playable playable)
    {
        EventHandler.CallUpdateGameRunningStateEvent(GameRunningState.Pause, "Graph");
        if (TimelineManager.Instance.doMuteBGM)
        {
            AudioManager.Instance.noBGMSnapshot.TransitionTo(Settings.bgmMutingDuration);
        }
    }
    public override void OnGraphStop(Playable playable)
    {
        EventHandler.CallUpdateGameRunningStateEvent(GameRunningState.GamePlay, "Graph");
        AudioManager.Instance.normalSnapshot.TransitionTo(Settings.bgmMutingDuration);
    }
}
