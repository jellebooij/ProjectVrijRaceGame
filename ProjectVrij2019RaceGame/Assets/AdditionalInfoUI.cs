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

    [SerializeField]
    private TextMeshProUGUI infoText;

    public PlayerStateHandler playerStateHandler;
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

        if (playerStateHandler.type == PlayerState.Spectating) {
            infoText.gameObject.SetActive(true);
            infoText.text = "YOU ARE CURRENTLY SPECTATING!";
        } else {
            infoText.gameObject.SetActive(false);
        }
    }

    private int CheckForPlayers() {
        int aliveAmount = 1;
        foreach (Transform child in otherCarsTransform) {
             if (child.tag == "Pod") {
                 aliveAmount += 1;
             }
        }

        return aliveAmount;
    }
}
