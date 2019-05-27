using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunPowerup : BaseAttackPowerup{


    public Vector3 laserOriginOffset = Vector3.zero;
    public float laserDistance = 50f;

    public Transform carTransform;
    public LayerMask layerMask;
    public float blastDuration = 1f;
    public float laserForce = 20f;
    public float laserDamage = 40f;
    public GameObject laser;

    public MachineGunPowerup() {
        type = PowerupType.Laser;
        shotTimer = shotDuration;
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
        laser.SetActive(false);
         AttackPowerupExecutionOrder = null;
    }

    private void LaserPowerupExecution() {
        if (timer <= 0) {
            AttackPowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && cooldownTimer > coolDownDuration) {
            Debug.Log(timer);

            Debug.Log("Shoot");

            isShooting = true;
            shotTimer = 0;
        }

        Debug.Log(isShooting);

        if (isShooting) {
            LaserBlast();
            shotTimer += Time.deltaTime;
            if (shotTimer > shotDuration) {
                cooldownTimer = 0;
                isShooting = false;
            }

            laser.SetActive(true);
        }

        if (!isShooting) {
            cooldownTimer += Time.deltaTime;
            laser.SetActive(false);
        }
    }

    void LaserBlast() {
        RaycastHit hit;

        if (Physics.Raycast(carTransform.position + laserOriginOffset, carTransform.forward, out hit, laserDistance, layerMask)) {
            if (hit.transform.gameObject.GetComponent<Rigidbody>() != null) {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(carTransform.forward * laserForce, hit.point);
            }
            if (hit.transform.GetComponent<Health>() != null) {
                hit.transform.GetComponent<Health>().TakeDamage(laserDamage);
            }
        }
    }

}
