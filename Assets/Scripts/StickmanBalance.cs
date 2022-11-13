using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


public class StickmanBalance : MonoBehaviour
{
    public float restingAngle = 0f;
    public float force = 10f;
    [FormerlySerializedAs("RestingUpdate")] public bool restingUpdate = true;
    
    
    // Movement Variables
    [FormerlySerializedAs("WalkSpeed")] public float walkSpeed = 10;
    [FormerlySerializedAs("JumpPower")] public float jumpPower = 50;
    public bool jumpCooled;
    public bool isOnGround;
    public bool isWalking;
    public float walkingAngle = 40;
    public bool walkingCoRunning = false;
    public bool walkingRightFoot;
    public bool walkingDirectionRight;
    public Bones[] bonesArray;
    
    [SerializeField] public Rigidbody2D mainBody;



    // Class to hold all references to body parts.
    [Serializable]
    public class Bones
    {
        public Rigidbody2D muscle;
        [FormerlySerializedAs("MuscleAngle")] public float muscleAngle;
        [FormerlySerializedAs("MuscleForce")] public float muscleForce;
        [FormerlySerializedAs("RestingAngle")] public float restingAngle;

    }

    private void Awake()
    {
        
        //Disable collision with own body parts
        Collider2D [] colliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colliders.Length; i++)
        {
            for (int j = i + 1; j < colliders.Length; j++)
            {
                if (colliders[i] == colliders[j])
                    continue;
                Physics2D.IgnoreCollision(colliders[i], colliders[j]);
            }
        }
    }
    

    private void Update()
    {
        
        // Character Input Events
        if (Input.GetAxisRaw("Horizontal") >= 1)
        {
            MoveRight();
            isWalking = true;
            StartCoroutine("StartWalkAnimation",walkingDirectionRight);
            

        }

        if (Input.GetAxisRaw("Horizontal") <= -1)
        {
            MoveLeft();
            isWalking = true;
            StartCoroutine("StartWalkAnimation",!walkingDirectionRight);

        }

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            StopWalking();
        }


        if (Input.GetButton("Jump"))
        {
            print("JumpPressed");
            StartJump();
        }

        




    }
    

    private void FixedUpdate()
    {
        // Add rotational force to achieve characters balance.
        foreach (Bones currentBone in bonesArray)
        {
            currentBone.muscle.MoveRotation(Mathf.LerpAngle(currentBone.muscle.rotation, currentBone.muscleAngle, currentBone.muscleForce * Time.deltaTime));

        }

        
    }


    private void MoveRight()
    {
        walkingDirectionRight = true;
        mainBody.velocity = new Vector2(2 * (walkSpeed * Time.deltaTime), mainBody.velocity.y);

    }

    private void MoveLeft()
    {
        walkingDirectionRight = false;
        mainBody.velocity = new Vector2(2 * (walkSpeed * Time.deltaTime) * -1, mainBody.velocity.y);
    }
    
    
    //Handle and stop movement
    private void StopWalking()
    {
        isWalking = false;
        mainBody.velocity = new Vector2(0 * (walkSpeed * Time.deltaTime) * -1, mainBody.velocity.y);
        StopCoroutine(nameof(StartWalkAnimation));
        walkingCoRunning = false;
        bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
        bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
    }

    private void StartJump()
    {
        StartCoroutine("StartCooldown", 1f);
        if (jumpCooled && isOnGround)
        {
            print("Jump");
            mainBody.velocity = new Vector2(mainBody.velocity.x, jumpPower);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Floor"))
        {
            isOnGround = true;
        }
    }

    public IEnumerator StartCooldown(float time)
    {
        jumpCooled = false;
        isOnGround = false;
        yield return new WaitForSeconds(time);
        jumpCooled = true;
    }

    public  IEnumerator StartWalkAnimation()
    {
        if (walkingCoRunning == false)
        {
            walkingCoRunning = true;
            while (isWalking)
            {


                if (walkingRightFoot)
                {
                    if (walkingDirectionRight)
                    {
                        bonesArray[2].muscleAngle = 40;
                    }
                    else
                    {
                        bonesArray[2].muscleAngle = -40; 
                    }
                    bonesArray[2].muscleAngle = 40;
                    yield return new WaitForSeconds(0.3f);
                    walkingRightFoot = !walkingRightFoot;
                    bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
                }

                else
                {
                    if (walkingDirectionRight)
                    {
                        bonesArray[4].muscleAngle = 40;
                    }
                    else
                    {
                        bonesArray[4].muscleAngle = -40; 
                    }

                    bonesArray[4].muscleAngle = 40;
                    yield return new WaitForSeconds(0.3f);
                    walkingRightFoot = !walkingRightFoot;
                    bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
                }
                

            }
               
        }
    }
}
