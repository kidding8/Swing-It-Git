using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct AirPower
{
    public string name;
    public int id;
    public int maxPowerUses;
    public Sprite powerSprite;
}

public class ThrowHook : MonoBehaviour
{
    
    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    private AuxManager aux;
    private Canvas canvas;

    [Header("Powers")]
    [Space(4)]
    public AirPower[] airPowers;
    private AirPower currentAirPower;
    public GameObject airPowerPanel;
    public ObjectPooler airPowerImagePool;
    public Slider airPowerTimerSlider;
    public float powerCooldown = 1f;
    private float timerPower;
    private int maxPowerAmmo;
    private int currentPowerAmmo;


    //private bool isUsingPower = false;

    [Header("AirJump")]
    [Space(3)]
    public float airJumpForce;

    [Header("Dashing")]
    [Space(3)]
    public int maxAirDash = 3;
    public float timeToDash = 1f;
    private float dashingTime = 0;
    public float dashingSpeed = 10f;
    private bool isDashing = false;
    private bool stoppedDashing = true;

    [Header("Teleport")]
    [Space(3)]
    //public int maxAirTeleport = 3;
    public float distanceToTeleport = 30f;
    private RaycastHit2D[] hits;
    private Vector3 bestPosToTeleport;
    private bool nearestPoint = true;
    public float step = 0.5f;

    [Header("Explosion")]
    [Space(3)]
    public int maxAirExplosion = 3;
    public float distanceToExplode = 30f;
    public float forceToApply = 50f;

    [Header("Spring")]
    [Space(4)]
    public ObjectPooler springPool;
    public float SpringDuration = 5f;
    private float currentSpringDuration;

    [Header("Rope")]
    [Space(4)]
    public ObjectPooler ropePool;
    public float timeToNextRope = 0.4f;
    private float timerRope;


    [Header("BallDestroyer")]
    [Space(4)]
    public ObjectPooler ballDestroyerPool;


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


    [Header("Magnet")]
    [Space(4)]
    public ObjectPooler magnetPool;
    public float timeToNextMagnet = 0.5f;
    private float timerMagnet;


    [Header("Jump Boost")]
    [Space(4)]
    public float timeToNextJump = 1f;
    private float timerJump;

   

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

