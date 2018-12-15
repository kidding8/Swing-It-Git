using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{

    private GameManager GM;
    private AuxManager aux;
    private EffectsManager EM;
    public Vector2 destiny;
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
    private int count = 0;
    private bool isAlreadyCut = false;
    private bool drawRope = true;
    private bool isAttachedToPlayer = true;
    private LineRenderer nodeLineRenderer;
    private Vector3 screenPos;
    private Camera cam;
    private int index;
    private bool destroyRope = false;
    private bool alreadyJumped = false;
    private bool particlesDone = false;
    private ThrowHook playerThrowHook;
    // Use this for initialization
    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
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
        //edgeCol = GetComponent<EdgeCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttachedToPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, destiny, speed);
            if((Vector2)transform.position == destiny)
            {
                if (!particlesDone)
                {
                    particlesDone = true;
                    EM.CreateHookGrabParticle(destiny);
                    GM.AddCombo();
                }

                playerThrowHook.isAttachedToHook = true;



            }
            else
            {
                playerThrowHook.isAttachedToHook = false;
            }
        } else if ((Vector2)transform.position != destiny && !alreadyJumped)
        {
            GetComponent<Rigidbody2D>().isKinematic = false;
            GM.RemoveCombo();
            playerThrowHook.Jump();
            alreadyJumped = true;
        }
            
       
        if ((Vector2)transform.position != destiny && isAttachedToPlayer)
        {
            if (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
            {
                CreateNode();
            }
        }
        else if (isDone == false)
        {
            isDone = true;

           /* while (Vector2.Distance(player.transform.position, lastNode.transform.position) > distance)
            {
                CreateNode();
            }*/

            count = 0;
            if (!destroyRope) {
                HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
                hingeLastNode.enabled = true;
            
                lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
            }
            //CreateCollider();

        }
        else
        {
            
            if (destroyRope)
            {
                if (isAttachedToPlayer)
                {
                    playerThrowHook.DisableRope();
                }
                UnhookRope();

            }
        }
        //CreateCollider();

    }

    public void DestroyRope(GameObject node)
    {
        if (isAlreadyCut || !isDone)
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
        destroyRope = true;
        drawRope = false;
        isAlreadyCut = true;
        
    }

    private void LateUpdate()
    {
        RenderLine();

        screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos.x < -disToDestroyX)
        {
            DisableRope();
        }
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
        //node.GetComponent<HingeJoint2D>().enabled = true;
        HingeJoint2D hingeLastNode = lastNode.GetComponent<HingeJoint2D>();
        hingeLastNode.enabled = true;
        hingeLastNode.connectedBody = node.GetComponent<Rigidbody2D>();

        lastNode = node;

        nodeList.Add(lastNode);

        vertexCount++;

    }
    public void UnhookRope()
    {
        destroyRope = true; 
        lastNode.GetComponent<HingeJoint2D>().enabled = false;
        isAttachedToPlayer = false;
        isDone = true;
    }

    public void DisableRope()
    {
        // Debug.Log("Disabled");
        //lastNode.GetComponent<HingeJoint2D>().enabled = true;
        //lastNode.GetComponent<SpriteRenderer>().color = Color.green;
        //destiny = Vector2.zero;
        //
        for (int i = 1; i < nodeList.Count; i++)
        {
            //nodeList[i].GetComponent<SpriteRenderer>().color = Color.green;
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
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.SetActive(false);
        }
    }

}
