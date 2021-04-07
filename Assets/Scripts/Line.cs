using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    // LineRenderer instance (just line).
    public LineRenderer line;

    public void Start()
    {
        // Get LineRenderer component and initialize the line.
        line = GetComponent<LineRenderer>();
    }

    // UpdateLinePointPosition changes the position of some point of the line.
    public void UpdateLinePointPosition(int pointIndex, Transform newPosition)
    {
        // If the line exists, then set new point position.
        if (line != null)
        {
            line.SetPosition(pointIndex, newPosition.position);
        }
    }
}
