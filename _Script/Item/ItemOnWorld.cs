using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using Farm.CropNamespace;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ItemOnWorld : MonoBehaviour
    {
        public int itemId;
        public int amount;
        
        private ItemDetail itemDetail;
        public ItemDetail CurrentItemDetail { get { return itemDetail; } }
        public bool isInitialized = false;
        public bool isFlyingToPicker = false;
        public bool canPick;
        private Transform pickerTrans;
        [SerializeField]private SpriteRenderer spriteRenderer;
        private BoxCollider2D coll;

        private bool isShowingItemTip;
        void Start()
        {
            coll = GetComponent<BoxCollider2D>();

            if (itemId != 0)
            {
                Init(itemId);
            }
        }

        private void Update()
        {
            GoToPicker();
        }

        private void OnDisable()
        {
            if (isShowingItemTip)
            {
                InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);
                isShowingItemTip = false;
            }
        }

        public virtual void Init(int id)
        {
            coll = GetComponent<BoxCollider2D>();
            itemId = id;
            itemDetail = InventoryManager.Instance.GetItemDetails(itemId);
            canPick = itemDetail.canPick;

            if (itemDetail != null)
            {
                spriteRenderer.sprite = itemDetail.onWorldSprite ? itemDetail.onWorldSprite : itemDetail.inventoryIcon;
            }

            if (itemDetail.itemType == ItemType.ReapableScenery)
            {
                gameObject.AddComponent<ReapItem>();
                gameObject.GetComponent<ReapItem>().InitCropData(CropManager.Instance.GetCropDetails(id));
            }

            Vector2 newSize = new Vector2(spriteRenderer.sprite.bounds.size.x, spriteRenderer.sprite.bounds.size.y);
            coll.size = newSize;
            coll.offset = new Vector2(0, spriteRenderer.sprite.bounds.center.y);
            isInitialized = true;
        }

        public void BePickedUp(int leftAmount,Transform picker) 
        {
            if (leftAmount == amount) return;
            if (leftAmount == 0)
            {
                StartToGoToPicker(picker);
            }
            else
            {
                amount = leftAmount;
                GameObject itemToGoToPicker = Instantiate(gameObject);
                itemToGoToPicker.GetComponent<ItemOnWorld>().StartToGoToPicker(picker);
            }
        }

        private void StartToGoToPicker(Transform picker)
        {
            isFlyingToPicker = true;
            pickerTrans =  picker;
            EventHandler.CallSoundEffectEvent(SoundName.PickUp, transform.position);
        }
        
        private void GoToPicker()
        {
            if (isFlyingToPicker)
            {
                if (Vector3.Distance(transform.position, pickerTrans.position) > 0.2f)
                {
                    Vector3 dir = (pickerTrans.position - transform.position).normalized;
                    transform.position += dir*Settings.pickItemSpeed*Time.deltaTime;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        void OnMouseEnter()
        {
            ShowItemTip();
        }

        void OnMouseOver()
        {
            ShowItemTip();
        }

        void OnMouseExit()
        {
            InventoryManager.Instance.itemToolTip.gameObject.SetActive(false);
            isShowingItemTip = false;
        }

        private void ShowItemTip()
        {
            isShowingItemTip = true;
            InventoryManager.Instance.itemToolTip.SetUpUI(itemDetail);
        }
    }
}

