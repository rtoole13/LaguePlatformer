using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField]
    protected int maxHealth = 1;

    protected int currentHealth;

    [SerializeField]
    private UnityEvent hasDied = new UnityEvent();
    private List<DamageOverTime> activeDOTs = new List<DamageOverTime>();

    void Start()
    {
        currentHealth = maxHealth;
    }
    void Update()
    {
        TakeDamageFromDOTs();
    }

    private void TakeDamageFromDOTs()
    {
        for (int i = activeDOTs.Count - 1; i >= 0; i--)
        {
            DamageOverTime DOT = activeDOTs[i];
            if (DOT.CheckTime())
            {
                Debug.Log("damge");
                TakeDamage(DOT.damagePerTick, false);
            }
            if (!DOT.isActive)
                activeDOTs.RemoveAt(i);
        }
    }

    public virtual void TakeDamage(int damage, bool isExplosive)
    {
        currentHealth -= damage;
        if (currentHealth < 1) {
            hasDied?.Invoke();
            currentHealth = maxHealth;
        }
    }

    public void AddDamageOverTime(GameObject source, int damagePerTick, int timeBetweenTicks, int ticks) //Worried about a source not being a gameObject here
    {
        DamageOverTime DOT = new DamageOverTime(source, damagePerTick, timeBetweenTicks, ticks, false);
        activeDOTs.Add(DOT);
    }
    public void AddDamageOverTime(GameObject source, int damagePerTick, int timeBetweenTicks, bool indefinite) //Worried about a source not being a gameObject here
    {
        DamageOverTime DOT = new DamageOverTime(source, damagePerTick, timeBetweenTicks, 1, true);
        activeDOTs.Add(DOT);
    }

    public void RemoveDamageOverTimeBySource(GameObject gameObject)
    {
        for (int i = activeDOTs.Count - 1; i >= 0; i--)
        {
            DamageOverTime DOT = activeDOTs[i];
            if (DOT.source == gameObject)
            {
                activeDOTs.Remove(DOT);
            }

        }
    }
}

public class DamageOverTime
{
    public GameObject source;
    public int damagePerTick;
    public int timeBetweenTicks;
    public int ticks;
    public int ticksRemaining;
    public bool isActive = true;
    private bool indefinite;
    private float tickTime;

    public DamageOverTime(GameObject _source, int _damagePerTick, int _timeBetweenTicks, int _ticks, bool _indefinite) //Construction
    {
        source = _source;
        damagePerTick = _damagePerTick;
        timeBetweenTicks = _timeBetweenTicks;
        ticks = _ticks;
        ticksRemaining = ticks;
        tickTime = Time.time;
        indefinite = _indefinite;
    }
    public bool CheckTimeIndefinite()
    {
        float newTime = Time.time;
        if (newTime - tickTime >= timeBetweenTicks)
        {
            Debug.Log(newTime + " vs " + tickTime);
            tickTime = newTime;
            return true;
        }
        return false;
    }

    public bool CheckTimeFinite()
    {
        float newTime = Time.time;
        if (newTime - tickTime >= timeBetweenTicks)
        {
            ticksRemaining -= 1;
            if (ticksRemaining <= 0)
            {
                isActive = false;
            }

            tickTime = newTime;
            return true;
        }
        return false;
    }

    public bool CheckTime()
    {
        if (indefinite)
        {
            return CheckTimeIndefinite();
        }
        else
        {
            return CheckTimeFinite();
        }
    }
}
