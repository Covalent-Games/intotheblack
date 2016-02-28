using UnityEngine;
using System.Collections;

public class StarMapSceneManager : MonoBehaviour {

	public static StarSystemData SystemSelected;

	private HostilityManager _hostilityManager;

	private void Awake() {

		_hostilityManager = GetComponent<HostilityManager>();

		GameManager.LoadGame();
		if (StarSystemData.StartSystemMapTable.Count == 0) {
			FindObjectOfType<StarClusterGenerator>().GenerateNewStarSystems();
		}
	}

	private void Start() {

		_hostilityManager.UpdateSystemHostility();
	}

	public void JumpToSystem() {

		StarSystemData.StarSystemLoaded = SystemSelected;
		GameStateData.State.TurnCount++;
		GameStateData.State.ExpandHostility = true;
		GameManager.SaveGame();
		Application.LoadLevel("SystemLevel");
	}
}
