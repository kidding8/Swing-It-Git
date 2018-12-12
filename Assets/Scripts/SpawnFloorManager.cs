using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFloorManager : MonoBehaviour {
    public static SpawnFloorManager instance;
    public ObjectPooler[] floorObstacles;
    private GameManager GM;
    private AuxManager aux;
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
        GM = GameManager.instance;
        aux = AuxManager.instance;
    }

    private float GetRandomHookX()
    {
        return Random.Range(xOffset.x, xOffset.y);
    }

    // Update is called once per frame
    void Update () {
        if (transform.position.x < spawnPoint.position.x && GM.isPlaying())
        {
            CreateObstacle();
        }
    }

    private int GetObstacle()
    {
        return Random.Range(0, floorObstacles.Length);
    }

    void CreateObstacle()
    {
        int obsIndex = GetObstacle();
        GameObject obstacle = floorObstacles[obsIndex].GetPooledObject();
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
