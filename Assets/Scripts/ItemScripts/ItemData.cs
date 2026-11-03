using System.ComponentModel;
using Unity;
using UnityEngine;


public enum ItemType
{
    Weapon,
    Consumable,
    LightSource,
    Tool,
}


[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class ItemData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public int uses;
    public ItemType itemType;
    public float offset;

    public GameObject prefab;

}
