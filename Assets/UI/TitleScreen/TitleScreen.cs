using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour, IPointerClickHandler {

	public void OnPointerClick(PointerEventData eventData) {
		//create asset to load main menu scene
		//MapLoader.LoadLayoutFromResource("Simple_1");
		SceneManager.LoadScene("MainMenu");
	}
}
