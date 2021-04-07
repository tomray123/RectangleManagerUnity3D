using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rectangle : MonoBehaviour, IPooledObject
{
    // Sprite of the object.
    private SpriteRenderer sprite;
    // Dictionary which contains connection lines and their points connected to the rectangle.
    public Dictionary<GameObject, int> connections;

    // Start is called before the first frame update
    void Start()
    {
        // Get SpriteRenderer component and initialize the sprite.
        sprite = GetComponent<SpriteRenderer>();
    }

    // OnObjectSpawn is called when the object is spawned.
    public void OnObjectSpawn()
    {
        // Create and set new color to the sprite
        Color newColor = new Color(Random.Range(0, 256), Random.Range(0, 256), Random.Range(0, 256));
        sprite.color = newColor;
    }
}
