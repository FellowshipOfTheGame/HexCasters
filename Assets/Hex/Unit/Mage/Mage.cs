using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexUnit))]
public class Mage : MonoBehaviour {
	public HexUnit unit;
	public Golem ownedGolem;

	[Header("Animation prefabs")]
	public GameObject animFireball;
	public GameObject animLightningBolt;

	public GameObject animRain;
	public GameObject animSnow;

	public GameObject animRockStrike;
	public GameObject animImbueLife;

	public GameObject animWind;

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
		GameObject golemObj = GameManager.GM.AddObject(
			GameManager.GM.prefabGolem,
			targetCell.X, targetCell.Y,
			unit.team);
		ownedGolem = golemObj.GetComponent<Golem>();
		ownedGolem.owner = this;
		// ownedGolem.storedEffect = targetCell.effect;
		// targetCell.SetEffect(Effect.NONE);
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

	public void AnimateFireball(HexCell target) {
		Instantiate(animFireball, target.transform, false);
	}

	public void AnimateLightningBolt(HexCell target) {
		Instantiate(animLightningBolt, target.transform, false);
	}

	public void AnimateRain(HexCell target) {
		Instantiate(animRain, target.transform, false);
	}

	public void AnimateSnow(HexCell target) {
		Instantiate(animSnow, target.transform, false);
	}

	public void AnimateRockStrike(HexCell firstOccupiedCell) {
		GameObject anim = Instantiate(animRockStrike);
		RockStrikeAnimController contr =
			anim.GetComponent<RockStrikeAnimController>();
		contr.Init(unit.cell, firstOccupiedCell);
	}

	public void AnimateImbueLife(HexCell target) {
		Instantiate(animImbueLife, target.transform, false);
	}

	public void AnimateCallWinds(HexCell origin, HexCell target) {
		var wind = AnimateGenericWind(origin, target);
		if (origin.effect == Effect.FLAMES) {
			wind.startColor = Color.red;
		} else if (origin.effect == Effect.STORM) {
			wind.startColor = Color.blue;
		} else {
			wind.startColor = Color.white;
		}
	}

	public void AnimateFlight(HexCell origin, HexCell target) {
		AnimateGenericWind(origin, target);
	}

	public ParticleSystem.MainModule AnimateGenericWind(HexCell origin, HexCell target) {
		GameObject ps =  Instantiate(animWind, origin.transform, false);
		ps.transform.LookAt(target.transform, Vector2.up);
		return ps.GetComponent<ParticleSystem>().main;
	}

	public void PlaySpellSFX(string name) {
		FindObjectOfType<AudioManager>().Play(name);
	}
}
