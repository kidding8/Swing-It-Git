using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberScript : MonoBehaviour
{
    private EffectsManager EM;
    private ThrowHook throwHook;
    // Start is called before the first frame update
    void Start()
    {
        throwHook = AuxManager.instance.GetPlayer().GetComponent<ThrowHook>();
        EM = EffectsManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            OnDeath();
        }
        else if (other.CompareTag("Player"))
        {
            OnDeath();

        }
    }

    private void OnDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }

    private void OnExplosive()
    {

    }
}
