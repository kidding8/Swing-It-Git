using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowHook : MonoBehaviour
{

    private GameManager GM;
    private EffectsManager EM;
    public ObjectPooler hook;
    public GameObject hookToInstantiate;
    private bool ropeActive;
    private GameObject currrentHook;
    private Rigidbody2D rb;
    private SpawnHookManager SM;
    public float forceRopeGrab = 1f;
    public float forceRopeLeave = 1f;
    public float torqueToAdd = 1f;
    private RopeScript ropeScript;
    /* private float lastClickTime = 0;
     public float catchTime = .25f;*/
    private float timerHook;
    private float timerNextHook = 0.4f;
    private float timerJump;
    private float timerNextJump = 0.5f;
    public ParticleSystem smokeParticle;
    private bool isPressed = false;
    public bool isAttachedToHook = false;
    public float fallMultiplier = 2.5f;
    public bool useFallMultiplier = true;
    public float maxVelocity = -30f;
    private Vector3 directionHook;
    public bool isInvicible = false;

    public float distanceToGround = 3f;
    public LayerMask whatIsGround;

    int backFlips = 0;
    int frontFlips = 0;

    private float rotMin = -360f;
    private float rotMax = 360f;

    void Start()
    {
        GM = GameManager.instance;
        SM = SpawnHookManager.instance;
        EM = EffectsManager.instance;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        timerHook += Time.deltaTime;
        timerJump += Time.deltaTime;

        /*if(rb.velocity.x > 11)
        {
            EM.speeding = true;
        }
        else
        {
            EM.speeding = false;
        }
        */

        if (isAttachedToHook && isPressed)
        {
           
            Vector3 vel = rb.velocity;
            if (vel.x > 0)
                vel.x += 15 * Time.deltaTime;
            else
                vel.x -= 15 * Time.deltaTime;


            if (vel.y > 0)
                vel.y += 10 * Time.deltaTime;
            else
                vel.y -= 10 * Time.deltaTime;

            backFlips = 0;
            frontFlips = 0;

            rb.velocity = vel;

            // var angulo = CalculateAngle(transform.position, destinyHook);

        }
        else if (useFallMultiplier)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
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
        }


        /*if(currentAngle >= startedAngle - 10 && currentAngle <= startedAngle + 10)
        {
            currentSpins++;
            Debug.Log("HOLY FUCKING SHITTTT : " + currentSpins);
        }*/



        if (rb.velocity.y < maxVelocity)
        {
            Vector3 vel = rb.velocity;
            vel.y = maxVelocity;
            rb.velocity = vel;
        }


        //#if UNITY_EDITOR


        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && timerHook > timerNextHook)
        {
            Hook();

            if (backFlips > 0)
            {
                EM.GenerateText("Backflip x" + backFlips, transform);
            }
            else if (frontFlips > 0)
            {
                EM.GenerateText("Frontflip x" + frontFlips, transform);
            }
            SetRotationMinMax();

        }

        else if (Input.GetMouseButtonUp(0) && isPressed)
        {
            Unhook();
            //AuxManager.instance.GetCamera().GetComponent<CameraFollow>().AddTarget(transform);
        }

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            Jump();
        }

        #region Android
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

        //Debug.DrawRay(transform.position, -Vector2.up * distanceToGround);
        if (CheckIfGrounded())
        {
            Debug.Log("grounded");
            
        }

    }

    private bool CheckIfGrounded()
    {
        return Physics2D.Raycast(transform.position, -Vector2.up, distanceToGround, whatIsGround);
    }

    public void Jump()
    {
        timerJump = 0;
        Vector2 velocityVector = rb.velocity;
        if (velocityVector.y < 10f)
        {
            velocityVector.y = 10f;
        }
        else
        {
            velocityVector.y += 5f;
        }
        //  velocityVector.y += 0.5f;
        rb.velocity = velocityVector;
        smokeParticle.gameObject.SetActive(true);
    }

    private void Unhook()
    {
        isPressed = false;
        if (ropeActive)
        {
            //ropeScript.UnhookRope();
            DisableRope();
            EM.CreateCameraShake(0.05f);
            rb.AddForce(new Vector3(0.3f, 1f, 0) * forceRopeLeave, ForceMode2D.Force);
            rb.AddTorque(torqueToAdd);
        }
    }

    private void Hook()
    {
        timerHook = 0;
        isPressed = true;
        if (ropeActive == false)
        {
            GameObject closestHook = SM.GetCurrentFarthestHook();

            if (closestHook != null)
            {
                CreateHook(closestHook.transform.position, false);
                EM.CreateCameraShake(0.05f);
                /*directionHook = destinyHook - (Vector2)transform.position;
                directionHook.Normalize();*/

                Vector3 forceDir = new Vector3(1f, 0.5f, 0f);
                rb.AddForce(forceDir * forceRopeGrab, ForceMode2D.Force);

            }
            else
            {
                //ropeActive = false;
                Vector3 downPos = new Vector3(transform.position.x, transform.position.y - 15, 0);
                CreateHook(downPos, true);
            }
        }
    }

    private void CreateHook(Vector2 pos, bool noTarget)
    {
        currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
        /*currrentHook = hook.GetPooledObject();
        currrentHook.transform.position = transform.position;
        currrentHook.transform.rotation = Quaternion.identity;
        currrentHook.SetActive(true);*/
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

    private void GotoClosestHook(Vector2 destiny)
    {
        Vector2 direction = (Vector2)transform.position - destiny;
        direction.Normalize();
        StartCoroutine(Jump(0.2f, -direction * 20, rb.velocity));

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
        
        if (other.CompareTag("Enemy") || other.CompareTag("Hook") || other.CompareTag("Wall"))
        {
            // other.gameObject.SetActive(false);
            if (!isInvicible)
            {
                GM.RemoveLife();
            }
            
        }
    }

    public void DisableRope()
    {
        ropeScript.UnhookRope();
        
        ropeActive = false;
        isAttachedToHook = false;
        currrentHook = null;
    }

    IEnumerator Jump(float waitSeconds, Vector2 newVelocity, Vector2 oldVelocity)
    {
        rb.velocity = newVelocity;
        yield return new WaitForSeconds(waitSeconds);
        rb.velocity = oldVelocity;
        rb.AddForce(new Vector3(1f, 0.1f, 0) * forceRopeLeave * 3, ForceMode2D.Force);
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


