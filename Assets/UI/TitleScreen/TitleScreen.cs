using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

	private float timeInTitleScreen;
	public float maxTimeInTitleScreen;
    public AsyncOperation asyncLoad;

	public void Start() {
		timeInTitleScreen = 0.0f;
    StartLoadingMainMenu();
		AudioManager.AM.Play("Menu");
		AudioManager.AM.SetVolume("soundsVolume", -20.0f);
		AudioManager.AM.SetVolume("musicVolume", -10.0f);
	}

	public void Update() {
		timeInTitleScreen += Time.deltaTime;
	}

	public void GoToMainMenu() {
        asyncLoad.allowSceneActivation = true;
	}

    public void StartLoadingMainMenu() {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene() {
        asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        asyncLoad.allowSceneActivation = false;
        while (!asyncLoad.isDone) {
            if (asyncLoad.progress >= 0.9f &&
                    (timeInTitleScreen >= maxTimeInTitleScreen ||
                    Input.GetMouseButton(0))) {
                GoToMainMenu();
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }
}
