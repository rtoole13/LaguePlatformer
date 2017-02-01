using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2d))]
public class Player : MonoBehaviour {

    public float jumpHeight = 4;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.1f;
    float moveSpeed = 6;
    float gravity;
    float velocityXsmoothing;
    float jumpVelocity;
    Vector3 velocity;

    Controller2d controller;

	// Use this for initialization
	void Start () {
        controller = GetComponent<Controller2d>();

        gravity = -2 * jumpHeight / Mathf.Pow(timeToJumpApex, 2f);
        jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        print("Gravity: " + gravity + " Jump Velocity: " + jumpVelocity);
	}
	
	// Update is called once per frame
	void Update () {
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
           
        }
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXsmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
	}
}