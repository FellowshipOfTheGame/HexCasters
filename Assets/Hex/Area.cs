using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : HashSet<HexCell> {
	public Area() : base() {}

	public Area(IEnumerable<HexCell> cells) : base() {
		UnionWith(cells);
	}
}
