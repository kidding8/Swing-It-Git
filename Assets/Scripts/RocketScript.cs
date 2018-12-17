using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    private AuxManager aux;
    private EffectsManager EM;
    private GameObject player;
    private bool activateRocket = false;
    private Vector3 initialPos;
    public float maxDistance = 30f;
    public float rocketSpeed = 10f;
    private ThrowHook throwHook;
    public float destroyRadius = 10f;
    // Start is called before the first frame update
    void Start()
    {
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
            throwHook.isInvicible = true;
            throwHook.DisableRope();
            if(Vector3.Distance(initialPos, transform.position) > maxDistance)
            {
                ReachedDistination();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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
        throwHook.isInvicible = false;
        throwHook.Jump();
        OnDeath();
        aux.DestroyInRadius(transform.position, destroyRadius);
    }

    private void OnDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }
}
