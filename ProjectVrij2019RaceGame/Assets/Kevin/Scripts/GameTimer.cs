using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour {

    public float gameDuration = 1f; // 5 minutes

    [SerializeField]
    private TextMeshProUGUI timerText;

    private void Update() {
        gameDuration += Time.deltaTime;
        UpdateLevelTimer(gameDuration);

        if (gameDuration < 0) {
            gameDuration = 0;
            timerText.text = "00:00";
        }
    }

    public void UpdateLevelTimer(float totalSeconds) {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.RoundToInt(totalSeconds % 60f);

        string formatedSeconds = seconds.ToString();

        if (seconds == 60) {
            seconds = 0;
            minutes += 1;
        }

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
