using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrainHandler : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private ParticleSystem smokeParticle;

    [SerializeField] private float breakSmokeEmissions;
    [SerializeField] private float driftSmokeMultiply;
    [SerializeField] private float smokeEmissionsDecrease;

    ParticleSystem.EmissionModule smokeEmission;
    
    private float particleRate;

    private void Awake()
    {
        smokeEmission = smokeParticle.emission;
        smokeEmission.rateOverTime = 0;

        trailRenderer.emitting = false;
    }

    void Update()
    {
        particleRate = Mathf.Lerp(particleRate, 0, Time.deltaTime * smokeEmissionsDecrease);
        smokeEmission.rateOverTime = particleRate;

        if (CarController.instance.IsTireScreeching(out float lateralVelocity, out bool isBreaking))
        {
            trailRenderer.emitting = true;

            if (isBreaking)
                particleRate = breakSmokeEmissions;
            else 
                particleRate = Mathf.Abs(lateralVelocity) * driftSmokeMultiply;
        }
        else 
            trailRenderer.emitting = false;
    }
}
