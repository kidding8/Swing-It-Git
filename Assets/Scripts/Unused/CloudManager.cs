using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour {
    private AuxManager aux;
    private Camera cam;
    public ObjectPooler cloudPrefab;
    float camWidth;
    //Set this variable to how often you want the Cloud Manager to make clouds in seconds.
    //For Example, I have this set to 2
    public float delay;
    //If you ever need the clouds to stop spawning, set this variable to false, by doing: CloudManagerScript.spawnClouds = false;
    public static bool spawnClouds = true;

    // Use this for initialization
    void Start()
    {
        //Begin SpawnClouds Coroutine
        aux = AuxManager.instance;
        cam = aux.GetCamera();
        camWidth = cam.orthographicSize * cam.aspect;
        StartCoroutine(SpawnClouds());
    }

    private void Update()
    {
        transform.position = new Vector3(cam.transform.position.x + camWidth, transform.position.y, transform.position.z);
    }


    IEnumerator SpawnClouds()
    {
        //This will always run
        while (true)
        {
            //Only spawn clouds if the boolean spawnClouds is true
            while (spawnClouds)
            {
                //Instantiate Cloud Prefab and then wait for specified delay, and then repeat
                GameObject cloud = cloudPrefab.GetPooledObject();
                cloud.SetActive(true);
                cloud.transform.position = transform.position;
                cloud.GetComponent<CloudScript>().Reset(transform.position);
                
                Debug.Log("CLoud spawn");
                yield return new WaitForSeconds(delay);
            }
        }
    }
}
