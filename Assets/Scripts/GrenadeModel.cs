using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeModel : ProjectileMovementModel
{
    private ICanFire fireInput;

    public float explosionRadius;
    public float explosionForce;

    public void Initialize(Vector2 velocityTarget, float dragCoefficient, ICanFire fireInputController)
    {
        Initialize(velocityTarget, dragCoefficient);
        fireInput = fireInputController;
        fireInput.OnAltFire += DetonateSelf;
    }

    private void DetonateSelf(Vector2 direction)
    {
        fireInput.OnAltFire -= DetonateSelf;
        ApplyForces();
        Destroy(gameObject);
    }
    private void ApplyForces()
    {
        Vector3 origin = transform.position;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explosionRadius, Vector2.up);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.rigidbody != null)
                {
                    IForceAffected movementModel = hit.rigidbody.GetComponent<IForceAffected>();
                    if (movementModel != null)
                    {
                        
                        Vector2 distance = (hit.transform.position - origin);
                        float invNormDist = Mathf.Pow(Mathf.Clamp((explosionRadius - distance.magnitude) / explosionRadius, 0.1f, 1f), 2);
                        movementModel.AddExternalForce(explosionForce * invNormDist * distance.normalized);
                    }
                }
            }
    }
}
