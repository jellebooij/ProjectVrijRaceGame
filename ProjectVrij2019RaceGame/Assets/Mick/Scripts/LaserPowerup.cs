using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPowerup : BaseAttackPowerup{


    public Vector3 laserOriginOffset = Vector3.zero;
    public float laserDistance = 50f;

    public Transform carTransform;
    public LayerMask layerMask;
    public float blastDuration = 1f;

    public LaserPowerup() {
        type = PowerupType.Laser;
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

        if (Input.GetButtonDown("Fire1")) {
            Debug.Log(timer);

            Debug.Log("Shoot");

            //need to fix this coroutine shit or add timer manually


            //IEnumerator laserBlast = LaserBlast();
            //StartCoroutine(laserBlast);
        }
    }

    IEnumerator LaserBlast() {
        RaycastHit hit;

        if (Physics.Raycast(carTransform.position + laserOriginOffset, carTransform.forward, out hit, laserDistance, layerMask)) {
            if (hit.transform.gameObject.GetComponent<Rigidbody>() != null) {
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(carTransform.forward * 1000, hit.point);
            }
            if (hit.transform.GetComponent<Health>() != null) {
                hit.transform.GetComponent<Health>().GetDamage(2);
            }
        }
        yield return new WaitForSeconds(blastDuration);
    }

}
