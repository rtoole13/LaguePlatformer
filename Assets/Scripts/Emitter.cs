using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emitter : MonoBehaviour
{
    Emission emission;
    public float timeBetweenEmission;
    public float emissionDuration;
    public float whenToStartWarn;


    private bool emitting = false;
    private bool warning = false;
    private float emissionTimer;
    private float durationTimer;
    private float warnTimer;
    private float warnTime;
    
    void Start()
    {
        emissionTimer = timeBetweenEmission;
        durationTimer = emissionDuration;
        warnTimer = whenToStartWarn;

        warnTime = emissionDuration - whenToStartWarn;
        emission = gameObject.GetComponentInChildren<Emission>();
    }

    void Update()
    {
        if (!emitting)
        {

            if (CheckEmissionTimerExpired())
            {
                emitting = true;
                emission.Emit();
            }
        }
        else
        {
            if (warning == false)
            {
                warnTimer -= Time.deltaTime;
                if (warnTimer < 0f)
                {
                    warning = true;
                    warnTimer = whenToStartWarn;
                    emission.Warn();
                }
            }
            if (CheckDurationTimerExpired())
            {
                emitting = false;
                warning = false;
                emission.DisableEmit();
            }
        }
    }

    private bool CheckDurationTimerExpired()
    {
        durationTimer -= Time.deltaTime;
        if (durationTimer < 0f)
        {
            durationTimer = emissionDuration;
            return true;
        }
        return false;
    }

    private bool CheckEmissionTimerExpired()
    {
        emissionTimer -= Time.deltaTime;
        if (emissionTimer < 0f)
        {
            emissionTimer = timeBetweenEmission;
            return true;
        }
        return false;
    }
}
