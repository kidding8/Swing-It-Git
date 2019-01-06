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

        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && PM.IsState(States.STATE_NORMAL) && PM.CanHook())
        {

            Hook();

        }

        else if (Input.GetMouseButtonUp(0) && isPressingButton)
        {
            UnhookHook();
        }

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            PM.Jump(PM.jumpForce);
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
            default:
                Debug.Log("No power selected to create");
                break;
        }
    }


    public void UnhookHook()
    {
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
            default:
                Debug.Log("No power selected to create on target");
                break;
        }
    }

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
            Debug.Log("No Target");
            /*Vector3 downPos = new Vector3(transform.position.x, transform.position.y - 10, 0);
            CreateHook(downPos, true);*/
        }
        //}
    }

    public void CreateRope(GameObject target)
    {
        //currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        currentHook = ropePool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        ropeScript = currentHook.GetComponent<RopeScript>();
        ropeScript.AddRopeToTarget(target);
        //ropeActive = true;
    }

    private void DestroyRope()
    {

        isPressingButton = false;
        if (ropeScript != null)
            ropeScript.UnhookRope();

        currentHook = null;
        EM.CreateCameraShake(0.05f);
        rb.AddForce(new Vector3(0.3f, 1f, 0) * PM.forceRopeLeave, ForceMode2D.Force);
        rb.AddTorque(PM.torqueAddedRopeLeave);
    }

    

    private void CreateGrapple(GameObject target)
    {
        currentHook = grapplePool.GetPooledObject();
        currentHook.transform.position = transform.position;
        currentHook.transform.rotation = Quaternion.identity;
        currentHook.SetActive(true);
        grappleScript = currentHook.GetComponent<GrappleScript>();
        grappleScript.CreateDistanceJoint(target, disJoint);
    }

    private void DestroyGrapple()
    {
        if (grappleScript != null)
            grappleScript.DestroyDistanceJoint();
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
        if (springScript != null)
            springScript.DestroySpring();
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
}


