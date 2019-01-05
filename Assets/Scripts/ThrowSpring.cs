using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowSpring : MonoBehaviour
{
    private GameManager GM;
    private EffectsManager EM;
    private PlayerManager PM;
    private Rigidbody2D rb;
    private bool isPressingSpring = false;
    public ObjectPooler springPool;
    private GameObject currentSpring;
    private SpringScript springScript;
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
        if (Input.GetMouseButtonDown(0) && GM.isPlaying() && PM.playerState == States.STATE_NORMAL && PM.playerPower == Power.POWER_SPRING)
        {

            if (PM.CanCollect())
            {
                AddSpring();
            }

        }
        else if (Input.GetMouseButtonUp(0) && isPressingSpring)
        {
            DestroySpring();
            //AuxManager.instance.GetCamera().GetComponent<CameraFollow>().AddTarget(transform);
        }
    }

    private void AddSpring()
    {
        isPressingSpring = true;
        GameObject closestGrabber = PM.GetCurrentGrabbableObject();

        if (closestGrabber != null)
        {
            CreateSpring(closestGrabber);
        }
    }

    private void CreateSpring(GameObject grabber)
    {
        currentSpring = springPool.GetPooledObject();
        currentSpring.transform.position = transform.position;
        currentSpring.transform.rotation = Quaternion.identity;
        currentSpring.SetActive(true);
        springScript = currentSpring.GetComponent<SpringScript>();
        springScript.CreateSpring(grabber);
    }

    private void DestroySpring()
    {
        isPressingSpring = false;
        if (springScript != null)
            springScript.DestroySpring();
    }
}
