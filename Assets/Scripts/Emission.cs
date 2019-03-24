using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Emission : MonoBehaviour
{
    public UnityEvent enteredEmission;
    public UnityEvent exitedEmission;
    private MeshRenderer meshRenderer;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {

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
        enteredEmission.Invoke();
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        exitedEmission.Invoke();
    }
}
