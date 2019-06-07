using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerup : BasePowerup{


    public GameObject shield;
    public Health health;
    public float shieldPoints;
    public GameObject shieldGameObject;
    public GameObject thisGameObject;

    public ShieldPowerup() {
        type = PowerupType.Shield;
    }

    public override void StartPowerup() {
        timer = duration;
        thisGameObject.GetComponent<ShieldSwitch>().EnableShield();
        ClientBehaviour.instance.ActivateShield();
        Debug.Log("Started");
        PowerupExecutionOrder = ExcecutePowerup;
        shieldPoints = health.health; 
    }
    public override void ExcecutePowerup() {

        Debug.Log("Executing");
        ShieldPowerupExecution();
    }
    public override void StopPowerup() {
        shieldGameObject.SetActive(false);
        PowerupExecutionOrder = null;
    }

    private void ShieldPowerupExecution() {
        if (timer <= 0) {
            PowerupExecutionOrder = StopPowerup;
        }

        shieldGameObject.SetActive(true);
        
        timer -= Time.deltaTime;

        health.health = shieldPoints;
    }

}
