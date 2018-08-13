using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Text timerIndicator;
	public static Timer T;

	private float startTime;
	private bool countingFiveSeconds;
	public float turnTime;
	public bool hasTimeLimit;

	void Awake() {
		if (T != null) {
			Destroy(gameObject);
			return;
		}
		T = this;
		hasTimeLimit = false;
		if (TimerMenu.isSelected) {
			turnTime = TimerMenu.turnTime;
		}
		if (turnTime > 0) {
			startTime = Time.time;
			hasTimeLimit = true;
			countingFiveSeconds = false;
		}
	}

	void Update () {
		if (!hasTimeLimit || GameManager.GM.state == GameManager.GameState.RESULTS) {
			return;
		}
		float t = Time.time - startTime; //growing count
		float secondsLeft = (turnTime - t);
		timerIndicator.text = secondsLeft.ToString("f0");
		if ((int)secondsLeft == 5 && !countingFiveSeconds) {
			AudioManager.AM.Play("Clock");
			countingFiveSeconds = true;
		}
		if ((int)secondsLeft == 0) {
			GameManager.GM.EndTurn();
		}
	}

	public void ResetCountdownTimer() {
		if (hasTimeLimit) {
			AudioManager.AM.Stop("Clock");
			countingFiveSeconds = false;
			startTime = Time.time;
			if (GameManager.GM.state == GameManager.GameState.RESULTS) {
				timerIndicator.text = "";
			}
			else {
				GameManager.GM.state = GameManager.GameState.OVERVIEW;
			}
		}
	}

}
