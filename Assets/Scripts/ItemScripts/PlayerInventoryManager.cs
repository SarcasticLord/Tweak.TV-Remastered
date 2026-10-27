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
        foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
        {
            if (child != gameObject.transform && !items.Contains(child.gameObject) && items.Count < maxItems) // Exclude the parent itself
            {
                items.Add(child.gameObject);
            }
        }

        foreach (GameObject item in items)
        {
            if (item != items[selectedItem])
            {
               item.gameObject.GetComponent<ItemHandler>().isSelected = false;
               item.SetActive(false);
            }
        }
    }



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
            if (selectedItem >= items.Count)
            {
                selectedItem = 0;
            }
            items[selectedItem].gameObject.GetComponent<ItemHandler>().isSelected = true;
            items[selectedItem].SetActive(true);
            Debug.Log(selectedItem);
        }
    }
}
