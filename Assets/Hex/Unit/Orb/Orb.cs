using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexUnit))]
public class Orb : MonoBehaviour {
	private HexUnit unit;

	public const int ORB_HEAL = 3;

	void Awake() {
		unit = GetComponent<HexUnit>();
	}

	void Start() {
		unit.asOrb = this;
		unit.DeathEvent += OrbDeath;
		unit.TurnSwapEvent += OrbTurnSwap;
	}

	void OrbDeath() {
		GameManager.GM.RegisterVictory(unit.team.Opposite());
	}

	void OrbTurnSwap() {
		foreach (HexCell cell in unit.cell.EnumerateNeighbors()) {
			if (cell.unit != null && cell.unit.team == unit.team) {
				cell.unit.Heal(ORB_HEAL);
			}
		}
	}
}
