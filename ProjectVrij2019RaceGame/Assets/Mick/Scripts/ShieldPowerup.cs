using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : BasePowerup{


    public GameObject shield;
    public Health health;
    public float shieldPoints;

    public ShieldPowerup() {
        type = PowerupType.Shield;
    }

    public override void StartPowerup() {
        timer = duration;
        Debug.Log("Started");
        PowerupExecutionOrder = ExcecutePowerup;
        shieldPoints = health.health; 
    }
    public override void ExcecutePowerup() {

        Debug.Log("Executing");
        ShieldPowerupExecution();
    }
    public override void StopPowerup() {
         PowerupExecutionOrder = null;
    }

    private void ShieldPowerupExecution() {
        if (timer <= 0) {
            PowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;

        health.health = shieldPoints;
    }

}
