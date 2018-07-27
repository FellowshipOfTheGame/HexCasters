using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(HexGrid))]
public class GameManager : MonoBehaviour {

	private static event Action initEvent;

	public Button endTurnButton;
	public HexGrid grid;
	public RawImage turnIndicator;
	public RawImage winnerIndicator;
	public Text winnerMessage;
	public GameObject spellList;

	public static GameManager GM;

	private Team _turn;
	public Team turn {
		get {
			return _turn;
		}
		set {
			_turn = value;
			UpdateTurnIndicator();
		}
	}
	private bool turnTransition;
	public HashSet<HexUnit>[] teams;

	public HexCell selectedCell;
	public HexCell hoveredCell;
	public HexCell _pairedUnitCell;
	public HexCell pairedUnitCell {
		get {
			return _pairedUnitCell;
		}
		set {
			if (pairedUnitCell != null) {
				pairedUnitCell.highlight = Highlight.NONE;
			}
			_pairedUnitCell = value;
			if (pairedUnitCell != null) {
				pairedUnitCell.highlight = Highlight.RELEVANT;
			}
		}
	}
	public HexUnit selectedUnit {
		get {
			if (selectedCell == null) {
				return null;
			}
			return selectedCell.unit;
		}
	}
	public Golem selectedGolem {
		get {
			if (selectedUnit == null) {
				return null;
			}
			return selectedUnit.asGolem;
		}
	}

	public enum GameState {
		OVERVIEW,
		MOVE_SELECT_DEST,
		// MOVE_ANIM, (maybe)
		SPELL_CHOICE,
		SPELL_SELECT_TARGETS,
		// ATK_ANIM, // (probably, but later)
		RESULTS
	}

	[SerializeField]
	private GameState _state;
	public GameState state {
		get {
			return _state;
		}
		set {
			pairedUnitCell = null;
			ExitState();
			_state = value;
			EnterState();
		}
	}

	public Area validTargets;
	public Spell selectedSpell;
	public List<HexCell> spellTargets;
	public Area spellAOE;
	private HashSet<HexUnit> toBeDestroyed;
	private HashSet<HexUnit> beingDestroyed;
	private Team winner;

	public GameObject prefabGolem;

	void Awake() {
		if (GM != null) {
			Destroy(gameObject);
			return;
		}
		GM = this;
		winner = Team.NONE;
		turn = Team.RED;
		turnTransition = false;
		teams = new HashSet<HexUnit>[(int) Team.N_TEAMS];
		for (int i = 0; i < teams.Length; i++) {
			teams[i] = new HashSet<HexUnit>();
		}
		toBeDestroyed = new HashSet<HexUnit>();
		beingDestroyed = new HashSet<HexUnit>();

		if (initEvent != null) {
			initEvent();
		}
		initEvent = null;

		Begin();
	}

	public void Begin() {
		state = GameState.OVERVIEW;
	}

