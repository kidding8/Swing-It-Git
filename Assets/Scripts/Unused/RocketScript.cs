using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    private AuxManager aux;
    private EffectsManager EM;
    private PlayerManager PM;
    private GameObject player;
    public float speed = 10f;
    public bool useSpeed = true;
    private bool activateRocket = false;
    private Vector3 initialPos;
    public float maxDistance = 30f;
    public float rocketSpeed = 10f;
    private ThrowHook throwHook;
    public float destroyRadius = 10f;
    // Start is called before the first frame update
    void Start()
    {
        PM = PlayerManager.instance;
        EM = EffectsManager.instance;
        aux = AuxManager.instance;
        player = aux.GetPlayer();
        throwHook = player.GetComponent<ThrowHook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activateRocket)
        {
            transform.Translate(Vector3.right * Time.deltaTime * rocketSpeed);
            player.transform.position = transform.position + Vector3.up;
            throwHook.UnhookHook();
            //PM.SetPlayerState(States.STATE_ROCKET);
            if (Vector3.Distance(initialPos, transform.position) > maxDistance)
            {
                ReachedDistination();
            }
        }
        else if(useSpeed)
        {
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PM.CanCollectObjects())
        {
            initialPos = transform.position;
            activateRocket = true;
        }
        else if(other.CompareTag("Wall"))
        {
            OnDeath();
        }
    }

    /*private void OnEnable()
    {
        //initialPos = transform.position;
        activateRocket = false;
    }*/

    private void ReachedDistination()
    {
        activateRocket = false;
        PM.Jump(PM.jumpForce);
        OnDeath();
        PM.SetPlayerState(States.STATE_NORMAL);
        aux.DestroyInRadius(transform.position, destroyRadius);
    }

    private void OnDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }
}
