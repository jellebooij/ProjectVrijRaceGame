using UnityEngine;

public class PowerupActions : MonoBehaviour {
    public CarController carController;
    public Health health;

    public delegate void AttackPowerupAction();
    [SerializeField]
    public AttackPowerupAction AttackPowerup;

    public delegate void DefensePowerupAction();
    [SerializeField]
    public DefensePowerupAction DefensePowerup;
    public Vector3 laserOriginOffset;
    public float laserDistance;
    public LayerMask layerMask;
    public float shieldHealthRegeneration = 10;

    private void Start() {
        carController = GetComponent<CarController>();
        health = GetComponent<Health>();
    }

    private void Update() {
        if (AttackPowerup != null) {
            AttackPowerup();
        }

        if (DefensePowerup != null) {
            DefensePowerup();
        }
    }

    //Laser

    public void LaserStartAction() {
        carController.currentSpeed *= 2;

        AttackPowerup = LaserUpdateAction;


    }


    public void LaserUpdateAction() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("Shoot");
            RaycastHit hit;
            if (Physics.Raycast(transform.position + laserOriginOffset, transform.forward,out hit , laserDistance, layerMask)) {
                Debug.Log(hit.transform.gameObject);
                if (hit.transform.gameObject.GetComponent<Rigidbody>() != null) {
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * 1000, hit.point);
                }
            }
        }
    }

    public void LaserEndAction() {
        Debug.Log("Ended");
        carController.currentSpeed /= 2;

        AttackPowerup = null;
    }




    public void ShieldStartAction() {

        DefensePowerup = ShieldUpdateAction;
        health.maxHealth = 200;
        health.health = 200;
    }


    public void ShieldUpdateAction() {
        Debug.Log("Shield");
        health.health += Time.deltaTime * shieldHealthRegeneration;
    }

    public void ShieldEndAction() {
        Debug.Log("Ended");
        if (health.health >= 100) {
            health.maxHealth = 100;
        }
        DefensePowerup = null;
    }

    private void OnDrawGizmos() {
        Debug.DrawLine(transform.position + laserOriginOffset, transform.forward * laserDistance, Color.cyan);
    }

}
