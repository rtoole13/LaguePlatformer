using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private Vector3 respawnPoint;
    private bool alive = true;

    private void Start()
    {
        respawnPoint = transform.position;
    }
    public void Kill()
    {
        if (alive)
        {
            alive = false;
            Respawn();
        }
    }
    public void Respawn()
    {
        transform.position = respawnPoint;
        alive = true;
    }
    public void SetRespawnPoint(Vector3 position)
    {
        Debug.Log("Respawn point moved to: " + position);
        respawnPoint = position;
    }
}
