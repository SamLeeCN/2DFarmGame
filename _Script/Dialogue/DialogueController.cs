using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.Dialogue{

    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        NPCMovement npcMovement;
        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();
        public Stack<DialoguePiece> dialogueStack;
        
        public UnityEvent OnFinishEvent;
        public bool canTalk;
        public bool isTalking;

        private GameObject uiSign;
        void Start()
        {
            npcMovement = GetComponent<NPCMovement>();
        }
        private void Awake()
        {
            FillDialogueStack();
            uiSign = transform.Find("UISign").gameObject;
        }
        void Update()
        {
            uiSign.SetActive(canTalk && !isTalking);
            if (InputManager.Instance.TalkWithNPCInput && canTalk && !isTalking && !BagPanel.Instance.IsOpen)
            {
                StartCoroutine(DialogueRoutine());
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = !npcMovement.isMovingAnimation && npcMovement.isInteractable;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                canTalk = false;
            }
        }
        public void FillDialogueStack()
        {
            dialogueStack = new Stack<DialoguePiece>();
            for (int i = dialogueList.Count - 1; i >= 0 ; i--)
            {
                dialogueList[i].isDone = false;
                dialogueStack.Push(dialogueList[i]);
            }

        }

        private IEnumerator DialogueRoutine()
        {
            isTalking = true;
            if (dialogueStack.TryPop(out DialoguePiece result))
            {
                EventHandler.CallDialogueEvent(result);
                yield return new WaitUntil(()=>result.isDone);
                isTalking = false;
            }
            else
            {
                EventHandler.CallDialogueEvent(null);
                FillDialogueStack(); // Reset the stack when the dialogue panel close
                isTalking = false;
                OnFinishEvent?.Invoke();
            }
        }
    }
}