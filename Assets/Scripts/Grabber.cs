﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    private PlayerManager PM;
    private EffectsManager EM;
    private ThrowHook throwHook;
    private List<RopeScript> attachedRopes;
    public bool isTeleport = false;
    bool isAttached = false;
    // Start is called before the first frame update
    void Start()
    {
        throwHook = AuxManager.instance.GetPlayer().GetComponent<ThrowHook>();
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        
        attachedRopes = new List<RopeScript>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            OnDeath();
        }
        else if (other.CompareTag("Player") && PM.isTargetable)
        {
            OnDeath();
            if(PM.useGrabberJump)
                OnExplosive();
        }
    }

    public void OnDeath() 
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
        isAttached = false;
        PM.RemoveGrabbableObject(gameObject);
        foreach(RopeScript rope in attachedRopes)
        {
            if(rope != null)
            {
                rope.DesattachRopeFromHook();
            }
                
        }
    }

    private void OnExplosive()
    {
        PM.Jump(PM.grabberJumpForce);
    }

    public void AddRope(RopeScript rope)
    {
        isAttached = true;
        attachedRopes.Add(rope);
    }

    public void CheckIfTeleporter()
    {
        if (isTeleport)
        {
            PM.TeleportToPoint(transform);
        }
    }

}
