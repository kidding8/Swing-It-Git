﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBottomManager : MonoBehaviour {

    private SpawnBottomManager instance;
    private AuxManager aux;
    private GameManager GM;
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
        aux = AuxManager.instance;
        
    }


    void Update()
    {
        if (transform.position.x < spawnPoint.position.x && GM.isPlaying())
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

    void CreateObstacle()
    {
        int obsIndex = GetObstacle();
        GameObject obstacle = obstaclesList[obsIndex].GetPooledObject();
        obstacle.SetActive(true);
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
