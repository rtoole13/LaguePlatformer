using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2d : MonoBehaviour {


    public LayerMask collisionMask;

    const float skinWidth = 0.015f;

    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float maxClimbAngle = 80;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D boxColl;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    // Use this for initialization
    void Start () {
        boxColl = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
    }

    public void Move(Vector3 dist)
    {
        UpdateRaycastOrigins();
        collisions.Reset();

        if (dist.x != 0)
        {
            HorizontalCollisions(ref dist);
        }
        if (dist.y != 0)
        {
            VerticalCollisions(ref dist);
        }

        transform.Translate(dist);
    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxColl.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.botLeft  = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.botRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft  = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);

    }

    void HorizontalCollisions(ref Vector3 translate)
    {
        float directionX = Mathf.Sign(translate.x);
        float rayLength = Mathf.Abs(translate.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.botLeft : raycastOrigins.botRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            if (hit)
            {

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        translate.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref translate, slopeAngle);
                    translate.x += distanceToSlopeStart * directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    translate.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        translate.y = Mathf.Abs(translate.x) * Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad);
                    }
                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector3 translate)
    {
        float directionY = Mathf.Sign(translate.y);
        float rayLength = Mathf.Abs(translate.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.botLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + translate.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                translate.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {
                    translate.x = translate.y * Mathf.Sign(translate.x) / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad);
                }
                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
        if (collisions.climbingSlope)
        {
            float directionX = Mathf.Sign(translate.x);
            rayLength = Mathf.Abs(translate.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.botLeft:raycastOrigins.botRight)) + Vector2.up * translate.y;

        }
    }

    void ClimbSlope(ref Vector3 translate, float slopeAngle)
    {
        float moveDist = Mathf.Abs(translate.x);
        float climbTranslateY = moveDist * Mathf.Sin(slopeAngle * Mathf.Deg2Rad);

        if (translate.y <= climbTranslateY)
        {
            translate.x = moveDist * Mathf.Sign(translate.x) * Mathf.Cos(slopeAngle * Mathf.Deg2Rad);
            translate.y = climbTranslateY;
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;

        public float slopeAngle, slopeAngleOld;
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }
    void CalculateRaySpacing()
    {
        Bounds bounds = boxColl.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount   = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);

    }
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 botLeft, botRight;
    }
}