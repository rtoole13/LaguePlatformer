using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Emission : MonoBehaviour
{
    public UnityEvent enteredEmission;
    public UnityEvent exitedEmission;
    private MeshRenderer renderer;
    private BoxCollider2D collider;

    private void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        collider = GetComponent<BoxCollider2D>();
    }
    void Start()
    {

    }

    public virtual void Warn()
    {
        renderer.material.color = new Color(0.9716981f, 0.3620951f, 0.4919671f, 0.3921569f);
    }

    public virtual void Emit()
    {
        renderer.material.color = new Color(0.4390019f, 0.972549f, 0.3607844f, 0.3921569f);
        collider.enabled = true;
        renderer.enabled = true;
    }

    public virtual void DisableEmit()
    {
        collider.enabled = false;
        renderer.enabled = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("we collidin'");
        enteredEmission.Invoke();
    }
    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("we no longer collidin'");
        exitedEmission.Invoke();
    }
}
