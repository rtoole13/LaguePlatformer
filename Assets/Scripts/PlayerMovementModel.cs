﻿using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(PlayerInputModel))]
public class PlayerMovementModel : MovementModel {
    #region EVENTS
    public delegate void CameraZoomTo(bool closeView);

    public event CameraZoomTo OnZoomChange;
    #endregion

    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;

    [Range(0f, 1f)]
    public float idleThreshold = 0.1f;

    public float velocityCameraZoomThreshold;
    private bool cameraClose = true;

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
    private float slipSpeed = 5f;
    [SerializeField]
    private float maxGroundSpeed = 40f;
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
    private Animator playerAnimator;

    private bool lookingRight = true;
    private float slipAngleThreshold = 35f;
    private float landingSlideAngleThreshold = 25f;
    private bool isAirborne = false;
    private bool recentlyLanded = false;
    private float slipSpeedDamp = 0.5f;

    private float currentSlideSpeedTarget;
    private Vector2 landingVelocity;
    
    private float velocityXsmoothing;
    private float jumpVelocity;

    private float slideVelocityStopThreshold;
    private float slideVelocityThreshold;
    

    protected override void Awake()
    {
        base.Awake();
        playerInput = GetComponent<PlayerInputModel>();
        playerAnimator = GetComponent<Animator>();
    }

    protected override void Start () {
        base.Start();
        ResetGravity();

        slideVelocityThreshold = relativeSlideThreshold * moveSpeed;
        slideVelocityStopThreshold = relativeSlideStopThreshold * moveSpeed;

        currentSlideSpeedTarget = slideSpeed;
        landingVelocity = new Vector2(slideSpeed, 0f);
    }
	
	protected override void Update () {
        float deltaTime = Time.deltaTime;
        recentlyLanded = false;

        PossiblyZoomCamera();
        //collision from above or below, stop velocity y
        if (controller.collisions.above)
        {
            velocity.y = 0;
        }
        if (controller.collisions.below)
        {
            if (isAirborne)
            {
                //was airborne last frame, just landed
                recentlyLanded = true;
                landingVelocity = velocity;
            }
            velocity.y = 0;

            //animation
            playerAnimator.SetBool("isAirborne", false);
        }
        else
        {
            //animation
            playerAnimator.SetBool("isAirborne", true);
        }
        //Get player input
        Vector2 input = playerInput.movement;
        if (!playerInput.sliding)
            currentSlideSpeedTarget = slideSpeed;
        
        if (playerInput.jump && controller.collisions.below)
        {
            velocity.y = jumpVelocity;

            //animation
            playerAnimator.SetBool("jumped", true);
        }

        velocity.x = GetTargetVelocityX(input.x);
        velocity.y += gravity * deltaTime;

        if (Mathf.Abs(velocity.x) < idleThreshold)
        {
            playerAnimator.SetBool("isRunning", false);
        }
        else
        {
            playerAnimator.SetBool("isRunning", true);
        }

        if ((velocity.x < 0 && lookingRight) || (velocity.x > 0 && !lookingRight))
            FlipLookDirection();

        if (effectedByExternalForces)
            applyForces(ref velocity, deltaTime);

        isAirborne = !controller.collisions.below;
        controller.Move(velocity * deltaTime);
	}

    private void PossiblyZoomCamera()
    {
        //NOTE only makes sense if a camera is listening ;)
        if (cameraClose && (velocity.magnitude > velocityCameraZoomThreshold))
        {
            Debug.Log("To Far!");
            cameraClose = false;
            InvokeCameraZoom();
        }
        else if (!cameraClose && (velocity.magnitude <= velocityCameraZoomThreshold))
        {
            if (isAirborne)
            {
                return;
            }
            Debug.Log("To Close!");
            cameraClose = true;
            InvokeCameraZoom();
        }
    }

    private float GetTargetVelocityX(float inputX)
    {
        float velocityTargetX = inputX * moveSpeed;
        if (controller.collisions.below)
        {
            //grounded
            Vector2 slopeNormal = controller.GetSlopeNormal(Mathf.Sign(velocity.x));

            float slopeAngle = Vector2.Angle(slopeNormal, Vector2.up);
            if (recentlyLanded && slopeAngle >= landingSlideAngleThreshold)
            {

                //ignore smooth damping
                return DetermineLandingVelocity(inputX, slopeNormal);
            }
            if (slopeAngle <= slipAngleThreshold)
            {
                velocityTargetX = DetermineVelocityNormal(inputX);
            }
            else
            {
                velocityTargetX = DetermineVelocitySlipSlope(inputX, slopeNormal);
            }
            return Mathf.SmoothDamp(velocity.x, velocityTargetX, ref velocityXsmoothing, accelerationTimeGrounded);
        }


        //airborne
        return Mathf.SmoothDamp(velocity.x, inputX * moveSpeed, ref velocityXsmoothing, accelerationTimeAirborne);
    }
    private float DetermineLandingVelocity(float inputX, Vector2 slopeNormal)
    {
        float overrideDirection = Mathf.Sign(slopeNormal.x);
        float overrideSlideSpeedTarget;
        if (velocity.x * overrideDirection > 0)
        {
            //moving in override direction
            overrideSlideSpeedTarget = Mathf.Clamp(Mathf.Abs(Vector2.Dot(landingVelocity, Vector2.Perpendicular(slopeNormal))), 0f, maxGroundSpeed);
        }
        else
        {
            //opposing override direction
            Vector2 velocityTarget = landingVelocity;
            velocityTarget.x = landingVelocity.x - velocity.x;
            overrideSlideSpeedTarget = Mathf.Clamp(Mathf.Abs(Vector2.Dot(velocityTarget, Vector2.Perpendicular(slopeNormal))), 0f, maxGroundSpeed);
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

    private float DetermineVelocitySlipSlope(float inputX, Vector2 slopeNormal)
    {
        if (inputX * slopeNormal.x > 0)
        {
            //moving in override direction
            return inputX * slideSpeed;
        }
        return Mathf.Clamp((1-slipSpeedDamp) * velocity.x + (Mathf.Sign(slopeNormal.x) * slipSpeed), -maxGroundSpeed, maxGroundSpeed);
    }

    public override void ResetGravity()
    {
        gravity = -2f * jumpHeight / Mathf.Pow(timeToJumpApex, 2f);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }

    private void FlipLookDirection()
    {
        lookingRight = !lookingRight;
        Vector3 localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        transform.localScale = localScale;
    }

    void InvokeCameraZoom()
    {
        OnZoomChange?.Invoke(cameraClose);
    }
}