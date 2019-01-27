using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{

    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    [Header("Rope")]
    [Space(4)]
    public ObjectPooler ropePool;
    public float timeToNextRope = 0.4f;
    private float timerRope;

    [Header("Spring")]
    [Space(4)]
    public ObjectPooler springPool;
    public float timeToNextSpring = 0.9f;
    private float timerSpring;

    [Header("Teleporter")]
    [Space(4)]
    public ObjectPooler teleporterPool;
    public float timeToNextTeleport = 0.9f;
    private float timerTeleport;

    [Header("Grapple")]
    [Space(4)]
    public ObjectPooler grapplePool;
    public float timeToNextGrapple = 0.4f;
    private float timerGrapple;


    [Header("Grapple")]
    [Space(4)]
    public ObjectPooler magnetPool;
    public float timeToNextMagnet = 0.5f;
    private float timerMagnet;


    [Header("Jump Boost")]
    [Space(4)]
    public float timeToNextJump = 1f;
    private float timerJump;

    [Header("Dashing")]
    [Space(3)]

    public float timeToDash = 1f;
    private float dashingTime = 0;
    public float dashingSpeed = 10f;
    private bool isDashing = false;
    private bool stoppedDashing = true;

    [Header("Jumps")]
    [Space(3)]
    public bool limitRopeJump = true;
    public int maxRopeJumps = 2;
    [HideInInspector]
    public int currentAirJumps = 0;

    [Header("Misc")]
    [Space(4)]
    public GameObject playerFire;

    private GameObject currentHook;
    private GameObject currentTarget;
    private Rigidbody2D rb;
    private DistanceJoint2D disJoint;


    private RopeScript ropeScript;
    private SpringScript springScript;
    private TeleporterScript teleporterScript;
    private GrappleScript grappleScript;

    private bool isPressingButton = false;
    private GameObject magnetTarget = null;

    private float previousVelocity = 0;
    private bool alreadySpinned = false;
    private float durationOfSpin = 1f;
    private float timerSpin;

    public float tappingTimerOffset = 2f;
    private float tappingTimer;
    private bool canTap = false;

    void Start()
    {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        disJoint = GetComponent<DistanceJoint2D>();
        playerFire.SetActive(false);
        currentAirJumps = maxRopeJumps;
    }
    private void Update()
    {
        timerRope += Time.deltaTime;
        timerSpring += Time.deltaTime;
        timerTeleport += Time.deltaTime;
        timerGrapple += Time.deltaTime;
        timerJump += Time.deltaTime;
        timerMagnet += Time.deltaTime;

        if(canTap)
            tappingTimer += Time.deltaTime;

        if (alreadySpinned)
        {
            timerSpin += Time.deltaTime;
        }

        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && PM.CanHook())
        {
            Hook();
            
            /*if (PM.IsState(States.STATE_CLOSE_TO_GROUND))
            {
                AirJump();
            }
            else
            {
                Hook();
            }*/

        }

        else if (Input.GetMouseButtonUp(0) && isPressingButton)
        {
            UnhookHook();
        }

        /*else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            PM.AirJump();
        }*/

        if (PM.IsState(States.STATE_MAGNET))
        {
            MoveMagnet(magnetTarget);
        }

        if (PM.IsState(States.STATE_ON_FIRE) && alreadySpinned && timerSpin > durationOfSpin && rb.velocity.y < 0)
        {
            timerSpin = 0;
            alreadySpinned = false;
            playerFire.gameObject.SetActive(false);
            PM.SetNewPlayerState(States.STATE_NORMAL);
        }

        if (isDashing)
        {
            dashingTime += Time.deltaTime;
            stoppedDashing = false;
            rb.velocity = Vector2.right * dashingSpeed;
        }

        if (dashingTime >= timeToDash && !stoppedDashing)
        {
            isDashing = false;
            rb.velocity = Vector2.right * 20f;
            stoppedDashing = true;
        }

        /*if (rb.velocity.magnitude < previousVelocity && alreadySpinned)
        {
            alreadySpinned = false;
            PM.SetColor(Color.white);
        }*/
        playerFire.transform.position = transform.position;
    }

    public void Hook()
    {
        canTap = true;
        tappingTimer = 0;
        switch (PM.playerPower)
        {
            case Power.POWER_ROPE:
                if (timerRope > timeToNextRope)
                {
                    CreateNewHook();
                }

                break;

            case Power.POWER_SPRING:
                if (timerSpring > timeToNextSpring)
                {
                    CreateNewHook();
                }
                break;

            case Power.POWER_TELEPORT:
                if (timerTeleport > timeToNextTeleport)
                {
                    CreateNewHook();
                }
                break;

            case Power.POWER_GRAPPLE:
                if (timerGrapple > timeToNextGrapple)
                {
                    CreateNewHook();
                }
                break;
            case Power.POWER_MAGNET:
                if (timerMagnet > timeToNextMagnet)
                {
                    CreateNewHook();
                }
                break;

            default:
                Debug.Log("No power selected to create");
                break;
        }
    }

    public void UnhookHook()
    {
        rb.gravityScale = PM.gravityUnhooked;
        isPressingButton = false;
        currentHook = null;

        if(canTap && tappingTimer <= tappingTimerOffset)
        {
            DoHability();
            canTap = false;
        }

        switch (PM.playerPower)
        {
            case Power.POWER_ROPE:
                DestroyRope();
                break;

            case Power.POWER_SPRING:
                DestroySpring();
                break;

            case Power.POWER_TELEPORT:
                DestroyTeleporter();
                break;

            case Power.POWER_GRAPPLE:
                DestroyGrapple();
                break;
            case Power.POWER_MAGNET:
                PM.SetNewPlayerState(States.STATE_NORMAL);
                magnetTarget = null;
                break;
            default:
                Debug.Log("No power selected to destroy");
                break;
        }

        if (rb.velocity.y > 0 && !alreadySpinned)
        {
            alreadySpinned = true;
            playerFire.gameObject.SetActive(true);
            PM.SetNewPlayerState(States.STATE_ON_FIRE);
            //PM.SetColor(Color.red);
            //StartCoroutine(PM.ChangeColor(2f));
        }
    }

    public void DoHability()
    {
        if (currentAirJumps >= 2)
            return;
        if (currentAirJumps == 0)
        {
            //AirJump();
            SelectPower();
            EM.CreateAirJump(transform.position);
            currentAirJumps++;
            GM.AirBoostImage(false, false);
        }
        else if (currentAirJumps == 1)
        {
            SelectPower();
            currentAirJumps++;
            EM.CreateAirJump(transform.position);
            GM.AirBoostImage(true, false);
        }
    }

    private void SelectPower()
    {
        switch (PM.playerHability)
        {
            case Hability.HABILITY_JUMP:
                AirJump();
                break;
            case Hability.HABILITY_DASH:
                AirDash();
                break;
            case Hability.HABILITY_TELEPORT:

                break;
            case Hability.HABILITY_TORNADO:

                break;
            case Hability.HABILITY_SHOOTING:

                break;
        }
    }

    private void CreateHookOnTarget(GameObject target)
    {
        switch (PM.playerPower)
        {
            case Power.POWER_ROPE:
                
                CreateRope(target);
                break;

            case Power.POWER_SPRING:
                CreateSpring(target);
                break;

            case Power.POWER_TELEPORT:
                CreateTeleporter(target);
                break;

            case Power.POWER_GRAPPLE:
                CreateGrapple(target);
                break;
            case Power.POWER_MAGNET:
                PM.SetNewPlayerState(States.STATE_MAGNET);
                /*Vector3 vel = rb.velocity;
                rb.velocity = vel /2;*/
                magnetTarget = target;
                //rb.velocity = Vector2
                /*Vector3 dir = target.transform.position - transform.position;
                dir.Normalize();
                rb.AddForce(dir * 50f, ForceMode2D.Impulse);*/
                break;
            default:
                Debug.Log("No power selected to create on target");
                break;
        }
    }

    private void CreateNewHook()
    {
        timerRope = 0;
        isPressingButton = true;
        rb.gravityScale = PM.gravityHooked;
        currentTarget = PM.GetCurrentGrabbableObject();
        previousVelocity = rb.velocity.magnitude;
        alreadySpinned = false;
        if (currentTarget != null)
        {
            CreateHookOnTarget(currentTarget);
            EM.CreateCameraShake(0.05f);
        }
        else
        {
            DoHability();
        }
    }

    public void ResetAirPower()
    {
        currentAirJumps = 0;
        if (currentAirJumps == 0)
        {
            GM.AirBoostImage(true, true);
            GM.AirBoostImage(false, true);
        }
    }

    public void CreateRope(GameObject target)
    {
        currentHook = ropePool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        ropeScript = currentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(target);
    }

    private void DestroyRope()
    {
        if (ropeScript != null)
            ropeScript.UnhookRope();
        EM.CreateCameraShake(0.05f);
        rb.AddForce(new Vector3(0.3f, 1f, 0) * PM.forceRopeLeave, ForceMode2D.Force);
        rb.AddTorque(PM.torqueAddedRopeLeave);
    }

    private void CreateSpring(GameObject target)
    {
        currentHook = springPool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        springScript = currentHook.GetComponent<SpringScript>();
        springScript.CreateSpring(target);
    }

    private void DestroySpring()
    {
        isPressingButton = false;
        if (springScript != null)
            springScript.DestroySpring();
        currentHook = null;
        EM.CreateCameraShake(0.05f);
    }

    private void CreateTeleporter(GameObject target)
    {
        currentHook = teleporterPool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        teleporterScript = currentHook.GetComponent<TeleporterScript>();
        teleporterScript.CreateTeleporterDestiny(target);
    }

    private void DestroyTeleporter()
    {
        if (teleporterScript != null)
            teleporterScript.DestroyTeleporter();
    }

    private void CreateGrapple(GameObject target)
    {
        if(grappleScript == null)
        {
            currentHook = grapplePool.GetPooledObject();
            currentHook.transform.position = transform.position;
            currentHook.transform.rotation = Quaternion.identity;
            currentHook.SetActive(true);
            grappleScript = currentHook.GetComponent<GrappleScript>();
        }
        
        grappleScript.CreateGrapple(target, disJoint);
    }

    private void DestroyGrapple()
    {
        if (grappleScript != null)
            grappleScript.DestroyGrapple();
    }

    private void MoveMagnet(GameObject target)
    {
        if(target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            //dir.Normalize();
            
            rb.AddForce(dir * 5f);
            //Debug.DrawRay(transform.position, dir * 20f);
        }
        
    }

    public void AirJump()
    {
        Vector2 velocityVector = rb.velocity;
        //velocityVector.y = jumpForce;
        if (velocityVector.y < PM.jumpForce - 5)
        {
            velocityVector.y = PM.jumpForce;
        }
        else
        {
            velocityVector.y += PM.jumpForce / 2;
        }

        if (velocityVector.x <= PM.jumpForce / 2)
        {
            velocityVector.x = PM.jumpForce / 2;
        }
        //  velocityVector.y += 0.5f;
        rb.velocity = velocityVector;
    }

    public void AirDash()
    {
        isDashing = true;
        dashingTime = 0;

    }

    #region ExtraFunctions
    /*private void DestroyMagnet()
    {
        /*if (magnetScript != null)
            magnetScript.DestroyGrapple();
    }*/

    /*public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }*/

    /* public void LimitDistance()
    {
        useDistanceLimit = true;
        float dis = Vector2.Distance(transform.position, connectedHook.position);
            if (dis > distance)
            {
                Vector3 dir = transform.position - connectedHook.position;


                Vector3 offset = transform.position - connectedHook.position;
              //  transform.position = connectedHook.position + Vector3.ClampMagnitude(offset, distance);
    }*/

    /*public void CreateRopeWithoutTarget(Vector2 pos, bool noTarget)
    {
        currentHook = ropePool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        ropeScript = currentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(pos, noTarget);
    }*/

    /*public void CreateRope(GameObject hook)
    {
        //currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        currentHook = ropePool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        ropeScript = currentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(hook, false);
        //ropeActive = true;
    }*/

    /*private void CreateHookOnTarget(GameObject target)
    {
        switch (PM.playerPower)
        {
            case Power.POWER_ROPE:

                CreateRope(target);

                break;

            case Power.POWER_SPRING:

                break;

            case Power.POWER_TELEPORT:

                break;

            case Power.POWER_GRAPPLE:

                break;
            default:
                Debug.Log("No power selected to create on target");
                break;
        }
    }*/

    #endregion
}


