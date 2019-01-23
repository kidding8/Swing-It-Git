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
        if (other.CompareTag("Wall"))
        {
            onDeath();
        }else if (other.CompareTag("Player") && isHook)
        {
            onDeath();

        }
    }

    private void onDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
        if(isHook)
        {
            PM.RemoveGrabbableObject(gameObject);
        }
    }
}
