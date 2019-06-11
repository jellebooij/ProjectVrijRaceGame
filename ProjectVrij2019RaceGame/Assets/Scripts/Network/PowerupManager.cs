using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{

    public Dictionary<int, GameObject> currentPowerUps = new Dictionary<int, GameObject>();
    public Dictionary<GameObject, int> gameObjectMap = new Dictionary<GameObject, int>();

    public GameObject attackPowerup;

    public void AddPowerup(int id, Vector3 position)
    {
        
        GameObject current = Instantiate(attackPowerup, position, Quaternion.identity, transform) as GameObject;
        currentPowerUps.Add(id, current);
        gameObjectMap.Add(current, id);

    }

    public void Removepowerup(int id)
    {

        if (gameObjectMap.ContainsValue(id))
        {
            gameObjectMap.Remove(currentPowerUps[id]);
            Destroy(currentPowerUps[id]);
            currentPowerUps.Remove(id);
        }
        
    }

}
