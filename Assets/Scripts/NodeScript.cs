using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    private LineRenderer line;
    private bool alreadyUsed = false;
    private bool useLine = false;

    public Transform targetNode;
    private PlayerManager PM;
    private HingeJoint2D hinge2D;
    
    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        hinge2D = GetComponent<HingeJoint2D>();
        PM = PlayerManager.instance;
        
    }

    public void SetNewLineTarget(Transform target)
    {
        if (target == null)
        {
            Debug.Log("FUCKEDDDD NO NEW LINE");
            return;
        }
            
            

        line.enabled = true;
        line.positionCount = 2;
        targetNode = target.transform;
        useLine = true;
        hinge2D.enabled = true;
        /*Rigidbody2D targetRb = target.GetComponent<Rigidbody2D>(); 
        if(targetRb != null)
        {
            hinge2D.enabled = true;
            hinge2D.connectedBody = targetRb;
        }
        else
        {
            Debug.Log("No RB");
        }*/
        
    }
    
    public void RemoveLineTarget()
    {
        
        //line.positionCount = 0;
        line.enabled = false;
        useLine = false;
        targetNode = null;
        hinge2D.connectedBody = null;
        hinge2D.enabled = false;
        //transform.parent = null;
        //gameObject.SetActive(false);
        
    }


    public void DestroyNode()
    {
        
        transform.parent = null;
        gameObject.SetActive(false);
        alreadyUsed = true;
    }

    private void Update()
    {
        if(useLine && targetNode != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, targetNode.transform.position);
            /*if (alreadyUsed)
            {
                line.startColor = Color.green;
                line.endColor = Color.green;
            }*/
        }
        /*Vector3 screenPosition = AuxManager.instance.GetCamera().WorldToScreenPoint(transform.position);
        if (screenPosition.x < 300)
        {
            RemoveLineTarget();
            DestroyNode();
        }*/
            /*if(hinge2D.connectedBody == null)
            {
                RemoveLineTarget();
                DestroyNode();
            }
            else
            {
                line.positionCount = 0;
            }*/
        }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
           if(PM.destroyRope) {
                RemoveLineTarget();
                
                RopeScript ropeScript = GetComponentInParent<RopeScript>();
                if (ropeScript != null)
                    ropeScript.RopeIsDestroyed();
                //DestroyNode();
            }
            
        }else if (other.CompareTag("Wall"))
        {
            
           // RemoveLineTarget();
            //DestroyNode();
            //transform.parent = null;
            /* RemoveLineTarget();
             gameObject.SetActive(false);*/
            /*alreadyUsed = true;
            gameObject.SetActive(false);*/
            //RemoveLineTarget();

        }
    }
}
