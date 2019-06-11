using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{

    private Dictionary<int, GameObject> currentPowerUps = new Dictionary<int, GameObject>();
    public GameObject attackPowerup;

    public void AddPowerup(int id, Vector3 position)
    {
        
        GameObject current = Instantiate(attackPowerup, position, Quaternion.identity, transform) as GameObject;
        currentPowerUps.Add(id, current);
    }

    public void Removepowerup(int id)
    {
        Destroy(currentPowerUps[id]);
        currentPowerUps.Remove(id);
    }

}
