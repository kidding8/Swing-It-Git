﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    
    private PlayerManager PM;
    private AuxManager aux;
    private EffectsManager EM;
    private List<RopeScript> attachedRopes;

    public bool randomSprite = false;
    public bool isFriendly = false;
    private GameObject player;
    
    private SpriteRenderer sRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //throwHook = AuxManager.instance.GetPlayer().GetComponent<ThrowHook>();
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        aux = AuxManager.instance;
        player = aux.GetPlayer();
        attachedRopes = new List<RopeScript>();
        sRenderer = GetComponent<SpriteRenderer>();

        if (isFriendly)
        {
            TurnFriendly();
        }
            
        else{
            TurnDeadly();
        }
           

        if(randomSprite)
            sRenderer.sprite = aux.GetGrabberSprite();
    }

   private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            OnDeath();
        }
        /*else if (other.CompareTag("Player") && PM.CanCollectObjects() && PM.useGrabberJump)
        {
            OnDeath();
            
            OnExplosive();
        }*/
    }

    public void TurnFriendly()
    {
        sRenderer.color = aux.friendlyColor;
        gameObject.tag = "Grabber";
    }

    public void TurnDeadly()
    {
        sRenderer.color = aux.deadlyColor;
        gameObject.tag = "Enemy";
    }

    public void OnDeath() 
    {
        //EM.SetCoinPickUpParticles(transform.position);
        //EM.CreateDisappearingCircle(transform.position);
        
        gameObject.SetActive(false);
        if (isFriendly)
            TurnFriendly();
        else
            TurnDeadly();
        // isAttached = false;
        if (randomSprite)
            sRenderer.sprite = AuxManager.instance.GetGrabberSprite();
        PM.RemoveGrabbableObject(gameObject);
        /*foreach(RopeScript rope in attachedRopes)
        {
            if(rope != null)
            {
                rope.DesattachRopeFromHook();
            }
                
        }*/
    }

    private void OnExplosive()
    {
        PM.Jump(PM.grabberJumpForce);
    }
}
