using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class MapLoader {

	public static MapLayout layout { get; private set; }
	public static bool loading { get; private set; }

	public static readonly string MAP_BASE_SCENE_NAME = "MapBaseScene";
	public static readonly string MAP_RESOURCE_SUBDIR = "Maps";

	public static void LoadLayoutFromExternalFile(string filePath) {
		// TODO
	}

	public static void LoadLayoutFromResource(string mapName) {
		MapLayout ml = Resources.Load(
			MAP_RESOURCE_SUBDIR + "/" + mapName) as MapLayout;
		LoadLayout(ml);
	}

	public static void LoadLayout(MapLayout ml) {
		layout = ml;
		loading = true;
		SceneManager.LoadScene(MAP_BASE_SCENE_NAME);
	}

	public static void LoadEnd() {
		loading = false;
		layout = null;
		GameManager.GM.Begin();
	}

}
