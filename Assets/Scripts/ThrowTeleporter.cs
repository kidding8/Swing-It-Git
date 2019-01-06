using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowTeleporter : MonoBehaviour
{
    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    private Rigidbody2D rb;
    private bool isPressingTeleporter = false;
    public ObjectPooler teleporterPool;
    private GameObject currentTeleporter;
    private TeleporterScript teleporterScript;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && PM.playerState == States.STATE_NORMAL && PM.playerPower == Power.POWER_TELEPORT)
        {

            if (PM.CanHook())
            {
                AddTeleporter();
            }

        }
        else if (Input.GetMouseButtonUp(0) && isPressingTeleporter)
        {
            DestroyTeleporter();
            //AuxManager.instance.GetCamera().GetComponent<CameraFollow>().AddTarget(transform);
        }
    }

    private void AddTeleporter()
    {
        isPressingTeleporter = true;
        GameObject closestGrabber = PM.GetCurrentGrabbableObject();

        if (closestGrabber != null)
        {
            CreateTeleporter(closestGrabber);
        }
    }
    */
    
}
