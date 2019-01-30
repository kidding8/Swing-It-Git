using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class States
{
    public const int STATE_HIDDEN = 0;
    public const int STATE_NORMAL = 1;
    public const int STATE_HOOKED = 2;
    public const int STATE_DASHING = 3;
    public const int STATE_SPRING = 4;
    public const int STATE_CLOSE_TO_GROUND = 5;
    public const int STATE_GROUNDED = 6;
    public const int STATE_GRAPPLE = 7;
    public const int STATE_TELEPORT = 8;
    //public const int STATE_MAGNET = 10;
   //public const int STATE_ON_FIRE = 11;
}

public class Power
{
    public const int POWER_NONE = 0;
    public const int POWER_JUMP = 1;
    public const int POWER_DASH = 2;
    public const int POWER_TELEPORT = 3;
    public const int POWER_SPRING = 4;
    public const int POWER_EXPLOSION = 5;
    public const int POWER_SHOOTING = 6;
}

public class Hooks
{
    //Jump, Dash, Teleport, Tornado, Shooting, Barrel, Decoy
    public const int HOOK_ROPE = 0;
    public const int HOOK_SPRING = 1;
}

public class AuxManager : MonoBehaviour {

    public static AuxManager instance;
    private PlayerManager PM;
    
    public Canvas canvas;
    public Camera mainCamera;
    public GameObject player;

    public Transform SpawnnedObjectsTransform;
    public ObjectPooler nodePool;
    public ObjectPooler indicatorPool;
    public ObjectPooler singleCoinPool;
    public ObjectPooler[] coinPool;
    public ObjectPooler boxPool;
    public ObjectPooler groundBulletPool;
    public ObjectPooler emptyRbPool;
    public Sprite[] backgroundSprites;
    public Sprite[] grabberSprites;
    public Material shadowMaterial;
    public Color shadowColor;
    public Color foreGroundColor;
    public Color enemyColor;
    public Sprite availableHook;
    public Sprite unavailableHook;
    public GameObject grabObjectIndicator;
    public GameObject wall;
    public LayerMask layerMaskOtherObjectsAround;
    public Canvas inGameCanvas;
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

    private void Start()
    {
        PM = PlayerManager.instance;
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

    public GameObject GetGrabObjectIndicator()
    {
        return grabObjectIndicator;
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

    public ObjectPooler GetGroundBulletPool()
    {
        return groundBulletPool;
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

    public Sprite GetGrabberSprite()
    {
        return grabberSprites[Random.Range(0, grabberSprites.Length)];
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

    public void DestroyInRadius(Vector3 pos, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, radius);
        
        foreach (Collider2D col in colliders)
        {
            if (col.tag == "Enemy")
            {
                col.gameObject.SetActive(false);
            }
            else if(col.tag == "Hook")
            {
                PM.RemoveGrabbableObject(col.gameObject);
                col.gameObject.SetActive(false);
            }
        }
    }

    public bool IsDistanceLower(Vector3 a, Vector3 b, float dist)
    {
        return (a - b).sqrMagnitude <= dist * dist;
    }

    public bool IsDistanceHigher(Vector3 a, Vector3 b, float dist)
    {
        return (a - b).sqrMagnitude >= dist * dist;
    }

    public static float KineticEnergy(Rigidbody2D rb)
    {
        // mass in kg, velocity in meters per second, result is joules
        return 0.5f * rb.mass * Mathf.Pow(rb.velocity.magnitude, 2);
    }

    public Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }
}
