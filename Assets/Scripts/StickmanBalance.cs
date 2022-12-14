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

    public GameInstanceManager gameInstanceManager;
    // Stickman player vars
    [Header("Stickman attributes")]
    public string stickname;
    private bool _isAlive = true;
    public float currentHp;
    public float maxHp;
    public float attackDamage;
    public float attackForce;
    public float attackRange = 0.8f;
    private bool _attackRightArm;
    private bool _currentTargetDirectionRight;
    public GameObject _currentTarget;
    public GameObject _bestTarget;
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
    private bool _canAtk = true;
    [Space(20)]
    
    [Header("Bone Vars")]
    [SerializeField] private Bones[] bonesArray;
    [SerializeField] private Rigidbody2D mainBody;
    #endregion
    // Class to hold all references to body parts.

    #region Classes

    [Serializable] public class Bones
    {
        public Rigidbody2D muscle;
        public float muscleAngle;
        public float muscleForce;
        public float restingAngle;
    }

    #endregion

    #region Awake

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

    #endregion

    #region FixedUpdate

    private void FixedUpdate()
    {
        // Add rotational force to achieve characters balance.
        if (_isAlive)
        {
            foreach (Bones currentBone in bonesArray)
            {
                currentBone.muscle.MoveRotation(Mathf.LerpAngle(currentBone.muscle.rotation, currentBone.muscleAngle, currentBone.muscleForce * Time.deltaTime));

            }

            if (_currentTarget != null)
            {
                // Move towards current target
                if (Vector3.Distance(transform.GetChild(0).transform.position, _currentTarget.transform.GetChild(0).transform.position) >
                    attackRange)
                {
                    if (transform.GetChild(0).transform.position.x < _currentTarget.transform.GetChild(0).transform.position.x)
                    {
                        _currentTargetDirectionRight = true;
                        MoveRight();
                        _isWalking = true;
                        if (_walkingCoRunning == false)
                        {
                            StartCoroutine(nameof(StartWalkAnimation));
                        }

                    }
                    else
                    {
                        _currentTargetDirectionRight = false;
                        MoveLeft();
                        _isWalking = true;
                        if (_walkingCoRunning == false)
                        {
                            StartCoroutine(nameof(StartWalkAnimation));
                        }
                    }

                }
                else
                {
                    StopWalking();
                    _isWalking = false;
                }

                // if (Vector3.Distance(transform.GetChild(0).transform.position, currentTarget.transform.GetChild(0).transform.position) <
                //     attackRange &&
                //     _canAtk)
                // {
                //     //Disableattcks
                //
                //     //pick random attack
                //     PunchAttack1();
                //     _canAtk = false;
                //     StartCoroutine(AttackCooldown(1f));
                // }
            }
        }
    }

    #endregion

    #region Start Match Called

    // Start Match

    public void StartMatch()
    {
        StartCoroutine(FindClosestTarget());
    }

    #endregion
    
    #region Movement

    private void MoveRight()
    {
        bonesArray[6].muscleAngle = -20f;
        bonesArray[7].muscleAngle = 50f;
        bonesArray[8].muscleAngle = 140f;
        bonesArray[9].muscleAngle = 230f;
        
        _walkingDirectionRight = true;
        mainBody.velocity = new Vector2(walkSpeed * Time.deltaTime, mainBody.velocity.y);
    }

    private void MoveLeft()
    {
        bonesArray[6].muscleAngle = -130f;
        bonesArray[7].muscleAngle = -240f;
        bonesArray[8].muscleAngle = 40f;
        bonesArray[9].muscleAngle = -20f;
        _walkingDirectionRight = false;
        mainBody.velocity = new Vector2(walkSpeed * Time.deltaTime * -1, mainBody.velocity.y);
    }
    
    
    //Handle and stop movement
    private void StopWalking()
    {
        mainBody.velocity = new Vector2(0, mainBody.velocity.y);
        StopCoroutine(nameof(StartWalkAnimation));
        _walkingCoRunning = false;
        bonesArray[2].muscleAngle = bonesArray[2].restingAngle;
        bonesArray[4].muscleAngle = bonesArray[4].restingAngle;
    }

    private void StartJump()
    {
        
        if (_jumpCooled && _isOnGround)
        {
            _isOnGround = false;
            mainBody.velocity = new Vector2(mainBody.velocity.x, jumpPower);
            if ( _jumpCoRunning == false)
            {
                StartCoroutine(nameof(StartJumpCooldown), Random.Range(0.8f,1.5f)); 
            }
            
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

    #endregion
    
    #region Combat

    public void TakeDamage(GameObject hitBy, Collider2D hitLocation, float hitVelocity)
    {
        if (_isAlive)
        {
            currentHp = currentHp - (hitVelocity / 5);
            if (currentHp <= 0)
            {
                HandleDeath(hitBy);
            }
        }
        

    }

    private void HandleDeath(GameObject killedBy)
    {
        _isAlive = false;
        gameInstanceManager.spawnedStickmen.Remove(this.gameObject);
        foreach (var go in gameInstanceManager.spawnedStickmen)
        {
            print(go);
        }

    }
    //Find closest target
    public IEnumerator FindClosestTarget()
    {
        while (true)
        {
            float bestDistance = Mathf.Infinity;
            for (int i = 0; i < gameInstanceManager.spawnedStickmen.Count; i++)
            {
                if (gameObject != gameInstanceManager.spawnedStickmen[i])
                {
                    float currentDistance = Vector2.Distance(transform.GetChild(0).transform.position,
                        gameInstanceManager.spawnedStickmen[i].transform.GetChild(0).transform.position);
                    if (currentDistance < bestDistance)
                    {
                        bestDistance = currentDistance;
                        _bestTarget = gameInstanceManager.spawnedStickmen[i];
                    }

                    _currentTarget = _bestTarget;
                    yield return new WaitForSeconds(0.5f);
                }
            }
               
            
        }
    }
    void PunchAttack1()
    {
        if (_currentTargetDirectionRight)
        {
            if (_attackRightArm)
            {
                bonesArray[7].muscle.AddForce(new Vector2(attackForce,Random.Range(-100f,100f)),ForceMode2D.Impulse);
                _attackRightArm = !_attackRightArm;
            }
            else
            {
                bonesArray[9].muscle.AddForce(new Vector2(attackForce,Random.Range(-100f,100f)),ForceMode2D.Impulse);
                _attackRightArm = !_attackRightArm;
            }
        }
        else
        {
            if (_attackRightArm)
            {
                bonesArray[7].muscle.AddForce(new Vector2(attackForce * -1,Random.Range(-100f,100f)),ForceMode2D.Impulse);
                _attackRightArm = !_attackRightArm;
            }
            else
            {
                bonesArray[9].muscle.AddForce(new Vector2(attackForce * -1,Random.Range(-100f,100f)),ForceMode2D.Impulse);
                _attackRightArm = !_attackRightArm;
            }
        }
            
    }
    //AttackCooldown
    IEnumerator AttackCooldown(float time)
    {
        yield return new WaitForSeconds(time);
        _canAtk = true;
    }
    #endregion
}
