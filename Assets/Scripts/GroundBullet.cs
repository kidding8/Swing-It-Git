using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundBullet : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistanceDetectPlayer = 20f;
    private AuxManager aux;
    private CameraFollow camFollow;
    private Transform player;
    private bool isAlreadyATarget = false;
    // Start is called before the first frame update
    void Start()
    {
        aux = AuxManager.instance;
        player = aux.GetPlayer().transform;
        camFollow = aux.GetCamera().GetComponent<CameraFollow>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, player.position) < maxDistanceDetectPlayer)
        {
            if (!isAlreadyATarget)
            {
                camFollow.AddTarget(transform);
                isAlreadyATarget = true;
            }
        }
        else if (isAlreadyATarget)
        {
            RemoveCamTarget();
        }

        transform.Translate(transform.right * speed * Time.deltaTime);
    }

    private void RemoveCamTarget()
    {
        camFollow.RemoveTarget(transform);
        isAlreadyATarget = false;
    }
}
