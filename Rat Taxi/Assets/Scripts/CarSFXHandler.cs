using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSFXHandler : MonoBehaviour
{
    [SerializeField] private AudioSource tireScreech;
    [SerializeField] private AudioSource engine;
    [SerializeField] private AudioSource carHit;

    private float enginePitch = 0.5f;
    private float tireScreechPitch = 0.5f;

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
}
