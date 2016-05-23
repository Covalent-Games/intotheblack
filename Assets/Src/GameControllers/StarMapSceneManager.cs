using UnityEngine;
using System.Collections;

public class StarMapSceneManager : MonoBehaviour {

	public static StarSystemData SystemSelected;

	public StarClusterGenerator SystemGenerator;

	//private HostilityManager _hostilityManager;
	private GalaxySimulator _galaxySimulator;

	private void Awake() {

		//_hostilityManager = GetComponent<HostilityManager>();
		_galaxySimulator = GetComponent<GalaxySimulator>();

		GameManager.LoadGame();
		if (SystemGenerator.GenerateNewMap || StarSystemData.StartSystemMapTable.Count == 0) {
			SystemGenerator.GenerateNewStarSystems();
		}
	}

	private void Start() {

		_galaxySimulator.UpdateGalaxy();
		SystemGenerator.DrawStarMap();
		Camera.main.GetComponent<StarMapCameraController>().SetCameraToCurrentSystem();
	}

	public void JumpToSystem() {

		StarSystemData.StarSystemLoaded = SystemSelected;
		GameStateData.State.TurnCount++;
		GameStateData.State.ExpandHostility = true;
		GameManager.SaveGame();
		Application.LoadLevel("SystemLevel");
	}

	public void StepSimulation() {

		RedStar[] redStars = GameObject.FindObjectsOfType<RedStar>();
		foreach (RedStar star in redStars) {
			Destroy(star.gameObject);
		}
		_galaxySimulator.UpdateGalaxy();
		Camera.main.GetComponent<StarMapCameraController>().SetCameraToCurrentSystem();
	}
}
