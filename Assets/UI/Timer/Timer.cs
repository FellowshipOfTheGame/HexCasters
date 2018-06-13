using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Text timerIndicator;
	public static Timer T;

	private float startTime;
	public float turnTime;

	void Start () {
		T = this;
		startTime = Time.time;
		turnTime = 10f;
	}

	void Update () {
		if (GameManager.GM.state == GameManager.GameState.RESULTS) {
			return;
		}
		float t = Time.time - startTime; //growing count
		float secondsLeft = (turnTime - (t % 60));
		timerIndicator.text = secondsLeft.ToString("f0");
		if ((int)secondsLeft == 0) {
			GameManager.GM.EndTurn();
		}
	}

	public void ResetCountdownTimer() {
		startTime = Time.time;
		if (GameManager.GM.state == GameManager.GameState.RESULTS) {
			timerIndicator.text = "";
		}
		else {
			GameManager.GM.state = GameManager.GameState.OVERVIEW;
		}
	}

}
