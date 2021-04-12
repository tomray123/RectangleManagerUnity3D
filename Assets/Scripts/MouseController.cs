using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour, IInputController
{
    public float timeBetweenClicks = 0f;
    public bool isFirstClick = false;
    public Vector2 firstClickPosition;
    public Vector2 secondClickPosition;
    public float maxDoubleClickDistance = 1f;
    public float maxTimeBetweenClicks = 0.3f;
    public float MaxRayLength = 15f;
    public RaycastHit2D hit;
    public bool isDragging = false;
    public GameObject activeObject;
    public Vector2 objectStartPosition;
    public bool isDrawingConnection = false;
    public Line drawingLine;

    // Singleton instance.
    public static MouseController Instance;

    private void Awake()
    {
        // Setting the singleton instance to the MouseController.
        Instance = this;

        timeBetweenClicks = 0f;
        isFirstClick = false;
        isDragging = false;
        isDrawingConnection = false;
    }

    // Defines the input method for deleting an element.
    public void InputToDeleteElement()
    {
        if (activeObject != null)
        {
            Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();
            if (activeObjectScript == null)
            {
                Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                return;
            }
            foreach (Line line in activeObjectScript.connections.Keys)
            {
                line.UpdateLinePointPosition(0, Vector3.zero);
                line.UpdateLinePointPosition(1, Vector3.zero);
                int indexPoint = 0;
                if (activeObjectScript.connections[line] == 0)
                {
                    indexPoint = 1;
                }
                line.DeleteLinePoint(indexPoint);
                line.gameObject.SetActive(false);
            }
            activeObjectScript.connections.Clear();
            activeObject.SetActive(false);
            activeObject = null;
        }
    }

    // Defines the input method for dragging an element.
    public void InputToDragElement(string layerName)
    {
        // Getting layer's number and creating a layerMask.
        int layerNumber = LayerMask.NameToLayer(layerName);
        int layerMask = 1 << layerNumber;

        if (Input.GetMouseButton(0))
        {
            if (!isDrawingConnection)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (activeObject != null)
                {
                    isDragging = true;
                    activeObject.transform.position = mousePosition;             
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (!isDrawingConnection)
            {
                if (isDragging)
                {
                    SpriteRenderer spriteRenderer = activeObject.GetComponent<SpriteRenderer>();

                    if (spriteRenderer == null)
                    {
                        Debug.LogWarning("No SpriteRenderer component is attached to this GameObject.");
                        return;
                    }
                    spriteRenderer.sortingOrder -= 1;

                    if (IsEmptySpace(activeObject, Camera.main.ScreenToWorldPoint(Input.mousePosition), layerMask))
                    {
                        activeObject = null;
                    }
                    else
                    {
                        activeObject.transform.position = objectStartPosition;
                    }

                    isDragging = false;
                }
            }
        }
    }

    // Defines the input method for starting a connection (line).
    public void InputToDrawConnection(string tag, string layerName)
    {
        // Getting layer's number and creating a layerMask.
        int layerNumber = LayerMask.NameToLayer(layerName);
        int layerMask = 1 << layerNumber;

        if (Input.GetMouseButtonDown(1))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);

            if (!isDrawingConnection)
            {
                if (hit.collider != null)
                {
                    activeObject = hit.collider.gameObject;
                    Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();
                    if (activeObjectScript == null)
                    {
                        Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                        return;
                    }
                    GameObject spawnedLine = ObjectPooler.Instance.SpawnFromPool(tag, mousePosition, Quaternion.identity);
                    if (spawnedLine == null)
                    {
                        Debug.LogWarning("GameObject from pool with tag " + tag + " doesn't exist.");
                        return;
                    }
                    Line lineScript = spawnedLine.GetComponent<Line>();
                    if (lineScript == null)
                    {
                        Debug.LogWarning("No Line component is attached to this GameObject.");
                        return;
                    }
                    lineScript.UpdateLinePointPosition(0, activeObject.transform.position);
                    lineScript.UpdateLinePointPosition(1, activeObject.transform.position);
                    lineScript.begin = activeObjectScript;
                    activeObjectScript.connections.Add(lineScript, 0);
                    drawingLine = lineScript;
                    isDrawingConnection = true;
                }

            }
            else
            {
                if (hit.collider != null && hit.collider.gameObject != activeObject)
                {
                    activeObject = hit.collider.gameObject;
                    drawingLine.UpdateLinePointPosition(1, activeObject.transform.position);
                    Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();
                    if (activeObjectScript == null)
                    {
                        Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                        return;
                    }
                    drawingLine.end = activeObjectScript;
                    activeObjectScript.connections.Add(drawingLine, 1);
                }
                else
                {
                    Rectangle activeObjectScript = activeObject.GetComponent<Rectangle>();
                    if (activeObjectScript == null)
                    {
                        Debug.LogWarning("No Rectangle component is attached to this GameObject.");
                        return;
                    }
                    activeObjectScript.connections.Remove(drawingLine);
                    drawingLine.UpdateLinePointPosition(0, Vector3.zero);
                    drawingLine.UpdateLinePointPosition(1, Vector3.zero);
                    drawingLine.gameObject.SetActive(false);
                }
                isDrawingConnection = false;
                activeObject = null;
            }   
        }
        if (isDrawingConnection)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            drawingLine.UpdateLinePointPosition(1, mousePosition);
        }
    }

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
            Debug.Log("Found an intersection");
            return false;
        }
    }

    public int SingleOrDoubleClick()
    {
        if (isFirstClick)
        {
            timeBetweenClicks += Time.deltaTime;
        }
        if (timeBetweenClicks > 5f)
        {
            timeBetweenClicks = 0f;
            isFirstClick = false;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!isDrawingConnection)
            {
                Debug.Log(timeBetweenClicks);
                if (timeBetweenClicks > 0f && timeBetweenClicks < maxTimeBetweenClicks)
                {
                    secondClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    Vector2 distanceBetweenClicks = secondClickPosition - firstClickPosition;
                    if (distanceBetweenClicks.magnitude < maxDoubleClickDistance)
                    {
                        Debug.Log("DoubleClick");
                        isFirstClick = false;
                        timeBetweenClicks = 0f;
                        return 2;
                    }
                }

                isFirstClick = true;
                firstClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                timeBetweenClicks = 0f;
                return 1;
            }
        }
        return 0;
    }

    public void CheckAllInput(string tag, string elementLayerName, string lineLayerName)
    {
        // Getting layer's number and creating a layerMask.
        int elementLayerNumber = LayerMask.NameToLayer(elementLayerName);
        int elementLayerMask = 1 << elementLayerNumber;

        // Getting layer's number and creating a layerMask.
        int lineLayerNumber = LayerMask.NameToLayer(lineLayerName);
        int lineLayerMask = 1 << lineLayerNumber;

        switch (SingleOrDoubleClick())
        {
            case 1:
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hit = Physics2D.Raycast(mousePosition, Vector2.zero);
                
                if (hit.collider != null)
                {
                    activeObject = hit.collider.gameObject;
                    if (activeObject.layer == elementLayerNumber)
                    {
                        objectStartPosition = activeObject.transform.position;
                        SpriteRenderer spriteRenderer = activeObject.GetComponent<SpriteRenderer>();

                        if (spriteRenderer == null)
                        {
                            Debug.LogWarning("No SpriteRenderer component is attached to this GameObject.");
                            return;
                        }
                        spriteRenderer.sortingOrder += 1;
                    }
                    else
                    {
                        activeObject = null;
                    }
                }
                else
                {
                    // Set position to spawn an element.
                    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    position.z = 0;

                    GameObject spawnedObject = ObjectPooler.Instance.SpawnFromPool(tag, position, Quaternion.identity);

                    // Check if spawnedObject exists.
                    if (spawnedObject == null)
                    {
                        Debug.LogWarning("GameObject from pool with tag " + tag + " doesn't exist.");
                        return;
                    }

                    if (!IsEmptySpace(spawnedObject, position, elementLayerMask))
                    {
                        spawnedObject.SetActive(false);
                    }
                }

                break;

            case 2:

                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, elementLayerMask);
                if (hit.collider != null)
                {
                    activeObject = hit.collider.gameObject;
                    InputToDeleteElement();
                }

                hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, lineLayerMask);
                if (hit.collider != null)
                {
                    activeObject = hit.collider.gameObject;
                    GameObject line = activeObject.transform.parent.gameObject;
                    activeObject = null;
                    Line lineScript = line.GetComponent<Line>();
                    if (lineScript == null)
                    {
                        Debug.LogWarning("No Line component is attached to this GameObject.");
                        return;
                    }
                    lineScript.DeleteLinePoint(0);
                    lineScript.DeleteLinePoint(1);
                    line.SetActive(false);
                }

                break;
        }

        InputToDragElement(elementLayerName);
        InputToDrawConnection("line", "Rectangles");
    }
}
