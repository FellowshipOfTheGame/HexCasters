using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipSpellcastButton : MonoBehaviour {
    public void Click() {
        GameManager.GM.selectedSpell = null;
        GameManager.GM.state = GameManager.GameState.OVERVIEW;
    }
}
