using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : Field
{
    public float gravityValue = -1f;
    public void Start()
    {
        enterMessage = " entered a zero gravity field!";
        exitMessage = " exited a zero gravity field!";
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + enterMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        MovementModel movementModel = collision.GetComponent<MovementModel>();
        if (fieldAffected != null && movementModel != null)
        {
            fieldAffected.OnFieldEnter(this);
            movementModel.SetGravity(gravityValue);
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + exitMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        MovementModel movementModel = collision.GetComponent<MovementModel>();
        if (fieldAffected != null && movementModel != null)
        {
            fieldAffected.OnFieldExit(this);
            movementModel.ResetGravity();
        }
    }
}
