using Unity.Netcode;
using System;

[Serializable]
public class Debuff : INetworkSerializable
{
    public enum StatusEffect { None, Oil }
    public enum DamageType { Fire, Ice, Poison, Bleed, Void }

    public StatusEffect statusEffect;
    public DamageType damageType;
    public int amount;
    public int maxDuration;
    public int duration;
    public bool isDOT;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref statusEffect);
        serializer.SerializeValue(ref damageType);
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref maxDuration);
        serializer.SerializeValue(ref duration);
        serializer.SerializeValue(ref isDOT);
    }
}
 