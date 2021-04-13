using UnityEngine;

public interface IInputController
{
    // CheckAllInput must contain input handling.
    void CheckAllInput(string elementTag, string elementLayerName, string lineLayerName);
}
