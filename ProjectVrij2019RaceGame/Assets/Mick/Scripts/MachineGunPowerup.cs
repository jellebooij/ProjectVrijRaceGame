using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunPowerup : BasePowerup{


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
        PowerupExecutionOrder = ExcecutePowerup;
    }
    public override void ExcecutePowerup() {

        Debug.Log("Executing");
        LaserPowerupExecution();
    }
    public override void StopPowerup() {
         PowerupExecutionOrder = null;
    }

    private void LaserPowerupExecution() {
        if (timer <= 0) {
            PowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;

        cooldownTimer += Time.deltaTime;

        if ((Input.GetButton("Fire1") || (Input.GetAxisRaw("TriggerL") > 0)) && cooldownTimer > coolDownDuration) {
            Debug.Log("Shooting");
            ShootBullets();
        }
    }

    void ShootBullets() {
        var randomNumberX = Random.Range(-strayFactor, strayFactor);
        var randomNumberY = Random.Range(-strayFactor, strayFactor);
        var randomNumberZ = Random.Range(-strayFactor, strayFactor);

        Vector3 rotationOffset = Vector3.ClampMagnitude(new Vector3(randomNumberX, randomNumberY, randomNumberZ), strayFactor);

        Quaternion bulletRotation = firePoint.transform.rotation * Quaternion.Euler(rotationOffset);

        var bullet = Instantiate(bulletPrefab, firePoint.transform.position, bulletRotation);
        
        bullet.transform.Rotate(rotationOffset.x, rotationOffset.y, rotationOffset.z);
        ClientBehaviour.instance.FireMachineGun(firePoint.transform.position, bulletRotation);

        cooldownTimer = 0;
    }

}
