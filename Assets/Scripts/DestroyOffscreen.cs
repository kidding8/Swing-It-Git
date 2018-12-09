using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOffscreen : MonoBehaviour {
    private AuxManager aux;
    private Camera cam;
    public bool destroyX = false;
    public bool destroyY = false;
    public bool isHook = false;
    public Vector2 widthThreshold;
    //public Vector2 heightThreshold;
    private Vector2 screenPosition;
    // Use this for initialization
    void Start () {
        aux = AuxManager.instance;
        cam = aux.GetCamera();
	}
	
	// Update is called once per frame
	void LateUpdate () {
        screenPosition = cam.WorldToScreenPoint(transform.position);
        if (destroyX && screenPosition.x < widthThreshold.x) {
            if (isHook)
            {
                SpawnHookManager.instance.RemoveHookList(this.gameObject);
            }
            //SpawnManager.instance.RemovePlatformList(gameObject);
            gameObject.SetActive(false);
            
        }
    }
}
