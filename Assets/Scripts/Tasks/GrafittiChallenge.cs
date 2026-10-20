using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GrafittiChallenge : MonoBehaviour
{
    bool UpIsInput;
    bool UpIsSent;
    bool DownIsInput;
    bool DownIsSent;
    bool LeftIsInput;
    bool LeftIsSent;
    bool RightIsInput;
    bool RightIsSent;
    bool AnyIsInput;
    bool AnyIsSent;
    Vector2 CommandInputs;
    public InputActionMap Grafitti;
    public PlayerInput playerInput;
    public enum Direction {Up, Left, Down, Right}
    public Direction lastKnownDirectionInput;
    public Direction[] directionOrder = new Direction[5];
    public int minigameCursor;
    bool inputLoaded;
    public GameObject mainCanvas;
    public GameObject player;

    public GameObject[] inputImages;

    public bool systemActive;
    Vector3 storeLinearVelocity;
    public Sprite arrowSprite;
    public Sprite correctSprite;
    public TaskMasterScript taskMasterScript;
    public bool canBeStarted;
    

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        Grafitti = playerInput.actions.FindActionMap(nameof(Grafitti));
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(false);
        }
    }
    [ContextMenu(nameof(Activate))]
    public void Activate()
    {
        if(!canBeStarted){return;}
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(true);
        }
        CreateCommandPattern();
        systemActive=true;
        mainCanvas.SetActive(false);
        //storeLinearVelocity = player.GetComponent<Rigidbody>().linearVelocity;
        //player.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        player.GetComponent<PlayerInput>().enabled = false;
    }
    void OnProvideMarks(InputValue movementValue)
    {
        CommandInputs = movementValue.Get<Vector2>();
    }

    void CreateCommandPattern()
    {
        for (int i = 0; i < 5; i++)
        {
            Direction direction = (Direction)Random.Range(0, 4);
            directionOrder[i] = direction;
            inputImages[i].transform.rotation = Quaternion.Euler(0, 0, 180+(((int)direction)*90));
        }
    }

    void Update()
    {
        if(!systemActive){return;}
        InterpretInputs();
        InsertInputs();
        AcceptInputs();
    }

    void InterpretInputs()
    {
        (UpIsInput, DownIsInput, LeftIsInput, RightIsInput) = (CommandInputs.y>0, CommandInputs.y<0, CommandInputs.x<0, CommandInputs.x>0);
        AnyIsInput=UpIsInput||DownIsInput||LeftIsInput||RightIsInput;
    }

    void InsertInputs()
    {
        
        if (UpIsInput)
        {
            if(UpIsSent){return;}
            lastKnownDirectionInput=Direction.Up;
            UpIsSent=true;
        }
        else
        {
            UpIsSent=false;
        }

        if (DownIsInput)
        {
            if(DownIsSent){return;}
            lastKnownDirectionInput=Direction.Down;
            DownIsSent=true;
        }
        else
        {
            DownIsSent=false;
        }

        if (LeftIsInput)
        {
            if(LeftIsSent){return;}
            lastKnownDirectionInput=Direction.Left;
            LeftIsSent=true;
        }
        else
        {
            LeftIsSent=false;
        }

        if (RightIsInput)
        {
            if(RightIsSent){return;}
            lastKnownDirectionInput=Direction.Right;
            RightIsSent=true;
        }
        else
        {
            RightIsSent=false;
        }

        if (AnyIsInput)
        {
            if(AnyIsSent){return;}
            inputLoaded=true;
            AnyIsSent=true;
        }
        else
        {
            AnyIsSent=false;
        }


        
    }

    void AcceptInputs()
    {
        
        if(!inputLoaded){return;}
        inputLoaded=false;
        AddressInputs();
        
    }

    void AddressInputs()
    {
        if(lastKnownDirectionInput == directionOrder[minigameCursor])
        {
            inputImages[minigameCursor].GetComponent<Image>().sprite = correctSprite;
            minigameCursor++;
            if(minigameCursor==directionOrder.Length){Deactivate();}
        }
        else
        {
            FailState();
        }
        
    }

    void Deactivate()
    {
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(false);
        }
        player.GetComponent<PlayerInput>().enabled = false;
        systemActive=false;
        mainCanvas.SetActive(true);
        //player.GetComponent<Rigidbody>().linearVelocity = storeLinearVelocity;
        taskMasterScript.EndTask();
        canBeStarted=false;
        player.GetComponent<PlayerInput>().enabled=true;
    }

    void FailState()
    {
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(false);
            inputImages[i].GetComponent<Image>().sprite = arrowSprite;
        }
        player.GetComponent<PlayerInput>().enabled = false;
        systemActive=false;
        mainCanvas.SetActive(true);
        minigameCursor=0;
        player.GetComponent<PlayerInput>().enabled=true;

    }
}
