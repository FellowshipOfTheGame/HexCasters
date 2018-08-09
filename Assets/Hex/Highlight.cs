using UnityEngine;

public static class Highlight {
	public static readonly Color NONE = new Color(0.0f, 0.0f, 0.0f, 0.0f);
	public static readonly Color IN_RANGE = new Color(1.0f, 1.0f, 1.0f, 0.50f);
	public static readonly Color RELEVANT = new Color(0.6f, 0.6f, 1.0f, 0.75f);
	public static readonly Color IN_AOE = new Color(0.9f, 0.7f, 0.3f, 0.6f);
	public static readonly Color SELECTED = new Color(0.5f, 0.5f, 0.5f, 0.75f);
	public static readonly Color CAN_ACT = new Color(1.0f, 1.0f, 1.0f, 0.3f);
}