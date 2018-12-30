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
    public bool goTowardsHook = false;
    private ObjectPooler nodePool;
    private GameObject player;
    private GameObject lastNode;
    

    private List<GameObject> nodeList;

    private List<GameObject> bottomNodeList;
    private List<GameObject> upperNodeList;

    private bool isDone = false;
    private LineRenderer line;
    private int vertexCount = 2;

    //private EdgeCollider2D edgeCol;
    private bool isAlreadyCut = false;
    private bool drawRope = true;
    private bool isAttachedToPlayer = false;
    private LineRenderer nodeLineRenderer;
    private Vector3 screenPos;
    private Camera cam;
    private int index;

    private bool alreadyJumped = false;
    private bool particlesDone = false;
    private ThrowHook playerThrowHook;
    private bool noTarget;
    private CameraFollow camFollow;
    private Rigidbody2D rb;
    private bool isFirstNode = true;
    private bool useLine = false;
    private Transform firstNode = null;

    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        cam = aux.GetCamera();
        player = aux.GetPlayer();
        playerThrowHook = player.GetComponent<ThrowHook>();
        lastNode = transform.gameObject;
        nodePool = aux.GetNodePool();
        nodeList = new List<GameObject>();
        bottomNodeList = new List<GameObject>();
        upperNodeList = new List<GameObject>();
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
                if ((Vector2)transform.position == destiny )
                {
                    if (!goTowardsHook)
                    {
                        GrabbedHook();
                    } 
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

                MovingTowardsHook();
            }
        //CreateCollider();

        if(useLine && firstNode != null)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, firstNode.transform.position);
        }
    }

    private void GrabbedHook()
    {
        if (!noTarget)
        {
            if (!particlesDone)
            {
                particlesDone = true;
                EM.CreateHookGrabParticle(destiny);
                GM.AddCombo();
            }

            while (Vector2.Distance(player.transform.position, lastNode.transform.position) > PM.ropeDistance)
            {
                CreateNode();
            }

            isDone = true;

            camFollow.AddTarget(transform);

            HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
            hingeLastNode.enabled = true;

            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            lastNode.GetComponent<HingeJoint2D>().connectedBody = playerRb;

            PM.SetNewLineTarget(lastNode.transform);

            PM.SetNewHook(gameObject);
            Grabber grabber = attachedHook.GetComponent<Grabber>();
            grabber.CheckIfTeleporter();
            if (attachedHook.activeInHierarchy)
                grabber.AddRope(this);
            else
            {
                UnhookRope();
                grabber.OnDeath();

            }

            //playerThrowHook.LimitDistance();
            PM.currentJumps = PM.maxRopeJumps;


            /*DistanceJoint2D distanceLastNode = GetComponent<DistanceJoint2D>();
            distanceLastNode.enabled = true;
            distanceLastNode.autoConfigureConnectedAnchor = true;
            distanceLastNode.autoConfigureDistance = true;
            //distanceLastNode.distance = distance;
            distanceLastNode.anchor = Vector2.zero;
            distanceLastNode.connectedBody = playerRb;*/

            //playerThrowHook.ConnectDistanceJoint(rb);
            /* DistanceJoint2D disjoint = GetComponent<DistanceJoint2D>();
             disjoint.enabled = true;
             disjoint.connectedBody = playerRb;*/


            /* NodeScript nodeScript = lastNode.GetComponent<NodeScript>();
             if (nodeScript != null)
             {
                 //Debug.Log("IS NOTT MOTHERFUCKING NULL: " + nodeScript.isUsed());
                 nodeScript.SetNewLineTarget(player.transform);
             }*/



        }
        else if(!alreadyJumped)
        {
            LeftHookBeforeDestination();
        }

            

    }


    private void LeftHookBeforeDestination()
    {
        DesattachRopeFromHook();
        GM.RemoveCombo();
        if(PM.currentJumps > 0)
        {
            PM.Jump(PM.jumpForce);
            PM.currentJumps--;
        }
        //Debug.Log("Left Hook Before Destination");
        alreadyJumped = true;
    }

    public void DesattachRopeFromHook()
    {
        if (isAttachedToPlayer)
            UnhookRope();
        rb.isKinematic = false;
    }

    private void MovingTowardsHook()
    {
        PM.RemoveHook();
        //Debug.Log("Moving Towards Hook");
    }

    public void AddRope(GameObject hook, bool noTarget)
    {
        isAttachedToPlayer = true;
        this.noTarget = noTarget;
        attachedHook = hook;
        this.destiny = hook.transform.position;
    }

    public void AddRope(Vector2 destiny, bool noTarget)
    {
        isAttachedToPlayer = true;
        this.noTarget = noTarget;
        attachedHook = null;
        this.destiny = destiny;
    }

    /*public void DestroyRope(GameObject node)
    {
        if (isAlreadyCut || !isDone || !isAttachedToPlayer)
            return;

        if (isAttachedToPlayer)
        {
            GM.RemoveCombo();
            playerThrowHook.DisableRope();
        }

        bottomNodeList.Clear();
        upperNodeList.Clear();

        index = nodeList.IndexOf(node);
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (i > index)
            {
                bottomNodeList.Add(nodeList[i]);
            }
            else
            {
                upperNodeList.Add(nodeList[i]);

            }
        }

        upperNodeList[upperNodeList.Count - 1].GetComponent<HingeJoint2D>().enabled = false;
        nodeLineRenderer = nodeList[index].AddComponent<LineRenderer>();
        if (nodeLineRenderer != null)
        {
            nodeLineRenderer.startWidth = 0.1f;
            nodeLineRenderer.material.color = Color.green;
        }
        //destroyRope = true;
        drawRope = false;
        isAlreadyCut = true;
        UnhookRope();
    }*/

    private void LateUpdate()
    {
        //RenderLine();

        /* screenPos = cam.WorldToScreenPoint(transform.position);
         if (screenPos.x < -disToDestroyX)
         {
             DisableRope();
         }*/
    }

    /*void RenderLine()
    {

        if (drawRope)
        {
            if (isAttachedToPlayer)
                lr.positionCount = vertexCount;
            else
                lr.positionCount = vertexCount - 1;

            int i;
            for (i = 0; i < nodeList.Count; i++)
            {

                lr.SetPosition(i, nodeList[i].transform.position);

            }
            if (isAttachedToPlayer)
                lr.SetPosition(i, player.transform.position);
        }
        else
        {
            nodeLineRenderer.positionCount = bottomNodeList.Count;
            int i;
            for (i = 0; i < bottomNodeList.Count; i++)
            {

                nodeLineRenderer.SetPosition(i, bottomNodeList[i].transform.position);

            }
            //nodeLineRenderer.SetPosition(i, player.transform.position);

            lr.positionCount = upperNodeList.Count;

            for (int a = 0; a < upperNodeList.Count; a++)
            {
                lr.SetPosition(a, upperNodeList[a].transform.position);
            }

        }

    }*/

    /*void CreateCollider()
    {
        int i;
        List<Vector2> edgePoints = new List<Vector2>();
        for (i = 0; i < nodeList.Count; i++)
        {

            edgePoints.Add(transform.InverseTransformPoint(nodeList[i].transform.position));
        }
        edgePoints.Add(transform.InverseTransformPoint(player.transform.position));

        edgeCol.points = edgePoints.ToArray();
    }*/

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

        /* DistanceJoint2D distanceLastNode = lastNode.GetComponent<DistanceJoint2D>();
         distanceLastNode.enabled = true;
         distanceLastNode.autoConfigureConnectedAnchor = true;
         distanceLastNode.autoConfigureDistance = false;
         distanceLastNode.distance = distance;
         distanceLastNode.anchor = Vector2.zero;
         distanceLastNode.connectedBody = nodeRb;*/

        if (!useLine)
        {
            useLine = true;
            firstNode = node.transform;
            line.positionCount = 2;
        }
        else{
            NodeScript nodeScript = lastNode.GetComponent<NodeScript>();
            if (nodeScript != null)
            {
                nodeScript.SetNewLineTarget(node.transform);
            }
        }
        
        /*LineRenderer lineLastNode = lastNode.GetComponent<LineRenderer>();
        lineLastNode.positionCount = 2;
        lineLastNode.SetPosition(0, lastNode.transform.position);
        lineLastNode.SetPosition(1, node.transform.position);*/



        lastNode = node;

        nodeList.Add(lastNode);

        //vertexCount++;
        Debug.Log("Created Node");
    }

    public void UnhookRope()
    {
        // playerThrowHook.DisconnectDistanceJoint();
        //playerThrowHook.DontLimitDistance();
       /* DistanceJoint2D disjoint = GetComponent<DistanceJoint2D>();
        disjoint.enabled = false;*/
        PM.RemoveHook();
        if (camFollow != null)
            camFollow.RemoveTarget(transform);
        if (lastNode != null)
            lastNode.GetComponent<HingeJoint2D>().enabled = false;
        isAttachedToPlayer = false;
        isDone = true;
        
        /*if(gameObject != null)
        SetLayerRecursively(0);*/

        /*int count = transform.childCount;
        int ax = 0;
        while (ax <= count) { 

            for (int i = 0; i < transform.childCount; i++)
            {
                ax++;
                Transform trans = transform.GetChild(i);
                if (trans != null)
                    trans.gameObject.layer = 0;
            }
        }*/
    }



    public void DisableRope()
    {
        for (int i = 1; i < nodeList.Count; i++)
        {
            nodeList[i].gameObject.SetActive(false);
            nodeList[i].transform.parent = aux.GetSpawnTransform();

        }
        if (nodeLineRenderer != null)
        {
            Destroy(nodeLineRenderer);
        }
        /*nodeList.Clear();
        lr.positionCount = 0;*/
        Destroy(gameObject);
        //nodeLineRenderer.positionCount = 0;
        //gameObject.SetActive(false);

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
        }else if (other.CompareTag("Ground") && isAttachedToPlayer)
        {
            LeftHookBeforeDestination();
            UnhookRope();
        }
    }

    public void SetLayerRecursively(int layerNumber)
    {
        foreach (Transform trans in gameObject.GetComponentsInChildren<Transform>(true))
        {
            trans.gameObject.layer = layerNumber;
        }
    }
}
