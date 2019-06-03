using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissilePowerup : BasePowerup{


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
    public Vector2 missileLockRectangleFractional;
    public float lockRectangleFractionalHeight = 0.2f;
    public float shootTimePeriod = 0.5f;
    private float currentShootTime;
    private float currentPartialShootTimePeriod;
    private int amountOfShots;

    public HomingMissilePowerup() {
        type = PowerupType.HomingMissile;
    }

    public override void StartPowerup() {
        timer = duration;
        Debug.Log("Started");
        PowerupExecutionOrder = ExcecutePowerup;
        currentShootTime = 0;
    }
    public override void ExcecutePowerup() {

        enemiesAlive = GetPlayerList();
        Debug.Log("Executing");
        HomingMissilePowerupExecution();
    }
    public override void StopPowerup() {
         PowerupExecutionOrder = null;
    }

    private void HomingMissilePowerupExecution() {
        if (timer <= 0) {
            PowerupExecutionOrder = StopPowerup;
        }
        timer -= Time.deltaTime;

        for (int i = 0; i < timeBeforeIsLocked.Count; i++) {
            if (timeBeforeIsLocked[i] > 0) {
                timeBeforeIsLocked[i] -= Time.deltaTime;
            } else if (timeBeforeIsLocked[i] <= 0) {
                if (!enemiesLocked.Contains(enemiesLocking[i])) {
                    enemiesLocked.Add(enemiesLocking[i]);
                }
            }
        }

        if (Input.GetButtonDown("Fire1")) {
            Debug.Log("Shooting");
            currentPartialShootTimePeriod = shootTimePeriod / enemiesLocked.Count;
            amountOfShots = enemiesLocked.Count;
            currentShootTime = shootTimePeriod;

        } else {
            DetectenemiesInViewFrustrum();
        }

        if (currentShootTime >= 0) {
            currentShootTime -= Time.deltaTime;
            for (int i = 0; i < enemiesLocked.Count; i++) {
                if (currentShootTime < (currentPartialShootTimePeriod * i) && amountOfShots == (i + 1)) {
                    ShootMissile(enemiesLocked[i].transform);
                    enemiesLocked.Remove(enemiesLocked[i]);
                    amountOfShots--;
                }
            }

            if (currentShootTime <= 0) {
                enemiesLocking.Clear();
                enemiesLocked.Clear();
                timeBeforeIsLocked.Clear();
            }
        }

        Debug.Log(timeBeforeIsLocked[0]);
    }


    void DetectenemiesInViewFrustrum() {
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
        for (int i = 0; i < enemiesAlive.Count; i++) {
            if (GeometryUtility.TestPlanesAABB(planes, enemiesAlive[i].GetComponent<Collider>().bounds)) {

                RaycastHit hit;
                bool isHit = Physics.Raycast(cam.transform.position, enemiesAlive[i].transform.position - cam.transform.position, Mathf.Infinity, layerMask);
                //all xonditions met so enemies can be locked onto
                if (!enemiesLocking.Contains(enemiesAlive[i]) && isHit && isObjectInPartOfScreen(enemiesAlive[i].transform.position)) {
                    enemiesLocking.Add(enemiesAlive[i]);
                    timeBeforeIsLocked.Add(lockTime);
                }
            } else {
                if (enemiesLocking.Contains(enemiesAlive[i])) {
                    enemiesLocking.Remove(enemiesAlive[i]);
                    timeBeforeIsLocked.RemoveAt(enemiesLocking.IndexOf(enemiesAlive[i]));
                }
            }
        }
    }

    private bool isObjectInPartOfScreen(Vector3 worldPosition) {
        float widthPixels = Screen.width;
        float heightPixels = Screen.height;
        Debug.Log(widthPixels);
        Vector3 toWorldPosition = cam.WorldToScreenPoint(worldPosition);
        bool isDetected = false;
        if (toWorldPosition.x > (widthPixels - missileLockRectangleFractional.x * widthPixels) / 2 && toWorldPosition.x < (widthPixels - (widthPixels - missileLockRectangleFractional.x * widthPixels) / 2)) {
           if (toWorldPosition.y > ((heightPixels - missileLockRectangleFractional.y * heightPixels) / 2 + lockRectangleFractionalHeight * heightPixels) && 
                toWorldPosition.y < (heightPixels - (heightPixels - missileLockRectangleFractional.y * heightPixels) / 2 + lockRectangleFractionalHeight * heightPixels)) {
                isDetected = true;
            }

        } else {
            isDetected = false;
        }
        return isDetected;
    }


    void ShootMissile(Transform target) {
        GameObject missile = Instantiate(missilePrefab, carTransform.position + homingMissileOriginOffset, carTransform.rotation);
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
