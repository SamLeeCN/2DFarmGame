using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class TeleportEntrance : MonoBehaviour, IInteractable
{//Attach on the point to start teleport
    public TeleportType teleportType = TeleportType.Trans;
    public bool isEnterable = true;
    public bool isSameSceneTeleport = true;
    public GameSceneSO sceneToGo;
    [Header("TransTeleport")]
    [Range(0, 20)] public int teleportDestinationIdToGo;
    [Header("PosTeleport")]
    public Vector3 posToGo;
    public void TriggerAction()
    {
        if (isEnterable)
        {
            SceneLoadManager.Instance.TeleportFromEntrance(this);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.playerControler.currentInteractable = GetComponent<IInteractable>();
            GameManager.Instance.playerControler.isInInteractArea = true;
            //TODO:KeyPrompt
            //KeyPrompt.Instance.AddKeyPrompt(InputManager.Instance.interactAction);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.playerControler.isInInteractArea = false;
            //TODO:KeyPrompt
            //KeyPrompt.Instance.DeleteKeyPrompt(InputManager.Instance.interactAction);
        }
    }

}
