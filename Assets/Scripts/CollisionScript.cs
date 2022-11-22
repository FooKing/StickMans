using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionScript : MonoBehaviour
{
    [SerializeField] private StickmanBalance mainStickScript;
    public void OnCollisionEnter2D(Collision2D collision)
    {
        //print(collision.gameObject.GetComponentsInParent<StickmanBalance>());
       if(collision.gameObject.CompareTag("Stickman")) 
       {
           collision.gameObject.GameObject().GetComponentInParent<StickmanBalance>().TakeDamage(this.GameObject(),collision.collider, collision.relativeVelocity.magnitude);
       }
    }
}
