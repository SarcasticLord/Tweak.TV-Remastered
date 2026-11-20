using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInventoryManager : MonoBehaviour
{
    public InputActionReference itemSelect;

    public List<GameObject> items = new List<GameObject>();
    public List<GameObject> itemSlots = new List<GameObject>();
    public int maxItems = 3;
    private int selectedItem;

    public GameObject hotbar;

    public GameObject itemSlotPrefab;

    public Sprite emptySlot;
    

    private void OnEnable()
    {
        itemSelect.action.performed += OnSelectItem;
        itemSelect.action.canceled += OnSelectItem;
    }

    private void OnDisable()
    {
        itemSelect.action.performed -= OnSelectItem;
        itemSelect.action.canceled -= OnSelectItem;
    }

    private void Awake()
    {

        if (itemSlotPrefab != null)
        {
            for (int i = 0; i < maxItems; i++)
            {
                GameObject newSlot = Instantiate(itemSlotPrefab);
                newSlot.transform.parent = hotbar.transform;
            }

            foreach (Transform slot in hotbar.transform)
            {
                Image slotIcon = slot.transform.GetChild(0).GetComponent<Image>();
                slotIcon.sprite = emptySlot;
                itemSlots.Add(slot.gameObject);
            }
        }
    }

    private void Update()
    {
        //cycles through each item in the inventory
        foreach (GameObject item in items)
        {
            //Changes slot item icon image to match item in inventory
            int itemIndex = items.IndexOf(item);
            Image slotIcon = itemSlots[itemIndex].transform.GetChild(0).GetComponent<Image>();
            BaseItem itemScript = item.gameObject.GetComponent<BaseItem>();
            Renderer mesh = item.gameObject.GetComponent<Renderer>();

            //hides items that are not selected
            if (item != items[selectedItem])
            {
                //mesh.enabled = false;
                //itemScript.isSelected = false;
                item.SetActive(false);
            }

        }
    }


    //Switches betweens items
    private void OnSelectItem(InputAction.CallbackContext select)
    {
        if (select.ReadValueAsButton())
        {
            CycleItems();
        }
    }

    public void Drop(GameObject droppedItem)
    {
        int itemIndex = items.IndexOf(droppedItem);
        Debug.Log(itemIndex);
        Image slotIcon = itemSlots[itemIndex].transform.GetChild(0).GetComponent<Image>();
        RefreshHUDInventory();
    }


    public void CycleItems()
    {
        if (items.Count > 0)
        {
            selectedItem++;
            //ensures selected item stays in range of items in inventoryem
            if (selectedItem >= items.Count)
            {
                selectedItem = 0;
            }
            BaseItem itemScript = items[selectedItem].gameObject.GetComponent<BaseItem>();
            Renderer mesh = items[selectedItem].gameObject.GetComponent<Renderer>();
            //mesh.enabled = true;
            itemScript.isSelected = true;
            items[selectedItem].SetActive(true);
            Debug.Log(selectedItem);
        }
    }


    public void RefreshHUDInventory()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            Image icon = itemSlots[i].transform.GetChild(0).GetComponent<Image>();

            if (i < items.Count)
            {
                BaseItem item = items[i].GetComponent<BaseItem>();
                icon.sprite = item.itemData.icon;
            }
            else
            {
                icon.sprite = emptySlot;
            }
        }
    }
}