    private List<GameObject> airPowerList;

    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        disJoint = GetComponent<DistanceJoint2D>();
        playerFire.SetActive(false);
        canvas = aux.GetCanvas();
        airPowerList = new List<GameObject>();
        //currentAirPower = airPowers[0];
        CreateNewAirPower(1);
    }
    private void Update()
    {
        timerRope += Time.deltaTime;
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

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            AirBallDestroyer();
        }

        /*if (PM.IsState(States.STATE_MAGNET))
        {
            MoveMagnet(magnetTarget);
        }*/

        /*if (PM.IsState(States.STATE_ON_FIRE) && alreadySpinned && timerSpin > durationOfSpin && rb.velocity.y < 0)
        {
            timerSpin = 0;
            alreadySpinned = false;
            playerFire.gameObject.SetActive(false);
            PM.SetNewPlayerState(States.STATE_NORMAL);
        }*/

        if (PM.IsPlayerState(States.STATE_DASHING))
        {
            dashingTime += Time.deltaTime;
            //stoppedDashing = false;
            rb.velocity = Vector2.right * dashingSpeed;

            if (dashingTime >= timeToDash)
            {
                //isDashing = false;
                PM.SetPlayerState(States.STATE_NORMAL);
                rb.velocity = Vector2.right * 20f;
                dashingTime = 0;
                //stoppedDashing = true;
            }

        }

        /*if (dashingTime >= timeToDash && PM.IsPlayerState(States.STATE_DASHING))
        {
            //isDashing = false;
            PM.SetPlayerState(States.STATE_NORMAL);
            rb.velocity = Vector2.right * 20f;
            //stoppedDashing = true;
        }*/

        /*if (rb.velocity.magnitude < previousVelocity && alreadySpinned)
        {
            alreadySpinned = false;
            PM.SetColor(Color.white);
        }*/
        if(PM.IsPlayerState(States.STATE_GRAPPLE) || PM.IsPlayerState(States.STATE_DASHING) || PM.playerHooks == Hooks.HOOK_SPRING)
        {
            playerFire.transform.position = transform.position;
            playerFire.gameObject.SetActive(true);
        }
        else
        {
            playerFire.gameObject.SetActive(false);
        }
        
    }

    public void Hook()
    {
        canTap = true;
        tappingTimer = 0;
       
        if (timerRope > timeToNextRope)
        {
          CreateNewHook();
        }

    }

    public void UnhookHook()
    {
        rb.gravityScale = PM.gravityUnhooked;
        isPressingButton = false;
        currentHook = null;
        PM.SetPlayerState(States.STATE_NORMAL);

        if (canTap && tappingTimer <= tappingTimerOffset)
        {
            DoAirPower();
            canTap = false;
        }

        if (PM.playerHooks == Hooks.HOOK_SPRING)
        {
            DestroySpring();
        }
        else
        {
            DestroyRope();
        }
        /*if (rb.velocity.y > 0 && !alreadySpinned)
        {
            alreadySpinned = true;
            playerFire.gameObject.SetActive(true);
            PM.SetNewPlayerState(States.STATE_ON_FIRE);
            //PM.SetColor(Color.red);
            //StartCoroutine(PM.ChangeColor(2f));
        }*/
    }

    public void DoAirPower()
    {
        if (currentPowerAmmo >= currentAirPower.maxPowerUses || !PM.CanAirPower())
            return;

        switch (currentAirPower.id)
        {
            case Power.POWER_JUMP:
                AirJump();
                break;
            case Power.POWER_DASH:
                AirDash();
                break;
            case Power.POWER_TELEPORT:
                AirTeleport();
                break;
            case Power.POWER_SPRING:
                AirSpring();
                break;
        }
        currentPowerAmmo++;
        RemoveLastAirPowerImage();
        /*if (currentAirJumps == 0)
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
        }*/
    }

    private void CreateNewAirPower()
    {
        currentAirPower = GetNewAirPowerRandom();
        currentPowerAmmo = 0;
        
        for (int i = 0; i < currentAirPower.maxPowerUses; i++)
        {
            AddAirPowerImage();
        }

    }

    private void CreateNewAirPower(int power)
    {
        currentAirPower = airPowers[power];
        currentPowerAmmo = 0;

        for (int i = 0; i < currentAirPower.maxPowerUses; i++)
        {
            AddAirPowerImage();
        }
    }

    private AirPower GetNewAirPowerRandom()
    {
        return airPowers[Random.Range(0, airPowers.Length)];
    }

    public void GetNewAirPower()
    {
        ResetAirPower();
        CreateNewAirPower();
    }

    private void AddAirPowerImage()
    {
        GameObject ammo = airPowerImagePool.GetPooledObject();
        ammo.SetActive(true);
        ammo.transform.SetParent(airPowerPanel.transform, false);
        ammo.GetComponent<Image>().sprite = currentAirPower.powerSprite;
        airPowerList.Add(ammo);
    }

    private void RemoveLastAirPowerImage()
    {
        if(airPowerList.Count >= 1)
        {
            GameObject obj = airPowerList[airPowerList.Count - 1];
            obj.gameObject.SetActive(false);
            airPowerList.Remove(obj);
        }
        
    }

    private void SelectAirPower()
    {
        switch (currentAirPower.id)
        {
            case Power.POWER_JUMP:
                AirJump();
                break;
            case Power.POWER_DASH:
                AirDash();
                break;
            case Power.POWER_TELEPORT:
                AirTeleport();
                break;
            case Power.POWER_EXPLOSION:
                AirExplosion();
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
            if (PM.playerHooks == Hooks.HOOK_SPRING)
            {
                CreateSpring(currentTarget);
            }
            else
            {
                CreateRope(currentTarget);
            }
            


            EM.CreateCameraShake(0.05f);
        }
        /*else
        {
            DoAirPower();
        }*/
        /*PM.SetNewPlayerState(States.STATE_MAGNET);
          magnetTarget = target;*/
    }

    public void ResetAirPower()
    {
        foreach(GameObject obj in airPowerList)
        {
            obj.SetActive(false);
        }
        airPowerList.Clear();
        currentPowerAmmo = 0;
        /*currentAirJumps = 0;
        if (currentAirJumps == 0)
        {
            GM.AirBoostImage(true, true);
            GM.AirBoostImage(false, true);
        }*/
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
        PM.SetPlayerState(States.STATE_DASHING);
        //isDashing = true;
        dashingTime = 0;

    }

    public void AirSpring()
    {
        DestroyRope();
        PM.playerHooks = Hooks.HOOK_SPRING;
        //PM.SetPlayerState(States.STATE_SPRING);
        StartCoroutine(AirSpringTimer(SpringDuration));
    }

    public void AirBallDestroyer()
    {
        GameObject ball = ballDestroyerPool.GetPooledObject();
        ball.SetActive(true);
        ball.transform.position = transform.position;
    }

    public void AirTeleport()
    {
        if(!Physics2D.OverlapPoint(transform.position+Vector3.right * distanceToTeleport))
        {
            transform.position += Vector3.right * distanceToTeleport;
        }
        else if(!nearestPoint)
        {
            hits = Physics2D.RaycastAll(transform.position, Vector3.right * distanceToTeleport);
            bestPosToTeleport = hits[0].point;
            foreach(RaycastHit2D h in hits)
            {
                if (h.distance < Vector2.Distance(bestPosToTeleport, transform.position) &&
                    !Physics2D.OverlapPoint(h.point + h.normal * 0.3f))
                {
                    bestPosToTeleport = h.point;
                }
            }

            transform.position = bestPosToTeleport;

        }
        else
        {
            Vector2 aux = bestPosToTeleport;
            while (Physics2D.OverlapPoint(aux))
            {
                aux += step * Vector2.right;
            }
            if(Vector2.Distance(aux, transform.position + Vector3.right * distanceToTeleport) < Vector2.Distance(bestPosToTeleport, transform.position))
            {
                bestPosToTeleport = aux;
            }

            transform.position = bestPosToTeleport;
        }

        rb.velocity = Vector2.zero;
         
    }

    public void AirExplosion()
    {
        
    }

    public void AirGrapple()
    {
        //PM.SetPlayerState(States.STATE_GRAPPLE);
        CreateGrapple(PM.GetFarthestGrabbableWithoutRadius());
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

    IEnumerator AirSpringTimer(float seconds)
    {
        airPowerTimerSlider.gameObject.SetActive(true);
        float animationTime = 0f;
        while (animationTime < seconds)
        {
            Vector3 topOfPlayer = new Vector3(transform.position.x, transform.position.y + 1f);
            airPowerTimerSlider.gameObject.transform.position = aux.WorldToUISpace(aux.inGameCanvas, topOfPlayer);
            animationTime += Time.deltaTime;
            float lerpValue = animationTime / seconds;
            airPowerTimerSlider.value = Mathf.Lerp(100f, 0f, lerpValue);
            yield return null;
        }
        airPowerTimerSlider.gameObject.SetActive(false);
        PM.SetPlayerState(States.STATE_NORMAL);
        PM.playerHooks = Hooks.HOOK_ROPE;
        DestroySpring();
        //comboCount = 0;
        //DeathMenu();

    }

    private void CreateGrapple(GameObject target)
    {
        if (grappleScript == null)
        {
            currentHook = grapplePool.GetPooledObject();
            currentHook.transform.position = transform.position;
            currentHook.transform.rotation = Quaternion.identity;
            currentHook.SetActive(true);
            grappleScript = currentHook.GetComponent<GrappleScript>();
        }

        grappleScript.CreateGrapple(target, disJoint);
    }

   /* private void DestroyGrapple()
    {
        if (grappleScript != null)
            grappleScript.DestroyGrapple();
    }*/

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

    /*

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

   

    private void MoveMagnet(GameObject target)
    {
        if(target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            //dir.Normalize();
            
            rb.AddForce(dir * 5f);
            //Debug.DrawRay(transform.position, dir * 20f);
        }
        
    }*/

    #endregion
}


