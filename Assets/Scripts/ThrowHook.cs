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
    private RopeScript ropeScript;
    /* private float lastClickTime = 0;
     public float catchTime = .25f;*/
    private float timerHook;
    private float timerNextHook = 0.4f;
    private float timerJump;
    private float timerNextJump = 0.5f;
    public ParticleSystem smokeParticle;
    
    private bool isPressed = false;
    //private Transform lastHookAttached;
    // Use this for initialization
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

        if(rb.velocity.x > 11)
        {
            EM.speeding = true;
        }
        else
        {
            EM.speeding = false;
        }



//#if UNITY_EDITOR


        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && timerHook > timerNextHook)
        {
            Hook();
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
            rb.AddForce(new Vector3(0.3f, 1f, 0) * forceRopeLeave, ForceMode2D.Impulse);

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
                Vector2 destiny = closestHook.transform.position;

                currrentHook = (GameObject)Instantiate(hookToInstantiate, transform.position, Quaternion.identity);
                /*currrentHook = hook.GetPooledObject();
                currrentHook.transform.position = transform.position;
                currrentHook.transform.rotation = Quaternion.identity;
                currrentHook.SetActive(true);*/
                ropeScript = currrentHook.GetComponent<RopeScript>();
                ropeScript.destiny = destiny;
                ropeActive = true;

                EM.CreateCameraShake(0.05f);

                Vector3 dir = transform.position - (Vector3) destiny;
                //Debug.DrawLine(transform.position, direction);
                //rb.AddForceAtPosition(-direction * forceRopeLeave * 10, transform.position);
                rb.AddForce(dir * forceRopeLeave, ForceMode2D.Force);

            }
            else
            {
                ropeActive = false;
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
        Transform closest = SM.GetClosestHook();
        transform.position = new Vector3(closest.position.x, closest.position.y, transform.position.z);
        rb.velocity = new Vector2(0.1f, 1f) * 10f;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Magnitude: " + (int)rb.velocity.magnitude);
        GUI.Label(new Rect(10, 30, 100, 20), "velocity X: " + (int)rb.velocity.x);
        GUI.Label(new Rect(10, 50, 100, 20), "velocity Y: " + (int)rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // other.gameObject.SetActive(false);
            GM.OnDeath();
        }
    }

    public void DisableRope()
    {
        ropeActive = false;
        currrentHook = null;
    }

    IEnumerator Jump(float waitSeconds, Vector2 newVelocity, Vector2 oldVelocity)
    {
        rb.velocity = newVelocity;
        yield return new WaitForSeconds(waitSeconds);
        rb.velocity = oldVelocity;
        rb.AddForce(new Vector3(1f, 0.1f, 0) * forceRopeLeave * 3, ForceMode2D.Force);
    }
}


