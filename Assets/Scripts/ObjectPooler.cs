using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class describes the one type objects pool. 
[System.Serializable]
public class Pool
{
    // Pool tag.
    public string tag;
    // Pool objects prefab.
    public GameObject prefab;
    // Start pool size.
    public int size;
}

public class ObjectPooler : MonoBehaviour
{
    // Pools list.
    public List<Pool> pools;
    // Dictionary of tags and objects in pool.
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Singleton instance.
    public static ObjectPooler Instance;

    private void Awake()
    {
        // Setting the singleton instance to the ObjectPooler.
        Instance = this;
    }

    // Start is called before the first frame update.
    void Start()
    {
        // Initializing poolDictionary.
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Create objects and add them to the pool.
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            // Add the pool with its tag to the dictionary.
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // SpawnFromPool activates object from the object pool.
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // Check for presence of the required tag.
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag" + tag + "doesn't exist.");
            return null;
        }

        // Creating new GameObject to put the spawned object in it. 
        GameObject objectToSpawn = null;
        // isFreeFound shows whether inactive gameobject is in the pool.
        bool isFreeFound = false;

        // Search for inactive object to spawn from pool.
        for (int i = 0; i < poolDictionary[tag].Count; i++)
        {
            // Get object from the pool.
            objectToSpawn = poolDictionary[tag].Dequeue();

            // If the object is active we won't use it.
            if (objectToSpawn.activeSelf)
            {
                poolDictionary[tag].Enqueue(objectToSpawn);
            }
            // Else we found inactive object that can be used.
            else
            {
                isFreeFound = true;
                break;
            }
        }
        // Check for presence of the available object from the pool.
        if (!isFreeFound)
        {
            // Find pool with required tag.
            Pool pool = pools.Find(x => x.tag.Contains(tag));

            if (pool != null)
            {
                // Instantiate the object.
                objectToSpawn = Instantiate(pool.prefab);
            }
        }

        // Activate the object.
        objectToSpawn.SetActive(true);
        // Set position and rotation of the object.
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Getting the interface of spawned object in order to call OnObjectSpawn() method.
        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        // If spawned object contains classes derived from IPooledObject, OnObjectSpawn() is called.
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        // Add spawned object back to queue.
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}
