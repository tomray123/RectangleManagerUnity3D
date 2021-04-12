using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public bool isDrawingLine;
    public Line currentLine;
    public IInputController inputController;
    // Start is called before the first frame update
    void Start()
    {
        isDrawingLine = false;
        inputController = GetComponent<IInputController>();
    }

    // Update is called once per frame
    void Update()
    {
        inputController.InputToAddElement("rect", "Rectangles");
        //inputController.InputToDeleteElement();
        inputController.InputToDragElement("Rectangles");
        inputController.InputToDrawConnection("line", "Rectangles");
    }
}
