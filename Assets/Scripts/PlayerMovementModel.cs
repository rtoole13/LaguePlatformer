using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerInputModel))]
public class PlayerMovementModel : MovementModel {

    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;

    [SerializeField]
    private float accelerationTimeAirborne = 0.5f;
    [SerializeField]
    private float accelerationTimeGrounded = 0.1f;
    [SerializeField]
    private float accelerationTimeRecentlyLanded = 0.05f;
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private float slideSpeed = 15f;
    [SerializeField]
    [Range(0f,1f)]
    private float relativeSlideThreshold = 0.5f;
    [SerializeField]
    [Range(0f, 1f)]
    private float relativeSlideStopThreshold = 0.0001f;
    [SerializeField]
    [Range(0f, 1f)]
    private float relativeAscendingSlideDecay = 0.1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float relativeHorizontalSlideDecay = 0.05f;

    private PlayerInputModel playerInput;

    private float slipAngleThreshold = 35f;
    private float landingSlideAngleThreshold = 5f;
    private bool isAirborne = false;
    private bool recentlyLanded = false;

    private float currentSlideSpeedTarget;
    private Vector2 landingVelocity;
    private float overrideSlideSpeedTarget;
    private float overrideDirection = 1f;

    private bool overrideSlide = false;
    
    private float velocityXsmoothing;
    private float jumpVelocity;

    private float slideVelocityStopThreshold;
    private float slideVelocityThreshold;
    private Vector3 velocity;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInputModel>();
    }
    protected override void Start () {
        base.Start();
        ResetGravity();

        slideVelocityThreshold = relativeSlideThreshold * moveSpeed;
        slideVelocityStopThreshold = relativeSlideStopThreshold * moveSpeed;

        currentSlideSpeedTarget = slideSpeed;
        overrideSlideSpeedTarget = slideSpeed;
        landingVelocity = new Vector2(slideSpeed, 0f);
    }
	
	void Update () {
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
        //Get player input
        Vector2 input = playerInput.movement;
        if (!playerInput.sliding)
            currentSlideSpeedTarget = slideSpeed;
        
        if (playerInput.jump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        velocity.x = GetTargetVelocityX(input.x);
        velocity.y += gravity * Time.deltaTime;

        isAirborne = !controller.collisions.below;
        controller.Move(velocity * Time.deltaTime);

        
	}

    private float GetTargetVelocityX(float inputX)
    {
        if (controller.collisions.below)
        {
            //grounded
            Vector2 slopeNormal = controller.GetSlopeNormal(Mathf.Sign(velocity.x));

            float slopeAngle = Vector2.Angle(slopeNormal, Vector2.up);
            if (recentlyLanded && slopeAngle >= landingSlideAngleThreshold)
            {
                return DetermineLandingVelocity(inputX, slopeNormal);
            }
            if (overrideSlide)
            {
                return Mathf.SmoothDamp(velocity.x, DetermineOverrideSlideVelocity(inputX, slopeNormal), ref velocityXsmoothing, accelerationTimeGrounded);
            }
            if (slopeAngle <= slipAngleThreshold)
            {
                return Mathf.SmoothDamp(velocity.x, DetermineVelocityNormal(inputX), ref velocityXsmoothing, accelerationTimeGrounded);
            }
            else
            {
                return Mathf.SmoothDamp(velocity.x, DetermineVelocitySlipSlope(slopeNormal), ref velocityXsmoothing, accelerationTimeGrounded);
            }
        }

        //airborne
        return Mathf.SmoothDamp(velocity.x, inputX * moveSpeed, ref velocityXsmoothing, accelerationTimeAirborne);
    }
    private float DetermineLandingVelocity(float inputX, Vector2 slopeNormal)
    {
        overrideDirection = Mathf.Sign(slopeNormal.x);
        overrideSlide = true;
        overrideSlideSpeedTarget = Mathf.Abs(Vector2.Dot(landingVelocity, Vector2.Perpendicular(slopeNormal)));

        return overrideDirection * overrideSlideSpeedTarget;
    }
    private float DetermineOverrideSlideVelocity(float inputX, Vector2 slopeNormal)
    {
        overrideSlideSpeedTarget *= (1f - relativeHorizontalSlideDecay);
        if (overrideSlideSpeedTarget <= slideSpeed)
        {
            currentSlideSpeedTarget = slideSpeed;
            overrideSlide = false;
        }
        return overrideDirection * overrideSlideSpeedTarget;
    }
    private float DetermineVelocityNormal(float inputX)
    {
        if (playerInput.sliding)
        {   
            //On the ground and sliding
            if (controller.collisions.descendingSlope)
            {
                //Descending a slope, slide speed is constant
                return inputX * slideSpeed;
            }
            else {
                if (controller.collisions.climbingSlope)
                {
                    //climbing a slope while sliding
                    currentSlideSpeedTarget *= (1f - relativeAscendingSlideDecay);
                }
                else
                {
                    //sliding on a horizontal
                    currentSlideSpeedTarget *= (1f - relativeHorizontalSlideDecay);
                }
                
                return  inputX * currentSlideSpeedTarget;
            }
        }

        return inputX * moveSpeed;
    }

    private float DetermineVelocitySlipSlope(Vector2 slopeNormal)
    {
        return velocity.x + (Mathf.Sign(slopeNormal.x) * 5f);
    }

    public override void ResetGravity()
    {
        gravity = -2f * jumpHeight / Mathf.Pow(timeToJumpApex, 2f);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
}