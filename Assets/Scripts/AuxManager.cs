using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuxManager : MonoBehaviour {

    public static AuxManager instance;

    
    public Canvas canvas;
    public Camera mainCamera;
    public GameObject player;

    public Transform SpawnnedObjectsTransform;
    public ObjectPooler nodePool;
    public ObjectPooler indicatorPool;
    public ObjectPooler singleCoinPool;
    public ObjectPooler[] coinPool;
    public ObjectPooler boxPool;
    public Sprite[] backgroundSprites;
    public Material shadowMaterial;
    public Color shadowColor;
    public Sprite availableHook;
    public Sprite unavailableHook;

    public LayerMask layerMaskOtherObjectsAround;
    public float radiusOtherObjectsAround = 5f;

    private bool canSpawn = false;

    
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

    /*private void Update()
    {
        if (GameManager.instance.isPlaying())
            canSpawn = true;
    }

    public bool CanSpawn()
    {
        return canSpawn;
    }*/

    public Transform GetSpawnTransform()
    {
        return SpawnnedObjectsTransform;
    }

    public Material GetShadowMaterial()
    {
        return shadowMaterial;
    }

    public Color GetShadowColor()
    {
        return shadowColor;
    }

    public Camera GetCamera()
    {
        return mainCamera;
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public Canvas GetCanvas()
    {
        return canvas;
    }

    public ObjectPooler GetNodePool()
    {
        return nodePool;
    }

    public ObjectPooler GetBoxPool()
    {
        return boxPool;
    }

    public ObjectPooler GetIndicatorPool()
    {
        return indicatorPool;
    }

    public ObjectPooler GetCoinPool()
    {
        return coinPool[Random.Range(0,coinPool.Length)];
    }

    public ObjectPooler GetSingleCoinPool()
    {
        return singleCoinPool;
    }
    public Sprite GetAvailableHookSprite()
    {
        return availableHook;
    }

    public Sprite GetUnavailableHookSprite()
    {
        return unavailableHook;
    }

    public Sprite GetBackgroundSprite()
    {
        return backgroundSprites[Random.Range(0, backgroundSprites.Length)];
    }

    public Vector3 GetLowerRightCorner()
    {
        Vector3 lowerRightScreen = new Vector3(Screen.width, 0, 1);
        Vector3 lowerRight = mainCamera.ScreenToWorldPoint(lowerRightScreen);
        return lowerRight;
    }

    public Vector3 GetUpperRightCorner()
    {
        Vector3 upperRightScreen = new Vector3(Screen.width, Screen.height, 1);
        Vector3 upperRight = mainCamera.ScreenToWorldPoint(upperRightScreen);
        return upperRight;
    }

    public Vector3 GetUpperLeftCorner()
    {
        Vector3 upperLeftScreen = new Vector3(0, Screen.height, 1);
        Vector3 upperLeft = mainCamera.ScreenToWorldPoint(upperLeftScreen);
        return upperLeft;
    }

    public Vector3 GetLowerLeftCorner()
    {
        Vector3 lowerLeftScreen = new Vector3(0, 0, 1);
        Vector3 lowerLeft = mainCamera.ScreenToWorldPoint(lowerLeftScreen);
        return lowerLeft;
    }

    public bool IsOtherObjectsAround(Vector3 pos)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, radiusOtherObjectsAround, layerMaskOtherObjectsAround);
        if(colliders.Length > 0)
        {
            return true;
        }
        return false;
    }
}
