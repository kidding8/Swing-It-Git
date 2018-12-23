using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBottomManager : MonoBehaviour {

    /*[System.Serializable]
    public struct Obstacle
    {
        public int index;
        public ObjectPooler ObstaclePool;
    }*/

    private SpawnBottomManager instance;
    private AuxManager aux;
    private GameManager GM;
    private PlayerManager PM;
   
    //public ObjectPooler grassPool;
    public List<ObjectPooler> obstaclesList;

    public Transform spawnPoint;
    public Vector2 xOffset;
    public Vector2 yOffset;

    

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
        PM = PlayerManager.instance;
        aux = AuxManager.instance;
        
    }


    void Update()
    {
        if (transform.position.x < spawnPoint.position.x && GM.isPlaying() && PM.canSpawnEnemies)
        {
            CreateObstacle();
        }
        //GetClosestHook();
        //Debug.Log("Number of List" + listPlatforms.Count);
    }


    /* int RandomPlatform()
    {
        int plat = Random.Range(0, hooksList.Length);
        return plat;
    }*/

    private float GetRandomHookX()
    {
        return Random.Range(xOffset.x, xOffset.y);
    }

    private float GetRandomHookY()
    {
        return Random.Range(yOffset.x, yOffset.y);
    }

    private int GetObstacle()
    {
        return Random.Range(0, obstaclesList.Count);
    }

    public void RemovePlatformList(GameObject platform)
    {
        //hooksList.Remove(platform);
    }

    private void InCaseExtra(int index, GameObject obj)
    {
        /*switch (index) {
            case 0:
                Debug.Log("Created");
                obj.GetComponent<CoinFormation>().CreateCoins();
                break;
            case 1:
                Debug.Log("Created 2");
                obj.GetComponent<CoinFormation>().CreateCoins();
                break;
        }*/

    }

    void CreateObstacle()
    {
        int obsIndex = GetObstacle();
        GameObject obstacle = obstaclesList[obsIndex].GetPooledObject();
        obstacle.SetActive(true);
        //InCaseExtra(obstaclesList[obsIndex].index, obstacle);
        Vector3 newPos = new Vector3(transform.position.x + GetRandomHookX(), transform.position.y + GetRandomHookY(), transform.position.z);
        int safetyNet = 0;
        while (aux.IsOtherObjectsAround(newPos))
        {
            safetyNet++;
           // Debug.Log("entrou :" + safetyNet);
            newPos = new Vector3(transform.position.x + GetRandomHookX(), transform.position.y + GetRandomHookY(), transform.position.z);

            if(safetyNet > 10)
            {
                Debug.Log("EXDECEUUUU");
                break;
            }
        }
            obstacle.transform.position = newPos;
        //hooksList.Add(hook);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

    }

}
