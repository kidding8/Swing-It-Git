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
    public float step = 20f;
    private bool isAttachedToPlayer = false;
    private bool isDone = false;
    private bool renderDisJoint = false;

    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        //disJoint = GetComponent<DistanceJoint2D>();
        line = GetComponent<LineRenderer>();
        player = aux.GetPlayer();
        playerRb = player.GetComponent<Rigidbody2D>();
        //disJoint.enabled = false;
    }

    void Update()
    {
        if (isAttachedToPlayer && !isDone)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, PM.ropeSpeed);

            if ((Vector2)transform.position == destiny)
            {

               AttachDistanceJoint();

            }
        }
        else if ((Vector2)transform.position != destiny && !isAttachedToPlayer)
        {
            LeftHookBeforeDestination();
        }else if(isAttachedToPlayer && isDone)
        {
            if(disJoint.distance >= 1f)
            {

            }
                //disJoint.distance -= 0.2f;
        }

        RenderLine();
    }

    private void AttachDistanceJoint()
    {
        disJoint.enabled = true;
        //float dist = Vector2.Distance(transform.position, destiny);
        disJoint.autoConfigureDistance = true;
        disJoint.maxDistanceOnly = true;
        //disJoint.distance = Vector2.Distance(transform.position, destiny);
        disJoint.connectedBody = destinyGrabber.GetComponent<Rigidbody2D>();
        
        PM.SetNewPlayerState(States.STATE_GRAPPLE);
        isDone = true;
        //isAttachedToPlayer = true;
    }

    private void LeftHookBeforeDestination()
    {

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

    public void CreateDistanceJoint(GameObject grabber, DistanceJoint2D dist)
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

    public void DestroyDistanceJoint()
    {
        PM.SetNewPlayerState(States.STATE_NORMAL);
        isAttachedToPlayer = false;
        disJoint.enabled = false;
        renderDisJoint = false;
    }
}