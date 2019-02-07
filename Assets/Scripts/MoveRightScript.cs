using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightScript: MonoBehaviour {

    // Use this for initialization
    public float speed = 3f;
    private EffectsManager EM;
    public bool isBallDestroyer;
    public float timeAlive = 3;
    public float speedMultiplier = 0.05f;
    private float timer;
    private float currentSpeed ;
	void Start () {
        EM = EffectsManager.instance;
        currentSpeed = speed;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (isBallDestroyer)
        {
            currentSpeed *= 1+speedMultiplier;
        }
        transform.Translate(-transform.right * currentSpeed * Time.smoothDeltaTime);
        if(timer > timeAlive)
        {
            OnDeath();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if (other.CompareTag("Wall")){
            onDeath();
        }*/
    }

    private void OnDeath()
    {
        timer = 0;
        currentSpeed = speed;
        EM.CreateEnemyEffects(transform.position);
        gameObject.SetActive(false);
    }
}
