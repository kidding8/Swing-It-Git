using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPowerScript : MonoBehaviour
{
    private PlayerManager PM;
    

    // Start is called before the first frame update
    void Start()
    {
        PM = PlayerManager.instance;    
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PM.GetNewAirPower();
            gameObject.SetActive(false);
        }
    }
}
