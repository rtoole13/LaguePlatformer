using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class MovementController : MonoBehaviour {


    public LayerMask collisionMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    public const float skinWidth = 0.015f;

    public float maxClimbAngle { get; private set; }
    public float maxDescendAngle { get; private set; }

    private float horizontalRaySpacing;
    private float verticalRaySpacing;

    private BoxCollider2D boxColl;
    public RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    private void Awake()
    {
        maxClimbAngle = 65;
        maxDescendAngle = 70;
    }
    // Use this for initialization
    void Start () {
        boxColl = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
    }

    public void Move(Vector3 dist)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.translateOld = dist;
        if (dist.y < 0)
        {
            DescendSlope(ref dist);
        }
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
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        translate = collisions.translateOld;
                    }

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
                    Debug.Log("weee");
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
            Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.botLeft:raycastOrigins.botRight) + Vector2.up * translate.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    translate.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
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

    void DescendSlope(ref Vector3 translate)
    {
        float directionX = Mathf.Sign(translate.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.botRight : raycastOrigins.botLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(translate.x))
                    {
                        float moveDistance = Mathf.Abs(translate.x);
                        float descendVelocityY = moveDistance * Mathf.Sin(slopeAngle * Mathf.Deg2Rad) ;

                        translate.x = moveDistance * Mathf.Sign(translate.x) * Mathf.Cos(slopeAngle * Mathf.Deg2Rad);
                        translate.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }
                }
            }
        }
    }
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;
        public bool climbingSlope;
        public bool descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 translateOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    public Vector2 GetSlopeNormal(float currentDirection)
    {
        if (!collisions.below)
            return Vector2.up;

        Vector2 rayOrigin = (currentDirection == -1) ? raycastOrigins.botRight : raycastOrigins.botLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            return hit.normal;
        }
        return Vector2.up;
    }
    public float GetSlopeAngle(float currentDirection)
    {
        if (!collisions.below)
            return 0f;

        Vector2 rayOrigin = (currentDirection == -1) ? raycastOrigins.botRight : raycastOrigins.botLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            return Vector2.Angle(hit.normal, Vector2.up);
        }
        return 0f;
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 botLeft, botRight;
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
}


