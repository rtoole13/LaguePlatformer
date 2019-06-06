using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ICanFire))]
public class GrenadeLauncher : MonoBehaviour
{
    public GrenadeModel projectile;
    public float initialSpeed;
    [Range(0f,1f)]
    public float drag;

    [SerializeField]
    private Transform weaponOrigin;

    private ICanFire fireInput;

    void Awake()
    {
        fireInput = GetComponent<ICanFire>();
        fireInput.OnFire += HandleFire;
    }

    void OnDestroy()
    {
        fireInput.OnFire -= HandleFire;
    }

    void HandleFire(Vector2 direction)
    {
        var spawnedProjectile = Instantiate(projectile, weaponOrigin.position, Quaternion.identity);
        spawnedProjectile.Initialize(initialSpeed * direction, drag, fireInput);
    }
}
