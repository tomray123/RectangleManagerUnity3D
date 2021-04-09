﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour, IInputController
{
    // Singleton instance.
    public static MouseController Instance;

    private void Awake()
    {
        // Setting the singleton instance to the MouseController.
        Instance = this;
    }

    // Defines the input method for adding an element.
    public void InputToAddElement(string tag, string layerName = "Rectangles")
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Set position to spawn an element.
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;

            GameObject spawnedObject = ObjectPooler.Instance.SpawnFromPool(tag, position, Quaternion.identity);

            // Check for spawnedObject exists.
            if (spawnedObject == null)
            {
                Debug.LogWarning("GameObject from pool with tag " + tag + " doesn't exist.");
                return;
            }

            SpriteRenderer spriteRenderer = spawnedObject.GetComponent<SpriteRenderer>();
            // Check for spriteRenderer is attached to spawnedObject.
            if (spriteRenderer == null)
            {
                Debug.LogWarning("No SpriteRenderer component attached to this GameObject.");
                return;
            }

            // Deactivating the object in order to check for free space firstly.
            spawnedObject.SetActive(false);
            // Getting the sprite size.
            Vector2 itemSize = spriteRenderer.bounds.size;

            // Getting layer's number and creating a layerMask.
            int layerNumber = LayerMask.NameToLayer(layerName);
            int layerMask = 1 << layerNumber;

            // OverlapBox returns Collider2D which intersects created box.
            Collider2D intersection = Physics2D.OverlapBox(position, itemSize, 0f, layerMask);
            // Check for intersections.
            if (intersection == null)
            {
                spawnedObject.SetActive(true);
            }
            else
            {
                Debug.Log("Found an intersection.");
            }
        }
    }

    // Defines the input method for deleting an element.
    public void InputToDeleteElement()
    {

    }

    // Defines the input method for dragging an element.
    public void InputToDragElement()
    {

    }

    // Defines the input method for starting a connection (line).
    public void InputToStartDrawingConnection()
    {

    }

    // Defines the input method for drawing and finishing a connection (line).
    public void InputToKeepDrawingConnection()
    {

    }

    // Defines the input method for deleting a connection (line).
    public void InputToDeleteConnection()
    {

    }
}
