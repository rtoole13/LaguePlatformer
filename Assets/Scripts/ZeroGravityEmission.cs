using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ZeroGravityFieldEnter : UnityEvent<Vector3> { }

public class ZeroGravityEmission : Emission
{
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        Debug.Log("Zero G!");
    }
}
