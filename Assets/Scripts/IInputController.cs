using UnityEngine;

public interface IInputController
{
    void CheckAllInput(string tag, string elementLayerName, string lineLayerName);
    /*
    // Defines the input method for adding an element.
    void InputToAddElement(string tag, string layerName);
    // Defines the input method for deleting an element.
    void InputToDeleteElement();
    // Defines the input method for dragging an element.
    void InputToDragElement(string layerName);
    // Defines the input method for starting a connection (line).
    void InputToDrawConnection(string tag, string layerName);
    // Defines the input method for drawing and finishing a connection (line).
    void InputToDeleteConnection();
    */
}
