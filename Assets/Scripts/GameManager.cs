using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(HexGrid))]
public class GameManager : MonoBehaviour {

	private static event Action initEvent;

	public HexGrid grid;
	public RawImage turnIndicator;
	public RawImage winnerIndicator;
	public Text gameStateIndicator;
	
	public static GameManager GM;

	private int _turn;
	public int turn {
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
				pairedUnitCell.highlightLevel = HighlightLevel.NONE;
			}
			_pairedUnitCell = value;
			if (pairedUnitCell != null) {
				pairedUnitCell.highlightLevel = HighlightLevel.RELEVANT;
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
		PREP,
		OVERVIEW,
		MOVE_SELECT_DEST,
		// MOVE_ANIM, (maybe)
		SPELL_CHOICE,
		SPELL_SELECT_TARGETS,
		// ATK_ANIM, // (probably, but later)
		RESULTS
	}

	private const string STATE_NAME_OVERVIEW = "Overview";
	private const string STATE_NAME_MOVE_SELECT_DEST = "Move";
	private const string STATE_NAME_SPELL_CHOICE = "Select Spell";
	private const string STATE_NAME_SPELL_SELECT_TARGETS = "Cast Spell";

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

	private Area validTargets;
	public Spell selectedSpell;
	private List<HexCell> spellTargets;
	private Area spellAOE;
	private HashSet<HexUnit> toBeDestroyed;
	private HashSet<HexUnit> beingDestroyed;
	private int winner;

	public GameObject prefabGolem;

