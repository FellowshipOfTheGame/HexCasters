using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HexUnit))]
public class HP : MonoBehaviour {
	public GameObject prefabHealthBar;
	public GameObject prefabDamageDisplay;
	public float healthBarHeight = 0.5f;
	public float damageDisplayHeight = 0;
	public float damageDisplayDuration = 1.0f;

	private GameObject healthBar;
	private GameObject healthpoint;
	private GameObject damageDisplay;
	private TMPro.TMP_Text damageDisplayText;
	private Transform healthBarFill;
	private Text hpText;

	public int max;
	private int? _current;
	public int? current {
		get {
			return _current;
		}
		set {
			var prev = _current;
			_current = (int) Mathf.Max(0, Mathf.Min((float) value, max));
			Vector3 scale = healthBarFill.localScale;
			scale.x = (float) _current / max;
			healthBarFill.localScale = scale;
			if (_current == 0) {
				OnReachZero();
			} else if (prev != null) {
				DisplayDamage(prev, current);
			}
		}
	}

	public event Action OnReachZero = delegate {};

	void Awake() {
		healthBar = Instantiate(prefabHealthBar, transform, false);
		damageDisplay = Instantiate(prefabDamageDisplay, transform, false);
		healthBarFill = healthBar.transform.Find("Fill");
		current = max;
		healthpoint = healthBar.transform.Find("Healthpoint").gameObject;
		Transform textTr = healthpoint.transform.Find("Text");
		hpText = textTr.GetComponent<Text>();
		textTr = damageDisplay.transform.Find("Text");
		damageDisplayText = textTr.GetComponent<TMPro.TMP_Text>();
		Vector3 pos = healthBar.transform.localPosition;
		pos.y = healthBarHeight;
		healthBar.transform.localPosition = pos;
		pos = damageDisplay.transform.localPosition;
		pos.y = damageDisplayHeight;
		damageDisplay.transform.localPosition = pos;

		damageDisplay.SetActive(false);
	}

	public void DisplayDamage(int? prev, int? cur) {
		if (cur < prev) {
			damageDisplayText.text = (cur - prev).ToString();
		} else {
			damageDisplayText.text = string.Format("+{0}", cur - prev);
		}
		damageDisplay.SetActive(true);
		// reset invoke timer
		CancelInvoke("DisableDamageDisplay");
		Invoke("DisableDamageDisplay", damageDisplayDuration);
	}

	private void DisableDamageDisplay(){
		damageDisplay.SetActive(false);
	}

	public void UpdateNumberDisplay() {
		hpText.text = string.Format("{0}/{1}", current, max);
	}

	public void EnableNumberDisplay() {
		healthpoint.SetActive(true);
	}

	public void DisableNumberDisplay() {
		healthpoint.SetActive(false);
	}
}
