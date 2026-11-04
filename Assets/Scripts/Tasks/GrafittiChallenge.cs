
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
    public Vector2 CommandInputs;
    
    
    public enum Direction {Up, Left, Down, Right}
    public Direction lastKnownDirectionInput;
    public Direction[] directionOrder = new Direction[5]; //The commands randomly chosen
    public int minigameCursor; //Where you are in the minigame
    bool inputLoaded; //do you have an input selected
    public GameObject mainCanvas;
    public GameObject player;

    public GameObject[] inputImages;

    public bool systemActive;
    
    public Sprite arrowSprite;
    public Sprite correctSprite;
    
    public bool canBeStarted;
    public MeshRenderer grafittiResult;
    public Material[] grafittiResultImages;
    public TaskEnder wallTaskEnder;
    PlayerInput pInput;
    public static GrafittiChallenge instance { get; private set; }
    public void Awake()
    {
        //Singleton design patttern
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        pInput = player.GetComponent<PlayerInput>();
        
        
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(false); //Disable images until needed
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
        GrafittiControllerAttach();
    }
    

    void CreateCommandPattern() //Randomize pattern and display visuals for it
    {
        for (int i = 0; i < 5; i++)
        {
            Direction direction = (Direction)Random.Range(0, 4);
            directionOrder[i] = direction;
            inputImages[i].transform.rotation = Quaternion.Euler(0, 0, ((int)direction)*90);
        }
    }

    void Update()
    {
        if(!systemActive){return;}
        InterpretInputs();
        InsertInputs();
        AcceptInputs();
    }

    void InterpretInputs() //Convert keyboard inputs to a set of directional inputs
    {
        
        (UpIsInput, DownIsInput, LeftIsInput, RightIsInput) = (CommandInputs.y>0, CommandInputs.y<0, CommandInputs.x<0, CommandInputs.x>0);
        AnyIsInput=UpIsInput||DownIsInput||LeftIsInput||RightIsInput;
    }

    void InsertInputs() //lock inputs so holding an input does nothing
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

    void AcceptInputs() //Only accept in one input at a time, no diagonals
    {
        
        if(!inputLoaded){return;}
        inputLoaded=false;
        AddressInputs();
        
    }

    void AddressInputs() //If an input is correct, move on. If not, fail. If all complete, complete task
    {
        if(lastKnownDirectionInput == directionOrder[minigameCursor])
        {
            inputImages[minigameCursor].GetComponent<Image>().sprite = correctSprite;
            inputImages[minigameCursor].transform.rotation=Quaternion.Euler(0, 0, 0);
            minigameCursor++;
            if(minigameCursor==directionOrder.Length){Deactivate();}
        }
        else
        {
            FailState();
        }
    }

    void Deactivate() //End task, reset everything
    {
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(false);
            inputImages[i].GetComponent<Image>().sprite = arrowSprite;
        }
        
        systemActive=false;
        mainCanvas.SetActive(true);
        TaskMasterScript.instance.CompleteGrafitti();
        Debug.Log("Successful Grafitti...Restoring controls.");
        wallTaskEnder.canGrafitti=false;
        minigameCursor=0;
        PlayerControllerReattach();
        grafittiResult.material = grafittiResultImages[Random.Range(0, grafittiResultImages.Length)];
        
    }

    public void FailState() //End task, reset everything, let player go back in
    {
        for (int i = 0; i < 5; i++)
        {
            inputImages[i].SetActive(false);
            inputImages[i].GetComponent<Image>().sprite = arrowSprite;
        }
        systemActive=false;
        mainCanvas.SetActive(true);
        minigameCursor=0;
        PlayerControllerReattach();
        
        Debug.Log("Failed input...Restoring controls.");
        

    }

    void PlayerControllerReattach() //Have player controls reattach properly
    {
        if (pInput != null)
        {
            pInput.SwitchCurrentActionMap("Player");
        }
        else
        {
            Debug.Log("PLAYER ISSUE");
        }
    }

    void GrafittiControllerAttach() //Switch to grafitti controls
    {
        if (pInput != null)
        {
            pInput.SwitchCurrentActionMap("Grafitti");
        }
        else
        {
            Debug.Log("GRAFITTI ISSUE");
        }
    }
}
