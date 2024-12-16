using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    public class ItemBounce : MonoBehaviour
    {
        [SerializeField]private Transform spriteTrans;
        private BoxCollider2D coll;
        public float gravity = -3.5f;
        private bool isGround;
        private float distance;
        private Vector2 direction;
        private Vector3 targetPos;


        private void Awake()
        {
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }


        public void InitBoucnceItem(Vector3 target, Vector2 direction)
        {
            coll.enabled = false;
            targetPos = target;
            this.direction = direction;
            distance = Vector3.Distance(targetPos, transform.position);
            spriteTrans.position += Vector3.up * 1.5f;
        }

        

        void Start()
        {

        }

        void Update()
        {
            Bounce();
        }

        private void Bounce()
        {
            isGround = spriteTrans.position.y <= transform.position.y;
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
            }

            if (!isGround)
            {
                spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                spriteTrans.position = transform.position;
                coll.enabled = true;
            }
        }
    }
}