using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSFXHandler : MonoBehaviour
{
    public static CarSFXHandler instance;

    [SerializeField] private AudioSource tireScreech;
    [SerializeField] private AudioSource engine;
    [SerializeField] private AudioSource carHit;
    [SerializeField] private AudioSource jumping;
    [SerializeField] private AudioSource landing;
    [SerializeField] private ParticleSystem angry;

    private float enginePitch = 0.5f;
    private float tireScreechPitch = 0.5f;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        UpdateEngineSFX();
        UpdateTireScreechSFX();
    }

    private void UpdateEngineSFX()
    {
        float velocityMagnitude = CarController.instance.GetVelocityMagnitude();

        float engineVolume = velocityMagnitude * 0.05f;
        engineVolume = Mathf.Clamp(engineVolume, 0.2f, 1f);

        engine.volume = Mathf.Lerp(engine.volume, engineVolume, Time.deltaTime * 10);

        enginePitch = velocityMagnitude * 0.2f;
        enginePitch = Mathf.Clamp(enginePitch, 0.5f, 2f);

        engine.pitch = Mathf.Lerp(engine.pitch, enginePitch, Time.deltaTime * 1.5f);
    }

    private void UpdateTireScreechSFX()
    {
        if (CarController.instance.IsTireScreeching(out float lateralVelocity, out bool isBreaking))
        {
            if (isBreaking)
            {
                tireScreech.volume = Mathf.Lerp(tireScreech.volume, 1f, Time.deltaTime * 10);
                tireScreechPitch = Mathf.Lerp(tireScreechPitch, 0.5f, Time.deltaTime * 10);
            }
            else
            {
                tireScreech.volume = Mathf.Abs(lateralVelocity) * 0.05f;
                tireScreechPitch = Mathf.Abs(lateralVelocity) * 0.1f;
            }
        }
        else
        {
            tireScreech.volume = Mathf.Lerp(tireScreech.volume, 0, Time.deltaTime * 10);
        }
    }

    public void PlayJumpSFX()
    {
        jumping.Play();
    }

    public void PlayLandSFX()
    {
        landing.Play();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float realtiveVelocity = collision.relativeVelocity.magnitude;

        float volume = realtiveVelocity * 0.1f;

        carHit.pitch = Random.Range(0.95f, 1.05f);
        carHit.volume = volume;

        if (!carHit.isPlaying)
        {
            carHit.Play();
            if (PassengerManager.instance.hasPassenger)
            {
                angry.Play();
                PassengerManager.instance.crashes++;
            }
        }
    }
}
