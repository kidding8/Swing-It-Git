﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /*public enum powers
    {
        Hook,Spring,Teleport,Grapple,Magnet
    }

    public enum habilities
    {
        Jump, Dash, Teleport, Tornado, Shooting, Barrel, Decoy
    }*/

    public int playerState = States.STATE_HIDDEN;
    public int playerHooks = Hooks.HOOK_ROPE;
    //public int playerPower = Power.POWER_JUMP;
    //public int playerHability = Hability.HABILITY_JUMP;


    public static PlayerManager instance;
    private AuxManager aux;
    private GameManager GM;
    private EffectsManager EM;

    [Header("Player Controls")]
    public float maxSpeed = 40f;
    public float forceRopeLeave = 10f;
    public float torqueAddedRopeLeave = 1f;
    public float jumpForce = 15f;
    public bool useFallMultiplier = true;
    public float fallMultiplier = 1.4f;
    public float maxYVelocity = -20;
    public bool invincible = false;
    
    //public powers powerss;
    [Header("Rope")]
    [Space(3)]
    public float xVelocityMultiplierHooked = 15;
    public float yVelocityMultiplierHooked = 10;
    public float gravityUnhooked = 2f;
    public float gravityHooked = 1f;
    public float ropeSpeed = 1f;
    public float ropeDistance = 2f;
    public bool destroyRope = false;

    private float timerHookIndicator;
    private float timerForNextHookIndicator = 0.4f;

    [Header("Grabber")]
    [Space(3)]
    public bool useGrabberJump = true;
    public float grabberJumpForce = 10f;
    public float radiusToGrab = 200f;

    

    [Header("Enemies")]
    [Space(3)]
    public bool canSpawnEnemies = true;
    public bool canShootMissiles = true;
    public bool canShootGuidedMissiles = true;
    

    [Header("Ground")]
    [Space(3)]
    public float distanceToGround = 3f;
    public float dragGrounded = 0.8f;
    public LayerMask whatIsGround;
    public float timeToDieGrounded = 1f;
    private bool isGrounded = false;
    private float groundedTimer = 0;

    [Header("Line")]
    [Space(3)]
    public float startWidth = 1.0f;
    public float endWidth = 1.0f;
    public float threshold = 0.1f;
    List<Vector3> linePoints = new List<Vector3>();
    int lineCount = 0;
    Vector3 lastLinePos = Vector3.one;

    //components
    private LineRenderer line;
    private Transform lastNode;
    private CameraFollow camFollow;
    private ThrowHook throwHook;
    private Rigidbody2D rb;
    private HingeJoint2D hinge;
    //private gameObjects
    private List<GameObject> grabbableObjectsList;
    private GameObject currentHook;
    private GameObject currentGrababbleObject;

    

    private GameObject grabObjectIndicator;
    private GameObject previousTargetHook;
    private DistanceJoint2D disJoint;
    //private GameObject currentFlyingPlane;


    //bools
    private bool alreadyCountedFlips = false;
    private bool useLine;

    //Rotation
    private int backFlips = 0;
    private int frontFlips = 0;
    private float rotMin = -360f;
    private float rotMax = 360f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        aux = AuxManager.instance;
        GM = GameManager.instance;
        EM = EffectsManager.instance;
        rb = GetComponent<Rigidbody2D>();
        grabbableObjectsList = new List<GameObject>();
        grabObjectIndicator = aux.GetGrabObjectIndicator();
        line = GetComponent<LineRenderer>();
        camFollow = aux.GetCamera().GetComponent<CameraFollow>();
        throwHook = GetComponent<ThrowHook>();
        hinge = GetComponent<HingeJoint2D>();
        disJoint = GetComponent<DistanceJoint2D>();

    }

    private void Update()
    {
        timerHookIndicator += Time.deltaTime;
        GameObject hookTemp = GetFarthestGrabbableObjectInRadiusNewTest();
        
        if (timerHookIndicator >= timerForNextHookIndicator && CanHook())
        {
            timerHookIndicator = 0;
            if (hookTemp != null)
            {
                grabObjectIndicator.SetActive(true);
                //grabObjectIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                grabObjectIndicator.transform.position = hookTemp.transform.position;
                //AddTargetHookToCamera(hookTemp);
            }
            else
            {

                hookTemp = GetClosestGrabbableObjectInRadius();
                if (hookTemp != null)
                {
                    //grabObjectIndicator.GetComponent<SpriteRenderer>().color = Color.red;
                    grabObjectIndicator.SetActive(true);
                    grabObjectIndicator.transform.position = hookTemp.transform.position;
                    //AddTargetHookToCamera(hookTemp);
                }
                else
                {
                   /* if (previousTargetHook != null)
                        camFollow.RemoveTarget(previousTargetHook.transform);*/
                    grabObjectIndicator.SetActive(false);
                }
            }

            currentGrababbleObject = hookTemp;

            /*if(previousTargetHook != hookTemp && hookTemp != null)
            {
                camFollow.AddTarget(hookTemp.transform);
                camFollow.RemoveTarget(previousTargetHook.transform);
                
            }*/
            //previousTargetHook = hookTemp;

        }

        if (useLine && lastNode != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, lastNode.transform.position);
        }

        isGrounded = CheckIfGrounded();
        if (isGrounded)
        {
            rb.drag = dragGrounded;
            rb.angularDrag = dragGrounded;
            if(rb.velocity.magnitude < 5)
            {
                groundedTimer += Time.deltaTime;

                if(groundedTimer > timeToDieGrounded)
                {
                    GM.OnDeath();
                }
            }
            else
            {
                groundedTimer = 0;
            }
           /* if(playerState == States.STATE_NORMAL)
            {
                playerState = States.STATE_CLOSE_TO_GROUND;
            }*/
            /*if (rb.velocity.x >= -0.7f && rb.velocity.x <= 0.7f && playerState == States.STATE_GROUNDED)
            {
                GM.OnDeath();
            }*/
        }

        else
        {
           /* if (playerState == States.STATE_CLOSE_TO_GROUND || playerState == States.STATE_GROUNDED)
            {
                playerState = States.STATE_NORMAL;
            }*/
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }

        /*Vector3 thisPos = transform.position;
        float dist = Vector3.Distance(lastLinePos, thisPos);
        if (dist >= threshold)
        {
            lastLinePos = thisPos;
            if (linePoints == null)
                linePoints = new List<Vector3>();
            linePoints.Add(thisPos);
            line.positionCount = linePoints.Count;
            line.SetPosition(linePoints.Count - 1, thisPos);
        }*/
            

        

        //UpdateLine();
        //Debug.Log(playerState.ToString());
    }

    void UpdateLine()
    {
        /*line.SetWidth(startWidth, endWidth);*/
        line.positionCount = linePoints.Count;

        for (int i = lineCount; i < linePoints.Count; i++)
        {
            line.SetPosition(i, linePoints[i]);
        }
        lineCount = linePoints.Count;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
            Debug.Log("Reached Max Speed");
        }

        if(playerState == States.STATE_HOOKED)
        {
            HookedVelocity();
            backFlips = 0;
            frontFlips = 0;

            // var angulo = CalculateAngle(transform.position, destinyHook);
            if (!alreadyCountedFlips)
            {
                CheckIfFlipped();
            }
        }

        else
        {

            if (IsPlayerState(States.STATE_NORMAL) && rb.velocity.y < maxYVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxYVelocity);
               // Debug.Log("limited falling velocity");
            }

            if (useFallMultiplier)
            {
                rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
            }

            if (rb.rotation > rotMax)
            {
                frontFlips++;
                SetRotationMinMax();
            }
            if (rb.rotation < rotMin)
            {
                backFlips++;
                SetRotationMinMax();
            }
        }

    }

    private void HookedVelocity()
    {
        Vector3 vel = rb.velocity;
        if (vel.x > 0)
            vel.x += xVelocityMultiplierHooked * Time.deltaTime;
        else
            vel.x -= xVelocityMultiplierHooked * Time.deltaTime;


        if (vel.y > 0)
            vel.y += yVelocityMultiplierHooked * Time.deltaTime;
        else
            vel.y -= yVelocityMultiplierHooked * Time.deltaTime;

        rb.velocity = vel;
    }

    private void CheckIfFlipped()
    {
        if (backFlips > 0)
        {
            EM.GenerateText("Backflip x" + backFlips, transform.position);
        }
        else if (frontFlips > 0)
        {
            EM.GenerateText("Frontflip x" + frontFlips, transform.position);
        }
        SetRotationMinMax();
        alreadyCountedFlips = true;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 110, 100, 20), "Magnitude: " + (int)rb.velocity.magnitude);
        GUI.Label(new Rect(10, 130, 100, 20), "velocity X: " + (int)rb.velocity.x);
        GUI.Label(new Rect(10, 150, 100, 20), "velocity Y: " + (int)rb.velocity.y);
        GUI.Label(new Rect(10, 170, 100, 20), "Cam Targets: " + (int)camFollow.GetTargetsNum());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            // other.gameObject.SetActive(false);
            //if(playerState != States.STATE_ON_FIRE)

            GM.RemoveLife();

        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground")
        {
            /*if (playerState == States.STATE_CLOSE_TO_GROUND)
            {
                    playerState = States.STATE_GROUNDED;
            }*/
        }else if (other.gameObject.CompareTag("Rocks"))
        {
            //EM.GenerateText("Bounce 500", transform.position);
            if(rb.velocity.magnitude > 5)
                EM.CreateEnemyEffects(transform.position);
            //GM.AddCombo(50);
            //ResetAirJump();
        }
    }

    public void DoAirPower()
    {
        throwHook.DoAirPower();
    }

    public void GetNewAirPower()
    {
        throwHook.GetNewAirPower();
    }

    public void ResetAirJump()
    {
        throwHook.AddPowerAmmo();
        //throwHook.ResetAirPower();
       /*else if(currentAirJumps == 1)
        {
            GM.AirBoostImage(true, true);
            GM.AirBoostImage(false, false);
        }else if(currentAirJumps == 2)
        {
            GM.AirBoostImage(true, false);
            GM.AirBoostImage(false, false);
        }*/

    }

    public bool CheckIfGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector2.up, distanceToGround, whatIsGround);
    }

    public void Jump(float jumpHeight)
    {
        Vector2 velocityVector = rb.velocity;
        //velocityVector.y = jumpForce;
        if (velocityVector.y < jumpHeight -5)
        {
            velocityVector.y = jumpHeight;
        }
        else
        {
            velocityVector.y += jumpHeight/2;
        }
            //  velocityVector.y += 0.5f;
        rb.velocity = velocityVector;
        //rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight));
    }

    public void JumpUpwards(float amount)
    {
        Vector2 velocity = rb.velocity;
        rb.velocity = new Vector2(velocity.x, amount);
    }

    public void LoseMomentum(float percentage)
    {
        Vector2 velocity = rb.velocity;
        velocity.x = rb.velocity.x * percentage;
        rb.velocity = velocity;
    }

    public void AddImpulsiveForce(Vector3 dir, float amount)
    {
        rb.AddForce(dir * amount, ForceMode2D.Impulse);
    }

    public void AddDirectionalVelocity(Vector3 dir, float amount)
    {
        rb.velocity = dir * amount;
    }

    public void JumpXY(float jumpHeight)
    {
        Vector2 velocityVector = rb.velocity;
        //velocityVector.y = jumpForce;
        if (velocityVector.y < jumpHeight)
        {
            velocityVector.y = jumpHeight;
        }
        else
        {
            velocityVector.y += jumpHeight;
        }
        if (velocityVector.x >= 1f && velocityVector.x <= -1f)
        {
            velocityVector.x = jumpHeight;
        }
        else if (velocityVector.x < 1f && velocityVector.x > -jumpHeight)
        {
            velocityVector.x = -jumpHeight;
        }
        else if (velocityVector.x > -1f && velocityVector.x < jumpHeight)
        {
            velocityVector.x = jumpHeight;
        }
        //  velocityVector.y += 0.5f;
        rb.velocity = velocityVector;
        //rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(-2.0f * Physics2D.gravity.y * jumpHeight));
    }

    private void GroundJump()
    {
        Vector2 velocityVector = rb.velocity;
        if (velocityVector.x < 5f)
        {
            velocityVector.x = 10f;
        }

        velocityVector.y = jumpForce;
        //  velocityVector.y += 0.5f;

        rb.velocity = velocityVector;
    }

    #region PlayerState

    public void SetPlayerState(int state)
    {
        playerState = state;
    }

    public bool IsPlayerState(int state)
    {
        return playerState == state;
    }

    #endregion

    #region Hooked

    public void SetNewHook(GameObject hook)
    {
        SetPlayerState(States.STATE_HOOKED);
        currentHook = hook;
    }

    private void AddTargetHookToCamera(GameObject hookTemp)
    {
        if (previousTargetHook != hookTemp)
        {
            camFollow.AddTarget(hookTemp.transform);
            if (previousTargetHook != null)
                camFollow.RemoveTarget(previousTargetHook.transform);
            previousTargetHook = hookTemp;
        }
    }

    public void SetNewLineTarget(Transform target)
    {
        line.enabled = true;
        line.positionCount = 2;
        lastNode = target;
        useLine = true;
        hinge.enabled = true;
        hinge.connectedBody = target.GetComponent<Rigidbody2D>();
    }

    public void RemoveLineTarget()
    {
        line.positionCount = 0;
        line.enabled = false;
        useLine = false;
        lastNode = null;
        hinge.enabled = false;
    }

    public void RemoveHook()
    {
        //SetPlayerState(States.STATE_NORMAL);
        // rb.gravityScale = gravityUnhooked;
        //RemoveLineTarget();
        currentHook = null;
        
    }

    public GameObject GetCurrentHook()
    {
        return currentHook;
    }

    #endregion

    #region grabbable

    public GameObject GetCurrentGrabbableObject()
    {
        return currentGrababbleObject;
    }

    public void TeleportToPoint(Transform point)
    {
        transform.position = point.position;
        //throwHook.DisableRope();
    }

    public void RemoveGrabbableObject(GameObject obj)
    {
        if(obj != null)
            grabbableObjectsList.Remove(obj);
    }

    public void AddGrabbableObject(GameObject obj)
    {
        if(obj != null)
            grabbableObjectsList.Add(obj);
    }

    public GameObject GetFarthestGrabbableObjectInRadiusNewTest()
    {
        GameObject farthest = null;
        //float distance = 0;
        float maxX = float.NegativeInfinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in grabbableObjectsList)
        {
            float x = obj.transform.position.x;
            Vector3 diff = obj.transform.position - currentPosition;
            float curDistance = diff.magnitude;
            //float dirNum = AngleDir(Vector3.forward, diff, Vector3.up);

            if (x > maxX && curDistance < radiusToGrab)
            {
                farthest = obj;
                maxX = x;
                //distance = curDistance;
            }
        }

        if (farthest != null)
        {
            return farthest;
        }
        return null;

    }


    public GameObject GetFarthestGrabbableObjectInRadius()
    {
        GameObject farthest = null;
        float distance = 0;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in grabbableObjectsList)
        {
            Vector3 diff = obj.transform.position - currentPosition;
            float curDistance = diff.sqrMagnitude;
            float dirNum = AngleDir(Vector3.forward, diff, Vector3.up);
            if (curDistance > distance && curDistance < radiusToGrab && dirNum == 1)
            {
                farthest = obj;
                distance = curDistance;
            }
        }

        if (farthest != null)
        {
            return farthest;
        }
        return null;

    }

    public GameObject GetClosestGrabbableObjectInRadius()
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in grabbableObjectsList)
        {
            Vector3 directionToTarget = obj.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr && dSqrToTarget < radiusToGrab)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = obj;
            }
        }

        return bestTarget;
    }

    public GameObject GetClosestGrabbableWithoutRadius()
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in grabbableObjectsList)
        {
            Vector3 directionToTarget = obj.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = obj;
            }
        }

        return bestTarget;
    }

    public GameObject GetFarthestGrabbableWithoutRadius()
    {
        GameObject farthest = null;
        float distance = 0;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in grabbableObjectsList)
        {
            Vector3 diff = obj.transform.position - currentPosition;
            float curDistance = diff.sqrMagnitude;
            float dirNum = AngleDir(Vector3.forward, diff, Vector3.up);
            if (curDistance > distance && dirNum == 1)
            {
                farthest = obj;
                distance = curDistance;
            }
        }
        return farthest;

    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

    #endregion
    
    #region verifications

    public bool CanCollectCoins()
    {
        return playerState == States.STATE_HOOKED || playerState == States.STATE_NORMAL;
    }

    public bool CanCollectObjects()
    {
        return playerState == States.STATE_HOOKED || playerState == States.STATE_NORMAL;
    }

    public bool CanHook()
    {
        return playerState == States.STATE_NORMAL || playerState == States.STATE_SPRING;
    }

    public bool CanDie()
    {
        return true; 
    }

    public bool CanAirPower()
    {
        return playerState == States.STATE_NORMAL;
    }
    #endregion

    void SetRotationMinMax()
    {
        rotMin = rb.rotation - 360f;
        rotMax = rb.rotation + 360f;
    }

    public void OnContinue()
    {
        //ropeScript.UnhookRope();
        throwHook.UnhookHook();
        GameObject closest = GetClosestGrabbableWithoutRadius();
        transform.position = new Vector3(closest.transform.position.x, closest.transform.position.y, transform.position.z);
        rb.velocity = new Vector2(1f, 1f) * 10f;
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public DistanceJoint2D GetDistanceJoint()
    {
        return disJoint;
    }
    /*public IEnumerator ChangeColor(float waitTime)
    {
        SetColor(Color.red);
        yield return new WaitForSeconds(waitTime);
        SetColor(Color.white);
    }*/

    /*public bool isPower(int power)
    {
        return powerss.Equals(power);
    }*/

    /*switch (powerss) {
           case powers.Hook:
               playerPower = Power.POWER_ROPE;
               break;
           case powers.Spring:
               playerPower = Power.POWER_SPRING;
               break;
           case powers.Teleport:
               playerPower = Power.POWER_TELEPORT;
               break;
           case powers.Grapple:
               playerPower = Power.POWER_GRAPPLE;
               break;
           case powers.Magnet:
               playerPower = Power.POWER_MAGNET;
               break;
       }*/
}

