using UnityEngine;

public interface IForceAffected
{
    void AddExternalForce(Vector2 force);
    Vector2 GetNetForce();
}