using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class MovementModel : MonoBehaviour, IForceAffected
{
    public bool effectedByExternalForces;
    [Range(0.1f,2f)]
    public float mass = 1f;
    private float inverseMass;

    protected List<Vector2> externalForces = new List<Vector2>();

    protected float gravity = -40f;
    protected MovementController controller;
    protected Vector2 velocity;

    protected virtual void Awake()
    {
        inverseMass = 1f / mass;
    }

    protected virtual void Start()
    {
        gravity = -40f;
        controller = GetComponent<MovementController>();        
    }

    protected virtual void Update()
    {
        //NOTE nothing forces child classes to call on applyForces()..

        float deltaTime = Time.deltaTime;
        //collision from above or below, stop velocity y
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        velocity.y += gravity * deltaTime;

        if(effectedByExternalForces)
            applyForces(ref velocity, deltaTime);

        controller.Move(velocity * deltaTime);
    }

    protected void applyForces(ref Vector2 vel, float deltaTime)
    {
        vel += GetNetForce() * deltaTime * inverseMass;
        externalForces.Clear();
    }

    public virtual void ZeroGravity()
    {
        gravity = -1f;
    }
    public virtual void SetGravity(float newGrav)
    {
        gravity = newGrav;
    }
    public virtual void ResetGravity()
    {
        gravity = -40f;
    }

    public void AddExternalForce(Vector2 force)
    {
        externalForces.Add(force);
    }

    public Vector2 GetNetForce()
    {
        Vector2 netForce = Vector2.zero;
        for (int i = 0; i < externalForces.Count; i++)
        {
            netForce += externalForces[i];
        }
        return netForce;
    }


}
