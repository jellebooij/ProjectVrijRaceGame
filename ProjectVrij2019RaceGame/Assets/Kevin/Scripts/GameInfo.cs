using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameInfo : MonoBehaviour {

    public float countdown = 5f;
    private bool startCountdown = false;

    [SerializeField]
    private TextMeshProUGUI infoText;
    [SerializeField]
    private float fadeSpeed = 0.05f;

    private void Start() {
        infoText.CrossFadeAlpha(1, fadeSpeed, false);
        infoText.text = "Starting game in... " + Mathf.FloorToInt(countdown).ToString();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            startCountdown = true;
        }

        StartTimer();
    }

    public void StartTimer() {

        if(startCountdown) {
            countdown -= Time.deltaTime;
        }

        infoText.text = "Starting game in... " + Mathf.FloorToInt(countdown).ToString();

        if (countdown < 1) {
            StartCoroutine(StartGame());
        }
    }

    public IEnumerator StartGame() {
        countdown = 0;
        infoText.text = "Begin!";
        yield return new WaitForSeconds(1f);
        infoText.CrossFadeAlpha(0, fadeSpeed, false);
    }
}
