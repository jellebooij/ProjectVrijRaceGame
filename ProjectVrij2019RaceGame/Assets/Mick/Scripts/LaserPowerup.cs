using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPowerup : BasePowerup{


    public Vector3 laserOriginOffset = Vector3.zero;
    public float laserDistance = 50f;

    public LayerMask layerMask;
    public float blastDuration = 1f;
    public float laserForce = 1000f;
    public float laserDamage = 40f;
    public GameObject laser;

    public LaserPowerup() {
        type = PowerupType.Laser;
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
        laser.SetActive(false);
         PowerupExecutionOrder = null;
    }

    private void LaserPowerupExecution() {
        if (timer <= 0) {
            PowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && cooldownTimer > coolDownDuration) {
            Debug.Log(timer);

            Debug.Log("Shoot");
            LaserBlast();
            laser.SetActive(true);
            isShooting = true;
            cooldownTimer = 0;
        }
        cooldownTimer += Time.deltaTime;
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
