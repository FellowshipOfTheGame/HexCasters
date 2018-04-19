using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexUnit))]
public class Mage : MonoBehaviour {
	public HexUnit unit;
	public Golem ownedGolem;

	void Awake() {
		unit = GetComponent<HexUnit>();
		ownedGolem = null;
	}

	void Start() {
		unit.asMage = this;
		unit.DeathEvent += MageDeath;
		unit.MoveEvent += MageMove;
	}

	public void SpawnGolem(HexCell targetCell) {
		GameObject golemObj = GameManager.GM.AddUnit(
			GameManager.GM.prefabGolem,
			targetCell.X, targetCell.Y,
			unit.team);
		ownedGolem = golemObj.GetComponent<Golem>();
		ownedGolem.owner = this;
		ownedGolem.storedEffect = targetCell.effect;
		targetCell.SetEffect(Effect.NONE);
	}

	void MageDeath() {
		if (ownedGolem != null) {
			ownedGolem.owner = null;
			ownedGolem.unit.Die();
		}
	}

	void MageMove() {
		// TODO testing
		// if (ownedGolem != null) {
		// 	ownedGolem.unit.hasMoved = true;
		// }
	}
}
