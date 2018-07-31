using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUnitsMovesScript : MonoBehaviour {

	private GameManager g;
	public enum Spells {
		FIREBALL,
		LIGHTNING_BOLT,
		SUMMON_STORM,
		BLIZZARD,
		//ROCK_STRIKE,
		IMBUE_LIFE,
		CALL_WINDS,
		FLIGHT
	}

	void Start() {
		g = GameManager.GM;
		StartCoroutine(AddDelay());
	}

	IEnumerator AddDelay() {
		HexCell[] validCells = null;
		HexUnit[] units = null;
		int delay = 1;

		while (this != null) {
			for (int i = 1; i <= 2; i++) {
				units = new HexUnit[g.teams[i].Count];
				g.teams[i].CopyTo(units);

				// Red team moves
				for (int j = 0; j < units.Length; j++) {
					if (units.Length == 0) {
						yield return null;
					}
					// MOVE
					Highlight(units[j], GameManager.GameState.MOVE_SELECT_DEST);
					yield return new WaitForSeconds(delay);
					// Get valid cells
					validCells = new HexCell[g.validTargets.Count];
					g.validTargets.CopyTo(validCells);
					// Move to random cell
					g.Click(validCells[Random.Range(0, validCells.Length-1)]);
					yield return new WaitForSeconds(delay);
					// SPELL
					if (units[j].isMage) {
						g.SpellSelected(
							((Spells)Random.Range(0,
								System.Enum.GetValues(typeof(Spells)).Length-1)).ToString()
						);
						yield return new WaitForSeconds(delay);
						// Get valid cells
						validCells = new HexCell[g.validTargets.Count];
						g.validTargets.CopyTo(validCells);
						// Cast spell
						g.Click(validCells[Random.Range(0, validCells.Length-1)]);
						yield return new WaitForSeconds(delay);
					}
				}
			}
		}

	}

	public void Highlight(HexUnit u, GameManager.GameState state) {
		g.Click(u.cell);
		g.selectedCell = u.cell;
		g.state = state;
	}

}
