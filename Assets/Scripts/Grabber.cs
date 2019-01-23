using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabber : MonoBehaviour
{
    
    private PlayerManager PM;
    private AuxManager aux;
    private EffectsManager EM;
    private List<RopeScript> attachedRopes;

    public bool isTeleport = false;
    private GameObject player;
    
    private SpriteRenderer sRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //throwHook = AuxManager.instance.GetPlayer().GetComponent<ThrowHook>();
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        aux = AuxManager.instance;
        player = PM.GetPlayer();
        attachedRopes = new List<RopeScript>();
        sRenderer = GetComponent<SpriteRenderer>();
        sRenderer.color = aux.foreGroundColor;
        sRenderer.sprite = aux.GetGrabberSprite();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            OnDeath();
        }
        else if (other.CompareTag("Player") && PM.CanCollectObjects() && PM.useGrabberJump)
        {
            OnDeath();
            
            OnExplosive();
        }
    }

    public void OnDeath() 
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        
        gameObject.SetActive(false);
        // isAttached = false;
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

    public void AddRope(RopeScript rope)
    {
        //isAttached = true;
        attachedRopes.Add(rope);
    }

    public void RemoveRope(RopeScript rope)
    {
        //isAttached = false;
        attachedRopes.Remove(rope);
    }

    


    public void CheckIfTeleporter()
    {
        if (isTeleport)
        {
            PM.TeleportToPoint(transform);
        }
    }

}
