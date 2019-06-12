using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AdditionalInfoUI : MonoBehaviour
{
    public Transform otherCarsTransform;
    public TextMeshProUGUI playersAliveCountText;
    private int playersAliveCount; 
    // Start is called before the first frame update
    void Start()
    {
        playersAliveCount = CheckForPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        playersAliveCount = CheckForPlayers();
        playersAliveCountText.text = playersAliveCount.ToString();
    }

    private int CheckForPlayers() {
        int aliveAmount = 0;
        foreach (Transform child in otherCarsTransform) {
             if (child.tag == "Pod") {
                 aliveAmount += 1;
             }
        }

        return aliveAmount;
    }
}
