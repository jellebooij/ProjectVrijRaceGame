using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissilePowerup : BaseAttackPowerup{


    public Vector3 homingMissileOriginOffset = Vector3.zero;
    public Camera cam;
    private Plane[] planes;
    private Collider objectCollider;
    public LayerMask layerMask;

    public GameObject missilePrefab;
    public List<GameObject> enemiesAlive;
    public List<GameObject> enemiesLocking = new List<GameObject>();
    public List<GameObject> enemiesLocked = new List<GameObject>();
    public List<float> timeBeforeIsLocked = new List<float>();
    public float lockTime;
    public Vector2 missileLockRectangle;

    public HomingMissilePowerup() {
        type = PowerupType.HomingMissile;
    }

    public override void StartPowerup() {
        timer = duration;
        Debug.Log("Started");
        AttackPowerupExecutionOrder = ExcecutePowerup;
    }
    public override void ExcecutePowerup() {

        enemiesAlive = GetPlayerList();
        Debug.Log("Executing");
        HomingMissilePowerupExecution();
    }
    public override void StopPowerup() {
         AttackPowerupExecutionOrder = null;
    }

    private void HomingMissilePowerupExecution() {
        if (timer <= 0) {
            AttackPowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;
        if (Input.GetButtonDown("Fire1")) {
            Debug.Log("Shooting");
            for (int i = 0; i < enemiesLocking.Count; i++) {
                ShootMissile(enemiesLocking[i].transform);
            }
            enemiesLocking.Clear();

        } else {
            DetectenemiesInViewFrustrum();
        }
    }


    void DetectenemiesInViewFrustrum() {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        for (int i = 0; i < enemiesAlive.Count; i++) {
            if (GeometryUtility.TestPlanesAABB(planes, enemiesAlive[i].GetComponent<Collider>().bounds)) {

                RaycastHit hit;
                bool isHit = Physics.Raycast(cam.transform.position, enemiesAlive[i].transform.position - cam.transform.position, Mathf.Infinity, layerMask);
                if (!enemiesLocking.Contains(enemiesAlive[i]) && isHit) {
                    enemiesLocking.Add(enemiesAlive[i]);
                }
            } else {
                if (enemiesLocking.Contains(enemiesAlive[i])) {
                    enemiesLocking.Remove(enemiesAlive[i]);
                }
            }
        }
    }


    void ShootMissile(Transform target) {
        GameObject missile = Instantiate(missilePrefab, carTransform.position + homingMissileOriginOffset, Quaternion.identity);
        missile.GetComponent<HomingMissile>().homingTransform = target;
        missile.GetComponent<HomingMissile>().missileOwner = carTransform.gameObject;
    }

    private List<GameObject> GetPlayerList() {
        GameObject[] allPods = GameObject.FindGameObjectsWithTag("Pod");
        List<GameObject> pods = null;
        pods = new List<GameObject>(allPods);
        pods.Remove(carTransform.gameObject);

        Debug.Log("First Item: " + pods[0].name);
        return pods;
    }

}
