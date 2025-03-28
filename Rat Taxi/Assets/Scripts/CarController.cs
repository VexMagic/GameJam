using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CarController : MonoBehaviour
{
    public static CarController instance;

    [SerializeField] private float drift;
    [SerializeField] private float acceleration;
    [SerializeField] private float turn;
    [SerializeField] private float minTurnValue;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float jumpOffset;

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer shadow;

    [SerializeField] private AnimationCurve jumpCurve;

    private float accelerationInput;
    private float steeringInput;

    private float rotationAngle;
    private float velocity;

    bool isJumping;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();
    }

    private void ApplyEngineForce()
    {
        velocity = Vector2.Dot(transform.up, rb.velocity);

        if (velocity > maxSpeed && accelerationInput > 0)
            return;

        if (velocity < -maxSpeed * 0.5 && accelerationInput < 0)
            return;

        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0)
            return;

        if (accelerationInput == 0)
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        else
            rb.drag = 0;

        Vector2 engineForce = transform.up * accelerationInput * acceleration;

        rb.AddForce(engineForce, ForceMode2D.Force);
    }

    private void ApplySteering()
    {
        float minSpeedForTurning = (rb.velocity.magnitude / minTurnValue);
        minSpeedForTurning = Mathf.Clamp01(minSpeedForTurning);

        rotationAngle -= steeringInput * turn * minSpeedForTurning;

        rb.MoveRotation(rotationAngle);
    }

    public void SetInputVector(Vector2 input)
    {
        steeringInput = input.x;
        accelerationInput = input.y;
    }

    private void KillOrthogonalVelocity()
    {
        Vector2 forward = transform.up * Vector2.Dot(rb.velocity, transform.up);
        Vector2 right = transform.right * Vector2.Dot(rb.velocity, transform.right);

        rb.velocity = forward + right * drift;
    }

    private float GetLateralVelocity()
    {
        return Vector2.Dot(transform.right, rb.velocity);
    }

    public float GetVelocityMagnitude()
    {
        return rb.velocity.magnitude;
    }

    public bool IsTireScreeching(out float lateralVelocity, out bool isBreaking)
    {
        lateralVelocity = GetLateralVelocity();
        isBreaking = false;

        if (accelerationInput < 0 && velocity > 0)
        {
            isBreaking = true;
            return true;
        }
        else if (Mathf.Abs(GetLateralVelocity()) > 4.0f)
        {
            return true;
        }

        return false;
    }

    public void StartJump(float jumpHeight, float jumpPush)
    {
        if (!isJumping)
        {
            StartCoroutine(Jump(jumpHeight, jumpPush));
        }
    }

    private IEnumerator Jump(float jumpHeight, float jumpPush)
    {
        isJumping = true;

        float statTime = Time.time;

        while (isJumping) 
        {
            float percentage = (Time.time - statTime) / jumpDuration;
            percentage = Mathf.Clamp01(percentage);

            sprite.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(percentage) * jumpHeight;
            
            shadow.transform.localPosition = new Vector3(1, -1) * jumpOffset * jumpCurve.Evaluate(percentage) * jumpHeight;
            shadow.transform.localScale = sprite.transform.localScale * 0.75f;


            if (percentage == 1)
                break;

            yield return null;
        }

        sprite.transform.localScale = Vector3.one;
        shadow.transform.localPosition = Vector3.zero;
        shadow.transform.localScale = Vector3.one;

        isJumping = false;
    }
}
