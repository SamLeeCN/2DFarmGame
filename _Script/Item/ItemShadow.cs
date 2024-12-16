using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemShadow : MonoBehaviour
    {
        public SpriteRenderer itemSpriteRenderer;
        public SpriteRenderer shadowSpriteRenderer;

        private void Awake()
        {
            shadowSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            shadowSpriteRenderer.sprite = itemSpriteRenderer.sprite;
            shadowSpriteRenderer.color = new Color(0, 0, 0, 0.3f);
        }

        void Update()
        {

        }
    }
}