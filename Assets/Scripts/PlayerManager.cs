using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    private Rigidbody2D rb;
    private bool isHooked;
    private GameObject currentHook;

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
        rb = GetComponent<Rigidbody2D>();
    }
    public void SetNewHook(GameObject hook)
    {
        isHooked = true;
        currentHook = hook;
    }

    public bool IsHooked()
    {
        return isHooked;
    }

    public void RemoveHook()
    {
        isHooked = false;
        currentHook = null;
    }

    public void AddImpulsiveForce(Vector3 dir, float amount)
    {
        rb.AddForce(dir * amount, ForceMode2D.Impulse);
    }

}

