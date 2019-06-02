using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePowerup: MonoBehaviour
{

    public delegate void PowerupAction();
    public PowerupAction PowerupExecutionOrder;

    public float timer = 0;
    public float duration;
    public float cooldownTimer = 0;
    public float coolDownDuration = 1f;

    public Transform carTransform;

    public bool isShooting;

    public PowerupType type { get; protected set; }
    public abstract void StartPowerup();
    public abstract void ExcecutePowerup();
    public abstract void StopPowerup();

}

public enum PowerupType { None, MachineGun, Laser, HomingMissile, Shield }