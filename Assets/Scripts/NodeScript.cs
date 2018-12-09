using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeScript : MonoBehaviour {


    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            //gameObject.SetActive(false);
           if(GameManager.instance.destroyRope) {
                
                RopeScript ropeScript = GetComponentInParent<RopeScript>();
                ropeScript.DestroyRope(this.gameObject);
               // ropeScript.UnhookRope();
            }
            
        }
    }
}
