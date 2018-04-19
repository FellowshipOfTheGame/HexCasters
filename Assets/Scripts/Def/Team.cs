using UnityEngine;

public static class Team {
	public const int NONE = -1;
	public const int LEFT = 0;
	public const int RIGHT = 1;

	public static Color[] COLORS = {
		new Color(0.8f, 0.2f, 0.2f, 1.0f),
		new Color(0.2f, 0.2f, 0.8f, 1.0f)
	};

	public static int Opposite(int team) {
		return (1 - team);
	}
}