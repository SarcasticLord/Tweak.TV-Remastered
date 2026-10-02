using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
    public bool isTrapped; //Should prevent player from moving
    public bool isPoisoned; //Should put a weird screen effect on that makes it hard to see
    public bool drankCoffee; //Should allow player to dash
    public bool isDead; //Should open game over screen
    public float storeDefaultSpeed;
    public float stunnedSpeed;
    public float storeDefaultJumpForce;
    public float stunnedJumpForce;
    public PlayerMovementScript playerMovementScript;

    void Start()
    {
        playerMovementScript=GetComponent<PlayerMovementScript>();
        storeDefaultSpeed=playerMovementScript.playerSpeed;
        storeDefaultJumpForce=playerMovementScript.playerJumpForce;

    }
    [ContextMenu(nameof(OnStun))]
    public void OnStun()
    {
        playerMovementScript.playerSpeed=stunnedSpeed;
        playerMovementScript.playerJumpForce=stunnedJumpForce;

    }
    [ContextMenu(nameof(OnRemoveStun))]
    public void OnRemoveStun()
    {
        playerMovementScript.playerSpeed=storeDefaultSpeed;
        playerMovementScript.playerJumpForce=storeDefaultJumpForce;

    }
}
