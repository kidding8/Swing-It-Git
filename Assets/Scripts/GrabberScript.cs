using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberScript : MonoBehaviour
{
    private PlayerManager PM;
    private EffectsManager EM;
    private ThrowHook throwHook;
    private SpawnHookManager SHM;
    private List<RopeScript> attachedRopes;
    // Start is called before the first frame update
    void Start()
    {
        throwHook = AuxManager.instance.GetPlayer().GetComponent<ThrowHook>();
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        SHM = SpawnHookManager.instance;
        attachedRopes = new List<RopeScript>();
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
            OnExplosive();
        }
    }

    private void OnDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
        SHM.RemoveHookList(gameObject);
        foreach(RopeScript rope in attachedRopes)
        {
            if(rope != null)
                rope.DesattachRopeFromHook();
        }
    }

    private void OnExplosive()
    {
        PM.AddImpulsiveForce(Vector3.up , 7f);
    }

    public void AddRope(RopeScript rope)
    {
        attachedRopes.Add(rope);
    }


}
