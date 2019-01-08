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
    private bool reachedTarget = false;
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
    private ThrowHook playerThrowHook;
    private CameraFollow camFollow;
    private Rigidbody2D rb;
    private bool useLine = false;
    private Transform firstNode = null;
    private Grabber targetGrabber = null;
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

        PM.currentJumps = PM.maxRopeJumps;
        /*if (targetGrabber != null)
            targetGrabber.CheckIfTeleporter();*/



    }


    private void LeftHookBeforeDestination()
    {
        if (targetGrabber != null)
        {
            targetGrabber.RemoveRope(this);
        }

        DesattachRopeFromHook();

        GM.RemoveCombo();
        if (PM.currentJumps > 0)
        {
            PM.BigJump();
            PM.currentJumps--;
        }

        alreadyJumped = true;
    }

    public void DesattachRopeFromHook()
    {
        if (isAttachedToPlayer)
            UnhookRope();
        rb.isKinematic = false;
    }

    public void AddRope(GameObject target)
    {
        isAttachedToPlayer = true;
        attachedHook = target;
        targetGrabber = attachedHook.GetComponent<Grabber>();
        if (targetGrabber != null)
            targetGrabber.AddRope(this);
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
        isAttachedToPlayer = false;
        isDone = true;
    }



    public void DisableRope()
    {
        /*for (int i = 1; i < nodeList.Count; i++)
        {
            nodeList[i].gameObject.SetActive(false);
            nodeList[i].transform.parent = aux.GetSpawnTransform();

        }*/
        rb.isKinematic = true;
        if (lastNode != null)
            lastNode.GetComponent<HingeJoint2D>().connectedBody = null;
        particlesDone = false;
        isAttachedToPlayer = false;
        isDone = false;
        useLine = false;
        gameObject.SetActive(false);


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
