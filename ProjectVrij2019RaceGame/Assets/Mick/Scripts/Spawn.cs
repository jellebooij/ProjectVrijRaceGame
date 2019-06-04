using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    public Transform[] spawns;
    public GameObject[] isOccupied;
    public float durationBeforeRespawn = 10;
    public float[] timer;
    public float percentageFallOff = .1f;
    public bool[] timerReset;
    public GameObject[] objectsToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        isOccupied = new GameObject[spawns.Length];
        timer = new float[spawns.Length];
        timerReset = new bool[spawns.Length];
        for (int i = 0; i < timerReset.Length; i++) {
            timerReset[i] = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isOccupied.Length);
        

        for (int i = 0; i < timer.Length; i++) {
            timer[i] += Time.deltaTime;

            if (isOccupied[i] == null && !timerReset[i]) {
                timer[i] = 0;
                timerReset[i] = true;
            }

            if (timer[i] > durationBeforeRespawn) {

                if (isOccupied[i] == null) {
                    isOccupied[i] = (GameObject)Instantiate(objectsToSpawn[Random.Range(0, objectsToSpawn.Length)], spawns[i].position, Quaternion.identity);
                    durationBeforeRespawn = durationBeforeRespawn - (durationBeforeRespawn / 10) * percentageFallOff;
                    Debug.Log(durationBeforeRespawn);
                    timerReset[i] = false;
                    timer[i] = 0;
                } else {
                    timer[i] = 0; 
                    return;
                }
            }
        }

        

    }
}
