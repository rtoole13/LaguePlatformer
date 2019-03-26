using UnityEngine;
using System.Collections;

public class PlayerMovementModel : MovementModel {

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

    [SerializeField]
    [Range(0f, 1f)]
    private float relativeAscendingSlideDecay = 0.1f;

    [SerializeField]
    [Range(0f, 1f)]
    private float relativeHorizontalSlideDecay = 0.05f;

    private bool justLanded = false;
    private float currentSlideSpeedTarget;
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
    }
	
	void Update () {
        justLanded = !controller.collisions.below;
        //collision from above or below, stop velocity y
        if (controller.collisions.above || controller.collisions.below)
        {
            if (justLanded)
            {
                Debug.Log("Landed");
                velocity.y = 0;
            }
            else
            {
                velocity.y = 0;
            }
        }
        //Debug.Log(justLanded);
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
        controller.Move(velocity * Time.deltaTime);

        
	}

    private float GetTargetVelocityX(float inputX)
    {
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