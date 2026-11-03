using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : BaseItem
{
    public Light flashlight;
    protected override void OnUseItem(InputAction.CallbackContext use)
    {
        if (isSelected && use.ReadValueAsButton())
        {
            Debug.Log("Input read");
            flashlight.enabled = !flashlight.enabled;
        }
    }
}
