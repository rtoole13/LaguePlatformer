using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Field : MonoBehaviour
{
    protected MeshRenderer meshRenderer;
    protected BoxCollider2D boxCollider;

    protected string enterMessage = " entered a field!";
    protected string exitMessage = " exited a field!";

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public virtual void Warn()
    {
        meshRenderer.material.color = new Color(0.9716981f, 0.3620951f, 0.4919671f, 0.3921569f);
    }

    public virtual void Emit()
    {
        meshRenderer.material.color = new Color(0.4390019f, 0.972549f, 0.3607844f, 0.3921569f);
        boxCollider.enabled = true;
        meshRenderer.enabled = true;
    }

    public virtual void DisableEmit()
    {
        boxCollider.enabled = false;
        meshRenderer.enabled = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + enterMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        if (fieldAffected != null)
            fieldAffected.OnFieldEnter(this);
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name + exitMessage);
        IFieldAffected fieldAffected = collision.GetComponent<IFieldAffected>();
        if (fieldAffected != null)
            fieldAffected.OnFieldExit(this);
    }
}
