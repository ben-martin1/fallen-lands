using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Damageable : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<float> maxHealth;
    [SerializeField] private NetworkVariable<float> currentHealth;
    private float millisecond;
    [SerializeField] private NetworkVariable<int> second;
    private Rigidbody rb;

    [SerializeField] protected List<Debuff> activeDebuffs;
    protected enum StatusEffect { None, Oil }
    protected enum DamageType { Fire, Ice, Poison, Bleed, Void }

    [SerializeField] protected List<StatusEffect> statusEffects;

    [SerializeField] private Transform destroyed;
    [SerializeField] private bool spawnDestroyed;
    [SerializeField] private bool destroyOnDeath;
    [SerializeField] private float destroyedDespawnTime;

    [SerializeField] private NetworkVariable<bool> isEnemy;

    [SerializeField] private EnemyAI enemyAI;

    void Start()
    {
        if (isEnemy.Value == true) enemyAI = GetComponent<EnemyAI>();
        second.Value = 0;
        rb = GetComponent<Rigidbody>();
        currentHealth.Value = maxHealth.Value;
    }
    public override void OnNetworkSpawn()
    {
        currentHealth.Value = maxHealth.Value;
    }
    void Update()
    {
        millisecond += Time.deltaTime;
        if (millisecond >= 1) OnSecondTick();
    }
    protected virtual void OnSecondTick()
    {
        millisecond = 0;
        ApplyTickDebuffsServerRpc();
    }

    [ServerRpc]
    private void ApplyTickDebuffsServerRpc()
    {
        foreach (Debuff activeDebuff in activeDebuffs)
        {
            int damage = activeDebuff.amount;
            int duration = activeDebuff.duration;
            if (statusEffects.Contains((StatusEffect)activeDebuff.statusEffect))
            {
                switch (activeDebuff.statusEffect)
                {
                    case Debuff.StatusEffect.None:
                        break;
                    case Debuff.StatusEffect.Oil:
                        Debug.Log("Has Oil Status Effect");
                        if (activeDebuff.damageType == Debuff.DamageType.Fire) damage *= 2;
                        break;
                    default:
                        break;
                }
            }
            currentHealth.Value -= damage;
            activeDebuff.duration--;
            if (currentHealth.Value <= 0) OnDeath();
            if (activeDebuff.duration <= 0)
            {
                activeDebuffs.Remove(activeDebuff);
                statusEffects.Remove((StatusEffect)activeDebuff.statusEffect);
            }
        }
    }
    [ServerRpc]
    public void ApplyOneShotDebuffServerRpc(Debuff debuff)
    {
        switch (debuff.damageType)
        {
            case Debuff.DamageType.Fire:
                if (statusEffects.Contains(StatusEffect.Oil)) currentHealth.Value -= debuff.amount * 2f;
                Debug.Log("Double damaged- Oil x Fire");
                break;
            case Debuff.DamageType.Ice:
                break;
            case Debuff.DamageType.Poison:
                break;
            case Debuff.DamageType.Bleed:
                break;
            case Debuff.DamageType.Void:
                break;
            default:
                break;
        }
    }
    [ServerRpc]
    public void AddActiveDebuffsServerRpc(Debuff[] attackerDebuffs)
    {
        foreach (Debuff newDebuff in attackerDebuffs)
        {
            if (!statusEffects.Contains((StatusEffect)newDebuff.statusEffect) || newDebuff.statusEffect == 0)
            {
                statusEffects.Add((StatusEffect)newDebuff.statusEffect);
            }
            if (newDebuff.isDOT == false)
            {
                ApplyOneShotDebuffServerRpc(newDebuff);
                return;
            }
            if (activeDebuffs.Contains(newDebuff))
            {
                if (newDebuff.duration > activeDebuffs[activeDebuffs.IndexOf(newDebuff)].duration)
                {
                    activeDebuffs[activeDebuffs.IndexOf(newDebuff)].duration = newDebuff.duration;
                }
            }
            else if (!activeDebuffs.Contains(newDebuff))
            {
                activeDebuffs.Add(newDebuff);
            }
        }
    }
    public void OnHit(ulong shooterID, float damage, float force, Vector3 hit)
    {
        currentHealth.Value -= damage;
        if (currentHealth.Value <= 0) OnDeath();
        if (rb != null) rb.AddForce(hit * force);
        //healthBar.SetProgress(currentHealth.Value / maxHealth, 3);
        LogShooter(shooterID);
    }
    protected void LogShooter(ulong shooterID)
    {
        if (enemyAI != null) enemyAI.LogShooter(shooterID);
    }
    protected void OnDeath()
    {
        if (spawnDestroyed)
        {
            Transform spawnDestroyedGO = Instantiate(destroyed, transform.position, transform.rotation);
            spawnDestroyedGO.GetComponent<NetworkObject>().Spawn(true);
            Destroy(spawnDestroyedGO.gameObject, destroyedDespawnTime);
        }
        if (destroyOnDeath)
        {
            Destroy(gameObject);
            gameObject.GetComponent<NetworkObject>().Spawn(false);
        }
        if (enemyAI != null) enemyAI.OnDeath(); 
    }
    [ServerRpc]
    public void OnHitServerRpc(ulong shooterID, float damage, float force, Vector3 hit)
    {
        OnHit(shooterID, damage, force, hit);
    }

}
