using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverPlayer : MonoBehaviour
{
    private AuxManager aux;
    private GameObject player;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        aux = AuxManager.instance;
        player = aux.GetPlayer();
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = player.transform.TransformPoint(new Vector2(-0.5f, 0.5f));

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
