using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//*****************************************
//Creator: SamLee 
//Description: 
//***************************************** 
namespace Farm.InventoryNamespace{
    public class Chest : MonoBehaviour, IPointerClickHandler
    {

        [SerializeField] private InventoryDataSO chestDataTemplate;
        public InventoryDataSO chestData;

        public bool isOpen = false;

        public int index = -1;
        private void OnEnable()
        {
            isOpen = false;
        }
        void Start()
        {

        }

        void Update()
        {

        }



        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {

            if (eventData.button != PointerEventData.InputButton.Right) return;

            if (isOpen) return;

            UIManager.Instance.OpenChestPanel(chestData, this);
            
        }


        public void InitChest(int InitIndex = -1)
        {
            index = InitIndex;
            if (chestData == null)
                chestData = Instantiate(chestDataTemplate);
            if (index == -1)
            {
                index = InventoryManager.Instance.GenerateAvailableIndex();
                InventoryManager.Instance.AddChestData(this);
            }
            else
            {
                chestData.items = InventoryManager.Instance.GetChest(index);
            }
        }
    }
}