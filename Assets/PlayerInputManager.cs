using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{

    public static PlayerInputManager instance;
    public PlayerManager player;

    //THNK ABOUT GOALS IN STEPS
    //1. FIND A WAY TO READ VALUES OF A JOY STICK
    //2. Move Character based on those values
    PLayerCONTROLS playerControls;

    [Header("CAMERA MOVEMENT INPUT")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("PLAYER MOVEMENT INPUT")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("PLAYER ACTON INPUT")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;


    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        //When the scene changes run this logic
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // If we are loading into out world scene, enable our player controls
        if (newScene.buildIndex == World_Save_Game_Manager.Instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        //otherwise we must be at main menu, disable our player controls
        //THis is so our character can't move if we enter things like character creation menu etc
        else
        {
            instance.enabled = false;
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PLayerCONTROLS();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            //  HOLDING THE INPUT SETS IT TO TRUE
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            //  LETTING GO OF THE INPUT SETS IT TO FALSE
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;
        }

        playerControls.Enable();
    }

    private void OnDestroy()
    {
        //If We Destroy This Object, Unsubscribe from the event
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    //If We MINIMIZE OR LOWER THE WINDOW, STOP ADJUSTING INPUTS
    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }


    private void Update()
    {
        HandleAllInputs();
    }

    private void HandleAllInputs()
    {
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprintInput();

    }
    //MOVEMENT
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        //RETURNS THE ABSOLUTE NUMER, MEANING NUMBER WITHOUGHT A NEGATIVE SIGN
        moveAmount = Mathf.Clamp01(Mathf.Abs(movementInput.y) + Mathf.Abs(movementInput.x));
        // WE CLAMP THE VALUES, SO THEY ARE 0, 0.5 or 1
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;       
        }
        //  WE PASS 0 ON THE HORIZONTAL BECAUSE WE ONLY WANT NON-STRAFING MOVEMENT
        // WE USE THE HORIZONTAL WHEN WE ARE STRAFING OR LOCKED IN

        if (player == null)
            return;

        //IF WE ARE NOT LOCKED ON, ONLY USE THE MOVEAMOUNT
        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount,player.playerNetworkManager.isSprinting.Value);

        //  IF WE ARE LOCKED ON PASS THE HORIZONTAL MOVEMENT AS WELL
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    //  ACTIONS
    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            
            dodgeInput = false;
            //  FUTURE NOTE: RETURN (DO NOTHING) IF MENU OR UI WINDOW IS OPEN
            //perform a dodge
            player.playerLocomotionManager.AttemptToPerformDodge();
        }
    }
    private void HandleSprintInput()
    {
        if(sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();

        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
        // Handle Sprinting
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            //IF WE HAVE A UI WINDOW OPEN, SIMPLY RETURN WITHOUT DOING ANYTHING

            //  ATTEMPT TO PERFORM A JUMP
            player.playerLocomotionManager.AttemptToPerformJump();
        }
            
    }

}
