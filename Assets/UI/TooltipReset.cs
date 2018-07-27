using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipReset : MonoBehaviour {
	void OnEnable() {
		foreach (var tooltip in GameObject.FindObjectsOfType<Tooltip>()) {
			tooltip.Clear();
		}
	}
}
