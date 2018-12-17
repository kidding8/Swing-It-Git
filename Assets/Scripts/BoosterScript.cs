using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterScript : MonoBehaviour {

    public float Boost = 10f;
    private EffectsManager EM;
    private bool isTriggerd = false;
    private void Start()
    {
        EM = EffectsManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isTriggerd)
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            rb.velocity = new Vector2(velocity.x, Boost);
            onDeath();
        }
        else if (other.CompareTag("Wall"))
        {
            onDeath();
        }
    }

    private void onDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }
}
