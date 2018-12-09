using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterScript : MonoBehaviour {

    public float Boost = 10f;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            rb.velocity = new Vector2(velocity.x, Boost);
        }
    }
}
