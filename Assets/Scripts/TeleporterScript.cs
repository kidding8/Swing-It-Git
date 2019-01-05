using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
{
    private GameManager GM;
    private AuxManager aux;
    private EffectsManager EM;
    private PlayerManager PM;
    private LineRenderer line;
    private GameObject destinyGrabber;
    private Vector2 destiny;
    private GameObject player;
    private Rigidbody2D playerRb;
    private bool isAttachedToPlayer = false;
    private bool isDone = false;
    private bool renderDisJoint = false;

    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        line = GetComponent<LineRenderer>();
        player = aux.GetPlayer();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isAttachedToPlayer && !isDone)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, PM.ropeSpeed);

            if ((Vector2)transform.position == destiny)
            {

                TeleportPlayer();

            }
        }
        else if ((Vector2)transform.position != destiny && !isAttachedToPlayer)
        {
            LeftHookBeforeDestination();
        }

        RenderLine();
    }

    private void TeleportPlayer()
    {
        PM.SetNewPlayerState(States.STATE_TELEPORT);
        isDone = true;
        isAttachedToPlayer = true;
        PM.TeleportToPoint(destinyGrabber.transform);
        playerRb.velocity = new Vector3(0.5f, 0.8f, 0f) * 20f;
        DestroyTeleporter();
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

    public void CreateTeleporterDestiny(GameObject grabber)
    {
        if (grabber != null)
        {
            isAttachedToPlayer = true;
            destinyGrabber = grabber;
            destiny = grabber.transform.position;
            renderDisJoint = true;
        }
    }

    public void DestroyTeleporter()
    {
        PM.SetNewPlayerState(States.STATE_NORMAL);
        isAttachedToPlayer = false;
        renderDisJoint = false;
    }
}
