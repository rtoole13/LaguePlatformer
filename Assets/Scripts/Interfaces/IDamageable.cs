using UnityEngine;
public interface IDamageable
{
    void TakeDamage(int damage, bool isExplosive);
    void AddDamageOverTime(GameObject source, int damagePerTick, int timeBetweenTicks, int ticks); //Worried about a source not being a gameObject here
    void AddDamageOverTime(GameObject source, int damagePerTick, int timeBetweenTicks, bool indefinite); //Worried about a source not being a gameObject here
    void RemoveDamageOverTimeBySource(GameObject gameObject);
}
