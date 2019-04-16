using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovementModel : MovementModel
{
    public bool bouncy = false;
    private bool recentlyLanded;
    private bool isAirborne;
    private Vector2 landingVelocity;

    public float drag { get; private set; }

    protected override void Start()
    {
        base.Start();
        landingVelocity = new Vector2(0f, 0f);
    }
    void Update()
    {
        recentlyLanded = false;

        //collision from above or below, stop velocity y
        if (controller.collisions.above)
        {
            velocity.y = 0;
        }
        else if (controller.collisions.below)
        {
            if (isAirborne)
            {
                //was airborne last frame, just landed
                recentlyLanded = true;
                landingVelocity = velocity;
            }
            velocity.y = 0;
        }
        //velocity.x = GetTargetVelocityX(input.x);
        velocity.y += gravity * Time.deltaTime;

        isAirborne = !controller.collisions.below;
        controller.Move(velocity * Time.deltaTime);

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
