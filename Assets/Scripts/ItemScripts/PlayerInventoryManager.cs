using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventoryManager : MonoBehaviour
{
    public InputActionReference itemSelect;

    public List<GameObject> items = new List<GameObject>();
    public int maxItems = 3;
    private int selectedItem;

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



    private void LateUpdate()
    {
        //cycles through each child of the gameobject and adds it to items 
        foreach (BaseItem child in gameObject.GetComponentsInChildren<BaseItem>())
        {
            if (child != gameObject.transform && !items.Contains(child.gameObject) && items.Count < maxItems) // Exclude the parent itself
            {
                items.Add(child.gameObject);
            }
        }
        //cycles through each item in the inventory and disables items that are not selected
        foreach (GameObject item in items)
        {
            if (item != items[selectedItem])
            {
               BaseItem itemScript = item.gameObject.GetComponent<BaseItem>();
               itemScript.isSelected = false;
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
            itemScript.isSelected = true;
            items[selectedItem].SetActive(true);
            Debug.Log(selectedItem);
        }
    }
}
