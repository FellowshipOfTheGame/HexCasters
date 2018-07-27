using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
/// <summary>
/// Object must have a canvas somewhere up the hierarchy AND have a child named
/// "Tooltip", which will be enabled once hover is detected.
/// </summary>
public class Tooltip : MonoBehaviour {

	/// <summary>
	/// How long to wait (in seconds) before displaying the text.
	/// </summary>
	public float delay = 1.0f;

	private bool beingHovered;
	private float hoverTime;

	private EventTrigger trigger;
	private Canvas canvas;
	private GameObject tooltipObj;

	public void Clear() {
		tooltipObj.SetActive(false);
		beingHovered = false;
		hoverTime = 0;
	}

	void Awake() {
		trigger = GetComponent<EventTrigger>();
		FindCanvas();
		FindTooltip();
	}

	void Start() {
		RegisterEvents();
	}

	void Update() {
		if (beingHovered) {
			hoverTime += Time.deltaTime;
			if (hoverTime >= delay) {
				UpdateTooltipPosition();
				ShowTooltip();
			}
		} else {
			HideTooltip();
		}
	}

	void FindCanvas() {
		Transform p = transform.parent;
		do {
			canvas = p.GetComponent<Canvas>();
			p = p.parent;
		} while (canvas == null);
	}

	void FindTooltip() {
		tooltipObj = transform.Find("Tooltip").gameObject;
	}

	void RegisterEvents() {
		EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
		pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
		pointerEnterEntry.callback.AddListener(PointerEnter);
		trigger.triggers.Add(pointerEnterEntry);

		EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
		pointerExitEntry.eventID = EventTriggerType.PointerExit;
		pointerExitEntry.callback.AddListener(PointerExit);
		trigger.triggers.Add(pointerExitEntry);

		EventTrigger.Entry clickEntry = new EventTrigger.Entry();
		clickEntry.eventID = EventTriggerType.PointerClick;
		clickEntry.callback.AddListener(PointerExit);
		trigger.triggers.Add(clickEntry);
	}

	void PointerEnter(BaseEventData data) {
		foreach (var tooltip in Object.FindObjectsOfType<Tooltip>()) {
			tooltip.hoverTime = 0;
		}
		beingHovered = true;
	}

	void PointerExit(BaseEventData data) {
		hoverTime = 0;
		beingHovered = false;
	}

	void UpdateTooltipPosition() {
		(tooltipObj.transform as RectTransform).position = Input.mousePosition;
	}

	void ShowTooltip() {
		tooltipObj.SetActive(true);
	}

	void HideTooltip() {
		tooltipObj.SetActive(false);
	}

}
