﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnHookManager : MonoBehaviour
{
    public static SpawnHookManager instance;

    private AuxManager aux;
    private GameManager GM;

    public ObjectPooler hookPool;
    private List<GameObject> hooksList;

    public ObjectPooler missiles;
    public Transform spawnMissileTop;
    public Transform spawnMissileDown;

    public bool shootMissiles = true;
    public float timeBetweenMissiles = 2f;

    public Transform spawnPoint;
    public Vector2 xOffset;
    public Vector2 yOffset;
    public ObjectPooler coinObject;
    private GameObject player;
    public float radiusToGrab = 10f;

    public GameObject hookIndicator;
    private GameObject currentFarthestHook;

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
        hooksList = new List<GameObject>();
        player = aux.GetPlayer();
    }

    public void StartGame()
    {
        CreateFirstHook();

        if (shootMissiles)
        {
            StartCoroutine(EnemySidesGenerator());
        }
    }

    void Update()
    {

        if (transform.position.x < spawnPoint.position.x && GM.isPlaying())
        {
            CreateHook();
        }

        currentFarthestHook = GetFarthestHook();

        if (currentFarthestHook != null)
        {
            hookIndicator.SetActive(true);
            hookIndicator.transform.position = currentFarthestHook.transform.position;
            /*SpriteRenderer sRenderer = currentFarthestHook.GetComponent<SpriteRenderer>();
            sRenderer.sprite = aux.GetAvailableHookSprite();*/
        }
        else
        {
            /*SpriteRenderer sRenderer = currentFarthestHook.GetComponent<SpriteRenderer>();
            sRenderer.sprite = aux.GetUnavailableHookSprite();*/
            hookIndicator.SetActive(false);
        }

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
    public void RemoveHookList(GameObject hook)
    {
        hooksList.Remove(hook);
    }

    void CreateHook()
    {
        GameObject hook = hookPool.GetPooledObject();
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
        hooksList.Add(hook);
        transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

    }

    void CreateFirstHook()
    {
        GameObject hook = hookPool.GetPooledObject();
        hook.SetActive(true);
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y + GetRandomHookY(), transform.position.z);
        hook.transform.position = newPos;
        hooksList.Add(hook);
        //transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);

    }

    IEnumerator EnemySidesGenerator()
    {
        float delay = timeBetweenMissiles;
        yield return new WaitForSeconds(delay);
        SpawnMissile();
        StartCoroutine(EnemySidesGenerator());
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

    void CreateCoin()
    {
        GameObject coin = coinObject.GetPooledObject();
        coin.SetActive(true);
        coin.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
    }

    public GameObject GetCurrentFarthestHook()
    {
        return currentFarthestHook;
    }

    public GameObject GetFarthestHook()
    {
        //List<GameObject> newList = new List<GameObject>();
        GameObject farthest = null;
        float distance = 0;
        Vector3 currentPosition = player.transform.position;
        foreach (GameObject hook in hooksList)
        {
            Vector3 diff = hook.transform.position - currentPosition;
            float curDistance = diff.sqrMagnitude;
            float dirNum = AngleDir(player.transform.forward, diff, transform.up);
            if (curDistance > distance && curDistance < radiusToGrab && dirNum == 1)
            {
                farthest = hook;
                distance = curDistance;
            }
        }
        //hooksList.Remove(farthest.gameObject);

        // Debug.DrawLine(player.transform.position, farthest.transform.position, Color.red);
        if (farthest != null)
        {
            return farthest;
        }
        return null;

    }

    public Transform GetClosestHook()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = player.transform.position;
        foreach (GameObject potentialTarget in hooksList)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget.transform;
            }
        }

        return bestTarget;
    }

    float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);

        if (dir > 0f)
        {
            return 1f;
        }
        else if (dir < 0f)
        {
            return -1f;
        }
        else
        {
            return 0f;
        }
    }

}