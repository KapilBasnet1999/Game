using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Stamina Regeneration")]
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationAmount = 2;
    [SerializeField] float staminaRegenerationDelay = 0.5f;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    public int CalculateStaminaBasedOnLevel(int endurance)//  MAY NOT INCLUDE ENDURANCE BUT INSTEAD BASE LEVEL STAT IN FUTURE
    {
        float stamina = 0;

        //  CREATE AN EQUATION FOR HOW YOU WANT YOUR STAMINA TO BE CALCULATED
        stamina = endurance * 10;
        return Mathf.RoundToInt(stamina);
    }

    public virtual void RegenerateStamina()
    {
        //  ONLY OWNERS CAN EDIT THEIR NETWORK VARIABLES
        if (!character.IsOwner)
            return;

        //  WE DON'T WANT TO TEGENERATE STAMINA IF WE ARE USING IT
        if (character.characterNetworkManager.isSprinting.Value)
            return;
        if (character.isPerformingAction)
            return;

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer = staminaTickTimer + Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                }
            }
        }


    }

    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        // WE ONLY WANT TO RESET THE REGENERATION IF THE ACTION USED STAMINA
        //  WE DONT WANT TO RESET THE REGENERATION IF WE ARE ALREADY REGENERATIING STAMINA
        if (currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }
}
