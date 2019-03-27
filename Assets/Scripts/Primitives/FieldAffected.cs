using System;
using UnityEngine;

public class FieldAffected : MonoBehaviour, IFieldAffected
{
    public void OnFieldEnter<T>(T field)
    {
        //Entering Field
    }

    public void OnFieldExit<T>(T field)
    {
        //Leaving field
    }
}