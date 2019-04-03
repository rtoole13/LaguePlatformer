using UnityEngine;
using System.Collections;

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

  
    private bool isAirborne = false;
    private bool recentlyLanded = false;

    private float currentSlideSpeedTarget;
    private float currentOverrideSpeedTarget;
    private float overrideDirection = 1f;

    private bool overrideSlide = false;
    private bool isSliding = false;
    
    private float velocityXsmoothing;
    private float jumpVelocity;

    private float slideVelocityStopThreshold;
    private float slideVelocityThreshold;
    private Vector3 velocity;

	protected override void Start () {
        base.Start();
        ResetGravity();

        slideVelocityThreshold = relativeSlideThreshold * moveSpeed;
        slideVelocityStopThreshold = relativeSlideStopThreshold * moveSpeed;

        currentSlideSpeedTarget = slideSpeed;
        currentOverrideSpeedTarget = slideSpeed;
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
                currentOverrideSpeedTarget = velocity.magnitude;
            }
                
            
            velocity.y = 0;
        }
        //Get player input
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isSliding = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            currentSlideSpeedTarget = slideSpeed;
            isSliding = false;
        }
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
            isSliding = false;
        }

        float targetVelocityX = GetTargetVelocityX(input.x);
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXsmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;

        isAirborne = !controller.collisions.below;
        controller.Move(velocity * Time.deltaTime);

        
	}

    private float GetTargetVelocityX(float inputX)
    {
        if (recentlyLanded)
        {
            //landing from a jump
            float directionX = Mathf.Sign(inputX);
            Vector2 rayOrigin = (directionX == -1) ? controller.raycastOrigins.botRight : controller.raycastOrigins.botLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, controller.collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if ((slopeAngle != 0) && (hit.distance - MovementController.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)))
                {
                    //Slope's in the normal x dir
                    overrideDirection = Mathf.Sign(hit.normal.x);
                    overrideSlide = true;

                    Debug.Log("Initial velX: " + currentOverrideSpeedTarget);
                    return overrideDirection * currentOverrideSpeedTarget;
                }
            }
        }
        //overridden control slide
        if (overrideSlide)
        {
            Vector2 rayOrigin = (Mathf.Sign(velocity.x) == -1) ? controller.raycastOrigins.botRight : controller.raycastOrigins.botLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, controller.collisionMask);
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if ((slopeAngle != 0) && (hit.distance - MovementController.skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x)))
                {
                    if (slopeAngle >= controller.maxClimbAngle)
                    {
                        //Slope's in the normal x dir
                        overrideDirection = Mathf.Sign(hit.normal.x);
                        return overrideDirection * currentOverrideSpeedTarget;
                    }
                }
            }
            currentOverrideSpeedTarget *= (1f - relativeHorizontalSlideDecay);
            //Debug.Log("Next velX: " + currentOverrideSpeedTarget);
            if (currentOverrideSpeedTarget <= slideSpeed)
            {
                currentSlideSpeedTarget = slideSpeed;
                overrideSlide = false;
            }
            return overrideDirection * currentOverrideSpeedTarget;
        }

        //normal controls
        if (isSliding && controller.collisions.below)
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

    public override void ResetGravity()
    {
        gravity = -2f * jumpHeight / Mathf.Pow(timeToJumpApex, 2f);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
    }
}