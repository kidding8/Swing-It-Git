using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{

    private GameManager GM;
    private AuxManager aux;
    private EffectsManager EM;
    private PlayerManager PM;
    private Vector2 destiny = Vector2.zero;
    private GameObject attachedHook;
    public float disToDestroyX = 300f;
    public bool goTowardsTarget = false;
    //private GameObject targetToFollow = null;
    private ObjectPooler nodePool;
    private GameObject player;
    private GameObject lastNode;

    private List<GameObject> nodeList;

    private bool isDone = false;
    private LineRenderer line;

    private bool isAttachedToPlayer = false;
    private Vector3 screenPos;
    private Camera cam;

    private bool alreadyJumped = false;
    private bool particlesDone = false;
    //private ThrowHook playerThrowHook;
    private CameraFollow camFollow;
    private Rigidbody2D rb;
    private bool useLine = false;
    private Transform firstNode = null;
    //private Grabber targetGrabber = null;
    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        cam = aux.GetCamera();
        player = aux.GetPlayer();
        //playerThrowHook = player.GetComponent<ThrowHook>();
        lastNode = gameObject;
        nodePool = aux.GetNodePool();
        nodeList = new List<GameObject>();
        nodeList.Add(transform.gameObject);
        line = GetComponent<LineRenderer>();
        camFollow = cam.GetComponent<CameraFollow>();
        rb = GetComponent<Rigidbody2D>();
        //edgeCol = GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        if (isAttachedToPlayer && !isDone)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, PM.ropeSpeed);
            if ((Vector2)transform.position == destiny)
            {
                GrabbedHook();
            }
        }
        else if ((Vector2)transform.position != destiny && !alreadyJumped)
        {
            LeftHookBeforeDestination();
        }

        if ((Vector2)transform.position != destiny && isAttachedToPlayer && !isDone)
        {
            if (Vector2.Distance(player.transform.position, lastNode.transform.position) > PM.ropeDistance)
            {
                CreateNode();
            }
        }

        if (useLine && firstNode != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, firstNode.transform.position);
        }
    }

    private void GrabbedHook()
    {
        isDone = true;
        
        while (Vector2.Distance(player.transform.position, lastNode.transform.position) > PM.ropeDistance)
        {
            CreateNode();
        }
        
        HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
        hingeLastNode.enabled = true;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        lastNode.GetComponent<HingeJoint2D>().connectedBody = playerRb;

        NodeScript nodeScript = lastNode.GetComponent<NodeScript>();

        if (nodeScript != null)
        {
            nodeScript.SetNewLineTarget(player.transform);
        }

        PM.SetNewHook(gameObject);

        if (!particlesDone)
        {
            particlesDone = true;
            EM.CreateStarGrabber(attachedHook.transform.position);
        }


        // camFollow.AddTarget(transform);

        /*GameObject node = nodePool.GetPooledObject();
        node.transform.position = player.transform.position;
        node.transform.rotation = Quaternion.identity;
        node.transform.SetParent(transform);


        node.SetActive(true);
        NodeScript nodeScript = lastNode.GetComponent<NodeScript>();
        if (nodeScript != null)
        {
            nodeScript.SetNewLineTarget(player.transform);
        }*/




        //PM.SetNewLineTarget(lastNode.transform);



        //transform.DetachChildren();

    }

    private void LeftHookBeforeDestination()
    {
        DesattachRopeFromHook();
        alreadyJumped = true;
    }

    public void DesattachRopeFromHook()
    {
        RopeIsDestroyed();
        rb.isKinematic = false;
    }

    public void AddRope(GameObject target)
    {
        isAttachedToPlayer = true;
        //rb.isKinematic = true;
        attachedHook = target;
        lastNode = gameObject;
        isDone = false;
        useLine = false;
        particlesDone = false;
        //targetGrabber = attachedHook.GetComponent<Grabber>();
        /*if (targetGrabber != null)
            targetGrabber.AddRope(this);*/
        destiny = target.transform.position;
    }

    void CreateNode()
    {

        Vector2 pos2Create = player.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= PM.ropeDistance;
        pos2Create += (Vector2)lastNode.transform.position;

        GameObject node = nodePool.GetPooledObject();
        node.transform.position = pos2Create;
        node.transform.rotation = Quaternion.identity;
        node.transform.SetParent(transform);

        
        node.SetActive(true);

        Rigidbody2D nodeRb = node.GetComponent<Rigidbody2D>();

        HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
        hingeLastNode.enabled = true;
        hingeLastNode.anchor = Vector2.zero;
        hingeLastNode.connectedBody = nodeRb;

        
        if (!useLine)
        {
            useLine = true;
            firstNode = node.transform;
            line.positionCount = 2;
        }
        else
        {
            NodeScript nodeScript = lastNode.GetComponent<NodeScript>();
            if (nodeScript != null)
            {
                nodeScript.SetNewLineTarget(node.transform);
            }
        }

        lastNode = node;

        nodeList.Add(lastNode);
    }

    public void RopeIsDestroyed()
    {
        if (isAttachedToPlayer)
        {
            UnhookRope();
        }
    }

    public void UnhookRope()
    {

        if (PM != null)
            PM.RemoveHook();
       if (camFollow != null)
            camFollow.RemoveTarget(transform);
        if (lastNode != null)
            lastNode.GetComponent<HingeJoint2D>().enabled = false;
        NodeScript nodeScript = lastNode.GetComponent<NodeScript>();
        if (nodeScript != null)
        {
            nodeScript.RemoveLineTarget();
        }
        isAttachedToPlayer = false;
        isDone = true;
       // transform.DetachChildren();
    }

    public void DisableRope()
    {

        rb.isKinematic = true;
        gameObject.SetActive(false);
        transform.DetachChildren();
        nodeList.Clear();
        /*for (int i = 1; i < nodeList.Count; i++)
        {

            nodeList[i].GetComponent<NodeScript>().DestroyNode();
            //nodeList[i].transform.parent = null;
            //nodeList[i].gameObject.SetActive(false);
            
        }*/

        /*foreach(Transform child in transform)
        {

            NodeScript node = child.GetComponent<NodeScript>();
            if(node != null)
            {
                node.RemoveLineTarget();
            }
            child.transform.parent = null;
            child.gameObject.SetActive(false);
        }*/
        //int i = 0;
        /*while(nodeList.Count > 1)
        {
            
            nodeList[i].GetComponent<NodeScript>().RemoveLineTarget();
            nodeList.RemoveAt(i);
            i++;
        }*/

        // rb.isKinematic = true;
        /*if (lastNode != null)
            lastNode.GetComponent<HingeJoint2D>().connectedBody = null;
        particlesDone = false;
        isAttachedToPlayer = false;
        isDone = false;
        line.positionCount = 0;
        useLine = false;
        firstNode = null;*/
        //lastNode = gameObject;
        //isDone = false;
        //



    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && isAttachedToPlayer)
        {
            other.gameObject.SetActive(false);
        }
        else if (other.CompareTag("Wall"))
        {
            DisableRope();
        }
        else if (other.CompareTag("Ground") && isAttachedToPlayer)
        {
            LeftHookBeforeDestination();
            UnhookRope();
        }
    }

    /*public void SetLayerRecursively(int layerNumber)
    {
        foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }*/

    /*public void AddRope(Vector2 destiny, bool noTarget)
    {
        isAttachedToPlayer = true;
        this.noTarget = noTarget;
        attachedHook = null;
        this.destiny = destiny;
    }*/

    /* public void AddRopeToTarget(GameObject hook)
     {
         isAttachedToPlayer = true;
         this.noTarget = false;
         attachedHook = hook;
         goTowardsTarget = true;
         this.destiny = hook.transform.position;
     }*/
}
