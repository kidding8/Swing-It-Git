using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterScript : MonoBehaviour {

    public float boost = 10f;
    private AuxManager aux;
    public float radiusToDestroy = 300f;
    private EffectsManager EM;
    public bool useRotation = false;

    private void Start()
    {
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
    }

   private void Update()
   {
        if (useRotation)
        {
            transform.localEulerAngles = new Vector3(0, 0, -Mathf.PingPong(Time.time * 20, 30));
            transform.Translate(Vector3.up * 2 * Time.deltaTime);
        }
   }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            if(velocity.x <= 4)
            {
                velocity.x = 5;
            }
            velocity.y = boost;
            rb.velocity = velocity;
            aux.DestroyInRadius(transform.position, radiusToDestroy);
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
        EM.GenerateText("Destruction 25", transform.position);
        gameObject.SetActive(false);
    }
}
