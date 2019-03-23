using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RespawnPointEnter: UnityEvent<Vector3> { }

public class RespawnPoint : MonoBehaviour
{
    public RespawnPointEnter playerEnteredRespawnPoint;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerEnteredRespawnPoint.Invoke(transform.position);
    }
}
