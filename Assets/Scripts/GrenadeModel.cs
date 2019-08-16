using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeModel : ProjectileMovementModel
{
    private ICanFire fireInput;
    private float damageDifference;
    public float explosionRadius;
    public float explosionForce;

    public float damageMinimum;
    public float damageMaximum;

    
    protected override void Awake()
    {
        base.Awake();
        damageDifference = damageMaximum - damageMinimum;
    }
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
                    Vector2 distance = (hit.transform.position - origin);
                    float normalizedDistance = Mathf.Clamp((explosionRadius - distance.magnitude) / explosionRadius, 0.1f, 1f);

                    IForceAffected movementModel = hit.rigidbody.GetComponent<IForceAffected>();
                    if (movementModel != null)
                    {
                        float invNormDistSq = Mathf.Pow(normalizedDistance, 2);
                        movementModel.AddExternalForce(explosionForce * invNormDistSq * distance.normalized);
                    }
                    IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        float damage = damageMinimum + damageDifference * normalizedDistance;
                        damageable.TakeDamage((int)damage, true);
                    }
                }
            }
    }
}
