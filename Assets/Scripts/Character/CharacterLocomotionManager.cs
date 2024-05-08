using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -40f;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundCheckSphereRadius = 1;
        [SerializeField] protected Vector3 yVelocity;   // THE FORCE AT WHICH OUR CHARACTER IS PULLED UP OR DOWN (Jumping or Falling)
        [SerializeField] protected float groundedYVelocity = -20; // THE FORCE AT WHICH OUR CHARACTER IS STICKING TO THE GROUND WHILST THEY ARE GROUNDED
        [SerializeField] protected float fallStartYVelocity = -5; // THE FORCE AT WHICH OUR CHARACTER BEGINS TO FALL WHEN THEY BECOME UNGROUNDED (RISES AS THEY FALL LONGER)
        protected bool fallingVelocityHAsBeenSet = false;
        [SerializeField] protected float inAirTimer = 0;

        [Header("Flags")]
        public bool isRolling = false;


        protected virtual void Awake()
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update()
        {
            HandleGroundCheck();

            if (character.isGrounded)
            {
                //  IF WE ARE NOT ATTEMPTING TO JUMP OR MOVE UPWARD
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHAsBeenSet = false;
                    yVelocity.y = groundedYVelocity;
                }
            }
            else
            {
                //  IF WE ARE NOT JUMPING, AND OUR FALLING VELOCITY HAS NOT BEEN SET
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHAsBeenSet)
                {
                    fallingVelocityHAsBeenSet = true;
                    yVelocity.y = fallStartYVelocity;
                }
                inAirTimer = inAirTimer + Time.deltaTime;
                character.animator.SetFloat("InAirTimer", inAirTimer);
                Debug.Log("Before:" + yVelocity.y);
                yVelocity.y += gravityForce * Time.deltaTime;
                Debug.Log("After:" + yVelocity.y);
            }

            //  THERE SHOULD ALWAYS BE SOME FORCE APPLIED TO THE Y VELOCITY
            //Debug.Log("Final:" + yVelocity.y);
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck()
        {
            //character.isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
            //Debug.Log("IsJumping:" + character.characterNetworkManager.isJumping.Value);
            if (character.characterNetworkManager.isJumping.Value)
            {
                character.isGrounded = false;
            } else
            {
                character.isGrounded = true;
            }
            //Debug.Log("isGrounded:" + character.isGrounded);
        }

        //  DRAWS OUR GROUND CHECK SPHERE IN SCENE VIEW
        protected void OnDrawGizmosSelected()
        {
            //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }
    }
}
