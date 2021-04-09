using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool isDrawingLine;
    public Line currentLine;
    // Start is called before the first frame update
    void Start()
    {
        isDrawingLine = false;
    }

    // Update is called once per frame
    void Update()
    {
        MouseController.Instance.InputToAddElement("rect", "Rectangles");
    }
}