	// TODO remove, this is debug
	public void Update() {
		if (Input.GetKey(KeyCode.Alpha1)
				&& Input.GetKey(KeyCode.Alpha2)
				&& Input.GetKey(KeyCode.Alpha3)) {
			SceneManager.LoadScene("Test");
		}
		switch (state) {
			case GameState.OVERVIEW:
				if (Input.GetKeyDown(KeyCode.Space)) {
					// GM loaded in main menu as Background Map scene
					if (BackgroundMapLoader.BMLoader == null) {
						EndTurn();
					}
				}
				break;
			case GameState.MOVE_SELECT_DEST:
				if (InputCancel()) {
					selectedCell = null;
					state = GameState.OVERVIEW;
				}
				break;
			case GameState.SPELL_CHOICE:
				selectedSpell = null;
				// GM loaded in main menu as Background Map scene
				if (BackgroundMapLoader.BMLoader != null) {
					return;
				}
				if (Input.GetKeyDown(KeyCode.Alpha0)) {
					state = GameState.OVERVIEW;
				} else if (Input.GetKeyDown(KeyCode.Alpha1)) {
					selectedSpell = Spell.FIREBALL;
				} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
					selectedSpell = Spell.LIGHTNING_BOLT;
				} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
					selectedSpell = Spell.SUMMON_STORM;
				} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
					selectedSpell = Spell.BLIZZARD;
				} else if (Input.GetKeyDown(KeyCode.Alpha5)) {
					selectedSpell = Spell.ROCK_STRIKE;
				} else if (Input.GetKeyDown(KeyCode.Alpha6)) {
					selectedSpell = Spell.IMBUE_LIFE;
				} else if (Input.GetKeyDown(KeyCode.Alpha7)) {
					selectedSpell = Spell.CALL_WINDS;
				} else if (Input.GetKeyDown(KeyCode.Alpha8)) {
					selectedSpell = Spell.FLIGHT;
				}
				if (selectedSpell != null) {
					state = GameState.SPELL_SELECT_TARGETS;
				}
				break;
			case GameState.SPELL_SELECT_TARGETS:
				if (InputCancel()) {
					state = GameState.SPELL_CHOICE;
				}
				break;
		}
	}

	public void Click(HexCell cell) {
		switch (state) {
			case GameState.OVERVIEW:
				if (cell.unit != null && cell.unit.team == turn
						&& !cell.unit.hasMoved) {
					selectedCell = cell;
					pairedUnitCell = null;
					state = GameState.MOVE_SELECT_DEST;
				}
				break;
			case GameState.MOVE_SELECT_DEST:
				if (validTargets.Contains(cell)) {
					selectedCell.MoveContentTo(cell);
					selectedCell = cell;
					selectedUnit.hasMoved = true;
					selectedUnit.MoveEvent();
					if (selectedUnit.isMage) {
						state = GameState.SPELL_CHOICE;
					} else {
						if (selectedUnit.isOrb) {
							PlaySFX("OrbMove");
						}
						state = GameState.OVERVIEW;
					}
				} else {
					state = GameState.OVERVIEW;
				}
				break;
			case GameState.SPELL_CHOICE:
				break;
			case GameState.SPELL_SELECT_TARGETS:
				if (validTargets.Contains(cell)) {
					RemoveSpellHighlight();

					spellTargets.Add(cell);
					if (spellTargets.Count < selectedSpell.numOfTargets) {
						UpdateSpellValidTargets();
					} else {
						selectedSpell.Cast(
							selectedUnit, spellTargets, spellAOE);
						EndCharacterTurn();
						state = GameState.OVERVIEW;
					}
				}
				break;
		}
	}

	public void HoverEnter(HexCell cell) {
		// GM loaded in main menu as Background Map scene
		if (BackgroundMapLoader.BMLoader != null) {
			return;
		}
		hoveredCell = cell;
		ShowHealthpointText(cell.unit);
		switch (state) {
			case GameState.OVERVIEW:
				UpdateActionableUnitsHighlight();
				cell.highlight = Highlight.IN_RANGE;
				HighlightPairedUnit(cell.unit);
				break;
			case GameState.MOVE_SELECT_DEST:
				if (validTargets.Contains(cell)) {
					cell.highlight = Highlight.RELEVANT;
				}
				HighlightPairedUnit(cell.unit);
				break;
			case GameState.SPELL_CHOICE:
				cell.highlight = Highlight.IN_RANGE;
				HighlightPairedUnit(cell.unit);
				break;
			case GameState.SPELL_SELECT_TARGETS:
				UpdateSpellAOE();
				break;
		}
	}

	public void HoverExit(HexCell cell) {
		// GM loaded in main menu as Background Map scene
		if (BackgroundMapLoader.BMLoader != null) {
			return;
		}
		hoveredCell = null;
		RemoveHealthpointText(cell.unit);
		switch (state) {
			case GameState.OVERVIEW:
				cell.highlight = Highlight.NONE;
				pairedUnitCell = null;
				UpdateActionableUnitsHighlight();
				break;
			case GameState.MOVE_SELECT_DEST:
				if (validTargets.Contains(cell)) {
					cell.highlight = Highlight.IN_RANGE;
				} else {
					cell.highlight = Highlight.NONE;
				}
				pairedUnitCell = null;
				if (selectedCell != null) {
					selectedCell.highlight = Highlight.SELECTED;
				}
				break;
			case GameState.SPELL_CHOICE:
				cell.highlight = Highlight.NONE;
				selectedCell.highlight = Highlight.SELECTED;
				pairedUnitCell = null;
				break;
			case GameState.SPELL_SELECT_TARGETS:
				foreach (HexCell c in validTargets) {
					c.highlight = Highlight.IN_RANGE;
				}
				foreach (HexCell c in spellAOE) {
					if (validTargets.Contains(c)) {
						c.highlight = Highlight.IN_RANGE;
					} else if (!spellTargets.Contains(c)) {
						c.highlight = Highlight.NONE;
					}
				}
				break;
		}
	}

	private void EnterState() {
		switch (state) {
			case GameState.OVERVIEW:
				if (selectedCell != null) {
					selectedCell.highlight = Highlight.NONE;
				}
				selectedCell = null;
				UpdateActionableUnitsHighlight();
				break;
			case GameState.MOVE_SELECT_DEST:
				int r = selectedUnit.movespeed;
				validTargets = selectedCell.Radius(r, true, true, false);
				foreach (HexCell c in validTargets) {
					c.highlight = Highlight.IN_RANGE;
				}
				break;
			case GameState.SPELL_CHOICE:
				spellList.SetActive(true);
				selectedCell.highlight = Highlight.SELECTED;
				break;
			case GameState.SPELL_SELECT_TARGETS:
				if (selectedCell != null) {
					selectedCell.highlight = Highlight.NONE;
				}
				spellTargets = new List<HexCell>();
				spellAOE = new Area();
				UpdateSpellValidTargets();
				if (validTargets.Count == 0) {
					// TODO this could break
					state = GameState.SPELL_CHOICE;
				}
				break;
			case GameState.RESULTS:
				ShowWinner();
				break;
		}
		if (hoveredCell != null) {
			HoverEnter(hoveredCell);
		}
	}

	private void ExitState() {
		if (hoveredCell != null) {
			HexCell cell = hoveredCell;
			HoverExit(hoveredCell);
			hoveredCell = cell;
		}
		switch (state) {
			case GameState.OVERVIEW:
				foreach (HexUnit u in teams[(int) turn]) {
					u.cell.highlight = Highlight.NONE;
				}
				break;
			case GameState.MOVE_SELECT_DEST:
				foreach (HexCell c in validTargets) {
					c.highlight = Highlight.NONE;
				}
				pairedUnitCell = null;
				break;
			case GameState.SPELL_CHOICE:
				spellList.SetActive(false);
				break;
			case GameState.SPELL_SELECT_TARGETS:
				RemoveSpellHighlight();
				break;
		}
	}

	public GameObject AddObject(
			GameObject obj, int x, int y, Team team=Team.NONE) {
		GameObject inst = Instantiate(obj);
		grid[x, y].content = inst;
		HexUnit unit = inst.GetComponent<HexUnit>();
		if (unit != null) {
			teams[(int) team].Add(unit);
			unit.team = team;
		}
		return inst;
	}

	public void RemoveUnit(HexUnit unit) {
		if (turnTransition) {
			toBeDestroyed.Add(unit);
		} else {
			DestroyUnit(unit);
		}
	}

	private void DestroyUnit(HexUnit unit) {
		unit.DeathEvent();
		if (unit.team != Team.NONE) {
			teams[(int) unit.team].Remove(unit);
		}
		unit.cell.content = null;
		Destroy(unit.gameObject);
		if (unit.team != Team.NONE
				&& teams[(int) unit.team].All(u => u.isOrb)) {
			RegisterVictory(unit.team.Opposite());
		}
	}

	public void EndTurn() {
		foreach (HexCell c in teams[(int) turn]
					.Select(unit => unit.cell)
					.Except(new List<HexCell> { hoveredCell })) {
			c.highlight = Highlight.NONE;
		}
		turnTransition = true;
		foreach (HexUnit unit in teams[(int) turn]
				.Union(teams[(int) Team.NONE])) {
			unit.EndTurn();
		}
		turn = turn.Opposite();
		foreach (HexUnit unit in teams[(int) turn]) {
			unit.StartTurn();
		}
		turnTransition = false;

		while (toBeDestroyed.Count > 0) {
			beingDestroyed.UnionWith(toBeDestroyed);
			toBeDestroyed.Clear();
			foreach (HexUnit unit in beingDestroyed) {
				DestroyUnit(unit);
			}
			beingDestroyed.Clear();
		}
		toBeDestroyed.Clear();
		UpdateActionableUnitsHighlight();
		Timer.T.ResetCountdownTimer();
		if (hoveredCell != null) {
			ShowHealthpointText(hoveredCell.unit);
		}
	}

	void HighlightPairedUnit(HexUnit unit) {
		if (unit != null) {
			if (unit.isMage && unit.asMage.ownedGolem != null) {
				pairedUnitCell = unit.asMage.ownedGolem.unit.cell;
			} else if (unit.isGolem && unit.asGolem.owner != null) {
				pairedUnitCell = unit.asGolem.owner.unit.cell;
			}
		}
	}

	void ShowHealthpointText(HexUnit unit) {
		if (unit != null) {
			unit.ShowHealthPoint();
		}
	}

	void RemoveHealthpointText(HexUnit unit) {
		if (unit != null) {
			unit.HideHealthPoint();
		}
	}

	void RemoveSpellHighlight() {
		foreach (HexCell cell in
					validTargets
						.Union(spellAOE)
						.Union(spellTargets)) {
			cell.highlight = Highlight.NONE;
		}
	}

	void UpdateSpellValidTargets() {
		validTargets = new Area(
			selectedSpell.GetValidNextTargets(
				selectedUnit,
				spellTargets));
		foreach (HexCell cell in validTargets) {
			cell.highlight = Highlight.IN_RANGE;
		}
		foreach (HexCell cell in spellTargets) {
			cell.highlight = Highlight.RELEVANT;
		}
	}

	void UpdateSpellAOE() {
		if (validTargets.Contains(hoveredCell)) {
			spellAOE = new Area(selectedSpell.GetAOE(
				selectedUnit, hoveredCell, spellTargets));
			foreach (HexCell c in spellAOE) {
				c.highlight = Highlight.IN_AOE;
				if (c.unit != null) {
					c.highlight = Highlight.RELEVANT; //change highlight sprite and name
				}
			}
		}
	}

	void UpdateTurnIndicator() {
		turnIndicator.color = TeamExtensions.COLORS[(int) turn];
	}

	void UpdateActionableUnitsHighlight() {
		foreach (HexCell c in teams[(int) turn]
					.Where(unit => !unit.hasMoved)
					.Select(unit => unit.cell)) {
			c.highlight = Highlight.CAN_ACT;
		}
	}

	public void EndCharacterTurn() {
		if (selectedUnit != null) {
			selectedUnit.hasMoved = true;
		}
	}

	public void RegisterVictory(Team team) {
		winner = team;
		state = GameState.RESULTS;
	}

	void ShowWinner() {
		winnerMessage.text = winner + " Team win!";
		winnerMessage.color = TeamExtensions.COLORS[(int) winner];
		winnerIndicator.gameObject.SetActive(true);
		endTurnButton.interactable = false;
	}

	public void BackToMainMenu() {
		SceneManager.LoadScene("MainMenu");
	}

	bool InputCancel() {
		return Input.GetKeyDown(KeyCode.Backspace)
				|| (Input.GetKeyDown(KeyCode.Escape) &&
					 (BackgroundMapLoader.BMLoader == null))
				|| Input.GetMouseButtonDown(1);
	}

	public void SpellSelected(String spellName) {
		FieldInfo info = typeof(Spell).GetField(
			spellName,
			System.Reflection.BindingFlags.Public
				| System.Reflection.BindingFlags.Static);
		selectedSpell = info.GetValue(null) as Spell;
		state = GameState.SPELL_SELECT_TARGETS;
	}

	public void PlaySFX(string name) {
		FindObjectOfType<AudioManager>().Play(name);
	}

	/*
	 * Registers an Action to be performed after the GameManager is
	 * instantiated.
	 * If the GM already exists, the Action is performed instantly.
	 */
	public static void AfterInit(Action act) {
		if (GM == null) {
			initEvent += act;
		} else {
			act();
		}
	}

	public void EndTurnButton() {
		if (state == GameState.OVERVIEW) {
			EndTurn();
		}
	}

}
