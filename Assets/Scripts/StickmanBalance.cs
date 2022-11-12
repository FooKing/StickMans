using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;


public class StickmanBalance : MonoBehaviour
{
    public float restingAngle = 0f;
    public float force = 50f;
    public bool RestingUpdate = true;
    
    
    // Movement Variables
    public float WalkSpeed = 50;
    public float JumpPower = 50;
    public bool jumpCooled;
    public bool isOnGround;

    public Bones[] bonesArray;
    
    [SerializeField] public Rigidbody2D mainBody;



    // Class to hold all references to body parts.
    [Serializable]
    public class Bones
    {
        public Rigidbody2D muscle;
        public float MuscleAngle;
        public float MuscleForce;

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

        }

        if (Input.GetAxisRaw("Horizontal") <= -1)
        {
            MoveLeft();

        }

        if (Input.GetButton("Jump"))
        {
            StartJump();
        }




    }

    private void FixedUpdate()
    {
        // Add rotational force to achieve characters balance.
        foreach (Bones currentBone in bonesArray)
        {
            currentBone.muscle.MoveRotation(Mathf.LerpAngle(currentBone.muscle.rotation, currentBone.MuscleAngle, currentBone.MuscleForce * Time.deltaTime));

        }

        
    }


    private void MoveRight()
    {
        mainBody.velocity = new Vector2(10 * (WalkSpeed * Time.deltaTime), mainBody.velocity.y);

    }

    private void MoveLeft()
    {
        mainBody.velocity = new Vector2(10 * (WalkSpeed * Time.deltaTime) * -1, mainBody.velocity.y);
    }

    private void StartJump()
    {
        StartCoroutine("StartCooldown", 1f);
        if (jumpCooled && isOnGround)
        {
            mainBody.velocity = new Vector2(mainBody.velocity.x, JumpPower);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {

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
}
