using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Ground & Jumping")]
    [SerializeField] float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 1;
    [SerializeField] protected Vector3 yVelocity; //    THE FORCE AT WHICH OUR CHARACTER WILL BE PULLED UP OR DOWN
    [SerializeField] protected float groundedYVelocity = -20;   //  THE FORCE AT WHICH OUR CHARACTER IS STICKING TO THE GROUND WHILST THEY ARE GROUNDED
    [SerializeField] protected float fallStartYVelocity = -5; //    THE FORCE AT WHICH OUR CHARACTER BEGINS TO FALL WHEN THEY BECOME UNGROUNDED (RISES AS THEY FALL LONGER)
    [SerializeField] protected bool fallingVelocityHasBeenSet = false;
    [SerializeField] protected float inAirTimer = 0;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Update()
    {
        HandleGroundCheck();
        //  IF WE ARE NOT ATTEMPTING TO JUMP OR MOVE UPWARD
        if (character.isGrounded)
        {
            if (yVelocity.y < 0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            //  IF WE ARENT JUMPING AND OUR FALLING VELOCITY HAS NOT BEEN SET
            if (!character.isJumping && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer = inAirTimer + Time.deltaTime;

            yVelocity.y += gravityForce * Time.deltaTime;

            character.characterController.Move(yVelocity * Time.deltaTime);
        }
       
    }

    protected void HandleGroundCheck()
    {
        character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
    }

    //  DRAWNOUR GROUND SPHERE IN THE SCENE VIEW
    protected void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    }
}
