using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
    public bool isTrapped; //Should prevent player from moving
    public bool isPoisoned; //Should put a weird screen effect on that makes it hard to see
    public bool drankCoffee; //Should allow player to dash
    public bool isDead; //Should open game over screen
    public bool isSlowed; //Slows player speed
    public float slowDuration = 5f;
    
    public float slowedSpeed = 4f;
    public float slowedSlideSpeed = 4f;

    public GameObject slowedEffect; 

    public float storeDefaultSpeed;
    private float storeDefaultSlideSpeed;
    public float stunnedSpeed;
    public float storeDefaultJumpForce;
    public float stunnedJumpForce;
    public PlayerMovementScript playerMovementScript;

    void Start()
    {
        playerMovementScript = gameObject.GetComponent<PlayerMovementScript>();
        storeDefaultSpeed = playerMovementScript.playerSpeed;
        storeDefaultSlideSpeed = playerMovementScript.playerSlideSpeed;
        storeDefaultJumpForce = playerMovementScript.playerJumpForce;
    }
    [ContextMenu(nameof(OnStun))]
    public void OnStun()
    {
        playerMovementScript.playerSpeed = stunnedSpeed;
        playerMovementScript.playerJumpForce = stunnedJumpForce;

    }
    [ContextMenu(nameof(OnRemoveStun))]
    public void OnRemoveStun()
    {
        playerMovementScript.playerSpeed = storeDefaultSpeed;
        playerMovementScript.playerJumpForce = storeDefaultJumpForce;

    }
    [ContextMenu(nameof(OnSlowed))]
    public void OnSlowed()
    {
        isSlowed = true;
        slowedEffect.SetActive(true);
        playerMovementScript.playerSpeed = slowedSpeed;
        playerMovementScript.playerSlideSpeed = slowedSlideSpeed;
    }
    [ContextMenu(nameof(OnRemoveSlowed))]
    public void OnRemoveSlowed()
    {
        isSlowed = false;
        slowedEffect.SetActive(false);
        playerMovementScript.playerSpeed = storeDefaultSpeed;
        playerMovementScript.playerSlideSpeed = storeDefaultSlideSpeed;
    }
}
