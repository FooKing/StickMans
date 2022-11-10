using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;


public class StickmanBalance : MonoBehaviour
{
    public float restingAngle = 0f;
    public float force = 750f;
    public bool RestingUpdate = true;

    public Bones[] bonesArray;


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
        if (Input.GetAxis("Horizontal") < 50f);
        {
            Debug.Log(Input.GetAxis("Horizontal"));
            Rigidbody2D pelvis = bonesArray[1].muscle;
            pelvis.AddForce(bonesArray[1].muscle.transform.right * (Input.GetAxis("Horizontal") * 20));
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

    
}
