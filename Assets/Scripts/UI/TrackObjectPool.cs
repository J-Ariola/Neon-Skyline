using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObjectPool : MonoBehaviour {

    public GameObject prefab;
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    //Returns instance of prefab
    public GameObject GetObject()
    {
        GameObject spawnedGameObject;

        //If there is an inactive instance of the prefab ready to return, return that
        if (inactiveInstances.Count > 0)
        {
            //Remove instance from the collection of inactive instances
            spawnedGameObject = inactiveInstances.Pop();
        }
        //Otherwise, create a new instance
        else
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(prefab);

            //Add the PooledObject component to the prefab so we know it came from this pool
            PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
            pooledObject.pool = this;
        }

        //Put the instance in the root of the scene and enable it
        spawnedGameObject.transform.SetParent(null);
        //spawnedGameObject.transform.localScale = new Vector3(1, 1, 1);
        spawnedGameObject.SetActive(true);

        //return a reference to the instance
        return spawnedGameObject;
    }
    // Return an instance of the prefab to the pool
    public void ReturnObject(GameObject toReturn)
    {
        PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

        // if the instance came from this pool, return it to the pool
        if (pooledObject != null && pooledObject.pool == this)
        {
            // make the instance a child of this and disable it
            toReturn.transform.SetParent(transform);
            toReturn.SetActive(false);

            // add the instance to the collection of inactive instances
            inactiveInstances.Push(toReturn);
        }
        // otherwise, just destroy it
        else
        {
            Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
            Destroy(toReturn);
        }
    }
}

//Identifies the pool that a GameObject came from
public class PooledObject : MonoBehaviour
{
    public TrackObjectPool pool;
}
