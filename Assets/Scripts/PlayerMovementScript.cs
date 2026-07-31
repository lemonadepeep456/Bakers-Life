using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class PlayerMovementScript : MonoBehaviour
{

    public Vector3 leftDirection;
    public Vector3 rightDirection;
    public Vector3 upDirection;
    public Vector3 downDirection;

    public Vector3 playerFacingRightOffset;
    public Vector3 playerFacingLeftOffset;
    public Vector3 playerFacingDownOffset;
    public Vector3 playerFacingUpOffset;

    public bool canMove;
    public bool normalForm;



    public int playerFacing;

   public Animator animator;


    void Start()
    {
        canMove = true;
        normalForm = true;

    }
    void Update()
    {
        if (canMove == true && normalForm == true)
        {
            if (Input.GetKey(KeyCode.A))
            {
                GetComponent<Transform>().position += leftDirection * Time.deltaTime;
                GetComponent<Animator>().Play("Abigail(SideWalkLeft)");
             //  GetComponent<SpriteRenderer>().flipX = true; 
                playerFacing = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                GetComponent<Transform>().position += rightDirection * Time.deltaTime;
                GetComponent<Animator>().Play("Abigail(SideWalkRight)");
              // GetComponent<SpriteRenderer>().flipX = false; //Flips or unflips sprites(if you don't want to manually make left and right sprites and reuse one side sprite)
                playerFacing = 1;
            }
            else if (Input.GetKey(KeyCode.W))
            {
                GetComponent<Transform>().position += upDirection * Time.deltaTime;
                GetComponent<Animator>().Play("Abigail(BackWalk)");
                playerFacing = 2;
            }
            else if (Input.GetKey(KeyCode.S)) //When key is being held down
            {
                GetComponent<Transform>().position += downDirection * Time.deltaTime; // Makes Position of the player/gameobject move
                GetComponent<Animator>().Play("Abigail(FrontWalk)"); //Animation that plays during it
                playerFacing = -2; //Determines the direction the player is facing to allow it to play the side of that direction
            }
            else if (playerFacing == -2)
            {
                GetComponent<Animator>().Play("Abigail(FrontIdle)");
            }
            else if (playerFacing == -1)
            {
                GetComponent<Animator>().Play("Abigail(SideIdleLeft)");
               // GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (playerFacing == 1)
            {
                GetComponent<Animator>().Play("Abigail(SideIdleRight)"); 
                // GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (playerFacing == 2)
            {
                GetComponent<Animator>().Play("Abigail(BackIdle)");

            }


        }
    }
}