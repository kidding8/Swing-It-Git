using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

   

    public float limitCameraTop = 30f;
    public float limitCameraBottom = 30f;
    
    //public Transform target;
    public Vector3 offset;
    private Vector3 newOffset;
    public float dampTime = 0.15f;
    public float maxZoom = 14f;
    public float minZoom = 8f;
    public float offsetDistance = 4f;

    private List<Transform> targets;
    private Transform playerTrans;
    //private Transform hookTrans;
    private Rigidbody2D playerRb;
    private Vector3 velocity = Vector3.zero;
    private Camera cam;
    private AuxManager aux;
    private Vector3 refVelocity;

    //private Rigidbody2D playerRb;


   /* public float nearCamZoom = 0.5f;
    public float farCamZoom = 1f;
    public float normalSpeed = 1f;
    public float maxSpeed = 10f;
    public float timeToZoom = 1f;*/
    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        aux = AuxManager.instance;
       
        targets = new List<Transform>();
        targets.Add(aux.GetPlayer().transform);
        playerTrans = aux.GetPlayer().transform;

        /*target = aux.GetPlayer().transform;*/
        playerRb = playerTrans.GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        if (playerTrans == null)
            return;
        Move();
        Zoom();
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(minZoom, maxZoom, (GetGreatestDistance() + offsetDistance) / 30);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newZoom, Time.deltaTime);
        //Debug.Log("Distance: " + GetGreatestDistance());
    }

    private void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        newOffset.z = 0;
        if (targets.Count <= 1)
        {
           newOffset.y = GetOffsetVertical();
        }
        newOffset.x = offset.x;
        centerPoint.z = transform.position.z;
        Vector3 newCenter = centerPoint + newOffset;
        Vector3 newPos;
        if(targets.Count <= 1)
        {
            newPos = Vector3.SmoothDamp(transform.position, newCenter, ref refVelocity, dampTime - 0.3f);
        }
        else
        {
            newPos = Vector3.SmoothDamp(transform.position, newCenter, ref refVelocity, dampTime);
        }
        

        float maxY = limitCameraTop - cam.orthographicSize;
        float minY = limitCameraBottom + cam.orthographicSize;
        transform.position = new Vector3(newPos.x, Mathf.Clamp(newPos.y, -minY, maxY), newPos.z);
    }

    private float GetGreatestDistance()
    {

        if (targets.Count <= 1)
        {
            return 0;
        }

        //Vector3 average = playerTrans.position + hookTrans.position;
        /* var bounds = new Bounds(targets[0].position, Vector3.zero);
         for (int i = 0; i < targets.Count; i++)
         {
             bounds.Encapsulate(targets[i].position);
         }
         return bounds.size.x;*/
        float farthestDistance = 0;
        Transform farthestObject = null;
        for (int i = 0; i < targets.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, targets[i].transform.position);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestObject = targets[i];
            }
        }

        return Vector3.Distance(playerTrans.position, farthestObject.position);
    }


    private float GetOffsetVertical()
    {
        return playerRb.velocity.y > 0 ? offset.y : offset.y * -1;
        
        
    }

    private Vector3 GetCenterPoint()
    {
        /*if(hookTrans == null)
        {
            return playerTrans.position;
        }

        Vector3 average = playerTrans.position + hookTrans.position;
        */


        if(targets.Count == 1)
        {
            return targets[0].position;
        }

        Vector3 average = Vector3.zero;
        for (int i = 0; i < targets.Count; i++)
        {
            average += targets[i].position;
        }


        /*var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }*/
        return average/ targets.Count;
    }

    public void AddTarget(Transform newTarget)
    {
        //hookTrans = newTarget;
        if(newTarget != null)
            targets.Add(newTarget);
    }

    public void RemoveTarget(Transform newTarget)
    {
        if(targets.Contains(newTarget))
            targets.Remove(newTarget);
        //hookTrans = null;
    }

    private void MoveCamToPlayer()
    {
       /* Vector3 newTarget = new Vector3(target.position.x + offsetX, target.position.y, target.position.z);

        Vector3 point = cam.WorldToViewportPoint(newTarget);
        Vector3 delta = newTarget - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
        Vector3 destination = transform.position + delta;
        Vector3 newPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);

        float maxY = limitCameraTop - orthoSize;
        float minY = limitCameraBottom + orthoSize;
        transform.position = new Vector3(newPos.x, Mathf.Clamp(newPos.y, -minY, maxY), newPos.z);*/
    }

    private void ZoomOut()
    {
        /*float vel = playerRb.velocity.x;
        float f = Mathf.InverseLerp(0f, maxSpeed, vel);
        //f = Mathf.SmoothStep(0, farCamZoom, f);


        f = Mathf.Lerp(orthoSize, f, timeToZoom);
        cam.orthographicSize = Mathf.Lerp(nearCamZoom, farCamZoom, f);*/
    }

    public int GetTargetsNum()
    {
        return targets.Count;
    }
}
