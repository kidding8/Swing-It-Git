using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringScript : MonoBehaviour
{
    private GameManager GM;
    private AuxManager aux;
    private EffectsManager EM;
    private PlayerManager PM;
    private SpringJoint2D spring;
    private DistanceJoint2D disJoint;
    private LineRenderer line;
    private GameObject destinyGrabber;
    private Vector2 destiny;
    private GameObject player;
    private Rigidbody2D playerRb;
    private Grabber targetGrabber;
    private GameObject targetToDraw = null;
    private Rigidbody2D rb;
    private bool isAttachedToPlayer = false;
    private bool isDone = false;
    private bool renderSpring = false;
    private bool alreadyJumped = false;
    private bool retractSpring = false;
    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        spring = GetComponent<SpringJoint2D>();
        disJoint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        player = aux.GetPlayer();
        playerRb = player.GetComponent<Rigidbody2D>();
        spring.enabled = false;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttachedToPlayer && !isDone)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, PM.ropeSpeed);

            if ((Vector2)transform.position == destiny)
            {

                AttachSpring();

            }
        }
        else if ((Vector2)transform.position != destiny && !alreadyJumped)
        {
            LeftHookBeforeDestination();
        }

        RenderLine();

        if (retractSpring && spring.distance > 1f)
        {
            spring.distance -= 0.8f * Time.deltaTime;
        }
    }

    private void AttachSpring()
    {
        spring.enabled = true;
        spring.connectedBody = playerRb;
        PM.SetNewPlayerState(States.STATE_SPRING);
        isDone = true;
        isAttachedToPlayer = true;
    }

    private void LeftHookBeforeDestination()
    {
        rb.isKinematic = false;
        PM.DoAirBoost();

        alreadyJumped = true;
    }

    private void RenderLine()
    {
        if (renderSpring)
        {
            line.positionCount = 2;
            if(targetToDraw != null)
            {
                line.SetPosition(0, targetToDraw.transform.position);
            }
            else
            {
                line.SetPosition(0, player.transform.position);
            }
            
            line.SetPosition(1, transform.position);
        }

    }

    public void CreateSpring(GameObject grabber)
    {
        if (grabber != null)
        {
            isAttachedToPlayer = true;
            destinyGrabber = grabber;
            destiny = grabber.transform.position;
            targetToDraw = null;
            renderSpring = true;
            alreadyJumped = false;
            targetGrabber = destinyGrabber.GetComponent<Grabber>();
            //if (targetGrabber != null)
            //targetGrabber.AddRope(this);
        }
    }

    public void DestroySpring()
    {

        isAttachedToPlayer = false;
        isDone = true;
        PM.SetNewPlayerState(States.STATE_NORMAL);
        if (!retractSpring)
            AddNewEmptyRb();
    }

    private void AddNewEmptyRb()
    {
        retractSpring = true;
        GameObject newEmptyObj = aux.emptyRbPool.GetPooledObject();
        newEmptyObj.SetActive(true);
        newEmptyObj.transform.position = player.transform.position;
        targetToDraw = newEmptyObj;
        //spring.autoConfigureDistance = true;
        //spring.frequency = 1f;
        spring.dampingRatio = 0.2f;
        spring.connectedBody = newEmptyObj.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isAttachedToPlayer)
        {
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Wall"))
        {
            OnDeath();
        }
       
    }

    private void OnDeath()
    {
        isAttachedToPlayer = false;
        renderSpring = false;
        retractSpring = false;
        spring.enabled = false;
        isDone = false;
        rb.isKinematic = true;
        if (targetToDraw != null)
        {
            targetToDraw.gameObject.SetActive(false);
            targetToDraw = null;
        }
        gameObject.SetActive(false);
    }
}
