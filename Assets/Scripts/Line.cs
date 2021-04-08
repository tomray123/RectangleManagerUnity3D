using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    // LineRenderer instance (just line).
    public LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        // Get LineRenderer component and initialize the line.
        line = GetComponent<LineRenderer>();

        // Create and set new color to the line
        Color newColor = new Color(Random.Range(0.0f, 1f), Random.Range(0.0f, 1f), Random.Range(0.0f, 1f));
        line.startColor= newColor;
        line.endColor = newColor;
    }

    // UpdateLinePointPosition changes the position of some point of the line.
    public void UpdateLinePointPosition(int pointIndex, Vector3 newPosition)
    {
        // If the line exists, then set new point position.
        if (line != null)
        {
            line.SetPosition(pointIndex, newPosition);
        }
    }
}
