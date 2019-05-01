using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickExplosion : MonoBehaviour
{
    public float force;
    public float radius;
    public int mouseKey;

    private float radiusSq;
    private void Awake()
    {
        radiusSq = radius * radius;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mouseKey))
        {
            Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            location.z = 0;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(location, radius, Vector2.up);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.rigidbody != null)
                {
                    IForceAffected movementModel = hit.rigidbody.GetComponent<IForceAffected>();
                    if (movementModel != null)
                    {
                        
                        Vector2 distance = (hit.transform.position - location);
                        float invNormDist = Mathf.Clamp((radius - distance.magnitude) / radius, 0.1f, 1f);
                        float factor = Mathf.Pow(invNormDist, 2);
                        Debug.Log("dist: " + distance);
                        Debug.Log("inv norm dist: " + invNormDist);
                        Debug.Log("Factor: " + factor);
                        Debug.Log("Force: " + force * factor * distance.normalized);
                        movementModel.AddExternalForce(force * factor * distance.normalized);
                    }
                }
            }
        }    
    }
}
