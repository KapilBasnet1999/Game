using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerAnimationManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    protected override void Awake()
    {
        base.Awake();

        // Do More Stuff, Only for THE PLAYER

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimationManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }
    protected override void Update()
    {
        base.Update();
        //  If We DO NOT OWN THIS GAMEOBJECT, WE DO NOT CONTROL OR EDIT IT
        if (!IsOwner) 
        {
            return;
        }

        // HANDLE ALL OF OUR CHARACTERS MOVEMENT
        playerLocomotionManager.HandleAllMovement();

        //  REGEN STAMINA
        playerStatsManager.RegenerateStamina();
    }

    protected override void LateUpdate()
    {
        if (!IsOwner)
            return;

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //If This Is The Player Object Owned by this client
        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;

            playerNetworkManager.currentStamina.OnValueChanged += PLayerUIManager.instance.playerUiHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            //  THIS WILL BE MOVED WHEN SAVING AND LOADING IS ADDED
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnLevel(playerNetworkManager.endurance.Value);
            PLayerUIManager.instance.playerUiHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            
        }

    }
}

