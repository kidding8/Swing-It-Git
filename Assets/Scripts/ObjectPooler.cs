using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler: MonoBehaviour {

    public GameObject pooledObject;
    public Transform SpawnnedObjectsTransform;
    public int pooledAmount;

    List<GameObject> listObjects;
	void Awake () {
        listObjects = new List<GameObject>();

        for (int i=0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject) Instantiate(pooledObject);
            obj.transform.SetParent(SpawnnedObjectsTransform,false);
            obj.SetActive(false);
            listObjects.Add(obj);
        }
	}
	
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < listObjects.Count; i++)
        {
            if (!listObjects[i].activeInHierarchy)
            {
                return listObjects[i];
            }
        }

        GameObject obj = (GameObject)Instantiate(pooledObject);
        obj.SetActive(false);
        obj.transform.SetParent(SpawnnedObjectsTransform, false);
        listObjects.Add(obj);
        return obj;
    }

}
