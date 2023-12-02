using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AddDebuff : NetworkBehaviour
{
    [SerializeField] protected List<Debuff> debuffs;
    // Start is called before the first frame update
    void Start()
    {
        int r = Mathf.RoundToInt(Random.value * 9f);
        for (int i = 0; i < r; i++)
        {
            Debuff debuff = new Debuff();
            debuff.amount = Mathf.RoundToInt(Random.value * 5f);
            debuff.duration = Mathf.RoundToInt(Random.value * 53f);
            debuff.maxDuration = debuff.duration;
            int damageTypeInt = Mathf.RoundToInt(Random.value * 4f);
            debuff.damageType = (Debuff.DamageType)damageTypeInt;
            int statusEffectInt = Mathf.RoundToInt(Random.value);
            debuff.statusEffect = (Debuff.StatusEffect)statusEffectInt;
            debuffs.Add(debuff);
        }
    }
   
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collission");
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            foreach (Debuff debuff in debuffs)
            {
                AddDebuffServerRpc(debuff);
            }
            
        }
    }
    [ServerRpc]
    private void AddDebuffServerRpc(Debuff debuff)
    {
    }    
}
