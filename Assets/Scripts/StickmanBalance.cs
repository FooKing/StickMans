using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickmanBalance : MonoBehaviour
{
    public float restingAngle = 0f;
    public float force = 750f;

    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        rb.MoveRotation(Mathf.LerpAngle(rb.rotation, restingAngle, force * Time.deltaTime));
    }
}
