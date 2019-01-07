using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleScript : MonoBehaviour
{
    private GameManager GM;
    private AuxManager aux;
    private EffectsManager EM;
    private PlayerManager PM;
    private DistanceJoint2D disJoint;
    private LineRenderer line;
    private GameObject destinyGrabber;
    private Vector2 destiny;
    private GameObject player;
    private Rigidbody2D playerRb;
    private Rigidbody2D rb;
    private GameObject targetToDraw;
    public float step = 20f;
    private bool isAttachedToPlayer = false;
    private bool isDone = false;
    private bool renderDisJoint = false;


    private bool alreadyJumped = false;
    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        player = aux.GetPlayer();
        playerRb = player.GetComponent<Rigidbody2D>();
        //disJoint.enabled = false;
        targetToDraw = player;
    }

    void Update()
    {
        if (isAttachedToPlayer && !isDone)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, PM.ropeSpeed);

            if ((Vector2)transform.position == destiny)
            {

               AttachGrapple();

            }
        }
        else if ((Vector2)transform.position != destiny && !isAttachedToPlayer)
        {
            LeftHookBeforeDestination();

        }else if(isAttachedToPlayer && isDone)
        {
            if(disJoint.distance >= 1f)
            {
                disJoint.distance -= step * Time.deltaTime;
            }

        }else if(!isAttachedToPlayer && disJoint.distance >= 0.4f)
        {
            disJoint.distance -= (step * 2) * Time.deltaTime;
        }

        RenderLine();
    }

    private void AttachGrapple()
    {
        disJoint.enabled = true;
        //float dist = Vector2.Distance(transform.position, destiny);
        disJoint.autoConfigureDistance = true;
        disJoint.maxDistanceOnly = false;
        //disJoint.distance = Vector2.Distance(transform.position, destiny);
        disJoint.connectedBody = destinyGrabber.GetComponent<Rigidbody2D>();
        PM.SetNewPlayerState(States.STATE_GRAPPLE);
        isDone = true;
    }

    private void LeftHookBeforeDestination()
    {
        rb.isKinematic = false;
        if (PM.currentJumps > 0)
        {
            PM.BigJump();
            PM.currentJumps--;
        }

        alreadyJumped = true;
    }

    private void RenderLine()
    {
        if (renderDisJoint)
        {
            line.positionCount = 2;
            line.SetPosition(0, player.transform.position);
            line.SetPosition(1, transform.position);
        }

    }

    public void CreateGrapple(GameObject grabber, DistanceJoint2D dist)
    {
        if (grabber != null)
        {
            isAttachedToPlayer = true;
            destinyGrabber = grabber;
            destiny = grabber.transform.position;
            renderDisJoint = true;
            disJoint = dist;
        }
    }

    public void DestroyGrapple()
    {
        PM.SetNewPlayerState(States.STATE_NORMAL);
        isAttachedToPlayer = false;
        //disJoint.enabled = false;
        //renderDisJoint = false;
    }

}