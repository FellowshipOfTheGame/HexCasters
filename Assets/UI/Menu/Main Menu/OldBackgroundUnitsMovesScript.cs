using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldBackgroundUnitsMovesScript : MonoBehaviour {

	private GameManager g;

	void Start() {
		g = GameManager.GM;
		StartCoroutine(AddDelay());
	}

	IEnumerator AddDelay() {
		HexCell[] validCellsBlue = null;
		HexCell[] validCellsRed = null;
		HexUnit[] magesRed = null;
		HexUnit[] magesBlue = null;
		int delay = 1;

		magesBlue = new HexUnit[g.teams[2].Count];
		g.teams[2].CopyTo(magesBlue);
		magesRed = new HexUnit[g.teams[1].Count];
		g.teams[1].CopyTo(magesRed);

		// Highlight the red team orb
		HighlightUnit(magesRed[0], GameManager.GameState.MOVE_SELECT_DEST);
		yield return new WaitForSeconds(delay);

		// Move it twice
		for (int i = 0; i < 2; i++) {
			// Highlight the first red mage
			HighlightUnit(magesRed[1], GameManager.GameState.MOVE_SELECT_DEST);
			yield return new WaitForSeconds(delay);
			// Get valid cells
			validCellsRed = new HexCell[g.validTargets.Count];
			g.validTargets.CopyTo(validCellsRed);
			// Move
			g.Click(validCellsRed[8]);
			yield return new WaitForSeconds(delay);
		}

		// Highlight all blue team units
		HighlightUnit(magesBlue[1], GameManager.GameState.MOVE_SELECT_DEST);
		yield return new WaitForSeconds(delay);
		// Get valid cells
		validCellsBlue = new HexCell[g.validTargets.Count];
		g.validTargets.CopyTo(validCellsBlue);
		// Move
		g.Click(validCellsBlue[12]);
		yield return new WaitForSeconds(delay);

		// Blue mage summons Golem
		g.SpellSelected("IMBUE_LIFE");
		HighlightUnit(magesBlue[1], GameManager.GameState.SPELL_SELECT_TARGETS);
		yield return new WaitForSeconds(delay);
		// Get valid cells
		validCellsBlue = new HexCell[g.validTargets.Count];
		g.validTargets.CopyTo(validCellsBlue);
		// Summon
		g.Click(validCellsBlue[5]);
		yield return new WaitForSeconds(delay);

		// Red mage moves forward
		HighlightUnit(magesRed[1], GameManager.GameState.MOVE_SELECT_DEST);
		yield return new WaitForSeconds(delay);
		// Get valid cells
		validCellsRed = new HexCell[g.validTargets.Count];
		g.validTargets.CopyTo(validCellsRed);
		// Move
		g.Click(validCellsRed[8]);
		yield return new WaitForSeconds(delay);
		HighlightUnit(magesRed[1], GameManager.GameState.MOVE_SELECT_DEST);
		yield return new WaitForSeconds(delay);
		// Get valid cells
		validCellsRed = new HexCell[g.validTargets.Count];
		g.validTargets.CopyTo(validCellsRed);
		// Move
		g.Click(validCellsRed[1]);
		yield return new WaitForSeconds(delay);

		// Red mage attacks
		g.SpellSelected("FIREBALL");
		HighlightUnit(magesRed[1], GameManager.GameState.SPELL_SELECT_TARGETS);
		yield return new WaitForSeconds(delay);
		// Get valid cells
		validCellsRed = new HexCell[g.validTargets.Count];
		g.validTargets.CopyTo(validCellsRed);
		// Casts spell
		g.Click(validCellsRed[10]);
		yield return new WaitForSeconds(delay);
	}

	public void HighlightUnit(HexUnit u, GameManager.GameState state) {
		g.Click(u.cell);
		g.selectedCell = u.cell;
		g.state = state;
	}

}
