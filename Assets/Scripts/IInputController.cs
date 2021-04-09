using UnityEngine;

public interface IInputController
{
    // Defines the input method for adding an element.
    void InputToAddElement(string tag, string layerName);
    // Defines the input method for deleting an element.
    void InputToDeleteElement();
    // Defines the input method for dragging an element.
    void InputToDragElement();
    // Defines the input method for starting a connection (line).
    void InputToStartDrawingConnection();
    // Defines the input method for drawing and finishing a connection (line).
    void InputToKeepDrawingConnection();
    // Defines the input method for deleting a connection (line).
    void InputToDeleteConnection();
}
