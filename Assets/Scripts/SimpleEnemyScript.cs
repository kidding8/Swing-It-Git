using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyScript : MonoBehaviour
{
    private EffectsManager EM;
    private PlayerManager PM;
    public bool isHook = false;
    public bool rotate = false;
    public float rotateAmount = 2f;
    private void Start()
    {
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
    }

    private void Update()
    {
        if (rotate)
        {
            transform.Rotate(Vector3.forward * rotateAmount);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Destroyer"))
        {
            onDeath();
        }else if (other.CompareTag("Player") && !PM.CanDie())
        {
            onDeath();

        }
    }

    private void onDeath()
    {
        if (isHook)
        {
            PM.RemoveGrabbableObject(gameObject);
        }
        EM.CreateEnemyEffects(transform.position);
        gameObject.SetActive(false);
       
    }
}
