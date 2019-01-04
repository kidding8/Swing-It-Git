using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFloorManager : MonoBehaviour {

    [System.Serializable]
    public struct FloorEnemies
    {
        public ObjectPooler ObstaclePool;
        public int probability;
    }

    public static SpawnFloorManager instance;
    private GameManager GM;
    private AuxManager aux;
    private PlayerManager PM;

    public FloorEnemies[] floorEnemiesList;

    public ObjectPooler[] floorObstacles;
    
    public Transform spawnPoint;
    public Vector2 xOffset;
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
    // Use this for initialization
    void Start () {
        PM = PlayerManager.instance;
        GM = GameManager.instance;
        aux = AuxManager.instance;
    }

    private float GetRandomHookX()
    {
        return Random.Range(xOffset.x, xOffset.y);
    }

    // Update is called once per frame
    void Update () {
        if (transform.position.x < spawnPoint.position.x && GM.isPlaying() && PM.canSpawnEnemies)
        {
            CreateObstacle();
        }
    }

    private int GetObstacle()
    {
       
        int itemWeight = 0;

        for (int i = 0; i < floorEnemiesList.Length; i++)
        {
            itemWeight += floorEnemiesList[i].probability;
        }

        int randomValue = Random.Range(0, itemWeight);

        for (int j = 0; j < floorEnemiesList.Length; j++)
        {
            if(randomValue <= floorEnemiesList[j].probability)
            {
                return j;
            }
            randomValue -= floorEnemiesList[j].probability;
        }
        return 0;
    }

    void CreateObstacle()
    {
        int obsIndex = GetObstacle();
        GameObject obstacle = floorEnemiesList[obsIndex].ObstaclePool.GetPooledObject();
        obstacle.SetActive(true);
        Vector3 newPos = new Vector3(transform.position.x + GetRandomHookX(), transform.position.y, transform.position.z);
        /*int safetyNet = 0;
        while (aux.IsOtherObjectsAround(newPos))
        {
            safetyNet++;
            // Debug.Log("entrou :" + safetyNet);
            newPos = new Vector3(transform.position.x + GetRandomHookX(), transform.position.y, transform.position.z);

            if (safetyNet > 10)
            {
                Debug.Log("EXDECEUUUU");
                break;
            }
        }*/
        obstacle.transform.position = newPos;
        //hooksList.Add(hook);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

    }
}
