using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementModel))]
public class FieldAffected : MonoBehaviour, IFieldAffected
{
    MovementModel movementModel;
    void Start()
    {
        movementModel = GetComponent<MovementModel>();
    }
    public void OnFieldEnter(Field field)
    {
        //Do stuff  
        throw new NotImplementedException();
    }

    public void OnFieldExit(Field field)
    {
        //Undo stuff
        throw new NotImplementedException();
    }
}