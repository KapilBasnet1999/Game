using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;

    //These values will be taken from input manager
    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;

    [Header("MOVEMENT SETTINGS")]

    private Vector3 moveDirection = Vector3.zero;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingspeed = 2;
    [SerializeField] float runningSpeed = 5;
    [SerializeField] float rotationSpeed = 15;
    [SerializeField] float sprintingSpeed = 6.5f;
    [SerializeField] float sprintingStaminaCost = 0.25f;

    [Header("DODGE")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 2.5f;
    [SerializeField] float jumpStaminaCost = 2.5f;
    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.verticalMovement.Value = verticalMovement;
            player.characterNetworkManager.horozontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horozontalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;

            //IF NOT LOCKED ON PASS THE MOVE AMOUNT
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
            
            //  IF LOCKED ON PASS HOR AND VERT
        }
    }


    public void HandleAllMovement()
    {


        HandleGroundedMovement();
        HandleRotation();
        //Grounded Movement
        //Aerial
        //Movement
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
        //Clamp THE MOVEMENTS
    }
    private void HandleGroundedMovement()
    {
        if (!player.canMove)
        {
            return;
        }

        GetMovementValues();

        //Our Movement Direction is based on our cameras facing perspective and our movement inputs
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //Move at a running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                //Move at Walking Speed
                player.characterController.Move(moveDirection * walkingspeed * Time.deltaTime);
            }
        }

        if (PlayerInputManager.instance.moveAmount > 0.5f) 
        {
            //Move at a running speed
            player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            //Move at Walking Speed
            player.characterController.Move(moveDirection * walkingspeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
        {
            return;
        }

        Vector3 targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            //Set sprinting to false
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        //  IF WE ARE OUT OF STAMINA, SET SPRINTING TO FALSE

        //  IF WE ARE MOVING WE WANT TO SET SPRINTING TO TRUE

        if (moveAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }

        //  IF WE ARE STATIONARY/slowly moving WE WANT TO SET SPRINTING EQUAL TO FALSE
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;
        }
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
            return;

        if (player.playerNetworkManager.currentStamina.Value <= 0)
            return;
        
        //  IF WE ARE MOVING WHEM WE ATTEMPT TO DODGE WE WILL ROLL, IF WEARE STATIONARY WE WILL BACKSTEP
        if (PlayerInputManager.instance.moveAmount > 0)
        {


            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true,true,false,false);
            //  PERFORM A ROLL ANIMATION
        }
        else
        {
            //PERFORM A BACKSTEP ANIMATION

            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true,false,false);
        }
        player.playerNetworkManager.currentStamina.Value -= dodgeStaminaCost;
    }

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction)
            return;

        //  IF WE'RE OUT OF STAMINA WE DON'T WANNA ALLOW A JUMP

        if (player.playerNetworkManager.currentStamina.Value <= 0)
            return;
        //  IF WE ARE ALREADY IN JUMP WE DON'T WISH TO ALLOW JUMP
        if (player.isJumping)
            return;
        //  IF WE ARE GROUNDED, WE DO MOT WANT TO ALLOW A JUMP
        if (player.isGrounded)
            return;
        //  IF WE ARE TWO HANDING OUR WEAPON, PLAY THE TWO HANDED JUMP ANIMATION (PROBABLY WON'T INCLUDE TWO HAND)
        player.playerAnimatorManager.PlayTargetActionAnimation("Main_Jump_01", false);

        player.isJumping = true;
        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

    }

    public void ApplyJumpingVelocity()
    {
        //  APPLY AN UPWARD VELOCITY
    }
}
