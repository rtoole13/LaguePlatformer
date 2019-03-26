using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class MovementModel : MonoBehaviour
{
    protected float gravity;
    protected MovementController controller;

    protected virtual void Start()
    {
        gravity = -40f;
        controller = GetComponent<MovementController>();        
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
}
