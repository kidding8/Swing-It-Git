using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {

    private LineRenderer line;
    private bool useLine = false;
    private Transform targetNode;
    private PlayerManager PM;
    private HingeJoint2D hinge2D;
    
    private void Start()
    {
        line = GetComponent<LineRenderer>();
        hinge2D = GetComponent<HingeJoint2D>();
        PM = PlayerManager.instance;
    }

    public void SetNewLineTarget(Transform target)
    {
        line.enabled = true;
        line.positionCount = 2;
        targetNode = target;
        useLine = true;
    }
    
    public bool isUsed()
    {
        return useLine;
    }
    public void RemoveLineTarget()
    {
        //line.positionCount = 0;
        line.enabled = false;
        useLine = false;
        targetNode = null;
    }

    private void Update()
    {
        if(useLine && targetNode != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, targetNode.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            //gameObject.SetActive(false);
           if(PM.destroyRope) {
                RemoveLineTarget();
                hinge2D.enabled = false;
                RopeScript ropeScript = GetComponentInParent<RopeScript>();
                if (ropeScript != null)
                    ropeScript.DestroyedRope();
            }
            
        }
    }
}
