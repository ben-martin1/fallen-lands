using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(menuName = "Debuff/FireDebuff")]
public class FireDebuff : DebuffSO, INetworkSerializable
{
    public override void Apply(GameObject target)
    {
        
    }
    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        serializer.SerializeValue(ref amount);
        serializer.SerializeValue(ref duration);
        serializer.SerializeValue(ref maxDuration);

        serializer.SerializeValue(ref statusEffect);
        serializer.SerializeValue(ref damageType);
    }

}
