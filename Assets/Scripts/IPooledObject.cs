using UnityEngine;

public interface IPooledObject
{
    // Method which will be executed when the object is spawned.
    void OnObjectSpawn();
}
