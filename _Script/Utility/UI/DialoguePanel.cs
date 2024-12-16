using Farm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
public class DialoguePanel : UISingleton<DialoguePanel>
{
    public TextMeshProUGUI dialogueText;
    public Image faceLeft, faceRight;
    public TextMeshProUGUI nameLeft, nameRight;
    public GameObject continueBox;

    private void OnEnable()
    {
        EventHandler.DialogueEvent += OnDialogueEvent;        
    }

    private void OnDisable()
    {
        EventHandler.DialogueEvent -= OnDialogueEvent;
    }

    private void OnDialogueEvent(DialoguePiece piece)
    {
        StartCoroutine(ShowDialogue(piece));
    }

    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if (piece != null)
        {
            piece.isDone = false;
            
            dialogueText.text = string.Empty;
            IsOpen = true;

            if (piece.characterName != string.Empty)
            {
                if (piece.onLeft)
                {
                    faceLeft.gameObject.SetActive(true);
                    faceRight.gameObject.SetActive(false);
                    faceLeft.sprite = piece.characterSprite;
                    nameLeft.text = piece.characterName;
                }
                else
                {
                    faceLeft.gameObject.SetActive(false);
                    faceRight.gameObject.SetActive(true);
                    faceRight.sprite = piece.characterSprite;
                    nameRight.text = piece.characterName;
                }
            }
            else
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }

            string completeString = piece.text;
            int textLen = completeString.Length;

            float eachCharWaitTime = Settings.dialogueTextCompleteTime / textLen;

            for (int i = 0; i < textLen; i++)
            {
                dialogueText.text = completeString.Substring(0, i + 1);
                yield return new WaitForSeconds(eachCharWaitTime);
            }

            piece.isDone = true;

            if (piece.hasToPause && piece.isDone)
                continueBox.SetActive(true);
        }
        else
        {
            // When passing the parameter null, the chat panel would close
            IsOpen = false;
            yield break;
        }
    }

    private void Awake()
    {
        continueBox.SetActive(false);
    }
}
