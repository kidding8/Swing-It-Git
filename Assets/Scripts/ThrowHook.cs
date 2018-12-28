using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{

    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    private GameController GC;
    public ObjectPooler hook;
    public GameObject hookToInstantiate;
    //public float forceRopeGrab = 1f;
    
    private bool ropeActive;
    private GameObject currrentHook;
    private Rigidbody2D rb;
    private SpawnHookManager SM;
    
    private RopeScript ropeScript;
    /* private float lastClickTime = 0;
     public float catchTime = .25f;*/
    private float timerHook;
    private float timerNextHook = 0.4f;
    private float timerJump;
    private float timerNextJump = 0.5f;
    public ParticleSystem smokeParticle;
    private bool isPressed = false;
    
    public bool isInvicible = false;
    private bool alreadyFlipped = false;

    private DistanceJoint2D disjoint;

    public float distanceToGround = 3f;
    public LayerMask whatIsGround;

    int backFlips = 0;
    int frontFlips = 0;

    private float rotMin = -360f;
    private float rotMax = 360f;

    private float distance = 0;
    private Transform connectedHook;
    private bool useDistanceLimit = false;

    void Start()
    {
        GM = GameManager.instance;
        SM = SpawnHookManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        GC = GameController.instance;
        rb = GetComponent<Rigidbody2D>();
        disjoint = GetComponent<DistanceJoint2D>();
        disjoint.enabled = false;
    }
    private void Update()
    {
        timerHook += Time.deltaTime;
        timerJump += Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && timerHook > timerNextHook)
        {
            if (CheckIfGrounded())
            {
                GroundJump();
            }
            else
            {
                Hook();
            }

        }

        else if (Input.GetMouseButtonUp(0) && isPressed)
        {
            Unhook();
            //AuxManager.instance.GetCamera().GetComponent<CameraFollow>().AddTarget(transform);
        }

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            PM.Jump(PM.jumpForce);
        }

        if (CheckIfGrounded())
        {
            rb.drag = PM.dragGrounded;
            rb.angularDrag = PM.dragGrounded;

            if (rb.velocity.magnitude <= 1f)
            {
                GM.OnDeath();
            }
        }
        else
        {
            rb.drag = 0f;
            rb.angularDrag = 0f;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        if (useDistanceLimit)
        {
            float dis = Vector2.Distance(transform.position, connectedHook.position);
            if (dis > distance)
            {
                Vector3 dir = transform.position - connectedHook.position;


                Vector3 offset = transform.position - connectedHook.position;
              //  transform.position = connectedHook.position + Vector3.ClampMagnitude(offset, distance);
                
            }

            

            /*if (Vector3.Distance(transform.position, connectedHook.position) > distance)
            {
                transform.position = (transform.position - connectedHook.position).normalized * distance + transform.position;
                Debug.Log("Limitinggggggggggggg");
            }*/
        }
        /*if(rb.velocity.x > 11)
        {
            EM.speeding = true;
        }
        else
        {
            EM.speeding = false;
        }
        */

        if (PM.isHooked && isPressed)
        {
            //rb.freezeRotation = true;
            Vector3 vel = rb.velocity;
            if (vel.x > 0)
                vel.x += PM.xVelocityMultiplierHooked * Time.deltaTime;
            else
                vel.x -= PM.xVelocityMultiplierHooked * Time.deltaTime;


            if (vel.y > 0)
                vel.y += PM.yVelocityMultiplierHooked * Time.deltaTime;
            else
                vel.y -= PM.yVelocityMultiplierHooked * Time.deltaTime;

            backFlips = 0;
            frontFlips = 0;

            rb.velocity = vel;

            // var angulo = CalculateAngle(transform.position, destinyHook);
            if (!alreadyFlipped)
            {
                CheckIfFlipped();
            }
        }
        else
        {
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

            if (rb.velocity.y < PM.maxYVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, -20);
                Debug.Log("limited falling velocity");
            }
            if (PM.useFallMultiplier)
            {
                rb.velocity += Vector2.up * Physics2D.gravity * (PM.fallMultiplier - 1) * Time.deltaTime;
            }

        }

        
        /*if(currentAngle >= startedAngle - 10 && currentAngle <= startedAngle + 10)
        {
            currentSpins++;
            Debug.Log("HOLY FUCKING SHITTTT : " + currentSpins);
        }*/


        #region Android
        //#if UNITY_EDITOR


        //#elif UNITY_ANDROID

        /*foreach (Touch touch in Input.touches)
        {*/

        /* if (Input.touches.Length > 0)
         {
             Touch touch = Input.touches[0];
             if (touch.position.x < Screen.width / 2 && timerHook > timerNextHook)
             {
                 if (touch.phase == TouchPhase.Began)
                 {
                     /*if(isPressed){
                         Unhook();
                     }*/
        /*Hook();

    }
    else if (touch.phase == TouchPhase.Ended && isPressed)
    {
        Unhook();
    }
}


        //#endif
        */
        #endregion

    }

    private bool CheckIfGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector2.up, distanceToGround, whatIsGround);
    }

   /* public void LimitDistance()
    {
        useDistanceLimit = true;
        connectedHook = PM.GetCurrentHook().transform;
        distance = Vector3.Distance(transform.position, connectedHook.position);
    }*/

    public void TravelToPoint(Transform point)
    {
        float dis = Vector2.Distance(transform.position, point.position);
        if (dis > distance) dis = distance;

        transform.position = transform.position + (point.position - transform.position).normalized * distance;
    }

    public void DontLimitDistance()
    {
        useDistanceLimit = false;
    }

    public void DisconnectDistanceJoint()
    {
        disjoint.enabled = false;
    }

    private void CheckIfFlipped()
    {
        if (backFlips > 0)
        {
            EM.GenerateText("Backflip x" + backFlips, transform);
        }
        else if (frontFlips > 0)
        {
            EM.GenerateText("Frontflip x" + frontFlips, transform);
        }
        SetRotationMinMax();
        alreadyFlipped = true;
    }

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
        isPressed = false;
        if (ropeActive)
        {
            DisableRope();
            EM.CreateCameraShake(0.05f);
            rb.AddForce(new Vector3(0.3f, 1f, 0) * PM.forceRopeLeave, ForceMode2D.Force);
            rb.AddTorque(PM.torqueAddedRopeLeave);
        }
        alreadyFlipped = false;
    }

    private void Hook()
    {
        timerHook = 0;
        isPressed = true;
        rb.gravityScale = PM.gravityHooked;
        if (ropeActive == false)
        {
            GameObject closestHook = SM.GetCurrentFarthestHook();

            if (closestHook != null)
            {
                CreateHook(closestHook, false);
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
        }
    }

    private void CreateHook(GameObject hook, bool noTarget)
    {
        currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        /*currrentHook = hook.GetPooledObject();
        currrentHook.transform.position = transform.position;
        currrentHook.transform.rotation = Quaternion.identity;
        currrentHook.SetActive(true);*/
        ropeScript = currrentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(hook, noTarget);
        ropeActive = true;
    }

    private void CreateHook(Vector2 pos, bool noTarget)
    {
        currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        ropeScript = currrentHook.GetComponent<RopeScript>();
        ropeScript.AddRope(pos, noTarget);
        ropeActive = true;
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
        GameObject closest = SM.GetClosestHookWithoutLimit();
        transform.position = new Vector3(closest.transform.position.x, closest.transform.position.y, transform.position.z);
        rb.velocity = new Vector2(1f, 1f) * 10f;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 110, 100, 20), "Magnitude: " + (int)rb.velocity.magnitude);
        GUI.Label(new Rect(10, 130, 100, 20), "velocity X: " + (int)rb.velocity.x);
        GUI.Label(new Rect(10, 150, 100, 20), "velocity Y: " + (int)rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            // other.gameObject.SetActive(false);
            if (!isInvicible)
            {
                GM.RemoveLife();
            }
            
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Ground")
        {
            
        }
    }

    public void DisableRope()
    {
        ropeScript.UnhookRope();
        ropeActive = false;
        currrentHook = null;
    }
   
    void SetRotationMinMax()
    {
        rotMin = rb.rotation - 360f;
        rotMax = rb.rotation + 360f;
    }

    public static float CalculateAngle(Vector3 from, Vector3 to)
    {
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }
}


