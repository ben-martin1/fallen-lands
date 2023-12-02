using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public abstract class ITimeManager : NetworkBehaviour
{
    protected float millisecond;
    [SerializeField] protected NetworkVariable<int> second;
    [SerializeField] protected NetworkVariable<int> minute;
    [SerializeField] protected NetworkVariable<int> hour;

    void Start()
    {
        second.Value = 0;
        minute.Value = 0;
        hour.Value = 0;
    }

    // Update is called once per frame
    void Update()
    {
        millisecond += Time.deltaTime;
        if (millisecond >= 1) OnSecondTick();
    }
    public virtual void OnSecondTick()
    {
        millisecond = 0f;
        second.Value++;
        Debug.Log("New second: " + second);
    }
}
