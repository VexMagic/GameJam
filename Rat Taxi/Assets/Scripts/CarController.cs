using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    public static CarController instance;

    [SerializeField] private float drift;
    [SerializeField] private float acceleration;
    [SerializeField] private float turn;
    [SerializeField] private float minTurnValue;
    [SerializeField] private float breakMultiplier;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float jumpDuration;
    [SerializeField] private float jumpOffset;
    [SerializeField] private float minPickupSpeed;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D hitbox;

    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteRenderer shadow;

    [SerializeField] private GameObject cameraFollow;
    [SerializeField] private float maxCameraOffset;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private ParticleSystem landing;

    private float accelerationInput;
    private float steeringInput;

    private float rotationAngle;
    private float velocity;

    private bool isJumping;

    private void Awake()
    {
        instance = this;
    }

    private void FixedUpdate()
    {
        if (!PassengerManager.instance.hasStarted)
            return;

        ApplyEngineForce();

        KillOrthogonalVelocity();

        ApplySteering();

        SetCameraTarget();

        InteractWithPickupSpot();
    }

    private void InteractWithPickupSpot()
    {
        if (Mathf.Abs(velocity) < minPickupSpeed && PassengerManager.instance.currentSpot != null && !isJumping)
        {
            if (PassengerManager.instance.hasPassenger)
            {
                if (PassengerManager.instance.currentSpot.isTarget)
                {
                    PassengerManager.instance.hasPassenger = false;
                    PassengerManager.instance.SetNewPassengers();
                }
            }
            else
            {
                if (PassengerManager.instance.currentSpot.hasPassenger)
                {
                PassengerManager.instance.hasPassenger = true;
                PassengerManager.instance.SetTargetSpot();
                }
            }
        }
    }

    private void SetCameraTarget()
    {
        float cameraOffset = velocity / maxSpeed;
        cameraOffset = Mathf.Clamp01(cameraOffset);

        if (velocity < 0)
        {
            cameraOffset = velocity / (maxSpeed * 0.5f);
            cameraOffset = Mathf.Clamp(cameraOffset, -0.5f, 0);
        }

        cameraFollow.transform.localPosition = new Vector3(0, cameraOffset * maxCameraOffset, 0);
    }

    private void ApplyEngineForce()
    {
        if (isJumping && accelerationInput < 0)
            accelerationInput = 0;

        velocity = Vector2.Dot(transform.up, rb.velocity);

        if (velocity > maxSpeed && accelerationInput > 0)
            return;

        if (velocity < -maxSpeed * 0.5 && accelerationInput < 0)
            return;

        if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed && accelerationInput > 0 && !isJumping)
            return;

        if (accelerationInput == 0)
            rb.drag = Mathf.Lerp(rb.drag, 3.0f, Time.fixedDeltaTime * 3);
        else
            rb.drag = 0;

        Vector2 engineForce = transform.up * accelerationInput * acceleration;

        if (velocity > 0 && accelerationInput < 0)
        {
            engineForce *= breakMultiplier;
        }

        rb.AddForce(engineForce, ForceMode2D.Force);

        
    }

    public Vector2 GetMoveDirection()
    {
        return rb.velocity;
    }

    private void ApplySteering()
    {
        if (isJumping) 
           return;

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

        if (isJumping)
            return false;

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
        float jumpDuration = rb.velocity.magnitude * 0.05f;

        jumpHeight = jumpHeight * rb.velocity.magnitude * 0.05f;
        jumpHeight = Mathf.Clamp01(jumpHeight);

        shadow.transform.parent = null;

        SetJumpCollisions(true);

        CarSFXHandler.instance.PlayJumpSFX();

        rb.AddForce(rb.velocity.normalized * jumpPush * 10, ForceMode2D.Impulse);

        while (isJumping) 
        {
            float percentage = (Time.time - statTime) / jumpDuration;
            percentage = Mathf.Clamp01(percentage);

            sprite.transform.localScale = Vector3.one + Vector3.one * jumpCurve.Evaluate(percentage) * jumpHeight;

            shadow.transform.position = transform.position + (new Vector3(1, -1) * jumpOffset * jumpCurve.Evaluate(percentage) * jumpHeight);
            shadow.transform.rotation = transform.rotation;
            shadow.transform.localScale = sprite.transform.localScale * 0.75f;

            if (percentage == 1)
                break;

            yield return null;
        }

        hitbox.enabled = false;
        if (Physics2D.OverlapCircle(transform.position, 1.4f) && Physics2D.OverlapCircle(transform.position, 1.4f).GetComponent<Tilemap>() == null)
        {
            isJumping = false;

            StartJump(0.2f, 0.1f);
            hitbox.enabled = true;
        }
        else
        {
            hitbox.enabled = true;

            sprite.transform.localScale = Vector3.one;
            shadow.transform.parent = transform;
            shadow.transform.localPosition = Vector3.zero;
            shadow.transform.localEulerAngles = Vector3.zero;
            shadow.transform.localScale = Vector3.one;

            SetJumpCollisions(false);

            if (jumpHeight > 0.1f)
            {
                landing.Play();

                CarSFXHandler.instance.PlayLandSFX();
            }

            isJumping = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Jump"))
        {
            JumpData jumpData = collision.GetComponent<JumpData>();
            StartJump(jumpData.jumpHeight, jumpData.jumpPush);
        }
    }

    private void SetJumpCollisions(bool ignore)
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject item in temp)
        {
            Physics2D.IgnoreCollision(hitbox, item.GetComponent<Collider2D>(), ignore);
        }
    }

    public float GetSpeedPercentage()
    {
        float speed = Mathf.Abs(velocity);
        return Mathf.Clamp01(speed / maxSpeed);
    }
}
