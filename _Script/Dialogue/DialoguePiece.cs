using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Farm.Dialogue{
    [System.Serializable]
    public class DialoguePiece
    {
        public string id;
        public int index;
        public string characterName;
        public Sprite characterSprite;
        public bool onLeft;
        [TextArea]
        public string text;
        public Sprite sprite;
        [HideInInspector]public bool isDone;
        public bool hasToPause;

        public UnityEvent afterTalkEvent;

    }
}