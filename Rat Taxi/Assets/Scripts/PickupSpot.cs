using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpot : MonoBehaviour
{
    [SerializeField] private GameObject passenger;
    [SerializeField] private SpriteRenderer highlight;

    private bool IsTarget;
    private bool HasPassenger;

    public bool isTarget => IsTarget;
    public bool hasPassenger => HasPassenger;

    public void SetPassenger(bool active)
    {
        passenger.SetActive(active);
    }

    public void SetDestination()
    {
        IsTarget = true;
        QuestPointerWindow.instance.AddPointer(transform.position);
        SetDisplay();
    }

    public void SetPassenger()
    {
        HasPassenger = true;
        QuestPointerWindow.instance.AddPointer(transform.position);
        SetDisplay();
    }

    public void ResetValues()
    {
        IsTarget = false;
        HasPassenger = false;
        Invoke(nameof(SetDisplay), 0.1f);
    }

    private void SetDisplay()
    {
        highlight.gameObject.SetActive(hasPassenger || isTarget);
        passenger.SetActive(hasPassenger);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == CarController.instance.gameObject)
        {
            PassengerManager.instance.currentSpot = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == CarController.instance.gameObject)
        {
            if (PassengerManager.instance.currentSpot == this)
            {
                PassengerManager.instance.currentSpot = null;
            }
        }
    }

    public float GetDistance()
    {
        return Vector3.Distance(CarController.instance.transform.position, transform.position);
    }
}
