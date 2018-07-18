using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexUnit))]
public class Golem : MonoBehaviour {
	public Mage owner;
	// public string storedEffect;

	public int GOLEM_DMG = 2;
	public int GOLEM_SELF_DMG = 1;

	public HexUnit unit;

	void Awake() {
		unit = GetComponent<HexUnit>();
	}

	void Start() {
		// if (storedEffect != Effect.NONE) {
		// 	transform.Find(storedEffect).gameObject.SetActive(true);
		// }
		unit.asGolem = this;
		unit.DeathEvent += GolemDeath;
		unit.MoveEvent += GolemMove;
		unit.TurnSwapEvent += GolemTurnSwap;
	}

	void GolemDeath() {
		// unit.cell.ApplyEffect(storedEffect);
		if (owner != null) {
			// owner wasn't destroyed
			owner.ownedGolem = null;
		}
	}

	void GolemMove() {
		// TODO testing
		// if (owner != null) {
		// 	owner.unit.hasMoved = true;
		// }
	}

	void GolemTurnSwap() {
		foreach (HexCell cell in unit.cell.EnumerateNeighbors()) {
			if (cell.unit != null
					&& cell.unit.team == unit.team.Opposite()) {
				cell.unit.Damage(GOLEM_DMG);
			}
		}
		unit.Damage(GOLEM_SELF_DMG);
	}
}
