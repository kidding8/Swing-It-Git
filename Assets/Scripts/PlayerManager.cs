using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

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
    [HideInInspector]
    public int currentJumps = 0;
    [HideInInspector]
    public bool isHooked = false;
    [HideInInspector]
    public bool isTargetable = true;

    private Rigidbody2D rb;
    
    
    private GameObject currentHook;

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

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
            Debug.Log("Reached Max Speed");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentJumps = maxRopeJumps;
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

    public void AddImpulsiveForce(Vector3 dir, float amount)
    {
        rb.AddForce(dir * amount, ForceMode2D.Impulse);
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
}

