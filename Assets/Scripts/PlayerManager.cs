using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    private AuxManager aux;
    private GameManager GM;

    public float maxSpeed = 40f;
    public float forceRopeLeave = 10f;
    public float torqueAddedRopeLeave = 1f;
    public float jumpForce = 15f;
    public float fallMultiplier = 1.4f;
    public bool useFallMultiplier = true;
    public float dragGrounded = 0.8f;
    public float xVelocityMultiplierHooked = 15;
    public float yVelocityMultiplierHooked = 10;
    public float maxYVelocity = -20;
    public float gravityUnhooked = 2f;
    public float gravityHooked = 1f;

    public bool useGrabberJump = true;
    public float grabberJumpForce = 10f;

    public bool limitRopeJump = true;
    public int maxRopeJumps = 2;

    public bool canSpawnEnemies = true;
    public bool canShootMissiles = true;
    public bool canShootGuidedMissiles = true;

    public float ropeSpeed = 1f;
    public float ropeDistance = 2f;

    public float radiusToGrab = 200f;

    public bool invincible = false;


    [HideInInspector]
    public int currentJumps = 0;
    [HideInInspector]
    public bool isHooked = false;
    [HideInInspector]
    public bool isTargetable = true;



    private Rigidbody2D rb;
    private List<GameObject> grabbableObjectsList;
    private GameObject currentHook;
    private GameObject currentGrababbleObject;
    private GameObject grabObjectIndicator;

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
        rb = GetComponent<Rigidbody2D>();
        currentJumps = maxRopeJumps;
        grabbableObjectsList = new List<GameObject>();
        grabObjectIndicator = aux.GetGrabObjectIndicator();
    }

    private void Update()
    {
        GameObject hookTemp = GetFarthestGrabbableObjectInRadius();

        if (hookTemp != null)
        {
            grabObjectIndicator.SetActive(true);
            grabObjectIndicator.transform.position = hookTemp.transform.position;
            /*SpriteRenderer sRenderer = currentFarthestHook.GetComponent<SpriteRenderer>();
            sRenderer.sprite = aux.GetAvailableHookSprite();*/
        }
        else
        {
            /*SpriteRenderer sRenderer = currentFarthestHook.GetComponent<SpriteRenderer>();
            sRenderer.sprite = aux.GetUnavailableHookSprite();*/
            hookTemp = GetClosestGrabbableObjectInRadius();
            if (hookTemp != null)
            {
                grabObjectIndicator.SetActive(true);
                grabObjectIndicator.transform.position = hookTemp.transform.position;
            }
            else
            {
                grabObjectIndicator.SetActive(false);
            }
        }
        currentGrababbleObject = hookTemp;
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
            Debug.Log("Reached Max Speed");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Enemy") || other.CompareTag("Wall"))
        {
            // other.gameObject.SetActive(false);
            if (!isTargetable)
            {
                GM.RemoveLife();
            }

        }
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

    public void AddImpulsiveForce(Vector3 dir, float amount)
    {
        rb.AddForce(dir * amount, ForceMode2D.Impulse);
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

    public void SetNewHook(GameObject hook)
    {
        isHooked = true;
        currentHook = hook;
    }

    public void SetIsTargetable(bool newBool)
    {
        isTargetable = newBool;
    }

    public void RemoveHook()
    {
        isHooked = false;
        currentHook = null;
    }

    public GameObject GetCurrentHook()
    {
        return currentHook;
    }

    public GameObject GetCurrentGrabbableObject()
    {
        return currentGrababbleObject;
    }

    public void TeleportToPoint(Transform point)
    {
        transform.position = point.position;
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

    public GameObject GetFarthestGrabbableObjectInRadius()
    {
        GameObject farthest = null;
        float distance = 0;
        Vector3 currentPosition = transform.position;
        foreach (GameObject obj in grabbableObjectsList)
        {
            Vector3 diff = obj.transform.position - currentPosition;
            float curDistance = diff.sqrMagnitude;
            float dirNum = AngleDir(transform.forward, diff, transform.up);
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

}

