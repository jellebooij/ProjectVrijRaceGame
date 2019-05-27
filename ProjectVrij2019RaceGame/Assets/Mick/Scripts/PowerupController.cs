using UnityEngine;

public class PowerupController : MonoBehaviour {
    public Health healthReference;
    BaseAttackPowerup currentPowerup;
    LaserPowerup laser;
    NoPowerup none;
    public float laserDuration = 10;
    public float laserDistance = 50f;
    public Vector3 laserOriginOffset = Vector3.zero;

    public LayerMask layerMask;

    public float shield;
    public GameObject laserFx;


    private void Awake() {
        healthReference = GetComponent<Health>();
        //laser initialization

        laser = new LaserPowerup();
        none = new NoPowerup();

        laser.laser = laserFx;
        laser.layerMask = layerMask;
        laser.carTransform = transform;
        laser.duration = laserDuration;
        laser.laserDistance = laserDistance;
        laser.laserOriginOffset = laserOriginOffset;
        laser.blastDuration = 1f;

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
