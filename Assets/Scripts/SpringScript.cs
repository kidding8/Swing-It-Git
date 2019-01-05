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
    private bool isAttachedToPlayer = false;
    private bool isDone = false;
    private bool renderSpring = false;
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
        else if ((Vector2)transform.position != destiny)
        {
            LeftHookBeforeDestination();
        }

        RenderLine();
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

    }

    private void RenderLine()
    {
        if (renderSpring)
        {
            line.positionCount = 2;
            line.SetPosition(0, player.transform.position);
            line.SetPosition(1, transform.position);
        }
        
    }

    public void CreateSpring(GameObject grabber)
    {
        if(grabber != null)
        {
            isAttachedToPlayer = true;
            destinyGrabber = grabber;
            destiny = grabber.transform.position;
            renderSpring = true;
        }
    }

    public void DestroySpring()
    {
        PM.SetNewPlayerState(States.STATE_NORMAL);
        isAttachedToPlayer = false;
        spring.enabled = false;
        renderSpring = false;
    }
}
