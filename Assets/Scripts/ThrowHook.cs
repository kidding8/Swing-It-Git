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
    private float previousVelocityX;
    private bool isPressed = false;
    public bool isAttachedToHook = false;
    public float fallMultiplier = 2.5f;
    public bool useFallMultiplier = true;
    public float maxVelocity = -30f;
    private Vector2 destinyHook;
    private Vector3 directionHook;
    private float currentAngle = 0;
    private float startedAngle = 0;
    private int currentSpins = 0;
    private bool alreadyAngled = false;
    //private Transform lastHookAttached;
    // Use this for initialization

    /*float flips = 0;
    float deltaRotation = 0;
    float currentRotation = 0;*/
    int backFlips = 0;
    int frontFlips = 0;

    private float rotMin = -360f;
    private float rotMax = 360f;


    private float maxMagnitude = 0;
    private bool alreadyMaxedMagnitude = false;
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
            /* if(rb.velocity.x < previousVelocityX)
             {
                 Vector3 vel = rb.velocity;
                 vel.x = previousVelocityX;
                 rb.velocity = vel;
             }*/
            //if (rb.velocity.magnitude < 16) { 
            Vector3 vel = rb.velocity;
            if(vel.x > 0)
            vel.x += 10 * Time.deltaTime;
            else
            vel.x -= 10 * Time.deltaTime;


            if (vel.y > 0)
                vel.y += 10 * Time.deltaTime;
            else
                vel.y -= 10 * Time.deltaTime;
            
            backFlips = 0;
            frontFlips = 0;

            rb.velocity = vel;

            //var curFwd = Vector3.right;
            // measure the angle rotated since last frame:
            //var ang = Vector3.Angle(curFwd, directionHook);
            var angulo = CalculateAngle(transform.position, destinyHook);
            if (!alreadyAngled)
            {
                startedAngle = angulo;
                alreadyAngled = true;
            }
            currentAngle = angulo;
            //Debug.Log("Angle: " + angulo);
            /*var direction = Quaternion.AngleAxis(angulo, Vector3.up) * Vector3.right * 10;
            Debug.DrawRay(transform.position, direction);
           // Debug.Log("Calcu")
            if (ang > 0.01)
            { // if rotated a significant angle...
              // fix angle sign...
                if (Vector3.Cross(curFwd, directionHook).x < 0) ang = -ang;
                if(ang > 160)
                currentAngle += 1; // accumulate in curAngleX...

                directionHook = destinyHook - (Vector2)transform.position;
                directionHook.Normalize(); // and update lastFwd
            }*/
            //Debug.Log(Vector3.Distance(transform.position, currrentHook.transform.position));
            /*if(Vector3.Distance(transform.position, destinyHook) < 0.5f)
            {
                vel *= -1;
                Debug.Log("Fucked");
                rb.velocity = vel;
            }*/
            //}
            //rb.velocity = rb.velocity * 2f * (Time.deltaTime * 60);
        }
        else if(rb.velocity.y < 0 && useFallMultiplier)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else
        {
            
            currentAngle = 0;
            startedAngle = 0;
            alreadyAngled = false;
            currentSpins = 0;

            /*if(rb.velocity.magnitude > 30 && !alreadyMaxedMagnitude)
            {
                maxMagnitude = rb.velocity.magnitude;
            }
            else if(!alreadyMaxedMagnitude && maxMagnitude != 0)
            {
                alreadyMaxedMagnitude = true;
                EM.GenerateText("Max Velocity", transform);
            }*/


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

        
        if(currentAngle >= startedAngle - 10 && currentAngle <= startedAngle + 10)
        {
            currentSpins++;
            Debug.Log("HOLY FUCKING SHITTTT : " + currentSpins);
        }
        


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
            if(backFlips > 0)
            {
                EM.GenerateText("Backflip x" + backFlips, transform);
            } else if(frontFlips > 0)
            {
                EM.GenerateText("Frontflip x" + frontFlips, transform);
            }
            SetRotationMinMax();
            alreadyMaxedMagnitude = false;
            /* deltaRotation = 0;
             backFlips = 0;
             frontFlips = 0;*/

        }
        else if (Input.GetMouseButtonUp(0) && isPressed)
        {
            Unhook();
        }



        /* if (Input.GetMouseButtonDown(0) && GM.isPlaying() && timerHook > timerNextHook)
         {

             Hook();
             //lastClickTime = Time.time;
         }*/

        

        else if (Input.GetMouseButtonDown(1) && GM.isPlaying())
        {
            Jump();
        }

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

            else if (touch.position.x > Screen.width / 2 && timerJump > timerNextJump)
            {
                Jump();
            }
        }*/

//#endif

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
            ropeScript.UnhookRope();
            DisableRope();
            EM.CreateCameraShake(0.05f);
            rb.AddForce(new Vector3(0.3f, 1f, 0) * forceRopeLeave, ForceMode2D.Force);
            rb.AddTorque(torqueToAdd);

            /* Vector2 velocityVector = rb.velocity;

             velocityVector.y += 3f;
             velocityVector.y += 0.5f;
             rb.velocity = velocityVector;*/

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
                destinyHook = closestHook.transform.position;

                currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
                /*currrentHook = hook.GetPooledObject();
                currrentHook.transform.position = transform.position;
                currrentHook.transform.rotation = Quaternion.identity;
                currrentHook.SetActive(true);*/
                ropeScript = currrentHook.GetComponent<RopeScript>();
                ropeScript.destiny = destinyHook;
                ropeActive = true;

                EM.CreateCameraShake(0.05f);

                //Vector3 dir = rb.velocity - destiny;

                directionHook = destinyHook - (Vector2)transform.position;
                directionHook.Normalize();

                Vector3 rightRay = new Vector3(1f, 0.5f, 0f);
                //Debug.DrawRay(transform.position, -dir);

                //rb.AddForceAtPosition(-direction * forceRopeLeave * 10, transform.position);
                //Vector3 dir = rb.velocity;
                //rb.velocity = velocity * 1.2f;
                
                rb.AddForce(rightRay * forceRopeGrab, ForceMode2D.Force);
                
            }
            else
            {
                //ropeActive = false;

                currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
                Vector3 downPos = new Vector3(transform.position.x ,transform.position.y-15, 0);
                ropeScript = currrentHook.GetComponent<RopeScript>();
                ropeScript.destiny = downPos;
                ropeScript.noTarget = true;
                ropeActive = true;

                Debug.Log("NO ROPE NEARBY");
            }
        }
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
        ropeScript.UnhookRope();
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
        if (other.CompareTag("Enemy") || other.CompareTag("Hook"))
        {
            // other.gameObject.SetActive(false);
            GM.RemoveLife();
        }
    }

    public void DisableRope()
    {
        ropeActive = false;
        isAttachedToHook = false;
        currrentHook = null;
        alreadyAngled = false;
    }

    IEnumerator Jump(float waitSeconds, Vector2 newVelocity, Vector2 oldVelocity)
    {
        rb.velocity = newVelocity;
        yield return new WaitForSeconds(waitSeconds);
        rb.velocity = oldVelocity;
        rb.AddForce(new Vector3(1f, 0.1f, 0) * forceRopeLeave * 3, ForceMode2D.Force);
    }
}


