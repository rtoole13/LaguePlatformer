using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeModel : ProjectileMovementModel
{
    private ICanFire fireInput;

    public void Initialize(Vector2 velocityTarget, float dragCoefficient, ICanFire fireInputController)
    {
        Initialize(velocityTarget, dragCoefficient);
        fireInput = fireInputController;
        fireInput.OnAltFire += DetonateSelf;
    }

    private void DetonateSelf(Vector2 direction)
    {
        fireInput.OnAltFire -= DetonateSelf;
        Destroy(gameObject);
    }
}
