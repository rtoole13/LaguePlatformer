using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementModel : MovementModel
{
    public bool bouncy = false;
    [Range(0f, 1f)]
    public float relativeBounceLoss;

    [Range(0f, 4f)]
    public float bounceThreshold;

    private float inverseRelativeBounceLoss;
    private bool recentlyLanded;
    private bool isAirborne;
    private Vector2 landingVelocity;

    public float drag { get; private set; }

    protected override void Start()
    {
        inverseRelativeBounceLoss = 1f - relativeBounceLoss;
        base.Start();
        landingVelocity = new Vector2(0f, 0f);
    }
    protected override void Update()
    {
        float deltaTime = Time.deltaTime;
        recentlyLanded = false;

        //collision from above or below, stop velocity y
        if (controller.collisions.above)
        {
            if (bouncy)
            {
                velocity.y = (Mathf.Abs(velocity.y) > bounceThreshold) ? -velocity.y * inverseRelativeBounceLoss : 0;
            }
            else
            {
                velocity.y = 0;
            }
            velocity.y = (bouncy)? - velocity.y : 0;
        }
        else if (controller.collisions.below)
        {
            if (isAirborne)
            {
                //was airborne last frame, just landed
                recentlyLanded = true;
                landingVelocity = velocity;
            }
            if (bouncy)
            {
                velocity.y = (Mathf.Abs(velocity.y) > bounceThreshold) ? -velocity.y * inverseRelativeBounceLoss : 0;
            }
            else
            {
                velocity.y = 0;
            }
        }
        //velocity.x = GetTargetVelocityX(input.x);
        velocity.y += gravity * deltaTime;
        if (effectedByExternalForces)
            applyForces(ref velocity, deltaTime);

        isAirborne = !controller.collisions.below;
        controller.Move(velocity * deltaTime);
    }

    public void Initialize(Vector2 velocityTarget, float dragCoefficient)
    {
        drag = dragCoefficient;
        SetVelocity(velocityTarget);
    }

    private void SetVelocity(Vector2 velocityTarget)
    {
        velocity = velocityTarget;
    }
}
