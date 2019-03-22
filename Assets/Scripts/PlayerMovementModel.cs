using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2d))]
public class PlayerMovementModel : MonoBehaviour {

    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    private float accelerationTimeAirborne = 0.2f;
    private float accelerationTimeGrounded = 0.1f;

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

    private float relativeAscendingSlideDecay = 0.1f;

    private float relativeHorizontalSlideDecay = 0.05f;

    private bool holdingSlide = false;
    private float currentSlideSpeedTarget;
    private bool isSliding = false;
    private float gravity;
    private float velocityXsmoothing;
    private float jumpVelocity;
    private float slideVelocityStopThreshold;
    private float slideVelocityThreshold;
    private Vector3 velocity;

    Controller2d controller;
	void Start () {
        controller = GetComponent<Controller2d>();

        gravity = -2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2f);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);

        slideVelocityThreshold = relativeSlideThreshold * moveSpeed;
        slideVelocityStopThreshold = relativeSlideStopThreshold * moveSpeed;

        currentSlideSpeedTarget = slideSpeed;
    }
	
	void Update () {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
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
        controller.Move(velocity * Time.deltaTime);
	}

    private float GetTargetVelocityX(float inputX)
    {
        if (isSliding)
        {
            if (controller.collisions.descendingSlope)
            {
                return inputX * slideSpeed;
            }
            else {
                if (controller.collisions.climbingSlope)
                {
                    currentSlideSpeedTarget *= (1f - relativeAscendingSlideDecay);
                }
                else
                {
                    currentSlideSpeedTarget *= (1f - relativeHorizontalSlideDecay);
                }
                
                return (controller.collisions.climbingSlope)? inputX * currentSlideSpeedTarget : inputX * currentSlideSpeedTarget;
            }
        }

        return inputX * moveSpeed;
    }
    #region DEBUG
    private void UpdateParameters()
    {

    }
    #endregion //DEBUG
}