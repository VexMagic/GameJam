using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Tree : MonoBehaviour
{
    [SerializeField] private GameObject standing;
    [SerializeField] private GameObject fallen;

    private bool hasFallen;

    private void Awake()
    {
        standing.SetActive(true);
        fallen.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == CarController.instance.gameObject && !hasFallen)
        {
            float angle = UtilsClass.GetAngleFromVectorFloat(CarController.instance.GetMoveDirection());
            fallen.transform.localEulerAngles = new Vector3(0, 0, angle - 90);

            standing.SetActive(false);
            fallen.SetActive(true);

            hasFallen = true;
        }
    }
}
