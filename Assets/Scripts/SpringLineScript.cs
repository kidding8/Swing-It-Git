﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringLineScript : MonoBehaviour
{
    private AuxManager aux;
    private Transform target;
    private Rigidbody2D targetRb;
    private Rigidbody2D boxRb;
   // private LineRenderer line;
    private SpringJoint2D spring;
    private GameObject box;
    private bool isStarted = false;
    void Start()
    {
        aux = AuxManager.instance;
        target = transform.GetChild(0);
        targetRb = target.GetComponent<Rigidbody2D>();
        //line = GetComponent<LineRenderer>();
        spring = GetComponent<SpringJoint2D>();
        //SetNewLine();
        //isStarted = true;
    }

    // Update is called once per frame
    void Update()
    {

        /*if (spring.attachedRigidbody != boxRb)
            line.SetPosition(1, box.transform.position);
        else
            RemoveNewLine();*/

        /*if(spring.attachedRigidbody != boxRb)
        {
            RemoveNewLine();
        }*/
    }

    public void SetNewLine()
    {
        /*if (isStarted)
        {*/
           box = aux.GetBoxPool().GetPooledObject();
            /*line.positionCount = 2;
            line.SetPosition(0, transform.position);*/
            box.SetActive(true);
            //targetRb.isKinematic = true;
            boxRb = box.GetComponent<Rigidbody2D>();
            box.transform.position = new Vector3(transform.position.x, transform.position.y -3, transform.position.z);
            //newBox.transform.parent = transform;
            /*if(spring == null)
            {
                spring = gameObject.AddComponent<SpringJoint2D>();
                Debug.Log("Added");
            }*/

            spring.connectedBody = boxRb;
            //spring.connectedAnchor = newBox.transform.position;
        //}
    }

    private void RemoveNewLine()
    {
        targetRb.isKinematic = false;
        spring.connectedBody = targetRb;
        //line.SetPosition(1, target.position);
    }

    private void OnEnable()
    {

        //Debug.Log("Started new thing");
        if (isStarted)
        {
            //Debug.Log("ENABLED");
            SetNewLine();
        }
            
    }
}