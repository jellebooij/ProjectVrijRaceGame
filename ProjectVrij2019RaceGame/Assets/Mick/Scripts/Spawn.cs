using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public Transform[] spawns;
    public GameObject[] isOccupied;
    public float beginTime = 10;
    public float timer;
    public float percentageFallOff = .1f;
    public GameObject[] objectsToSpawn;

    public int spawnIndex;
    // Start is called before the first frame update
    void Start()
    {
        isOccupied = new GameObject[spawns.Length];
        spawnIndex = Random.Range(0, spawns.Length - 1);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isOccupied.Length);

        timer += Time.deltaTime;

        if (timer > beginTime) {


            if (isOccupied[spawnIndex] == null) {
                isOccupied[spawnIndex] = (GameObject)Instantiate(objectToSpawn[Random.Range(0, objectToSpawn.Length)], spawns[spawnIndex].position, Quaternion.identity);
                beginTime = beginTime - (beginTime / 10) * percentageFallOff;
                Debug.Log(beginTime);

                spawnIndex = Random.Range(0, spawns.Length);
                timer = 0;
            } else {
                spawnIndex = Random.Range(0, spawns.Length);
                timer = 0;
            }
        }

    }
}
