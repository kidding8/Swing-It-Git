using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberBox : MonoBehaviour
{
    private AuxManager aux;
    private LineRenderer line;

    private GameObject box;
    private Transform[] childTrans;
    private bool canSpawnIt = false;
    private HingeJoint2D lastNodeJoint;
    void Start()
    {
        aux = AuxManager.instance;
        line = GetComponent<LineRenderer>();
        StartChilds();
        //SetNewLine();
        //isStarted = true;
        
    }

    private void Update()
    {
        if (canSpawnIt)
        {
            CreateNewBox();
            canSpawnIt = false;
        }

        DrawLine();
        
    }

    private void OnDisable()
    {
        if(lastNodeJoint != null)
            lastNodeJoint.enabled = true;
        canSpawnIt = true;
    }

    private void StartChilds()
    {
        int count = transform.childCount;
        childTrans = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            childTrans[i] = transform.GetChild(i).transform;
        }
        int lenght = childTrans.Length-1;
        line.positionCount = lenght;
        lastNodeJoint = childTrans[lenght].GetComponent<HingeJoint2D>();
    }

    private void DrawLine()
    {
        for (int i = 0; i < childTrans.Length -1; i++)
        {
            line.SetPosition(i, childTrans[i].transform.position);
        }
    }

    public void CreateNewBox()
    {

        box = aux.bouncyBallPool.GetPooledObject();
        box.SetActive(true);
        box.transform.position = childTrans[childTrans.Length - 1].position;
        if (lastNodeJoint != null)
        {
            lastNodeJoint.enabled = true;
            Rigidbody2D rbbox = box.GetComponent<Rigidbody2D>();
            rbbox.velocity = Vector2.zero;
            lastNodeJoint.connectedBody = rbbox;
        }
        else
        {
            lastNodeJoint = childTrans[childTrans.Length - 1].gameObject.AddComponent<HingeJoint2D>();
            Rigidbody2D rbbox = box.GetComponent<Rigidbody2D>();
            lastNodeJoint.breakForce = 100;
            lastNodeJoint.breakTorque = 100;
            rbbox.velocity = Vector2.zero;
            lastNodeJoint.connectedBody = rbbox;
        }
    }

    private void RemoveNewLine()
    {
      
        //line.SetPosition(1, target.position);
    }

    /*private void OnEnable()
    {

        //Debug.Log("Started new thing");
        if (isStarted)
        {
            //Debug.Log("ENABLED");
            SetNewLine();
        }
            
    }*/
}
