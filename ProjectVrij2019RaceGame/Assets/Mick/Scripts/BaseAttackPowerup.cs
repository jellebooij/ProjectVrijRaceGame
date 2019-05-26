using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAttackPowerup: MonoBehaviour
{

    public delegate void AttackPowerupAction();
    public AttackPowerupAction AttackPowerupExecutionOrder;

    public float timer = 0; 
    public float duration;

    public PowerupType type { get; protected set; }
    public abstract void StartPowerup();
    public abstract void ExcecutePowerup();
    public abstract void StopPowerup();

}

public enum PowerupType { None, Shield, Laser }