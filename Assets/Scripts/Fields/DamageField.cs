using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField : Field
{
    [SerializeField]
    private int damagePerTick = 1;

    [SerializeField]
    private int timeBetweenTicks = 4;

    public Material emissionMaterial;
    public void Start()
    {
        enterMessage = " entered a damage gravity field!";
        exitMessage = " exited a damage gravity field!";
    }

    public override void Emit()
    {
        meshRenderer.material = emissionMaterial;
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + enterMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (fieldAffected != null && damageable != null)
        {
            fieldAffected.OnFieldEnter(this);
            damageable.AddDamageOverTime(gameObject, damagePerTick, timeBetweenTicks, true);
        }
    }
    public override void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + exitMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (fieldAffected != null && damageable != null)
        {
            fieldAffected.OnFieldExit(this);
            damageable.RemoveDamageOverTimeBySource(gameObject);
        }
    }
}