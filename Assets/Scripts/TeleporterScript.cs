using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterScript : MonoBehaviour
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
        PM.SetPlayerState(States.STATE_TELEPORT);
        isDone = true;
        Vector3 dir = destinyGrabber.transform.position - player.transform.position;
        PM.AddDirectionalVelocity(dir.normalized, 20f);
        PM.TeleportToPoint(destinyGrabber.transform);
        DestroyTeleporter();
    }

    private void LeftHookBeforeDestination()
    {
        PM.DoAirPower();
    }

    public void CreateTeleporterDestiny(GameObject grabber)
    {
        if (grabber != null)
        {
            isAttachedToPlayer = true;
            isDone = false;
            destinyGrabber = grabber;
            destiny = grabber.transform.position;
        }
    }

    public void DestroyTeleporter()
    {
        PM.SetPlayerState(States.STATE_NORMAL);
        isAttachedToPlayer = false;
        isDone = true;
        gameObject.SetActive(false);
    }
}
