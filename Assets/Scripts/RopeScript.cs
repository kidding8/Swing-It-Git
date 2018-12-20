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
    public float speed = 1f;
    public float distance = 2f;
    public float disToDestroyX = 300f;
    private ObjectPooler nodePool;
    private GameObject player;
    private GameObject lastNode;
    

    private List<GameObject> nodeList;

    private List<GameObject> bottomNodeList;
    private List<GameObject> upperNodeList;

    private bool isDone = false;
    private LineRenderer lr;
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
        lr = GetComponent<LineRenderer>();
        camFollow = cam.GetComponent<CameraFollow>();
        rb = GetComponent<Rigidbody2D>();
        //edgeCol = GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttachedToPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, speed);
            if ((Vector2)transform.position == destiny)
            {
                GrabbedHook();
            }
        }
        else if ((Vector2)transform.position != destiny && !alreadyJumped)
        {
            LeftHookBeforeDestination();
        }


        if ((Vector2)transform.position != destiny && isAttachedToPlayer)
        {
            if (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
            {
                CreateNode();
            }

            MovingTowardsHook();
        }
        //CreateCollider();

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

            camFollow.AddTarget(transform);
            isDone = true;

            HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
            hingeLastNode.enabled = true;

            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();

            PM.SetNewHook(gameObject);
            attachedHook.GetComponent<GrabberScript>().AddRope(this);

           while (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
           {
               CreateNode();
           }
        }
        else
        {
            LeftHookBeforeDestination();
        }

            

    }

    private void LeftHookBeforeDestination()
    {
        rb.isKinematic = false;
        GM.RemoveCombo();
        playerThrowHook.Jump();
        alreadyJumped = true;
    }

    public void DesattachRopeFromHook()
    {
        rb.isKinematic = false;
    }

    private void MovingTowardsHook()
    {
        PM.RemoveHook();
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

    public void DestroyRope(GameObject node)
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
    }

    private void LateUpdate()
    {
        RenderLine();

        /* screenPos = cam.WorldToScreenPoint(transform.position);
         if (screenPos.x < -disToDestroyX)
         {
             DisableRope();
         }*/
    }

    void RenderLine()
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

    }

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
        pos2Create *= distance;
        pos2Create += (Vector2)lastNode.transform.position;

        GameObject node = nodePool.GetPooledObject();
        node.transform.position = pos2Create;
        node.transform.rotation = Quaternion.identity;
        node.transform.SetParent(transform);
        node.SetActive(true);

        HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
        hingeLastNode.enabled = true;
        hingeLastNode.connectedBody = node.GetComponent<Rigidbody2D>();

        lastNode = node;

        nodeList.Add(lastNode);

        vertexCount++;

    }

    public void UnhookRope()
    {
        if (camFollow != null)
            camFollow.RemoveTarget();
        if (lastNode != null)
            lastNode.GetComponent<HingeJoint2D>().enabled = false;
        isAttachedToPlayer = false;
        isDone = true;
        PM.RemoveHook();
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
