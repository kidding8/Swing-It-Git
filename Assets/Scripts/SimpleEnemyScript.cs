using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyScript : MonoBehaviour
{
    private EffectsManager EM;
    public bool isHook = false;
    private void Start()
    {
        EM = EffectsManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            onDeath();
            if (isHook)
            {
                SpawnHookManager.instance.RemoveHookList(this.gameObject);
            }
        }
    }

    private void onDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }
}
