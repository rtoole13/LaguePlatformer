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
        IGravityFieldAffected fieldAffected = collision.GetComponent<IGravityFieldAffected>();
        if (fieldAffected != null)
            fieldAffected.OnFieldEnter(this);
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + exitMessage);
        IGravityFieldAffected fieldAffected = collision.GetComponent<IGravityFieldAffected>();
        if (fieldAffected != null)
            fieldAffected.OnFieldExit(this);
    }
}
