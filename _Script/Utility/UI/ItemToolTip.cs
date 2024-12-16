using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace
{
    public class ItemToolTip : MonoBehaviour
    {
        public RectTransform rectTransform;
        [Header("UI Field")]
        [SerializeField] private TextMeshProUGUI itemNameTxt;
        [SerializeField] private TextMeshProUGUI itemTypeTxt;
        [SerializeField] private TextMeshProUGUI descriptionTxt;
        [SerializeField] private TextMeshProUGUI priceTxt;
        [SerializeField] private GameObject bottomPart;

        [SerializeField] private GameObject requireItemDisplay;
        [SerializeField] private List<SlotUI> requireItemSlots;


        Vector3[] corners = new Vector3[4];
        Vector2 mousePos;

        [SerializeField] private float cornerOffset = 20;//Depends on the size of your cursor
        private Vector3 cumulativeScale;

        public void SetUpUI(ItemDetail itemDetail)
        {   
            gameObject.SetActive(true);

            itemNameTxt.text = itemDetail.itemName;
            itemTypeTxt.text = itemDetail.itemType.ToString();
            descriptionTxt.text = itemDetail.description;
            if (itemDetail.itemType == ItemType.Seed|| itemDetail.itemType == ItemType.Commodity|| itemDetail.itemType == ItemType.Furniture)
            {   
                bottomPart.SetActive(true);
                string buyingPrice = itemDetail.price.ToString();
                string sellingPrice = itemDetail.SellPrice.ToString();
                priceTxt.text = buyingPrice+ "/" + sellingPrice;
            }
            else
            {
                bottomPart.SetActive(false);
            }
            if (itemDetail.itemType == ItemType.Furniture){
                SetUpRequireItemsUI(itemDetail);
                requireItemDisplay.SetActive(true);
            }
            else
                requireItemDisplay.SetActive(false);


            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            UpdatePos();
        }
        private void UpdatePos()
        {
            cumulativeScale = ExtensionMethod.GetCumulativeScale(rectTransform);
            mousePos = Input.mousePosition;
            //Get 4 corners(from left buttom, clockwise) of the gameObject
            rectTransform.GetWorldCorners(corners);

            float width = (corners[2].x - corners[1].x);
            float height = (corners[1].y - corners[0].y);

            bool isLeftToTheScreen = mousePos.x < width;
            bool isRightToTheScreen = Screen.width - mousePos.x < width;
            bool isTopToTheScreen = Screen.height - mousePos.y < height;
            bool isButtomToTheScreen = mousePos.y < height;
            Vector2 anchorPosBeforeScale;
            if (isLeftToTheScreen && isTopToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);
            else if (isTopToTheScreen && !isRightToTheScreen && !isLeftToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);
            else if (isRightToTheScreen && isTopToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(2);
            else if (isLeftToTheScreen && !isTopToTheScreen && !isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);
            else if (isRightToTheScreen && !isTopToTheScreen && !isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(2);
            else if (isLeftToTheScreen && isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(0);
            else if (isButtomToTheScreen && !isLeftToTheScreen && !isRightToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(0);
            else if (isRightToTheScreen && isButtomToTheScreen) anchorPosBeforeScale = CalculateAnchorPosBeforeScale(3);
            else anchorPosBeforeScale = CalculateAnchorPosBeforeScale(1);

            rectTransform.anchoredPosition = anchorPosBeforeScale / cumulativeScale;
        }
        private Vector2 CalculateAnchorPosBeforeScale(int cornerIndex)
        {
            Vector2 anchorOffsetFromCorner = rectTransform.anchoredPosition * cumulativeScale - GetCornerAfterOffset(cornerIndex);
            return mousePos + anchorOffsetFromCorner;
        }
        private Vector2 GetCornerAfterOffset(int cornerIndex)
        {
            float x = corners[cornerIndex].x;
            float y = corners[cornerIndex].y;
            //depends on how your cursor looks
            //for most cursors only case 1 is needed
            switch (cornerIndex)
            {
                case 0:

                    break;
                case 1:
                    x += -cornerOffset;
                    y += cornerOffset;
                    break;
                case 2:

                    break;
                case 3:

                    break;
            }
            return new Vector2(x, y);
        }

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
        }


        private void SetUpRequireItemsUI(ItemDetail itemDetail)
        {
            BluePrintDetails bluePrintDetails = InventoryManager.Instance.GetBluePrintDetails(itemDetail.itemId);

            for (int i = 0; i < bluePrintDetails.requireItems.Length; i++)
            {
                
                requireItemSlots[i].SetUpDisplayItem(bluePrintDetails.requireItems[i].itemId, bluePrintDetails.requireItems[i].amount);
            }
        }
    }
}

