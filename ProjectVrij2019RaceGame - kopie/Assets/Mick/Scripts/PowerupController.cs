using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour {
    public Health healthReference;
    public BaseAttackPowerup currentPowerup;
    LaserPowerup laser;
    MachineGunPowerup machineGun;
    HomingMissilePowerup homingMissilePowerup;
    NoPowerup none;
    public float laserDuration = 10;
    public float laserDistance = 50f;
    public Vector3 laserOriginOffset = Vector3.zero;

    public LayerMask layerMask;

    public float shield;
    public GameObject laserFx;

    public GameObject bulletPrefab;
    public float machineGunDuration = 5f;
    public float machineGunStrayFactor = 20f;
    public float machineGunCooldown = 0.05f;

    public List<GameObject> enemyList;
    public float homingMissilePowerupDuration = 10f;
    public GameObject homingMissilePrefab;
    public Camera cam;
    public LayerMask setLayerMask;
    public float missileLockTime = 1f;


    private void Awake() {
        healthReference = GetComponent<Health>();
        //laser initialization

        laser = new LaserPowerup();
        machineGun = new MachineGunPowerup();
        homingMissilePowerup = new HomingMissilePowerup();
        none = new NoPowerup();

        laser.laser = laserFx;
        laser.layerMask = layerMask;
        laser.carTransform = transform;
        laser.duration = laserDuration;
        laser.laserDistance = laserDistance;
        laser.laserOriginOffset = laserOriginOffset;
        laser.blastDuration = 1f;

        //machinegun initialization

        machineGun.bulletPrefab = bulletPrefab;
        machineGun.duration = machineGunDuration;
        machineGun.carTransform = transform;
        machineGun.strayFactor = machineGunStrayFactor;
        machineGun.coolDownDuration = machineGunCooldown;

        //Homing missile initialization

        homingMissilePowerup.carTransform = transform;
        homingMissilePowerup.duration = homingMissilePowerupDuration;
        homingMissilePowerup.missilePrefab = homingMissilePrefab;
        homingMissilePowerup.cam = cam;
        homingMissilePowerup.layerMask = setLayerMask;
        homingMissilePowerup.lockTime = missileLockTime;

        currentPowerup = none;
        Debug.Log(currentPowerup.type);
    }

    private void Update() {
        //Debug.Log(currentPowerup.type);



        if (currentPowerup.AttackPowerupExecutionOrder != null) {
            currentPowerup.AttackPowerupExecutionOrder();
        } else {
            currentPowerup = none;
        }
        Debug.Log(currentPowerup.type);

            //if (currentpowerup.attackpowerupexecutionorder != null) {
            //    currentpowerup.attackpowerupexecutionorder();
            //}
            //if (currentpowerup.attackpowerupexecutionorder == null) {
            //    currentpowerup = null;
            //}

        }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Laser"){
            currentPowerup = laser;
            currentPowerup.StartPowerup();
            Debug.Log(currentPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "MachineGun") {
            currentPowerup = machineGun;
            currentPowerup.StartPowerup();
            Debug.Log(currentPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "HomingMissilePowerup") {
            currentPowerup = homingMissilePowerup;
            homingMissilePowerup.StartPowerup();
            Debug.Log(currentPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "Shield") {
            healthReference.armor = 100;
            Destroy(other.gameObject);
        }

        if (other.tag == "Health") {
            healthReference.health = 100;
            Destroy(other.gameObject);
        }
    }
}


public class NoPowerup : BaseAttackPowerup {


    public NoPowerup() {
        type = PowerupType.None;
    }

    public override void StartPowerup() {
    }
    public override void ExcecutePowerup() {

    }
    public override void StopPowerup() {
    }

}
