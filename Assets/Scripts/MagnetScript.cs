using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetScript : MonoBehaviour
{
    private GameManager GM;
    private AuxManager aux;
    private EffectsManager EM;
    private PlayerManager PM;
    private GameObject destinyGrabber;
    private Vector2 destiny;
    private GameObject player;
    private bool isAttachedToPlayer = false;
    private bool isDone = false;

    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        player = aux.GetPlayer();
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
        else if ((Vector2)transform.position != destiny && isAttachedToPlayer)
        {
            LeftHookBeforeDestination();
        }

    }

    private void TeleportPlayer()
    {
        PM.SetNewPlayerState(States.STATE_TELEPORT);
        isDone = true;

        //playerRb.velocity = new Vector3(0.5f, 0.8f, 0f) * 20f;
        Vector3 dir = destinyGrabber.transform.position - player.transform.position;
        //PM.BigJump();
        PM.AddDirectionalVelocity(dir.normalized, 20f);
        PM.TeleportToPoint(destinyGrabber.transform);
        DestroyTeleporter();
    }

    private void LeftHookBeforeDestination()
    {
        PM.DoHability();

    }

    public void CreateTeleporterDestiny(GameObject grabber)
    {
        if (grabber != null)
        {
            isAttachedToPlayer = true;
            destinyGrabber = grabber;
            destiny = grabber.transform.position;
        }

    }

    public void DestroyTeleporter()
    {
        PM.SetNewPlayerState(States.STATE_NORMAL);
        isAttachedToPlayer = false;
        //gameObject.SetActive(false);
    }
}
