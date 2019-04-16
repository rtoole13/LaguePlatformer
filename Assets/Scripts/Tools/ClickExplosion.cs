using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickExplosion : MonoBehaviour
{
    public float force;
    public float duration;
    public float radius;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 location = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            location.z = 0;

            RaycastHit2D[] hits = Physics2D.CircleCastAll(location, radius, Vector2.up);
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(new Vector2(0, force));
                }
                    
            }
        }    
    }
}
