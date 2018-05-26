using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour {

	private static int lastId = 0;
	private int id;

	public int movespeed = 0;
	private int _hp;
	public int hp {
		get {
			return _hp;
		}
		private set {
			_hp = value;
			hbarFill.localScale = new Vector3((float) _hp / maxHP, 1.0f, 1.0f);
		}
	}
	public int maxHP;
	public Team team = Team.NONE;
	public HexCell cell;
	public Orb asOrb;
	public Mage asMage;
	public Golem asGolem;
	// public Golem ownedGolem;

	public bool isImmobile;
	public bool isInvincible;
	public bool isMage {
		get { return asMage != null; }
	}
	public bool isGolem {
		get { return asGolem != null; }
	}
	public bool isOrb {
		get { return asOrb != null; }
	}
	public bool isObstacle;
	public bool hasMoved;

	public Action DeathEvent;
	public Action MoveEvent;
	public Action TurnSwapEvent;

	private SpriteRenderer rend;
	private SpriteRenderer teamRend;
	private Transform hbarFill;

	void Awake() {
		id = lastId;
		lastId++;
		rend = GetComponent<SpriteRenderer>();
		Transform trTransform = transform.Find("TeamOverlay");
		if (trTransform != null) {
			teamRend = trTransform.GetComponent<SpriteRenderer>();
		}
		if (!isInvincible) {
			hbarFill = transform.Find("Healthbar").Find("Fill");
			DeathEvent = UnitDeath;
		}
		if (!isImmobile) {
			MoveEvent = delegate {};
		}
		TurnSwapEvent = CheckFlames;
	}

	void Start() {
		if (!isInvincible) {
			hp = maxHP;
		}
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
		// CheckFlames();
		hasMoved = false;
	}

	public void EndTurn() {
		// CheckFlames();
		TurnSwapEvent();
	}

	private void CheckFlames() {
		if (cell.effect == Effect.FLAMES) {
			Damage(Spell.FIRE_DPT);
		}
	}

	// Returns true if unit is still alive after the hit
	public bool Damage(int dmg) {
		if (dmg < 0) {
			throw new System.ArgumentException(
				"Argument cannot be negative",
				"dmg");
		}
		if (isInvincible) {
			return true;
		}
		hp -= dmg;
		if (hp <= 0) {
			Die();
			return false;
		}
		return true;
	}

	public void Heal(int heal) {
		if (heal < 0) {
			throw new System.ArgumentException(
				"Argument cannot be negative",
				"heal");
		}
		if (!isInvincible) {
			hp = Math.Min(hp + heal, maxHP);
		}
	}

	public void Die() {
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