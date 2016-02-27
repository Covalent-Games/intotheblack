using UnityEngine;
using System.Collections;

public class StarMapSceneManager : MonoBehaviour {

	public static StarSystemData SystemSelected;

	private void Awake() {

		GameManager.LoadGame();
		if (StarSystemData.StartSystemMapTable.Count == 0) {
			FindObjectOfType<StarClusterGenerator>().GenerateNewStarSystems();
		}
	}

	public void JumpToSystem() {

		StarSystemData.StarSystemLoaded = SystemSelected;
		GameStateData.State.TurnCount++;
		GameManager.SaveGame();
		Application.LoadLevel("SystemLevel");
	}
}
