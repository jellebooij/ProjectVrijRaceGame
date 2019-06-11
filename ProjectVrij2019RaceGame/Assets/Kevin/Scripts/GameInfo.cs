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
    private float fadeSpeed = 1.0f;

    private void Start() {
        infoText.gameObject.SetActive(false);
        infoText.text = "Starting game in... " + Mathf.FloorToInt(countdown).ToString();

    }

    private void Update() {
        //if (Input.GetKeyDown(KeyCode.E)) {
        //    EliminatePlayer();
        //}

        StartTimer();
    }

    public void BeginTimer() {
        infoText.gameObject.SetActive(true);
        countdown = 5;
        startCountdown = true;
        StopCoroutine(StartGame());
    }

    public void StartTimer() {

        if(startCountdown) {
            countdown -= Time.deltaTime;

            infoText.text = "Starting game in... " + Mathf.FloorToInt(countdown).ToString();

            if (countdown < 1) {
                StartCoroutine(StartGame());
            }
        }
    }

    public IEnumerator StartGame() {
        countdown = 0;
        infoText.text = "Begin!";
        yield return new WaitForSeconds(1f);
        infoText.gameObject.SetActive(false);
        startCountdown = false;
    }

    public IEnumerator EliminatePlayer() {
        infoText.text = "YOU HAVE ELIMINATED A PLAYER!";
        infoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        infoText.gameObject.SetActive(false);
    }

    public IEnumerator PlayerDefeated() {
        infoText.text = "YOU HAVE BEEN DEFEATED!";
        infoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        infoText.gameObject.SetActive(false);
    }
}
