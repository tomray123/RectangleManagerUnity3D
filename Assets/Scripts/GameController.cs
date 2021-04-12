using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Input Controller Interface.
    public IInputController inputController;

    // Start is called before the first frame update.
    void Start()
    {
        inputController = GetComponent<IInputController>();
    }

    // Update is called once per frame.
    void Update()
    {
        // Calling an interface method without binding to a specific input type.
        inputController.CheckAllInput("rect", "Rectangles", "Connections");
    }
}
