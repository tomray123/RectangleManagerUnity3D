using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour, IInputController
{
    // Time between clicks is intended to detect doublec clicks.
    public float timeBetweenClicks = 0f;

    // isFirstClick indicates the first left mouse button click.
    public bool isFirstClick = false;

    // Left mouse button clicks positions.
    public Vector2 firstClickPosition;
    public Vector2 secondClickPosition;

    // Max distance betwen clicks to detect double click.
    public float maxDoubleClickDistance = 1f;

    // Max time betwen clicks to detect double click.
    public float maxTimeBetweenClicks = 0.3f;

    public RaycastHit2D hit;

    // The object which was clicked or interacted with.
    public GameObject activeObject;

    // isDragging is intended to detect the drag mode.
    public bool isDragging = false;

    // Object position before dragging.
    public Vector2 objectStartPosition;

    // Distance between element position and mouse position when dragging.
    public Vector2 dragOffset;

    // isDrawingConnection is intended to detect the drawing connections mode.
    public bool isDrawingConnection = false;

    // Current drawing line.
    public Line drawingLine;

    // Start is called before the first frame update.
    public void Start()
    {
        timeBetweenClicks = 0f;
        isFirstClick = false;
        isDragging = false;
        isDrawingConnection = false;
    }

    // CheckAllInput checks for every input and runs the appropriate methods.
    public void CheckAllInput(string elementTag, string elementLayerName, string lineLayerName)
    {

        // Getting an element layer number and creating a layerMask.
        int elementLayerNumber = LayerMask.NameToLayer(elementLayerName);
        int elementLayerMask = 1 << elementLayerNumber;

        // Getting a line layer number and creating a layerMask.
        int lineLayerNumber = LayerMask.NameToLayer(lineLayerName);
        int lineLayerMask = 1 << lineLayerNumber;

        // Check for single or double click on left mouse button.
        switch (SingleOrDoubleClick())
        {
            // If single click, then set an active object (if exists) or try to spawn an element.
            case 1:

                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, elementLayerMask);

                // If the clicked object is an element (rectangle), then set its start position in case of dragging and increase the sorting order in layers.
                if (hit.collider != null)
                {
                    activeObject = hit.collider.gameObject;

                    objectStartPosition = activeObject.transform.position;
                    SpriteRenderer spriteRenderer = activeObject.GetComponent<SpriteRenderer>();

                    // Set the offset between mouse position and elemnet position.
                    dragOffset = objectStartPosition - mousePosition;

                    // Checking for the existence of a SpriteRenderer script attached to this object.
                    if (spriteRenderer == null)
                    {
                        Debug.LogWarning("No SpriteRenderer component is attached to this GameObject.");
                        return;
                    }

                    spriteRenderer.sortingOrder += 1;
                }
                else
                {
                    hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, lineLayerMask);

                    // If none of objects were clicked, then try to spawn an element.
                    if (hit.collider == null)
                    {
                        
                        SpawnAnElement(elementTag, elementLayerNumber, mousePosition);
                    }
                }

                break;

            // If double click, then detect what to delete.
            case 2:

                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Check for double click on an element (rectangle).
                hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, elementLayerMask);
                if (hit.collider != null)
                {
                    // Set as active object and the delete it.
                    activeObject = hit.collider.gameObject;
                    DeleteElement();
                }
                else
                {
                    // Check for double click on a line and delete it.
                    hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, lineLayerMask);
                    if (hit.collider != null)
                    {
                        // Set as active object and the delete it.
                        activeObject = hit.collider.gameObject;
                        DeleteLine();
                    }
                }

                break;
        }

        // Check the input to drag the element.
        InputToDragElement(elementLayerMask);
        // Check the input to draw the connection.
        InputToDrawConnection("line", elementLayerMask);
    }

    // DeleteElement deletes the element (rectangle) and lines connected to it.
    public void DeleteElement()
    {
        // Checking for the existence of an object to be deleted.
        if (activeObject != null)
        {
            Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();

            // Checking for the existence of an Rectangle script attached to this object.
            if (activeObjectScript == null)
            {
                Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                return;
            }

            // Deleting all lines attached to this element.
            foreach (Line line in activeObjectScript.connections.Keys)
            {
                line.UpdateLinePointPosition(0, Vector3.zero);
                line.UpdateLinePointPosition(1, Vector3.zero);
                int indexPoint = 0;
                // Defining the end of the line attache to other elements.
                if (activeObjectScript.connections[line] == 0)
                {
                    indexPoint = 1;
                }
                line.DeleteLinePoint(indexPoint);
                line.gameObject.SetActive(false);
            }

            // Clear the connections and delete an element.
            activeObjectScript.connections.Clear();
            activeObject.SetActive(false);
            activeObject = null;
        }
    }

    // DeleteLine deletes the line and clears all connections.
    public void DeleteLine()
    {
        // Get the line object.
        GameObject line = activeObject.transform.parent.gameObject;
        // Set the line as inactive object.
        activeObject = null;

        Line lineScript = line.GetComponent<Line>();

        // Checking for the existence of a Line script attached to this object.
        if (lineScript == null)
        {
            Debug.LogWarning("No Line component is attached to this GameObject.");
            return;
        }

        // Set start and begin points to zero.
        lineScript.UpdateLinePointPosition(0, Vector3.zero);
        lineScript.UpdateLinePointPosition(1, Vector3.zero);

        // Clear all connections and put the line back in object pool.
        lineScript.DeleteLinePoint(0);
        lineScript.DeleteLinePoint(1);
        line.SetActive(false);
    }

    // StartDragElement sets the position of element to mouse position with some offset.
    public void StartDragElement()
    {
        // Checking whether connections drawing mode is disabled.
        if (!isDrawingConnection)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // If element exists, then start the drag mode and update element position.
            if (activeObject != null)
            {
                isDragging = true;
                activeObject.transform.position = mousePosition + dragOffset;
            }
        }
    }

    // FinishDragElement sets the position of element to mouse position with some offset.
    public void FinishDragElement(int elementLayerMask)
    {
        // Checking whether connections drawing mode is disabled.
        if (!isDrawingConnection)
        {
            // Checking whether dragging mode is enabled.
            if (isDragging)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                SpriteRenderer spriteRenderer = activeObject.GetComponent<SpriteRenderer>();

                // Checking for the existence of a spriteRenderer script attached to this object.
                if (spriteRenderer == null)
                {
                    Debug.LogWarning("No SpriteRenderer component is attached to this GameObject.");
                    return;
                }

                // Setting sorting order layer back to default.
                spriteRenderer.sortingOrder -= 1;

                // If an obstacle is encountered, then return to the starting position.
                if (!IsEmptySpace(activeObject, mousePosition + dragOffset, elementLayerMask))
                {
                    activeObject.transform.position = objectStartPosition;
                }

                // Disable dragging mode
                activeObject = null;
                isDragging = false;
            }
        }
    }

    // Defines the input method for dragging an element.
    public void InputToDragElement(int elementLayerMask)
    {
        if (Input.GetMouseButton(0))
        {
            StartDragElement();
        }
        if (Input.GetMouseButtonUp(0))
        {
            FinishDragElement(elementLayerMask);
        }
    }

    // StartDrawConnection is called on right button click on an element (rectangle).
    public void StartDrawConnection(string tag, Vector3 position)
    {
        Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();

        // Checking for the existence of a Rectangle script attached to this object.
        if (activeObjectScript == null)
        {
            Debug.LogWarning("No Rectangle component is attached to this GameObject.");
            return;
        }

        GameObject spawnedLine = ObjectPooler.Instance.SpawnFromPool(tag, position, Quaternion.identity);

        // Checking for the existence of a line in object pool.
        if (spawnedLine == null)
        {
            Debug.LogWarning("GameObject from pool with tag " + tag + " doesn't exist.");
            return;
        }

        Line lineScript = spawnedLine.GetComponent<Line>();

        // Checking for the existence of a Line script attached to this object.
        if (lineScript == null)
        {
            Debug.LogWarning("No Line component is attached to this GameObject.");
            return;
        }

        // Set the line points to the element.
        lineScript.UpdateLinePointPosition(0, activeObject.transform.position);
        lineScript.UpdateLinePointPosition(1, activeObject.transform.position);

        // Bind start point with the element.
        lineScript.begin = activeObjectScript;
        activeObjectScript.connections.Add(lineScript, 0);
        drawingLine = lineScript;
        // Enable the drawing mode.
        isDrawingConnection = true;
    }

    // FinishDrawConnection is called on right button click on an element (rectangle) while drawing mode is enabled.
    public void FinishDrawConnection()
    {
        // Checking whether clicked element exists and it's not the start element.
        if (hit.collider != null && hit.collider.gameObject != activeObject)
        {
            activeObject = hit.collider.gameObject;
            drawingLine.UpdateLinePointPosition(1, activeObject.transform.position);

            Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();

            // Checking for the existence of a Rectangle script attached to this object.
            if (activeObjectScript == null)
            {
                Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                return;
            }

            // Bind end point with the element.
            drawingLine.end = activeObjectScript;
            activeObjectScript.connections.Add(drawingLine, 1);
        }
        // If neither collider is detected, then delete the line.
        else
        {
            Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();

            // Checking for the existence of a Rectangle script attached to this object.
            if (activeObjectScript == null)
            {
                Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                return;
            }

            // Remove the line point from the element connections.
            activeObjectScript.connections.Remove(drawingLine);
            drawingLine.UpdateLinePointPosition(0, Vector3.zero);
            drawingLine.UpdateLinePointPosition(1, Vector3.zero);
            drawingLine.gameObject.SetActive(false);
        }
        // Disable drawing connections mode.
        isDrawingConnection = false;
        activeObject = null;
    }

    // Defines the input method for drawing a connection (line).
    public void InputToDrawConnection(string tag, int elementLayerMask)
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, elementLayerMask);

            // If the element was clicked with right mouse button and drawing mode was disabled, then start drawing.
            if (!isDrawingConnection)
            {
                if (hit.collider != null)
                {
                    activeObject = hit.collider.gameObject;
                    StartDrawConnection(tag, mousePosition);
                }
            }
            else
            {
                FinishDrawConnection();
            }   
        }

        // Set the end line point position to mouse position when drawing mode is enabled.
        if (isDrawingConnection)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            drawingLine.UpdateLinePointPosition(1, mousePosition);
        }
    }

    // Checks for empty space to place the objectToCheck there. 
    public bool IsEmptySpace(GameObject objectToCheck, Vector2 position, int layerMask)
    {
        Collider2D collider = objectToCheck.GetComponent<Collider2D>();

        // Check if collider is attached to spawnedObject.
        if (collider == null)
        {
            Debug.LogWarning("No Collider2D component is attached to this GameObject.");
            return false;
        }

        // Getting the sprite size.
        Vector2 itemSize = collider.bounds.size;

        // Deactivating the object in order to check for empty space firstly.
        objectToCheck.SetActive(false);

        // OverlapBox returns Collider2D which intersects created box.
        Collider2D intersection = Physics2D.OverlapBox(position, itemSize, 0f, layerMask);

        objectToCheck.SetActive(true);

        // Check for intersections.
        if (intersection == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // SingleOrDoubleClick detects single or double click and returns 1 and 2 accordingly. It also returns 0 when no clicks are detected.
    public int SingleOrDoubleClick()
    {
        // Counts the time between clicks.
        if (isFirstClick)
        {
            timeBetweenClicks += Time.deltaTime;
        }

        // If time between clicks is huge, then set it to 0.
        if (timeBetweenClicks > 5f)
        {
            timeBetweenClicks = 0f;
            isFirstClick = false;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            // Checking whether connections drawing mode is disabled.
            if (!isDrawingConnection)
            {
                // If little time has passed between the clicks and the distance between them is small, then we count a double click.
                if (timeBetweenClicks > 0f && timeBetweenClicks < maxTimeBetweenClicks)
                {
                    secondClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 distanceBetweenClicks = secondClickPosition - firstClickPosition;
                    if (distanceBetweenClicks.magnitude < maxDoubleClickDistance)
                    {
                        isFirstClick = false;
                        timeBetweenClicks = 0f;
                        return 2;
                    }
                }

                // If double click is not detected, then it was a single click.
                isFirstClick = true;
                firstClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                timeBetweenClicks = 0f;
                return 1;
            }
        }
        return 0;
    }

    // SpawnAnElement gets the element from the object pool and place it on the scene if there any empty space.
    public void SpawnAnElement(string tag, int elementLayerNumber, Vector3 position)
    {
        // Getting the layer mask.
        int elementLayerMask = 1 << elementLayerNumber;
        position.z = 0;
        GameObject spawnedObject = ObjectPooler.Instance.SpawnFromPool(tag, position, Quaternion.identity);

        // Check if spawnedObject exists.
        if (spawnedObject == null)
        {
            Debug.LogWarning("GameObject from pool with tag " + tag + " doesn't exist.");
            return;
        }

        // If there is no free space on the scene, then we return the object to the pool.
        if (!IsEmptySpace(spawnedObject, position, elementLayerMask))
        {
            spawnedObject.SetActive(false);
        }
    }
}
