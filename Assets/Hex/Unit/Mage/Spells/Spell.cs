using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Spell {

	// All delegates receive a list of cells which the
	// player has *already clicked on* previously as the targets
	// for the spell

	public delegate IEnumerable<HexCell> GetAOEDelegate(
		HexUnit caster, HexCell hovered, List<HexCell> targets);
	public delegate IEnumerable<HexCell> GetValidNextTargetsDelegate(
		HexUnit caster, List<HexCell> curTargets);
	public delegate void CastDelegate(
		HexUnit caster, List<HexCell> targets, Area aoe);


	// FIREBALL
	public const int FIREBALL_RANGE = 2;
	public const int FIREBALL_RADIUS = 1;
	public const int FIRE_DPT = 3;
	public static readonly Spell FIREBALL = new Spell(
		1,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Fireball");
			caster.asMage.AnimateFireball(targets[0]);
			foreach (HexCell cell in aoe) {
				cell.ApplyFlames();
			}
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			return caster.cell.Radius(FIREBALL_RANGE);
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			return hovered.Radius(FIREBALL_RADIUS);
		});


	// LIGHTNING BOLT
	public const int LIGHTNING_BOLT_RANGE = 2;
	public const int LIGHTNING_BOLT_DAMAGE = 4;
	public static readonly Spell LIGHTNING_BOLT = new Spell(
		1,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Bolt");
			foreach (HexCell cell in aoe) {
				caster.asMage.AnimateLightningBolt(cell);
				if (cell.unit != null) {
					cell.unit.Damage(LIGHTNING_BOLT_DAMAGE);
				}
			}
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			return caster.cell.Radius(LIGHTNING_BOLT_RANGE);
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			return hovered.StormConnectedComponent();
		});


	// SUMMON STORM
	public const int SUMMON_STORM_RANGE = 3;
	public const int SUMMON_STORM_RADIUS = 1;
	public static readonly Spell SUMMON_STORM = new Spell(
		1,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Storm");
			foreach (HexCell cell in aoe) {
				cell.ApplyStorm();
				caster.asMage.AnimateRain(cell);
			}
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			return caster.cell.Radius(SUMMON_STORM_RANGE);
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			return hovered.Radius(SUMMON_STORM_RADIUS);
		});


	// BLIZZARD
	public const int BLIZZARD_RANGE = 2;
	public static readonly Spell BLIZZARD = new Spell(
		1,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Blizzard");
			targets[0].ApplySnow(); // SetEffect expands through storms
			foreach (HexCell cell in aoe) {
				caster.asMage.AnimateSnow(cell);
			}
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			return caster.cell.Radius(BLIZZARD_RANGE);
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			return hovered.StormConnectedComponent();
		});


	// ROCK STRIKE
	public const int ROCK_STRIKE_RANGE = 4;
	public const int ROCK_STRIKE_DAMAGE = 3;
	public static readonly Spell ROCK_STRIKE = new Spell(
		1,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Rock");
			// Can't use clicked target because enemy hit is not necessarily
			// in the cell the player clicked on
			// HexCell target = aoe
			// 	.Where(cell => cell.content != null)
			// 	.FirstOrDefault();
            List<HexCell> affectedUnits = aoe
                .Where(cell => cell.content != null)
                .OrderBy(cell => cell.ManhattanDistanceTo(caster.cell))
                .ToList();
            HexCell target = affectedUnits[0];
            HexCell animEnd = null;
            if (target != null) {
                bool alive = false;
                bool mobile = !target.unit.isImmobile;
                if (!target.unit.isInvincible) {
                    alive = target.unit.Damage(ROCK_STRIKE_DAMAGE);
                }
                if (mobile) {
                    int dir = caster.cell.DirectionTo(target);
                    HexCell knockedBack = target.GetNeighbor(dir);
                    if (knockedBack != null) {
                        if (knockedBack.content == null && knockedBack.terrain.transponible) {
                            if (alive) {
                                target.MoveContentTo(knockedBack);
                            }
                        } else if (knockedBack.unit != null) {
                            knockedBack.unit.Damage(ROCK_STRIKE_DAMAGE);
                        }
                    }
                }
                animEnd = target;
            } else {
                // get furthest cell from origin in AoE
                animEnd = aoe
                    .OrderByDescending(
                        cell => cell.ManhattanDistanceTo(caster.cell))
                    .First();
            }
            caster.asMage.AnimateRockStrike(animEnd);
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			return Enumerable.Range(0, HexCell.directions.Length)
				.SelectMany(dir => caster.cell.Line(dir, ROCK_STRIKE_RANGE));
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			int dir = caster.cell.DirectionTo(hovered);
			var line = caster.cell
                .Line(dir, ROCK_STRIKE_RANGE);
            var farthestCell = line
                .OrderByDescending(
                    cell => cell.ManhattanDistanceTo(caster.cell))
                .First();
            var neighbor = farthestCell.GetNeighbor(dir);
            if (farthestCell.unit != null
                    && !farthestCell.unit.isImmobile
                    && neighbor != null
                    && neighbor.terrain.transponible) {
                line.Add(neighbor);
            }
            return line;
		});


	// IMBUE LIFE
	public const int IMBUE_LIFE_HEAL_RANGE = 3;
	public const int IMBUE_LIFE_GOLEM_RANGE = 1;
	public const int IMBUE_LIFE_HEAL = 2;
	public static readonly Spell IMBUE_LIFE = new Spell(
		1,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			HexCell target = targets[0];
			if (target.content == null) {
				GameManager.GM.PlaySFX("SummonGolem");
				caster.asMage.SpawnGolem(target);
			} else {
				GameManager.GM.PlaySFX("Life");
				target.unit.Heal(IMBUE_LIFE_HEAL);
			}
			caster.asMage.AnimateImbueLife(target);
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			IEnumerable<HexCell> heal =
				caster.cell.Radius(IMBUE_LIFE_HEAL_RANGE)
					.Where(cell => cell.unit != null && !cell.unit.isInvincible);
			if (caster.asMage.ownedGolem != null) {
				// can't spawn another golem
				return heal;
			}
			return heal
				.Union(
					caster.cell.Radius(IMBUE_LIFE_GOLEM_RANGE)
						.Where(cell => cell.content == null)
						.Where(cell => cell.terrain.transponible));
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			return hovered.AsArea();
		});


	// CALL WINDS
	public const int CALL_WINDS_RANGE = 3;
	public const int CALL_WINDS_LENGTH = 2;
	public static readonly Spell CALL_WINDS = new Spell(
		2,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Wind");
			caster.asMage.AnimateCallWinds(targets[0], targets[1]);
			HexCell start = targets[0];
			foreach (HexCell target in aoe) {
				target.ApplyEffect(start.effect);
			}
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			if (curTargets.Count == 1) {
				return Enumerable.Range(0, HexCell.directions.Length)
					.SelectMany(
						dir => curTargets[0].Line(
							dir, CALL_WINDS_LENGTH, false, false));
			}
			return caster.cell.Radius(CALL_WINDS_RANGE)
				.Where(cell => cell.effect != Effect.NONE);
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			if (targets.Count == 1) {
				int dir = targets[0].DirectionTo(hovered);
				Area line = targets[0].Line(
					dir, CALL_WINDS_LENGTH, false, false);
				if (targets[0].effect == Effect.SNOW) {
					return line
						.SelectMany(cell => cell.StormConnectedComponent());
				} else {
					return line;
				}
			}
			return hovered.AsArea();
		});


	// FLIGHT
	public const int FLIGHT_RANGE = 2;
	public const int FLIGHT_DEST_DIST = 2;
	public static readonly Spell FLIGHT = new Spell(
		2,
		delegate (HexUnit caster, List<HexCell> targets, Area aoe) {
			GameManager.GM.PlaySFX("Flight");
			caster.asMage.AnimateFlight(targets[0], targets[1]);
			targets[0].MoveContentTo(targets[1]);
			GameManager.GM.selectedCell = caster.cell;
		},
		delegate (HexUnit caster, List<HexCell> curTargets) {
			if (curTargets.Count == 0) {
				return caster.cell.Radius(FLIGHT_RANGE)
					.Where(cell => cell.unit != null && !cell.unit.isImmobile);
			} else {
				return curTargets[0].Radius(FLIGHT_DEST_DIST)
					.Where(cell => cell.content == null)
					.Where(cell => cell.terrain.transponible);
			}
		},
		delegate (HexUnit caster, HexCell hovered, List<HexCell> targets) {
			return hovered.AsArea();
		});


	public int numOfTargets;
	public CastDelegate Cast;
	public GetValidNextTargetsDelegate GetValidNextTargets;
	public GetAOEDelegate GetAOE;

	public Spell(
			int numOfTargets,
			CastDelegate cast,
			GetValidNextTargetsDelegate getValidNextTargets,
			GetAOEDelegate getAOE) {
		this.numOfTargets = numOfTargets;
		this.Cast = cast;
		this.GetValidNextTargets = getValidNextTargets;
		this.GetAOE = getAOE;
	}

}
