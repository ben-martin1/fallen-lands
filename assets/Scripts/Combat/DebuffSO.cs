using UnityEngine;
using Unity.Netcode;


public abstract class DebuffSO : ScriptableObject, INetworkSerializable
{
    public enum StatusEffect { None, Oil}
    public enum DamageType { Fire, Ice, Poison, Bleed, Void }

    public StatusEffect statusEffect;
    public DamageType damageType;
    public int amount;
    public int maxDuration;
    public int duration;

    public abstract void Apply(GameObject target);
    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
    public DebuffSO()
    {
    }
}
