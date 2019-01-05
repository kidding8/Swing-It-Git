using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{

    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    public ObjectPooler hookPool;
    public GameObject hookToInstantiate;
    //public float forceRopeGrab = 1f;
    
   // private bool ropeActive;
    private GameObject currrentHook;
    private Rigidbody2D rb;
    
    private RopeScript ropeScript;
    /* private float lastClickTime = 0;
     public float catchTime = .25f;*/
    private float timerHook;
    private readonly float timerNextHook = 0.4f;
    private float timerJump;
    private readonly float timerNextJump = 0.5f;
    public ParticleSystem smokeParticle;
    private bool isPressingHooked = false;

    void Start()
    {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        timerHook += Time.deltaTime;
        timerJump += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && timerHook > timerNextHook && PM.playerState == States.STATE_NORMAL && PM.playerPower == Power.POWER_HOOK)
        {
            
            if(PM.CanCollect())
            {
                Hook();
            }

        }

        else if (Input.GetMouseButtonUp(0) && isPressingHooked)
        {
            Unhook();
            //AuxManager.instance.GetCamera().GetComponent<CameraFollow>().AddTarget(transform);
        }

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            PM.Jump(PM.jumpForce);
        }

       
    }

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

    public void Jump()
    {
        timerJump = 0;
        Vector2 velocityVector = rb.velocity;
        //velocityVector.y = jumpForce;
        if (velocityVector.y < PM.jumpForce)
        {
            velocityVector.y = PM.jumpForce;
        }
        else
        {
            velocityVector.y += PM.jumpForce / 1.6f;
        }
        //  velocityVector.y += 0.5f;
        rb.velocity = velocityVector;
        smokeParticle.gameObject.SetActive(true);
    }

    private void GroundJump()
    {
        Vector2 velocityVector = rb.velocity;
        if (velocityVector.x < 5f)
        {
            velocityVector.x = 10f;
        }

        velocityVector.y = PM.jumpForce;
        //  velocityVector.y += 0.5f;
        
        rb.velocity = velocityVector;
        smokeParticle.gameObject.SetActive(true);
    }

    private void Unhook()
    {
        rb.gravityScale = PM.gravityUnhooked;
        //rb.freezeRotation = false;
        
        isPressingHooked = false;
        /*if (ropeActive)
        {*/
            DisableRope();
            EM.CreateCameraShake(0.05f);
            rb.AddForce(new Vector3(0.3f, 1f, 0) * PM.forceRopeLeave, ForceMode2D.Force);
            rb.AddTorque(PM.torqueAddedRopeLeave);
        //}
    }

    private void Hook()
    {
        timerHook = 0;
        isPressingHooked = true;
        rb.gravityScale = PM.gravityHooked;
        /*if (ropeActive == false)
        {*/
            GameObject closestHook = PM.GetCurrentGrabbableObject();

            if (closestHook != null)
            {
                CreateHook(closestHook);
                EM.CreateCameraShake(0.05f);
                
                /*directionHook = destinyHook - (Vector2)transform.position;
                directionHook.Normalize();*/

                /*Vector3 forceDir = new Vector3(1f, 0.5f, 0f);
                rb.AddForce(forceDir * forceRopeGrab, ForceMode2D.Force);*/

            }
            else
            {
                //ropeActive = false;
                Vector3 downPos = new Vector3(transform.position.x, transform.position.y - 10, 0);
                CreateHook(downPos, true);
            }
        //}
    }

    public void CreateHook(GameObject hook)
    {
        //currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        currrentHook = hookPool.GetPooledObject();
        currrentHook.transform.position = transform.position;
        currrentHook.transform.rotation = Quaternion.identity;
        currrentHook.SetActive(true);
        ropeScript = currrentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(hook, false);
        //ropeActive = true;
    }

    public void CreateHookToTarget(GameObject target)
    {
        //currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        currrentHook = hookPool.GetPooledObject();
        currrentHook.transform.position = transform.position;
        currrentHook.transform.rotation = Quaternion.identity;
        currrentHook.SetActive(true);
        ropeScript = currrentHook.GetComponent<RopeScript>();
        ropeScript.AddRopeToTarget(target);
        //ropeActive = true;
    }

    public void CreateHook(Vector2 pos, bool noTarget)
    {
        currrentHook = hookPool.GetPooledObject();
        currrentHook.transform.position = transform.position;
        currrentHook.transform.rotation = Quaternion.identity;
        currrentHook.SetActive(true);
        ropeScript = currrentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(pos, noTarget);
        //ropeActive = true;
    }


    private void SmokeScreen()
    {

        //
        /*Vector2 direction = (Vector2)transform.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        smokeParticle.transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x, transform.rotation.y, direction.z));*/

        /*Vector3 targetDir = (Vector2)transform.position - destiny;
        float angleBetween = Vector3.Angle(Vector3.up, targetDir);

        smokeParticle.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleBetween ));
        smokeParticle.gameObject.SetActive(true);*/
    }

    public void OnContinue()
    {
        //ropeScript.UnhookRope();
        DisableRope();
        GameObject closest = PM.GetClosestGrabbableWithoutRadius();
        transform.position = new Vector3(closest.transform.position.x, closest.transform.position.y, transform.position.z);
        rb.velocity = new Vector2(1f, 1f) * 10f;
    }

    

    public void DisableRope()
    {
        if(ropeScript !=null)
            ropeScript.UnhookRope();
        //ropeActive = false;
        currrentHook = null;
    }

    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }
}


