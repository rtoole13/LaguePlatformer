using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpikeTrap : MonoBehaviour
{
    public UnityEvent spikeTrapEnter;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        spikeTrapEnter.Invoke();
    }
}
