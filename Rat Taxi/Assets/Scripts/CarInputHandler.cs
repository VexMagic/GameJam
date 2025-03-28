using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    void Update()
    {
        Vector2 input = Vector2.zero;

        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        CarController.instance.SetInputVector(input);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CarController.instance.StartJump(0.5f, 0);
        }
    }
}
