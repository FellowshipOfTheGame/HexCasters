using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HexUnit))]
public class HP : MonoBehaviour {
	public GameObject prefabHealthBar;
	public float healthBarHeight = 0.5f;

	private GameObject healthBar;
	private GameObject healthpoint;
	private Transform healthBarFill;
	private Text hpText;

	public int max;
	private int _current;
	public int current {
		get {
			return _current;
		}
		set {
			_current = Mathf.Max(0, Mathf.Min(value, max));
			Vector3 scale = healthBarFill.localScale;
			scale.x = (float) _current / max;
			healthBarFill.localScale = scale;
			if (_current == 0) {
				OnReachZero();
			}
		}
	}

	public event Action OnReachZero = delegate {};

	void Awake() {
		healthBar = Instantiate(prefabHealthBar, transform, false);
		healthBarFill = healthBar.transform.Find("Fill");
		current = max;
		healthpoint = healthBar.transform.Find("Healthpoint").gameObject;
		Transform textTr = healthpoint.transform.Find("Text");
		hpText = textTr.GetComponent<Text>();
		Vector3 pos = healthBar.transform.localPosition;
		pos.y = healthBarHeight;
		healthBar.transform.localPosition = pos;
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
