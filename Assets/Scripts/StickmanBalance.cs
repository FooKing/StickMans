using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class StickmanBalance : MonoBehaviour
{
    #region Variables

    public GameObject gameInstanceManager;
    // Stickman player vars
    [Header("Stickman attributes")]
    public string stickname;
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    public List<GameObject> enemyList;
    public GameObject currentTarget;
    [Space(20)]
    
    
    // Movement Variables
    [Header("Movement settings")]
    [SerializeField] private float walkSpeed = 200;
    [SerializeField] private float jumpPower = 50;
    [SerializeField] private float walkingAngle = 40;
    [SerializeField] private float strideSpeed = 0.3f;
    private bool _jumpCooled = true;
    private bool _jumpCoRunning;
    private bool _isOnGround;
    private bool _isWalking;
    private bool _walkingCoRunning;
    private bool _walkingRightFoot;
    private bool _walkingDirectionRight;
    [Space(20)]
    
    [Header("Bone Vars")]
    [SerializeField] private Bones[] bonesArray;
    [SerializeField] private Rigidbody2D mainBody;
    #endregion
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
        
        // HandleJump
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
        
        //Handle Fixed inputs
        if (Input.GetAxisRaw("Horizontal") >= 1)
        {
            MoveRight();
            _isWalking = true;
            if (_walkingCoRunning == false)
            {
                StartCoroutine(nameof(StartWalkAnimation));
            }
        }

        if (Input.GetAxisRaw("Horizontal") <= -1)
        {
            MoveLeft();
            _isWalking = true;
            if (_walkingCoRunning == false)
            {
                StartCoroutine(nameof(StartWalkAnimation)); 
            }
        }

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            StopWalking();
            _isWalking = false;
        }
        
    }


    private void MoveRight()
    {
        float variance = Random.Range(0, 50);
        _walkingDirectionRight = true;
        mainBody.velocity = new Vector2((walkSpeed +variance)  * Time.deltaTime, mainBody.velocity.y);

    }

    private void MoveLeft()
    {
        float variance = Random.Range(0, 50);
        _walkingDirectionRight = false;
        mainBody.velocity = new Vector2((walkSpeed + variance) * Time.deltaTime * -1, mainBody.velocity.y);
    }
    
    
    //Handle and stop movement
    private void StopWalking()
    {
        mainBody.velocity = new Vector2(0 * Time.deltaTime, mainBody.velocity.y);
        StopCoroutine(nameof(StartWalkAnimation));
        _walkingCoRunning = false;
        bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
        bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
    }

    private void StartJump()
    {
        print(_jumpCooled);
        print(_isOnGround);
        
        if (_jumpCooled && _isOnGround)
        {
            _isOnGround = false;
            mainBody.velocity = new Vector2(mainBody.velocity.x, jumpPower);
            if ( _jumpCoRunning == false)
            {
                StartCoroutine(nameof(StartJumpCooldown), 1f); 
            }
            
        }
        
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Floor") && _jumpCoRunning == false)
        {
            _isOnGround = true;
        }
    }

    public IEnumerator StartJumpCooldown(float time)
    {
        _jumpCooled = false;
        _isOnGround = false;
        _jumpCoRunning = true;
        yield return new WaitForSeconds(time);
        _jumpCooled = true;
        _jumpCoRunning = false;
    }

    public  IEnumerator StartWalkAnimation()
    {
        _walkingCoRunning = true;
            while (_isWalking)
            {

                //Change right thigh angle then alternate to left
                if (_walkingRightFoot)
                {
                    if (_walkingDirectionRight)
                    {
                        bonesArray[2].muscleAngle = walkingAngle;
                    }
                    else
                    {
                        bonesArray[2].muscleAngle = walkingAngle * -1; 
                    }
                    yield return new WaitForSeconds(strideSpeed);
                    _walkingRightFoot = !_walkingRightFoot;
                    bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
                }
                //Change left thigh angle then alternate to right
                else
                {
                    if (_walkingDirectionRight)
                    {
                        bonesArray[4].muscleAngle = walkingAngle;
                    }
                    else
                    {
                        bonesArray[4].muscleAngle = walkingAngle * -1; 
                    }
                    
                    yield return new WaitForSeconds(strideSpeed);
                    _walkingRightFoot = !_walkingRightFoot;
                    bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
                }
            }
    }
    
    //Add enemies to list and start searching.
    public void addEnemiesToList(List<GameObject> spawnedSticks)
    {
        foreach (var VARIABLE in spawnedSticks)
        {
            if (VARIABLE != gameObject)
            {
                enemyList.Add(VARIABLE);
            }
        }

        StartCoroutine(GetClosestEnemy());

    }
    
    
    //Find closest enemy
    public IEnumerator GetClosestEnemy()
    {
        while (true)
        { 
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = transform.position;
            for (int i = 0; i < enemyList.Count; i++)
            {
                Vector3 directionToTarget = enemyList[i].transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = enemyList[i];
                }

                currentTarget = bestTarget;
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
