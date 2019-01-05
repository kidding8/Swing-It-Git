using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowGrapple : MonoBehaviour
{
    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    private Rigidbody2D rb;
    private bool isPressingGrapple = false;
    public ObjectPooler grapplePool;
    private GameObject currentGrapple;
    private GrappleScript grappleScript;
    // Start is called before the first frame update
    void Start()
    {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && PM.playerState == States.STATE_NORMAL && PM.playerPower == Power.POWER_GRAPPLE)
        {

            if (PM.CanCollect())
            {
                AddDistanceJoint();
            }

        }
        else if (Input.GetMouseButtonUp(0) && isPressingGrapple)
        {
            DestroyDistanceJoint();
            //AuxManager.instance.GetCamera().GetComponent<CameraFollow>().AddTarget(transform);
        }
    }

    private void AddDistanceJoint()
    {
        isPressingGrapple = true;
        GameObject closestGrabber = PM.GetCurrentGrabbableObject();

        if (closestGrabber != null)
        {
            CreateDistanceJoint(closestGrabber);
        }
    }

    private void CreateDistanceJoint(GameObject grabber)
    {
        currentGrapple = grapplePool.GetPooledObject();
        currentGrapple.transform.position = transform.position;
        currentGrapple.transform.rotation = Quaternion.identity;
        currentGrapple.SetActive(true);
        grappleScript = currentGrapple.GetComponent<GrappleScript>();
        grappleScript.CreateDistanceJoint(grabber);
    }

    private void DestroyDistanceJoint()
    {
        isPressingGrapple = false;
        if (grappleScript != null)
            grappleScript.DestroyDistanceJoint();
    }
}