	void Awake() {
		if (GM != null) {
			Destroy(gameObject);
			return;
		}
		GM = this;
		winner = Team.NONE;
		turn = 0;
		turnTransition = false;
		teams = new HashSet<HexUnit>[] {
			new HashSet<HexUnit>(),
			new HashSet<HexUnit>()
		};
		toBeDestroyed = new HashSet<HexUnit>();
		beingDestroyed = new HashSet<HexUnit>();

		if (initEvent != null) {
			initEvent();
		}
		initEvent = null;
		state = GameState.PREP;
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
					EndTurn();
				}
				break;
			case GameState.MOVE_SELECT_DEST:
				// if (Input.GetKeyDown(KeyCode.Backspace)) {
				if (InputCancel()) {
					selectedCell = null;
					state = GameState.OVERVIEW;
				}
				break;
			case GameState.SPELL_CHOICE:
				selectedSpell = null;
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
				// if (Input.GetKeyDown(KeyCode.Backspace)) {
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
		hoveredCell = cell;
		switch (state) {
			case GameState.OVERVIEW:
				UpdateActionableUnitsHighlight();
				cell.highlightLevel = HighlightLevel.IN_RANGE;
				// TODO spaghetti
				HighlightPairedUnit(cell.unit);
				break;
			case GameState.MOVE_SELECT_DEST:
				if (validTargets.Contains(cell)) {
					cell.highlightLevel = HighlightLevel.RELEVANT;
				}
				HighlightPairedUnit(cell.unit);
				break;
			case GameState.SPELL_CHOICE:
				cell.highlightLevel = HighlightLevel.IN_RANGE;
				HighlightPairedUnit(cell.unit);
				break;
			case GameState.SPELL_SELECT_TARGETS:
				UpdateSpellAOE();
				break;
		}
	}

	public void HoverExit(HexCell cell) {
		hoveredCell = null;
		switch (state) {
			case GameState.OVERVIEW:
				cell.highlightLevel = HighlightLevel.NONE;
				pairedUnitCell = null;
				UpdateActionableUnitsHighlight();
				break;
			case GameState.MOVE_SELECT_DEST:
				if (validTargets.Contains(cell)) {
					cell.highlightLevel = HighlightLevel.IN_RANGE;
				} else {
					cell.highlightLevel = HighlightLevel.NONE;
				}
				pairedUnitCell = null;
				if (selectedCell != null) {
					selectedCell.highlightLevel = HighlightLevel.SELECTED;
				}
				break;
			case GameState.SPELL_CHOICE:
				cell.highlightLevel = HighlightLevel.NONE;
				selectedCell.highlightLevel = HighlightLevel.SELECTED;
				pairedUnitCell = null;
				break;
			case GameState.SPELL_SELECT_TARGETS:
				foreach (HexCell c in validTargets) {
					c.highlightLevel = HighlightLevel.IN_RANGE;
				}
				foreach (HexCell c in spellAOE) {
					if (validTargets.Contains(c)) {
						c.highlightLevel = HighlightLevel.IN_RANGE;
					} else if (!spellTargets.Contains(c)) {
						c.highlightLevel = HighlightLevel.NONE;
					}
				}
				break;
		}
	}

	private void EnterState() {
		switch (state) {
			case GameState.OVERVIEW:
				gameStateIndicator.text = STATE_NAME_OVERVIEW;
				if (selectedCell != null) {
					selectedCell.highlightLevel = HighlightLevel.NONE;
				}
				selectedCell = null;
				UpdateActionableUnitsHighlight();
				break;
			case GameState.MOVE_SELECT_DEST:
				gameStateIndicator.text = STATE_NAME_MOVE_SELECT_DEST;
				int r = selectedUnit.movespeed;
				validTargets = selectedCell.Radius(r, true, true);
				foreach (HexCell c in validTargets) {
					c.highlightLevel = HighlightLevel.IN_RANGE;
				}
				break;
			case GameState.SPELL_CHOICE:
				gameStateIndicator.text = STATE_NAME_SPELL_CHOICE;
				selectedCell.highlightLevel = HighlightLevel.SELECTED;
				break;
			case GameState.SPELL_SELECT_TARGETS:
				gameStateIndicator.text = STATE_NAME_SPELL_SELECT_TARGETS;
				if (selectedCell != null) {
					selectedCell.highlightLevel = HighlightLevel.NONE;
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
				foreach (HexUnit u in teams[turn]) {
					u.cell.highlightLevel = HighlightLevel.NONE;
				}
				break;
			case GameState.MOVE_SELECT_DEST:
				foreach (HexCell c in validTargets) {
					c.highlightLevel = HighlightLevel.NONE;
				}
				pairedUnitCell = null;
				break;
			case GameState.SPELL_SELECT_TARGETS:
				RemoveSpellHighlight();
				break;
		}
	}

	public GameObject AddUnit(
			GameObject unitPrefab, int x, int y, int team=Team.NONE) {
		GameObject inst = Instantiate(unitPrefab);
		grid[x, y].content = inst;
		if (team != Team.NONE) {
			HexUnit unit = inst.GetComponent<HexUnit>();
			teams[team].Add(unit);
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
			teams[unit.team].Remove(unit);
		}
		unit.cell.content = null;
		Destroy(unit.gameObject);
		if (teams[unit.team].All(u => u.isOrb)) {
			RegisterVictory(Team.Opposite(unit.team));
		}
	}

	public void EndTurn() {
		foreach (HexCell c in teams[turn]
					.Select(unit => unit.cell)) {
			c.highlightLevel = HighlightLevel.NONE;
		}
		turnTransition = true;
		foreach (HexUnit unit in teams[turn]) {
			unit.EndTurn();
		}
		turn = Team.Opposite(turn);
		foreach (HexUnit unit in teams[turn]) {
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

	void RemoveSpellHighlight() {
		foreach (HexCell cell in
					validTargets
						.Union(spellAOE)
						.Union(spellTargets)) {
			cell.highlightLevel = HighlightLevel.NONE;
		}
	}

	void UpdateSpellValidTargets() {
		validTargets = new Area(
			selectedSpell.GetValidNextTargets(
				selectedUnit,
				spellTargets));
		foreach (HexCell cell in validTargets) {
			cell.highlightLevel = HighlightLevel.IN_RANGE;
		}
		foreach (HexCell cell in spellTargets) {
			cell.highlightLevel = HighlightLevel.RELEVANT;
		}
	}

	void UpdateSpellAOE() {
		if (validTargets.Contains(hoveredCell)) {
			spellAOE = new Area(selectedSpell.GetAOE(
				selectedUnit, hoveredCell, spellTargets));
			foreach (HexCell c in spellAOE) {
				c.highlightLevel = HighlightLevel.IN_AOE;
			}
		}
	}

	void UpdateTurnIndicator() {
		turnIndicator.color = Team.COLORS[turn];
	}

	void UpdateActionableUnitsHighlight() {
		foreach (HexCell c in teams[turn]
					.Where(unit => !unit.hasMoved)
					.Select(unit => unit.cell)) {
			c.highlightLevel = HighlightLevel.CAN_ACT;
		}
	}

	public void EndCharacterTurn() {
		if (selectedUnit != null) {
			selectedUnit.hasMoved = true;
		}
	}

	public void RegisterVictory(int team) {
		winner = team;
		state = GameState.RESULTS;
	}

	void ShowWinner() {
		winnerIndicator.color = Team.COLORS[winner];
	}

	bool InputCancel() {
		return Input.GetKeyDown(KeyCode.Backspace)
				|| Input.GetKeyDown(KeyCode.Escape)
				|| Input.GetMouseButtonDown(1);
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

}
