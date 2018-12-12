using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public float dampTime = 0.15f;
    private Vector3 velocity = Vector3.zero;


    public float limitCameraTop = 30f;
    public float limitCameraBottom = 30f;
    private float orthoSize;


    private Transform target;
    public float offsetY = 10;
    public float offsetX = 10f;
    private Camera cam;
    private AuxManager aux;
    private Rigidbody2D playerRb;


    public float nearCamZoom = 0.5f;
    public float farCamZoom = 1f;
    public float normalSpeed = 1f;
    public float maxSpeed = 10f;
    public float timeToZoom = 1f;
    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        aux = AuxManager.instance;
        orthoSize = cam.orthographicSize;
        target = aux.GetPlayer().transform;
        playerRb = target.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
   
        Vector3 newTarget = new Vector3(target.position.x + offsetX, target.position.y, target.position.z);
        /*if (!GM.isContinueMenu)
        {*/

        Vector3 point = cam.WorldToViewportPoint(newTarget);
        Vector3 delta = newTarget - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
        Vector3 destination = transform.position + delta;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

        //}

        float maxY = limitCameraTop - orthoSize;
        float minY = limitCameraBottom + orthoSize;
        transform.position = new Vector3(newPos.x, Mathf.Clamp(newPos.y, -minY, maxY), newPos.z);



        


        /*float t = Mathf.InverseLerp(normalSpeed, maxSpeed, GM.GetPlayer().GetComponent<Rigidbody2D>().velocity.magnitude); // returns a value between 0-1.
        t = Mathf.SmoothStep(0, 1, t); // smooth out the t value so that the camera will ease in and out nicely.
        Vector3 cameraPosition = Vector3.Lerp(nearCamZoom, farCamZoom, t);*/

        /*else
        {
            Vector3 point = cam.WorldToViewportPoint(target.transform.position);
            Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }*/

    }

    private void LateUpdate()
    {
        float vel = playerRb.velocity.x;
        float f = Mathf.InverseLerp(0f, maxSpeed, vel);
        //f = Mathf.SmoothStep(0, farCamZoom, f);


        f = Mathf.Lerp(orthoSize, f, timeToZoom);
        cam.orthographicSize = Mathf.Lerp(nearCamZoom, farCamZoom, f);
    }
}
