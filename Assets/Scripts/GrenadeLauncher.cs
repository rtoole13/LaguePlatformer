using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ICanFire))]
public class GrenadeLauncher : MonoBehaviour
{
    public ProjectileMovementModel projectile;
    public float initialSpeed;
    [Range(0f,1f)]
    public float drag;

    void Awake()
    {
        GetComponent<ICanFire>().OnFire += HandleFire;
    }

    void Update()
    {
        
    }
    void HandleFire(Vector2 direction)
    {
        var spawnedProjectile = Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
        spawnedProjectile.Initialize(initialSpeed * direction, drag);
    }
}
