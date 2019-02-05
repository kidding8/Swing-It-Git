using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnHookManager : MonoBehaviour
{
    [System.Serializable]
    public struct GrabberObstacle
    {
        public ObjectPooler grabberPool;
        public int probability;
    }

    public static SpawnHookManager instance;

    private AuxManager aux;
    private GameManager GM;
    private PlayerManager PM;

    public ObjectPooler[] hookPool;
    public GrabberObstacle[] grabberPool;
    //private List<GameObject> hooksList;

    public ObjectPooler missiles;
    public Transform spawnMissileTop;
    public Transform spawnMissileDown;

    public ObjectPooler guidedMissiles;
    public float timeBetweenGuidedMissiles = 8f;

    public float timeBetweenMissiles = 2f;

    public Transform spawnPoint;
    public Vector2 xOffset;
    public Vector2 yOffset;
    public ObjectPooler coinObject;
    private GameObject player;
    

    //public GameObject hookIndicator;
    //private GameObject currentFarthestHook;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        GM = GameManager.instance;
        aux = AuxManager.instance;
        PM = PlayerManager.instance;
        //hooksList = new List<GameObject>();
        player = aux.GetPlayer();
    }

    public void StartGame()
    {
        CreateFirstHook();

        if (PM.canShootMissiles)
        {
            StartCoroutine(EnemySidesGenerator());
        }
        if (PM.canShootGuidedMissiles)
        {
            StartCoroutine(GuidedMissiles());
        }
    }

    void Update()
    {

        if (transform.position.x < spawnPoint.position.x && GM.isPlaying())
        {
            CreateHook();
        }

        
        //currentFarthestHook = hookTemp;
    }

    private float GetRandomHookX()
    {
        return Random.Range(xOffset.x, xOffset.y);
    }

    private float GetRandomHookY()
    {
        return Random.Range(yOffset.x, yOffset.y);
    }

    private float GetRandomMissileY()
    {
        return Random.Range(spawnMissileTop.position.y, spawnMissileDown.position.y);
    }

    private float GetRandomMissileX()
    {
        return spawnMissileTop.position.x;
    }

    private int GetRandomHookPrefab()
    {
        return Random.Range(0, hookPool.Length);
    }

    private int GetGrabberObstacle()
    {

        int itemWeight = 0;

        for (int i = 0; i < grabberPool.Length; i++)
        {
            itemWeight += grabberPool[i].probability;
        }

        int randomValue = Random.Range(0, itemWeight);

        for (int j = 0; j < grabberPool.Length; j++)
        {
            if (randomValue <= grabberPool[j].probability)
            {
                return j;
            }
            randomValue -= grabberPool[j].probability;
        }
        return 0;
    }

    /*public void RemoveHookList(GameObject hook)
    {
        hooksList.Remove(hook);
    }*/

    void CreateHook()
    {
        int rand = GetGrabberObstacle();
        GameObject hook = grabberPool[rand].grabberPool.GetPooledObject();
        /*if(rand == 1)
        {
            hook.GetComponent<SpringLineScript>().SetNewLine();
        }*/
        hook.SetActive(true);
        Vector3 newPos = new Vector3(transform.position.x + GetRandomHookX(), transform.position.y + GetRandomHookY(), transform.position.z);
        int safetyNet = 0;
        while (aux.IsOtherObjectsAround(newPos))
        {
            safetyNet++;
            //Debug.Log("entrou :" + safetyNet);
            newPos = new Vector3(transform.position.x + GetRandomHookX(), transform.position.y + GetRandomHookY(), transform.position.z);

            if (safetyNet > 10)
            {
                Debug.Log("EXDECEUUUU");

                break;
            }
        }
        hook.transform.position = newPos;
        PM.AddGrabbableObject(hook);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

    }

    void CreateFirstHook()
    {
        GameObject hook = hookPool[0].GetPooledObject();
        hook.SetActive(true);
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + GetRandomHookY(), transform.position.z);
        hook.transform.position = newPos;
        PM.AddGrabbableObject(hook);
        //transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

    }

    IEnumerator EnemySidesGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMissiles);
            SpawnMissile();
        }
        
    }
    IEnumerator GuidedMissiles()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenMissiles);
            SpawnGuidedMissile();
        }
    }

    private void SpawnMissile()
    {
        GameObject newMissile = missiles.GetPooledObject();
        newMissile.SetActive(true);
        //newMissile.transform.rotation = Quaternion.Euler(0, 0, -90);
        Vector3 newPos;
        newPos = new Vector3(GetRandomMissileX(), GetRandomMissileY(), transform.position.z);
        newMissile.transform.rotation = Quaternion.identity;
        newMissile.transform.position = newPos;
    }

    private Vector3 RandomGuidedMissilePos()
    {
        int rand = Random.Range(0, 2);
        bool top = rand == 0 ? true : false;
        Vector3 topLeft = aux.GetUpperLeftCorner();
        Vector3 topRight = aux.GetUpperRightCorner();
        Vector3 downRight = aux.GetLowerRightCorner();
        /*if (top)
        {*/
        return new Vector3(Random.Range(topLeft.x + 10, topRight.x), topRight.y + 7);
        /*}
        else
        {
          return new Vector3(topRight.x + 3, Random.Range(topRight.y, downRight.y + 10));
        }*/

    }

    private void SpawnGuidedMissile()
    {
        GameObject newMissile = guidedMissiles.GetPooledObject();
        newMissile.SetActive(true);
        //newMissile.transform.rotation = Quaternion.Euler(0, 0, -90);

        Vector3 newPos = RandomGuidedMissilePos();
        newMissile.transform.rotation = Quaternion.identity;
        newMissile.transform.position = newPos;
    }


    void CreateCoin()
    {
        GameObject coin = coinObject.GetPooledObject();
        coin.SetActive(true);
        coin.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }

   /* public GameObject GetCurrentFarthestHook()
    {
        return currentFarthestHook;
    }*/

}
