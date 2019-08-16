using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField : Field
{
    [SerializeField]
    private int damagePerTick = 1;

    [SerializeField]
    private int timeBetweenTicks = 3;
    private float tickTime;
    public Material emissionMaterial;
    void Start()
    {
        enterMessage = " entered a damage gravity field!";
        exitMessage = " exited a damage gravity field!";
    }
    void Update()
    {
        float currentTime = Time.time;
        if (currentTime - tickTime > timeBetweenTicks)
        {
            DealDamage();
            tickTime = currentTime;
        }
    }
    public override void Emit()
    {
        meshRenderer.material = emissionMaterial;
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
        
    }
    private void DealDamage()
    {
        RaycastHit2D[] hits = new RaycastHit2D[10];
        boxCollider.Cast(Vector2.up, hits, 0f, true);
        foreach(RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damagePerTick, false);
                    Debug.Log("Field dealt " + damagePerTick + " to " + hit.collider.gameObject.name);
                }
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + enterMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (fieldAffected != null && damageable != null)
        {
            fieldAffected.OnFieldEnter(this);
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
        }
    }
}