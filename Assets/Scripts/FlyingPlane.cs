using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlane : MonoBehaviour
{
    private PlayerManager PM;
    private EffectsManager EM;

    public float moveSpeed = 2f;
    public float upSpeed = 30f;
    private bool isAttached = false;
    private HingeJoint2D joint;
    private Rigidbody2D playerRb;
    private GameObject player;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        joint = GetComponent<HingeJoint2D>();
        player = PM.GetPlayer();
        playerRb = player.GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
            
    }

    // Update is called once per frame
    void Update()
    {
        if (PM.playerState == States.STATE_FLYING)
        {
            //
           
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 velocity = rb.velocity;

                //rb.velocity = new Vector2(velocity.x, upSpeed);
                rb.AddForce(Vector3.up * upSpeed, ForceMode2D.Impulse);
            }
            else
            {
                rb.velocity = Vector2.right * moveSpeed;
                //rb.AddForce(-Vector3.up * 10f * Time.deltaTime);
                //rb.AddForce(moveSpeed * Vector2.right * Time.deltaTime);
                /*Vector2 velocity = rb.velocity;
                velocity.y += Physics2D.gravity.y * 70 * Time.deltaTime;
                rb.velocity = velocity;*/
            }
        }
    }

    void FixedUpdate()
    {
        //rb.velocity = moveSpeed * (rb.velocity.normalized);
        
    }

    public void SetAttachedPlayer()
    {
        //isAttached = true;
        rb.isKinematic = false;
        PM.SetNewPlayerState(States.STATE_FLYING);
        joint.enabled = true;
        joint.anchor = Vector2.zero;
        joint.connectedBody = playerRb;
       // playerRb.isKinematic = true;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            OnDeath();
        }
        else if (other.CompareTag("Player") && PM.CanCollectObjects())
        {
            SetAttachedPlayer();
        }
    }

    public void OnDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
        // isAttached = false;
       
    }

    private void OnExplosive()
    {
        PM.Jump(PM.grabberJumpForce);
    }
}
