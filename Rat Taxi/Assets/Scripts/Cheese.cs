using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheese : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == CarController.instance.gameObject)
        {
            PassengerManager.instance.GainCheese(100);
            Destroy(gameObject);
        }
    }
}
