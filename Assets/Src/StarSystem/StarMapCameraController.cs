using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class StarMapCameraController : MonoBehaviour {

	public static bool CameraMoving = false;

	private Vector3 _cameraVelocity = Vector3.zero;

	void Start() {

		if (GameStateData.State.PlayerOccupiedSystem != Guid.Empty) {
			StarMapSceneManager.SystemSelected = StarSystemData.StartSystemMapTable[GameStateData.State.PlayerOccupiedSystem];
			SetCameraToCurrentSystem(StarSystemData.StartSystemMapTable[GameStateData.State.PlayerOccupiedSystem]);
		} else {
			MoveCamToHome();
			StarMapSceneManager.SystemSelected = StarSystemData.PlayerHome;
		}
	}

	private void SetCameraToCurrentSystem(StarSystemData system) {

		//TODO: StarSystemLoaded isn't currently persistent across saves. Needs to be saved in GameData still.
		Vector3 newCamPos;

		newCamPos = system.GetPosition();
		FindObjectOfType<StarSystemInfoDisplay>().SetTargettedSystemInfo(system); 
		newCamPos.z = -10;
		Camera.main.transform.position = newCamPos;
	}

	private void MoveCamToHome() {

		Vector3 startCamPos = StarSystemData.EnemyHome.GetPosition();
		Vector3 endCamPos = StarSystemData.PlayerHome.GetPosition();
		startCamPos.z = Camera.main.transform.position.z;
		endCamPos.z = Camera.main.transform.position.z;
		Camera.main.transform.position = startCamPos;
		StartCoroutine(MoveCamToHomeRoutine(endCamPos));
		
	}

	private IEnumerator MoveCamToHomeRoutine(Vector3 destination) {

		CameraMoving = true;

		yield return new WaitForSeconds(2f);

		Vector3 velocity = Vector3.zero;

		while (Vector3.Distance(Camera.main.transform.position, destination) > 0.001f) {
			Camera.main.transform.position = Vector3.SmoothDamp(
				Camera.main.transform.position,
				new Vector3(
					destination.x,
					destination.y,
					Camera.main.transform.position.z),
				ref velocity,
				.5f,
				100f);
			yield return null;
		}
		CameraMoving = false;
		FindObjectOfType<StarSystemInfoDisplay>().SetTargettedSystemInfo(StarSystemData.PlayerHome);
	}

	public void MoveCameraToSelected() {

		StartCoroutine(MoveTowardsSelectedRoutine());
	}

	private IEnumerator MoveTowardsSelectedRoutine() {

		CameraMoving = true;

		Vector3 v = Vector3.zero;
		StarSystemData system = StarMapSceneManager.SystemSelected;
		Vector3 systemPos = system.GetPosition();

		// The 10.000001 is to account for the Z distance. Really we're just checking if the camera is 0.00001 orthagonically.
		while (Vector3.Distance(Camera.main.transform.position, systemPos) > 10.00001f) {
			systemPos = system.GetPosition();
			Camera.main.transform.position = Vector3.SmoothDamp(
				Camera.main.transform.position,
				new Vector3(systemPos.x, systemPos.y,Camera.main.transform.position.z),
				ref v,
				.10f,
				200f);
			yield return null;
		}

		CameraMoving = false;
	}

	private bool MouseInScreen() {

		Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
		if (screenRect.Contains(Input.mousePosition)) {
			return true;
		} else {
			return false;
		}
	}
}
