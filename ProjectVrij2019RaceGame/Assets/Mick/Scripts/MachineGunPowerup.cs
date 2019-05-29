using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunPowerup : BaseAttackPowerup{


    public Vector3 machineGunOriginOffset = Vector3.zero;
    public float shootDistance = 10f;
    public float strayFactor = 10f;
    public GameObject firePoint;
    
    public GameObject bulletPrefab;

    public MachineGunPowerup() {
        type = PowerupType.MachineGun;
    }

    public override void StartPowerup() {
        timer = duration;
        Debug.Log("Started");
        AttackPowerupExecutionOrder = ExcecutePowerup;
    }
    public override void ExcecutePowerup() {

        Debug.Log("Executing");
        LaserPowerupExecution();
    }
    public override void StopPowerup() {
         AttackPowerupExecutionOrder = null;
    }

    private void LaserPowerupExecution() {
        if (timer <= 0) {
            AttackPowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;

        cooldownTimer += Time.deltaTime;

        if (Input.GetButton("Fire1") && cooldownTimer > coolDownDuration) {
            Debug.Log("Shooting");
            ShootBullets();
        }
    }

    void ShootBullets() {
        var randomNumberX = Random.Range(-strayFactor, strayFactor);
        var randomNumberY = Random.Range(-strayFactor, strayFactor);
        var randomNumberZ = Random.Range(-strayFactor, strayFactor);
        var bullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        Vector3 rotationOffset = Vector3.ClampMagnitude(new Vector3(randomNumberX, randomNumberY, randomNumberZ), strayFactor);
        bullet.transform.Rotate(rotationOffset.x, rotationOffset.y, rotationOffset.z);
        cooldownTimer = 0;
    }

}
