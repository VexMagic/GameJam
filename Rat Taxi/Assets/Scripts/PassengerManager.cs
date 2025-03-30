using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassengerManager : MonoBehaviour
{
    public static PassengerManager instance;

    [SerializeField] private float destinationDistance;
    [SerializeField] private float newPassengerTime;
    [SerializeField] private float dropOffTime;
    [SerializeField] private float speedUpAmount;
    [SerializeField] private float distanceModifier;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI start;
    [SerializeField] private TextMeshProUGUI score;

    private float secondsLeft;
    private int cheeseScore;
    private float speed = 1f;

    private float startTimer = 3;

    private PickupSpot[] pickupSpots;

    private List<PickupSpot> recentSpots = new List<PickupSpot>();

    public bool hasPassenger;
    public PickupSpot currentSpot;

    public bool hasStarted;
    public int crashes;

    private void Awake()
    {
        instance = this;
        pickupSpots = FindObjectsOfType<PickupSpot>();
    }

    private void Start()
    {
        SetNewPassengers();
    }

    private void Update()
    {
        if (hasStarted)
        {
            start.gameObject.SetActive(false);
            timer.gameObject.SetActive(true);
            score.gameObject.SetActive(true);

            secondsLeft -= Time.deltaTime;
            timer.text = String.Format("{0:0.00}", secondsLeft);
            score.text = String.Format("{0:0000}", cheeseScore) + " CHEE$E";
            if (secondsLeft < 0)
            {
                SceneManager.LoadScene(2);
            }
        }
        else
        {
            start.gameObject.SetActive(true);
            timer.gameObject.SetActive(false);
            score.gameObject.SetActive(false);

            startTimer -= Time.deltaTime;
            start.text = String.Format("{0:0}", startTimer);
            if (startTimer < 0)
            {
                hasStarted = true;
            }
        }
    }

    public void SetTargetSpot()
    {
        List<PickupSpot> tempSpots = GetValidSpots();

        for (int i = 0; i < tempSpots.Count; i++)
        {
            if (tempSpots[i].GetDistance() < destinationDistance)
            {
                tempSpots.RemoveAt(i);
                i--;
            }
        }

        int index = UnityEngine.Random.Range(0, tempSpots.Count);

        tempSpots[index].SetDestination();
        AddRecentSpot(tempSpots[index]);

        secondsLeft = dropOffTime * speed * tempSpots[index].GetDistance() * distanceModifier;
        crashes = 0;
    }

    public void SetNewPassengers()
    {
        speed -= speedUpAmount;

        GainCheese((int)(10 * secondsLeft) - (crashes * 10));

        Debug.Log(speed);
        List<PickupSpot> tempSpots = GetClosestSpots(4);

        float tempDistance = 0;

        int passengerAmount = 2;
        for (int i = 0; i < passengerAmount; i++)
        {
            int tempIndex = UnityEngine.Random.Range(0, tempSpots.Count);

            tempDistance += tempSpots[i].GetDistance();
            tempSpots[tempIndex].SetPassenger();
            AddRecentSpot(tempSpots[tempIndex]);
            tempSpots.RemoveAt(tempIndex);
        }

        secondsLeft = newPassengerTime * speed * (tempDistance / passengerAmount) * distanceModifier;
    }

    public void GainCheese(int amount)
    {
        cheeseScore += amount;
        SceneTransition.instance.score = cheeseScore;
    }

    private void AddRecentSpot(PickupSpot spot)
    {
        recentSpots.Add(spot);
        if (recentSpots.Count >= 10)
        {
            recentSpots.RemoveAt(0);
        }
    }

    private List<PickupSpot> GetValidSpots()
    {
        QuestPointerWindow.instance.RemovePointers();
        ResetSpotValues();

        List<PickupSpot> tempSpots = pickupSpots.ToList();
        tempSpots.Remove(currentSpot);

        foreach (var item in recentSpots)
        {
            tempSpots.Remove(item);
        }

        return tempSpots;
    }

    private List<PickupSpot> GetClosestSpots(int amount)
    {
        List<PickupSpot> tempSpots = GetValidSpots();
        List<PickupSpot> tempNewSpots = new List<PickupSpot>();

        for (int i = 0; i < amount; i++)
        {
            PickupSpot tempSpot = null;
            float distance = 0;
            foreach (PickupSpot item in tempSpots)
            {
                if (distance == 0 || distance > item.GetDistance())
                {
                    tempSpot = item;
                    distance = item.GetDistance();
                }
            }

            tempSpots.Remove(tempSpot);
            tempNewSpots.Add(tempSpot);
        }

        return tempNewSpots;
    }

    private void ResetSpotValues()
    {
        foreach (var item in pickupSpots)
        {
            item.ResetValues();
        }
    }
}
