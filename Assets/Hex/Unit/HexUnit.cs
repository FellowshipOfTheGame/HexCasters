using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexUnit : MonoBehaviour {
	private static int lastId = 0;
	private int id;

	public int movespeed = 0;
	public HP hp;
	public Team team = Team.NONE;
	public HexCell cell;
	public Orb asOrb;
	public Mage asMage;
	public Golem asGolem;

	public bool isImmobile;
	public bool isInvincible { get; private set; }
	public bool isMage {
		get { return asMage != null; }
	}
	public bool isGolem {
		get { return asGolem != null; }
	}
	public bool isOrb {
		get { return asOrb != null; }
	}
	public bool hasMoved;

	public Action DeathEvent = delegate {};
	public Action MoveEvent = delegate {};
	public Action TurnSwapEvent = delegate {};

	private SpriteRenderer rend;
	private SpriteRenderer teamRend;
	private Transform hbarFill;

	void Awake() {
		id = lastId;
		lastId++;
		rend = GetComponent<SpriteRenderer>();
		hp = GetComponent<HP>();
		Transform trTransform = transform.Find("TeamOverlay");
		if (trTransform != null) {
			teamRend = trTransform.GetComponent<SpriteRenderer>();
		}
		isInvincible = (hp == null);
		if (!isInvincible) {
			hp.OnReachZero += Die;
			DeathEvent = UnitDeath;
		}
		TurnSwapEvent = CheckFlames;
	}

	void Start() {
		if (team != Team.NONE) {
			teamRend.color = TeamExtensions.COLORS[(int) team];
			if (team == Team.RED) {
				rend.flipX = true;
				teamRend.flipX = true;
			}
		}
	}

	void UnitDeath() {
	}

	public void StartTurn() {
		TurnSwapEvent();
		hasMoved = false;
	}

	public void EndTurn() {
		TurnSwapEvent();
	}

	private void CheckFlames() {
		if (cell.effect == Effect.FLAMES) {
			Damage(Spell.FIRE_DPT);
		}
	}

	public void ShowHealthPoint() {
		if (hp != null) {
			hp.UpdateNumberDisplay();
			hp.EnableNumberDisplay();
		}
	}

	public void HideHealthPoint() {
		if (hp != null) {
			hp.DisableNumberDisplay();
		}
	}

	// Returns true if unit is still alive after the hit
	public bool Damage(int dmg) {
		if (dmg < 0) {
			throw new System.ArgumentException(
				"Argument cannot be negative",
				"dmg");
		}
		if (hp == null) {
			return true;
		}
		if (isImmobile) {
			GameManager.GM.PlaySFX("BarrierDamage");
		}
		hp.current -= dmg;
		return hp.current > 0;
		// return true;
	}

	public void Heal(int heal) {
		if (heal < 0) {
			throw new System.ArgumentException(
				"Argument cannot be negative",
				"heal");
		}
		if (hp != null) {
			hp.current += heal;
		}
	}

	public void Die() {
		if (isMage) {
			GameManager.GM.PlaySFX("Death");
		}
		if (isImmobile) {
			GameManager.GM.PlaySFX("BarrierBreak");
		}
		GameManager.GM.RemoveUnit(this);
	}

	public override bool Equals(object obj) {
		if (obj == null) {
			return false;
		}
		HexUnit unit = obj as HexUnit;
		if (unit == null) {
			return false;
		}
		return unit.id == this.id;
	}

	public override int GetHashCode() {
		return id;
	}

}
