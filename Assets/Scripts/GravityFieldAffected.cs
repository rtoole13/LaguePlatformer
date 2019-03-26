using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementModel))]
public class GravityFieldAffected : MonoBehaviour, IGravityFieldAffected
{
    MovementModel movementModel;
    void Start()
    {
        movementModel = GetComponent<MovementModel>();
    }
    public void OnFieldEnter(GravityField field)
    {
        //Do stuff  
        movementModel.SetGravity(field.gravityValue);
    }

    public void OnFieldExit(GravityField field)
    {
        //Undo stuff
        movementModel.ResetGravity();
    }
}