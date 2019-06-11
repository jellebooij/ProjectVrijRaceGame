using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupController : MonoBehaviour {
    public Health healthReference;
    public BasePowerup currentAttackPowerup;
    public BasePowerup currentDefensePowerup;
    LaserPowerup laser;
    MachineGunPowerup machineGun;
    HomingMissilePowerup homingMissilePowerup;
    ShieldPowerup shieldPowerup;
    public NoPowerup none;
    public float laserDuration = 10;
    public float laserDistance = 50f;
    public Vector3 laserOriginOffset = Vector3.zero;

    public LayerMask layerMask;

    public float shieldLength;
    public GameObject laserFx;

    public GameObject bulletPrefab;
    public float machineGunDuration = 5f;
    public float machineGunStrayFactor = 20f;
    public float MachineGunFireRate = 0.05f;
    public GameObject MachineGunFirePoint;

    public float homingMissilePowerupDuration = 10f;
    public Vector3 homingMissileOriginOffset = Vector3.zero;
    public GameObject homingMissilePrefab;
    public Camera cam;
    public float missileLockTime = 1f;
    public Vector2 missileLockCenterRectangleFractional = new Vector2(0.3f, 0.4f);
    public float lockRectangleFractionalHeight = 0.2f;

    public float shieldDuration = 5f;
    public GameObject shield;

    private void Awake() {
        healthReference = GetComponent<Health>();
        //laser initialization

        laser = new LaserPowerup();
        machineGun = new MachineGunPowerup();
        homingMissilePowerup = new HomingMissilePowerup();
        shieldPowerup = new ShieldPowerup();
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
        machineGun.coolDownDuration = MachineGunFireRate;
        machineGun.firePoint = MachineGunFirePoint;

        //Homing missile initialization

        homingMissilePowerup.carTransform = transform;
        homingMissilePowerup.duration = homingMissilePowerupDuration;
        homingMissilePowerup.missilePrefab = homingMissilePrefab;
        homingMissilePowerup.cam = cam;
        homingMissilePowerup.layerMask = layerMask;
        homingMissilePowerup.lockTime = missileLockTime;
        homingMissilePowerup.missileLockRectangleFractional = missileLockCenterRectangleFractional;
        homingMissilePowerup.homingMissileOriginOffset = homingMissileOriginOffset;
        homingMissilePowerup.lockRectangleFractionalHeight = lockRectangleFractionalHeight;
        homingMissilePowerup.thisGameObject = gameObject;

        shieldPowerup.health = healthReference;
        shieldPowerup.duration = shieldDuration;
        shieldPowerup.shieldGameObject = shield;
        shieldPowerup.thisGameObject = this.gameObject;

        currentAttackPowerup = none;
        currentDefensePowerup = none;
        Debug.Log(currentAttackPowerup.type);
    }

    private void Update() {
        //Debug.Log(currentPowerup.type);



        if (currentAttackPowerup.PowerupExecutionOrder != null) {
            currentAttackPowerup.PowerupExecutionOrder();
        } else {
            currentAttackPowerup = none;
        }

        if (currentDefensePowerup.PowerupExecutionOrder != null) {
            currentDefensePowerup.PowerupExecutionOrder();
        } else {
            currentDefensePowerup = none;
        }

        //if (currentpowerup.attackpowerupexecutionorder != null) {
        //    currentpowerup.attackpowerupexecutionorder();
        //}
        //if (currentpowerup.attackpowerupexecutionorder == null) {
        //    currentpowerup = null;
        //}

    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Laser"){
            currentAttackPowerup = laser;
            currentAttackPowerup.StartPowerup();
            Debug.Log(currentAttackPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "MachineGun") {
            currentAttackPowerup = machineGun;
            currentAttackPowerup.StartPowerup();
            Debug.Log(currentAttackPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "HomingMissilePowerup") {
            currentAttackPowerup = homingMissilePowerup;
            currentAttackPowerup.StartPowerup();
            Debug.Log(currentAttackPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "Shield") {
            currentDefensePowerup = shieldPowerup;
            currentDefensePowerup.StartPowerup();
            Debug.Log(currentAttackPowerup.type);
            Destroy(other.gameObject);
        }

        if (other.tag == "Health") {
            ClientBehaviour.instance.TakeDamage(transform.gameObject.GetComponent<NetworkPlayer>().id, -50);
            Destroy(other.gameObject);
        }
    }
}



public class NoPowerup : BasePowerup {


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
