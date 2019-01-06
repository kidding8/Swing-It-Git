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
    void Start()
    {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        disJoint = GetComponent<DistanceJoint2D>();
    }
    private void Update()
    {
        timerRope += Time.deltaTime;
        timerSpring += Time.deltaTime;
        timerTeleport += Time.deltaTime;
        timerGrapple += Time.deltaTime;
        timerJump += Time.deltaTime;
        timerMagnet += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && PM.CanHook())
        {
            if (PM.IsState(States.STATE_CLOSE_TO_GROUND))
            {
                PM.BigJump();
            }
            else
            {
                Hook();
            }
            
        }else if (Input.GetMouseButtonUp(0) && isPressingButton)
        {
            UnhookHook();
        }

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            PM.BigJump();
        }

        if (PM.IsState(States.STATE_MAGNET))
        {
            MoveMagnet(magnetTarget);
        }

    }

    public void Hook()
    {
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

        if (currentTarget != null)
        {
            CreateHookOnTarget(currentTarget);
            EM.CreateCameraShake(0.05f);
        }
        else
        {
            PM.BigJump();
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
        //isPressingButton = false;
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
        currentHook = grapplePool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        grappleScript = currentHook.GetComponent<GrappleScript>();
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
            
            rb.AddForce(dir * 10f);
            //Debug.DrawRay(transform.position, dir * 20f);
        }
        
    }

    /*private void DestroyMagnet()
    {
        /*if (magnetScript != null)
            magnetScript.DestroyGrapple();*/
    //}

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

}


