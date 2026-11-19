using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    //public int uses;
    public BaseItem item;
    public bool limitedUses;

    private void Awake()
    {
        icon.sprite = item.itemData.icon;
    }


}
