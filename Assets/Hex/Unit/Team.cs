using UnityEngine;

public enum Team {
	NONE,
	RED,
	BLUE,
	N_TEAMS
}

public static class TeamExtensions {
	public static Color[] COLORS = {
		Color.white,
		new Color(0.8f, 0.2f, 0.2f, 1.0f),
		new Color(0.2f, 0.2f, 0.8f, 1.0f)
	};

	public static Team Opposite(this Team team) {
		if (team == Team.RED) {
			return Team.BLUE;
		}
		return Team.RED;
	}
}