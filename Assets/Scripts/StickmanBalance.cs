using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class StickmanBalance : MonoBehaviour
{
    public float restingAngle = 0f;
    public float force = 10f;
    public bool restingUpdate = true;
    
    
    // Movement Variables
    public float walkSpeed = 10;
    public float jumpPower = 50;
    public bool jumpCooled;
    public bool jumpCoRunning = false;
    public bool isOnGround;
    public bool isWalking;
    public float walkingAngle = 40;
    public bool walkingCoRunning = false;
    public bool walkingRightFoot;
    public bool walkingDirectionRight;
    public Bones[] bonesArray;
    public Rigidbody2D mainBody;



    // Class to hold all references to body parts.
    [Serializable]
    public class Bones
    {
        public Rigidbody2D muscle;
        public float muscleAngle;
        public float muscleForce;
        public float restingAngle;

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
            if (walkingCoRunning == false)
            {
                StartCoroutine(nameof(StartWalkAnimation));
            }
        }

        if (Input.GetAxisRaw("Horizontal") <= -1)
        {
            MoveLeft();
            isWalking = true;
            if (walkingCoRunning == false)
            {
                StartCoroutine(nameof(StartWalkAnimation)); 
            }
        }

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            StopWalking();
            isWalking = false;
        }


        if (Input.GetButtonDown("Jump"))
        {
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
        mainBody.velocity = new Vector2(0 * (walkSpeed * Time.deltaTime) * -1, mainBody.velocity.y);
        StopCoroutine(nameof(StartWalkAnimation));
        walkingCoRunning = false;
        bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
        bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
    }

    private void StartJump()
    {
        
        if (jumpCooled && isOnGround)
        {
            isOnGround = false;
            mainBody.velocity = new Vector2(mainBody.velocity.x, jumpPower);
            if ( jumpCoRunning == false)
            {
                StartCoroutine("StartCooldown", 1f); 
            }
            
        }
        
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Floor") && jumpCoRunning == false)
        {
            isOnGround = true;
        }
    }

    public IEnumerator StartCooldown(float time)
    {
        jumpCooled = false;
        isOnGround = false;
        jumpCoRunning = true;
        yield return new WaitForSeconds(time);
        jumpCooled = true;
        jumpCoRunning = false;
    }

    public  IEnumerator StartWalkAnimation()
    {
        walkingCoRunning = true;
            while (isWalking)
            {


                if (walkingRightFoot)
                {
                    if (walkingDirectionRight)
                    {
                        bonesArray[2].muscleAngle = walkingAngle;
                    }
                    else
                    {
                        bonesArray[2].muscleAngle = walkingAngle * -1; 
                    }
                    yield return new WaitForSeconds(0.2f);
                    walkingRightFoot = !walkingRightFoot;
                    bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
                }

                else
                {
                    if (walkingDirectionRight)
                    {
                        bonesArray[4].muscleAngle = walkingAngle;
                    }
                    else
                    {
                        bonesArray[4].muscleAngle = walkingAngle * -1; 
                    }
                    
                    yield return new WaitForSeconds(0.2f);
                    walkingRightFoot = !walkingRightFoot;
                    bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
                }
                

            }
            
    }
}
